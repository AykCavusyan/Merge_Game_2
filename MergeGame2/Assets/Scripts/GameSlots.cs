using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameSlots : MonoBehaviour, ISaveable
{
    public GameObject panel_Gameslots;
    private RectTransform rt;
    private Transform crossMark;
    public List<DropConditions> dropConditions = new List<DropConditions>();
    [SerializeField] public bool canDrop { get; private set; }
    [SerializeField] public GameObject containedItem { get; private set; }
    //public string slotName { get; private set; }

    public event Action<GameItems> OnDropHandler;

    //public event EventHandler<OnDroppedEventHandler> OnDropped;
    //public class OnDroppedEventHandler : EventArgs
    //{
    //    public GameItems gameItem;
    //}

    public event EventHandler<OnSlotAvailabilityEventHandler> onSlotFilled;
    public event EventHandler<OnSlotAvailabilityEventHandler> onSlotDischarged;
    public class OnSlotAvailabilityEventHandler : EventArgs
    {
        public GameObject gameSlot;
        public GameItems gameItem;
    }



    private void Awake()
    {
        //slotName = gameObject.name;
        canDrop = true;
        rt = GetComponent<RectTransform>(); 
        crossMark = transform.Find("CrossMark");
        panel_Gameslots = GameObject.Find("Panel_GameSlots");

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
        Vector3 itemDroppedPosition = new Vector3();

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

        rtGameItem.SetParent(panel_Gameslots.transform);
        rtGameItem.sizeDelta = (GetComponent<RectTransform>().sizeDelta) * .85f ; //new Vector2(122, 122);
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

        while (timeElapsed < lerpDuration)
        {

            // bu kýsým daha iyi olabilir
            gameItem.transform.localScale = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(1, 1, 1), timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        gameItem.transform.localScale = new Vector3(1, 1, 1);
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

        gameItem.isMoving = false;
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

        #region
        //float anchorY;

        //if (itemDroppedPositionIN.y == transform.position.y)
        //{
        //    anchorY = transform.position.y + 1;

        //}

        //float dx;
        //float dy;

        //if (itemDroppedPositionIN.x < transform.position.x )
        //{
        //     dx = transform.position.x - itemDroppedPositionIN.x;
        //}
        //else
        //{
        //     dx =  itemDroppedPositionIN.x - transform.position.x ;
        //}

        //if (itemDroppedPositionIN.y < transform.position.y)
        //{
        //    dy = transform.position.y - itemDroppedPositionIN.y;
        //}
        //else
        //{
        //    dy =  itemDroppedPositionIN.y - transform.position.y;
        //}


        //float lenghtAB = Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
        //float angleAB = Mathf.Atan(dy / dx);

        //Vector3 lerpAnchorPoint = new Vector3();
        //lerpAnchorPoint.x = lenghtAB * Mathf.Cos(angleAB + 60* Mathf.PI / 180) + transform.position.x;
        //lerpAnchorPoint.y = lenghtAB * Mathf.Cos(angleAB + 60* Mathf.PI / 180) + transform.position.y;
        //lerpAnchorPoint.z = 0;

        //return lerpAnchorPoint;
        #endregion
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

        //GameObject gameItemtoLoad = new GameObject();
        //gameItemtoLoad.transform.SetParent(panel_Gameslots.transform);
        //gameItemtoLoad.AddComponent<GameItems>().RestoreState(_dictFromItemIN);

        Drop(gameItemtoLoad.GetComponent<GameItems>());
        

    }
}
