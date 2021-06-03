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


    public event EventHandler<OnDroppedEventHandler> OnDropped;
    public class OnDroppedEventHandler : EventArgs
    {
        public GameItems gameItem;
    }


    public List<DropConditions> dropConditions = new List<DropConditions>();
    public event Action<GameItems> OnDropHandler;
    
    

    private void Awake()
    {
        canDrop = true;
        crossMark = transform.Find("CrossMark");

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

        OnDropped?.Invoke(this, new OnDroppedEventHandler { gameItem = gameItem });
    }

   
    private void PlaceItem(GameItems gameItem)
    {

        //bu kýsmý düzenleme lazým

        
        if (gameItem.transform.position.x == 0 && gameItem.transform.position.y == 0 && gameItem.transform.position.z == 0)
        {
            Debug.Log("default called");
            gameItem.transform.position = transform.position;
        }
        else
        {
            Debug.Log("lerp called");
            StartCoroutine(LerpItemPositions(gameItem.transform.position, gameItem));
        }


    }


    IEnumerator LerpItemPositions(Vector3 itemDroppedPosition,GameItems gameItem)
    {
        float lerpDuration = .1f;
        float timeElapsed = 0f;

        while (timeElapsed < lerpDuration)
        {
            gameItem.transform.position = Vector3.Lerp(itemDroppedPosition, transform.position, timeElapsed/lerpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        gameItem.transform.position = transform .position;
    }
}
