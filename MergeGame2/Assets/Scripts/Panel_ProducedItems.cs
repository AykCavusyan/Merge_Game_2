using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_ProducedItems : MonoBehaviour
{
    private GameObject innerPanelContainer;
    private int slotCount;
    private float slotWidth;
    private List<GameObject> slotList = new List<GameObject>();

    private void Awake()
    {
        innerPanelContainer = transform.GetChild(0).GetChild(0).gameObject;
    }

    private void Start()
    {
        GetSlotCount();
        InstantiateSlots();
    }

    void GetSlotCount()
    {
        slotCount = 7; // bu daha sonra playerinfordan yapýlacak
    }

    void InstantiateSlots()
    {
        for (int i = 0; i < slotCount; i++)
        {
            GameObject currentNewSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "Slot_ProducedItems"));
            RectTransform rt = currentNewSlot.GetComponent<RectTransform>();
            currentNewSlot.transform.SetParent(innerPanelContainer.transform, false);

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

    void ResetInnerPanelWidth()
    {
        float lastSlotPositionRightEnd = (slotList[slotList.Count - 1].GetComponent<RectTransform>().anchoredPosition.x) + slotWidth;
        RectTransform rt = innerPanelContainer.GetComponent<RectTransform>();
        if (rt.sizeDelta.x < lastSlotPositionRightEnd)
        {
            rt.sizeDelta = new Vector2(lastSlotPositionRightEnd, rt.sizeDelta.y);
        }
    }
}
