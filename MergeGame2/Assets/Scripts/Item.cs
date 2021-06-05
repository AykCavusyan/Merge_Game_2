using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
    public static Dictionary<int, ItemType> _itemDictionary = new Dictionary<int, ItemType>
    {
        {1, ItemType.Sword },
        {2, ItemType.Chicken },
        {3, ItemType.Sun },
        {4, ItemType.Potion },
        {5, ItemType.Heart },
    };
    


    public enum ItemType
    {
        Sword,
        Chicken,
        Sun,
        Potion,
        Heart,
    }

    public ItemType itemType;
    public int itemLevel;
    
    public Sprite GetSprite(ItemType itemType)
    {
        string itemTypeName = Enum.GetName(typeof(ItemType),(int)itemType);
        return ItemAssets.Instance.GetAssetSprite(itemTypeName);
    }

    public ItemType CreateItemForRelevatLevel(int itemLevel)
    {
        _itemDictionary.TryGetValue(itemLevel, out ItemType generatedItem);
        return generatedItem;

        //var enumLenght = Enum.GetNames(typeof(ItemType)).Length;
        //return (ItemType)UnityEngine.Random.Range(0, enumLenght);
    }






    // belki lazým olmayaiblir 
    public int GetItemLevel()
    {
        foreach (KeyValuePair<int, ItemType> item in _itemDictionary)
        {
            return item.Key;
        }

        return 1;

        //switch (itemType)
        //{
        //    default:
        //    case ItemType.Sword:  return 1;
        //    case ItemType.Chicken: return 2;
        //    case ItemType.Sun: return 3;
        //    case ItemType.Potion: return 4;
        //    case ItemType.Heart: return 5;
        //}
    }

  
}
