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
    private XButton_Panel[] xButton;

    private bool CR_Running = false;

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

        // can use lambda here just for testing and securing 
        xButton = transform.GetComponentsInChildren<XButton_Panel>();
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

        for (int i = 0; i < xButton.Length; i++)
        {
            xButton[i].onXButtonPressed += DisableVisibility;
        }

    }

    void OnDisable()
    {
        for (int i = 0; i < lowerButtons.Length; i++)
        {
            lowerButtons[i].GetComponent<ButtonHandler>().OnButtonPressed -= EnableVisibility;
        }

        for (int i = 0; i < xButton.Length; i++)
        {
            xButton[i].onXButtonPressed -= DisableVisibility;
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
        Debug.Log("on pointer up called");
        //imageToFade.raycastTarget = false; // to stop clicks which comes before image is faded by decrementalpha corooutine
        
        
        DisableVisibility();
        
    }

    void EnableVisibility(object sender, ButtonHandler.OnButtonPressedEventArgs e)
    {
        Debug.Log("event heard");

        if ( imageToFade.enabled == false)
        {

            imageToFade.enabled = true;
        
            StopAllCoroutines();
            CR_Running = false;
        
            StartCoroutine(IncrementPanelAlpha());
            activePanel = e.buttonIndex;
            OnenableVisibility?.Invoke(this, new OnEnableVisibilityEventArgs { panelIndex = this.activePanel });
        }

    }

    void DisableVisibility()
    {
        if (CR_Running == false)
        {
            StopAllCoroutines();
            StartCoroutine(DecrementPanelAlpha());
            OnDisableVisibility?.Invoke(this, new OnDisableVisibilityEventArgs { activePanel = this.activePanel });
        }
        
    }

    IEnumerator DecrementPanelAlpha()
    {
        CR_Running = true;

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

        CR_Running = false;

        //imageToFade.raycastTarget = true; // reenabe raycast which is disabled in onpointer up callback
    }

}
