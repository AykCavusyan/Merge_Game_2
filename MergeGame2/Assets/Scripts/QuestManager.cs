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
    public  List<Quest> _activeQuests = new List<Quest>(); // get set koyulacak !!!!
    public List<Quest> _inactiveQuests = new List<Quest>();  // get set koyulacak !!!
    [SerializeField] public List<GameItems> _presentGameItems = new List<GameItems>(); // get set konulacak !!!

    public List<Item.ItemType> _activeQuestItemsList { get; private set; } = new List<Item.ItemType>();

    public event EventHandler<OnQuestAddRemoveEventArgs> OnQuestAdded;
    public event EventHandler<OnQuestAddRemoveEventArgs> OnQuestRemoved;
    public class OnQuestAddRemoveEventArgs
    {
        //public Quest quest;
        public Item.ItemType itemType;
    }

    //public event EventHandler<AddRemoveQuestItemEventArgs> OnQuestItemExists;
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

    private void Start()
    {
        //ItemBag.Instance.OnGameItemCreated += CreatedItemCheck;
        /// bu ve altýndaki awake e alýnabilir mi ??
    }
    private void OnDisable()
    {
        //ItemBag.Instance.OnGameItemCreated -= CreatedItemCheck;
        ItemBag.Instance.OnGameItemCreated -= AddPresentGameItemsList;
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
        newQuest = new Quest(zoneNumber, taskNumber);
        _activeQuests.Add(newQuest);
        CheckAndFlagExistingItems(newQuest);
        questPanel.GetComponent<Quest_List>().InstantiateParentQuestContainers(newQuest);
        //OnQuestAdded?.Invoke(this, new OnQuestAddRemoveEventArgs {quest = newQuest });
    }

    void AddPresentGameItemsList(object  sender, ItemBag.OnGameItemCreatedEventArgs e)
    {
        if(e.gameItem.isRewardPanelItem == false)
        {
            _presentGameItems.Add(e.gameItem);
            Debug.Log(_presentGameItems.Count);
        }
    }

    private void RemoveFromQuestItemsList(Quest completedQuest)
    {
        foreach (Item questItem in completedQuest.itemsNeeded)
        {
            if (_activeQuestItemsList.Count(x => _activeQuestItemsList.Contains(questItem.itemType)) < 2)
            {
                OnQuestRemoved?.Invoke(this, new OnQuestAddRemoveEventArgs { itemType = questItem.itemType });
                //DeFlagSlotItems(questItem.itemType);
            }
            _activeQuestItemsList.Remove(questItem.itemType);
        }

    }

    // EVENT LÝSTENER OF COMPLETE
    private void CompleteQuest(Quest _completedQuest)
    {
        _activeQuests.Remove(_completedQuest);
        _inactiveQuests.Add(_completedQuest);
        RemoveFromQuestItemsList(_completedQuest);
    }

    
    // this will listen to the new questitems // TAKE FROM QUEST LIST !!!!
    private void CheckAndFlagExistingItems(Quest newQuest)
    {
        foreach (Item questItem in newQuest.itemsNeeded)
        {
            //FlagSlotItems(questItem.itemType);

            OnQuestAdded?.Invoke(this, new OnQuestAddRemoveEventArgs { itemType = questItem.itemType });
            _activeQuestItemsList.Add(questItem.itemType);
        }
    }

    //void FlagSlotItems(Item.ItemType itemTypeToFlag)
    //{
    //    var existingSlotItemsList = SlotsCounter.Instance.slotDictionary.Values.Where(x => x != null).ToList();

    //    //bool itemExistCheck = true;
    //    for (int i = 0; i < existingSlotItemsList.Count; i++)
    //    {
    //        //if (itemExistCheck && existingSlotItemsList[i].itemType == itemTypeToFlag)
    //        //{
    //        //    Debug.Log("Event is calling");
    //        //    OnQuestItemExists?.Invoke(this, new AddRemoveQuestItemEventArgs { itemType = itemTypeToFlag });
    //        //    itemExistCheck = false;
    //        //}

    //        if (existingSlotItemsList[i].isQuestItem == false && existingSlotItemsList[i].itemType == itemTypeToFlag)
    //        {
    //            OnQuestItemExists?.Invoke(this, new AddRemoveQuestItemEventArgs { itemType = itemTypeToFlag });
    //            //existingSlotItemsList[i].isQuestItem = true;
    //            //existingSlotItemsList[i].SetCheckMark(true);
    //            break;
    //        }
    //    }
    //}

    // remove the type from the list 
  

    //void DeFlagSlotItems(Item.ItemType itemTypeToDeflag)
    //{
    //    var existingSlotItemsList = SlotsCounter.Instance.slotDictionary.Values.Where(x => x != null).ToList();

    //    foreach (GameItems gameItem in existingSlotItemsList)
    //    {
    //        if(gameItem.isQuestItem == true && gameItem.itemType == itemTypeToDeflag)
    //        {
    //            gameItem.isQuestItem = false;
    //            gameItem.SetCheckMark(false);
    //        }
    //    }   

    //}

    //void CreatedItemCheck(object sender, ItemBag.OnGameItemCreatedEventArgs e)
    //{
    //    if (_activeQuestItemsList.Contains(e.gameItem.itemType))
    //    {
    //        OnQuestItemExists?.Invoke(this, new AddRemoveQuestItemEventArgs { itemType = itemTypeToFlag });

    //        //e.gameItem.isQuestItem = true;
    //        //e.gameItem.SetCheckMark(true);

    //        //OnQuestItemExists?.Invoke(this, new AddRemoveQuestItemEventArgs { itemType = e.gameItem.itemType });
    //    }
    //}

    void DestroyedItemcheck(object sender, GameItems.OnItemDestroyedEventArgs e)
    {
        if(e.gameItems.isRewardPanelItem == false)
        {
            _presentGameItems.Remove(e.gameItems);
            Debug.Log(_presentGameItems.Count);
        }

        foreach (GameItems gameItem in _presentGameItems)
        {
            if (gameItem.itemType == e.gameItems.itemType)
            {
                return;
            }
            else
            {
                Debug.Log("nomore is working");
                OnQuestItemNoMore?.Invoke(this, new AddRemoveQuestItemEventArgs { itemType = e.gameItems.itemType });
            }
        }

        //var existingSlotItemsList = SlotsCounter.Instance.slotDictionary.Values.Where(x => x != null).ToList();

        //Debug.Log(existingSlotItemsList.Count(x => existingSlotItemsList.Contains(e.gameItems)));


        //if (existingSlotItemsList.Count(x => existingSlotItemsList.Contains(e.gameItems)) < 3)
        //{

        //    OnQuestItemNoMore?.Invoke(this, new AddRemoveQuestItemEventArgs { itemType = e.gameItems.itemType });
        //}
    }

}
