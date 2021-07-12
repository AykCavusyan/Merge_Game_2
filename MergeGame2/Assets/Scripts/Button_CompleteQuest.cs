using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_CompleteQuest : Button_Base, IPointerDownHandler,IPointerUpHandler
{
    private Color buttonOriginalColor;
    private Color buttonFadedColor;

    public bool canClaim;
    //private Item questReward;
    private Quest activeQuest;

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
    }

    public void CreateButton(Quest quest)
    {
        activeQuest = quest;
        SetButtonAvalibility();
    }

    void SetButtonAvalibility(bool canCompleteIN = false)
    {
        if(canCompleteIN)
        {
            canClaim = true;
            transform.GetChild(1).GetComponent<Image>().color = buttonOriginalColor;
        }
        else
        {
            canClaim = false;
            transform.GetChild(1).GetComponent<Image>().color = buttonFadedColor;
        }
    }


    public new void OnPointerDown(PointerEventData eventData)
    {

    }

    public new void OnPointerUp(PointerEventData eventData)
    {
        if (canClaim == true)
        {
            OnQuestCompleted?.Invoke(this, new OnQuestCompletedEventArgs { quest = activeQuest });
        }
    }
}
