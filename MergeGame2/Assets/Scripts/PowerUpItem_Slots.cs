using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpItem_Slots : MonoBehaviour
{
    public int slotIDNumber;
    public bool isFree { get; private set; } = true;
    public GameObject containedItem { get;  set; }
    private Panel_PowerUpItems panelPowerUpItems;
    private float lerpDuration = .15f;

    private void Awake()
    {
        panelPowerUpItems = FindObjectOfType<Panel_PowerUpItems>();
    }

    public void Drop(GameItems gameItem, bool shakeWholePanel = false)
    {
        Image gameItemImage = gameItem.GetComponent<Image>();

        gameItemImage.maskable = false;
        gameItem.isInsidePowerUpPanel = true;
        Vector3 itemDropPos = transform.InverseTransformPoint(gameItem.transform.position);

        PlaceItem(gameItem, itemDropPos,gameItemImage, shakeWholePanel);
        UpdateParentSlot(gameItem);
        isFree = false;
    }

    void PlaceItem(GameItems gameItemIN, Vector2 itemDroppedPosIN, Image gameItemImage, bool shakeWholePanel)
    {
        RectTransform rtslot = GetComponent<RectTransform>();
        RectTransform rt = gameItemIN.GetComponent<RectTransform>();
        rt.SetParent(this.transform,false);
        rt.sizeDelta = rtslot.sizeDelta;
        rt.localScale = new Vector3(1, 1, 1);
        containedItem = gameItemIN.gameObject;
        rt.SetAsLastSibling();
        //rt.anchoredPosition = new Vector3 (0,0,0);
        //gameItemIN.isMoving = false;
        gameItemIN.initialGameSlot = this.gameObject;
        StartCoroutine(LerpItemPositionEnum(rt, itemDroppedPosIN,gameItemImage, shakeWholePanel));
    }

    IEnumerator LerpItemPositionEnum(RectTransform rtIN, Vector2 itemDroppedPosIN, Image gameItemImage,bool ShakeWholePanel)
    {
        float elapsedTime = 0f;
        Vector3 lerpPosition = new Vector2(0, 0);
        while (elapsedTime < lerpDuration)
        {            
            rtIN.anchoredPosition = Vector2.Lerp(itemDroppedPosIN, lerpPosition, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rtIN.anchoredPosition = lerpPosition;
        gameItemImage.maskable = true;

        if (ShakeWholePanel) panelPowerUpItems.SelectContainedItems();
    }

    void UpdateParentSlot(GameItems gameItemIN)
    {
        gameItemIN.initialGameSlot = this.gameObject;

    }

    public void DischargeItem()
    {
        isFree = true;
        containedItem = null;
    }
}
