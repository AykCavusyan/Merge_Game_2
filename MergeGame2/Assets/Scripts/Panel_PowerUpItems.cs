using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_PowerUpItems : MonoBehaviour
{
    private GameObject innerPanelContainer;
    private int slotCount;
    public float slotWidth { get; private set; }
    private float slotWidthAdjustedToParent;
    public List<(GameObject,float)> slotList { get; private set; } = new List<(GameObject,float)>();

    private void Awake()
    {
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
        
        gameItemIN.transform.SetParent(innerPanelContainer.transform,false);
        //gameItemIN.GetComponent<RectTransform>().anchorMin = slotList[0].Item1.GetComponent<RectTransform>().anchorMin;
        //gameItemIN.GetComponent<RectTransform>().anchorMax = slotList[0].Item1.GetComponent<RectTransform>().anchorMax;
        //gameItemIN.GetComponent<RectTransform>().pivot = slotList[0].Item1.GetComponent<RectTransform>().pivot;
        float itemDroppedPosX = gameItemIN.GetComponent<RectTransform>().localPosition.x;
        Debug.Log(itemDroppedPosX);
        
        for (int i = 0; i < slotList.Count; i++)
        {
            if(itemDroppedPosX <= slotList[i].Item2)
            {
                PowerUpItem_Slots powerupSlots = slotList[i].Item1.GetComponent<PowerUpItem_Slots>();

                if(powerupSlots.isFree)
                {
                    Debug.Log(i);
                    Debug.Log(slotList[i].Item2);
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
                    int index = slotList.IndexOf((slotList[i].Item1, slotList[i].Item2));
                    
                    MoveItemsInsideSlots(index);
                    powerupSlots.Drop(gameItemIN);
                }
                return;
            }
        }
    }

    void MoveItemsInsideSlots(int indexIN)
    {
        Debug.Log("working" + indexIN);
        Debug.Log(slotList.Count - 1);
        for (int i = slotList.Count-1; i > indexIN; i--)
        {
            Debug.Log("working" + i);
            PowerUpItem_Slots slot = slotList[i].Item1.GetComponent<PowerUpItem_Slots>();
            if (!slot.isFree)
            {
                Debug.Log("working"+i);
                GameItems itemToMove = slot.containedItem.GetComponent<GameItems>();
                slotList[i + 1].Item1.GetComponent<PowerUpItem_Slots>().Drop(itemToMove);
                slot.DischargeItem();
                   
            }         
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

    //void SetAcceptedItemGenres()
    //{
    //    acceptedItemGenres.Add(Item.ItemGenre.Chest);
    //}


}
