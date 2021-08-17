using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProducedItem_Slots : MonoBehaviour
{
    public int slotIDNumber;
    public bool isFree = true;
    public GameObject containedItem { get; private set; }

    private int maxStackAmount = 5;
    private GameObject[] stackedItems;
    private Item.ItemType itemType;
    private Text itemAmount;

    private void Awake()
    {
        stackedItems = new GameObject[maxStackAmount];
    }

    public void Drop(GameItems gameItem)
    {
        //// if gmaeitem is suitable ot put here ////
        

        PlaceItem(gameItem);
        UpdateParentSlot(gameItem);
        isFree = false;
    }

    void PlaceItem(GameItems gameItemIN)
    {

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
