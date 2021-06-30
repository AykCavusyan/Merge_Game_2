using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewards : MonoBehaviour
{
    private Transform inner_Panel_Container;

    private int slotIDNumber;
    private Rewards_LevelUp rewardsLevelUp;
    private List<Item.ItemGenre> rewardsList ;

    private int playerLevel;

    private void Awake()
    {
        inner_Panel_Container = transform.GetChild(2);
        playerLevel = 1;
    }

    private void Start()
    {
        InstantiateSlots();
    }

    void InstantiateSlots()
    {
        GenerateRewadsList(playerLevel);
        Debug.Log(rewardsLevelUp);

        for (int i = 0; i < rewardsList.Count; i++)
        {
            CreateNewSLot();
        }
    }


    void GenerateRewadsList(int playerLevel)
    {
        rewardsLevelUp = new Rewards_LevelUp(playerLevel);
        rewardsList = rewardsLevelUp.rewardList;
    }

    void CreateNewSLot()
    {
       GameObject currentNewSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotRewards"));

       currentNewSlot.transform.SetParent(inner_Panel_Container, false);

    }

}
