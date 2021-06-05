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


    public Sprite GetAssetSprite(string itemName)
    {
        return Resources.Load<Sprite>("Sprites/"+ itemName);
    }


   
}
