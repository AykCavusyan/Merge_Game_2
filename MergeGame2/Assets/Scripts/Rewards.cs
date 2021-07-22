using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Rewards : MonoBehaviour
{
    private Transform inner_Panel_Container;
    private GameObject player;
    private Button_Claim claimButton;

    public int currentLevelToClaim { get; private set; }
    private int slotIDNumber = 0;
    private List<int> levelsToClaim = new List<int>();

    //private Rewards_LevelUp newLevelRewardsList; // bu gerekli olmayabilir aþaðýda olsa yetebilir
   
    //public List<List<Item.ItemGenre>> rewardsList = new List<List<Item.ItemGenre>>();

    public Dictionary<int, List<Item.ItemGenre>> rewardsDict = new Dictionary<int, List<Item.ItemGenre>>();
    private List<GameObject> currentRewardSlots = new List<GameObject>();

    private int playerLevel;

    public event Action<GameItems> OnRewardItemGiven;
    public event Action<List<int>> OnListOfRewardLevelToClaimUpdated;

    private void Awake()
    {
        inner_Panel_Container = transform.GetChild(2);
        claimButton = transform.GetChild(3).GetComponent<Button_Claim>();
        player = GameObject.FindGameObjectWithTag("Player");
        //levelsToClaim.Add(1);
        
    }

    private void OnEnable()
    {    
        Init();
        PlayerInfo.Instance.OnLevelTextChanged += UpdateRewardsDict;
        claimButton.OnClaimed += ClaimReward;
    }

    private void OnDisable()
    {
        PlayerInfo.Instance.OnLevelTextChanged -= UpdateRewardsDict;
        claimButton.OnClaimed -= ClaimReward;

    }


    public void ConfigPanel(List<int> levelsToClaimIN, int playerLevelIN)
    {
        playerLevel = playerLevelIN; 
        levelsToClaim = levelsToClaimIN;
        if(levelsToClaimIN.Count == 0) levelsToClaim.Add(1);
        GenerateRewardsDict();
        //UpdateRewardsList(null, null);
        InstantiateSlots();
    }

    void Init()
    {
        if (PlayerInfo.Instance == null)
        {
            Instantiate(player);
        }
    }

    //void UpdateRewardsList(int playerLevelIN)
    //{

    //}

    //private int SetplayerLevel()
    //{
    //    playerLevel = PlayerInfo.Instance.currentLevel;
    //    return playerLevel;
    //}

    void GenerateRewardsDict()
    {
        foreach (int level in levelsToClaim)
        {
            var newLevelRewardsList = new Rewards_LevelUp(level);
            Debug.Log(level);
            rewardsDict.Add(level, newLevelRewardsList.rewardList);
        }
    }

    void UpdateLevelsToClaimList(int levelToClaimIN)
    {
        if (!levelsToClaim.Contains(levelToClaimIN)) levelsToClaim.Add(levelToClaimIN);
        else if (levelsToClaim.Contains(levelToClaimIN)) levelsToClaim.Remove(levelToClaimIN);

        OnListOfRewardLevelToClaimUpdated?.Invoke(levelsToClaim);
    }

    void UpdateRewardsDict(object sender, PlayerInfo.OnLevelChangedEventArgs e)
    {
        playerLevel = PlayerInfo.Instance.currentLevel;
        var newLevelRewardsList = new Rewards_LevelUp(playerLevel);
        rewardsDict.Add(playerLevel, newLevelRewardsList.rewardList);

        UpdateLevelsToClaimList(playerLevel);
        //rewardsList.Add(newLevelRewardsList.rewardList);
    }

    void InstantiateSlots()
    {
        slotIDNumber = 0;
        currentLevelToClaim = rewardsDict.Keys.Min();
        Debug.Log(currentLevelToClaim);
       // for (int i = 0; i < rewardsList[0].Count; i++)
        for (int i = 0; i < rewardsDict[currentLevelToClaim].Count; i++)
        {
            slotIDNumber++;
            currentRewardSlots.Add(CreateNewSLot(slotIDNumber,rewardsDict[currentLevelToClaim] ,i));
        }
        
        DisableChildrenImages();
    }


    private GameObject CreateNewSLot(int slotIDNumberIN,List<Item.ItemGenre> list ,int listIndex)
    {
        GameObject currentNewSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotRewards"));

        currentNewSlot.transform.SetParent(inner_Panel_Container, false);
        currentNewSlot.AddComponent<RewardSlots>().slotIDNumber = slotIDNumberIN;

        GameObject itemToAdd = ItemBag.Instance.GenerateItem(list[listIndex], currentLevelToClaim, true);
        currentNewSlot.GetComponent<RewardSlots>().Drop(itemToAdd.GetComponent<GameItems>());


        return currentNewSlot;
    }

    void DisableChildrenImages()
    {
        var childrenToDisable = inner_Panel_Container.GetComponentsInChildren<Image>();
        foreach (var childToDisable in childrenToDisable)
        {
            childToDisable.enabled = false;
        }
    }

    void ClaimReward(int i)
    {
        TransferClaimedItems();
        DestroySlots();
        RemoveClaimedListFromDict();
        InstantiateSlots();
    }

    void TransferClaimedItems()
    {
        for (int i = 0; i < currentRewardSlots.Count; i++)
        {
            GameItems itemtoTransfer = currentRewardSlots[i].GetComponent<RewardSlots>().containedItem;
            itemtoTransfer.CheckIfRewardItemIsQuestItem();

            OnRewardItemGiven?.Invoke(itemtoTransfer);

            PlayerInfo.Instance.emptySlots.First().Drop (itemtoTransfer);
        }
    }

    void DestroySlots()
    {

        for (int i = 0; i < currentRewardSlots.Count; i++)
        {
            Destroy(currentRewardSlots[i]);
        }
        currentRewardSlots.Clear();
    }

    void RemoveClaimedListFromDict()
    {
        rewardsDict.Remove(currentLevelToClaim);

        UpdateLevelsToClaimList(currentLevelToClaim);
    }

    //public object CaptureState()
    //{
    //    List<int> _levelsToClaim = levelsToClaim;

    //    return _levelsToClaim;
    //}

    //public void RestoreState (object state)
    //{
    //    levelsToClaim = (List<int>)state;
    //}

}
