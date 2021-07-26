using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_Base : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected RectTransform rectTransform;
    public int buttonIndex;
    protected Image notificationImage;
    private object subscribedScriptObject;

    public event EventHandler<OnButtonPressedEventArgs> OnButtonPressed;
    public class OnButtonPressedEventArgs : EventArgs
    {
        public int buttonIndex;
    }

    
    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        SubscribeToEvents();
    }

    protected virtual void RaiseOnButtonPressed(OnButtonPressedEventArgs e)
    {
        EventHandler<OnButtonPressedEventArgs> invoker = OnButtonPressed;
        if (invoker != null) invoker(this, e);
    }

    protected virtual void SubscribeToEvents()
    {
        if(buttonIndex == 3)
        {
            notificationImage = transform.GetChild(1).GetComponent<Image>();
            notificationImage.enabled = false;
            subscribedScriptObject = FindObjectsOfType<Button_CompleteQuest>();

            Button_CompleteQuest[] subscribedScripts = (Button_CompleteQuest[])subscribedScriptObject;
            if (subscribedScripts.Length > 0)
            {
                foreach (Button_CompleteQuest buttonScript in subscribedScripts)
                {
                    buttonScript.OnQuestCanComplete += NotificationBehavior;
                }
            }
        }
    }

    protected virtual void UnSubscribeFromEvents()
    {
        if (buttonIndex == 3) 
        { 
            Button_CompleteQuest[] subscribedScripts = (Button_CompleteQuest[])subscribedScriptObject;
            if (subscribedScripts.Length > 0)

            foreach (Button_CompleteQuest buttonScript in subscribedScripts)
            {
                if (buttonScript) buttonScript.OnQuestCanComplete -= NotificationBehavior;
            }
        }
    }

    protected virtual void NotificationBehavior(bool canDo)
    {

    }


    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnButtonPressed?.Invoke(this, new OnButtonPressedEventArgs { buttonIndex = this.buttonIndex });
    }

    protected virtual void OnDisable()
    {
        UnSubscribeFromEvents();
    }
}
