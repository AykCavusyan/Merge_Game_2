using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlots : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    private RectTransform rectTransform;
    private float lerpDuration = .1f;
    private GameObject parentPanel;
    private Image image; // bu kalkabililr gibi duruyor 
    [SerializeField] private List<Image> childImage;
    private Color originalColor;

    public bool isActive;

    private bool isFree;
    private GameObject slot_Item_Holder;

    public int slotIDNumber;
    public bool isPurchasedOnSession;

    Vector3 lerpedSize;
    Vector3 originalScale;

    public event Action <GameObject> onSlotPurchaseAttempt;
    public event EventHandler<OnInventoryItemPlacedEventArgs> onInventoryPlacedItem;
    public class OnInventoryItemPlacedEventArgs
    {
        public GameItems gameItem;
        public int slotIDNumber;
    }

    private void Awake()
    {
        image = GetComponent<Image>();
        childImage = transform.GetComponentsInChildren<Image>(true).ToList();
        rectTransform = GetComponent<RectTransform>();
        originalColor = transform.GetChild(0).GetComponent<Image>().color;


        // bu k�sma daha sonra bakar�z, sadece ge�venlik
        //if (image.enabled == true)
        //{
        //    image.enabled = false;

        //}

        parentPanel = GameObject.Find("Panel_BackToGame");
        originalScale = new Vector3(0f, 0f, 0f);
        lerpedSize = new Vector3(1f, 1f, 1f);
        
        isFree = true;
        slot_Item_Holder = transform.GetChild(0).GetChild(0).gameObject;

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

    public void Drop(GameItems gameItem)
    {
        Debug.Log("Inventory slots drop called");
        PlaceItem(gameItem);

        onInventoryPlacedItem?.Invoke(this, new OnInventoryItemPlacedEventArgs { slotIDNumber = slotIDNumber, gameItem = gameItem });

        isFree = false;
        //UpdateSlotStatus(this); BUNA DAHA SONRA GELECE��Z
    }

    void PlaceItem(GameItems gameItem)
    {
        RectTransform rt = gameItem.GetComponent<RectTransform>();
        childImage.Add(gameItem.GetComponent<Image>());
        gameItem.GetComponent<Image>().enabled = false;
        rt.SetParent(slot_Item_Holder.transform);
        rt.sizeDelta = slot_Item_Holder.GetComponent<RectTransform>().sizeDelta;
        rt.localScale = new Vector3(1, 1, 1);
        rt.SetAsLastSibling();
        rt.anchoredPosition = slot_Item_Holder.GetComponent<RectTransform>().anchoredPosition;


    }

    void PlaceSlots(object sender, Panel_Invetory.OnPanelSizedEventArgs e)
    {
        StopAllCoroutines();

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
        StopAllCoroutines();
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

        for (int i = 0; i < childImage.Count; i++)
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

        for (int i = 0; i < childImage.Count; i++)
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