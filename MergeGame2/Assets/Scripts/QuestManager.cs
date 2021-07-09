using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private static QuestManager _instance;
    public static QuestManager Instance { get { return _instance; } }

    private static readonly object _lock = new object(); // bu mu yoksa büyük harf class olan mý ??????

    private Quest _quest;
    public  List<Quest> _activeQuests; // get set koyulacak !!!!
    public  List<Quest> _inactiveQuests;  // get set koyulacak !!!

    private List<Item.ItemType> _activeQuestItemsList = new List<Item.ItemType>();

    public event EventHandler<OnQuestAddedEventArgs> OnQuestAdded;
    public class OnQuestAddedEventArgs
    {
        public Quest quest;
    }

    public event EventHandler<AddRemoveQuestItemEventArgs> OnQuestItemExists;
    public event EventHandler<AddRemoveQuestItemEventArgs> OnQuestItemNoMore;
    public class AddRemoveQuestItemEventArgs
    {
        public Item.ItemType itemType;
    }



    private void Awake()
    {
        if(_instance != null && _instance != this)
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
    }

    private void Start()
    {
        ItemBag itemBag = GetComponent<ItemBag>();
        ItemBag.Instance.OnGameItemCreated += CreatedItemCheck;
        MasterEventListener.Instance.OnDestroyedMasterEvent += DestroyedItemcheck;

    }
    private void OnDisable()
    {
        ItemBag.Instance.OnGameItemCreated -= CreatedItemCheck;
        MasterEventListener.Instance.OnDestroyedMasterEvent -= DestroyedItemcheck;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GenerateNewQuest(1, 1);
        }
    }

    // loaddda active quest listten bir bir zone ve tasknumber a göre alacak
    void GenerateNewQuest(int zoneNumber, int taskNumber)
    {
        _quest = new Quest(zoneNumber, taskNumber);
        _activeQuests.Add(_quest);
        AddToQuestItemsList(_quest);
        OnQuestAdded?.Invoke(this, new OnQuestAddedEventArgs {quest = _quest });
    }

    //private void ListActiveQuests(Quest _activeQuest)
    //{
    //    _activeQuests.Add(_activeQuest);
    //}

    // EVENT LÝSTENER OF COMPLETE
    private void CompleteQuest(Quest _completedQuest)
    {
        _activeQuests.Remove(_completedQuest);
        _inactiveQuests.Add(_completedQuest);
        RemoveFromQuestItemsList(_completedQuest);
    }

    //private void ListInactiveQuests(Quest _completedQuest)
    //{
    //    _inactiveQuests.Add(_completedQuest);
    //}


    
    // this will listen to the new questitems // TAKE FROM QUEST LIST !!!!
    private void AddToQuestItemsList(Quest newQuest)
    {
        foreach (Item questItem in newQuest.itemsNeeded)
        {
            FlagSlotItems(questItem.itemType);
            _activeQuestItemsList.Add(questItem.itemType);
        }
    }

    void FlagSlotItems(Item.ItemType itemTypeToFlag)
    {
        var existingSlotItemsList = SlotsCounter.Instance.slotDictionary.Values.Where(x => x != null).ToList();

        bool itemExistCheck = true;
        for (int i = 0; i < existingSlotItemsList.Count; i++)
        {
            if (itemExistCheck && existingSlotItemsList[i].itemType == itemTypeToFlag)
            {
                OnQuestItemExists?.Invoke(this, new AddRemoveQuestItemEventArgs { itemType = itemTypeToFlag });
                itemExistCheck = false;
            }

            if (existingSlotItemsList[i].isQuestItem == false && existingSlotItemsList[i].itemType == itemTypeToFlag)
            {
                existingSlotItemsList[i].isQuestItem = true;
            }
        }
    }

    // remove the type from the list 
    private void RemoveFromQuestItemsList(Quest completedQuest)
    {
        foreach (Item questItem in completedQuest.itemsNeeded)
        {
            if(_activeQuestItemsList.Count(x => _activeQuestItemsList.Contains(questItem.itemType)) < 2 )
            {
                DeFlagSlotItems(questItem.itemType);
            }
            _activeQuestItemsList.Remove(questItem.itemType);
        }

    }

    void DeFlagSlotItems(Item.ItemType itemTypeToDeflag)
    {
        var existingSlotItemsList = SlotsCounter.Instance.slotDictionary.Values.Where(x => x != null).ToList();

        foreach (GameItems gameItem in existingSlotItemsList)
        {
            if(gameItem.isQuestItem == true && gameItem.itemType == itemTypeToDeflag)
            {
                gameItem.isQuestItem = false;
            }
        }   

    }

    void CreatedItemCheck(object sender, ItemBag.OnGameItemCreatedEventArgs e)
    {
        if (_activeQuestItemsList.Contains(e.gameItem.itemType))
        {
            e.gameItem.isQuestItem = true;

            OnQuestItemExists?.Invoke(this, new AddRemoveQuestItemEventArgs { itemType = e.gameItem.itemType });
        }
    }

    void DestroyedItemcheck(object sender, GameItems.OnItemDestroyedEventArgs e)
    {
        var existingSlotItemsList = SlotsCounter.Instance.slotDictionary.Values.Where(x => x != null).ToList();

        if (existingSlotItemsList.Count(x => existingSlotItemsList.Contains(e.gameItems)) < 1)
        {
            OnQuestItemNoMore?.Invoke(this, new AddRemoveQuestItemEventArgs { itemType = e.gameItems.itemType });
        }
    }

}
