using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameItems : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler,IEndDragHandler, IDragHandler, IPointerDownHandler,IPointerUpHandler
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

    private TopPanelID[] topPanels;
    private Canvas canvas;

    public List<MergeConditions> mergeCondisitons = new List<MergeConditions>();

    public bool followCursor { get; set; } = true;
    public Vector3 startPosition;
    public bool canDrag { get; set; } = true;
    public GameObject initialGameSlot;

    private bool cr_Running = false;

    [SerializeField] private bool isSpawner = false;
    private bool canReactToClick = false;
    public bool isMoving = false;

    public bool isInventoryItem;
     
    private Image checkMark;
    
    
    public GameObject player;


    private RectTransform rectTransform;

    [SerializeField] private int itemLevel;
    [SerializeField] private Item.ItemGenre itemGenre;
    [SerializeField] public  Item.ItemType itemType; // get set olarak ayarlanmalý !!!!!
    [SerializeField] private bool givesXP;
    [SerializeField] private bool isCollectible;
    [SerializeField] private int xpValue;
    [SerializeField] private int itemPanelID;
    [SerializeField] public bool isQuestItem;
    [SerializeField] public bool isRewardPanelItem;

    public void CreateGameItem( int itemLevelIn, Item.ItemGenre itemGenreIn, Item.ItemType itemTypeIn, bool givesXPIn, bool isSpawnerIn, bool isCollectibleIn, int xpValueIn, int itemPanelIDIn, bool isQuestItemIN, bool isRewardPanelItemIN)
    {
        
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
        rectTransform = GetComponent<RectTransform>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        topPanels = GameObject.FindObjectsOfType<TopPanelID>();
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
    }

    void Init()
    {
        if (ItemBag.Instance == null)
        {
            Instantiate(player);
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

        //DestroyItem(gameItemDragged.gameObject);
        //DestroyItem(gameSlot.containedItem);

        //Destroy(gameItemDragged.gameObject);
        //Destroy(gameSlot.containedItem.gameObject);

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
                initialGameSlot.GetComponent<InventorySlots>().DischargeItem();
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
        Vector2 scaleFactor = new Vector2(.8f, .8f);

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
        Debug.Log("destroyed");

        if(initialGameSlot.GetComponent<GameSlots>().containedItem == this.gameObject)
        {
            initialGameSlot.GetComponent<GameSlots>().DischargeSlot();
        }

        OnItemDestroyed?.Invoke(this , new OnItemDestroyedEventArgs { gameItems =gameItem.GetComponent<GameItems>()} ); 
        Destroy(gameItem);
    }
}
