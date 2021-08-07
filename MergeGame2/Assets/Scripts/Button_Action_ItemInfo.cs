using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_Action_ItemInfo : MonoBehaviour ,IPointerDownHandler
{

    private Text actionText;
    private ItemSelector itemSelector;

    public event EventHandler<MasterEventListener.OnFinancialEvent> OnItemSold;

    private void Awake()
    {
        actionText = transform.GetChild(2).GetComponent<Text>();
        itemSelector = GameObject.Find("Canvas").GetComponent<ItemSelector>();
        //gameObject.SetActive(false);
    }


    public void SetItemActionValue()
    {
        actionText.text = itemSelector.selectedItem.goldValue.ToString();
    }

    private void SellItem()
    {
        GameItems itemToSell = itemSelector.selectedItem;
        itemToSell.CollectItem();


        //OnItemSold?.Invoke(this, new MasterEventListener.OnFinancialEvent { itemValue = itemToSell.goldValue });
   
        //itemToSell.DestroyItem(itemToSell.gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SellItem();
    }

 
}
