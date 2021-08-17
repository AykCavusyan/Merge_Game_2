using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_LowerButtonsPanel : MonoBehaviour
{
    private GridLayoutGroup gridLayoutGroup;
    private Canvas canvas;
    float canvasWidth;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        canvasWidth = canvas.GetComponent<RectTransform>().sizeDelta.x;
        
        Vector2 cellsize = new Vector2(Mathf.RoundToInt(canvasWidth / 6.2f), Mathf.RoundToInt(canvasWidth / 6.2f));
        Vector2 spacing = new Vector2(Mathf.RoundToInt(canvasWidth / 81), 0);
        gridLayoutGroup.cellSize = cellsize;
        gridLayoutGroup.spacing = spacing;
        gridLayoutGroup.padding.left = Mathf.RoundToInt(canvasWidth / 10);
        gridLayoutGroup.padding.right = Mathf.RoundToInt(canvasWidth / 10);
        gridLayoutGroup.padding.bottom = Mathf.RoundToInt(gridLayoutGroup.padding.right / 3f);
    }


}
