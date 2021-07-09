using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_List : MonoBehaviour
{
    private GameObject player;
    private Transform innerPanelContainer;
    
    //private GameObject slotQuestParent;
    //private int activeQuestAmount;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        innerPanelContainer = transform.GetChild(1).GetChild(0);
    }

    private void OnEnable()
    {
        Init();
        QuestManager.Instance.OnQuestAdded += InstantiateParentQuestContainers;
    }

    private void OnDisable()
    {
        QuestManager.Instance.OnQuestAdded -= InstantiateParentQuestContainers;
    }

    void Init()
    {
        if(QuestManager.Instance == null)
        {
            Instantiate(player);
        }
    }



    void InstantiateParentQuestContainers(object sender, QuestManager.OnQuestAddedEventArgs e)
    {
        CreateNewParentQuestContainer(e.quest);

    }



    void CreateNewParentQuestContainer(Quest quest)
    {
        GameObject newParentSlotcontainer = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotQuest_Parent"));
        newParentSlotcontainer.transform.SetParent(innerPanelContainer);
        newParentSlotcontainer.AddComponent<Quest_Parent_Container>().CreateQuestParentContainer(quest);
    }

}
