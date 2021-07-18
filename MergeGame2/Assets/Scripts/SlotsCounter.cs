using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed  class SlotsCounter : MonoBehaviour , ISaveable
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

    private void OnDisable()
    {
        for (int i = 0; i < gameSlots.Length; i++)
        {
            if(gameSlots[i]) gameSlots[i].GetComponent<GameSlots>().onSlotDischarged -= RemoveFromDictonary;
            if(gameSlots[i]) gameSlots[i].GetComponent<GameSlots>().onSlotFilled -= AddTodictionary;
        }
    }

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

    public object CaptureState()
    {
        Dictionary<string, object> _slotSaveDict = new Dictionary<string, object>();

        foreach (KeyValuePair<GameObject, GameItems> pair in slotDictionary)
        {
            if (pair.Value != null)
            {
                _slotSaveDict.Add(pair.Key.GetComponent<SaveableEntitiy>().GetUniqueIdentifier(), pair.Value.CaptureState());
            }
        }
        return _slotSaveDict;

        //foreach (KeyValuePair<GameObject,GameItems> pair in slotDictionary)
        //{
        //    _slotSaveDict.Add(pair.Key.GetComponent<SaveableEntitiy>().GetUniqueIdentifier(), pair.Value?.CaptureState() ?? null);
        //}

        //return _slotSaveDict;
    }

    public void RestoreState(object state)
    {
        Debug.Log("restorestate of counter is working");

        Dictionary<string, object> _slotSaveDictIN = (Dictionary<string, object>)state;

        foreach(GameSlots gameslot in FindObjectsOfType<GameSlots>())
        {
            string id = gameslot.GetComponent<SaveableEntitiy>().GetUniqueIdentifier();

            if (_slotSaveDictIN.ContainsKey(id))
            {
                gameslot.RestoreState(_slotSaveDictIN[id]);
            }
        }


        //foreach(KeyValuePair<GameObject, GameItems> pair in slotDictionary)
        //{
        //    string id = pair.Key.GetComponent<SaveableEntitiy>().GetUniqueIdentifier();

        //    if (_slotSaveDictIN.ContainsKey(id))
        //    {
        //        pair.Key.GetComponent<GameSlots>().RestoreState(_slotSaveDictIN[id]);
        //    }
        //}


        //foreach (string slotUniqueID in _slotSaveDictIN.Keys)
        //{
        //    // bu kýsým daha sona linQ ile yapýlsa daha temiz
        //    foreach (KeyValuePair<GameObject, GameItems> pair in slotDictionary)
        //    {
        //        if(pair.Key.GetComponent<SaveableEntitiy>().GetUniqueIdentifier() == slotUniqueID && pair.Value !=null)
        //        {
        //            pair.Key.GetComponent<GameSlots>().RestoreState(_slotSaveDictIN[slotUniqueID]);
        //        }
        //    }

        //}
    }
}
