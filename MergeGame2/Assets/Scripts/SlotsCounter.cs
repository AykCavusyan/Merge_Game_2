using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotsCounter : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject[] gameSlots;


    // bunu private get yapacaktým ama debugda bile listenin içeriði gözükmedi !!
    public List<GameObject> emptySlots;

    private void Awake()
    {
        emptySlots = new List<GameObject>();
        gameSlots = GameObject.FindGameObjectsWithTag("Container");
    }

    private void OnEnable()
    {
        for (int i = 0; i < gameSlots.Length; i++)
        {
            gameSlots[i].GetComponent<GameSlots>().onSlotDischarged += AddToEmptySlotList; 
            gameSlots[i].GetComponent<GameSlots>().onSlotFilled += RemoveFromEmptySlotList;
            Debug.Log("slots are filling");
        }
    }

    

    void Start()
    {
       // SlotTracker();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //void SlotTracker()
    //{
    //    for (int i = 0; i < gameSlots.Length; i++)
    //    {
    //        if (gameSlots[i].GetComponent<GameSlots>().canDrop == true)
    //        {
    //            emptySlots.Add(gameSlots[i]);
    //        }
    //        Debug.Log(emptySlots);
    //    }
    //}

    void AddToEmptySlotList(object sender, GameSlots.OnSlotAvailabilityEventHandler e)
    {
        emptySlots.Add(e.gameSlot);
        Debug.Log(emptySlots);
    }

    void RemoveFromEmptySlotList(object sender, GameSlots.OnSlotAvailabilityEventHandler e)
    {
        emptySlots.Remove(e.gameSlot);
        Debug.Log("remove event  called");
    }
}
