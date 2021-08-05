using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicImageResizeEffect : MonoBehaviour
{
    private Image image;
    private bool isActive = false;
    private Vector2 originalSize;
    private Vector2 lerpedSize;
    private float lerpDuration = .8f;
    private RectTransform rt;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalSize = GetComponent<RectTransform>().sizeDelta;
        lerpedSize = new Vector2(originalSize.x * 1.15f, originalSize.y * 1.15f);
        rt = GetComponent<RectTransform>();
    }
    void Update()
    {       
        if(image.enabled == false)
        {
            StopAllCoroutines();
            isActive = false;
            rt.sizeDelta = originalSize;
        }
        else if (image.enabled == true && isActive == false) Resize();
    }

    void Resize()
    {
        StartCoroutine(ResizeEnum());
    }

    IEnumerator ResizeEnum()
    {
        
        isActive = true;
        while (isActive)
        {
            
            float elapsedTime = 0f;
            while (elapsedTime < lerpDuration)
            {
                rt.sizeDelta = Vector2.Lerp(originalSize, lerpedSize, elapsedTime / lerpDuration);
                elapsedTime += Time.deltaTime;

                yield return null;
            }
            rt.sizeDelta = lerpedSize;

            elapsedTime = 0f;
            while (elapsedTime < lerpDuration)
            {
                rt.sizeDelta = Vector2.Lerp(lerpedSize, originalSize, elapsedTime / lerpDuration);
                elapsedTime += Time.deltaTime * 3f;

                yield return null;
            }
            rt.sizeDelta = originalSize;
            Debug.Log("updating lerp");
            yield return null;
        }
        
    }
}
