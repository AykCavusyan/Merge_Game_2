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
    private GameSlots[] gameSlots;
    private Color originalColor;

    public bool isActive;

    public bool isFree;
    public GameObject containedItem { get; private set; }

    private GameObject slot_Item_Holder;

    public int slotIDNumber;
    public bool isPurchasedOnSession;

    Vector3 lerpedSize;
    Vector3 originalSize;

    public event Action <GameObject> onSlotPurchaseAttempt;
    public event EventHandler<onInventoryItemModificationEventArgs> onInventoryPlacedItem;
    public event EventHandler<onInventoryItemModificationEventArgs> onInventoryRemovedItem;
    public class onInventoryItemModificationEventArgs
    {
        public GameItems gameItem;
        public InventorySlots slot;
    }

    private void Awake()
    {
        gameSlots = FindObjectsOfType<GameSlots>();
        image = GetComponent<Image>();
        childImage = transform.GetComponentsInChildren<Image>(true).ToList();
        rectTransform = GetComponent<RectTransform>();
        originalColor = transform.GetChild(0).GetComponent<Image>().color;


        // bu kýsma daha sonra bakarýz, sadece geüvenlik
        //if (image.enabled == true)
        //{
        //    image.enabled = false;

        //}

        //parentPanel = GameObject.Find("Panel_BackToGame");
        parentPanel = transform.parent.parent.parent.gameObject;
        originalSize = new Vector3(0f, 0f, 0f);
        lerpedSize = new Vector3(1f, 1f, 1f);
        
        isFree = true;
        slot_Item_Holder = transform.GetChild(0).GetChild(0).gameObject;

    }
    private void OnEnable()
    {
        if (parentPanel.GetComponent<Panel_Invetory>().panelIndex == 1)
        {
            parentPanel.GetComponent<Panel_Invetory>().OnPanelSized += PlaceSlots;
            parentPanel.GetComponent<Panel_Invetory>().OnPanelDisappear += DeplaceSlots;
        }

        //for (int i = 0; i < gameSlots.Length; i++)
        //{
        //    if(gameSlots[i] != null)
        //    gameSlots[i].onSlotFilled += UpdateChildImagesList;
        //}
    }


    private void OnDisable()
    {
        if (parentPanel.GetComponent<Panel_Invetory>().panelIndex == 1)
        {
            parentPanel.GetComponent<Panel_Invetory>().OnPanelSized -= PlaceSlots;
            parentPanel.GetComponent<Panel_Invetory>().OnPanelDisappear -= DeplaceSlots;
        }

        //for (int i = 0; i < gameSlots.Length; i++)
        //{
        //    if (gameSlots[i] != null)
        //        gameSlots[i].onSlotFilled -= UpdateChildImagesList;
        //}
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
        gameItem.isInventoryItem = true;
        PlaceItem(gameItem);
        UpdateItemParentSlot(gameItem);

        onInventoryPlacedItem?.Invoke(this, new onInventoryItemModificationEventArgs { slot = this, gameItem = gameItem });

        isFree = false;

        //UpdateSlotStatus(this); BUNA DAHA SONRA GELECEÐÝZ
    }

    void PlaceItem(GameItems gameItem)
    {
        RectTransform rt = gameItem.GetComponent<RectTransform>();
        childImage.Add(gameItem.GetComponent<Image>());
        gameItem.GetComponent<Image>().enabled = false;
        rt.SetParent(slot_Item_Holder.transform);
        rt.sizeDelta = slot_Item_Holder.GetComponent<RectTransform>().sizeDelta;
        rt.localScale = new Vector3(1, 1, 1);
        containedItem = gameItem.gameObject;
        rt.SetAsLastSibling();
        rt.anchoredPosition = slot_Item_Holder.GetComponent<RectTransform>().anchoredPosition;
        gameItem.isMoving = false;
        gameItem.initialGameSlot = this.gameObject;

    }

    void UpdateItemParentSlot(GameItems gameItem)
    {
        gameItem.initialGameSlot = this.gameObject;
    }

    public void DischargeItem(GameItems gameItemIN)
    {
        UpdateChildImagesList(gameItemIN);
        isFree = true;
        containedItem = null;

        onInventoryRemovedItem?.Invoke(this, new onInventoryItemModificationEventArgs { slot = this });
    }

    void UpdateChildImagesList(GameItems gameItem)
    {
        childImage.Remove(gameItem.GetComponent<Image>());
    }

    void PlaceSlots(object sender,EventArgs e)
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


    void DeplaceSlots(object  sender, EventArgs e)
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

        image.enabled = true;

        for (int i = 0; i < childImage.Count; i++)
        {
            childImage[i].enabled = true;
        }
        

        while (elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(originalSize, lerpedSize, elapsedTime / lerpDuration);
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
            rectTransform.localScale = Vector3.Lerp(lerpedSize, originalSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.localScale = originalSize;

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
