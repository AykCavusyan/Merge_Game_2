using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemBag : MonoBehaviour
{
    private Inventory inventory;
    public Item item;
    //public GameObject slotsPanel;

    public GameObject canvas;



    public GameObject[] gameSlots;
    public GameObject newGameItemIdentified;

    private void Awake()
    {
        inventory = GameObject.Find("Player").GetComponent<Inventory>();
        //slotsPanel = GameObject.Find("SlotsPanel");
        canvas = GameObject.Find("Canvas");
        
        gameSlots = GameObject.FindGameObjectsWithTag("Container");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {

        }
    }

    public void IdentifyItem(GameObject newGameItem)
    {
        newGameItemIdentified = newGameItem;
    }

    public int FindEmptySlotPosition()
    {
        for (int i = UnityEngine.Random.Range(0,gameSlots.Length) ; i < gameSlots.Length; i++)
        {
            Debug.Log("calling i" +i);
            if (gameSlots[i].GetComponent<GameSlots>().canDrop)
            {  
             return i;
            }

            if (i == gameSlots.Length -1)
            {
                for (int j = i; j >= 0; j--)
                {
                    Debug.Log("calling j " + j);
                    if (gameSlots[j].GetComponent<GameSlots>().canDrop)
                    {
                        return j;
                    }

                }
            }
        }
        return -1;
    }

    public GameObject GenerateItem(Item.ItemGenre itemGenre = Item.ItemGenre.Other, int itemLevel = 1)
    {     
        
        item = new Item() ;
        var itemName = item.CreateItemForRelevatLevel(itemLevel,itemGenre);
        item = new Item { itemType = itemName, itemGenre = itemGenre , itemLevel = itemLevel};

        GameObject newGameItem = new GameObject();
        //newGameItem.transform.SetParent(slotsPanel.transform);

       newGameItem.transform.SetParent(canvas.transform);      
        
        newGameItem.AddComponent<Image>().sprite = item.GetSprite(itemName);
        newGameItem.AddComponent<GameItems>();

        
        newGameItem.GetComponent<GameItems>().itemLevel = itemLevel;
        newGameItem.GetComponent<GameItems>().itemGenre = itemGenre;
        newGameItem.GetComponent<GameItems>().itemType = itemName;


        newGameItem.tag = item.itemType.ToString();
        newGameItem.layer = 5;

        return newGameItem;
    }

    
    public void GranaryAddGeneratedItem()
    {
        AddGeneratedItem(Item.ItemGenre.Meals);
    }

    public void ArmoryAddGeneratedItem()
    {
        AddGeneratedItem(Item.ItemGenre.Armor);
    }


    public void AddGeneratedItem(Item.ItemGenre itemGenre)
     {
        int i = FindEmptySlotPosition();

           if (i >= 0 && i <= gameSlots.Length)
           {
            
               GameObject newGameItem = GenerateItem(itemGenre);
               newGameItem.name = "GameItem" + i + 1;
               gameSlots[i].GetComponent<GameSlots>().Drop(newGameItem.GetComponent<GameItems>(), transform.position);
               IdentifyItem(newGameItem);

            
               //would be much better if made with an event - need to und how to pass the variable 
               //gameSlots[i].GetComponent<GameSlots>().canDrop = false;

               inventory.AddItemToList(item);
           }
           else
           {
            Debug.Log("There is no place left!");
           }
        
     }


}

     

