using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }


    public Sprite GetAssetSprite(Item.ItemType itemType)
    {
        string itemTypeName = Enum.GetName(typeof(Item.ItemType), (int)itemType);
        return Resources.Load<Sprite>("Sprites/"+ itemTypeName);
    }


   
}
