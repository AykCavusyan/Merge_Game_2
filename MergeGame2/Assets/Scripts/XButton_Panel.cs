using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XButton_Panel : MonoBehaviour
{
    private RectTransform rectTransform;
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
        lerpedSize = new Vector3(.85f, 1.2f, 1f);
    }

    private void OnEnable()
    {
        parentPanel.GetComponent<Panel_Invetory>().OnPanelSized += PlaceXButton;
        parentPanel.GetComponent<Panel_Invetory>().OnPanelDisappear += DeplaceXButton;
    }

    private void OnDisable()
    {
        parentPanel.GetComponent<Panel_Invetory>().OnPanelSized -= PlaceXButton;
        parentPanel.GetComponent<Panel_Invetory>().OnPanelDisappear -= DeplaceXButton;
    }

    void PlaceXButton(object sender, Panel_Invetory.OnPanelSizedEventArgs e)
    {
        if (image.enabled == false)
        {
            image.enabled = true;
        }

        StartCoroutine(XButtonMove(e.xButtonAnchorPoint));
    }

    void DeplaceXButton(object sender, Panel_Invetory.OnPanelDisappearEventArgs e)
    {
        StartCoroutine(XButtonDisappear());
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
}
