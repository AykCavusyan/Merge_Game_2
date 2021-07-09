using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest_Slots : MonoBehaviour
{
    private Item.ItemType containedQuestItem;

    private Image itemImage;
    private GameObject checkMark;
    private bool questItemExists = false;
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
        QuestManager.Instance.OnQuestItemExists += EnableCheckMark;
        QuestManager.Instance.OnQuestItemNoMore += DisableCheckMark;
    }

    private void OnDisable()
    {
        QuestManager.Instance.OnQuestItemExists -= EnableCheckMark;
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
        containedQuestItem = containedQuestItemIN.itemType;
        slotID = slotIDIn;
    }

    public void EnableCheckMark(object sender, QuestManager.AddRemoveQuestItemEventArgs e)
    {
        if (!questItemExists && containedQuestItem == e.itemType)
        {
            questItemExists = true;
            checkMark.SetActive(true);
            OnActivateQuestSlot?.Invoke(this, new OnQuestSlotStateChange { questSlot=this, isActive =true});
        }       
    }

    public void DisableCheckMark(object sender,  QuestManager.AddRemoveQuestItemEventArgs e)
    {
        if(questItemExists && containedQuestItem == e.itemType)
        {
            questItemExists = false;
            checkMark.SetActive(false);
            OnDisableQuestSlot?.Invoke(this, new OnQuestSlotStateChange { questSlot=this, isActive=false} );
        }       
    }


    //private void TryActivateQuestSlot(object sender, QuestManager.AddRemoveQuestItemEventArgs e)
    //{
    //    if ( containedQuestItem == e.itemType)
    //    {
    //        //questItemExists = true;
    //        checkMark.SetActive(true);

    //        OnActivateQuestSlot?.Invoke(this, this);
    //    }
    //}

  
}
