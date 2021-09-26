using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PowerUpItem_Slots : MonoBehaviour , ISaveable 
{
    public int slotIDNumber;
    public bool isFree { get; private set; } = true;
    public GameObject containedItem { get;  set; }
    //private Panel_PowerUpItems panelPowerUpItems;
    private float lerpDuration = .15f;
 

    //public static Action<PointerEventData> onPowerUpItemLongClicked;

    private void Awake()
    {
        //panelPowerUpItems = FindObjectOfType<Panel_PowerUpItems>();
    }

    public void Drop(GameItems gameItem)
    {
        Image gameItemImage = gameItem.GetComponent<Image>();

        gameItemImage.raycastTarget = false;

        gameItemImage.maskable = false;
        gameItem.isInsidePowerUpPanel = true;
        Vector3 itemDropPos = transform.InverseTransformPoint(gameItem.transform.position);

        PlaceItem(gameItem, itemDropPos,gameItemImage);
        UpdateParentSlot(gameItem);
        isFree = false;
    }

    void PlaceItem(GameItems gameItemIN, Vector2 itemDroppedPosIN, Image gameItemImage)
    {
        RectTransform rtslot = GetComponent<RectTransform>();
        RectTransform rt = gameItemIN.GetComponent<RectTransform>();
        rt.SetParent(this.transform,false);
        rt.sizeDelta = new Vector2(rtslot.sizeDelta.y, rtslot.sizeDelta.y);
        rt.localScale = new Vector3(1, 1, 1);
        containedItem = gameItemIN.gameObject;
        rt.SetAsLastSibling();
        //rt.anchoredPosition = new Vector3 (0,0,0);
        //gameItemIN.isMoving = false;
        gameItemIN.initialGameSlot = this.gameObject; // bu iki tane var ??? belki bu gereksiz 
        StartCoroutine(LerpItemPositionEnum(rt, itemDroppedPosIN,gameItemImage));
    }

    IEnumerator LerpItemPositionEnum(RectTransform rtIN, Vector2 itemDroppedPosIN, Image gameItemImage)
    {
        float elapsedTime = 0f;
        Vector3 lerpPosition = new Vector2(0, 0);
        while (elapsedTime < lerpDuration)
        {            
            rtIN.anchoredPosition = Vector2.Lerp(itemDroppedPosIN, lerpPosition, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rtIN.anchoredPosition = lerpPosition;
        gameItemImage.maskable = true;

        
    }

    void UpdateParentSlot(GameItems gameItemIN)
    {
        gameItemIN.initialGameSlot = this.gameObject;

    }

    public void DischargeItem()
    {
        isFree = true;
        containedItem = null;
    }

    public object CaptureState()
    {
        Dictionary<string, object> _dictFromItem = new Dictionary<string, object>();

        if(containedItem != null)
        {
            _dictFromItem = (Dictionary<string, object>)containedItem.GetComponent<GameItems>().CaptureState();
        }

        return _dictFromItem;
    }

    public void RestoreState(object state)
    {
        Dictionary<string, object> _dictFromItemIN = (Dictionary<string, object>)state;

        GameObject gameItemToLoad = ItemBag.Instance.GenerateItem(_dictFromItemIN);
        gameItemToLoad.transform.SetParent(this.transform);
        Drop(gameItemToLoad.GetComponent<GameItems>());

        //GameObject gameItemToLoad = new GameObject();
        //gameItemToLoad.transform.SetParent(this.transform);

        //Debug.Log("powerupitem slots restorestate  workign");

        //gameItemToLoad.AddComponent<GameItems>().RestoreState(_dictFromItemIN);
        //Drop(gameItemToLoad.GetComponent<GameItems>());
    }
    /*
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!containedItem) return;
        else 
        {
            longPressDetect_CR = StartCoroutine(DetectLongPress());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (longPressDetect_CR != null) StopCoroutine(longPressDetect_CR); 
        if(containedItem) containedItem.GetComponent<Image>().raycastTarget = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (longPressDetect_CR != null)
        {
            StopCoroutine(longPressDetect_CR);
        }

        if(containedItem) SetLongPressVars(false);
    }


    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        
            ExecuteEvents.Execute(containedItem, eventData, ExecuteEvents.initializePotentialDrag);
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLongPressed)
        {
            eventData.pointerDrag = containedItem.gameObject;
            ExecuteEvents.Execute(containedItem, eventData, ExecuteEvents.beginDragHandler);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLongPressed)
        {
            eventData.pointerDrag = containedItem.gameObject;
            ExecuteEvents.Execute(containedItem, eventData, ExecuteEvents.dragHandler);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLongPressed)
        {
            Debug.Log(eventData);
            ExecuteEvents.Execute(containedItem, eventData, ExecuteEvents.endDragHandler);
        }
    }


    private IEnumerator DetectLongPress()
    {
        float elapsedTime = 0f;

        while (elapsedTime < .5f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetLongPressVars(true);

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        ExecuteEvents.Execute(containedItem, eventData, ExecuteEvents.dragHandler);
       
    }

    private void SetLongPressVars(bool isLongPressedIN)
    {
        containedItem.GetComponent<Image>().raycastTarget = isLongPressedIN;
        isLongPressed = isLongPressedIN;
    }
    */

}
