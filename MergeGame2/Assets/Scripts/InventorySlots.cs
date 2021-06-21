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
    private Image image;
    [SerializeField] private Image[] childImage;
    private Color originalColor;

    public bool isActive;

    Vector3 lerpedSize;
    Vector3 originalScale;

    public event Action onSlotPurchaseAttempt; 

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

        parentPanel = transform.parent.parent.gameObject;
        originalScale = new Vector3(0f, 0f, 0f);
        lerpedSize = new Vector3(1f, 1f, 1f);
    }
    private void OnEnable()
    {
        parentPanel.GetComponent<Panel_Invetory>().OnPanelSized += PlaceSlots;
    }


    private void OnDisable()
    {
        parentPanel.GetComponent<Panel_Invetory>().OnPanelSized -= PlaceSlots;

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
        if (isActive== false)
        {
            Image childImageToFade = transform.GetChild(0).GetComponent<Image>();
            childImageToFade.color = new Color(.5f, .5f, .5f, .5f);

            transform.Find("Lock").gameObject.SetActive(true);
        }


        StartCoroutine(SlotsUpSize());
    }

    void DeplaceSlots(object  sender, Panel_Invetory.OnPanelSizedEventArgs e)
    {
        //StartCoroutine(SlotsDownSize());
    }

    IEnumerator SlotsUpSize()
    {
        float elapsedTime = 0f;

        yield return new WaitForSeconds(.15f);

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
            onSlotPurchaseAttempt?.Invoke();
        }

    }
}
