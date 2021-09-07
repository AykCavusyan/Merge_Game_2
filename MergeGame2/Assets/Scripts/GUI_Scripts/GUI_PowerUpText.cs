using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_PowerUpText : MonoBehaviour
{
    private RectTransform panelBGRect;
    private RectTransform panelFGRect;
    private RectTransform panelTextContainerRect;

    private RectTransform existingAmountTextMaskParentRect;
    private RectTransform existingAmountText_ParentContainer_Rect;

    //private RectTransform currentAmountRect;
    //private RectTransform previousAmountRect;
    //private RectTransform nextAmountRect;

    private RectTransform dividerRect;
    private RectTransform totalAmountRect;

    private Font font;
    private NumeratorMovement numeratorScript;
   


    private void Awake()
    {
        numeratorScript = GetComponent<NumeratorMovement>();

        panelBGRect = transform.parent.GetChild(0).GetComponent<RectTransform>();
        panelFGRect = transform.parent.GetChild(1).GetComponent<RectTransform>();
        panelTextContainerRect = GetComponent<RectTransform>();

        existingAmountTextMaskParentRect = transform.GetChild(0).GetComponent<RectTransform>();
        existingAmountText_ParentContainer_Rect = existingAmountTextMaskParentRect.GetChild(0).GetComponent<RectTransform>();

        //previousAmountRect = existingAmountTextMaskParentRect.GetChild(0).GetComponent<RectTransform>();
        //currentAmountRect = existingAmountTextMaskParentRect.GetChild(1).GetComponent<RectTransform>();
        //nextAmountRect = existingAmountTextMaskParentRect.GetChild(2).GetComponent<RectTransform>();


        dividerRect = transform.GetChild(1).GetComponent<RectTransform>();
        totalAmountRect = transform.GetChild(2).GetComponent<RectTransform>();

        font = Resources.Load<Font>("Fonts/" + "blambotcustom");


        //SceneConfig(); // bu sonra daha temel bir yere gidecek !!!
    }

    private void OnEnable()
    {
        GUI_PowerUpPanel.onPanelSetupComplete += SceneConfig;
    }

    private void OnDisable()
    {
        GUI_PowerUpPanel.onPanelSetupComplete -= SceneConfig;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.B)) SceneConfig();
    }

    public void SceneConfig()
    {
       
        float panelBGLeftPoint = (panelBGRect.sizeDelta.x/2) *-1;
        float panelFGLeftPoint = ((panelFGRect.sizeDelta.x/2) * -1) + panelFGRect.anchoredPosition.x;

        float middlePoint = panelFGLeftPoint + panelBGLeftPoint;
        float difference = panelFGLeftPoint - panelBGLeftPoint;

        panelTextContainerRect.anchoredPosition = new Vector2(middlePoint/2.015f, panelFGRect.anchoredPosition.y);
        panelTextContainerRect.sizeDelta = new Vector2(Mathf.Abs(difference), Mathf.Abs(difference));

        float containerMainComponentsSize = panelTextContainerRect.sizeDelta.x / 2.25f;
        Vector2 containerComponentsSizeVector = new Vector2(containerMainComponentsSize, containerMainComponentsSize * 1.15f);
        float containerMainComponentsPos = containerMainComponentsSize / 1.9f;

        existingAmountTextMaskParentRect.anchoredPosition = new Vector2(containerMainComponentsPos * -1, panelTextContainerRect.anchoredPosition.y);
        existingAmountTextMaskParentRect.sizeDelta = containerComponentsSizeVector;

        dividerRect.anchoredPosition = new Vector2(0, panelTextContainerRect.anchoredPosition.y);
        dividerRect.sizeDelta = containerComponentsSizeVector;

        totalAmountRect.anchoredPosition = new Vector2(containerMainComponentsPos, panelTextContainerRect.anchoredPosition.y);
        totalAmountRect.sizeDelta = containerComponentsSizeVector;

        CreateNumeratorSlots(containerComponentsSizeVector);

        //previousAmountRect.sizeDelta = containerComponentsSizeVector;
        //previousAmountRect.anchoredPosition = new Vector2(0,containerComponentsSizeVector.y);

        //currentAmountRect.sizeDelta = containerComponentsSizeVector;
        //currentAmountRect.anchoredPosition = new Vector2(0,0);
       
        //nextAmountRect.sizeDelta = containerComponentsSizeVector;
        //nextAmountRect.anchoredPosition =new Vector2(0,(containerComponentsSizeVector.y)*-1);

    }

    public void CreateNumeratorSlots(Vector2 containerComponentsSizeVectorIN)
    {
        for (int i = 0; i < PlayerInfo.Instance.powerUpSlotAmount + 1; i++)
        {
            GameObject numeratorSlot = new GameObject();
            Text numeratorSlotText = numeratorSlot.AddComponent<Text>();
            numeratorSlotText.text = i.ToString();
            numeratorSlotText.font = font;
            numeratorSlotText.fontSize = 28;
            numeratorSlotText.alignment = TextAnchor.MiddleCenter;
            numeratorSlot.name = $"ExistingAmountSlotNo {i}";
            RectTransform numeratorSlotRt = numeratorSlot.GetComponent<RectTransform>();
            numeratorSlot.transform.SetParent(existingAmountText_ParentContainer_Rect, false);

            numeratorSlotRt.sizeDelta = containerComponentsSizeVectorIN;
            numeratorSlotRt.anchoredPosition = new Vector2(0, 0 - (containerComponentsSizeVectorIN.y * i));

            //numeratorScript.PopulateDictionary(i, (numeratorSlotRt, numeratorSlotRt.anchoredPosition));

            //_dictExistingAmountSlots.Add(i, (numeratorSlotRt, numeratorSlotRt.anchoredPosition));
        }
    }

    public void AddNumeratorSlots()
    {
        // bunu sonra ekstra eklemek için kullanacaðýz
        // gene populate etmeyi unutmayalým 
    }

}
