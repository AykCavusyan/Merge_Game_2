using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Panel_Invetory : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    private RectTransform rectTransform;
    private Vector3 downscaleFactor;
    private Vector3 upScaleFactor;
    private Vector3 originalScale;
    private Vector3 zeroScale;
    private float lerpDuration = .09f;
    public int panelIndex; // bunun public olmasýna sonra çare bulalým 

    public event EventHandler<EventArgs> OnPanelSized;
    public event EventHandler<EventArgs> OnPanelDisappear;

    private Image[] childImagesToEnable;
    private Text[] textToEnable;

    private GameObject backgroundPanel;
    //private GameObject panelTextBox;

    private void Awake()
    {
        backgroundPanel = GameObject.Find("Background_PanelHolder");

        textToEnable = transform.GetComponentsInChildren<Text>(true);
        childImagesToEnable = transform.GetComponentsInChildren<Image>(true);

        rectTransform = GetComponent<RectTransform>();
        originalScale = new Vector3(rectTransform.localScale.x, rectTransform.localScale.y, rectTransform.localScale.z);
        downscaleFactor = new Vector3(.3f, .3f, 1);
        upScaleFactor = new Vector3(1.2f, 1.2f, 1);

        zeroScale = new Vector3(0, 0, 0);
        rectTransform.localScale = zeroScale;

    }

    private void OnEnable()
    {
        backgroundPanel.GetComponent<Panel_BackgroundPanelHolder>().OnenableVisibility += EnableVisibility;
        backgroundPanel.GetComponent<Panel_BackgroundPanelHolder>().OnDisableVisibility += DisableVisibility;
        
    }

    void OnDisable()
    {
        backgroundPanel.GetComponent<Panel_BackgroundPanelHolder>().OnenableVisibility -= EnableVisibility;
        backgroundPanel.GetComponent<Panel_BackgroundPanelHolder>().OnDisableVisibility -= DisableVisibility;
        
    }


    void Start()
    {
        ChildEnablerDisabler(false);
        Debug.Log(childImagesToEnable.Length);
    }

    void EnableVisibility(object sender, Panel_BackgroundPanelHolder.OnEnableVisibilityEventArgs e)
    {
        if (panelIndex == e.panelIndex)
        {
            StopAllCoroutines();
            ChildEnablerDisabler(true);

            Vector3 startingScale = Vector3.Scale(originalScale, downscaleFactor);
            rectTransform.localScale = startingScale;

            StartCoroutine(UpSizePanel(startingScale));
        }
    }

    void DisableVisibility(object sender, Panel_BackgroundPanelHolder.OnDisableVisibilityEventArgs e)
    {
        if (panelIndex == e.activePanel)
        {
            StopAllCoroutines();
            OnPanelDisappear?.Invoke(this , EventArgs.Empty);
            StartCoroutine(DownsizePanel());   
        }
    }


    IEnumerator UpSizePanel(Vector3 startingScale)
    {
        float elapsedTime = 0f;
        Vector3 lerpSize = Vector3.Scale(originalScale, upScaleFactor);

        while (elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(startingScale, lerpSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.localScale = lerpSize;
        StartCoroutine(NormalSizePanel(rectTransform.localScale));

    }

    IEnumerator NormalSizePanel(Vector3 upScale)
    {
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(upScale, originalScale, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rectTransform.localScale = originalScale;

        OnPanelSized?.Invoke(this, EventArgs.Empty );
    }

    IEnumerator DownsizePanel()
    {
        float elapsedTime = 0f;
        

        while (elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(originalScale, zeroScale, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.localScale = zeroScale;

        ChildEnablerDisabler(false);
    }

    //void UpdateChildImagesArray() 
    //{
    //    childImagesToEnable = transform.GetComponentsInChildren<Image>(true);

    //}

    void ChildEnablerDisabler(bool condition)
    {
       // UpdateChildImagesArray();
        Debug.Log(childImagesToEnable.Length);


        for (int i = 0; i < textToEnable.Length; i++)
        {
            textToEnable[i].gameObject.SetActive(condition);
        }

        for (int i = 0; i < childImagesToEnable.Length; i++)
        {
            childImagesToEnable[i].enabled = condition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    
    }

    public void OnPointerUp(PointerEventData eventData)
    {
     
    }


}
