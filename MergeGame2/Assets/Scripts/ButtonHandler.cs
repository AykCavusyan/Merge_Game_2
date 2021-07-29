using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHandler : Button_Base , IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    private Vector3 downScaleFactor;
    private Vector3 upScaleFactor;
    private float lerpDuration = .12f;

    private Image notificationImage;
    private Text notificationText;
    private Dictionary<Button_CompleteQuest,bool> buttonOriginalStatesDict = new Dictionary<Button_CompleteQuest, bool>();



    protected override void Awake()
    {
        base.Awake();
        originalScale = new Vector3(rectTransform.localScale.x, rectTransform.localScale.y, rectTransform.localScale.y);
        downScaleFactor = new Vector3(0.9f, 0.9f, 1);
        upScaleFactor = new Vector3(1.1f, 1.1f, 1);

    }
    private void OnDisable()
    {
        UnSubscribeFromEvents();
    }

    public void  SubscribeToEvents(Button_CompleteQuest buttonIn)
    {

        Debug.Log("subscribed");
        buttonIn.OnQuestCanComplete += NotificationBehavior;
        QuestManager.Instance.OnQuestCompleted += StopListeningQuestButton;
        buttonOriginalStatesDict.Add(buttonIn, buttonIn.canClaim);


        notificationImage = transform.GetChild(1).GetComponent<Image>();
        notificationImage.enabled = false;
        notificationText = notificationImage.transform.GetChild(0).GetComponent<Text>();
        notificationText.enabled = false;


    }

    protected virtual void UnSubscribeFromEvents()
    {
        if (buttonIndex == 3)
        {
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



    void StopListeningQuestButton(object sender, QuestManager.OnQuestAddRemoveEventArgs e)
    {
        if (e.button_CompleteQuest) 
        {

            buttonOriginalStatesDict.Remove(e.button_CompleteQuest);
            GetCurrentnotificationAmount();
            e.button_CompleteQuest.OnQuestCanComplete -= NotificationBehavior;
        }         
    }

    private void NotificationBehavior(Button_CompleteQuest sender, bool canDo)
    {
        Debug.Log("notiffication behav listenint woking ");

        if (canDo)
        {
            if (!notificationImage.enabled) EnableNotification();
            buttonOriginalStatesDict[sender] = true;

        }
        else
        {
            buttonOriginalStatesDict[sender] = false;
        }

        GetCurrentnotificationAmount();
    }

    void GetCurrentnotificationAmount()
    {
        int notificationAmount = 0;
        foreach (KeyValuePair<Button_CompleteQuest, bool> pair in buttonOriginalStatesDict)
        {
            if (pair.Value == true) notificationAmount++;
        }
        notificationText.text = notificationAmount.ToString();

        if (notificationAmount < 1) DisableNotification();
    }

    void EnableNotification()
    {
        notificationImage.enabled = true;
        notificationText.enabled = true;
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
