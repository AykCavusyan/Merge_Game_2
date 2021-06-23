using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector3 downScaleFactor;
    private Vector3 upScaleFactor;
    private float lerpDuration = .12f;
   
    public int buttonlIndex;



    public event EventHandler<OnButtonPressedEventArgs> OnButtonPressed;
    public class OnButtonPressedEventArgs : EventArgs
    {
        public int buttonIndex;
    }

    //private GameObject backgroundPanel;
    //private Transform canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = new Vector3(rectTransform.localScale.x, rectTransform.localScale.y, rectTransform.localScale.y);
        downScaleFactor = new Vector3(0.9f, 0.9f, 1);
        upScaleFactor = new Vector3(1.1f, 1.1f, 1);
        
        if (buttonlIndex == 1) 
        {
            GetInventoryStatus();
        }

        //canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Transform>();
        //backgroundPanel = canvas.Find("Background_PanelHolder").gameObject;
    }

    void GetInventoryStatus()
    {
        //BURASI YAPILACAK
    }

    void SendItemToEmptySlot() 
    {
        //BURASI YAPILACAK
    }

    void ButtonClicked()
    {
        StartCoroutine(DownSizeButton());
    }

    IEnumerator DownSizeButton()
    {
        float elapsedTime = 0f;
        Vector3 lerpSize = Vector3.Scale(originalScale, downScaleFactor);

        while (elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(originalScale, lerpSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.localScale = lerpSize;
        StartCoroutine(UpsizeButton(rectTransform.localScale));
    }

    IEnumerator UpsizeButton(Vector3 oldScale)
    {
        float elapsedTime = 0f;
        Vector3 lerpSize = Vector3.Scale(originalScale, upScaleFactor);

        while (elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(oldScale, lerpSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.localScale = lerpSize;
        StartCoroutine(NormalSizeButton(rectTransform.localScale));
    }

    IEnumerator NormalSizeButton(Vector3 oldScale)
    {
        float elapsedTime = 0f;
        

        while (elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(oldScale, originalScale, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.localScale = originalScale;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("pointerdown on menu button");
        ButtonClicked();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnButtonPressed?.Invoke(this, new OnButtonPressedEventArgs { buttonIndex = this.buttonlIndex });
        
        
        Debug.Log("called again pointerUP");
        //if (backgroundPanel.activeSelf == false)
        //{
        //    backgroundPanel.SetActive(true);
        //}
        
    }
}
