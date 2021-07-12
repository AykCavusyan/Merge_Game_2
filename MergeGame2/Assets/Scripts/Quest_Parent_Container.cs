using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class Quest_Parent_Container : MonoBehaviour
{
    private int slotAmount;
    //private string questName;

    //private List<Item.ItemType> requiredItems;

    //private Sprite[] sprite;
    //private Quest quest;
    private Item[] reqItemTypes;

    private Text nameText;

    private GameObject[] slots;
    private Transform slots_Parent;
    //private GameObject[] plusIcons;

    private GameObject completeButton;
    //private Color completeInactiveButtonColor;
    

    private bool[] allSlotsCheck;
    private bool canComplete = false;

    private void Awake()
    {
        //////////slots = transform.Fin("SlotQuests").gameObject];
        //slots = new GameObject[transform.child]
        //for (int i = 0; i < slots.Length; i++)
        //{
        //    slots[i] = transform.GetChild(i).gameObject;
        //    slots[i].SetActive(false);
        //}

        nameText = transform.GetChild(2).GetComponent<Text>();
        completeButton = transform.GetChild(3).GetChild(1).gameObject;
        slots_Parent = transform.GetChild(3).GetChild(0);
    }

    private void Start()
    {
        //slotAmount = quest.itemsNeeded.Count;
        //questName = quest.questName;
        //SetActivateButtonState(canComplete);
        //SetQuestName(questName);
        //PopulateSpriteArray(slotAmount);
        //InstantiateSlots(slotAmount, sprite);
    }

    public void CreateQuestParentContainer(Quest quest)
    {
        Debug.Log(quest);
        slotAmount = quest.itemsNeeded.Count;
        allSlotsCheck = new bool[slotAmount];
        PopulateReqItemArray(slotAmount, quest);
        CreateSlotsAndButton(slotAmount, reqItemTypes, quest);
        SetActivateButtonState(canComplete);
        SetQuestName(quest);
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

            newSlot.AddComponent<Quest_Slots>().CreateQuestSlot(reqItemType[i], i);
            newSlot.GetComponent<Quest_Slots>().OnActivateQuestSlot += TryActivateCompleteButton;
            newSlot.GetComponent<Quest_Slots>().OnDisableQuestSlot += TryActivateCompleteButton;

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

        if (!allSlotsCheck.Contains(false))
        {
            canComplete = true;
            SetActivateButtonState(canComplete);
        }
        else
        {
            canComplete = false;
            SetActivateButtonState(canComplete);
        }
    }

    void SetActivateButtonState(bool canCompleteIN)
    {
        completeButton.GetComponent<Button_CompleteQuest>().canClaim = canCompleteIN;
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


