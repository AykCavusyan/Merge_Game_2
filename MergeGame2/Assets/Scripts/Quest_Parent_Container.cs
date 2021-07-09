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
    private GameObject completeButton;
    private Color completeInactiveButtonColor;
    private GameObject[] plusIcons;

    private bool[] allSlotsCheck;
    private bool canComplete = false;

    private void Awake()
    {
        //////////slots = transform.Fin("SlotQuests").gameObject];
        
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = transform.GetChild(i).gameObject;
            slots[i].SetActive(false);
        }

        nameText = transform.GetChild(2).GetComponent<Text>();
        completeButton = transform.GetChild(3).GetChild(1).GetChild(1).gameObject; 
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
        slotAmount = quest.itemsNeeded.Count;
        allSlotsCheck = new bool[slotAmount];
        PopulateReqItemArray(slotAmount, quest);
        ActivateSlots(slotAmount, reqItemTypes, quest);
        SetActivateButtonState(canComplete);
        SetQuestName(quest);
    }

    void SetQuestName(Quest questIN)
    {
        nameText.text = questIN.questName;
    }

    void ActivateSlots(int reqItemAmountIN, Item[]reqItemType, Quest quest)
    {
        
        
        completeButton.AddComponent<Button_CompleteQuest>().CreateButton(quest);

        for (int i = 0; i < reqItemAmountIN; i++)
        {
            slots[i].SetActive(true);
            slots[i].AddComponent<Quest_Slots>().CreateQuestSlot(reqItemType[i], i);
            slots[i].GetComponent<Quest_Slots>().OnActivateQuestSlot += TryActivateCompleteButton;

            //GameObject newSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotQuests"));
            //slots[i] = newSlot;
            //newSlot.AddComponent<Quest_Slots>().CreateQuestSlot(reqItemType[i], i);
            //newSlot.GetComponent<Quest_Slots>().OnActivateQuestSlot += TryActivateCompleteButton;

            //newSlot.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = reqItemSprite[i];
            //change the alpha of the slotcontainer as well !! or wont be visible 

            if (i+1 <= reqItemAmountIN)
            {
                slots[i + 1].SetActive(true);
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

    }

    private void OnDisable()
    {
        foreach (GameObject slot in slots)
        {
            if(slot != null)
            {
                slot.GetComponent<Quest_Slots>().OnActivateQuestSlot -= TryActivateCompleteButton;
            }
        }
    }

}


