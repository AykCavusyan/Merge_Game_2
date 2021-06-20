using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ribbon_Panel : MonoBehaviour
{
    private RectTransform rectTransform;
    //private Vector3 originalPosition;
    private Vector3 startingPosition;
    private float lerpDuration = .05f;
    private GameObject parentPanel;
    private Image image;

    Vector3 lerpedSize;
    Vector3 originalScale;

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
        parentPanel = transform.parent.transform.Find("Panel_BackToGame").gameObject;
        //originalPosition = this.transform.parent.transform.Find("RibbonAnchorPoint").GetComponent<RectTransform>().anchoredPosition;
        originalScale = rectTransform.localScale;
        lerpedSize = new Vector3(1.2f, .85f, 1f);
    }

    void Start()
    {
        //Debug.Log(rectTransform.anchoredPosition);
    }

    private void OnEnable()
    {
        parentPanel.GetComponent<Panel_Invetory>().OnPanelSized += PlaceRibbon;
        parentPanel.GetComponent<Panel_Invetory>().OnPanelDisappear += DeplaceRibbon;
    }

    private void OnDisable()
    {
        parentPanel.GetComponent<Panel_Invetory>().OnPanelSized -= PlaceRibbon;
        parentPanel.GetComponent<Panel_Invetory>().OnPanelDisappear -= DeplaceRibbon;

    }

    void Update()
    {
        
    }

    void PlaceRibbon(object sender, Panel_Invetory.OnPanelSizedEventArgs e)
    {
        //Debug.Log(rectTransform.anchoredPosition);
        if (image.enabled == false)
        {
            image.enabled = true;
        }

        StartCoroutine(RibbonMove(e.ribbonAnchorPoint));
    }

    void DeplaceRibbon(object sender, Panel_Invetory.OnPanelDisappearEventArgs e)
    {
        StartCoroutine(RibbonDisappear());
        Debug.Log("ribbondisappear called");
        
    }

    IEnumerator RibbonMove(Vector3 ribbonAnchorPoint)
    {
        float elapsedTime = 0f;

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
}
