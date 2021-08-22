using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Panel_PowerUpItems : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private bool isFocused = false;
    private RectTransform panelBackground;
    private GameObject innerPanelContainer;
    private int slotCount;
    public float slotWidth { get; private set; }
    private float slotWidthAdjustedToParent;
    public List<(GameObject,float)> slotList { get; private set; } = new List<(GameObject,float)>();

    private void Awake()
    {
        panelBackground = transform.parent.GetChild(this.transform.GetSiblingIndex() - 1).GetComponent<RectTransform>();
        innerPanelContainer = transform.GetChild(0).GetChild(0).gameObject;
    }

    private void Start()
    {
        GetSlotCount();
        InstantiateSlots();
        //SetAcceptedItemGenres();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) AddExtraSlots();
    }

    void GetSlotCount()
    {
        slotCount = 7; // bu daha sonra playerinfordan yapýlacak
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
            Vector2 slotPosition = new Vector2(i * (slotWidth + (slotWidth / 8)), 0);

            rt.anchoredPosition = slotPosition;
            slotList.Add((currentNewSlot,rt.localPosition.x + (rt.sizeDelta.x/2) ));

            currentNewSlot.GetComponent<PowerUpItem_Slots>().slotIDNumber = i;
        }

        ResetInnerPanelWidth();
    }

    public void FindSlotsToMoveAndDrop(GameItems gameItemIN)
    {
        Vector3 itemDroppedPosX = innerPanelContainer.transform.InverseTransformPoint(gameItemIN.transform.position);



        //gameItemIN.transform.SetParent(innerPanelContainer.transform,false);
        //gameItemIN.GetComponent<RectTransform>().anchorMin = slotList[0].Item1.GetComponent<RectTransform>().anchorMin;
        //gameItemIN.GetComponent<RectTransform>().anchorMax = slotList[0].Item1.GetComponent<RectTransform>().anchorMax;
        //gameItemIN.GetComponent<RectTransform>().pivot = slotList[0].Item1.GetComponent<RectTransform>().pivot;
        //float itemDroppedPosX = gameItemIN.GetComponent<RectTransform>().localPosition.x;
        Debug.Log(itemDroppedPosX.x);
        
        for (int i = 0; i < slotList.Count; i++)
        {
            if(itemDroppedPosX .x <= slotList[i].Item2)
            {
                PowerUpItem_Slots powerupSlots = slotList[i].Item1.GetComponent<PowerUpItem_Slots>();

                if(powerupSlots.isFree)
                {
                    for(int x = 0; x < slotList.Count; x++)
                    {
                        Debug.Log("considered free slot");
                        if (slotList[x].Item1.GetComponent<PowerUpItem_Slots>().isFree)
                        {
                            slotList[x].Item1.GetComponent<PowerUpItem_Slots>().Drop(gameItemIN);
                            break;
                        }
                    }
                    
                }
                else
                {
                    //int index = slotList.IndexOf((slotList[i].Item1, slotList[i].Item2));
                    
                    MoveItemsInsideSlots(i, gameItemIN);
                    //powerupSlots.Drop(gameItemIN);
                }
                StartCoroutine(LerpPanelSizeEnum());
                return;
            }
        }
    }

    void MoveItemsInsideSlots(int indexIN, GameItems gameItemIN)
    {
        for (int i = slotList.Count-1; i > indexIN-1; i--)
        {
            PowerUpItem_Slots slot = slotList[i].Item1.GetComponent<PowerUpItem_Slots>();
            if (!slot.isFree)
            {
                GameItems itemToMove = slot.containedItem.GetComponent<GameItems>();
                slotList[i + 1].Item1.GetComponent<PowerUpItem_Slots>().Drop(itemToMove);
                slot.DischargeItem();
                   
            } 
            if(i == indexIN)
            {
                slot.Drop(gameItemIN);
            }
        }
    }

    IEnumerator LerpPanelSizeEnum()
    {
        Vector2 originalSize = panelBackground.sizeDelta;
        Vector2 scaleFactor = new Vector2(1.05f, 1.05f);
        float elapsedTime = 0f;
        float lerpDuration = .06f;
        int counter = 0;

        while (counter !=2)
        {
            while (elapsedTime < lerpDuration)
            {
                panelBackground.sizeDelta = Vector2.Lerp(originalSize, originalSize * scaleFactor, elapsedTime / lerpDuration);
                elapsedTime += Time.deltaTime;

                yield return null;
            }
            Debug.Log("asdad");
            panelBackground.sizeDelta = originalSize * scaleFactor;

            elapsedTime = 0f;
            while (elapsedTime < lerpDuration)
            {
                panelBackground.sizeDelta = Vector2.Lerp(originalSize * scaleFactor, originalSize, elapsedTime / lerpDuration);
                elapsedTime += Time.deltaTime;

                yield return null;
            }
            panelBackground.sizeDelta = originalSize;
            counter++;
            elapsedTime = 0F;

            yield return null;
        }
        


    }

    void AddExtraSlots()
    {
        GameObject currentNewSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "Slot_ProducedItems"));
        RectTransform rt = currentNewSlot.GetComponent<RectTransform>();
        currentNewSlot.transform.SetParent(innerPanelContainer.transform, false);
        rt.sizeDelta = new Vector2(slotWidthAdjustedToParent * .75f, slotWidthAdjustedToParent * .75f);

        Vector3 slotPosition = new Vector3(slotList.Count * (slotWidth + (slotWidth / 8)), 0, 0);
        rt.anchoredPosition = slotPosition;
        slotList.Add((currentNewSlot, rt.localPosition.x + (rt.sizeDelta.x / 2) ));

        currentNewSlot.GetComponent<PowerUpItem_Slots>().slotIDNumber = slotList.Count-1;

        ResetInnerPanelWidth();
    }

    void ResetInnerPanelWidth()
    {
        float lastSlotPositionRightEnd = (slotList[slotList.Count - 1].Item1.GetComponent<RectTransform>().anchoredPosition.x) + slotWidth;
        RectTransform rt = innerPanelContainer.GetComponent<RectTransform>();
        if (rt.sizeDelta.x < lastSlotPositionRightEnd)
        {
            rt.sizeDelta = new Vector2(lastSlotPositionRightEnd, rt.sizeDelta.y);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isFocused = true;
        StartCoroutine(CheckPointerPositionEnum());
        Debug.Log("isfocused");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isFocused = false;
        Debug.Log("isNOTfocused");

    }

    IEnumerator CheckPointerPositionEnum()
    {
        while (isFocused)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 localPointerPos = innerPanelContainer.transform.InverseTransformPoint(mouseWorldPosition);

            for (int i = 0; i < slotList.Count; i++)
            {
                if(localPointerPos.x <= slotList[i].Item2)
                {

                }
            }

            yield return null;
        }
        
    }



    //void SetAcceptedItemGenres()
    //{
    //    acceptedItemGenres.Add(Item.ItemGenre.Chest);
    //}


}
