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

    private Quest newQuest;
    private GameObject questPanel;
    public List<Quest> _activeQuests { get; private set; } = new List<Quest>();
    public List<Quest> _inactiveQuests { get; private set; } = new List<Quest>();  
    public List<GameItems> _presentGameItems { get; private set; } = new List<GameItems>(); 

    public List<Item.ItemType> _activeQuestItemsList { get; private set; } = new List<Item.ItemType>();

    public event Action<Quest> OnQuestCompleted;

    public event EventHandler<OnQuestAddRemoveEventArgs> OnQuestAdded;
    public event EventHandler<OnQuestAddRemoveEventArgs> OnQuestRemoved;
    public class OnQuestAddRemoveEventArgs
    {
        //public Quest quest;
        public Item.ItemType itemType;
    }

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

        ItemBag itemBag = GetComponent<ItemBag>();
        questPanel = GameObject.Find("Panel_QuestPanel");
    }

    private void OnEnable()
    {
        ItemBag.Instance.OnGameItemCreated += AddPresentGameItemsList;
        MasterEventListener.Instance.OnDestroyedMasterEvent += DestroyedItemcheck;
    }


    private void OnDisable()
    {
        ItemBag.Instance.OnGameItemCreated -= AddPresentGameItemsList;
        MasterEventListener.Instance.OnDestroyedMasterEvent -= DestroyedItemcheck;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GenerateNewQuest(1, 1);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GenerateNewQuest(1, 2);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            GenerateNewQuest(1, 3);
        }
    }

    void GenerateNewQuest(int zoneNumber, int taskNumber)
    {
        newQuest = new Quest(zoneNumber, taskNumber);
        _activeQuests.Add(newQuest);
        CheckAndFlagExistingItems(newQuest);
        GameObject newParentQuestContainer = questPanel.GetComponent<Quest_List>().InstantiateParentQuestContainers(newQuest);
        newParentQuestContainer.transform.GetChild(3).GetChild(1).GetComponent<Button_CompleteQuest>().OnQuestCompleted += CompleteQuest;
    }

    void AddPresentGameItemsList(object  sender, ItemBag.OnGameItemCreatedEventArgs e)
    {
        if(e.gameItem.isRewardPanelItem == false)
        {
            _presentGameItems.Add(e.gameItem);
            Debug.Log(_presentGameItems.Count);
        }
    }


    private void CompleteQuest(object sender, Button_CompleteQuest.OnQuestCompletedEventArgs e)
    {
        _activeQuests.Remove(e.quest);
        _inactiveQuests.Add(e.quest);
        RemoveFromQuestItemsList(e.quest);

        Button_CompleteQuest completeButton = (Button_CompleteQuest)sender;
        completeButton.OnQuestCompleted -= CompleteQuest;

        questPanel.GetComponent<Quest_List>().RemoveParentSlotContainers(e.quest);

        OnQuestCompleted?.Invoke(e.quest);

        foreach (Item item in e.quest.itemsNeeded)
        {
            GameItems gameItemToDestroy = _presentGameItems.FirstOrDefault(gameItem => gameItem.itemType == item.itemType);
            gameItemToDestroy.DestroyItem(gameItemToDestroy.gameObject);
        }
    }

    private void RemoveFromQuestItemsList(Quest completedQuest)
    {
        foreach (Item questItem in completedQuest.itemsNeeded)
        {
            _activeQuestItemsList.Remove(questItem.itemType);

            if(!_activeQuestItemsList.Any(itemType => itemType == questItem.itemType))
            {
                OnQuestRemoved?.Invoke(this, new OnQuestAddRemoveEventArgs { itemType = questItem.itemType });
            }
        }
    }

    private void CheckAndFlagExistingItems(Quest newQuest)
    {
        foreach (Item questItem in newQuest.itemsNeeded)
        {
            OnQuestAdded?.Invoke(this, new OnQuestAddRemoveEventArgs { itemType = questItem.itemType });
            _activeQuestItemsList.Add(questItem.itemType);
        }
    }

    void DestroyedItemcheck(object sender, GameItems.OnItemDestroyedEventArgs e)
    {
        if(e.gameItems.isRewardPanelItem == false)
        {
            _presentGameItems.Remove(e.gameItems);
            
            if(!_presentGameItems.Any(gameItems => gameItems.itemType == e.gameItems.itemType))
            {
                OnQuestItemNoMore?.Invoke(this, new AddRemoveQuestItemEventArgs { itemType = e.gameItems.itemType });
            }

        }

    }

}
