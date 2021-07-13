using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    private int zoneNumber;
    private int taskNumber;
    public string questName { get; private set; }
    public List<Item> itemsNeeded { get; private set; }
    public Item questItemReward { get; private set; }
    public int questXPReward { get; private set; }

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

        canComplete = false;
    }


    private static Dictionary<int, Dictionary<int,(string , List<Item> , Item, int )>> questDict = new Dictionary<int, Dictionary<int, (string , List<Item> , Item, int )>>
    {
         
        { 1,
            new Dictionary<int, (string , List<Item> , Item, int )>
            {
                {1, ("merge a pie", new List<Item> { new Item(Item.ItemGenre.Meals,3), new Item(Item.ItemGenre.Meals, 2), new Item(Item.ItemGenre.Meals,1) }, null, 1)},
                {2, ("merge a bigger pie", new List<Item> { new Item(Item.ItemGenre.Meals,3), new Item(Item.ItemGenre.Meals, 2), new Item(Item.ItemGenre.Meals,1) }, null, 5)},
                {3, ("merge an even bigger pie", new List<Item> { new Item(Item.ItemGenre.Meals,3), new Item(Item.ItemGenre.Meals, 2), new Item(Item.ItemGenre.Meals,1) }, new Item(Item.ItemGenre.Armor,5), 10)},

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

        
    
  
    private (string questName, List<Item> itemsNeeded, Item itemReward, int xpReward) GetQuestInfo (int zoneNumberIN, int taskNumberIN)
    {
        questDict.TryGetValue(zoneNumberIN, out Dictionary<int, (string , List<Item>, Item, int)> _innerDictionary);
        _innerDictionary.TryGetValue(taskNumberIN, out (string questName, List<Item> itemsNeeded, Item itemReward, int xpReward)  _questinfoTuple) ;

        return _questinfoTuple;
    }

}
