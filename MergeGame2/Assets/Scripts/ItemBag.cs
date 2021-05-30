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

    public GameSlots[] gameSlots;
    public GameObject newGameItemIdentified;
    

    // Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.Find("Player").GetComponent<Inventory>();
        canvas = GameObject.Find("Canvas");
       
        


        Debug.Log(inventory.slots.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Item AddRandomItem()
    {
        item = new Item();
        var randomItemName = item.GetRandomObject();
        return new Item { itemType = randomItemName };

    }

    public void IdentifyItem(GameObject newGameItem)
    {
        newGameItemIdentified = newGameItem;
    }

     public void GenerateItem()
    {
        item = AddRandomItem();

        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.CanAdd(i)) 
            {

                GameObject newGameItem = new GameObject();
                newGameItem.transform.SetParent(canvas.transform); 
                newGameItem.name = "GameItem" + i+1;
                newGameItem.AddComponent<Image>().sprite = item.GetSprite();
                newGameItem.AddComponent<GameItems>();
                newGameItem.tag = item.itemType.ToString();
                newGameItem.layer = 5;

                inventory.slots[i].GetComponent<GameSlots>().Drop(newGameItem.GetComponent<GameItems>());
                IdentifyItem(newGameItem);

                //would be much better if made with an event - need to und how to pass the variable 
                inventory.canDrop[i] = false;
                inventory.AddItemToList(item);

                break ;
            }
            
        }
    }

     
}
