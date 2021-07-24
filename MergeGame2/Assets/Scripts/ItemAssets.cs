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


   
}
