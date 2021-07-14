using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MasterEventListener : MonoBehaviour 
{
    private static MasterEventListener _instance;
    public static MasterEventListener Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    //private GameObject player;
    //private GameObject[] gameSlots;

    private ItemBag itemBag;
        
    public event EventHandler<GameItems.OnMergedEventArgs> OnMerged;
    public event EventHandler<GameItems.OnItemCollectedEventArgs> OnItemCollectted;
    public event EventHandler<GameItems.OnItemDestroyedEventArgs> OnDestroyedMasterEvent;
    public event EventHandler<GameItems.OnQuestItemEventArgs> OnItemIsQuestEventMaster;

    #region
    //private VisualEffects visualEffects;
    //private ScoreManager scoreManager;
    #endregion
    void Awake()
    {
        if (_instance != null & _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = this;
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }

      

      //player = GameObject.FindGameObjectWithTag("Player");
      //gameSlots = GameObject.FindGameObjectsWithTag("Container");

      #region
        //visualEffects = GameObject.FindGameObjectWithTag("ParticleSystem").GetComponent<VisualEffects>() ;
        //scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
        //Debug.Log("static working");
        #endregion
    }
    void OnEnable()
    {
        //Init();
        //for (int i = 0; i < gameSlots.Length; i++)
        //{
        //   gameSlots[i].GetComponent<GameSlots>().OnDropped += OnGameItemAdded;
        //}
    }



    void OnDisable()
    {

        ItemBag.Instance.OnGameItemCreated -= OnGameItemAdded;

        //if (gameSlots != null)
        //{
        //   for (int i = 0; i < gameSlots.Length; i++)
        //   {
        //      gameSlots[i].GetComponent<GameSlots>().OnDropped -= OnGameItemAdded;
        //   }
        //}
    }

    private void Start()
    {
        itemBag = GetComponent<ItemBag>();
        ItemBag.Instance.OnGameItemCreated += OnGameItemAdded;
    }

    //void Init()
    //{
    //    if (itemBag == null)
    //    {
    //        gameObject.AddComponent<ItemBag>();
    //    }
    //}

    void OnGameItemAdded(object sender, ItemBag.OnGameItemCreatedEventArgs e)
    {
        e.gameItem.OnMerged += MergeEvent;
        e.gameItem.OnItemCollected += ItemCollectEvent;
        //e.gameItem.OnQuestCheckmarkOn += ItemIsQuestItemEvent;
        e.gameItem.OnItemDestroyed += StopListeningDeadGameIteEvent;

        //e.gameItem.OnItemDestroyed += OnGameItemDestroyed;
    }


    void MergeEvent(object sender, GameItems.OnMergedEventArgs e)
    {
        OnMerged?.Invoke(sender, e);
    }

    void ItemCollectEvent(object sender, GameItems.OnItemCollectedEventArgs e)
    {
        OnItemCollectted?.Invoke(sender, e);
        
    }

    void ItemIsQuestItemEvent(object sender, GameItems.OnQuestItemEventArgs e)
    {
        OnItemIsQuestEventMaster?.Invoke(sender, e);
    }

    void StopListeningDeadGameIteEvent(object sender, GameItems.OnItemDestroyedEventArgs e)
    {
        OnDestroyedMasterEvent?.Invoke(sender, e);

        e.gameItems.OnMerged -= MergeEvent;
        e.gameItems.OnItemCollected -= ItemCollectEvent;
        //e.gameItems.OnQuestCheckmarkOn -= ItemIsQuestItemEvent;
        e.gameItems.OnItemDestroyed -= StopListeningDeadGameIteEvent;
    }

 
}
