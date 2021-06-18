using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Panel_Invetory : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 downscaleFactor;
    private Vector3 upScaleFactor;
    private Vector3 originalScale;
    private float lerpDuration = .12f;

    

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = new Vector3(rectTransform.localScale.x, rectTransform.localScale.y, rectTransform.localScale.z);
        downscaleFactor = new Vector3(.3f, .3f, 1);
        upScaleFactor = new Vector3(1.2f, 1.2f, 1);

    }

    private void OnEnable()
    {

        Vector3 startingScale = Vector3.Scale(originalScale, downscaleFactor);
        rectTransform.localScale = startingScale;

        StartCoroutine(UpSizePanel(startingScale));
    }


    void Start()
    {
       
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
    }

    
}
