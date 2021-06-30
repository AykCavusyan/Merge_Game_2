using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class XButton_Panel : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    private RectTransform rectTransform;
    private Vector3 startingPosition;
    private float lerpDuration = .05f;
    private GameObject backgroundPanelHolder;
    private Image image;
    

    Vector3 lerpedSize;
    Vector3 originalScale;

    Vector3 xButtonAnchorPoint;

    public event Action onXButtonPressed;
    [SerializeField] private int panelIndex;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (image.enabled == true)
        {
            image.enabled = false;
        }

        rectTransform = GetComponent<RectTransform>();
        startingPosition = rectTransform.anchoredPosition;
        //Debug.Log(rectTransform.anchoredPosition);
        backgroundPanelHolder = GameObject.Find("Background_PanelHolder");
        //xButtonAnchorPoint = transform.parent.Find("XButtonAnchorPoint").GetComponent<RectTransform>().anchoredPosition;
        xButtonAnchorPoint = transform.parent.GetChild(1).GetComponent<RectTransform>().anchoredPosition;
        //originalPosition = this.transform.parent.transform.Find("RibbonAnchorPoint").GetComponent<RectTransform>().anchoredPosition;
        originalScale = rectTransform.localScale;
        lerpedSize = new Vector3(.85f, 1.2f, 1f);
    }

    private void OnEnable()
    {
        backgroundPanelHolder.GetComponent<Panel_BackgroundPanelHolder>().OnenableVisibility += PlaceXButton;
        backgroundPanelHolder.GetComponent<Panel_BackgroundPanelHolder>().OnDisableVisibility += DeplaceXButton;
    }

    private void OnDisable()
    {
        backgroundPanelHolder.GetComponent<Panel_BackgroundPanelHolder>().OnenableVisibility -= PlaceXButton;
        backgroundPanelHolder.GetComponent<Panel_BackgroundPanelHolder>().OnDisableVisibility -= DeplaceXButton;
    }

    void PlaceXButton(object sender, Panel_BackgroundPanelHolder.OnEnableVisibilityEventArgs e)
    {
        Debug.Log("placeitem X button called");
        if (panelIndex == e.panelIndex)
        {
            StopAllCoroutines();
            if (image.enabled == false)
            {
                image.enabled = true;
            }

            StartCoroutine(XButtonMove(xButtonAnchorPoint));
        }
    }

        

    void DeplaceXButton(object sender, Panel_BackgroundPanelHolder.OnDisableVisibilityEventArgs e)
    {
        if ( panelIndex == e.activePanel)
        {
            StopAllCoroutines();
            StartCoroutine(XButtonDisappear());
        }

        
    }

    IEnumerator XButtonMove(Vector3 xButtonAnchorPoint)
    {
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            rectTransform.anchoredPosition = Vector3.Lerp(startingPosition, xButtonAnchorPoint, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.anchoredPosition = xButtonAnchorPoint;
        StartCoroutine(XButtonSize());
    }

    IEnumerator XButtonSize()
    {
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(originalScale, lerpedSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.localScale = lerpedSize;
        StartCoroutine(XButtonResize(lerpedSize));
    }

    IEnumerator XButtonResize(Vector3 lerpedSize)
    {
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(lerpedSize, originalScale, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.localScale = originalScale;
    }

    IEnumerator XButtonDisappear()
    {
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, startingPosition, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.anchoredPosition = startingPosition;
        if (image.enabled == true)
        {
            image.enabled = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onXButtonPressed?.Invoke();
    }
}
