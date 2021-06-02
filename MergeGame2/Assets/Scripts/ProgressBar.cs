using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] Image _renderer;
    [Range(0, 1)] [SerializeField] float _currentHealth;

    private RectTransform barSprite;

    private float currentXP;
    private float xpToNextLevel;
    private float lerpDuration;

    private float progressAmount; 
    
    
    

    


    
    float startValue = 0.1f;
    float endValue = 0.5f;
    float valueToLerp;

    // Start is called before the first frame update
    void Start()
    {

        _renderer = transform.Find("Bar").transform.Find("BarSprite").GetComponent<Image>();
        _renderer.material.SetFloat("_FillRate", startValue);

        //lerpDuration = 3;

        //_renderer = transform.Find("Bar").transform.Find("BarSprite").GetComponent<Renderer>();
        //progressBarCurrent = .1f;

        //barSprite.GetComponent<Image>().fillAmount = progressBarCurrent;



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
        float lerpDuration = 2;
        while (timeElapsed < lerpDuration)
        {
            _renderer.material.SetFloat("_FillRate", Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration));
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        valueToLerp = endValue;
    }


}
