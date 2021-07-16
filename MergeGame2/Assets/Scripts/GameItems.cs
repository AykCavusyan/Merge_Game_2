using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameItems : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler,IEndDragHandler, IDragHandler, IPointerDownHandler,IPointerUpHandler,ISaveable
{

    public event Action<PointerEventData> OnBeginDragHandler;
    public event Action<PointerEventData> OnDragHandler;
    public event Action<PointerEventData, bool> OnEndDragHandler;

    public event EventHandler<OnQuestItemEventArgs> OnQuestCheckmarkOn;
    public class OnQuestItemEventArgs
    {
        public Item.ItemType itemType;
    }


    public event EventHandler<OnItemDestroyedEventArgs> OnItemDestroyed;
    public class OnItemDestroyedEventArgs : EventArgs
    {
        public GameItems gameItems;
    }

    public event EventHandler<OnMergedEventArgs> OnMerged;
    public class OnMergedEventArgs : EventArgs
    {
        public Vector3 mergePos;
        public Sprite sprite;
        public int itemLevel;
    }

    public event EventHandler<OnItemCollectedEventArgs> OnItemCollected;
    public class OnItemCollectedEventArgs : EventArgs
    {
        public int xpValue;
    }

    public GameObject player;
    private TopPanelID[] topPanels;
    private Canvas canvas;
    private RectTransform rectTransform;

    public List<MergeConditions> mergeCondisitons = new List<MergeConditions>();
    public bool followCursor { get; set; } = true;
    public Vector3 startPosition;
    public bool canDrag { get; set; } = true;
    private bool cr_Running = false;
 
    private bool canReactToClick = false;
    public bool isMoving = false;
    private Image checkMark;

    private Vector2 originalSizeDelta;
    public GameObject initialGameSlot;
    public bool isInventoryItem;

    [SerializeField] private int itemLevel;
    [SerializeField] private Item.ItemGenre itemGenre;
    [SerializeField] public  Item.ItemType itemType; // get set olarak ayarlanmalý !!!!!
    [SerializeField] private bool givesXP;
    [SerializeField] private bool isSpawner = false;
    [SerializeField] private bool isCollectible;
    [SerializeField] private int xpValue;
    [SerializeField] private int itemPanelID;
    [SerializeField] public bool isQuestItem;
    [SerializeField] public bool isRewardPanelItem;

    public void CreateGameItem( int itemLevelIn, Item.ItemGenre itemGenreIn, Item.ItemType itemTypeIn, bool givesXPIn, bool isSpawnerIn, bool isCollectibleIn, int xpValueIn, int itemPanelIDIn, bool isQuestItemIN, bool isRewardPanelItemIN)
    {
        gameObject.AddComponent<Image>().sprite = GetSpriteImage(itemTypeIn);
        itemLevel = itemLevelIn;
        itemGenre = itemGenreIn;
        itemType = itemTypeIn;
        givesXP = givesXPIn;
        isSpawner = isSpawnerIn;
        isCollectible = isCollectibleIn;
        xpValue = xpValueIn;
        itemPanelID = itemPanelIDIn;
        isQuestItem = isQuestItemIN;
        isRewardPanelItem = isRewardPanelItemIN;

        GameObject cornerPanel = Instantiate(Resources.Load<GameObject>("Prefabs/" + "_checkmarkContainer"));
        cornerPanel.transform.SetParent(this.gameObject.transform, false);
        checkMark = cornerPanel.GetComponent<Image>();

        if (isQuestItem)
        {
            checkMark.enabled = true;
        }
    }

     private void Awake()
    {
        rectTransform = gameObject.AddComponent<RectTransform>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        topPanels = GameObject.FindObjectsOfType<TopPanelID>();

        gameObject.AddComponent<SaveableEntitiy>();

    }

    private void OnEnable()
    {
        Init();
        QuestManager.Instance.OnQuestAdded += SetCheckMark;
        QuestManager.Instance.OnQuestRemoved += RemoveCheckMark;
    }

    private void OnDisable()
    {
        QuestManager.Instance.OnQuestAdded -= SetCheckMark;
        QuestManager.Instance.OnQuestRemoved -= RemoveCheckMark;
    }

    private void Start()
    {
        rectTransform.localScale = new Vector3(1, 1, 1);
        originalSizeDelta = rectTransform.sizeDelta;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    CaptureState();
        //}
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    RestoreState(_variablesDict);
        //}
    }

    void Init()
    {
        if (ItemBag.Instance == null)
        {
            Instantiate(player);
        }
    }

    private Sprite GetSpriteImage(Item.ItemType itemTypeIN)
    {
        return ItemAssets.Instance.GetAssetSprite(itemTypeIN);
    }

    public void CheckIfRewardItemIsQuestItem()
    {
        isRewardPanelItem = false;

        foreach (Item.ItemType itemTypeReq in QuestManager.Instance._activeQuestItemsList)
        {
            if(itemType == itemTypeReq)
            {
                isQuestItem = true;
                checkMark.enabled = true;
                
                return;
            }
        }
    }


    public  void SetCheckMark(object sender, QuestManager.OnQuestAddRemoveEventArgs e)
    {

        if(isQuestItem!=true && isRewardPanelItem ==false && itemType == e.itemType)
        {
            isQuestItem = true;
            checkMark.enabled = true;
            //OnQuestCheckmarkOn?.Invoke(this, new OnQuestItemEventArgs { itemType = this.itemType });
        }
    }

    public void RemoveCheckMark(object sender, QuestManager.OnQuestAddRemoveEventArgs e)
    {
        if(isQuestItem!=false && isRewardPanelItem==false && itemType == e.itemType )
        {
            isQuestItem = false;
            checkMark.enabled = false;
        }
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag)
        {
            return;
        }

        rectTransform.SetParent(canvas.transform);
        rectTransform.SetAsLastSibling();
        
        isMoving = true;
        initialGameSlot.GetComponent<GameSlots>().DischargeSlot();
        OnBeginDragHandler?.Invoke(eventData);

        #region
        //var results = new List<RaycastResult>();
        //EventSystem.current.RaycastAll(eventData , results);

        //initialGameSlot = null;

        //foreach (var result in results)
        //{
        //    initialGameSlot = result.gameObject.GetComponent<GameSlots>();        
        //}


        //Debug.Log(eventData);
        //canvasGroup.blocksRaycasts = false;
        //canvasGroup.alpha = .6f;
        #endregion
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag)
        {
            return;
        }

        OnDragHandler?.Invoke(eventData);

        if (followCursor)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag)
        {
            return;
        }

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        GameSlots gameSlot = null;
        ButtonHandler inventoryButton = null;

        foreach (var result in results)
        {
            gameSlot = result.gameObject.GetComponent<GameSlots>();
            inventoryButton = result.gameObject.GetComponent<ButtonHandler>();

            if (gameSlot != null || inventoryButton != null && inventoryButton.buttonlIndex == 1) 
            {

                break;
            }
        }

        if (gameSlot != null )
        {

            if (gameSlot.Accepts(this) && gameSlot.canDrop == true)
            {
                canDrag = false;

                gameSlot.Drop(this);

                OnEndDragHandler?.Invoke(eventData, true);

                canDrag = true; 
                return;
            }

            

            else if (gameSlot.Accepts(this) && gameSlot.canDrop == false)
            {

                if (gameSlot.containedItem != null && AcceptMerge(this, gameSlot))
                {
                    canDrag = false;

                    Vector3 mergePosition = gameSlot.containedItem.transform.position;
                    GameItems mergedItem = Merge(this, gameSlot, this.itemLevel);

                    DestroyItem(gameSlot.containedItem);

                    gameSlot.Drop(mergedItem, mergePosition);

                    OnEndDragHandler?.Invoke(eventData, true);
                    // sprite kýsýmý sadeleþebilir !!!
                    OnMerged?.Invoke(this, new OnMergedEventArgs { mergePos = gameSlot.containedItem.transform.position, sprite = gameSlot.containedItem.GetComponent<Image>().sprite, itemLevel = itemLevel });

                    DestroyItem(this.gameObject);
                    

                    return;
                }
                else
                {
                    canDrag = false;
                    initialGameSlot.GetComponent<GameSlots>().Drop(gameSlot.containedItem.GetComponent<GameItems>());
                    gameSlot.Drop(this);

                    OnEndDragHandler?.Invoke(eventData, true);
                    canDrag = true;
                    return;
                }
            }
        }

        else if (inventoryButton != null && inventoryButton.buttonlIndex == 1 )
        {
            if (PlayerInfo.Instance.emptySlots.Count > 0)
            {
                
                //int targetSlotIDNumber = PlayerInfo.Instance.GetDictionaryAmount() + 1;

                //InventorySlots[] inventorySlots = (InventorySlots[])GameObject.FindObjectsOfType(typeof(InventorySlots));

                foreach (InventorySlots item in PlayerInfo.Instance.emptySlots)
                {
                    if(item.isActive == true)
                    {
                        StopAllCoroutines();
                        rectTransform.sizeDelta = originalSizeDelta;

                        item.Drop(this);
                        canDrag = false;
                        //isInventoryItem = true;                  
                        return;
                    }
                    else
                    {
                        SetItemBack(eventData);
                        return;
                    }
                }

            }
            else
            {
                SetItemBack(eventData);
            }
            return;
        }


        SetItemBack(eventData);
    }


    void SetItemBack(PointerEventData eventData)
    {
        canDrag = false;
        Debug.Log("The Item SLot Container is already full or invalid position");
        initialGameSlot.GetComponent<GameSlots>().Drop(this);
        OnEndDragHandler?.Invoke(eventData, false);
        canDrag = true;
    }

    //            var results2 = new List<RaycastResult>();
    //            EventSystem.current.RaycastAll(eventData, results2);
    //            Debug.Log(results2);

    //            gameItemExisting = null;

    //            foreach (var result in results2)
    //            {
                    
    //                if( result.gameObject.GetInstanceID() == this.gameObject.GetInstanceID())
    //                {
    //                    continue;
    //                }
    //                gameItemExisting = result.gameObject.GetComponent<GameItems>();

    //                if (gameItemExisting != null)
    //                {
    //                    break;
    //                }
    //            }
    //            if (gameItemExisting != null && AcceptMerge(gameItemDragged, gameItemExisting))
    //            {


    //                GameItems mergedItem = Merge(gameItemDragged, gameItemExisting, gameItemDragged.itemLevel);

    //                gameSlot.Drop(mergedItem, gameItemExisting.transform.position);
    //                initialGameSlot.canDrop = true;
    //                OnEndDragHandler?.Invoke(eventData, true);

    //                // sprite kýsýmý sadeleþebilir !!!
    //                OnMerged?.Invoke(this, new OnMergedEventArgs { mergePos = gameItemExisting.transform.position, sprite=gameItemExisting.GetComponent<Image>().sprite, itemLevel = itemLevel });

    //                return;
    //            }
    //            else
    //            {
                   
    //                gameSlot.Drop(gameItemDragged);
    //                initialGameSlot.Drop(gameItemExisting);
                    
    //                return ;
    //            }
                
    //        }
    //    }

    //    Debug.Log("The Item SLot Container is already full or invalid position");
    //    rectTransform.anchoredPosition = startPosition;
    //    OnEndDragHandler?.Invoke(eventData, false);

    //    //Debug.Log(eventData);
    //    //canvasGroup.blocksRaycasts = true;
    //    //canvasGroup.alpha = 1f;
    //}

    public bool AcceptMerge(GameItems gameItemDragged, GameSlots gameSlot )
    {
        if (gameItemDragged.itemType== gameSlot.containedItem.GetComponent<GameItems>().itemType)
        {
            return true;
        }
        else
        {
            return false;
        }        
    }

    public GameItems Merge(GameItems gameItemDragged, GameSlots gameSlot, int itemLevel)
    {
        GameItems mergedItem = ItemBag.Instance.GenerateItem( gameItemDragged.itemGenre, gameItemDragged.itemLevel +1 ).GetComponent<GameItems>();
        mergedItem.transform.localScale = default(Vector3);

        return mergedItem;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Vector2 oldsize = rectTransform.sizeDelta;
        if(cr_Running==false)StartCoroutine(DownsizeItemOnClick());

        if (canReactToClick == true)
        {
            if (isInventoryItem == true)
            {
                GameSlots slotToMove = ItemBag.Instance.FindEmptySlotPosition();
                initialGameSlot.GetComponent<InventorySlots>().DischargeItem(this);
                slotToMove.Drop(this);
                isInventoryItem = false;
                canDrag = true;
                

            }
            else if ( isCollectible == true)
            {
                CollectItem();
                Debug.Log("collected");
            }

        }

        if (isSpawner == true || isCollectible == true || isInventoryItem ==true)
        {
            StartCoroutine(InputListener());
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (canReactToClick == true && isSpawner == true && isInventoryItem == false)
        {
            ItemBag.Instance.AddGeneratedItem(itemGenre, transform.position);
            #region
            //Debug.Log(transform.position + "gameitem transform position");
            //Debug.Log(initialGameSlot.transform.position + "gameslot transform position");
            //Debug.Log(rectTransform.anchoredPosition + "gameitem anchored position");
            //Debug.Log(transform.localPosition + "gameitem localposition");
            #endregion
        }
    }

    IEnumerator InputListener()
    {

        float validClickLimit = .35f;
        float timeElapsed = 0;

        while (timeElapsed < validClickLimit && isMoving == false)
        {
            canReactToClick = true;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canReactToClick = false;
    }

    IEnumerator DownsizeItemOnClick()
    {
        cr_Running = true;

        Vector2 oldSize = rectTransform.sizeDelta;
        Vector2 scaleFactor = new Vector2(.5f, .5f);

        float lerpDuration = .2f;
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            rectTransform.sizeDelta = Vector2.Lerp(oldSize, oldSize * scaleFactor, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.sizeDelta = oldSize * scaleFactor;
        StartCoroutine(UpsizeItem(oldSize, lerpDuration));
    }

    IEnumerator UpsizeItem(Vector2 oldSize, float lerpDuration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < lerpDuration)
        {
            rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, oldSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rectTransform.sizeDelta = oldSize;

        cr_Running = false;
    }

    void CollectItem()
    {
        canDrag = false;

        initialGameSlot.GetComponent<GameSlots>().DischargeSlot();
        OnItemCollected?.Invoke(this, new OnItemCollectedEventArgs { xpValue = this.xpValue });
        MoveItemToTopPanel();
    }

    void MoveItemToTopPanel()
    {
        GameObject panelToMove = ChoosePanelToMove(itemPanelID);
        StartCoroutine(MoveItemToPanelEnum(panelToMove));
    }

    private GameObject ChoosePanelToMove(int itemPanelID)
    {
        for (int i = 0; i < topPanels.Length; i++)
        {
            if (topPanels[i].panelID == itemPanelID)
            {
                return topPanels[i].gameObject;
            }
        }
        return null;
    }

    IEnumerator MoveItemToPanelEnum(GameObject paneltoMove)
    {
        float elapsedTime = 0f;
        float lerpDuration = .5f;

        Vector2 originalPosition = GetComponent<RectTransform>().position;
        Vector2 lerpPosition = paneltoMove.transform.GetChild(0).GetComponent<RectTransform>().position;
        Vector2 oldSize = GetComponent<RectTransform>().sizeDelta;
        Vector2 lerpSizeFactor = new Vector2(.6f, .6f);


        while (elapsedTime < lerpDuration)
        {
            GetComponent<RectTransform>().position = Vector2.Lerp(originalPosition, lerpPosition, elapsedTime / lerpDuration);
            GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(oldSize, oldSize * lerpSizeFactor, elapsedTime / lerpDuration);
            GetComponent<RectTransform>().localEulerAngles += new Vector3(0, 0, 10f);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.position = lerpPosition;
        DestroyItem(this.gameObject);
    }

    public void DestroyItem(GameObject gameItem)
    {

        if (initialGameSlot.GetComponent<GameSlots>() && initialGameSlot.GetComponent<GameSlots>().containedItem == this.gameObject)
        {
            initialGameSlot.GetComponent<GameSlots>().DischargeSlot();
        }
        else if(initialGameSlot.GetComponent<InventorySlots>() && initialGameSlot.GetComponent<InventorySlots>().containedItem == this.gameObject)
        {
            initialGameSlot.GetComponent<InventorySlots>().DischargeItem(this);
        }

        OnItemDestroyed?.Invoke(this , new OnItemDestroyedEventArgs { gameItems =gameItem.GetComponent<GameItems>()} ); 
        Destroy(gameItem);
    }

    public object CaptureState()
    {
        //_variablesDict = new Dictionary<string, object>(); 

        Dictionary<string, object> _variablesDict = new Dictionary<string, object>();

        SizeSaveData data = new SizeSaveData();
        data.sizeDelta = new SerializableVector2(originalSizeDelta);

        _variablesDict.Add("originalSizeDelta", data.sizeDelta); 
        _variablesDict.Add("initialGameSlot", this.initialGameSlot);
        _variablesDict.Add("isInventoryItem", this.isInventoryItem);
        _variablesDict.Add("itemLevel", this.itemLevel);
        _variablesDict.Add("itemGenre", this.itemGenre.ToString());
        _variablesDict.Add("itemType", this.itemType.ToString());
        _variablesDict.Add("givesXP", this.givesXP);
        _variablesDict.Add("isSpawner", this.isSpawner);
        _variablesDict.Add("isCollectible", this.isCollectible);
        _variablesDict.Add("xpValue", this.xpValue);
        _variablesDict.Add("itemPanelID", this.itemPanelID);
        _variablesDict.Add("isQuestItem", this.isQuestItem);
        _variablesDict.Add("isRewardPanelItem", this.isRewardPanelItem);

        Debug.Log(_variablesDict.Count + "save system of gameitems working -- dict created");
        return _variablesDict;
    }

    [System.Serializable]
    struct SizeSaveData
    {
        public SerializableVector2 sizeDelta;
    }

    public void RestoreState(object state)
    {
        Debug.Log("load system of gameitems working ");

        Dictionary<string, object> _variablesDictIN = (Dictionary<string, object>)state;

        //string parsedItemGenreValue = (string)_variablesDict["itemGenre"];
        //string parsedItemTypeValue = (string)_variablesDict["itemType"];

        SizeSaveData data = (SizeSaveData)_variablesDictIN["originalSizeDelta"];
        originalSizeDelta = data.sizeDelta.ToVector2(); 
        initialGameSlot =   (GameObject)_variablesDictIN["initialGameSlot"];
        isInventoryItem =   (bool)_variablesDictIN["isInventoryItem"];
        itemLevel =         (int)_variablesDictIN["itemLevel"];
        itemGenre =         (Item.ItemGenre)Enum.Parse(typeof(Item.ItemGenre), (string)_variablesDictIN["itemGenre"]); 
        itemType =          (Item.ItemType)Enum.Parse(typeof(Item.ItemType), (string)_variablesDictIN["itemType"]); 
        givesXP =           (bool)_variablesDictIN["givesXP"];
        isSpawner =         (bool)_variablesDictIN["isSpawner"];
        isCollectible =     (bool)_variablesDictIN["isCollectible"];
        xpValue =           (int)_variablesDictIN["xpValue"];
        itemPanelID =       (int)_variablesDictIN["itemPanelID"];
        isQuestItem =       (bool)_variablesDictIN["isQuestItem"]; ;
        isRewardPanelItem = (bool)_variablesDictIN["isRewardPanelItem"]; ;

        CreateGameItem(itemLevel, itemGenre, itemType, givesXP, isSpawner, isCollectible, xpValue, itemPanelID, isQuestItem, isRewardPanelItem);
    }

}

