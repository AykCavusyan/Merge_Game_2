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
    public List<GameObject> emptySlots;

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

        emptySlots = new List<GameObject>();
        gameSlots = GameObject.FindGameObjectsWithTag("Container");
    }

    private void OnEnable()
    {
        for (int i = 0; i < gameSlots.Length; i++)
        {
            gameSlots[i].GetComponent<GameSlots>().onSlotDischarged += AddToEmptySlotList; 
            gameSlots[i].GetComponent<GameSlots>().onSlotFilled += RemoveFromEmptySlotList;
        }
    }

    

    void Start()
    {
       // SlotTracker();
    }

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
    }

    void RemoveFromEmptySlotList(object sender, GameSlots.OnSlotAvailabilityEventHandler e)
    {
        emptySlots.Remove(e.gameSlot);
    }
}
