using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private RectTransform barSprite;
    private float progressAmountPercentage;
    private float progressBarCurrent;
    private float lerpSpeed; 

    private float progressBarMaximum;
    
    private float progressAmount;

    private float chipSpeed;
    private float lerpTimer;
    private float percentComplete;

    


    float lerpDuration = 3;
    float startValue = 0;
    float endValue = 5;
    float valueToLerp;

    // Start is called before the first frame update
    void Start()
    {
        barSprite = (RectTransform)transform.Find("Bar").transform.Find("BarSprite");
        progressBarCurrent = .1f;

        barSprite.GetComponent<Image>().fillAmount = progressBarCurrent;

        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(Lerp());




        }
    }

    void SetBarFill()
    {
        float timeELapsed = 0;

        while (timeELapsed < lerpDuration)
        {
            barSprite.GetComponent<Image>().fillAmount += Mathf.Lerp(.1f, .8f, timeELapsed/lerpDuration);
            timeELapsed += Time.deltaTime;
        }
        
    }

    IEnumerator Lerp()
    {
        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)
        {
            barSprite.GetComponent<Image>().fillAmount = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        valueToLerp = endValue;
    }


}
