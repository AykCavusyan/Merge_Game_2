using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_CompleteQuest : Button_Base, IPointerDownHandler,IPointerUpHandler
{
    private Color buttonOriginalColor;
    private Color buttonFadedColor;

    //private ButtonHandler buttonHandler;
    public bool canClaim;
    //private Item questReward;
    private Quest activeQuest;

    public Action<Button_CompleteQuest,bool> OnQuestCanComplete;
    public event EventHandler<OnQuestCompletedEventArgs> OnQuestCompleted;
    public class OnQuestCompletedEventArgs
    {
        public Quest quest;
    }


    protected override void Awake()
    {
        base.Awake();
        buttonOriginalColor = transform.GetChild(1).GetComponent<Image>().color;
        buttonFadedColor = new Color(.5f, .5f, .5f, .5f);

        transform.GetChild(1).GetComponent<Image>().color = buttonFadedColor;

        ButtonHandler[] buttonHandlerArray = FindObjectsOfType<ButtonHandler>();
        foreach (ButtonHandler button in buttonHandlerArray)
        {
            if (button.buttonIndex == 3)
            {
                button.SubscribeToEvents(this);

                return;
            }
        }
    }

    public void CreateButton(Quest quest)
    {
        activeQuest = quest;
        //SetButtonAvalibility(); // bu gerekli olmayabilir quest panel de bakýyor zaten baþlangýçta ??? 
    }

    public void SetButtonAvalibility(bool canCompleteIN = false)
    {
        if(canCompleteIN)
        {
            Debug.Log("buton availability on button initialization YES");
            canClaim = true;
            transform.GetChild(1).GetComponent<Image>().color = buttonOriginalColor;
            OnQuestCanComplete?.Invoke(this, canCompleteIN);
        }
        else
        {
            Debug.Log("buton availability on button initialization NO");

            canClaim = false;
            transform.GetChild(1).GetComponent<Image>().color = buttonFadedColor;
            OnQuestCanComplete?.Invoke(this, canCompleteIN);
        }
    }


    public new void OnPointerDown(PointerEventData eventData)
    {

    }

    public new void OnPointerUp(PointerEventData eventData)
    {
        if (canClaim == true)
        {
            OnQuestCompleted?.Invoke(this, new OnQuestCompletedEventArgs { quest = activeQuest});
        }
    }
}
