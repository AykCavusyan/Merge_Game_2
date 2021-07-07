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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GenerateNewQuest(1, 1);
        }
    }


    void GenerateNewQuest(int zoneNumber, int taskNumber)
    {
        _quest = new Quest(zoneNumber, taskNumber);
        _activeQuests.Add(_quest);
        AddToQuestItemsList(_quest);
    }


    //private void ListActiveQuests(Quest _activeQuest)
    //{
    //    _activeQuests.Add(_activeQuest);
    //}

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
            if (!_activeQuestItemsList.Contains(questItem.itemType))
            {
                FlagSlotItems(questItem.itemType);
            }
            _activeQuestItemsList.Add(questItem.itemType);
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

  
    // check all active items and flag if they are questitems
    void FlagSlotItems(Item.ItemType itemTypeToFlag)
    {
        var existingSlotItemsList = SlotsCounter.Instance.slotDictionary.Values.Where(x=> x!=null).ToList();

        foreach (GameItems gameItem in existingSlotItemsList)
        {
            if(gameItem.isQuestItem == false && gameItem.itemType == itemTypeToFlag)
            {
               gameItem.isQuestItem = true;
            }
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


    // yeni  gelen itemi dinleyecek EVENTLÝSTENER !!
    void AddedItemCheck(GameItems newDroppedItem)
    {
        if (_activeQuestItemsList.Contains(newDroppedItem.itemType))
        {
            newDroppedItem.isQuestItem = true;
        }
    }

  

}
