using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_List : MonoBehaviour
{
    private GameObject player;

    //private GameObject slotQuestParent;
    //private int activeQuestAmount;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnEnable()
    {
        Init();
    }

    void Init()
    {
        if(QuestManager.Instance == null)
        {
            Instantiate(player);
        }
    }

    void InstantiateParentQuestContainers()
    {
        foreach (Quest quest in QuestManager.Instance._activeQuests)
        {
            CreateNewParentQuestContainer();
        }
    }

    void CreateNewParentQuestContainer()
    {
        GameObject newParentSlotcontainer = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotQuest_Parent"));
        newParentSlotcontainer.transform.SetParent(this.transform);
    }

}
