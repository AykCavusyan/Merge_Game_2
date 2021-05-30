using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class GameSlots : MonoBehaviour
{
    private Transform crossMark;
    public bool canDrop;
    private GameObject itemBag;
    
    


    public List<DropConditions> dropConditions = new List<DropConditions>();
    public event Action<GameItems> OnDropHandler;
    
    

    private void Awake()
    {
        canDrop = true;
        crossMark = transform.Find("CrossMark");
        itemBag = GameObject.Find("Button");
    }

    private void Start()
    {
       
    }

    private void Update()
    {
        
    }

    public bool Accepts (GameItems gameItem)
    {
        return dropConditions.TrueForAll(cond => cond.Check(gameItem));
    }


    public void Drop (GameItems gameItem)
    {  
            OnDropHandler?.Invoke(gameItem);
            PlaceItem(gameItem);
            crossMark.gameObject.SetActive(false);
            canDrop = false;
    }

    public void Merge(GameItems firstItem, GameItems secondItem, Vector3 pos)
    {
        OnDropHandler?.Invoke(firstItem);
        MergeItem(firstItem, secondItem, pos);
        GetComponent<VisualEffects>().MergeAnimation(firstItem.transform.position, firstItem.GetComponent<Image>().sprite);
        canDrop = false;
    }

    private void MergeItem(GameItems firstItem, GameItems secondItem, Vector3 pos)
    {
        
        Destroy(firstItem.gameObject);
        Destroy(secondItem.gameObject);
        
        // bu kýsým düzgün deðil
        itemBag.GetComponent<ItemBag>().GenerateItem();
        GameObject newMergedItem = itemBag.GetComponent<ItemBag>().newGameItemIdentified;
        newMergedItem.transform.position = pos;

        


    }

    private void PlaceItem(GameItems gameItem)
    {
        gameItem.transform.position = transform.position;
    }

}
