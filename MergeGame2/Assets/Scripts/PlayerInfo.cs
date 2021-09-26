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
    private Panel_PowerUpItems panelPowerUpItems;

    private Dictionary<InventorySlots, GameItems> inventory = new Dictionary<InventorySlots, GameItems>();

    private Dictionary<int, object> _itemsDictToLoad = new Dictionary<int, object>();
    private Dictionary<int, object> _powerUpItemsDictToLoad = new Dictionary<int, object>();

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
    public int powerUpSlotAmount { get; set; } = 9;
    //private GameObject levelBar;
    
    //private MasterEventListener masterEventListener; // bu gerekli mi bakmak lazým

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

    public event EventHandler<MasterEventListener.OnPopupTextEventArgs> OnPopupTextDisplay;
    private GameObject levelBar;
    private GameObject goldBar;
    private GameObject energyBar;
    private GameObject gemBar;

    //public int  GetInitializeOrder()
    //{
    //    return initializeOrder;
    //}


    private void Awake()
    {
        if (_instance != null && _instance != this) //BUNU SINGLETON PATLADIÐI ÝÇÝN SÝLDÝM - YAPMAK LAZIM
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
        MasterEventListener.Instance.OnItemCollectted += CalculateXPnGoldFromGameItems;
        QuestManager.Instance.OnQuestCompleted += CalculateXPnGoldFromQuests; // singletonu patatýyor diye burada normal yerine almak lazým
    }
    private void OnDisable()
    {
        SceneController.Instance.OnSceneLoaded -= SceneConfig;
        MasterEventListener.Instance.OnItemCollectted -= CalculateXPnGoldFromGameItems;
        QuestManager.Instance.OnQuestCompleted -= CalculateXPnGoldFromQuests;



        if (levelPanel) levelPanel.GetComponent<Rewards>().OnListOfRewardLevelToClaimUpdated -= UpdateListOfRewardLevelsToClaim;
        if (button_Claim) button_Claim.OnClaimed -= UpdateLastLevelClaimed;
        if (buttonActionItemInfo) buttonActionItemInfo.OnItemSold -= CalculateCurrentGold;
        Button_AddPowerUpSlots.onPowerUpSlotBought -= CalculateCurrentGold;

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

            panelPowerUpItems = GameObject.Find("Panel_PowerUpItems_FG").GetComponent<Panel_PowerUpItems>();
            StartCoroutine(panelPowerUpItems.ConfigPanel(_powerUpItemsDictToLoad));

            levelPanel = GameObject.Find("Panel_LevelPanel");
            button_Claim = levelPanel.transform.GetChild(4).GetComponent<Button_Claim>();

            levelPanel.GetComponent<Rewards>().ConfigPanel(listOfRewardLevelsToClaim, currentLevel);
            levelPanel.GetComponent<Rewards>().OnListOfRewardLevelToClaimUpdated += UpdateListOfRewardLevelsToClaim;
            button_Claim.OnClaimed += UpdateLastLevelClaimed;

            buttonActionItemInfo = GameObject.Find("Panel_ItemInfo_BG").transform.GetChild(4).GetComponent<Button_Action_ItemInfo>();
            buttonActionItemInfo.OnItemSold += CalculateCurrentGold;

            Button_AddPowerUpSlots.onPowerUpSlotBought += CalculateCurrentGold;

            levelBar = GameObject.Find("Level_Bar_BackgroundHolder") ;
            goldBar = GameObject.Find("Gold_Bar_BackgroundHolder");
            energyBar = GameObject.Find("Energy_Bar_BackgroundHolder");
            gemBar = GameObject.Find("Gem_Bar_BackgroundHolder");
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
  
    void CalculateCurrentGold(object sender, MasterEventListener.OnFinancialEventArgs e)
    {
        //if (e.itemValue != default(int)) currentGold += e.itemValue;
        if (e.inventorySlotCost != default(int)) 
        {
            currentGold -= e.inventorySlotCost;
            OnPopupTextDisplay?.Invoke(this, new MasterEventListener.OnPopupTextEventArgs { amount = (e.inventorySlotCost)*-1, travelDirection = -1, originalPosition = goldBar.transform.position });
        } 

        if (e.powerUpSlotCost != default(int))
        {
            currentGold -= e.powerUpSlotCost;
            OnPopupTextDisplay?.Invoke(this, new MasterEventListener.OnPopupTextEventArgs { amount = (e.powerUpSlotCost)*-1, travelDirection = -1, originalPosition = goldBar.transform.position });
        }
        OnCurrentGoldChanged?.Invoke(EventArgs.Empty);
        

    }


    void CalculateXPnGoldFromGameItems(object sender, GameItems.OnItemCollectedEventArgs e)
    {
        if(e.xpValue != default(int))
        {
            currentXP += e.xpValue;
            OnPopupTextDisplay?.Invoke(this, new MasterEventListener.OnPopupTextEventArgs { amount = e.xpValue, travelDirection = -1, originalPosition = levelBar.transform.position });
            UpdateXpPoints();
        }
        if(e.goldValue != default(int))
        {
            currentGold += e.goldValue;
            OnPopupTextDisplay?.Invoke(this, new MasterEventListener.OnPopupTextEventArgs { amount = e.goldValue, travelDirection = -1, originalPosition = goldBar.transform.position });
            OnCurrentGoldChanged?.Invoke(EventArgs.Empty);
        }
    }

    void CalculateXPnGoldFromQuests(object sender, QuestManager.OnQuestAddRemoveEventArgs e)
    {
        if(e.quest.questXPReward != default(int))
        {
            currentXP += e.quest.questXPReward;
            OnPopupTextDisplay?.Invoke(this, new MasterEventListener.OnPopupTextEventArgs { amount = e.quest.questXPReward, travelDirection = -1 , originalPosition = levelBar.transform.position});
            UpdateXpPoints();
        }
        if(e.quest.questGoldReward != default(int))
        {
            currentGold += e.quest.questGoldReward;
            OnPopupTextDisplay?.Invoke(this, new MasterEventListener.OnPopupTextEventArgs { amount = e.quest.questGoldReward, travelDirection = -1, originalPosition = goldBar.transform.position });
            OnCurrentGoldChanged?.Invoke(EventArgs.Empty);
        }
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


    void GetXPToNextLevel(int currentLevel) // bu formülizeedilecek
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
        _variablesDict.Add("powerUpSlotAmount", powerUpSlotAmount);

        Dictionary<int, object> _itemsDict = new Dictionary<int, object>();
        foreach (KeyValuePair<InventorySlots,GameItems> pair in inventory )
        {
            if (pair.Value!=null) _itemsDict.Add(pair.Key.slotIDNumber, pair.Value.CaptureState());
        }
        _variablesDict.Add("itemsDict", _itemsDict);


        Dictionary<int, object> _powerUpItemsDict = new Dictionary<int, object>();
        foreach (GameObject slot in panelPowerUpItems.slotList)
        {
            PowerUpItem_Slots slotScript = slot.GetComponent<PowerUpItem_Slots>();
            if (slotScript.containedItem != null)
            {
                _powerUpItemsDict.Add(slotScript.slotIDNumber, slotScript.CaptureState());
            }
        }
        _variablesDict.Add("_powerUpItemsDict", _powerUpItemsDict);

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
        powerUpSlotAmount = (int)_VariablesDictIN["powerUpSlotAmount"];

        _itemsDictToLoad = (Dictionary<int, object>)_VariablesDictIN["itemsDict"];
        _powerUpItemsDictToLoad = (Dictionary<int, object>)_VariablesDictIN["_powerUpItemsDict"];

    }
}
