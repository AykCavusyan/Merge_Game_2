using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerInfo : MonoBehaviour
{
    private static PlayerInfo _instance;
    public static PlayerInfo Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    private Dictionary<InventorySlots, GameItems> inventory = new Dictionary<InventorySlots, GameItems>();
    public List<InventorySlots> emptySlots = new List<InventorySlots>();

    public int remainingInventorySlotAmount;
    public int currentInventorySlotAmount { get; private set; }
    public int maxInventorySlotAmount { get; private set; }


    public int currentXP { get; private set; }
    public int XPToNextLevel { get; private set; }
    public int currentLevel { get; private set; }
    //private GameObject levelBar;
    
    private MasterEventListener masterEventListener; // bu gerekli mi bakmak laz�m

    public event EventHandler<OnLevelChangedEventArgs> OnResetBar;
    public event EventHandler<OnLevelChangedEventArgs> OnLevelTextChanged;
    public event EventHandler<OnLevelChangedEventArgs> OnLevelNumberChanged;
    public class OnLevelChangedEventArgs
    {
        public string levelText;
        public int currentXP;
        public int xpToNextLevel;
    }

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

        masterEventListener = GetComponent<MasterEventListener>();
    }
    private void OnEnable()
    {
        MasterEventListener.Instance.OnItemCollectted += CalculateXPFromStars;
    }

    private void Start()
    {
        QuestManager.Instance.OnQuestCompleted += CalculateXPFromQuests;
    }
    private void OnDisable()
    {
        MasterEventListener.Instance.OnItemCollectted -= CalculateXPFromStars;
        QuestManager.Instance.OnQuestCompleted -= CalculateXPFromQuests;
    }


    public int GetDictionaryAmount()
    {
        return inventory.Count;
    }


    public void AugmentCurrentInventorySlotAmount(int updatedCurrentInventorySLotAmount)
    {
        currentInventorySlotAmount = updatedCurrentInventorySLotAmount;
    }

    public void ListenInventorySlots(InventorySlots inventorySlots)
    {
        inventorySlots.onInventoryPlacedItem += AddToDictionnary;
        inventorySlots.onInventoryRemovedItem += RemoveFromDictionnary;
        
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

        Debug.Log("listened");
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




    void CalculateXPFromStars(object sender, GameItems.OnItemCollectedEventArgs e)
    {
        currentXP += e.xpValue;
        UpdateXpPoints();

        Debug.Log("xp came from star is" + e.xpValue);
        Debug.Log("current XP is " + currentXP);
    }

    void CalculateXPFromQuests(Quest q)
    {
        currentXP += q.questXPReward;  
        UpdateXpPoints();

        Debug.Log("xp came from quest is" + q.questXPReward);
        Debug.Log("current XP is " + currentXP);
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
            //levelBar.GetComponent<LevelBar>().UpdateBarFill();
            currentXP -= XPToNextLevel;
            UpdateLevel(currentLevel);

            OnLevelTextChanged?.Invoke(this, new OnLevelChangedEventArgs { levelText = currentLevel.ToString() }); // bunu

            OnResetBar?.Invoke(this, null);
            //levelBar.GetComponent<LevelBar>().ResetBarFill();
            //levelBar.GetComponent<LevelBar>().ResetBarFillAmount();
        }

        Debug.Log("xp being added on event");
        OnLevelNumberChanged?.Invoke(this, new OnLevelChangedEventArgs { xpToNextLevel = XPToNextLevel, currentXP =currentXP});
        //levelBar.GetComponent<LevelBar>().UpdateBarFill( XPToNextLevel, currentXP);

    }

    void UpdateLevel(int oldLevel)
    {
        currentLevel = oldLevel + 1;
        Debug.Log("next level is " + currentLevel);
        GetXPToNextLevel(currentLevel);

    }

    //void UpdateLevelText(int level)
    //{
    //    levelText.UpdateText(level);
    //}

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


}
