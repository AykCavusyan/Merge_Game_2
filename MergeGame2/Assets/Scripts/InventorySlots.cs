using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlots : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    private RectTransform rectTransform;
    private float lerpDuration = .1f;
    private GameObject parentPanel;
    private Image image; // bu kalkabililr gibi duruyor 
    [SerializeField] private Image[] childImage;
    private Color originalColor;

    public bool isActive;

    public int slotIDNumber;
    public bool isPurchasedOnSession;

    Vector3 lerpedSize;
    Vector3 originalScale;

    public event Action <GameObject> onSlotPurchaseAttempt; 

    private void Awake()
    {
        image = GetComponent<Image>();
        childImage = transform.GetComponentsInChildren<Image>(true);
        rectTransform = GetComponent<RectTransform>();
        originalColor = transform.GetChild(0).GetComponent<Image>().color;


        // bu kýsma daha sonra bakarýz, sadece geüvenlik
        //if (image.enabled == true)
        //{
        //    image.enabled = false;

        //}

        parentPanel = GameObject.Find("Panel_BackToGame");
        originalScale = new Vector3(0f, 0f, 0f);
        lerpedSize = new Vector3(1f, 1f, 1f);
    }
    private void OnEnable()
    {
        parentPanel.GetComponent<Panel_Invetory>().OnPanelSized += PlaceSlots;
        parentPanel.GetComponent<Panel_Invetory>().OnPanelDisappear += DeplaceSlots;
    }


    private void OnDisable()
    {
        parentPanel.GetComponent<Panel_Invetory>().OnPanelSized -= PlaceSlots;
        parentPanel.GetComponent<Panel_Invetory>().OnPanelDisappear -= DeplaceSlots;

    }

    private void Start()
    {
        if (parentPanel.GetComponent<Image>().enabled == true)
        {
            PlaceSlots(null, null);
        }
    }

    void PlaceSlots(object sender, Panel_Invetory.OnPanelSizedEventArgs e)
    {
        int slotAppearOrder = slotIDNumber;

        if (isActive== false)
        {
            Image childImageToFade = transform.GetChild(0).GetComponent<Image>();
            childImageToFade.color = new Color(.5f, .5f, .5f, .5f);

            transform.Find("Lock").gameObject.SetActive(true);
            
            if (isPurchasedOnSession)
            {
                slotAppearOrder = slotAppearOrder / slotIDNumber;
            }
        }


        StartCoroutine(SlotsUpSize(slotAppearOrder));
    }

    void DeplaceSlots(object  sender, Panel_Invetory.OnPanelDisappearEventArgs e)
    {
        StartCoroutine(SlotsDownSize());
        if (isPurchasedOnSession)
        {
            isPurchasedOnSession = false;
        }
    }

    IEnumerator SlotsUpSize(int slotAppearOrder)
    {
        float elapsedTime = 0f;

        yield return new WaitForSeconds(slotAppearOrder * .05f);

        //image.enabled = true;

        for (int i = 0; i < childImage.Length; i++)
        {
            childImage[i].enabled = true;
        }
        

        while (elapsedTime < lerpDuration)
        {
            

            rectTransform.localScale = Vector3.Lerp(originalScale, lerpedSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.localScale = lerpedSize;
    }

    IEnumerator SlotsDownSize()
    {
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(lerpedSize, originalScale, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.localScale = originalScale;

        for (int i = 0; i < childImage.Length; i++)
        {
            childImage[i].enabled = false;
        }
    }

   void ActivateSlot()
    {
        transform.GetChild(0).GetComponent<Image>().color = originalColor;
        isActive = true;
        transform.Find("Lock").gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //if( moneyis enough){
        //    purchase slot evet fired
        //}

        if ( isActive == false)
        {
            ActivateSlot();
            onSlotPurchaseAttempt?.Invoke(this.gameObject);
        }

    }
}
