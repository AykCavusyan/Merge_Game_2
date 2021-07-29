using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_List : MonoBehaviour
{
    private GameObject player;
    private GameObject innerPanelContainer;
    private List<GameObject> parentSlotConainers = new List<GameObject>();
    
    //private GameObject slotQuestParent;
    //private int activeQuestAmount;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        innerPanelContainer = transform.GetChild(1).GetChild(0).gameObject;
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
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
        newParentSlotContainer.transform.SetParent(innerPanelContainer.transform, false);
        newParentSlotContainer.GetComponent<Quest_Parent_Container>().CreateQuestParentContainer(questIN);

        parentSlotConainers.Add(newParentSlotContainer);
        return newParentSlotContainer;
        
    }

    public void RemoveParentSlotContainers(Quest questIN)
    {
        foreach (GameObject parentSlotContainer in parentSlotConainers)
        {
            if(parentSlotContainer.GetComponent<Quest_Parent_Container>().quest == questIN)
            {
                parentSlotConainers.Remove(parentSlotContainer);
                Destroy(parentSlotContainer.gameObject);

                Debug.Log(parentSlotConainers.Count);
                return;
            }
        }
    }


}
