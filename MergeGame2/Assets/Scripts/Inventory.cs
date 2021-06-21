
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private Transform inner_Panel_Container;
    public GameObject player;

    private int currentSlotAmount;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inner_Panel_Container = transform.Find("Inner_Panel_Container");  
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PurchaseSLot();
        }
    }

    private void OnEnable()
    {
        Init();
    }

    private void Start()
    {
        currentSlotAmount = PlayerInfo.Instance.currentInventorySlotAmount;

        if (currentSlotAmount > 0)
        {
            InstantiateSlots();
        }
        
    }


    void Init()
    {
        if(PlayerInfo.Instance == null)
        {
            Instantiate(player);
        }
        else
        {

        }
    }

    void InstantiateSlots()
    {
        for (int i = 0; i < currentSlotAmount; i++)
        {
            CreateNewSlot(true);
        }

        CreateNewSlot(false);

        DisableChildrenImages();
    }

     void CreateNewSlot(bool isActive)
    {
        GameObject newSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotBackgroundActive"));

        newSlot.transform.SetParent(inner_Panel_Container, false);
        newSlot.AddComponent<InventorySlots>().isActive = isActive;

        if(isActive == false)
        {
            ListenInactiveSlot(newSlot);
        }
    } 

    

    void DisableChildrenImages()
    {
        var childrenToDisable = inner_Panel_Container.GetComponentsInChildren<Image>();
        foreach (var childToDisable in childrenToDisable)
        {
            childToDisable.enabled = false;
        }
    }

    void ListenInactiveSlot(GameObject newSLot)
    {
        newSLot.GetComponent<InventorySlots>().onSlotPurchaseAttempt += PurchaseSLot;
    }

    void PurchaseSLot()
    {

        currentSlotAmount++;
        PlayerInfo.Instance.AugmentCurrentInventorySlotAmount(currentSlotAmount);

        if (PlayerInfo.Instance.currentInventorySlotAmount < PlayerInfo.Instance.maxInventorySlotAmount)
        {
            CreateNewSlot(false);

        }
    }
}
