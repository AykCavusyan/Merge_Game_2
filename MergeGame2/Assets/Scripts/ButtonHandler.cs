using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class ButtonHandler : Button_Base , IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    private Vector3 downScaleFactor;
    private Vector3 upScaleFactor;
    private float lerpDuration = .12f;

    private Image notificationImage;
    private Text notificationText;
    private Dictionary<Button_CompleteQuest,bool> buttonOriginalStatesDict = new Dictionary<Button_CompleteQuest, bool>();
    private int notificationAmount = 0;
    //private object subscribedScriptObject;


    protected override void Awake()
    {
        base.Awake();
        originalScale = new Vector3(rectTransform.localScale.x, rectTransform.localScale.y, rectTransform.localScale.y);
        downScaleFactor = new Vector3(0.9f, 0.9f, 1);
        upScaleFactor = new Vector3(1.1f, 1.1f, 1);
        SubscribeToEvents();

    }
    private void OnDisable()
    {
        UnSubscribeFromEvents();
    }

    private void  SubscribeToEvents()
    {
        if (buttonIndex == 3)
        {
            notificationImage = transform.GetChild(1).GetComponent<Image>();
            notificationImage.enabled = false;
            notificationText = notificationImage.transform.GetChild(0).GetComponent<Text>();
            notificationText.enabled = false;

            QuestManager.Instance.OnQuestAdded += StartListeningQuestButtons;
            QuestManager.Instance.OnQuestCompleted += StopListeningQuestButton;
        }
    }

    protected virtual void UnSubscribeFromEvents()
    {
        if (buttonIndex == 3)
        {
            QuestManager.Instance.OnQuestAdded -= StartListeningQuestButtons;
            QuestManager.Instance.OnQuestCompleted -= StopListeningQuestButton;

            Button_CompleteQuest[] button_CompleteQuestArray = FindObjectsOfType<Button_CompleteQuest>();
            if (button_CompleteQuestArray.Length > 0)
            {
                foreach (Button_CompleteQuest button_CompleteQuest in button_CompleteQuestArray)
                {
                    if (button_CompleteQuest) button_CompleteQuest.OnQuestCanComplete -= NotificationBehavior;
                }
            }
        }
    }

    void StartListeningQuestButtons(object sender, QuestManager.OnQuestAddRemoveEventArgs e)
    {
        if (e.button_CompleteQuest)
        {
            e.button_CompleteQuest.OnQuestCanComplete += NotificationBehavior;
            buttonOriginalStatesDict.Add(e.button_CompleteQuest, e.button_CompleteQuest.canClaim);
        }
    }
    void StopListeningQuestButton(object sender, QuestManager.OnQuestAddRemoveEventArgs e)
    {
        if (e.button_CompleteQuest) 
        {
            DecrementNotification(e.button_CompleteQuest);
            buttonOriginalStatesDict.Remove(e.button_CompleteQuest);
            e.button_CompleteQuest.OnQuestCanComplete -= NotificationBehavior;
        }         
    }

    private void NotificationBehavior(Button_CompleteQuest sender, bool canDo)
    {
        if (!buttonOriginalStatesDict.ContainsKey(sender) || buttonOriginalStatesDict[sender] == canDo) return;

        if (canDo)
        {
            if (!notificationImage.enabled) EnableNotification();
            IncrementNotification(sender);
        }
        else
        {
            DecrementNotification(sender);
        }
    }
    void EnableNotification()
    {
        notificationImage.enabled = true;
        notificationText.enabled = true;
    }

    void IncrementNotification (Button_CompleteQuest senderIN)
    {
        buttonOriginalStatesDict[senderIN] = true;
        notificationAmount++;
        notificationText.text = notificationAmount.ToString();
    }

    void DecrementNotification(Button_CompleteQuest senderIN)
    {
        buttonOriginalStatesDict[senderIN] = false;
        notificationAmount--;
        notificationText.text = notificationAmount.ToString();
        if (notificationAmount < 1) DisableNotification();
    }

    void DisableNotification()
    {
        notificationText.enabled = false;
        notificationImage.enabled = false;
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
