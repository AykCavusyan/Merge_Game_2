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
    
    private MasterEventListener masterEventListener; // bu gerekli mi bakmak lazým

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
        //GetRemainingInventorySLotAmount(); // bu gerekli mi þu an belli deðil?? ileride bakýlacak

        SetCurrentLevel();
        //levelBar = GameObject.Find("Level_BG_Bar");
        //levelText = levelBar.transform.GetChild(0).GetChild(0).GetComponent<LevelText>();

        masterEventListener = GetComponent<MasterEventListener>();
    }
    private void OnEnable()
    {
        MasterEventListener.Instance.OnItemCollectted += UpdateXpPoints;
    }
    private void OnDisable()
    {
        MasterEventListener.Instance.OnItemCollectted -= UpdateXpPoints;
    }

    //public int GetRemainingInventorySLotAmount()
    //{
    //    remainingInventorySlotAmount = currentInventorySlotAmount - GetDictionaryAmount();

    //    Debug.Log(remainingInventorySlotAmount);
    //    return remainingInventorySlotAmount;
    //}

    public int GetDictionaryAmount()
    {
        return inventory.Count;
    }


    public void AugmentCurrentInventorySlotAmount(int updatedCurrentInventorySLotAmount)
    {
        currentInventorySlotAmount = updatedCurrentInventorySLotAmount;
        //GetRemainingInventorySLotAmount();
        //OnAugmentedMaxInventorySize?.Invoke();
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
        //GetRemainingInventorySLotAmount();
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
                Debug.Log(emptySlots.Count);
            }
        }

    }



    //void RemoveFromDictionnary(GameObject removedItem)
    //{
    //    if (ownedItems != null && ownedItems.Count > 0)
    //    {
    //        ownedItems.Remove(removedItem);
    //    }
    //    else
    //    {
    //        Debug.Log("no item left in your inventory");
    //    }
    //}

    void SetCurrentLevel(int savedLevel = 1)
    {
        currentLevel = savedLevel;
        GetXPToNextLevel(currentLevel);
    }

    void UpdateXpPoints(object sender, GameItems.OnItemCollectedEventArgs e)
    {
        
        currentXP += e.xpValue;
        Debug.Log("xp came from star is" + e.xpValue);
        Debug.Log("current xp is " + currentXP);

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


}
