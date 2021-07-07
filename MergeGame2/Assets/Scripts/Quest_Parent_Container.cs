using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class Quest_Parent_Container : MonoBehaviour
{
    private int slotAmount;
    private string questName;

    private List<Item.ItemType> requiredItems;
    
    private Sprite[] sprite;
    private Quest quest;


    private Text nameText;
    private GameObject[] slots;
    private GameObject completeButton;
    //private GameObject[] plusIcons;

    private bool canComplete = false;

    private void Awake()
    {
        nameText = transform.GetChild(2).GetComponent<Text>();
        completeButton = transform.GetChild(3).GetChild(1).GetChild(1).gameObject;
        
    }

    private void Start()
    {
        slotAmount = quest.itemsNeeded.Count;
        questName = quest.questName;
        SetActivateButtonState(canComplete);
        SetQuestName(questName);
        PopulateSpriteArray(slotAmount);
        InstantiateSlots(slotAmount, sprite);
    }

    void SetQuestName(string questNameIN)
    {
        nameText.text = questNameIN;
    }

    void InstantiateSlots(int reqItemAmountIN, Sprite[] reqItemSprite)
    {
        slots = new GameObject[reqItemAmountIN];

        for (int i = 0; i < reqItemAmountIN; i++)
        {
            GameObject newSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotQuests"));
            slots[i] = newSlot;
            newSlot.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = reqItemSprite[i];
            // change the alpha of the slotcontainer as well !! or wont be visible 
            
            if(i+1 <= reqItemAmountIN)
            {
                //INSTANTIATE PLUS ICON !!!!!!
            }

        }
    }

    void PopulateSpriteArray(int slotAmountIN)
    {
        sprite = new Sprite[slotAmountIN];

        for (int i = 0; i < slotAmountIN; i++)
        {
            sprite[i] = quest.itemsNeeded[i].GetSprite(quest.itemsNeeded[i].itemType); 
        }
    }


    void SetActivateButtonState(bool canCompleteIN)
    {
        //completeButton // activate the button check how I activated the level button might not be suitable !! see it
    }




}


