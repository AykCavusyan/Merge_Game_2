using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public ItemGenre itemGenre { get; private set; }
    public ItemType itemType { get; private set; }
    public int itemLevel { get; private set; }
    public bool givesXP { get; private set; } = false;
    public bool isSpawner { get; private set; } = false;
    public bool isCollectible { get; private set; } = false;
    public int xpValue { get; private set; } = 0;
    public int goldValue { get; private set; } = 0;
    public bool isMergeable { get; private set; } = true; // bunu daha sonra yapýcaz!!
    public int itemPanelID { get; private set; } = 0;
    public bool isQuestItem { get; private set; } = false;
    public bool isRewardPanelItem { get; private set; }
    public string itemName { get; private set; }
    public string itemDescription { get; private set; }

    public Item(ItemGenre itemGenre, int itemLevel, bool isRewardPanelItemIN =false)
    {

        this.itemType = CreateItemForRelevatLevel(itemLevel, itemGenre);
        this.goldValue = SetGoldValue(itemType);
        this.itemGenre = itemGenre;
        this.itemLevel = itemLevel;
        this.isRewardPanelItem = isRewardPanelItemIN;

        if (itemLevel >= 5 && itemGenre!= ItemGenre.Star)
        {
            givesXP = true;
            isSpawner = true;
        }
        else if(itemGenre == ItemGenre.Star)
        {
            isCollectible = true;
            SetXpValue(itemLevel);
            itemPanelID = 1;
        }

        foreach (Item.ItemType itemTpeReq in QuestManager.Instance._activeQuestItemsList)
        {
            if (itemType == itemTpeReq)
            {
                isQuestItem = true;
                break;
            }
        }
    }

    public ItemType CreateItemForRelevatLevel(int inputItemLevel, ItemGenre itemGenre)
    {
        _itemDictionary.TryGetValue(inputItemLevel, out Dictionary<ItemGenre, ItemType> _innerDictionary);
        _innerDictionary.TryGetValue(itemGenre, out ItemType generatedItem);

        return generatedItem;

    }

    private int SetGoldValue(ItemType itemTypeIN)
    {
        switch (itemTypeIN)
        {
            case ItemType.Armor_1:
            case ItemType.Meal_1:
                return 1;
            case ItemType.Armor_2:
            case ItemType.Meal_2:
                return 2;
            case ItemType.Armor_3:
            case ItemType.Meal_3:
                return 4;
            case ItemType.Armor_4:
            case ItemType.Meal_4:
                return 6;
            case ItemType.Armor_5:
            case ItemType.Meal_5:
                return 12;
            case ItemType.Armor_6:
            case ItemType.Meal_6:
                return 25;
            case ItemType.Armor_7:
            case ItemType.Meal_7:
                return 51;
            default: return 0;
        }
    }


    private void SetXpValue(int itemLevel)
    {
        switch (itemLevel) 
        {
            case 1:   xpValue = 1;
                break;
            case 2:   xpValue = 3;
                break;
            case 3:   xpValue = 8;
                break;
            case 4:   xpValue = 20;
                break;
            case 5:   xpValue = 50;
                break;
            default:  xpValue = 0;
                break;       
       }
        
    }

    public static Dictionary<int, Dictionary<ItemGenre, ItemType>> _itemDictionary = new Dictionary<int, Dictionary<ItemGenre, ItemType>>
    {
       {1, 
        new Dictionary<ItemGenre, ItemType> 
        {
            {ItemGenre.Chest, ItemType.Chest_1 },
            {ItemGenre.Gold, ItemType.Gold_1 },
            {ItemGenre.Armor, ItemType.Armor_1 },
            {ItemGenre.Meals, ItemType.Meal_1 },
            {ItemGenre.Star, ItemType.Star_1 },
        }
       },

        {2, 
         new Dictionary<ItemGenre, ItemType>
          {
             {ItemGenre.Chest, ItemType.Chest_2 },
             {ItemGenre.Meals, ItemType.Meal_2 },
             {ItemGenre.Armor, ItemType.Armor_2 },
             {ItemGenre.Star, ItemType.Star_2 },
          }
        },

        {3, 
          new Dictionary<ItemGenre, ItemType>
          {
            {ItemGenre.Chest, ItemType.Chest_3 },
            {ItemGenre.Meals, ItemType.Meal_3 },
            {ItemGenre.Armor, ItemType.Armor_3},
            {ItemGenre.Star, ItemType.Star_3 },
          }
        },

        {4,
          new Dictionary<ItemGenre, ItemType>
          {
            {ItemGenre.Chest, ItemType.Chest_4 },
            {ItemGenre.Meals, ItemType.Meal_4 },
            {ItemGenre.Armor, ItemType.Armor_4 },
            {ItemGenre.Star, ItemType.Star_4 },
          }
        },

        {5,
          new Dictionary<ItemGenre, ItemType>
          {
            {ItemGenre.Chest, ItemType.Chest_5 },
            {ItemGenre.Meals,ItemType.Meal_5 },
            {ItemGenre.Armor, ItemType.Armor_5 },
            {ItemGenre.Star, ItemType.Star_5 },
          }
        },

        {6,
          new Dictionary<ItemGenre, ItemType>
          {
            { ItemGenre.Meals,ItemType.Meal_6 },
            {ItemGenre.Armor, ItemType.Armor_6 },
          }
        },

        {7,
          new Dictionary<ItemGenre, ItemType>
          {
            { ItemGenre.Meals,ItemType.Meal_7 },
            {ItemGenre.Armor, ItemType.Armor_7 },
          }
        },

        {8,
          new Dictionary<ItemGenre, ItemType>
          {
            { ItemGenre.Meals,ItemType.Meal_8 },
            {ItemGenre.Armor, ItemType.Armor_8 },
          }
        },

        {9,
          new Dictionary<ItemGenre, ItemType>
          {
            { ItemGenre.Meals,ItemType.Meal_9 },
            {ItemGenre.Armor, ItemType.Armor_9 },
          }
        },

        {10,
          new Dictionary<ItemGenre, ItemType>
          {
            { ItemGenre.Meals,ItemType.Meal_10 },
            {ItemGenre.Armor, ItemType.Armor_10 },
          }
        },
    };

    public enum ItemType
    {
        Chest_1,
        Chest_2,
        Chest_3,
        Chest_4,
        Chest_5,
        Gold_1,
        Star_1,
        Star_2,
        Star_3,
        Star_4,
        Star_5,
        Meal_1,
        Meal_2,
        Meal_3,
        Meal_4,
        Meal_5,
        Meal_6,
        Meal_7,
        Meal_8,
        Meal_9,
        Meal_10,
        Armor_1,
        Armor_2,
        Armor_3,
        Armor_4,
        Armor_5,
        Armor_6,
        Armor_7,
        Armor_8,
        Armor_9,
        Armor_10
    }

    public enum ItemGenre
    {
        Chest,
        Gold,
        Star,
        Meals,
        Other,
        Armor,
        Sword,
        Axe,
        Hammer,
        Staff,
        Mace,
        Ranged
    }




}
