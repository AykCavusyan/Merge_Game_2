using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameSlots : MonoBehaviour, ISaveable
{
    private GameObject panel_GameItems;
    public GameObject panel_Gameslots { get; private set; }
    private Vector2 containedItemSize;
    private RectTransform rt;
    private Transform crossMark;
    public List<DropConditions> dropConditions = new List<DropConditions>();
    [SerializeField] public bool canDrop { get; private set; }
    [SerializeField] public GameObject containedItem { get; private set; }

    public event Action<GameItems> OnDropHandler;

    public event EventHandler<OnSlotAvailabilityEventHandler> onSlotFilled;
    public event EventHandler<OnSlotAvailabilityEventHandler> onSlotDischarged;
    public class OnSlotAvailabilityEventHandler : EventArgs
    {
        public GameObject gameSlot;
        public GameItems gameItem;
    }



    private void Awake()
    {
        canDrop = true;
        rt = GetComponent<RectTransform>(); 
        crossMark = transform.Find("CrossMark");
        panel_Gameslots = GameObject.Find("Panel_GameSlots");
        panel_GameItems = GameObject.Find("Panel_GameItems");
        containedItemSize = GetComponent<RectTransform>().sizeDelta *.85f;
    }

    private void Start()
    {
        if (canDrop == true)
        {
            onSlotDischarged?.Invoke(this, new OnSlotAvailabilityEventHandler { gameSlot = this.gameObject });
        }
    }


    public bool Accepts (GameItems gameItem)
    {
        return dropConditions.TrueForAll(cond => cond.Check(gameItem));
    }


    public void Drop (GameItems gameItem, Vector3 itemDroppedPositionIN =  default(Vector3))
    {
        OnDropHandler?.Invoke(gameItem); // is there even a listener ??
        gameItem.isInsidePowerUpPanel = false;
        //gameItem.isMoving = false;

        Vector3 itemDroppedPosition = new Vector3(); // is it necessary ???

        if (itemDroppedPositionIN == default(Vector3)) itemDroppedPosition = gameItem.transform.position;
        
        else itemDroppedPosition = itemDroppedPositionIN;
        
        if (gameItem.transform.localScale == default(Vector3))
        {
            SizeItem(gameItem);
        }

        
        PlaceItem(gameItem, itemDroppedPosition);
        UpdateItemParentSlot(gameItem);
        crossMark.gameObject.SetActive(false);
        canDrop = false;

        onSlotFilled?.Invoke(this, new OnSlotAvailabilityEventHandler { gameSlot = this.gameObject, gameItem=containedItem.GetComponent<GameItems>()});
    }

   
    private void SizeItem(GameItems gameItem)
    {
        StartCoroutine(LerpItemSize(gameItem));
    }

    private void PlaceItem(GameItems gameItem, Vector3 itemDroppedPosition )
    {
        RectTransform rtGameItem = gameItem.GetComponent<RectTransform>();

        rtGameItem.SetParent(panel_GameItems.transform);
        rtGameItem.sizeDelta = containedItemSize;
        
        rtGameItem.localScale = new Vector3(1, 1, 1);
        containedItem = gameItem.gameObject;
        rtGameItem.SetAsLastSibling();

        if (itemDroppedPosition != transform.position) StartCoroutine(LerpItemPositions(itemDroppedPosition, gameItem));
        else gameItem.transform.position = transform.position;
    }

    void UpdateItemParentSlot(GameItems gameItem)
    {
        gameItem.initialGameSlot = this.gameObject;
    }

    public void DischargeSlot()
    {
        containedItem = null;
        canDrop = true;

        onSlotDischarged?.Invoke(this, new OnSlotAvailabilityEventHandler { gameSlot = this.gameObject });
    }
 
    IEnumerator LerpItemSize(GameItems gameItem)
    {   
        float lerpDuration = .5f;
        float timeElapsed = 0;
        
        while (timeElapsed < lerpDuration && gameItem)
        {

         gameItem.transform.localScale = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(1, 1, 1), timeElapsed / lerpDuration);
         timeElapsed += Time.deltaTime;           

         yield return null;
        }

        if (gameItem) gameItem.transform.localScale = new Vector3(1, 1, 1);
    }



    IEnumerator LerpItemPositions(Vector3 itemDroppedPosition,GameItems gameItem)
    {
        float lerpDuration = .4f;
        float timeElapsed = 0f;        
      
        Vector3 lerpAnchorPosiiton = GetLerpAnchorPoint(itemDroppedPosition);

        while (timeElapsed < lerpDuration)
        {
           
            Vector3 pointABposition = Vector3.Lerp(itemDroppedPosition, lerpAnchorPosiiton, timeElapsed / lerpDuration);
            Vector3 pointBCposition = Vector3.Lerp(lerpAnchorPosiiton, transform.position, timeElapsed / lerpDuration);

            gameItem.transform.position = Vector3.Lerp(pointABposition, pointBCposition, timeElapsed/lerpDuration);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        
        gameItem.transform.position = transform.position;
    }

    public Vector3 GetLerpAnchorPoint(Vector3 itemDroppedPositionIN)
    { 
        if (RectTransformUtility.RectangleContainsScreenPoint(rt, itemDroppedPositionIN))
        {
            Debug.Log(RectTransformUtility.RectangleContainsScreenPoint(rt, itemDroppedPositionIN));
            return transform.position;
        }

        Vector2 itemDroppedPosition = new Vector2(itemDroppedPositionIN.x, itemDroppedPositionIN.y);
        Vector2 slotPosition = new Vector2(transform.position.x, transform.position.y);

        Debug.DrawLine(itemDroppedPosition, slotPosition, Color.red, 1f);

        Vector2 middlePoint = (itemDroppedPosition+ slotPosition) / 2;
        Vector2 direction = (slotPosition - itemDroppedPosition).normalized;
        Vector2 perpandicularDirection = Vector2.Perpendicular(direction);
        Vector2 anchorPoint = middlePoint + perpandicularDirection * 2f;

        Debug.DrawLine(middlePoint, anchorPoint, Color.red, 1f);


        return new Vector3(anchorPoint.x,anchorPoint.y, 0);
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

        GameObject gameItemtoLoad = ItemBag.Instance.GenerateItem(_dictFromItemIN);

        Drop(gameItemtoLoad.GetComponent<GameItems>());
    }
}
