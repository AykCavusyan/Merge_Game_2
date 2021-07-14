using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Base : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected RectTransform rectTransform;
    public int buttonlIndex;

    public event EventHandler<OnButtonPressedEventArgs> OnButtonPressed;
    public class OnButtonPressedEventArgs : EventArgs
    {
        public int buttonIndex;
    }

    
    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void RaiseOnButtonPressed(OnButtonPressedEventArgs e)
    {
        EventHandler<OnButtonPressedEventArgs> invoker = OnButtonPressed;
        if (invoker != null) invoker(this, e);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnButtonPressed?.Invoke(this, new OnButtonPressedEventArgs { buttonIndex = this.buttonlIndex });
    }
}
