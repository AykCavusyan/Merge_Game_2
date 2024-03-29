using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour , ISaveable, IInitializerScript
{
    private static QuestManager _instance;
    public static QuestManager Instance { get { return _instance; } }
    private static readonly object _lock = new object(); // bu mu yoksa b�y�k harf class olan m� ??????

    //private int initializeOrder;

    private Quest newQuest;
    private GameObject questPanel;
    private GameObject rewardPanel;
    public List<Quest> _activeQuests { get; private set; } = new List<Quest>();
    public List<Quest> _inactiveQuests { get; private set; } = new List<Quest>();  
    public List<GameItems> _presentGameItems { get; private set; } = new List<GameItems>(); 

    public List<Item.ItemType> _activeQuestItemsList { get; private set; } = new List<Item.ItemType>();

    public List<(int , int )> activeLevelTuplesToLoadList = new List<(int , int )>();

    public event EventHandler<OnQuestAddRemoveEventArgs> OnQuestCompleted;
    public event EventHandler<OnQuestAddRemoveEventArgs> OnQuestAdded;
    public event EventHandler<OnQuestAddRemoveEventArgs> OnItemIsNotQuestItemAnymore;
    public class OnQuestAddRemoveEventArgs
    {
        public Quest quest;
        public Item.ItemType itemType = Item.ItemType.None;
        public Button_CompleteQuest button_CompleteQuest;
    }
    public event EventHandler<AddRemoveQuestItemEventArgs> OnQuestItemNoMore;
    public class AddRemoveQuestItemEventArgs
    {
        public Item.ItemType itemType;
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
        ItemBag itemBag = GetComponent<ItemBag>(); // bu laz�m m� ??     
    }

    private void OnEnable()
    {
        ItemBag.Instance.OnGameItemCreated += AddPresentGameItemsList;
        MasterEventListener.Instance.OnDestroyedMasterEvent += DestroyedItemcheck;
        SceneController.Instance.OnSceneLoaded += SceneConfig;     
    }

    private void SceneConfig(object sender, SceneController.OnSceneLoadedEventArgs e )
    {        
        if(e._sceneToLoad == SceneNames.Scene.MergeScene && e.initializeOrder ==1)
        {
            questPanel = GameObject.Find("Panel_QuestPanel");
            rewardPanel = GameObject.Find("Panel_LevelPanel");

            rewardPanel.GetComponent<Rewards>().OnRewardItemGiven += AddPresentRewardItemsList; // bunu scene change de disable etmek laz�m

            if (activeLevelTuplesToLoadList.Count > 0)
            {
                foreach ((int,int) activeQuestLevelTuple in activeLevelTuplesToLoadList)
                {
                    GenerateNewQuest(activeQuestLevelTuple.Item1, activeQuestLevelTuple.Item2);
                }
            }
        }
    }




    private void OnDisable()
    {
        ItemBag.Instance.OnGameItemCreated -= AddPresentGameItemsList;
        MasterEventListener.Instance.OnDestroyedMasterEvent -= DestroyedItemcheck;
        SceneController.Instance.OnSceneLoaded -= SceneConfig;
        if (rewardPanel) rewardPanel.GetComponent<Rewards>().OnRewardItemGiven -= AddPresentRewardItemsList;
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
        Button_CompleteQuest button_CompleteQuest = newParentQuestContainer.transform.GetChild(3).GetChild(1).GetComponent<Button_CompleteQuest>();
        button_CompleteQuest.OnQuestCompleted += CompleteQuest;

        OnQuestAddRemoveEventArgs args = new OnQuestAddRemoveEventArgs { button_CompleteQuest = button_CompleteQuest } ;
        OnQuestAdded?.Invoke(this, args);
        Debug.Log(args.itemType);
    }

    void AddPresentGameItemsList(object  sender, ItemBag.OnGameItemCreatedEventArgs e)
    {
        if (e.gameItem.isRewardPanelItem == false)
        {
            _presentGameItems.Add(e.gameItem);
        }
        
    }

    void AddPresentRewardItemsList(GameItems itemfrommRewardPanel)
    {
        if(itemfrommRewardPanel.isRewardPanelItem == false)
        {
            _presentGameItems.Add(itemfrommRewardPanel);
        }
    }


    private void CompleteQuest(object sender, Button_CompleteQuest.OnQuestCompletedEventArgs e)
    {
        _activeQuests.Remove(e.quest);
        _inactiveQuests.Add(e.quest);
        RemoveFromQuestItemsList(e.quest);

        Button_CompleteQuest completeButton = (Button_CompleteQuest)sender;
        completeButton.OnQuestCompleted -= CompleteQuest;

        OnQuestCompleted?.Invoke(this, new OnQuestAddRemoveEventArgs { quest = e.quest, button_CompleteQuest = completeButton });

        //questPanel.GetComponent<Quest_List>().TransferCompletedParentSlotContainers(e.quest);

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
                OnItemIsNotQuestItemAnymore?.Invoke(this, new OnQuestAddRemoveEventArgs { itemType = questItem.itemType });
            }
        }
    }

    private void CheckAndFlagExistingItems(Quest newQuest)
    {
        foreach (Item questItem in newQuest.itemsNeeded)
        {
            Debug.Log(questItem.itemType);
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

    public object CaptureState()
    {
        Dictionary<string, object> _variablesDict = new Dictionary<string, object>();

        List<(int , int )> _activeQuestLevelsTupleList = new List<(int , int )>(); 
        foreach (Quest activeQuest in _activeQuests)
        {
            _activeQuestLevelsTupleList.Add((activeQuest.zoneNumber, activeQuest.taskNumber));
        }
        _variablesDict.Add("activeQuestLevelsTupleList",_activeQuestLevelsTupleList);

        return _variablesDict;
    }

    public void RestoreState(object state)
    {
        Dictionary<string, object> _variablesDictIN = (Dictionary<string, object>)state;

        activeLevelTuplesToLoadList = (List<(int,int)>)_variablesDictIN["activeQuestLevelsTupleList"];
    }
}
