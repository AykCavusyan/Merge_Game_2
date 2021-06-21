
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
    private GameObject currentNewSlot;

    private int slotIDNumber = 0;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inner_Panel_Container = GameObject.Find("Inner_Panel_Container").GetComponent<Transform>();  
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    PurchaseSLot();
        //}
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

     void CreateNewSlot(bool isActive, bool isPurchasedOnSession = false)
    {
        currentNewSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotBackgroundActive"));

        currentNewSlot.transform.SetParent(inner_Panel_Container, false);
        currentNewSlot.AddComponent<InventorySlots>().isActive = isActive ;

        slotIDNumber++;
        currentNewSlot.GetComponent<InventorySlots>().slotIDNumber = slotIDNumber;
        currentNewSlot.GetComponent<InventorySlots>().isPurchasedOnSession = isPurchasedOnSession;

        if(isActive == false)
        {
            ListenInactiveSlot(currentNewSlot);
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

    void ListenInactiveSlot(GameObject currentNewSlot)
    {
        currentNewSlot.GetComponent<InventorySlots>().onSlotPurchaseAttempt += PurchaseSlot;
    }

    void StopListeningPreviousSlot(GameObject oldSlot)
    {
        oldSlot.GetComponent<InventorySlots>().onSlotPurchaseAttempt -= PurchaseSlot;
    }

    void PurchaseSlot(GameObject sender)
    {
        StopListeningPreviousSlot(sender);

        // burda potansiyel bir hata var currenti arttýýrrken if e bakmýyoruz
        currentSlotAmount++;
        PlayerInfo.Instance.AugmentCurrentInventorySlotAmount(currentSlotAmount);

        if (PlayerInfo.Instance.currentInventorySlotAmount < PlayerInfo.Instance.maxInventorySlotAmount)
        {
            CreateNewSlot(false,true);

        }
    }

    private void OnDisable()
    {
        currentNewSlot.GetComponent<InventorySlots>().onSlotPurchaseAttempt -= PurchaseSlot;
    }
}
