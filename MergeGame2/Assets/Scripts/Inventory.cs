
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private Transform inner_Panel_Container;
    private GridLayoutGroup gridLayoutGroup;
    public GameObject player;

    private int currentSlotAmount;
    private GameObject currentNewSlot;

    public event EventHandler<OnInventorySlotCreatedEventArgs> OnInventorySlotCreated;
    public class OnInventorySlotCreatedEventArgs
    {
        public InventorySlots newActiveOrInactiveInventorySlot;
    }

    private int slotIDNumber = 0;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inner_Panel_Container = transform.GetChild(1).GetChild(0).GetComponent<Transform>(); // bu transform burada gerekli mi ???
        gridLayoutGroup = inner_Panel_Container.GetComponent<GridLayoutGroup>();
    }


    private void OnEnable()
    {
        Init();
    }

    private void Start()
    {
        float cellSize = inner_Panel_Container.GetComponent<RectTransform>().rect.width / 4.7f;
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
  
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

    public void ConfigPanel(Dictionary<int, object> _itemsDictIN)
    {
        currentSlotAmount = PlayerInfo.Instance.currentInventorySlotAmount;

        if (currentSlotAmount > 0)
        {
            InstantiateSlots(currentSlotAmount, _itemsDictIN);
        }
    }

    void InstantiateSlots(int currentSlotAmountIN, Dictionary<int, object> _itemsDictIN)
    {

        for (int i = 0; i < currentSlotAmountIN; i++)
        {

            GameObject instantiatedSlot = CreateNewSlot(true);
            int inventorySlotID = instantiatedSlot.GetComponent<InventorySlots>().slotIDNumber;

            if (_itemsDictIN.ContainsKey(inventorySlotID))
            {
                
                instantiatedSlot.GetComponent<InventorySlots>().RestoreState(_itemsDictIN[inventorySlotID]);
            }
        }

        CreateNewSlot(false);
        DisableChildrenImages();
    }

     private GameObject CreateNewSlot(bool isActive, bool isPurchasedOnSession = false)
    {
        currentNewSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotBackgroundActive"));

        currentNewSlot.transform.SetParent(inner_Panel_Container, false);
        currentNewSlot.AddComponent<InventorySlots>().isActive = isActive;
        slotIDNumber++;
        currentNewSlot.GetComponent<InventorySlots>().slotIDNumber = slotIDNumber;
        currentNewSlot.GetComponent<InventorySlots>().isPurchasedOnSession = isPurchasedOnSession;

        OnInventorySlotCreated?.Invoke(this, new OnInventorySlotCreatedEventArgs { newActiveOrInactiveInventorySlot = currentNewSlot.GetComponent<InventorySlots>() });
        //PlayerInfo.Instance.ListenInventorySlots(currentNewSlot.GetComponent<InventorySlots>());   // event daha iyi olaiblir mi ?

        if (isActive == true)
        {
            PlayerInfo.Instance.GenerateDictionary(currentNewSlot.GetComponent<InventorySlots>());
        }

        else if (isActive == false)
        {
            currentNewSlot.GetComponent<InventorySlots>().OnSlotPurchased += PurchaseSlot;
        }

        return currentNewSlot;

    } 

    

    void DisableChildrenImages()
    {
        var childrenToDisable = inner_Panel_Container.GetComponentsInChildren<Image>();
        foreach (var childToDisable in childrenToDisable)
        {
            childToDisable.enabled = false;
        }
    }

    //void ListenInactiveSlot(object sender, InventorySlots.onInventoryItemModificationEventArgs e)
    //{
    //    e.slot.OnSlotPurchased += PurchaseSlot;
    //}

 

    void PurchaseSlot(object sender, MasterEventListener.OnFinancialEvent e) //  InventorySlots.onInventoryItemModificationEventArgs e)
    {
        //InventorySlots inventorySlot = sender.GetComponent<InventorySlots>();
        e.inventorySlot.transform.GetChild(0).GetComponent<Image>().color = e.inventorySlot.originalColor;
        e.inventorySlot.transform.Find("Lock").gameObject.SetActive(false);
        e.inventorySlot.isActive = true;


        //if (PlayerInfo.Instance.currentInventorySlotAmount < PlayerInfo.Instance.maxInventorySlotAmount)
        //{
            currentSlotAmount++;
            PlayerInfo.Instance.AugmentCurrentInventorySlotAmount(currentSlotAmount);
            PlayerInfo.Instance.GenerateDictionary(e.inventorySlot);

            CreateNewSlot(false,true);
            StopListeningPreviousSlot(e.inventorySlot);
        //}
    }
    void StopListeningPreviousSlot(InventorySlots inventorySlotIN)
    {
        inventorySlotIN.OnSlotPurchased -= PurchaseSlot;
    }

    private void OnDisable()
    {
        InventorySlots[] currentInventorySlots = FindObjectsOfType<InventorySlots>();

        if (currentInventorySlots.Length > 0)
        {
            foreach (InventorySlots inventorySlot in currentInventorySlots)
            {
                inventorySlot.OnSlotPurchased -= PurchaseSlot;
            }
        }
    }
}
