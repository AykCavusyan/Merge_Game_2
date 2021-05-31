using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemBag : MonoBehaviour
{
    private Inventory inventory;
    public Item item;
    public GameObject canvas;

    public GameObject[] gameSlots;
    public GameObject newGameItemIdentified;

    private void Awake()
    {
        inventory = GameObject.Find("Player").GetComponent<Inventory>();
        canvas = GameObject.Find("Canvas");
        gameSlots = GameObject.FindGameObjectsWithTag("Container");
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(inventory.slots.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Item GenerateRandomItemType()
    {
        // bu item buradan çýkartýlaiblir mi ?---------------------
        item = new Item();
        var randomItemName = item.GetRandomObject();
        return new Item { itemType = randomItemName };

    }

    public void IdentifyItem(GameObject newGameItem)
    {
        newGameItemIdentified = newGameItem;
    }

    public int FindEmptySlotPosition()
    {
        for (int i = 0; i < gameSlots.Length; i++)
        {
            if (gameSlots[i].GetComponent<GameSlots>().canDrop)
            {
                return i;
            }
        }
        return -1;
        
    }

    public GameObject GenerateItem(GameObject item1 = null, GameObject item2=null)
    {
        if (item1 == null && item2 == null)
        {
            item = GenerateRandomItemType();
        }
        else
        {
            item = new Item { itemType = Item.ItemType.Sun };
        }

        GameObject newGameItem = new GameObject();
        newGameItem.transform.SetParent(canvas.transform);      
        newGameItem.AddComponent<Image>().sprite = item.GetSprite();
        newGameItem.AddComponent<GameItems>();
        newGameItem.tag = item.itemType.ToString();
        newGameItem.layer = 5;

        return newGameItem;
    }

    

     public void AddGeneratedItem()
     {
        int i = FindEmptySlotPosition();

           if (i >= 0 && i <= gameSlots.Length)
           {
            
               GameObject newGameItem = GenerateItem();
               newGameItem.name = "GameItem" + i + 1;
               gameSlots[i].GetComponent<GameSlots>().Drop(newGameItem.GetComponent<GameItems>());
               IdentifyItem(newGameItem);

            
               //would be much better if made with an event - need to und how to pass the variable 
               gameSlots[i].GetComponent<GameSlots>().canDrop = false;

               inventory.AddItemToList(item);
           }
           else
           {
            Debug.Log("There is no place left!");
           }
        
     }


}

     

