using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_ProducedItems : MonoBehaviour
{
    private GameObject innerPanelContainer;
    private int slotCount;
    private float slotWidth;
    private float slotWidthAdjustedToParent;
    public List<GameObject> slotList { get; private set; } = new List<GameObject>();
    public List<Item.ItemGenre> acceptedItemGenres { get; private set; } = new List<Item.ItemGenre>();

    private void Awake()
    {
        innerPanelContainer = transform.GetChild(0).GetChild(0).gameObject;
    }

    private void Start()
    {
        GetSlotCount();
        InstantiateSlots();
        SetAcceptedItemGenres();
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
            Vector3 slotPosition = new Vector3(i * (slotWidth + (slotWidth / 8)), 0, 0);

            rt.anchoredPosition = slotPosition;
            slotList.Add(currentNewSlot);

            currentNewSlot.GetComponent<ProducedItem_Slots>().slotIDNumber = i;
        }

        ResetInnerPanelWidth();
    }

    void AddExtraSlots()
    {
        GameObject currentNewSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "Slot_ProducedItems"));
        RectTransform rt = currentNewSlot.GetComponent<RectTransform>();
        currentNewSlot.transform.SetParent(innerPanelContainer.transform, false);
        rt.sizeDelta = new Vector2(slotWidthAdjustedToParent * .75f, slotWidthAdjustedToParent * .75f);

        Vector3 slotPosition = new Vector3(slotList.Count * (slotWidth + (slotWidth / 8)), 0, 0);
        rt.anchoredPosition = slotPosition;
        slotList.Add(currentNewSlot);

        currentNewSlot.GetComponent<ProducedItem_Slots>().slotIDNumber = slotList.Count-1;

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

    void SetAcceptedItemGenres()
    {
        acceptedItemGenres.Add(Item.ItemGenre.Chest);
    }


}
