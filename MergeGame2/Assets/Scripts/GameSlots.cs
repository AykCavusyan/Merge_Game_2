using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameSlots : MonoBehaviour
{
    public GameObject panel_Gameslots;
    private Transform crossMark;
    public List<DropConditions> dropConditions = new List<DropConditions>();
    [SerializeField] public bool canDrop { get; private set; }
    [SerializeField] public GameObject containedItem { get; private set; }

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
        canDrop = true;
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


    public void Drop (GameItems gameItem, Vector3 itemDroppedPosition = default(Vector3))
    {
        OnDropHandler?.Invoke(gameItem); // is there even a listener ??

        if (itemDroppedPosition == default(Vector3))
        {
            Debug.Log("sizeItemcalled");
            itemDroppedPosition = gameItem.transform.position;
        }

        if (gameItem.transform.localScale == default(Vector3))
        {
            SizeItem(gameItem);
        }

        
        PlaceItem(gameItem, itemDroppedPosition);
        UpdateItemParentSlot(gameItem);
        crossMark.gameObject.SetActive(false);
        canDrop = false;

        onSlotFilled?.Invoke(this, new OnSlotAvailabilityEventHandler { gameSlot = this.gameObject, gameItem=containedItem.GetComponent<GameItems>()});
        //OnDropped?.Invoke(this, new OnDroppedEventHandler { gameItem = gameItem });
    }

   
    private void SizeItem(GameItems gameItem)
    {
        StartCoroutine(LerpItemSize(gameItem));
    }

    private void PlaceItem(GameItems gameItem, Vector3 itemDroppedPosition)
    { 
        // size down the gameItem // this can be done in another way 
        gameItem.GetComponent<RectTransform>().sizeDelta = new Vector2(122, 122);
        // place the gameobjec in the contained item so that the script knows
        containedItem = gameItem.gameObject;
        //set the parent back to slots panel
        gameItem.GetComponent<RectTransform>().SetParent(panel_Gameslots.transform);
        gameItem.GetComponent<RectTransform>().SetAsLastSibling();
        // smoothly position the item
        StartCoroutine(LerpItemPositions(itemDroppedPosition, gameItem));
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
        gameItem.transform.position = transform .position;
    }

    public Vector3 GetLerpAnchorPoint(Vector3 itemDroppedPosition)
    {
        //float anchorX;
        float anchorY;

        if (itemDroppedPosition.y == transform.position.y)
        {
            anchorY = transform.position.y + 1;
            
        }


        //float pivotPointX;
        float dx;
        float dy;

        if (itemDroppedPosition.x < transform.position.x )
        {
           // pivotPointX = transform.position.x;
             dx = transform.position.x - itemDroppedPosition.x;
        }
        else
        {
            //pivotPointX = itemDroppedPosition.x;
             dx =  itemDroppedPosition.x - transform.position.x ;
        }

       // float pivotPointY ;
        if (itemDroppedPosition.y < transform.position.y)
        {
            //pivotPointY = transform.position.y;
            dy = transform.position.y - itemDroppedPosition.y;
        }
        else
        {
            //pivotPointY = itemDroppedPosition.y;
            dy =  itemDroppedPosition.y - transform.position.y;
        }



        // float dx = transform.position.x - itemDroppedPosition.x;
        // float dy = transform.position.y - itemDroppedPosition.y;

        float lenghtAB = Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
        float angleAB = Mathf.Atan(dy / dx);

        Vector3 lerpAnchorPoint = new Vector3();
        lerpAnchorPoint.x = lenghtAB * Mathf.Cos(angleAB + 60* Mathf.PI / 180) + transform.position.x;
        lerpAnchorPoint.y = lenghtAB * Mathf.Cos(angleAB + 60* Mathf.PI / 180) + transform.position.y;
        lerpAnchorPoint.z = 0;
        Debug.Log(lerpAnchorPoint);

        return lerpAnchorPoint;
    }
}
