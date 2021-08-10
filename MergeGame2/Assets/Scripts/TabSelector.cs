using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabSelector : MonoBehaviour,IPointerDownHandler
{
    private GameObject innerPanelContainerActiveQuest;
    private GameObject innerPanelContainerInactiveQuest;
    public GameObject focusedPanel { get; private set; }
    private GameObject activeQuestsTab;
    private GameObject inactiveQuestsTab;
    private GameObject activeTab;
    private ScrollRect scrollRect;

    private float panelWidth;
    private float lerpDuration = .1f;

    private GameObject[] panels;
    private GameObject[] tabs;

    private List<(Vector2, Vector2, Vector2)> listOfPanelPpositions = new List<(Vector2, Vector2,Vector2)>();

    private void Awake()
    {
        innerPanelContainerActiveQuest = transform.GetChild(1).GetChild(0).gameObject;
        innerPanelContainerInactiveQuest = transform.GetChild(1).GetChild(1).gameObject;
        activeQuestsTab = GameObject.Find("ActiveQuestsTab");
        inactiveQuestsTab = GameObject.Find("InactiveQuestsTab");
        scrollRect = transform.GetChild(1).GetComponent<ScrollRect>();


        panels = new GameObject[2];
        panels[0] = innerPanelContainerActiveQuest;
        panels[1] = innerPanelContainerInactiveQuest;

        tabs = new GameObject[2];
        tabs[0] = activeQuestsTab;
        tabs[1] = inactiveQuestsTab;
    }

    private void Start()
    {
        panelWidth = innerPanelContainerActiveQuest.GetComponent<RectTransform>().rect.width;
        SetPanelPositions();
    }

    void SetPanelPositions()
    {  
        float panelPositionX = 0;

        foreach (GameObject panel in panels)
        {       
            panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(panelPositionX, 0);
            panelPositionX += panelWidth;
        }
        focusedPanel = panels[0];
        activeTab = tabs[0];
    }

    void SlidePanels(int clickedButtonIndexIN)
    {
        float lateralDisplacementAmount = (Array.IndexOf(panels,focusedPanel) - clickedButtonIndexIN) * panelWidth;
        float overFloatAmout = lateralDisplacementAmount * 1.1f;

        foreach (GameObject panel in panels)
        {
            RectTransform rt = panel.GetComponent<RectTransform>();
            Vector2 originalPpos = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y);
            Vector2 floatPos = new Vector2((rt.anchoredPosition.x + overFloatAmout), rt.anchoredPosition.y);
            Vector2 finalPos = new Vector2(rt.anchoredPosition.x + lateralDisplacementAmount, rt.anchoredPosition.y);
            listOfPanelPpositions.Add((originalPpos, floatPos ,finalPos));
        }
        StartCoroutine(SlidePanelsEnum(listOfPanelPpositions, clickedButtonIndexIN));
    }

    IEnumerator SlidePanelsEnum(List<(Vector2,Vector2,Vector2)> listOfPanelPpositionsIN, int clickedButtonIndexIN)
    {
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(listOfPanelPpositionsIN[i].Item1, listOfPanelPpositionsIN[i].Item2, elapsedTime / lerpDuration);
            }
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].GetComponent<RectTransform>().anchoredPosition = listOfPanelPpositionsIN[i].Item2;
        }

        elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(listOfPanelPpositionsIN[i].Item2, listOfPanelPpositionsIN[i].Item3, elapsedTime / lerpDuration);
            }
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].GetComponent<RectTransform>().anchoredPosition = listOfPanelPpositionsIN[i].Item3;
        }

        focusedPanel = panels[clickedButtonIndexIN];
        listOfPanelPpositions.Clear();

        yield return null;
    }


    void ActivateTab(int clickedButtonIndexIN)
    {

        foreach (GameObject tab in tabs)
        {
            string spriteName = tab.GetComponent<Image>().sprite.name;
            if (Array.IndexOf(tabs, tab) == clickedButtonIndexIN)
            {
                if (spriteName.Contains("Disabled"))
                {
                    string newSpriteName = spriteName.Replace("Disabled", "Enabled");
                    tab.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + "UI_Assets/" + "Quest_Panel/" + newSpriteName);
                }          
            }
            else
            {
                if (spriteName.Contains("Enabled"))
                {
                    string newSpriteName = spriteName.Replace("Enabled", "Disabled");
                    tab.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + "UI_Assets/" + "Quest_Panel/" + newSpriteName);
                }
            }
        }

        activeTab = tabs[clickedButtonIndexIN];
    }

    void SetScrollRectFocus(int clickedButtonIndexIN)
    {
        scrollRect.content = panels[clickedButtonIndexIN].GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            foreach (GameObject tab in tabs)
            {
                if (result.gameObject == tab && tab!=activeTab)
                {
                    int clickedButtonIndex = Array.IndexOf(tabs, tab);
                    ActivateTab(clickedButtonIndex);
                    SlidePanels(clickedButtonIndex);
                    SetScrollRectFocus(clickedButtonIndex);
                }              
            }
            return;
        }
    }


}
