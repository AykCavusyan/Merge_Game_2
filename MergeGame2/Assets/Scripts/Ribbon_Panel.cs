using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ribbon_Panel : MonoBehaviour ,  IPointerDownHandler,IPointerUpHandler
{
    private RectTransform rectTransform;
    //private Vector3 originalPosition;
    private Vector3 startingPosition;
    private float lerpDuration = .05f;
    private GameObject backgroundPanelHolder;
    private Image image;
    [SerializeField] private int panelIndex;

    Vector3 lerpedSize;
    Vector3 originalScale;

    private Vector3 ribbonAnchorPoint;

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
        ribbonAnchorPoint = transform.parent.Find("RibbonAnchorPoint").GetComponent<RectTransform>().anchoredPosition;
        //originalPosition = this.transform.parent.transform.Find("RibbonAnchorPoint").GetComponent<RectTransform>().anchoredPosition;
        originalScale = rectTransform.localScale;
        lerpedSize = new Vector3(1.2f, .5f, 1f);
    }


    private void OnEnable()
    {
        backgroundPanelHolder.GetComponent<Panel_BackgroundPanelHolder>().OnenableVisibility += PlaceRibbon;
        backgroundPanelHolder.GetComponent<Panel_BackgroundPanelHolder>().OnDisableVisibility += DeplaceRibbon;
    }

    private void OnDisable()
    {
        backgroundPanelHolder.GetComponent<Panel_BackgroundPanelHolder>().OnenableVisibility -= PlaceRibbon;
        backgroundPanelHolder.GetComponent<Panel_BackgroundPanelHolder>().OnDisableVisibility -= DeplaceRibbon;

    }

    void Update()
    {
        
    }

    void PlaceRibbon(object sender, Panel_BackgroundPanelHolder.OnEnableVisibilityEventArgs e)
    {
        if (panelIndex == e.panelIndex)
        {
            StopAllCoroutines(); // gerekli mi deðil mi belli deðil ??
                                 //Debug.Log(rectTransform.anchoredPosition);
            if (image.enabled == false)
            {
                image.enabled = true;
            }

            StartCoroutine(RibbonMove(ribbonAnchorPoint));
        }
        
    }

    void DeplaceRibbon(object sender, Panel_BackgroundPanelHolder.OnDisableVisibilityEventArgs e)
    {
        if(panelIndex == e.activePanel)
        {
            StopAllCoroutines(); // gerekli mi deðil mi belli deðil ??
            StartCoroutine(RibbonDisappear());
        }

    }

    IEnumerator RibbonMove(Vector3 ribbonAnchorPoint)
    {
        float elapsedTime = 0f;
        yield return new WaitForSeconds(.15f);

        while (elapsedTime < lerpDuration)
        {
            rectTransform.anchoredPosition = Vector3.Lerp(startingPosition, ribbonAnchorPoint, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.anchoredPosition = ribbonAnchorPoint;
        StartCoroutine(RibbonSize());
    }

    IEnumerator RibbonSize()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(originalScale, lerpedSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.localScale = lerpedSize;
        StartCoroutine(RibbonResize(lerpedSize));
    }

    IEnumerator RibbonResize(Vector3 lerpedSize)
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


    IEnumerator RibbonDisappear()
    {
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.position, startingPosition, elapsedTime / lerpDuration);
            
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
        //necessary otherwise ribbon is transferring the click event to the background ;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //necessary otherwise ribbon is transferring the click event to the background 
    }
}
