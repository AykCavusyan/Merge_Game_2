using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProducedItem_Slots : MonoBehaviour
{
    public int slotIDNumber;
    public bool isFree { get; private set; } = true;
    public GameObject containedItem { get; private set; }

    private int maxStackAmount = 5;
    private GameObject[] stackedItems;
    private Item.ItemType itemType;
    private Text itemAmount;

    private RectTransform rtSlot;

    private void Awake()
    {
        stackedItems = new GameObject[maxStackAmount];
        rtSlot = GetComponent<RectTransform>();
    }

    public void Drop(GameItems gameItem)
    {

        PlaceItem(gameItem);
        UpdateParentSlot(gameItem);
        isFree = false;
    }

    void PlaceItem(GameItems gameItemIN)
    {
        RectTransform rt = gameItemIN.GetComponent<RectTransform>();
        rt.SetParent(this.transform);
        rt.sizeDelta = rtSlot.sizeDelta;
        rt.localScale = new Vector3(1, 1, 1);
        containedItem = gameItemIN.gameObject;
        rt.SetAsLastSibling();
        rt.anchoredPosition = rtSlot.anchoredPosition;
        gameItemIN.isMoving = false;
        gameItemIN.initialGameSlot = this.gameObject;

    }

    void TryStackItems(GameItems gameItemIN)
    {

    }

    void UpdateParentSlot(GameItems gameItemIN)
    {


    }

    void DischargeItem(GameItems gameItemIN)
    {
        isFree = true;
        containedItem = null;
    }
}
