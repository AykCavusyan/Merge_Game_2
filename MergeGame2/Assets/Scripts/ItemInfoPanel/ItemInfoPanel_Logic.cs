using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInfoPanel_Logic : MonoBehaviour 
{
    private float lerpDuration = .3f;

    private ItemSelector itemSelector;
    private Text itemName;
    private Text itemDescription;
    private GameObject actionButton;
    //private Text actionText;

    private void Awake()
    {
        itemName = transform.GetChild(1).GetComponent<Text>();
        itemDescription = transform.GetChild(2).GetComponent<Text>();
        actionButton = transform.GetChild(4).gameObject;
        //actionText = actionButton.transform.GetChild(2).GetComponent<Text>();
        itemSelector = GameObject.Find("Canvas").GetComponent<ItemSelector>();
    }


    private void OnEnable()
    {
        itemSelector.OnGameItemSelected += SetNameAndDescription;
        itemSelector.OnGameItemDeSelected += SetDefaultTextFields;
    }

    private void OnDisable()
    {
        itemSelector.OnGameItemSelected -= SetNameAndDescription;
        itemSelector.OnGameItemDeSelected -= SetDefaultTextFields;

    }

    private void Start()
    {
        SetDefaultTextFields(null,null);
    }

    void SetNameAndDescription(object sender, ItemSelector.OnGameItemSelectedEventArgs e)
    {
        ChangeTextAlpha(itemName);
        itemName.text = ItemAssets.Instance.GetItemNameAndDescription(e.itemType)[0];
        ChangeTextAlpha(itemDescription);
        itemDescription.text = ItemAssets.Instance.GetItemNameAndDescription(e.itemType)[1];

        if (e.goldValue > 0)
        {
            actionButton.SetActive(true);
            actionButton.GetComponent<Button_Action_ItemInfo>().SetItemActionValue();
        }
        else actionButton.SetActive(false);
    }

    void SetDefaultTextFields(object sender, ItemSelector.OnGameItemSelectedEventArgs e)
    {
        itemName.text = "";
        itemName.fontStyle = FontStyle.Bold;
        itemDescription.text = "tap an item to see the description of it";
        itemDescription.fontStyle = FontStyle.Italic;

        actionButton.SetActive(false);
    }

    void ChangeTextAlpha(Text textIN)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeTextAlphaEnum(textIN));
    }

    IEnumerator ChangeTextAlphaEnum(Text textIN)
    {
        Color originalValue = textIN.color;
        Color noAlpha = new Color(textIN.color.r, textIN.color.g, textIN.color.b, 0);
        Color fullAlpha = new Color(textIN.color.r, textIN.color.g, textIN.color.b, 1);

        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            textIN.color = Color.Lerp(originalValue, noAlpha, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        textIN.color = noAlpha;

        elapsedTime = 0;
        while (elapsedTime < lerpDuration)
        {
            textIN.color = Color.Lerp(noAlpha, fullAlpha, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        textIN.color = fullAlpha;
    }

}
