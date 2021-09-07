using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_PowerUpPanel : MonoBehaviour
{   
    float canvasWidth;

    private RectTransform panelPowerUp_BG;
    private RectTransform panelPowerUp_FG;
    private RectTransform scrollMask;
    private RectTransform innerPanelcontainer;
    private RectTransform addSlotButton;

    public static event Action onPanelSetupComplete;

   
    // Awake yerine Start çünkü Canvas Scaler Awake  de henüz canvas ý scale etmiþ olmuyor !

    private void Start()
    {
        canvasWidth = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.x;

        panelPowerUp_BG = transform.GetChild(0).GetComponent<RectTransform>();
        panelPowerUp_FG = transform.GetChild(1).GetComponent<RectTransform>();
        scrollMask = panelPowerUp_FG.GetChild(0).GetComponent<RectTransform>();
        innerPanelcontainer = scrollMask.GetChild(0).GetComponent<RectTransform>();
        addSlotButton = transform.GetChild(3).GetComponent<RectTransform>();

        panelPowerUp_BG.sizeDelta = new Vector2(canvasWidth / 1.2f, canvasWidth / (1.2f * 6.93f));
        panelPowerUp_BG.anchoredPosition = new Vector2(0, 0);

        panelPowerUp_FG.sizeDelta = new Vector2(panelPowerUp_BG.sizeDelta.x / 1.2f, panelPowerUp_BG.sizeDelta.y / 1.24f);
        panelPowerUp_FG.anchoredPosition = new Vector2(panelPowerUp_FG.sizeDelta.x / 14f, 0);

        scrollMask.sizeDelta = new Vector2(panelPowerUp_FG.sizeDelta.x / 1.07f, panelPowerUp_FG.sizeDelta.y);
        scrollMask.anchoredPosition = new Vector2(0, 0);

        innerPanelcontainer.sizeDelta = new Vector2(scrollMask.sizeDelta.x, scrollMask.sizeDelta.y * 1.25f);
        innerPanelcontainer.anchoredPosition = new Vector2(0, 0);

        addSlotButton.sizeDelta = new Vector2(innerPanelcontainer.sizeDelta.y / 1.53f, innerPanelcontainer.sizeDelta.y / 1.53f);
        addSlotButton.anchoredPosition = new Vector2((panelPowerUp_BG.sizeDelta.x / 2f) , 0);



        onPanelSetupComplete?.Invoke();
    }
}
