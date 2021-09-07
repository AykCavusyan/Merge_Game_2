using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Panel_PowerUpItems_ScrollSwitcher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IPointerExitHandler
                                                               ,IBeginDragHandler, IDragHandler
{
    private Canvas canvas;

    private Coroutine cr = null;
    private GameObject selectedItem = null;
    private Image selectedItemImage = null;
    private bool isLongPress = false;
    private bool isInsideDetachPoint = false;
    private ScrollRectFaster scrollRect;

    Panel_PowerUpItems panelPowerUp;
    float localXPosFirst;
    float localXPosLast;

    GameObject firstSlot;
    GameObject lastSlot;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        scrollRect = GetComponent<ScrollRectFaster>();
        scrollRect.scrollFactor = (scrollRect.scrollFactor / canvas.scaleFactor) +1;
        panelPowerUp = GetComponentInParent<Panel_PowerUpItems>();

        localXPosFirst = 0 - GetComponent<RectTransform>().sizeDelta.x / 3.5f;
        localXPosLast = GetComponent<RectTransform>().sizeDelta.x / 3.5f;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        PowerUpItem_Slots powerUpSlot = null;

        foreach (var result in results)
        {
            powerUpSlot = result.gameObject.GetComponent<PowerUpItem_Slots>();

            if(powerUpSlot != null && powerUpSlot.containedItem != null)
            {
                selectedItem = powerUpSlot.containedItem;
                selectedItemImage = powerUpSlot.GetComponent<Image>();
                cr = StartCoroutine(DetectLongPress());
          
                ExecuteEvents.Execute(selectedItem, eventData, ExecuteEvents.pointerDownHandler); // this is for the itembar to display iteminfo on clickdown

                break;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (selectedItem)
        {
            selectedItem = null;
        }

        if (cr != null)
        {
            StopCoroutine(cr);
        }

        isLongPress = false;
        isInsideDetachPoint = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cr != null)
        {
            StopCoroutine(cr);
        }

        isLongPress = false;
        isInsideDetachPoint = false;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (selectedItem)
        {
            firstSlot = panelPowerUp.slotList.First();
            lastSlot = panelPowerUp.slotList.Last();
        }
              
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (isLongPress || isInsideDetachPoint)
        {           
            eventData.pointerDrag = selectedItem;
            ExecuteEvents.Execute(selectedItem, eventData, ExecuteEvents.dragHandler);
        }

        if (selectedItem && !isInsideDetachPoint && this.transform.InverseTransformPoint(firstSlot.transform.position).x > localXPosFirst)
        {
            isInsideDetachPoint = true;
            EqualizeItemPositionToCursor();

            ExecuteEvents.Execute(selectedItem, eventData, ExecuteEvents.initializePotentialDrag);
            selectedItemImage.raycastTarget = true;

            eventData.pointerDrag = selectedItem;
            ExecuteEvents.Execute(selectedItem, eventData, ExecuteEvents.beginDragHandler);

            scrollRect.m_Dragging = false;
        }

        else if (selectedItem && !isInsideDetachPoint && this.transform.InverseTransformPoint(lastSlot.transform.position).x < localXPosLast)
        {
            isInsideDetachPoint = true;
            EqualizeItemPositionToCursor();

            ExecuteEvents.Execute(selectedItem, eventData, ExecuteEvents.initializePotentialDrag);
            selectedItemImage.raycastTarget = true;

            eventData.pointerDrag = selectedItem;
            ExecuteEvents.Execute(selectedItem, eventData, ExecuteEvents.beginDragHandler);

            scrollRect.m_Dragging = false;
        }
    }
    /*
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLongPress)
        {
            eventData.pointerDrag = selectedItem;
            ExecuteEvents.Execute(selectedItem, eventData, ExecuteEvents.endDragHandler);
        }
      
    }*/

    private IEnumerator DetectLongPress()
    {
        float elapsedTime = 0f;

        while (elapsedTime < .5f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        PowerUpItem_Slots powerUpSlot = null;

        foreach (var result in results)
        {
            powerUpSlot = result.gameObject.GetComponent<PowerUpItem_Slots>();

            if(powerUpSlot != null && powerUpSlot.containedItem == selectedItem)
            {
                ExecuteEvents.Execute(selectedItem, eventData, ExecuteEvents.initializePotentialDrag);
                isLongPress = true;
                selectedItemImage.raycastTarget = true;

                eventData.pointerDrag = selectedItem;
                ExecuteEvents.Execute(selectedItem, eventData, ExecuteEvents.beginDragHandler);

                scrollRect.m_Dragging = false;
                //eventData.pointerDrag = selectedItem;
                //ExecuteEvents.Execute(selectedItem, eventData, ExecuteEvents.dragHandler);

                //scrollRect.StopMovement();
                break;
            }
        }

    }
    
    private void EqualizeItemPositionToCursor()
    {
        if (!selectedItem) return;
        selectedItem.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    //private IEnumerator DetectScrollRange()
    //{
    //    var firstSlot = panelPowerUp.slotList.First();
    //    var lastSlot = panelPowerUp.slotList.Last();

    //    //this.transform.InverseTransformPoint(panelPowerUp.slotList.First().transform.position);
    //    //this.transform.InverseTransformPoint(panelPowerUp.slotList.Last().transform.position);
    //    while (this.transform.InverseTransformPoint(firstSlot.transform.position).x < localXPosFirst ||
    //           this.transform.InverseTransformPoint(lastSlot.transform.position).x > localXPosLast)
    //    {
    //        Debug.Log("inside desired point");
    //    }

    //    Debug.Log(localXPosFirst);
    //    Debug.Log(localXPosLast);

    //    while (scrollRect.horizontalNormalizedPosition >-10.5 && scrollRect.horizontalNormalizedPosition <13 || 
    //           scrollRect.horizontalNormalizedPosition < 13 && scrollRect.horizontalNormalizedPosition > -10.5)
    //    {
    //        yield return null;
    //    }

    //    Debug.Log("scrollrect position got");
    //}
    
}
