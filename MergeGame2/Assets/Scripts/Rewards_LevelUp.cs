using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewards_LevelUp
{
    public List<Item.ItemGenre> rewardList;
    public int rewardLevel;


    public static Dictionary<int, List<Item.ItemGenre>> _rewardsDictionary = new Dictionary<int, List<Item.ItemGenre>>
    {
        {1,
           new List<Item.ItemGenre>(3)
           {
             Item.ItemGenre.Armor,
             Item.ItemGenre.Meals,
             Item.ItemGenre.Meals,
            }
        },

        {2,
           new List<Item.ItemGenre>(3)
           {
             Item.ItemGenre.Armor,
             Item.ItemGenre.Meals,
             Item.ItemGenre.Meals,
           }
        },  
    };
        
    public Rewards_LevelUp(int playerLevel)
    {
        this.rewardLevel = playerLevel;
        this.rewardList = GenerateRewardList(rewardLevel);

    }


  

    public List<Item.ItemGenre> GenerateRewardList(int level)
    {
        _rewardsDictionary.TryGetValue(rewardLevel, out List<Item.ItemGenre> _rewardList);

        rewardList = _rewardList;
        Debug.Log(rewardLevel + "rewardslevel is");
        Debug.Log(rewardList.Count + "generate list amount ");

        return rewardList;

    }

   

}
