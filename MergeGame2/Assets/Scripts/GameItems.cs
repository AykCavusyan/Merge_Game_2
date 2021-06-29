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

    public event Action<GameItems> OnItemDestroyed;
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
    public GameSlots initialGameSlot;

    private bool cr_Running = false;

    [SerializeField] private bool isSpawner = false;
    private bool canReactToClicl = false;
    public bool isMoving = false;
    public GameObject player;


    private RectTransform rectTransform;

    [SerializeField] private int itemLevel;
    [SerializeField] private Item.ItemGenre itemGenre;
    [SerializeField] private Item.ItemType itemType;
    [SerializeField] private bool givesXP;
    [SerializeField] private bool isCollectible;
    [SerializeField] private int xpValue;
    [SerializeField] private int itemPanelID;

    public void CreateGameItem( int itemLevelIn, Item.ItemGenre itemGenreIn, Item.ItemType itemTypeIn, bool givesXPIn, bool isSpawnerIn, bool isCollectibleIn, int xpValueIn, int itemPanelIDIn)
    {
        
        itemLevel = itemLevelIn;
        itemGenre = itemGenreIn;
        itemType = itemTypeIn;
        givesXP = givesXPIn;
        isSpawner = isSpawnerIn;
        isCollectible = isCollectibleIn;
        xpValue = xpValueIn;
        itemPanelID = itemPanelIDIn;
    }

     private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        //itemBag = GameObject.Find("Button").GetComponent<ItemBag>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        //panel_Gameslots = GameObject.Find("Panel_GameSlots");
        topPanels = GameObject.FindObjectsOfType<TopPanelID>();
    }

    private void OnEnable()
    {
      Init();
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
        initialGameSlot.DischargeSlot();
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
        //gameItemExisting = null;

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
                
                //initialGameSlot.DischargeSlot();
                gameSlot.Drop(this);

                //initialGameSlot.canDrop = true;

                OnEndDragHandler?.Invoke(eventData, true);

                return;
            }

            

            else if (gameSlot.Accepts(this) && gameSlot.canDrop == false)
            {

                if (gameSlot.containedItem != null && AcceptMerge(this, gameSlot))
                {
                    Vector3 mergePosition = gameSlot.containedItem.transform.position;
                    GameItems mergedItem = Merge(this, gameSlot, this.itemLevel);
                    gameSlot.Drop(mergedItem, mergePosition);
                    
                    //initialGameSlot.DischargeSlot();

                    OnEndDragHandler?.Invoke(eventData, true);

                    // sprite kýsýmý sadeleþebilir !!!
                    OnMerged?.Invoke(this, new OnMergedEventArgs { mergePos = gameSlot.containedItem.transform.position, sprite = gameSlot.containedItem.GetComponent<Image>().sprite, itemLevel = itemLevel });

                    return;
                }
                else
                {
                    initialGameSlot.Drop(gameSlot.containedItem.GetComponent<GameItems>());
                    gameSlot.Drop(this);
                    

                    OnEndDragHandler?.Invoke(eventData, true);


                    return;
                }
            }
        }

        else if (inventoryButton != null && inventoryButton.buttonlIndex == 1 )
        {

            if (PlayerInfo.Instance.GetRemainingInventorySLotAmount() > 0)
            {
                canDrag = false;
                
                int targetSlotIDNumber = PlayerInfo.Instance.GetDictionaryAmount() + 1;

                InventorySlots[] inventorySlots = (InventorySlots[])GameObject.FindObjectsOfType(typeof(InventorySlots));

                foreach (InventorySlots item in inventorySlots)
                {
                    if (item.slotIDNumber == targetSlotIDNumber)
                    {
                        item.Drop(this);
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
        Debug.Log("The Item SLot Container is already full or invalid position");
        initialGameSlot.Drop(this);
        OnEndDragHandler?.Invoke(eventData, false);
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

        DestroyItem(gameItemDragged.gameObject);
        DestroyItem(gameSlot.containedItem);

        //Destroy(gameItemDragged.gameObject);
        //Destroy(gameSlot.containedItem.gameObject);

        return mergedItem;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Vector2 oldsize = rectTransform.sizeDelta;
        if(cr_Running==false)StartCoroutine(DownsizeItemOnClick());

        if (canReactToClicl == true)
        {
            Debug.Log("collect on double click working");
            CollectItem();
        }

        if (isSpawner == true || isCollectible == true)
        {
            StartCoroutine(InputListener());
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (canReactToClicl == true && isSpawner == true)
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
            canReactToClicl = true;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canReactToClicl = false;
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
        //move the item up to the relevant position
        Debug.Log("trying to collevt");
        initialGameSlot.DischargeSlot();
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
        Debug.Log("move item tp panel worked");
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

    void DestroyItem(GameObject gameItem)
    {
        //OnItemDestroyed?.Invoke(this); daha sonra yapýlacak, maksat master listerer ölmüþ itemleri dinlemeye çalýþmasýn
        Destroy(gameItem);
    }
}
