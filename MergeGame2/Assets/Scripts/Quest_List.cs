using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest_List : MonoBehaviour
{
    private GameObject player;
    private GameObject innerPanelContainerActiveQuest;
    private GameObject innerPanelContainerInactiveQuest;
    private GameObject focusedPanel;
    private TabSelector tabSelector;
    private List<GameObject> combinedPanelList;
    private List<GameObject> parentSlotContainersActive = new List<GameObject>();
    private List<GameObject> parentSlotContainersInactive = new List<GameObject>();

    private Vector3 zeroScale = new Vector3(0, 0, 0);
    private Vector3 upScale = new Vector3(1.1f, 1.1f, 1.1f);
    private Vector3 normalScale = new Vector3(1, 1, 1);
    private float lerpDuration = .05f;
    private bool cr_Running = false;

    private Panel_Invetory panel;

    //private GameObject slotQuestParent;
    //private int activeQuestAmount;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        innerPanelContainerActiveQuest = transform.GetChild(1).GetChild(0).gameObject;
        innerPanelContainerInactiveQuest = transform.GetChild(1).GetChild(1).gameObject;
        tabSelector = GetComponent<TabSelector>();
        panel = GetComponent<Panel_Invetory>();
    }


    private void OnEnable()
    {
        Init();
        if(panel.panelIndex == 3)
        {
            panel.OnPanelSized += PlaceParentSlotcontainers;
            panel.OnPanelDisappear += DeplaceParentSlotContainers;
        }
    }

    private void OnDisable()
    {
        if (panel.panelIndex == 3)
        {
            panel.OnPanelSized -= PlaceParentSlotcontainers;
            panel.OnPanelDisappear -= DeplaceParentSlotContainers;

        }
    }

    void Init()
    {
        if(QuestManager.Instance == null)
        {
            Instantiate(player);
        }
    }


    public GameObject InstantiateParentQuestContainers(Quest questIN)
    {
        GameObject newParentSlotContainer = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotQuest_Parent"));
        newParentSlotContainer.GetComponent<RectTransform>().localScale = zeroScale;
        newParentSlotContainer.transform.SetParent(innerPanelContainerActiveQuest.transform, false);
        newParentSlotContainer.GetComponent<Quest_Parent_Container>().CreateQuestParentContainer(questIN);

        parentSlotContainersActive.Add(newParentSlotContainer);

        StartCoroutine(ParentQuestContainersUpSizeEnum(newParentSlotContainer));

        return newParentSlotContainer;
        
    }

    public void TransferCompletedParentSlotContainers(Quest questIN)
    {
        foreach (GameObject parentSlotContainer in parentSlotContainersActive)
        {
            if(parentSlotContainer.GetComponent<Quest_Parent_Container>().quest == questIN)
            {
                parentSlotContainersActive.Remove(parentSlotContainer);
                parentSlotContainersInactive.Add(parentSlotContainer);

                parentSlotContainer.transform.SetParent(innerPanelContainerInactiveQuest.transform, false);

                //Destroy(parentSlotContainer.gameObject);

                return;
            }
        }
    }

    void PlaceParentSlotcontainers(object sender, EventArgs e)
    {
        focusedPanel = tabSelector.focusedPanel;

        if (focusedPanel.name == "Inner_Panel_Container_QuestPanel_ActiveQuest")
        {
            combinedPanelList = parentSlotContainersActive.Concat(parentSlotContainersInactive).ToList();
        }

        else
        {
            combinedPanelList = parentSlotContainersInactive.Concat(parentSlotContainersActive).ToList();
        }

        StartCoroutine(PlaceParentSlotcontainersEnum(combinedPanelList));
    }

    IEnumerator PlaceParentSlotcontainersEnum(List<GameObject> combinedListIN)
    {
        foreach (GameObject parentSlotContainer in combinedListIN)
        {
            StartCoroutine(ParentQuestContainersUpSizeEnum(parentSlotContainer));
            
            while (cr_Running == true)
            {
                yield return null;
            }
        }
    }

    IEnumerator ParentQuestContainersUpSizeEnum(GameObject newParentSlotContainerIN)
    {
        cr_Running = true;

        float elapsedTime = 0f;
        RectTransform rt = newParentSlotContainerIN.GetComponent<RectTransform>();

        while (elapsedTime < lerpDuration)
        {
            rt.localScale = Vector3.Lerp(zeroScale, upScale, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rt.localScale = upScale;


        elapsedTime = 0f;
        while (elapsedTime < lerpDuration/1.5f)
        {
            rt.localScale = Vector3.Lerp(upScale, normalScale, elapsedTime / (lerpDuration/1.5f));
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rt.localScale = normalScale;

        cr_Running = false;
    }

    void DeplaceParentSlotContainers(object sender, Panel_Invetory.OnPanelStateChangeEventArgs e)
    {
        StopAllCoroutines();

        List<GameObject> combinedList = parentSlotContainersActive.Concat(parentSlotContainersInactive).ToList();
        foreach (GameObject parentSlotContainer in combinedList)
        {
            parentSlotContainer.GetComponent<RectTransform>().localScale = zeroScale;
        }    
    }

   
}
