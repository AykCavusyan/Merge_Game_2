using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Quest_Slots : MonoBehaviour
{
    private Item.ItemType containedQuestItem;

    private Image itemImage;
    private GameObject checkMark;
    [SerializeField] private bool questItemExists = false;
    private GameObject player;
    public int slotID;

    public event EventHandler<OnQuestSlotStateChange> OnActivateQuestSlot;
    public event EventHandler<OnQuestSlotStateChange> OnDisableQuestSlot;
    public class OnQuestSlotStateChange
    {
        public Quest_Slots questSlot;
        public bool isActive;
    }



    private void Awake()
    {
        itemImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        checkMark = transform.GetChild(1).gameObject;
        checkMark.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnEnable()
    {
        Init();
        ItemBag.Instance.OnGameItemCreated += EnableCheckMark;
        //MasterEventListener.Instance.OnItemIsQuestEventMaster += EnableCheckMark; bunun masterdan belki çýkarýrýz bile !!!!
        QuestManager.Instance.OnQuestItemNoMore += DisableCheckMark;
    }

    private void OnDisable()
    {
        //MasterEventListener.Instance.OnItemIsQuestEventMaster -= EnableCheckMark;
        QuestManager.Instance.OnQuestItemNoMore -= DisableCheckMark;
    }



    void Init()
    {
        if (QuestManager.Instance == null)
        {
            Instantiate(player);
        }
    }

    public void CreateQuestSlot(Item containedQuestItemIN, int slotIDIn)
    {
        itemImage.sprite = containedQuestItemIN.GetSprite(containedQuestItemIN.itemType);
        itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 1);
        containedQuestItem = containedQuestItemIN.itemType;
        slotID = slotIDIn;

        InitialObjectAvailibilityCheck(containedQuestItemIN);
    }

    private void InitialObjectAvailibilityCheck(Item containedQuestItemIN)
    {
        var existingSlotItemsList = SlotsCounter.Instance.slotDictionary.Values.Where(x => x != null).ToList();

        foreach (GameItems gameItem in existingSlotItemsList)
        {
            if(gameItem.itemType == containedQuestItem)
            {
                questItemExists = true;
                checkMark.SetActive(true);
                OnActivateQuestSlot?.Invoke(this, new OnQuestSlotStateChange { questSlot = this, isActive = true });

                return;
            }
        }

    }


    public void EnableCheckMark(object sender, ItemBag.OnGameItemCreatedEventArgs e)
    {
        if (containedQuestItem == e.gameItem.itemType)
        {
            questItemExists = true;
            checkMark.SetActive(true);
            OnActivateQuestSlot?.Invoke(this, new OnQuestSlotStateChange { questSlot=this, isActive =true});
        }       
    }

    public void DisableCheckMark(object sender,  QuestManager.AddRemoveQuestItemEventArgs e)
    {
        if(containedQuestItem == e.itemType)
        {
            questItemExists = false;
            checkMark.SetActive(false);
            OnDisableQuestSlot?.Invoke(this, new OnQuestSlotStateChange { questSlot=this, isActive=false} );
        }       
    }



  
}
