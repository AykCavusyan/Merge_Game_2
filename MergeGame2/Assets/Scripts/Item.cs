using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public static Dictionary<int, Dictionary<ItemGenre, ItemType>> _itemDictionary = new Dictionary<int, Dictionary<ItemGenre, ItemType>>
    {
       {1, 
        new Dictionary<ItemGenre, ItemType> 
        {
            //{ItemGenre.Other, ItemType.Sword },
            {ItemGenre.Other , ItemType.Chicken },
            //{ItemGenre.Other, ItemType.Sun },
            //{ItemGenre.Other, ItemType.Potion },
            {ItemGenre.Armor, ItemType.Armor_1 },
            {ItemGenre.Meals, ItemType.Meal_1 }
        }
       },

        {2, 
         new Dictionary<ItemGenre, ItemType>
          {
             { ItemGenre.Meals, ItemType.Meal_2 },
             {ItemGenre.Armor, ItemType.Armor_2 },
          }
        },

        {3, 
          new Dictionary<ItemGenre, ItemType>
          {
            { ItemGenre.Meals, ItemType.Meal_3 },
            {ItemGenre.Armor, ItemType.Armor_3},
          }
        },

        {4,
          new Dictionary<ItemGenre, ItemType>
          {
            { ItemGenre.Meals, ItemType.Meal_4 },
            {ItemGenre.Armor, ItemType.Armor_4 },
          }
        },

        {5,
          new Dictionary<ItemGenre, ItemType>
          {
            { ItemGenre.Meals,ItemType.Meal_5 },
            {ItemGenre.Armor, ItemType.Armor_5 },
          }
        },
    };

    


    public enum ItemType
    {
        Sword,
        Chicken,
        Sun,
        Potion,
        Heart,
        Meal_1,
        Meal_2,
        Meal_3,
        Meal_4,
        Meal_5,
        Armor_1,
        Armor_2,
        Armor_3,
        Armor_4,
        Armor_5,
    }

    public enum ItemGenre
    {
        Meals,
        Other,
        Armor,
    }

    public ItemGenre itemGenre;
    public ItemType itemType;
    public int itemLevel;
    
    public Sprite GetSprite(ItemType itemType)
    {
        string itemTypeName = Enum.GetName(typeof(ItemType),(int)itemType);
        return ItemAssets.Instance.GetAssetSprite(itemTypeName);
    }

    public ItemType CreateItemForRelevatLevel(int inputItemLevel, ItemGenre itemGenre)
    {

         _itemDictionary.TryGetValue(inputItemLevel, out Dictionary<ItemGenre, ItemType> _innerDictionary);
        _innerDictionary.TryGetValue(itemGenre, out ItemType generatedItem);
   
        return generatedItem;

        //var enumLenght = Enum.GetNames(typeof(ItemType)).Length;
        //return (ItemType)UnityEngine.Random.Range(0, enumLenght);
    }


    

  //public ItemGenre GetItemGenre(int itemLevel, ItemType itemType)
  //  {
  //      _itemDictionary.TryGetValue(itemLevel, out Dictionary<ItemGenre, ItemType> _innerDictionary);

  //      foreach (KeyValuePair<ItemGenre, ItemType> item in _innerDictionary)
  //      {
  //          if (item.Value == itemType )
  //          {
  //              return item.Key;
  //          }
            
  //      }

  //      return ItemGenre.Other;

  //  }
}
