using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlots : MonoBehaviour
{
    private RectTransform rectTransform;
    private float lerpDuration =.1f;
    private GameObject parentPanel;
    private List<Image> childImage;

    private Color claimableColor;

    private GameObject slot_Item_Holder;
    public int slotIDNumber;

    Vector3 originalSize;
    Vector3 lerpedSize;


    private void Awake()
    {
        childImage = GetComponentsInChildren<Image>(true).ToList();
        rectTransform = GetComponent<RectTransform>();

        parentPanel = transform.parent.parent.gameObject;
        originalSize = new Vector3(0, 0, 0);
        lerpedSize = new Vector3(1, 1, 1);

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
        PlaceItem(gameItem);
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

    void PlaceSlots(object sender, EventArgs e)
    {
        StopAllCoroutines();
        int slotAppearOrder = slotIDNumber;

        StartCoroutine(SlotUpsize(slotAppearOrder));
    }

    void DeplaceSlots(object sender, EventArgs e)
    {
        StopAllCoroutines();
        StartCoroutine(SlotsDownSize());
    }

    IEnumerator SlotUpsize(int slotAppearOrder)
    {
        float elapsedTime = 0;

        yield return new WaitForSeconds(slotAppearOrder * .05f);

        for (int i = 0; i < childImage.Count; i++)
        {
            childImage[i].enabled = true;
        }

        while(elapsedTime < lerpDuration)
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
}
