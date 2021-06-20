using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class Panel_Invetory : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    private RectTransform rectTransform;
    private Vector3 downscaleFactor;
    private Vector3 upScaleFactor;
    private Vector3 originalScale;
    private float lerpDuration = .09f;

    private Image imageToDisable;

    private bool validForDisable = false;


    public Vector3 ribbonAnchorPoint;
    public Vector3 xButtonAnchorPoint;
    public event EventHandler<OnPanelSizedEventArgs> OnPanelSized;
    public event EventHandler<OnPanelDisappearEventArgs> OnPanelDisappear;
    public class OnPanelSizedEventArgs : EventArgs
    {
        public Vector3 ribbonAnchorPoint;
        public Vector3 xButtonAnchorPoint;
    }
    public class OnPanelDisappearEventArgs
    {

    }


    public int panelIndex;
    private GameObject backgroundPanel;

    private void Awake()
    {
        backgroundPanel = transform.parent.transform.parent.gameObject;


        imageToDisable = GetComponent<Image>();

        if (imageToDisable.enabled == true)
        {
            imageToDisable.enabled = false;
        }

        rectTransform = GetComponent<RectTransform>();
        originalScale = new Vector3(rectTransform.localScale.x, rectTransform.localScale.y, rectTransform.localScale.z);
        downscaleFactor = new Vector3(.3f, .3f, 1);
        upScaleFactor = new Vector3(1.2f, 1.2f, 1);
        ribbonAnchorPoint = transform.parent.transform.Find("RibbonAnchorPoint").GetComponent<RectTransform>().anchoredPosition;
        xButtonAnchorPoint = transform.parent.transform.Find("XButtonAnchorPoint").GetComponent<RectTransform>().anchoredPosition;

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
       
    }
    void EnableVisibility(object sender, Panel_BackgroundPanelHolder.OnEnableVisibilityEventArgs e)
    {
        if (panelIndex == e.panelIndex)
        {
            imageToDisable.enabled = true;

            Vector3 startingScale = Vector3.Scale(originalScale, downscaleFactor);
            rectTransform.localScale = startingScale;

            StartCoroutine(UpSizePanel(startingScale));
        }
    }

    void DisableVisibility(object sender, Panel_BackgroundPanelHolder.OnDisableVisibilityEventArgs e)
    {
        if (imageToDisable.enabled == true)
        {
            if (panelIndex == e.activePanel)
            {
                OnPanelDisappear?.Invoke(this , new OnPanelDisappearEventArgs { });
                StartCoroutine(DownsizePanel());
                
            }
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

        OnPanelSized?.Invoke(this, new OnPanelSizedEventArgs { ribbonAnchorPoint = ribbonAnchorPoint, xButtonAnchorPoint =xButtonAnchorPoint} );
    }

    IEnumerator DownsizePanel()
    {
        float elapsedTime = 0f;
        Vector3 zeroScale = new Vector3(0, 0, 0);

        while (elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(originalScale, zeroScale, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.localScale = zeroScale;
        imageToDisable.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //StartCoroutine(InputListener());
        Debug.Log("pointer down ");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("pointer up");
        //if (validForDisable == true)
        //{
        //    imageToDisable.enabled = false;
        //}
    }

    //IEnumerator InputListener()
    //{
    //    float validClickLimit = 1f;
    //    float elapsedTime = 0f;

    //    while (elapsedTime < validClickLimit)
    //    {
    //        validForDisable = true;
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    validForDisable = false;

    //}
}
