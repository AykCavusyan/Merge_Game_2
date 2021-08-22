using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpItem_Slots : MonoBehaviour
{
    public int slotIDNumber;
    public bool isFree { get; private set; } = true;
    public GameObject containedItem { get; private set; }

    private float lerpDuration = .15f;

    public void Drop(GameItems gameItem)
    {
        PlaceItem(gameItem);
        UpdateParentSlot(gameItem);
        isFree = false;
    }

    void PlaceItem(GameItems gameItemIN)
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
        StartCoroutine(LerpItemPositionEnum(rt, rt.anchoredPosition));
    }

    IEnumerator LerpItemPositionEnum(RectTransform rtIN, Vector3 anchoredPositionIN)
    {
        float elapsedTime = 0f;
        Vector3 lerpPosition = new Vector3(0, 0, 0);
        while (elapsedTime < lerpDuration)
        {
            rtIN.anchoredPosition = Vector3.Lerp(anchoredPositionIN, lerpPosition, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rtIN.anchoredPosition = lerpPosition;
    }

    void UpdateParentSlot(GameItems gameItemIN)
    {


    }

    public void DischargeItem()
    {
        isFree = true;
        containedItem = null;
    }
}
