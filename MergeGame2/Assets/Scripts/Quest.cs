using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public int zoneNumber { get; private set; }
    public int taskNumber{ get; private set; }
    public string questName { get; private set; }
    public List<Item> itemsNeeded { get; private set; }
    public Item questItemReward { get; private set; }
    public int questXPReward { get; private set; }
    public int questGoldReward { get; private set; }

    public bool canComplete;

    private bool isActive;
   
    //private (int taskNumber, string questName, Item.ItemType itemNeeded , Item.ItemGenre itemReward) tuple;


    public Quest(int zoneNumberIN, int taskNumberIN)
    {
        this.zoneNumber = zoneNumberIN;
        this.taskNumber = taskNumberIN;

        var questInfoFromDictTuple = GetQuestInfo(zoneNumber, taskNumber);
        this.questName = questInfoFromDictTuple.questName;
        this.itemsNeeded = questInfoFromDictTuple.itemsNeeded;
        this.questItemReward = questInfoFromDictTuple.itemReward;
        this.questXPReward = questInfoFromDictTuple.xpReward;
        this.questGoldReward = questInfoFromDictTuple.goldReward;

        canComplete = false;
    }


    private static Dictionary<int, Dictionary<int,(string , List<Item> , Item, int, int )>> questDict = new Dictionary<int, Dictionary<int, (string , List<Item> , Item, int , int)>>
    {
         
        { 1,
            new Dictionary<int, (string , List<Item> , Item, int,int )>
            {
                {1, ("merge a pie", new List<Item> { new Item(Item.ItemGenre.Meals,3), new Item(Item.ItemGenre.Meals, 2), new Item(Item.ItemGenre.Meals,1) }, null, 1, 5)},
                {2, ("merge a bigger pie", new List<Item> { new Item(Item.ItemGenre.Meals,3), new Item(Item.ItemGenre.Meals, 2), new Item(Item.ItemGenre.Meals,1) }, null, 5,10)},
                {3, ("merge an even bigger pie", new List<Item> { new Item(Item.ItemGenre.Meals,3), new Item(Item.ItemGenre.Meals, 2), new Item(Item.ItemGenre.Meals,1) }, new Item(Item.ItemGenre.Armor,5), 10,15)},

            }
        },

        //{2,
        //    new Dictionary<int, (string , List<Item.ItemType> , Item.ItemGenre )>
        //    {
        //        {1, ("Merge a Sword", new List<Item.ItemType> {Item.ItemType.Armor_2, Item.ItemType.Armor_3, Item.ItemType.Armor_3 }, Item.ItemGenre.Armor)},
        //        {2, ("Merge a bigger Sword", new List<Item.ItemType>{Item.ItemType.Armor_2, Item.ItemType.Armor_3, Item.ItemType.Armor_3 }, Item.ItemGenre.Armor)},
        //        {3, ("Merge an even Bigger Sword", new List<Item.ItemType>{Item.ItemType.Armor_2, Item.ItemType.Armor_3, Item.ItemType.Armor_3 }, Item.ItemGenre.Armor)},
        //    }
        //},

    };

        
    
  
    private (string questName, List<Item> itemsNeeded, Item itemReward, int xpReward, int goldReward) GetQuestInfo (int zoneNumberIN, int taskNumberIN)
    {
        questDict.TryGetValue(zoneNumberIN, out Dictionary<int, (string , List<Item>, Item, int, int)> _innerDictionary);
        _innerDictionary.TryGetValue(taskNumberIN, out (string questName, List<Item> itemsNeeded, Item itemReward, int xpReward, int goldReward)  _questinfoTuple) ;

        return _questinfoTuple;
    }

}
