using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerInfo : MonoBehaviour
{
    private static PlayerInfo _instance;
    public static PlayerInfo Instance { get { return _instance; } }
    private static readonly object _lock = new object();


    public int currentInventorySlotAmount { get; private set; }
    public int maxInventorySlotAmount { get; private set; }
    

    public List<GameObject> ownedItems { get; private set; }


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
    }




    public void AugmentCurrentInventorySlotAmount(int updatedCurrentInventorySLotAmount)
    {
        currentInventorySlotAmount = updatedCurrentInventorySLotAmount;

        //OnAugmentedMaxInventorySize?.Invoke();
    }



    void AddItem(GameObject newItem)
    {

        if (ownedItems != null && ownedItems.Count < currentInventorySlotAmount )
        {
            ownedItems.Add(newItem);
        }
        else if (ownedItems != null && ownedItems.Count < maxInventorySlotAmount)
        {
            Debug.Log("you have reached max possible inv slots");
        }
        else
        {
            Debug.Log("no place left in the inventory please puchase");
        }

    }

    void RemoveItem(GameObject removedItem)
    {
        if (ownedItems != null && ownedItems.Count > 0)
        {
            ownedItems.Remove(removedItem);
        }
        else
        {
            Debug.Log("no item left in your inventory");
        }
    }




}
