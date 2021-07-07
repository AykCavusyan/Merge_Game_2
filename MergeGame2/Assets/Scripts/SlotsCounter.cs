using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed  class SlotsCounter : MonoBehaviour
{

    private static SlotsCounter _instance;
    public static SlotsCounter Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    private GameObject[] gameSlots;

    // bunu private get yapacaktým ama debugda bile listenin içeriði gözükmedi !!
    public  List<GameObject> emptySlots = new List<GameObject>();

    public  Dictionary<GameObject, GameItems> slotDictionary = new Dictionary<GameObject, GameItems>(); // GET SET yapýacak !!!
    //public int emptySlots = 0;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            lock (_lock)
            {
                if(_instance == null)
                {
                    _instance = this;
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }

        //emptySlots = new List<GameObject>();
        gameSlots = GameObject.FindGameObjectsWithTag("Container");
    }

    private void OnEnable()
    {
        for (int i = 0; i < gameSlots.Length; i++)
        {
            slotDictionary.Add(gameSlots[i], null);
            gameSlots[i].GetComponent<GameSlots>().onSlotDischarged += RemoveFromDictonary; 
            gameSlots[i].GetComponent<GameSlots>().onSlotFilled += AddTodictionary;
        }
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

    //void AddToEmptySlotList(object sender, GameSlots.OnSlotAvailabilityEventHandler e)
    //{
    //    emptySlots.Add(e.gameSlot.get);
    //}

    //void RemoveFromEmptySlotList(object sender, GameSlots.OnSlotAvailabilityEventHandler e)
    //{
    //    emptySlots.Remove(e.gameSlot);
    //}

   void AddTodictionary(object sender, GameSlots.OnSlotAvailabilityEventHandler e)
    {
        slotDictionary[e.gameSlot] = e.gameItem;
        GenerateEMptySlotList();
    }

    void RemoveFromDictonary(object sender, GameSlots.OnSlotAvailabilityEventHandler e)
    {
        slotDictionary[e.gameSlot] = null;
        GenerateEMptySlotList();
    }

    void GenerateEMptySlotList()
    {
        emptySlots.Clear();

        foreach (KeyValuePair<GameObject,GameItems> entries  in slotDictionary)
        {
            if (entries.Value == null)
            {
                emptySlots.Add(entries.Key);
            }
        }
        
    }
}
