using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Panel_PowerUpItems : MonoBehaviour
{
    public bool isTemporarlyModified; // bunun çýkmasý gerekebilir belki de gereksiz ?

    private RectTransform panelBackground;
    private GameObject innerPanelContainer;
    private RectTransform thisPanel;
    private int slotCount;
    public float slotWidth { get; private set; }
    private float slotWidthAdjustedToParent;
    public List<GameObject> slotList { get; private set; } = new List<GameObject>();
    //private GameObject backupInitialGameSLot = null;
    public bool isFull { get; private set; } = false;
    private Text existingItemAmountText;
    private Text totalItemAmountText;
    List<(GameItems, Vector2,Vector2)> existingItemsList = new List<(GameItems, Vector2, Vector2)>();

    private void Awake()
    {
        thisPanel = GetComponent<RectTransform>();
        panelBackground = transform.parent.GetChild(this.transform.GetSiblingIndex() - 1).GetComponent<RectTransform>();
        innerPanelContainer = transform.GetChild(0).GetChild(0).gameObject;
        existingItemAmountText = transform.parent.GetChild(this.transform.GetSiblingIndex() + 1).GetChild(0).GetComponent<Text>();
        totalItemAmountText = transform.parent.GetChild(this.transform.GetSiblingIndex() + 1).GetChild(2).GetComponent<Text>();
    }

    private void Start()
    {
        GetSlotCount();
        InstantiateSlots();
        SetTotalItemAmount();
        SetExistingItemAmount();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) AddExtraSlots();
    }

    private void OnEnable()
    {
        ItemBag.Instance.OnGameItemCreated += StartListeningToGameItem;
        MasterEventListener.Instance.OnDestroyedMasterEvent += StoplisteningToGameItem;
    }

    private void OnDisable()
    {
        ItemBag.Instance.OnGameItemCreated -= StartListeningToGameItem;
        MasterEventListener.Instance.OnDestroyedMasterEvent -= StoplisteningToGameItem;

        GameItems[] gameItems = FindObjectsOfType<GameItems>();
        if(gameItems.Length > 0)
        {
            foreach (GameItems gameItem in gameItems)
            {
                gameItem.OnEndDragHandler -= EndDragMethods;
            }
        }
    }

    private void StartListeningToGameItem(object sender, ItemBag.OnGameItemCreatedEventArgs e)
    {
        e.gameItem.OnEndDragHandler += EndDragMethods;
    }

    private void StoplisteningToGameItem(object sender, GameItems.OnItemDestroyedEventArgs e)
    {
        e.gameItems.OnEndDragHandler -= EndDragMethods;
    }

    void GetSlotCount()
    {
        slotCount = 9; // bu daha sonra playerinfordan yapýlacak
    }

    private void SetTotalItemAmount()
    {
        totalItemAmountText.text = slotCount.ToString();
    }

    private  void SetExistingItemAmount()
    {
        int itemAmount = 0;

        for (int i = 0; i < slotList.Count; i++)
        {
            if (!slotList[i].GetComponent<PowerUpItem_Slots>().isFree)
            {
                itemAmount++;
            }         
        }

        existingItemAmountText.text =  itemAmount.ToString();
    }

    void InstantiateSlots()
    {
        slotWidthAdjustedToParent = innerPanelContainer.transform.parent.GetComponent<RectTransform>().sizeDelta.y;
        for (int i = 0; i < slotCount; i++)
        {
            GameObject currentNewSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "Slot_ProducedItems"));
            RectTransform rt = currentNewSlot.GetComponent<RectTransform>();
            currentNewSlot.transform.SetParent(innerPanelContainer.transform, false);
            rt.sizeDelta = new Vector2(slotWidthAdjustedToParent * .75f, slotWidthAdjustedToParent * .75f);
            

            if (i == 0)
            {
                slotWidth = rt.sizeDelta.x;
            }
            Vector2 slotPosition = new Vector2(i * (slotWidth), 0);

            rt.anchoredPosition = slotPosition;
            slotList.Add(currentNewSlot);

            currentNewSlot.GetComponent<PowerUpItem_Slots>().slotIDNumber = i;
        }

        ResetInnerPanelWidth();
    }
    public void ReceivePositionAndEvaluate(GameItems gameItemIN)
    {
        Vector3 itemCurrentPositionLocal = innerPanelContainer.transform.InverseTransformPoint(gameItemIN.transform.position);
        if (isFull && !gameItemIN.isInsidePowerUpPanel) return;

        foreach (GameObject slot in slotList)
        {
            RectTransform rtSlot = slot.GetComponent<RectTransform>();

            if(itemCurrentPositionLocal.x > rtSlot.localPosition.x && itemCurrentPositionLocal.x < (rtSlot.localPosition.x + slotWidth))
            {

                PowerUpItem_Slots scriptSlot = slot.GetComponent<PowerUpItem_Slots>();

                if (!scriptSlot.isFree && !gameItemIN.isInsidePowerUpPanel)
                {
                    for (int i = slotList.Count-1; i > scriptSlot.slotIDNumber-1; i--)
                    {
                        if(slotList[i].GetComponent<PowerUpItem_Slots>().isFree == false)
                        {
                            GameItems itemToMove = slotList[i].GetComponent<PowerUpItem_Slots>().containedItem.GetComponent<GameItems>();
                            slotList[i + 1].GetComponent<PowerUpItem_Slots>().Drop(itemToMove);
                            slotList[i].GetComponent<PowerUpItem_Slots>().DischargeItem();
                        }
                    }
                    //backupInitialGameSLot = gameItemIN.initialGameSlot;
                    gameItemIN.initialGameSlot = slot;
                    gameItemIN.isInsidePowerUpPanel = true;
                }

                else if (!scriptSlot.isFree && gameItemIN.isInsidePowerUpPanel)
                {
                    int ItemSlotID = gameItemIN.initialGameSlot.GetComponent<PowerUpItem_Slots>().slotIDNumber;

                    if (scriptSlot.slotIDNumber == ItemSlotID) return;

                    else if (scriptSlot.slotIDNumber > ItemSlotID )
                    {
                        //if(itemCurrentPositionLocal.x > rtSlot.localPosition.x )
                        //{

                        for (int i = ItemSlotID + 1; i < scriptSlot.slotIDNumber + 1; i++)
                        {
                            GameItems itemToMove = slotList[i].GetComponent<PowerUpItem_Slots>().containedItem.GetComponent<GameItems>();
                            slotList[i - 1].GetComponent<PowerUpItem_Slots>().Drop(itemToMove);
                            slotList[i].GetComponent<PowerUpItem_Slots>().DischargeItem();
                        }

                        //GameItems itemToMove =scriptSlot.containedItem.GetComponent<GameItems>();
                        //gameItemIN.initialGameSlot.GetComponent<PowerUpItem_Slots>().Drop(itemToMove);
                        //scriptSlot.DischargeItem();
                        gameItemIN.initialGameSlot = slot;
                        //}
                    }

                    else if (scriptSlot.slotIDNumber < ItemSlotID)
                    {
                        //if (itemCurrentPositionLocal.x < rtSlot.localPosition.x )
                        //{

                        for (int i = ItemSlotID - 1; i > scriptSlot.slotIDNumber - 1; i--)
                        {
                            GameItems itemToMove = slotList[i].GetComponent<PowerUpItem_Slots>().containedItem.GetComponent<GameItems>();
                            slotList[i + 1].GetComponent<PowerUpItem_Slots>().Drop(itemToMove);
                            slotList[i].GetComponent<PowerUpItem_Slots>().DischargeItem();
                        }

                        //GameItems itemToMove = scriptSlot.containedItem.GetComponent<GameItems>();
                        //gameItemIN.initialGameSlot.GetComponent<PowerUpItem_Slots>().Drop(itemToMove);
                        //scriptSlot.DischargeItem();
                        gameItemIN.initialGameSlot = slot;
                        // }
                    }
                }
            }
        }
    }

    //public void ReceiveItemRayAndEvaluate(GameItems gameItemIN,PowerUpItem_Slots powerUpSlotIN, bool wasOnTheSlot, bool isPointerDirectionPositiveIN, Vector3 previousPositionIN)
    //{
    //    Vector3 itemCurrentPosLocal = innerPanelContainer.transform.InverseTransformPoint(gameItemIN.transform.position);
    //    Vector3 previousPosition = innerPanelContainer.transform.InverseTransformPoint(previousPositionIN);

    //    if (isPointerDirectionPositiveIN && !powerUpSlotIN.isFree)
    //    {
    //        Debug.Log(itemCurrentPosLocal.x);
    //        Debug.Log(previousPosition.x);
    //        Debug.Log((powerUpSlotIN.GetComponent<RectTransform>().localPosition.x) + (slotWidth * .5)) ;

    //        //Vector3 difference = itemCurrentPosLocal - previousPosition;

    //        foreach (GameObject slot in slotList)
    //        {
    //            if( previousPosition.x< slot.GetComponent<RectTransform>().localPosition.x + (slotWidth * .5) && slot.GetComponent<RectTransform>().localPosition.x + (slotWidth * .5) < itemCurrentPosLocal.x)
    //            {
    //                Debug.Log(slot.GetComponent<PowerUpItem_Slots>().slotIDNumber);
    //            }
    //        }

            //if (itemCurrentPosLocal.x > powerUpSlotIN.GetComponent<RectTransform>().localPosition.x + (slotWidth/2))
            //{
            //    //if (previousPosition.x > powerUpSlotIN.GetComponent<RectTransform>().localPosition.x + (slotWidth * .5f)) return;
            //    //if (previousPosition.x > powerUpSlotIN.GetComponent<RectTransform>().localPosition.x + (slotWidth * .5f)) return;
            //    Debug.Log("positive working");
            //    //if (previousPosition.x > powerUpSlotIN.GetComponent<RectTransform>().localPosition.x) return;

            //    int slotID = powerUpSlotIN.slotIDNumber;
            //    if (slotID-1 >=0 && slotList[slotID-1].GetComponent<PowerUpItem_Slots>().isFree)
            //    {
            //        GameItems itemToMove = powerUpSlotIN.containedItem.GetComponent<GameItems>();
            //        slotList[slotID - 1].GetComponent<PowerUpItem_Slots>().Drop(itemToMove);
            //        powerUpSlotIN.DischargeItem();

            //        isTemporarlyModified = true;
            //    }
            //}
            
        //}

        //else if (!isPointerDirectionPositiveIN && !powerUpSlotIN.isFree)
        //{
        //    Debug.Log("negative working");
        //    if (itemCurrentPosLocal.x < (powerUpSlotIN.GetComponent<RectTransform>().localPosition.x) + (slotWidth / 2))
        //    {

        //        //if (previousPosition.x < powerUpSlotIN.GetComponent<RectTransform>().localPosition.x) return;

        //        int slotID = powerUpSlotIN.slotIDNumber;
        //        if (slotID+1<slotList.Count && slotList[slotID + 1].GetComponent<PowerUpItem_Slots>().isFree  )
        //        {

        //            GameItems itemToMove = powerUpSlotIN.containedItem.GetComponent<GameItems>();
        //            slotList[slotID + 1].GetComponent<PowerUpItem_Slots>().Drop(itemToMove);
        //            powerUpSlotIN.DischargeItem();

        //            isTemporarlyModified = true;
        //        }
        //    }
        //}

        //if (!wasOnTheSlot && !powerUpSlotIN.isFree)
        //{
        //    int slotID = powerUpSlotIN.slotIDNumber;
        //    for (int i = slotList.Count-1; i > slotID-1; i--)
        //    {
        //        PowerUpItem_Slots slot = slotList[i].Item1.GetComponent<PowerUpItem_Slots>();

        //        if (!slot.isFree)
        //        {
        //            GameItems itemToMove = slot.containedItem.GetComponent<GameItems>();
        //            slotList[i+1].Item1.GetComponent<PowerUpItem_Slots>().Drop(itemToMove);
        //            slot.DischargeItem();
        //        }
        //    }
        //}
    //}

    public void ResetSlotPositions(GameItems gameItemIN)
    {
        int initialSlotID = gameItemIN.initialGameSlot.GetComponent<PowerUpItem_Slots>().slotIDNumber;
        gameItemIN.isInsidePowerUpPanel = false;
        //gameItemIN.initialGameSlot = backupInitialGameSLot;

        for (int i = initialSlotID+1; i < slotList.Count; i++)
        {           
            if (slotList[i].GetComponent<PowerUpItem_Slots>().isFree) break;

            GameItems itemToMove = slotList[i].GetComponent<PowerUpItem_Slots>().containedItem.GetComponent<GameItems>();
            slotList[i-1].GetComponent<PowerUpItem_Slots>().Drop(itemToMove);
            slotList[i].GetComponent<PowerUpItem_Slots>().DischargeItem();         
        }
        StartCoroutine(ShiftPanelOnResetEnum());
        FullCheck();
    }
    private void EndDragMethods(PointerEventData pointerEvent, bool endBool, bool isInsidePowerUpPanel)
    {
        FullCheck();
        SetExistingItemAmount();

        if (!isFull && isInsidePowerUpPanel) StartCoroutine(LerpPanelSizeEnum());
    }

    private void FullCheck()
    {
        if (slotList.Any(slots => slots.GetComponent<PowerUpItem_Slots>().isFree == true)) isFull = false;
        else isFull = true;
    }

    //public void FindSlotsToMoveAndDrop(GameItems gameItemIN)
    //{
    //    Vector3 itemDroppedPosX = innerPanelContainer.transform.InverseTransformPoint(gameItemIN.transform.position);

    //    //gameItemIN.transform.SetParent(innerPanelContainer.transform,false);
    //    //gameItemIN.GetComponent<RectTransform>().anchorMin = slotList[0].Item1.GetComponent<RectTransform>().anchorMin;
    //    //gameItemIN.GetComponent<RectTransform>().anchorMax = slotList[0].Item1.GetComponent<RectTransform>().anchorMax;
    //    //gameItemIN.GetComponent<RectTransform>().pivot = slotList[0].Item1.GetComponent<RectTransform>().pivot;
    //    //float itemDroppedPosX = gameItemIN.GetComponent<RectTransform>().localPosition.x;

    //    for (int i = 0; i < slotList.Count; i++)
    //    {
    //        if(itemDroppedPosX .x <= slotList[i].Item2)
    //        {
    //            PowerUpItem_Slots powerupSlots = slotList[i].Item1.GetComponent<PowerUpItem_Slots>();

    //            if(powerupSlots.isFree)
    //            {
    //                for(int x = 0; x < slotList.Count; x++)
    //                {
    //                    if (slotList[x].Item1.GetComponent<PowerUpItem_Slots>().isFree)
    //                    {
    //                        slotList[x].Item1.GetComponent<PowerUpItem_Slots>().Drop(gameItemIN);
    //                        break;
    //                    }
    //                }

    //            }
    //            else
    //            {
    //                //int index = slotList.IndexOf((slotList[i].Item1, slotList[i].Item2));

    //                //MoveItemsInsideSlots(i, gameItemIN);
    //                //powerupSlots.Drop(gameItemIN);
    //            }
    //            StartCoroutine(LerpPanelSizeEnum());
    //            return;
    //        }
    //    }
    //}

    //void MoveItemsInsideSlots(int indexIN, GameItems gameItemIN)
    //{
    //    for (int i = slotList.Count-1; i > indexIN-1; i--)
    //    {
    //        PowerUpItem_Slots slot = slotList[i].GetComponent<PowerUpItem_Slots>();
    //        if (!slot.isFree)
    //        {
    //            GameItems itemToMove = slot.containedItem.GetComponent<GameItems>();
    //            slotList[i + 1].GetComponent<PowerUpItem_Slots>().Drop(itemToMove);
    //            slot.DischargeItem();

    //        } 
    //        if(i == indexIN)
    //        {
    //            slot.Drop(gameItemIN);
    //        }
    //    }
    //}

    IEnumerator LerpPanelSizeEnum()
    {
        Vector2 originalSize = panelBackground.sizeDelta;
        Vector2 scaleFactor = new Vector2(1.05f, 1.05f);
        float elapsedTime = 0f;
        float lerpDuration = .1f;

      
        while (elapsedTime < lerpDuration)
        {
            panelBackground.sizeDelta = Vector2.Lerp(originalSize, originalSize * scaleFactor, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        panelBackground.sizeDelta = originalSize * scaleFactor;

        elapsedTime = 0f;
        while (elapsedTime < lerpDuration)
        {
            panelBackground.sizeDelta = Vector2.Lerp(originalSize * scaleFactor, originalSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        panelBackground.sizeDelta = originalSize;

    }
        
    IEnumerator ShiftPanelOnResetEnum()
    {
        Vector2 originalLocation = thisPanel.anchoredPosition;
        float lerpAmout = thisPanel.sizeDelta.x / 93;
        Vector2 lerpedLocation = new Vector2(originalLocation.x - lerpAmout, originalLocation.y);
        float lerpDuration = .1f;
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            thisPanel.anchoredPosition = Vector2.Lerp(originalLocation, lerpedLocation, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        thisPanel.anchoredPosition = lerpedLocation;
        elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            thisPanel.anchoredPosition = Vector2.Lerp(lerpedLocation, originalLocation, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        thisPanel.anchoredPosition = originalLocation;
    }

    public void SelectContainedItems()
    {
        existingItemsList.Clear();

        foreach (GameObject slot in slotList)
        {
            if (slot.GetComponent<PowerUpItem_Slots>().containedItem)
            {
                GameItems existingItem = slot.GetComponent<PowerUpItem_Slots>().containedItem.GetComponent<GameItems>();
                Vector2 existingPosition = existingItem.GetComponent<RectTransform>().anchoredPosition;
                existingItemsList.Add((existingItem, existingPosition, new Vector2(existingPosition.x, Random.Range(existingPosition.y+slotWidth/2, existingPosition.y +slotWidth/ 1))));
               
            }
        }
        if(existingItemsList.Count>0)  StartCoroutine(ShakeItemsOnPanel(existingItemsList));
    }

    IEnumerator ShakeItemsOnPanel(List<(GameItems, Vector2, Vector2)> existingItemsListIN) //TEK TEK YOLLA DAA HIZLI OLUR !!!!!!!!!!!!!
    {
        Debug.Log("working");

        float elapsedTime = 0f;
        float lerpDuration = .24f;
        while (elapsedTime < lerpDuration)
        {
            foreach ((GameItems, Vector2,Vector2) item in existingItemsListIN)
            {
                item.Item1.GetComponent<Image>().maskable = false;
                item.Item1.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(item.Item2, item.Item3, elapsedTime / lerpDuration);
                //yield return null;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        foreach ((GameItems, Vector2, Vector2) item in existingItemsListIN)
        {
            item.Item1.GetComponent<RectTransform>().anchoredPosition = item.Item3;
        }
        //yield return null;

        elapsedTime = 0f;
        while (elapsedTime < lerpDuration)
        {
            foreach ((GameItems, Vector2, Vector2) item in existingItemsListIN)
            {
                item.Item1.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(item.Item3, item.Item2, elapsedTime / lerpDuration);
                //yield return null;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        foreach ((GameItems, Vector2, Vector2) item in existingItemsListIN)
        {
            item.Item1.GetComponent<RectTransform>().anchoredPosition = item.Item2;
            item.Item1.GetComponent<Image>().maskable = true;
        }
        yield return null;

    }

    void AddExtraSlots()
    {
        GameObject currentNewSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "Slot_ProducedItems"));
        RectTransform rt = currentNewSlot.GetComponent<RectTransform>();
        currentNewSlot.transform.SetParent(innerPanelContainer.transform, false);
        rt.sizeDelta = new Vector2(slotWidthAdjustedToParent * .75f, slotWidthAdjustedToParent * .75f);

        Vector2 slotPosition = new Vector3(slotList.Count * (slotWidth), 0);
        rt.anchoredPosition = slotPosition;
        slotList.Add(currentNewSlot);

        currentNewSlot.GetComponent<PowerUpItem_Slots>().slotIDNumber = slotList.Count-1;

        ResetInnerPanelWidth();
    }

    void ResetInnerPanelWidth()
    {
        float lastSlotPositionRightEnd = (slotList[slotList.Count - 1].GetComponent<RectTransform>().anchoredPosition.x) + slotWidth;
        RectTransform rt = innerPanelContainer.GetComponent<RectTransform>();
        if (rt.sizeDelta.x < lastSlotPositionRightEnd)
        {
            rt.sizeDelta = new Vector2(lastSlotPositionRightEnd, rt.sizeDelta.y);
        }
    }



    //IEnumerator CheckPointerPositionEnum()
    //{
    //    while (isFocused)
    //    {
    //        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        Vector3 localPointerPos = innerPanelContainer.transform.InverseTransformPoint(mouseWorldPosition);

    //        for (int i = 0; i < slotList.Count; i++)
    //        {
    //            if(localPointerPos.x <= slotList[i].Item2)
    //            {

    //            }
    //        }

    //        yield return null;
    //    }

    //}



    //void SetAcceptedItemGenres()
    //{
    //    acceptedItemGenres.Add(Item.ItemGenre.Chest);
    //}


}
