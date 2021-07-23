using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed  class SlotsCounter : MonoBehaviour , ISaveable , IInitializerScript
{

    private static SlotsCounter _instance;
    public static SlotsCounter Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    private GameObject[] gameSlots;

    private int initializeOrder  = 2;

    // bunu private get yapacaktým ama debugda bile listenin içeriði gözükmedi !!
    public  List<GameObject> emptySlots = new List<GameObject>();

    public  Dictionary<GameObject, GameItems> slotDictionary = new Dictionary<GameObject, GameItems>(); // GET SET yapýacak !!!
    private Dictionary<string, object> _itemsDictToLoad = new Dictionary<string, object>();
    //public int emptySlots = 0;

    public int GetInitializeOrder()
    {
        return initializeOrder;
    }

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
    }

    private void OnEnable()
    {
        SceneController.Instance.OnSceneLoaded += SceneConfig;

    }

    private void SceneConfig(object sender, SceneController.OnSceneLoadedEventArgs e)
    {
        string activeSceneName = SceneManager.GetActiveScene().name;

        if (e._sceneName == activeSceneName)
        {
            

            gameSlots = GameObject.FindGameObjectsWithTag("Container");

            for (int i = 0; i < gameSlots.Length; i++)
            {

                gameSlots[i].GetComponent<GameSlots>().onSlotDischarged += RemoveFromDictonary;
                gameSlots[i].GetComponent<GameSlots>().onSlotFilled += AddTodictionary;

                string id = gameSlots[i].GetComponent<SaveableEntitiy>().GetUniqueIdentifier();

                if (_itemsDictToLoad!=null && _itemsDictToLoad.ContainsKey(id))
                {
                    gameSlots[i].GetComponent<GameSlots>().RestoreState(_itemsDictToLoad[id]);
                }
                else
                slotDictionary.Add(gameSlots[i], null);
            }

            GenerateEMptySlotList();

            //SceneController.Instance.ModifyInitializedPanels(initializeOrder);
        }
    }


    private void OnDisable()
    {
        SceneController.Instance.OnSceneLoaded -= SceneConfig;

        if (gameSlots.Length > 0)
        {
            for (int i = 0; i < gameSlots.Length; i++)
            {
                if (gameSlots[i]) gameSlots[i].GetComponent<GameSlots>().onSlotDischarged -= RemoveFromDictonary;
                if (gameSlots[i]) gameSlots[i].GetComponent<GameSlots>().onSlotFilled -= AddTodictionary;
            }
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

        _itemsDictToLoad = (Dictionary<string, object>)state;

        //Dictionary<string, object> _slotSaveDictIN = (Dictionary<string, object>)state;

       //foreach (GameSlots gameslot in FindObjectsOfType<GameSlots>())
        //{
           // string id = gameslot.GetComponent<SaveableEntitiy>().GetUniqueIdentifier();

            //if (_slotSaveDictIN.ContainsKey(id))
            //{
              //  gameslot.RestoreState(_slotSaveDictIN[id]);
           // }
    //}


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
