using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlots : MonoBehaviour
{
    private RectTransform rectTransform;
    private float lerpDuration = .06f;
    private GameObject parentPanel;
    private List<Image> childImage;
    public GameItems containedItem;
    public bool cr_Runnning { get; private set; } = false;
    private ParticleSystem particles;

    private Color claimableColor;

    private GameObject slot_Item_Holder;
    public int slotIDNumber;

    Vector3 zeroSize;
    Vector3 upSize;
    Vector3 normalSize;


    private void Awake()
    {
        childImage = GetComponentsInChildren<Image>(true).ToList();
        rectTransform = GetComponent<RectTransform>();

        parentPanel = transform.parent.parent.gameObject;
        zeroSize = new Vector3(0f, 0f, 0f);
        upSize = new Vector3(1.35f, 1.35f, 1.35f);
        normalSize = new Vector3(1f, 1f, 1f);

        slot_Item_Holder = transform.GetChild(0).GetChild(0).gameObject;
        
    }
    private void OnEnable()
    {
        if(parentPanel.GetComponent<Panel_Invetory>().panelIndex == 4)
        {
            parentPanel.GetComponent<Panel_Invetory>().OnPanelSized += PlaceSlots;
            parentPanel.GetComponent<Panel_Invetory>().OnPanelDisappear += DeplaceSlots;
        }

    }

    private void OnDisable()
    {
        if (parentPanel.GetComponent<Panel_Invetory>().panelIndex == 4)
        {
            parentPanel.GetComponent<Panel_Invetory>().OnPanelSized -= PlaceSlots;
            parentPanel.GetComponent<Panel_Invetory>().OnPanelDisappear -= DeplaceSlots;
        }
    }

    private void Start()
    {
        if (parentPanel.GetComponent<Image>().enabled == true)
        {
            PlaceSlots(null,null);
        }
    }

    public void Drop(GameItems gameItem)
    {
        containedItem = gameItem;
        PlaceItem(gameItem);
        UpdateItemParentSlot(gameItem);
    }

    void PlaceItem(GameItems gameItem)
    {
        gameItem.canDrag = false;
        RectTransform rt = gameItem.GetComponent<RectTransform>();
        childImage.Add(gameItem.GetComponent<Image>());
        gameItem.GetComponent<Image>().enabled = false;
        rt.SetParent(slot_Item_Holder.transform);
        rt.sizeDelta = slot_Item_Holder.GetComponent<RectTransform>().sizeDelta;
        

        rt.localScale = new Vector3(1, 1, 1);
        rt.SetAsLastSibling();
        rt.anchoredPosition = slot_Item_Holder.GetComponent<RectTransform>().anchoredPosition;

    }
    void UpdateItemParentSlot(GameItems gameItemIN)
    {
        gameItemIN.initialGameSlot = this.gameObject;
    }

    public void SetupParticleSystem(ParticleSystem particlesIN)
    {
        particles = particlesIN;
        //particles.transform.position = this.transform.position;
        particles.Stop();
    }

    void PlaceSlots(object sender, EventArgs e)
    {
        StopAllCoroutines();
        int slotAppearOrder = slotIDNumber;

        StartCoroutine(SlotUpsize(slotAppearOrder));
    }

    public void DeplaceSlots(object sender, Panel_Invetory.OnPanelStateChangeEventArgs e)
    {
        StopAllCoroutines();
        int slotAppearOrder = slotIDNumber;

        StartCoroutine(SlotsDownSize(slotAppearOrder,e.isAnimatable));
    }

    IEnumerator SlotUpsize(int slotAppearOrder)
    {
        float elapsedTime = 0;

        yield return new WaitForSeconds(slotAppearOrder * .03f);

        for (int i = 0; i < childImage.Count; i++)
        {
            childImage[i].enabled = true;
        }

        while(elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(zeroSize, normalSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.localScale = normalSize;
    }    

    IEnumerator SlotsDownSize(int slotAppearOrder, bool isAnimatable) 
    {
        cr_Runnning = true;

        yield return new WaitForSeconds(slotAppearOrder * .05f);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(normalSize, upSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.localScale = upSize;

        elapsedTime = 0f;
        while (elapsedTime < lerpDuration)
        {
            rectTransform.localScale = Vector3.Lerp(upSize, zeroSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rectTransform.localScale = zeroSize;

         
        if (isAnimatable)
        {            
            particles.Play();
            float particleElapsedTime = 0f;
            while (particles.isPlaying)
            {                
                particleElapsedTime += Time.deltaTime; Debug.Log(particleElapsedTime);

                if (particleElapsedTime > (particles.main.duration) / 5f && cr_Runnning) cr_Runnning = false; 
                else yield return null;
            }
            particles.Stop();
        }
        else cr_Runnning = false;

        for (int i = 0; i < childImage.Count; i++)
        {
            childImage[i].enabled = false;
        }
        
    }
}
