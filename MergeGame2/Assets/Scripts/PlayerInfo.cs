using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerInfo : MonoBehaviour
{
    private static PlayerInfo _instance;
    public static PlayerInfo Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    private Dictionary<int, GameItems> inventory = new Dictionary<int, GameItems>();

    private int remainingInventorySlotAmount;
    public int currentInventorySlotAmount { get; private set; }
    public int maxInventorySlotAmount { get; private set; }
    

    //public List<GameObject> ownedItems { get; private set; }


    private void Awake()
    {
        //if (_instance != null && _instance != this) BUNU SINGLETON PATLADIÐI ÝÇÝN SÝLDÝM - YAPMAK LAZIM
        //{
        //    Destroy(this.gameObject);
        //}

        //else
        //{
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = this;
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        //}

        currentInventorySlotAmount = 5;
        maxInventorySlotAmount = 28;
        GetRemainingInventorySLotAmount();
    }

    

    public int GetRemainingInventorySLotAmount()
    {
        remainingInventorySlotAmount = currentInventorySlotAmount - GetDictionaryAmount();

        return remainingInventorySlotAmount;
    }

    public int GetDictionaryAmount()
    {
        return inventory.Count;
    }


    public void AugmentCurrentInventorySlotAmount(int updatedCurrentInventorySLotAmount)
    {
        currentInventorySlotAmount = updatedCurrentInventorySLotAmount;

        //OnAugmentedMaxInventorySize?.Invoke();
    }

    public void ListenInventorySlots(InventorySlots inventorySlots)
    {
        inventorySlots.onInventoryPlacedItem += AddToDictionnary;
    }


    void AddToDictionnary(object semder, InventorySlots.OnInventoryItemPlacedEventArgs e)
    {

        inventory.Add(e.slotIDNumber, e.gameItem);

    }

    //void RemoveFromDictionnary(GameObject removedItem)
    //{
    //    if (ownedItems != null && ownedItems.Count > 0)
    //    {
    //        ownedItems.Remove(removedItem);
    //    }
    //    else
    //    {
    //        Debug.Log("no item left in your inventory");
    //    }
    //}




}
