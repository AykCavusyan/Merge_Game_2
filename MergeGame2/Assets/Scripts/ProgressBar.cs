using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    Material _barSpriteShader;
    ScoreManager scoreManager;
    Text barFillPercent;

    private int currentXP;
    private float currentXPFloat;


    private int xpToNextLevel;
    private float xpToNextLevelFloat;

    private void Awake()
    {
        currentXP = 0;
        xpToNextLevel = 10;

        _barSpriteShader = transform.Find("Bar").transform.Find("BarSprite").GetComponent<Image>().material;
        _barSpriteShader.SetFloat("_FillRate", currentXP / xpToNextLevel);
        barFillPercent = transform.Find("Bar").transform.Find("BarFillPercent").gameObject.GetComponent<Text>();

        

        scoreManager = GameObject.Find("ScoreText").GetComponent<ScoreManager>();
    }

    private void OnEnable()
    {
        scoreManager.OnScoreUpdate += UpdateBarFill;
    }

    private void OnDisable()
    {
        scoreManager.OnScoreUpdate -= UpdateBarFill;
    }

    void Start()
    {
        currentXPFloat = (float)currentXP;
        xpToNextLevelFloat = (float)xpToNextLevel;

       
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void UpdateBarFill(object sender, ScoreManager.OnScoreUpdateEventHandler e)
    {
        StartCoroutine (SetBarFll(e.score));
    }



    IEnumerator SetBarFll(int xpGained)
    {
        float timeElapsed = 0;
        float lerpDuration = 0.5f;
        float xpGainedFloat = (float)xpGained;

        float valueToLerpFrom = currentXPFloat / xpToNextLevelFloat;
        Debug.Log(valueToLerpFrom);
        float valueToLerpTo = (currentXPFloat + xpGainedFloat) / xpToNextLevelFloat;
        Debug.Log(valueToLerpTo);

        while (timeElapsed < lerpDuration)
        {
            float lerpedValue = Mathf.Lerp(valueToLerpFrom, valueToLerpTo, timeElapsed / lerpDuration);
            _barSpriteShader.SetFloat("_FillRate", lerpedValue);

            float roundedPercentage = (float)Mathf.Round(lerpedValue * 100);
            barFillPercent.text = "% " + roundedPercentage;
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        currentXPFloat += xpGainedFloat;
       

        //tam lerplemediði için en son fiksleme iþi var
    }


}
