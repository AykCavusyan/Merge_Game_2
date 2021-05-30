
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    public bool[] canDrop ;
    public GameObject[] slots;
    private List<Item> itemList;
   

    

    // Start is called before the first frame update
    void Start()
    {
        slots = GameObject.FindGameObjectsWithTag("Container");
        canDrop = new bool[slots.Length];
        for (int i = 0; i < canDrop.Length; i++)
        {
            canDrop[i] = slots[i].GetComponent<GameSlots>().canDrop;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(itemList.Count);
    }

    public Inventory()
    {
        itemList = new List<Item>();
    }



    public void AddItemToList(Item item)
    {
        itemList.Add(item);
        
    }

    public bool CanAdd(int i)
    {
        return slots[i].GetComponent<GameSlots>().canDrop;
    }

  


}
