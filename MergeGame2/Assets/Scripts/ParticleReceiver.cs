using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleReceiver : MonoBehaviour
{
    private RectTransform rt;
    private Vector2 originalSize;
    private Vector2 lerpSize;
    private float lerpDuration = .15f;


    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        originalSize = rt.sizeDelta;
        lerpSize = new Vector2(originalSize.x * 1.2f, originalSize.y * 1.2f);
    }
    private void OnParticleCollision(GameObject other)
    {
        ChangeSize();
    }

    void ChangeSize()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeSizeEnum());
    }

    IEnumerator ChangeSizeEnum()
    {
        float elapsedTime = 0f;
        while (elapsedTime < lerpDuration)
        {
            rt.sizeDelta = Vector2.Lerp(originalSize, lerpSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rt.sizeDelta = lerpSize;

        elapsedTime = 0f;
        while (elapsedTime < lerpDuration)
        {
            rt.sizeDelta = Vector2.Lerp(lerpSize, originalSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rt.sizeDelta = originalSize;
    }
}
