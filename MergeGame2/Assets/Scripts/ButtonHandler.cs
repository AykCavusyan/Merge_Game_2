using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ButtonHandler : Button_Base , IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    private Vector3 downScaleFactor;
    private Vector3 upScaleFactor;
    private float lerpDuration = .12f;
   

    protected override void Awake()
    {
        base.Awake();
        originalScale = new Vector3(rectTransform.localScale.x, rectTransform.localScale.y, rectTransform.localScale.y);
        downScaleFactor = new Vector3(0.9f, 0.9f, 1);
        upScaleFactor = new Vector3(1.1f, 1.1f, 1);
        
    }

    protected override void NotificationBehavior(bool canDo)
    {
        if (canDo) notificationImage.enabled = true;
        else notificationImage.enabled = false;
    }

    private void ButtonClicked()
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

    public new void OnPointerDown(PointerEventData eventData)
    {
        ButtonClicked();
    }

    public new void OnPointerUp(PointerEventData eventData)
    {
        RaiseOnButtonPressed(new OnButtonPressedEventArgs { buttonIndex = this.buttonIndex});            
    }


}
