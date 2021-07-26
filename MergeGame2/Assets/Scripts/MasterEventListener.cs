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

    public class OnFinancialEvent
    {
        public InventorySlots inventorySlot;
        public int inventorySlotCost;
        public int itemValue;
    }

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
    }
    void OnDisable()
    {
        ItemBag.Instance.OnGameItemCreated -= OnGameItemAdded;
    }

    private void Start()
    {
        itemBag = GetComponent<ItemBag>();
        ItemBag.Instance.OnGameItemCreated += OnGameItemAdded;
    }

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
