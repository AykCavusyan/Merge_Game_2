using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_TopIconsPanel : MonoBehaviour
{
    private RectTransform levelBar;
    private RectTransform levelIcon;

    private RectTransform goldBar;
    private RectTransform goldIcon;

    private RectTransform energyBar;
    private RectTransform energyIcon;

    private RectTransform gemBar;
    private RectTransform gemIcon;

    private RectTransform canvas;

    private void Awake()
    {
        canvas = transform.parent.GetComponent<RectTransform>();

        levelBar = transform.GetChild(0).GetComponent<RectTransform>();
        levelIcon = levelBar.GetChild(0).GetChild(0).GetComponent<RectTransform>();

        goldBar = transform.GetChild(1).GetComponent<RectTransform>();
        goldIcon = goldBar.GetChild(0).GetChild(0).GetComponent<RectTransform>();

        energyBar = transform.GetChild(2).GetComponent<RectTransform>();
        energyIcon = energyBar.GetChild(0).GetChild(0).GetComponent<RectTransform>();

        gemBar = transform.GetChild(3).GetComponent<RectTransform>();
        gemIcon = gemBar.GetChild(0).GetChild(0).GetComponent<RectTransform>();
    }

    private void Start()
    {
        float canvasWidth = canvas.sizeDelta.x;

        GetComponent<RectTransform>().sizeDelta = new Vector2(canvasWidth, GetComponent<RectTransform>().sizeDelta.y);

        float paddingBetween = canvasWidth / 22f;
        float topPadding = canvasWidth / 40.5f;

        float barSizeBigX = canvasWidth / 3.85f;     
        float barSizeSmallX = canvasWidth /6f;
        float barSizeY = barSizeBigX / 4.66f;


        levelBar.sizeDelta = new Vector2(barSizeBigX, barSizeY);
        levelBar.anchoredPosition = new Vector2(canvasWidth / 4.76f, topPadding);
        levelIcon.sizeDelta = new Vector2(canvasWidth / 8.1f, canvasWidth / 8.1f);
        levelIcon.anchoredPosition = new Vector2(canvasWidth / 81f * -1, 0);
        levelIcon.GetComponent<CircleCollider2D>().offset = new Vector2(0, 0);
        levelIcon.GetComponent<CircleCollider2D>().radius = levelIcon.sizeDelta.x / 3.4f;


        goldBar.sizeDelta = new Vector2(barSizeSmallX, barSizeY);
        goldBar.anchoredPosition = new Vector2(levelBar.anchoredPosition.x + (levelBar.sizeDelta.x / 2) + paddingBetween + barSizeSmallX /2, topPadding);
        goldIcon.sizeDelta = new Vector2(canvasWidth / 13.6f, canvasWidth / 13.6f);
        goldIcon.anchoredPosition = new Vector2(canvasWidth / 162f, 0);
        goldIcon.GetComponent<CircleCollider2D>().offset = new Vector2(0, 0);
        goldIcon.GetComponent<CircleCollider2D>().radius = goldIcon.sizeDelta.x / 2f;

        energyBar.sizeDelta = new Vector2(barSizeSmallX, barSizeY);
        energyBar.anchoredPosition = new Vector2(goldBar.anchoredPosition.x + (goldBar.sizeDelta.x / 2) + paddingBetween + barSizeSmallX / 2, topPadding);
        energyIcon.sizeDelta = new Vector2(canvasWidth / 13.6f, canvasWidth / 13.6f);
        energyIcon.anchoredPosition = new Vector2(canvasWidth / 162f, 0);

        gemBar.sizeDelta = new Vector2(barSizeSmallX, barSizeY);
        gemBar.anchoredPosition = new Vector2(energyBar.anchoredPosition.x + (energyBar.sizeDelta.x / 2) + paddingBetween + barSizeSmallX / 2, topPadding);
        gemIcon.sizeDelta = new Vector2(canvasWidth / 13.6f, canvasWidth / 13.6f);
        gemIcon.anchoredPosition = new Vector2(canvasWidth / 162f, 0);
    }
}
