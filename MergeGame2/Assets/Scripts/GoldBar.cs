using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldBar : MonoBehaviour
{
    private Text goldText;

    private bool cr_Running = false;
    private float lerpDuration = .3f;
    private static IEnumerator runningCoroutine = null;
    private static Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();


    private void Awake()
    {
        goldText = transform.GetChild(2).GetComponent<Text>();
    }

    private void OnEnable()
    {
        PlayerInfo.Instance.OnCurrentGoldChanged += UpdateText;
    }

    private void OnDisable()
    {
        PlayerInfo.Instance.OnCurrentGoldChanged -= UpdateText;

    }

    private void Start()
    {
        goldText.text = PlayerInfo.Instance.currentGold.ToString();
    }

    void UpdateText(EventArgs eventArgs)
    {
        if(runningCoroutine == null && cr_Running == false)
        {
            runningCoroutine = UpdateTextEnum();
            StartCoroutine(runningCoroutine);
        }
        else
        {
            coroutineQueue.Enqueue(UpdateTextEnum());
        }
    }


    IEnumerator UpdateTextEnum()
    {
        cr_Running = true;

        float elapsedTime = 0f;
        int originalValue = int.Parse(goldText.text);
        int newValue = PlayerInfo.Instance.currentGold;

        while (elapsedTime < lerpDuration)
        {
            goldText.text = Mathf.Round(Mathf.Lerp(originalValue, newValue, elapsedTime / lerpDuration)).ToString();
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        goldText.text = newValue.ToString();

        cr_Running = false;
        Dequeue();

    }

    void Dequeue()
    {
        runningCoroutine = null;
        if (coroutineQueue.Count >0)
        {
            runningCoroutine = coroutineQueue.Dequeue();
            StartCoroutine(runningCoroutine);
        }
    }

}

