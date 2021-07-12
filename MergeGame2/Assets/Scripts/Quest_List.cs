using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_List : MonoBehaviour
{
    private GameObject player;
    private GameObject innerPanelContainer;
    
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
        //QuestManager.Instance.OnQuestAdded += InstantiateParentQuestContainers;
    }

    private void OnDisable()
    {
        //QuestManager.Instance.OnQuestAdded -= InstantiateParentQuestContainers;
    }

    void Init()
    {
        if(QuestManager.Instance == null)
        {
            Instantiate(player);
        }
    }



    public void InstantiateParentQuestContainers(Quest questIN)
    {
        //Debug.Log(e.quest.itemsNeeded.Count);
        GameObject newParentSlotcontainer = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotQuest_Parent"));
        newParentSlotcontainer.transform.SetParent(innerPanelContainer.transform, false);
        newParentSlotcontainer.GetComponent<Quest_Parent_Container>().CreateQuestParentContainer(questIN);
        
    }




}
