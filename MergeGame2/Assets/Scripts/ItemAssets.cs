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

    public Sprite Sword;
    public Sprite Chicken;
    public Sprite Sun;
    public Sprite Potion;
    public Sprite Heart;
}
