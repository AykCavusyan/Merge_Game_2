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


    private int slotIDNumber = 0;
    private Rewards_LevelUp rewardsLevelUp;
    public List<List<Item.ItemGenre>> rewardsList = new List<List<Item.ItemGenre>>();
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
        slotIDNumber = 0;

        for (int i = 0; i < rewardsList[0].Count; i++)
        {
            slotIDNumber++;
            currentRewardSlots.Add(CreateNewSLot(slotIDNumber,rewardsList[0] ,i));
        }
        
        DisableChildrenImages();
    }


    private GameObject CreateNewSLot(int slotIDNumberIN,List<Item.ItemGenre> list ,int listIndex)
    {
        GameObject currentNewSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotRewards"));

        currentNewSlot.transform.SetParent(inner_Panel_Container, false);
        currentNewSlot.AddComponent<RewardSlots>().slotIDNumber = slotIDNumberIN;

        GameObject itemToAdd = ItemBag.Instance.GenerateItem(list[listIndex], playerLevel, true);
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

    void ClaimReward(EventArgs e)
    {
        TransferClaimedItems();
        DestroySlots();
        RemoveClaimedlist();
        InstantiateSlots();
    }

    void TransferClaimedItems()
    {
        for (int i = 0; i < currentRewardSlots.Count; i++)
        {
            GameItems itemtoTransfer = currentRewardSlots[i].GetComponent<RewardSlots>().containedItem;
            itemtoTransfer.isRewardPanelItem = false;
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

    void RemoveClaimedlist()
    {
        rewardsList.RemoveAt(0);
    }

}
