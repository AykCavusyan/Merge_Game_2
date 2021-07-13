using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class Quest_Parent_Container : MonoBehaviour
{
    public Quest quest { get; private set; }
    private Item[] reqItemTypes;
    private int slotAmount;
    private Text nameText;

    private GameObject[] slots;
    private Transform slots_Parent;

    private GameObject completeButton;    

    private bool[] allSlotsCheck;
    private bool canComplete = false;

    private void Awake()
    {

        nameText = transform.GetChild(2).GetComponent<Text>();
        completeButton = transform.GetChild(3).GetChild(1).gameObject;
        slots_Parent = transform.GetChild(3).GetChild(0);
    }



    public void CreateQuestParentContainer(Quest questIN)
    {
        quest = questIN;
        slotAmount = questIN.itemsNeeded.Count;
        allSlotsCheck = new bool[slotAmount];
        PopulateReqItemArray(slotAmount, questIN);
        CreateSlotsAndButton(slotAmount, reqItemTypes, questIN);
        SetActivateButtonState(canComplete);
        SetQuestName(questIN);
    }

    void SetQuestName(Quest questIN)
    {
        nameText.text = questIN.questName;
    }

    void CreateSlotsAndButton(int reqItemAmountIN, Item[]reqItemType, Quest quest)
    {
        completeButton.AddComponent<Button_CompleteQuest>().CreateButton(quest);

        slots = new GameObject[reqItemAmountIN];

        for (int i = 0; i < reqItemAmountIN; i++)
        {

            GameObject newSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotQuests"));
            newSlot.transform.SetParent(slots_Parent,false);

            slots[i] = newSlot;

            //newSlot.AddComponent<Quest_Slots>().CreateQuestSlot(reqItemType[i], i);
            //newSlot.GetComponent<Quest_Slots>().OnActivateQuestSlot += TryActivateCompleteButton;
            //newSlot.GetComponent<Quest_Slots>().OnDisableQuestSlot += TryActivateCompleteButton;

            newSlot.AddComponent<Quest_Slots>().OnActivateQuestSlot += TryActivateCompleteButton;
            newSlot.GetComponent<Quest_Slots>().OnDisableQuestSlot += TryActivateCompleteButton;
            newSlot.GetComponent<Quest_Slots>().CreateQuestSlot(reqItemType[i], i);


            if (i+1 <= reqItemAmountIN-1)
            {
                GameObject newPlusIcon = Instantiate(Resources.Load<GameObject>("Prefabs/" + "PlusMark_QuestSlots"));
                newPlusIcon.transform.SetParent(slots_Parent,false);
            }
        }
        
    }

    void PopulateReqItemArray(int slotAmountIN, Quest questIN)
    {
        reqItemTypes = new Item[slotAmountIN];

        for (int i = 0; i < slotAmountIN; i++)
        {
            reqItemTypes[i] = questIN.itemsNeeded[i];
        }
    }


    void TryActivateCompleteButton(object sender, Quest_Slots.OnQuestSlotStateChange e)
    {
        Debug.Log("tryactivatecompletebutton");
        allSlotsCheck[e.questSlot.slotID] = e.isActive;

        if (!allSlotsCheck.Any(x => x == false))
        {
            Debug.Log("allSlotsCheck slots ALL TRUE");
            canComplete = true;
            SetActivateButtonState(canComplete);
        }
        else
        {
            Debug.Log("allSlotsCheck slots CONTAINS FALSE");

            canComplete = false;
            SetActivateButtonState(canComplete);
        }
    }

    void SetActivateButtonState(bool canCompleteIN)
    {
        completeButton.GetComponent<Button_CompleteQuest>().SetButtonAvalibility(canCompleteIN);
    }

    private void OnDisable()
    {
        foreach (GameObject slot in slots)
        {
            if(slot != null)
            {
                slot.GetComponent<Quest_Slots>().OnActivateQuestSlot -= TryActivateCompleteButton;
                slot.GetComponent<Quest_Slots>().OnDisableQuestSlot -= TryActivateCompleteButton;

            }
        }
    }

}


