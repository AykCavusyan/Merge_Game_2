using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicBar_Level : MonoBehaviour
{
    private GameObject player;
    private GameObject parentPanel;

    private Image progressBar;
    private Text progressText;

    private float originalAmount;

    private float lerpAmount;
    private float lerpDuration = .5f;


     void Awake()
    {
        progressBar = transform.GetChild(2).GetComponent<Image>();
        progressText = transform.GetChild(3).GetComponent<Text>();
        player = GameObject.FindGameObjectWithTag("Player");
        parentPanel = transform.parent.gameObject;
    }
    void OnEnable()
    {
        Init();
        PlayerInfo.Instance.OnLevelNumberChanged += GetFillAmount;
        parentPanel.GetComponent<Panel_Invetory>().OnPanelSized += UpdateBarFill;
        parentPanel.GetComponent<Panel_Invetory>().OnPanelDisappear += ResetBarFill;
    }
    void OnDisable()
    {
        PlayerInfo.Instance.OnLevelNumberChanged -= GetFillAmount;
        parentPanel.GetComponent<Panel_Invetory>().OnPanelSized -= UpdateBarFill;
        parentPanel.GetComponent<Panel_Invetory>().OnPanelDisappear -= ResetBarFill;
    }


    void Init()
    {
        if (PlayerInfo.Instance == null)
        {
            Instantiate(player);
        }
    }


    void UpdateBarFill(object sender, EventArgs e)
    {
        StopAllCoroutines();

        originalAmount = 0f;
        StartCoroutine(UpdateBarFillEnum(originalAmount, lerpAmount));
    }

    void ResetBarFill(object sender, EventArgs e)
    {
        StopAllCoroutines();

        originalAmount = progressBar.fillAmount;
        lerpAmount = 0f;
        StartCoroutine(UpdateBarFillEnum(originalAmount, lerpAmount));
    }

    IEnumerator UpdateBarFillEnum(float originalAmount, float lerpAmount)
    {
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            progressBar.fillAmount = Mathf.Lerp(originalAmount, lerpAmount, elapsedTime / lerpDuration);
            progressText.text = "%" + Mathf.Round(progressBar.fillAmount*100).ToString();
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        progressBar.fillAmount = lerpAmount;

    }

    private void GetFillAmount(object sender, PlayerInfo.OnLevelChangedEventArgs e)
    {
        int currentXP = e.currentXP;
        int xpToNextLevel = e.xpToNextLevel;
        lerpAmount = (float)currentXP / xpToNextLevel;
    }

}
