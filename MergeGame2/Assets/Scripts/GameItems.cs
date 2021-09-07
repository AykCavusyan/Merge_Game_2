using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameItems : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler,IEndDragHandler, IDragHandler, IPointerDownHandler,IPointerUpHandler //,ISaveable
{

    private bool isLongPress = false;

    public event Action<PointerEventData> OnBeginDragHandler;
    public event Action<PointerEventData> OnDragHandler;
    public event Action<PointerEventData, bool,bool> OnEndDragHandler;

    public event EventHandler<OnPossibleDropEffectsEventArgs> OnPossibleDropEffects;
    public class OnPossibleDropEffectsEventArgs
    {
        public Vector3 effectLocation;
        public VisualEffectsCanDrop.EffectType effectType;
        public bool canPlay;
    }

    public event EventHandler<OnQuestItemEventArgs> OnQuestCheckmarkOn;
    public class OnQuestItemEventArgs
    {
        public Item.ItemType itemType;
    }

    public event EventHandler<OnGameItemClickedEventArgs> OnGameItemClicked;
    public class OnGameItemClickedEventArgs
    {
        public Item.ItemType itemType;
        public int goldValue;       
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
        public GameItems mergedItem;
    }

    public event EventHandler<OnItemCollectedEventArgs> OnItemCollected;
    public class OnItemCollectedEventArgs : EventArgs
    {
        public int xpValue;
        public int goldValue;
        public int itemLevel;
        public int itemPanelID;
        public Vector2 position;
        public Item.ItemType ItemType;
    }

    public GameObject player;
    private GameObject panel_GameItems_Temporary;
    private TopPanelID[] topPanels;
    private Canvas canvas;
    private RectTransform rectTransform;
    private Panel_PowerUpItems powerUpItemsPanelMain;

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
    public bool isInsidePowerUpPanel;

    [SerializeField] private int itemLevel;
    [SerializeField] public Item.ItemGenre itemGenre;// get set olarak ayarlanmalý !!!!!
    [SerializeField] public  Item.ItemType itemType; // get set olarak ayarlanmalý !!!!!
    [SerializeField] private bool givesXP;
    [SerializeField] private bool isSpawner = false;
    [SerializeField] private bool isCollectible;
    [SerializeField] public int goldValue { get; private set; } 
    [SerializeField] private int xpValue;
    [SerializeField] private int itemPanelID;
    [SerializeField] public bool isQuestItem;
    [SerializeField] public bool isRewardPanelItem;
    [SerializeField] public bool isPowerUpItem;

    public void CreateGameItem( int itemLevelIn, Item.ItemGenre itemGenreIn, Item.ItemType itemTypeIn, bool givesXPIn, bool isSpawnerIn, bool isCollectibleIn, int xpValueIn, int goldValueIN, int itemPanelIDIn, bool isQuestItemIN, bool isRewardPanelItemIN,bool isPowerUpItemIN)
    {
        gameObject.AddComponent<Image>().sprite = GetSpriteImage(itemTypeIn);
        itemLevel = itemLevelIn;
        itemGenre = itemGenreIn;
        itemType = itemTypeIn;
        givesXP = givesXPIn;
        isSpawner = isSpawnerIn;
        isCollectible = isCollectibleIn;
        xpValue = xpValueIn;
        goldValue = goldValueIN;
        itemPanelID = itemPanelIDIn;
        isQuestItem = isQuestItemIN;
        isRewardPanelItem = isRewardPanelItemIN;
        isPowerUpItem = isPowerUpItemIN;

        GameObject cornerPanel = Instantiate(Resources.Load<GameObject>("Prefabs/" + "_checkmarkContainer"));
        cornerPanel.transform.SetParent(this.gameObject.transform, false);
        checkMark = cornerPanel.GetComponent<Image>();

        if (isQuestItem)
        {
            checkMark.enabled = true;
        }

        gameObject.layer = 5;
    }

     private void Awake()
    {
        rectTransform = gameObject.AddComponent<RectTransform>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        panel_GameItems_Temporary = GameObject.Find("Panel_GameItems_Temporary");
        topPanels = FindObjectsOfType<TopPanelID>();
        powerUpItemsPanelMain = FindObjectOfType<Panel_PowerUpItems>();
    }

    private void OnEnable()
    {
        Init();
        QuestManager.Instance.OnQuestAdded += SetCheckMark;
        QuestManager.Instance.OnItemIsNotQuestItemAnymore += RemoveCheckMark;
    }

    private void OnDisable()
    {
        QuestManager.Instance.OnQuestAdded -= SetCheckMark;
        QuestManager.Instance.OnItemIsNotQuestItemAnymore -= RemoveCheckMark;
    }

    private void Start()
    {
        rectTransform.localScale = new Vector3(1, 1, 1);
        originalSizeDelta = GameObject.FindObjectOfType<GameSlots>().containedItemSize;
    }

    private void Update()
    {
       // if(!isRewardPanelItem);

        if (isMoving)
        {
            var results = new List<RaycastResult>();
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            EventSystem.current.RaycastAll(eventData, results);

            GameSlots gameSlot = null;
            ButtonHandler inventoryButton = null;
            Panel_PowerUpItems powerUpItemsPanel = null;

            

            foreach(var result in results)
            {
                gameSlot = result.gameObject.GetComponent<GameSlots>();
                inventoryButton = result.gameObject.GetComponent<ButtonHandler>();
                powerUpItemsPanel = result.gameObject.GetComponent<Panel_PowerUpItems>();

                if(gameSlot != null || inventoryButton!=null || powerUpItemsPanel != null)
                {
                    break;
                }
            }

            if (gameSlot != null && gameSlot.containedItem != null &&gameSlot.containedItem.GetComponent<GameItems>().itemType == this.itemType)
            {
                StartCoroutine(ActivateOnpossibleDropEffects(gameSlot.gameObject));
            }
            else if (inventoryButton != null && inventoryButton.buttonIndex == 1)
            {
                StartCoroutine(ActivateOnpossibleDropEffects(inventoryButton.gameObject));
            }
            else if (powerUpItemsPanel != null)
            {
                if(cr_Running == false && rectTransform.sizeDelta == originalSizeDelta) 
                {
                    Vector2 modifiedSizeDelta = new Vector2(powerUpItemsPanel.slotWidth, powerUpItemsPanel.slotWidth);
                    StartCoroutine(DownSizeItemOnPanelHoverEnum(modifiedSizeDelta));
                }
                if (isPowerUpItem)
                {
                    powerUpItemsPanel.ReceivePositionAndEvaluate(this);
                }
            }
            if (powerUpItemsPanel == null)
            {
                if (cr_Running == false && rectTransform.sizeDelta != originalSizeDelta) StartCoroutine(UpSizeItemOnPanelHoverEnum());

                if(isPowerUpItem && isInsidePowerUpPanel)
                {
                    powerUpItemsPanelMain.ResetSlotPositions(this);
                }
            }
            if (gameSlot == null || gameSlot.containedItem == null || gameSlot.containedItem.GetComponent<GameItems>().itemType != itemType)
            {
                OnPossibleDropEffects?.Invoke(this, new OnPossibleDropEffectsEventArgs { canPlay = false, effectType = VisualEffectsCanDrop.EffectType.overGameItem });
            }
            if (inventoryButton == null || inventoryButton.buttonIndex != 1)
            {
                OnPossibleDropEffects?.Invoke(this, new OnPossibleDropEffectsEventArgs { canPlay = false, effectType = VisualEffectsCanDrop.EffectType.overInvetory });
            }

        }
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


    public void SetCheckMark(object sender, QuestManager.OnQuestAddRemoveEventArgs e)
    {

        if(isQuestItem!=true && isRewardPanelItem ==false && itemType == e.itemType)
        {
            UnityEngine.Debug.Log(sender);
            isQuestItem = true;
            checkMark.enabled = true;
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


        isMoving = true;
        GetComponent<Image>().raycastTarget = false;

        rectTransform.SetParent(panel_GameItems_Temporary.transform);
        rectTransform.SetAsLastSibling();

        if (initialGameSlot.GetComponent<GameSlots>()) 
        {
            initialGameSlot.GetComponent<GameSlots>().DischargeSlot();
        }
        else if (initialGameSlot.GetComponent<PowerUpItem_Slots>())
        {
            initialGameSlot.GetComponent<PowerUpItem_Slots>().DischargeItem();

        }

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

        if (followCursor)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
        OnDragHandler?.Invoke(eventData);
        /*
        if (isPowerUpItem)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            //Panel_PowerUpItems powerUpItemsPanel = null;
            PowerUpItem_Slots powerUpSlots = null;
            
            foreach (var result in results)
            {
                //powerUpItemsPanel = result.gameObject.GetComponent<Panel_PowerUpItems>();
                powerUpSlots = result.gameObject.GetComponent<PowerUpItem_Slots>();
                if (powerUpSlots != null) break;

            }

            if (powerUpSlots != null)
            {
                bool wasOnTheSlot = initialGameSlot.Equals(powerUpSlots);
                bool isPointerDirectionPositive = eventData.delta.x > 0;
                Vector3 previousPositionLocal = transform.TransformPoint(eventData.delta.x/canvas.scaleFactor, eventData.delta.y/canvas.scaleFactor, 0);

                powerUpItemsPanel.GetComponent<Panel_PowerUpItems>().ReceiveItemRayAndEvaluate(this, powerUpSlots, wasOnTheSlot, isPointerDirectionPositive, previousPositionLocal);
            }
            else if(powerUpSlots == null && powerUpItemsPanel.GetComponent<Panel_PowerUpItems>().isTemporarlyModified)
            {
                powerUpItemsPanel.GetComponent<Panel_PowerUpItems>().ResetSlotPositions();
            }
        }
        */
    }

     
    IEnumerator DownSizeItemOnPanelHoverEnum(Vector2 modifiedSizeDeltaIN)
    {
        cr_Running = true;

        float lerpDuration = .10f;
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            rectTransform.sizeDelta = Vector2.Lerp(originalSizeDelta, modifiedSizeDeltaIN, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rectTransform.sizeDelta = modifiedSizeDeltaIN;
        cr_Running = false;
    }

    IEnumerator UpSizeItemOnPanelHoverEnum()
    {
        cr_Running = true;

        //StopCoroutine("DownSizeItemOnPanelHoverEnum");

        float lerpDuration = .1f;
        float elapsedTime = 0f;
        Vector2 currentItemSize = rectTransform.sizeDelta;

        while (elapsedTime < lerpDuration)
        {
            rectTransform.sizeDelta = Vector2.Lerp(currentItemSize, originalSizeDelta, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rectTransform.sizeDelta = originalSizeDelta;

        cr_Running = false;
    }

    IEnumerator ActivateOnpossibleDropEffects(GameObject focusedItemIN)
    {
        float elapsedTime = 0f;
        float requiredTime = .45f;

        while (elapsedTime < requiredTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        var results = new List<RaycastResult>();
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        EventSystem.current.RaycastAll(eventData, results);

        GameSlots gameSlot = null;
        ButtonHandler inventoryButton = null;

        foreach (var result in results)
        {
            gameSlot = result.gameObject.GetComponent<GameSlots>();
            inventoryButton = result.gameObject.GetComponent<ButtonHandler>();

            if (gameSlot != null || inventoryButton != null)
            {
                break;
            }
        }

        if (gameSlot != null && gameSlot.gameObject == focusedItemIN)
            OnPossibleDropEffects?.Invoke(this, new OnPossibleDropEffectsEventArgs { canPlay = true, effectLocation = gameSlot.containedItem.transform.position, effectType = VisualEffectsCanDrop.EffectType.overGameItem });
        else if (inventoryButton!= null && inventoryButton.gameObject ==focusedItemIN)
            OnPossibleDropEffects?.Invoke(this, new OnPossibleDropEffectsEventArgs { canPlay = true, effectLocation = inventoryButton.transform.position , effectType = VisualEffectsCanDrop.EffectType.overInvetory});
        else
        {
            OnPossibleDropEffects?.Invoke(this, new OnPossibleDropEffectsEventArgs { canPlay = false, effectType = VisualEffectsCanDrop.EffectType.overGameItem });
            OnPossibleDropEffects?.Invoke(this, new OnPossibleDropEffectsEventArgs { canPlay = false, effectType = VisualEffectsCanDrop.EffectType.overInvetory });
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag)
        {
            return;
        }

        GetComponent<Image>().raycastTarget = true;

        StopAllCoroutines();
        cr_Running = false;
        isMoving = false;
        OnEndDragHandler?.Invoke(eventData, false, false);

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        GameSlots gameSlot = null;
        ButtonHandler inventoryButton = null;
        Panel_PowerUpItems powerUpItemsPanel = null;
        

        foreach (var result in results)
        {
            gameSlot = result.gameObject.GetComponent<GameSlots>();
            inventoryButton = result.gameObject.GetComponent<ButtonHandler>();
            powerUpItemsPanel = result.gameObject.GetComponent<Panel_PowerUpItems>();

            if (gameSlot != null || inventoryButton != null && inventoryButton.buttonIndex == 1 || powerUpItemsPanel!=null)
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
                Vector3 pointerDropPos = new Vector3(eventData.position.x, eventData.position.y, 0);

                OnEndDragHandler?.Invoke(eventData, true,false);

                canDrag = true; 
                return;
            }

            

            else if (gameSlot.Accepts(this) && gameSlot.canDrop == false)
            {

                if (gameSlot.containedItem != null && AcceptMerge(this, gameSlot))
                {
                    GameObject containedItem = gameSlot.containedItem;
                    canDrag = false;

                    GameItems mergedItem = Merge(this, gameSlot, this.itemLevel);
                    mergedItem.transform.position = containedItem.transform.position;

                    DestroyItem(gameSlot.containedItem);

                    gameSlot.Drop(mergedItem); 

                    OnEndDragHandler?.Invoke(eventData, true,false);
                    OnMerged?.Invoke(this, new OnMergedEventArgs { mergePos = containedItem.transform.position, sprite = containedItem.GetComponent<Image>().sprite, itemLevel = itemLevel ,mergedItem =mergedItem});

                    DestroyItem(this.gameObject);
                    

                    return;
                }
                else
                {
                    canDrag = false;
                    initialGameSlot.GetComponent<GameSlots>().Drop(gameSlot.containedItem.GetComponent<GameItems>());
                    gameSlot.Drop(this);

                    OnEndDragHandler?.Invoke(eventData, true,false);
                    canDrag = true;
                    return;
                }
            }
        }

        else if (inventoryButton != null && inventoryButton.buttonIndex == 1 )
        {
            if (PlayerInfo.Instance.emptySlots.Count > 0)
            {
                
                foreach (InventorySlots invSlot in PlayerInfo.Instance.emptySlots)
                {
                    if(invSlot.isActive == true)
                    {
                        StopAllCoroutines();
                        rectTransform.sizeDelta = originalSizeDelta;

                        invSlot.Drop(this);
                        canDrag = false;
                        cr_Running = false;
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

        else if (powerUpItemsPanel != null && isPowerUpItem)
        {

            foreach (GameObject slot in powerUpItemsPanel.slotList)
            {

                if (slot.GetComponent<PowerUpItem_Slots>().isFree)
                {
                    canDrag = false;

                    StopAllCoroutines();
                    rectTransform.sizeDelta = originalSizeDelta; // buna gerek var mý zaten küçültüyoruz bu paneliin üzerindeyken ??
                    //powerUpItemsPanel.FindSlotsToMoveAndDrop(this);

                    slot.GetComponent<PowerUpItem_Slots>().Drop(this);
                    cr_Running = false;

                    canDrag = true;

                    OnEndDragHandler?.Invoke(eventData, false, true);

                    return;
                }          
            }
        }

        SetItemBack(eventData);
    }


    void SetItemBack(PointerEventData eventData)
    {
        canDrag = false; UnityEngine.Debug.Log("The Item SLot Container is already full or invalid position");

        if (initialGameSlot.GetComponent<GameSlots>())
        {
            initialGameSlot.GetComponent<GameSlots>().Drop(this);
            OnEndDragHandler?.Invoke(eventData, false, false);
        }

        else if (initialGameSlot.GetComponent<PowerUpItem_Slots>())
        {
            foreach (GameObject slot in powerUpItemsPanelMain.slotList)
            {
                if (slot.GetComponent<PowerUpItem_Slots>().isFree)
                {
                    canDrag = false;

                    StopAllCoroutines();
                    rectTransform.sizeDelta = originalSizeDelta; // buna gerek var mý zaten küçültüyoruz bu paneliin üzerindeyken ??
                    //powerUpItemsPanel.FindSlotsToMoveAndDrop(this);

                    slot.GetComponent<PowerUpItem_Slots>().Drop(this);
                    cr_Running = false;

                    canDrag = true;

                    OnEndDragHandler?.Invoke(eventData, false, true);

                    return;
                }
            }
        }
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

        if (mergedItem.givesXP)
        {
            GameObject newItemXpStar = ItemBag.Instance.GenerateItem(Item.ItemGenre.Star);
            ItemBag.Instance.AddGeneratedItem(newItemXpStar, this.transform.position);
        }

        return mergedItem;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!isRewardPanelItem && !isInventoryItem )
        OnGameItemClicked?.Invoke(this, new OnGameItemClickedEventArgs { itemType = itemType , goldValue = goldValue});

        if(cr_Running==false)StartCoroutine(DownsizeItemOnClick());

        if (canReactToClick == true)
        {
            if (isInventoryItem == true)
            {       
                StopAllCoroutines();
                cr_Running = false;

                InventorySlots inventorySlot = this.initialGameSlot.GetComponent<InventorySlots>();

                GameSlots slotToMove = ItemBag.Instance.FindEmptySlotPosition();
                slotToMove.Drop(this); // bu aþaðýdaki boþluktaydý sýkýntý çýkartýrsa diye comment 
                inventorySlot.GetComponent<InventorySlots>().DischargeItem(this);
                
                isInventoryItem = false;
                canDrag = true;

            }
            else if ( isCollectible == true)
            {
                CollectItem();
            }
            else if (isSpawner ==true && isInventoryItem==false && isInsidePowerUpPanel == false)
            {
                GameObject newGameItem = ItemBag.Instance.GenerateItem(itemGenre);
                ItemBag.Instance.AddGeneratedItem(newGameItem, this.transform.position);
            }

        }

        if (isSpawner == true || isCollectible == true || isInventoryItem ==true)
        {
            StartCoroutine(InputListener());
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        //if (canReactToClick == true && isSpawner == true && isInventoryItem == false)
        //{
        //    GameObject newGameItem = ItemBag.Instance.GenerateItem(itemGenre);
        //    ItemBag.Instance.AddGeneratedItem(newGameItem, this.transform.position);           
        //}
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
        Vector2 scaleFactor = new Vector2(.87f, .87f);

        float lerpDuration = .1f;
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



    public void CollectItem()
    {
        canDrag = false;
        initialGameSlot.GetComponent<GameSlots>().DischargeSlot();
        OnItemCollected?.Invoke(this, new OnItemCollectedEventArgs {  itemLevel=this.itemLevel , xpValue = this.xpValue , goldValue=this.goldValue, itemPanelID = itemPanelID , position = GetComponent<RectTransform>().position, ItemType= itemType});
        DestroyItem(this.gameObject);
    }

    //void MoveItemToTopPanel()
    //{
    //    GameObject panelToMove = ChoosePanelToMove(itemPanelID);
    //    StartCoroutine(MoveItemToPanelEnum(panelToMove));
    //}

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

    //IEnumerator MoveItemToPanelEnum(GameObject paneltoMove)
    //{
    //    float elapsedTime = 0f;
    //    float lerpDuration = .5f;

    //    Vector2 originalPosition = GetComponent<RectTransform>().position;
    //    Vector2 lerpPosition = paneltoMove.transform.GetChild(0).GetComponent<RectTransform>().position;
    //    Vector2 oldSize = GetComponent<RectTransform>().sizeDelta;
    //    Vector2 lerpSizeFactor = new Vector2(.6f, .6f);


    //    while (elapsedTime < lerpDuration)
    //    {
    //        GetComponent<RectTransform>().position = Vector2.Lerp(originalPosition, lerpPosition, elapsedTime / lerpDuration);
    //        GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(oldSize, oldSize * lerpSizeFactor, elapsedTime / lerpDuration);
    //        GetComponent<RectTransform>().localEulerAngles += new Vector3(0, 0, 10f);
    //        elapsedTime += Time.deltaTime;

    //        yield return null;
    //    }
    //    transform.position = lerpPosition;
    //    DestroyItem(this.gameObject);
    //}

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
        Dictionary<string, object> _variablesDict = new Dictionary<string, object>();

            SerializableVector2 size = new SerializableVector2(originalSizeDelta);

            _variablesDict.Add("originalSizeDelta", size);
            _variablesDict.Add("isInventoryItem", this.isInventoryItem);
            _variablesDict.Add("itemLevel", this.itemLevel);
            _variablesDict.Add("itemGenre", this.itemGenre.ToString());
            _variablesDict.Add("itemType", this.itemType.ToString());
            _variablesDict.Add("givesXP", this.givesXP);
            _variablesDict.Add("isSpawner", this.isSpawner);
            _variablesDict.Add("isCollectible", this.isCollectible);
            _variablesDict.Add("xpValue", this.xpValue);
            _variablesDict.Add("goldValue",this.goldValue);
            _variablesDict.Add("itemPanelID", this.itemPanelID);
            _variablesDict.Add("isQuestItem", this.isQuestItem);
            _variablesDict.Add("isPowerUpItem", this.isPowerUpItem);
            //_variablesDict.Add("isRewardPanelItem", this.isRewardPanelItem);

        return _variablesDict;
    }


    public void RestoreState(object state)
    {
        Dictionary<string, object> _variablesDictIN = (Dictionary<string, object>)state;

            SerializableVector2 size = (SerializableVector2)_variablesDictIN["originalSizeDelta"];

            originalSizeDelta = size.ToVector2();
            isInventoryItem = (bool)_variablesDictIN["isInventoryItem"];
            itemLevel = (int)_variablesDictIN["itemLevel"];
            itemGenre = (Item.ItemGenre)Enum.Parse(typeof(Item.ItemGenre), (string)_variablesDictIN["itemGenre"]);
            itemType = (Item.ItemType)Enum.Parse(typeof(Item.ItemType), (string)_variablesDictIN["itemType"]);
            givesXP = (bool)_variablesDictIN["givesXP"];
            isSpawner = (bool)_variablesDictIN["isSpawner"];
            isCollectible = (bool)_variablesDictIN["isCollectible"];
            xpValue = (int)_variablesDictIN["xpValue"];
            goldValue = (int)_variablesDictIN["goldValue"];
            itemPanelID = (int)_variablesDictIN["itemPanelID"];
            isQuestItem = (bool)_variablesDictIN["isQuestItem"]; ;
            isPowerUpItem = (bool)_variablesDictIN["isPowerUpItem"];
            //isRewardPanelItem = (bool)_variablesDictIN["isRewardPanelItem"]; ;

            CreateGameItem(itemLevel, itemGenre, itemType, givesXP, isSpawner, isCollectible, xpValue, goldValue, itemPanelID, isQuestItem, isRewardPanelItem, isPowerUpItem);

    }

}

