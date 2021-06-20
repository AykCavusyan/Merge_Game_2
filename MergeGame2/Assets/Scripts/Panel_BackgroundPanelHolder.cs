using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Panel_BackgroundPanelHolder : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    private float lerpDuration = .2f;
    
    private Color initialColorValue;
    private Color finalColorValue;
    private Image imageToFade;

    private GameObject[] lowerButtons;
    //private GameObject[] buttonPanels;

    private int activePanel;

    public event EventHandler<OnEnableVisibilityEventArgs> OnenableVisibility;
    public event EventHandler<OnDisableVisibilityEventArgs> OnDisableVisibility;
    public class OnEnableVisibilityEventArgs : EventArgs
    {
        public int panelIndex;
    }
    public class OnDisableVisibilityEventArgs : EventArgs
    {
        public int activePanel;
    }

    void Awake()
    {
        imageToFade = GetComponent<Image>();
        
        if(imageToFade.enabled == true)
        {
            imageToFade.enabled = false;
        }       

        initialColorValue = imageToFade.color;
        finalColorValue = new Color(imageToFade.color.r, imageToFade.color.g, imageToFade.color.b, .6f);

        lowerButtons = GameObject.FindGameObjectsWithTag("Button UI");
        //buttonPanels = GameObject.FindGameObjectsWithTag("Panel_UI");
        
    }
    void Start()
    {

    }

    void OnEnable()
    {
        for (int i = 0; i < lowerButtons.Length ; i++)
        {
            lowerButtons[i].GetComponent<ButtonHandler>().OnButtonPressed += EnableVisibility;
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < lowerButtons.Length; i++)
        {
            lowerButtons[i].GetComponent<ButtonHandler>().OnButtonPressed -= EnableVisibility;
        }
    }

    IEnumerator IncrementPanelAlpha()
    {
        float elapsedTime = 0f;
        

        while (elapsedTime < lerpDuration)
        {
            imageToFade.color = Color.Lerp(initialColorValue, finalColorValue, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        imageToFade.color = finalColorValue;
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        DisableVisibility();
    }

    void EnableVisibility(object sender, ButtonHandler.OnButtonPressedEventArgs e)
    {
        Debug.Log("event heard");

        imageToFade.enabled = true;
        StartCoroutine(IncrementPanelAlpha());

        //for (int i = 0; i < buttonPanels.Length; i++)
        //{
        //    if (buttonPanels[i].GetComponent<Panel_Invetory>().panelIndex == e.buttonIndex)
        //    {
        //        continue;
        //    }
        activePanel = e.buttonIndex;
        OnenableVisibility?.Invoke(this, new OnEnableVisibilityEventArgs { panelIndex = this.activePanel });
        //}

    }

    void DisableVisibility()
    {
        StartCoroutine(DecrementPanelAlpha());

        OnDisableVisibility?.Invoke(this, new OnDisableVisibilityEventArgs { activePanel = this.activePanel });
    }

    IEnumerator DecrementPanelAlpha()
    {
        float elapsedTime = 0f;
        Debug.Log("decrementing");

        while (elapsedTime < lerpDuration)
        {
            imageToFade.color = Color.Lerp(finalColorValue, initialColorValue, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        imageToFade.color = initialColorValue;
        imageToFade.enabled = false;
    }

}
