using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Panel : MonoBehaviour
{
    private GameObject text_Popup;
    private Vector2 originalPositonText_Popup;
    private Vector2 targetPosition;
    private float lerpDuration = .1f;

    private void Awake()
    {
        targetPosition = transform.position;
        text_Popup = transform.GetChild(0).gameObject;
        originalPositonText_Popup = text_Popup.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7)) LerpIn();
    }

    void LerpIn()
    {
        Debug.Log("lerping in supposedly");
        StartCoroutine(LerpInEnum());
    }

    IEnumerator LerpInEnum()
    {
        float elapsedTime = 0;

        while (elapsedTime < lerpDuration)
        {
            text_Popup.transform.position = Vector2.Lerp(originalPositonText_Popup, targetPosition, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        text_Popup.transform.position = targetPosition;
    }
}
