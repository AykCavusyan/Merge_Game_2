using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class PlayerInfo : MonoBehaviour , ISaveable, IInitializerScript
{
    private static PlayerInfo _instance;
    public static PlayerInfo Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    //private int initializeOrder;

    private GameObject levelPanel;
    private Button_Claim button_Claim;
    private Button_Action_ItemInfo buttonActionItemInfo;
    private Inventory inventoryScript;

    private Dictionary<InventorySlots, GameItems> inventory = new Dictionary<InventorySlots, GameItems>();

    private Dictionary<int, object> _itemsDictToLoad = new Dictionary<int, object>();

    public List<InventorySlots> emptySlots = new List<InventorySlots>();

    public int remainingInventorySlotAmount;
    public int currentInventorySlotAmount { get; private set; }
    public int maxInventorySlotAmount { get; private set; }

    private List<int> listOfRewardLevelsToClaim = new List<int>();

    public int currentGold { get; private set; } = 10;
    public int currentXP { get; private set; }
    public int XPToNextLevel { get; private set; }
    public int currentLevel { get; private set; }
    public int lastClaimedLevel { get; private set; } = 1;
    //private GameObject levelBar;
    
    //private MasterEventListener masterEventListener; // bu gerekli mi bakmak laz�m

    public event EventHandler<OnLevelChangedEventArgs> OnResetBar;
    public event EventHandler<OnLevelChangedEventArgs> OnLevelTextChanged;
    public event EventHandler<OnLevelChangedEventArgs> OnLevelNumberChanged;
    public class OnLevelChangedEventArgs
    {
        public string levelText;
        public int currentXP;
        public int xpToNextLevel;
    }

    public event Action<EventArgs> OnCurrentGoldChanged;


    //public int  GetInitializeOrder()
    //{
    //    return initializeOrder;
    //}


    private void Awake()
    {
        if (_instance != null && _instance != this) //BUNU SINGLETON PATLADI�I ���N S�LD�M - YAPMAK LAZIM
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

        currentInventorySlotAmount = 5;
        maxInventorySlotAmount = 28;

        SetCurrentLevel();

        //masterEventListener = GetComponent<MasterEventListener>();
    }
    
    //private void OnEnable()
    //{
        
    //}

    private void Start()
    {
        SceneController.Instance.OnSceneLoaded += SceneConfig;
        MasterEventListener.Instance.OnItemCollectted += CalculateXPFromStars;
        QuestManager.Instance.OnQuestCompleted += CalculateXPFromQuests; // singletonu patat�yor diye burada normal yerine almak laz�m
    }
    private void OnDisable()
    {
        SceneController.Instance.OnSceneLoaded -= SceneConfig;
        MasterEventListener.Instance.OnItemCollectted -= CalculateXPFromStars;
        QuestManager.Instance.OnQuestCompleted -= CalculateXPFromQuests;

        if (levelPanel) levelPanel.GetComponent<Rewards>().OnListOfRewardLevelToClaimUpdated -= UpdateListOfRewardLevelsToClaim;
        if (button_Claim) button_Claim.OnClaimed -= UpdateLastLevelClaimed;
        if (buttonActionItemInfo) buttonActionItemInfo.OnItemSold -= CalculateCurrentGold;

        InventorySlots[] inventorySlots = FindObjectsOfType<InventorySlots>(); 
        if (inventorySlots.Length > 0)
        {
            foreach (InventorySlots inventorySlot in inventorySlots)
            {
                if (inventorySlot)
                {
                    inventorySlot.onInventoryPlacedItem -= AddToDictionnary;
                    inventorySlot.onInventoryRemovedItem -= RemoveFromDictionnary;
                    inventorySlot.OnSlotPurchased -= CalculateCurrentGold;
                } 
            }
        }
        
    }

    private void SceneConfig(object sender, SceneController.OnSceneLoadedEventArgs e)
    {       

        if (e._sceneToLoad == SceneNames.Scene.MergeScene && e.initializeOrder ==2)
        {
            inventoryScript = GameObject.Find("Panel_BackToGame").GetComponent<Inventory>();
            inventoryScript.OnInventorySlotCreated += ListenInventorySlots;
            inventoryScript.ConfigPanel(_itemsDictToLoad);
            

            levelPanel = GameObject.Find("Panel_LevelPanel");
            button_Claim = levelPanel.transform.GetChild(3).GetComponent<Button_Claim>();

            levelPanel.GetComponent<Rewards>().ConfigPanel(listOfRewardLevelsToClaim, currentLevel);
            levelPanel.GetComponent<Rewards>().OnListOfRewardLevelToClaimUpdated += UpdateListOfRewardLevelsToClaim;
            button_Claim.OnClaimed += UpdateLastLevelClaimed;

            buttonActionItemInfo = GameObject.Find("Panel_ItemInfo_BG").transform.GetChild(4).GetComponent<Button_Action_ItemInfo>();
            buttonActionItemInfo.OnItemSold += CalculateCurrentGold;

            
        }
    }

    public int GetDictionaryAmount()
    {
        return inventory.Count;
    }


    public void AugmentCurrentInventorySlotAmount(int updatedCurrentInventorySLotAmount)
    {
        currentInventorySlotAmount = updatedCurrentInventorySLotAmount;
    }

    public void ListenInventorySlots(object sender, Inventory.OnInventorySlotCreatedEventArgs e)
    {

        e.newActiveOrInactiveInventorySlot.onInventoryPlacedItem += AddToDictionnary; 
        e.newActiveOrInactiveInventorySlot.onInventoryRemovedItem += RemoveFromDictionnary;
        e.newActiveOrInactiveInventorySlot.OnSlotPurchased += CalculateCurrentGold;
        
    }
    public void GenerateDictionary(InventorySlots newInventorySlot)
    {
        inventory[newInventorySlot] = null;
        GenerateEMptySlotList();
    }

    void AddToDictionnary(object semder, InventorySlots.onInventoryItemModificationEventArgs e)
    {
        inventory[e.slot] = e.gameItem;
        GenerateEMptySlotList();
    }

    void RemoveFromDictionnary(object sender, InventorySlots.onInventoryItemModificationEventArgs e)
    {
        inventory[e.slot] = null;
        GenerateEMptySlotList();
    }

    void GenerateEMptySlotList()
    {
        emptySlots.Clear();

        foreach (KeyValuePair<InventorySlots, GameItems> entries in inventory)
        {
            if (entries.Value == null)
            {
                emptySlots.Add(entries.Key);
            }
        }

    }

    
    
    void CalculateCurrentGold(object sender, MasterEventListener.OnFinancialEvent e)
    {
        if (e.itemValue != default(int)) currentGold += e.itemValue;
        if (e.inventorySlotCost != default(int)) currentGold -= e.inventorySlotCost;
        OnCurrentGoldChanged?.Invoke(EventArgs.Empty);
    }


    void CalculateXPFromStars(object sender, GameItems.OnItemCollectedEventArgs e)
    {
        currentXP += e.xpValue;
        UpdateXpPoints();
    }

    void CalculateXPFromQuests(object sender, QuestManager.OnQuestAddRemoveEventArgs e)
    {
        currentXP += e.quest.questXPReward; //q.questXPReward;  
        UpdateXpPoints();
    }

    void SetCurrentLevel(int savedLevel = 1)
    {
        currentLevel = savedLevel;
        GetXPToNextLevel(currentLevel);
    }

    void UpdateXpPoints()
    {
        //currentXP += e.xpValue;
      
        while (currentXP >= XPToNextLevel)
        {

            OnLevelNumberChanged?.Invoke(this, new OnLevelChangedEventArgs {xpToNextLevel= XPToNextLevel, currentXP = XPToNextLevel });
            currentXP -= XPToNextLevel;
            UpdateLevel(currentLevel);

            OnLevelTextChanged?.Invoke(this, new OnLevelChangedEventArgs { levelText = currentLevel.ToString() }); // bunu

            OnResetBar?.Invoke(this, null);

        }

        OnLevelNumberChanged?.Invoke(this, new OnLevelChangedEventArgs { xpToNextLevel = XPToNextLevel, currentXP =currentXP});

    }

    void UpdateLevel(int oldLevel)
    {
        currentLevel = oldLevel + 1;
        Debug.Log("next level is " + currentLevel);
        GetXPToNextLevel(currentLevel);

    }


    void GetXPToNextLevel(int currentLevel) // bu form�lizeedilecek
    {
        if (currentLevel == 1)
        {
            XPToNextLevel = 10;
        }
        else if(currentLevel == 2)
        {
            XPToNextLevel = 20;
        }
        else if (currentLevel == 3)
        {
            XPToNextLevel = 30;
        }
        else if (currentLevel == 4)
        {
            XPToNextLevel = 40;
        }
        else if (currentLevel == 5)
        {
            XPToNextLevel = 50;
        }
    }

    void UpdateListOfRewardLevelsToClaim(List<int> ListOfRewardLevelsToClaimIN)
    {
        listOfRewardLevelsToClaim = ListOfRewardLevelsToClaimIN;
    }

    void UpdateLastLevelClaimed(int lastClaimedLevelIN)
    {
        lastClaimedLevel = lastClaimedLevelIN;
    }

    public object CaptureState()
    {
        Dictionary<string, object> _variablesDict = new Dictionary<string, object>();

        _variablesDict.Add("listOfRewardLevelsToClaim", listOfRewardLevelsToClaim);
        _variablesDict.Add("currentXP", currentXP);
        _variablesDict.Add("currentGold", currentGold);
        _variablesDict.Add("XPToNextLevel", XPToNextLevel);
        _variablesDict.Add("currentInventorySlotAmount", currentInventorySlotAmount);
        _variablesDict.Add("currentLevel", currentLevel);

        Dictionary<int, object> _itemsDict = new Dictionary<int, object>();
        foreach (KeyValuePair<InventorySlots,GameItems> pair in inventory )
        {
            if (pair.Value!=null) _itemsDict.Add(pair.Key.slotIDNumber, pair.Value.CaptureState());
        }
        _variablesDict.Add("itemsDict", _itemsDict);

        return _variablesDict;
    }

    public void RestoreState(object state) 
    {
        Dictionary<string, object> _VariablesDictIN = (Dictionary<string, object>)state;

        listOfRewardLevelsToClaim = (List<int>)_VariablesDictIN["listOfRewardLevelsToClaim"];
        currentInventorySlotAmount = (int)_VariablesDictIN["currentInventorySlotAmount"];
        currentXP = (int)_VariablesDictIN["currentXP"];
        currentGold = (int)_VariablesDictIN["currentGold"];
        XPToNextLevel = (int)_VariablesDictIN["XPToNextLevel"];
        currentLevel = (int)_VariablesDictIN["currentLevel"];

        _itemsDictToLoad = (Dictionary<int, object>)_VariablesDictIN["itemsDict"];


    }
}
