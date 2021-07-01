using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rewards : MonoBehaviour
{
    private Transform inner_Panel_Container;
    private GameObject player;
    private Button_Claim claimButton;


    //private int slotIDNumber = 0;
    private Rewards_LevelUp rewardsLevelUp;
    private List<List<Item.ItemGenre>> rewardsList = new List<List<Item.ItemGenre>>();
    private List<GameObject> currentRewardSlots = new List<GameObject>();

    private int playerLevel;



    private void Awake()
    {
        inner_Panel_Container = transform.GetChild(2);
        claimButton = transform.GetChild(3).GetComponent<Button_Claim>();
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    private void OnEnable()
    {    
        Init();
        PlayerInfo.Instance.OnLevelTextChanged += GenerateRewadsList;
        claimButton.OnClaimed += ClaimReward;
    }

    private void OnDisable()
    {
        PlayerInfo.Instance.OnLevelTextChanged -= GenerateRewadsList;
        claimButton.OnClaimed -= ClaimReward;

    }

    private void Start()
    {
        
        GenerateRewadsList(null,null);
        InstantiateSlots();
    }

    void Init()
    {
        if (PlayerInfo.Instance == null)
        {
            Instantiate(player);
        }
    }

    void GenerateRewadsList(object sender, PlayerInfo.OnLevelChangedEventArgs e)
    {
        SetplayerLevel();
        rewardsLevelUp = new Rewards_LevelUp(playerLevel);
        rewardsList.Add(rewardsLevelUp.rewardList);
    }



    void SetplayerLevel()
    {
        playerLevel = PlayerInfo.Instance.currentLevel;
    }



    void InstantiateSlots()
    {
        Debug.Log(rewardsLevelUp);

        for (int i = 0; i < rewardsList[0].Count; i++)
        {
            int slotIDNumber = 0;
            slotIDNumber++;
            currentRewardSlots.Add(CreateNewSLot(slotIDNumber,rewardsList[0] ,i));
        }
    }


    private GameObject CreateNewSLot(int slotIDNumberIN,List<Item.ItemGenre> list ,int listIndex)
    {
        GameObject currentNewSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotRewards"));

        currentNewSlot.transform.SetParent(inner_Panel_Container, false);
        
        currentNewSlot.AddComponent<RewardSlots>().slotIDNumber = slotIDNumberIN;

         
        GameObject itemToAdd = ItemBag.Instance.GenerateItem(list[listIndex], playerLevel);
        currentNewSlot.GetComponent<RewardSlots>().Drop(itemToAdd.GetComponent<GameItems>());


        return currentNewSlot;
    }

    void ClaimReward(EventArgs e)
    {
        DestroyOldSlots();
        RemoveClaimedlist();
        InstantiateSlots();
    }



    void DestroyOldSlots()
    {
        for (int i = 0; i < currentRewardSlots.Count; i++)
        {
            Destroy(currentRewardSlots[i]);
        }
    }

    void RemoveClaimedlist()
    {
        rewardsList.RemoveAt(0);
    }

}
