using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get { return _instance; } }
    private static ItemAssets _instance;
    private static readonly object _lock = new object();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = this;
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }
    }

    public Sprite GetAssetSprite(Item.ItemType itemType)
    {
        string itemTypeName = Enum.GetName(typeof(Item.ItemType), (int)itemType);
        return Resources.Load<Sprite>("Sprites/"+ itemTypeName);
    }

    public string[] GetItemNameAndDescription(Item.ItemType itemTypeIN)
    {
        string itemName;
        string itemDescription;

        switch (itemTypeIN)
        {
            case Item.ItemType.Star_1:
                itemName = "Experience Star Level 1";
                itemDescription = "Merge it with another Level 1 experience star to get even more experience!";
                break;
            case Item.ItemType.Star_2:
                itemName = "Experience Star Level 2";
                itemDescription = "Merge it with another Level 2 experience star to get even more experience!";
                break;
            case Item.ItemType.Star_3:
                itemName = "Experience Star Level 3";
                itemDescription = "Merge it with another Level 3 experience star to get even more experience!";
                break;
            case Item.ItemType.Star_4:
                itemName = "Experience Star Level 4";
                itemDescription = "Merge it with another Level 4 experience star to get even more experience!";
                break;
            case Item.ItemType.Star_5:
                itemName = "Experience Star Level 5";
                itemDescription = "Merge it with another Level 5 experience star to get even more experience!";
                break;
            case Item.ItemType.Meal_1:
                itemName = "Delicious Meal Level 1";
                itemDescription = "Merge it with another delicious meal of Level 1 to get a delicious meal Level 2!";
                break;
            case Item.ItemType.Meal_2:
                itemName = "Delicious Meal Level 2";
                itemDescription = "Merge it with another delicious meal of Level 2 to get a delicious meal Level 3!";
                break;
            case Item.ItemType.Meal_3:
                itemName = "Delicious Meal Level 3";
                itemDescription = "Merge it with another delicious meal of Level 3 to get a delicious meal Level 4!";
                break;
            case Item.ItemType.Meal_4:
                itemName = "Delicious Meal Level 4";
                itemDescription = "Merge it with another delicious meal of Level 4 to get a delicious meal Level 5!";
                break;
            case Item.ItemType.Meal_5:
                itemName = "Delicious Meal Level 5";
                itemDescription = "Merge it with another delicious meal of Level 5 to get a delicious meal Level 6!";
                break;
            default:
                itemName = "Item Does not have a name ";
                itemDescription = "Item Does not have a name ";
                break;
        }
        string[] nameAndDescription = new string[2] { itemName,itemDescription};
        return nameAndDescription;
    }

}
