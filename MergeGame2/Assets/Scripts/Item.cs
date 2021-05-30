using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
    public enum ItemType
    {
        Sword,
        Chicken,
        Sun,
        Potion,
        Heart,
    }

    public ItemType itemType;
    public Sprite GetSprite()
    {
        switch (itemType) { default:

            case ItemType.Sword:     return ItemAssets.Instance.Sword;
            case ItemType.Chicken:   return ItemAssets.Instance.Chicken;
            case ItemType.Sun:       return ItemAssets.Instance.Sun;
            case ItemType.Potion:    return ItemAssets.Instance.Potion;
            case ItemType.Heart:     return ItemAssets.Instance.Heart;


        }
    }

    public ItemType GetRandomObject()
    {
        var enumLenght = System.Enum.GetNames(typeof(ItemType)).Length;
        return (ItemType)Random.Range(0, enumLenght);
    }


}
