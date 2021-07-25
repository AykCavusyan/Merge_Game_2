
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
        Debug.Log(currentSlotAmount);

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
        //currentNewSlot.AddComponent<SaveableEntitiy>();
        slotIDNumber++;
        currentNewSlot.GetComponent<InventorySlots>().slotIDNumber = slotIDNumber;
        currentNewSlot.GetComponent<InventorySlots>().isPurchasedOnSession = isPurchasedOnSession;
        PlayerInfo.Instance.ListenInventorySlots(currentNewSlot.GetComponent<InventorySlots>());   // event daha iyi olaiblir mi ?

        if (isActive == true)
        {
            PlayerInfo.Instance.GenerateDictionary(currentNewSlot.GetComponent<InventorySlots>());
        }

        else if (isActive == false)
        {
            ListenInactiveSlot(currentNewSlot);
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

    void ListenInactiveSlot(GameObject currentNewSlot)
    {
        currentNewSlot.GetComponent<InventorySlots>().onSlotPurchaseAttempt += PurchaseSlot;
    }

 

    void PurchaseSlot(GameObject sender)
    {
        InventorySlots inventorySlot = sender.GetComponent<InventorySlots>();
        sender.transform.GetChild(0).GetComponent<Image>().color = inventorySlot.originalColor;
        sender.transform.Find("Lock").gameObject.SetActive(false);
        inventorySlot.isActive = true;


        if (PlayerInfo.Instance.currentInventorySlotAmount < PlayerInfo.Instance.maxInventorySlotAmount)
        {
            currentSlotAmount++;
            PlayerInfo.Instance.AugmentCurrentInventorySlotAmount(currentSlotAmount);
            PlayerInfo.Instance.GenerateDictionary(sender.GetComponent<InventorySlots>());

            CreateNewSlot(false,true);
            StopListeningPreviousSlot(sender);
        }
    }
    void StopListeningPreviousSlot(GameObject oldSlot)
    {
        oldSlot.GetComponent<InventorySlots>().onSlotPurchaseAttempt -= PurchaseSlot;
    }

    private void OnDisable()
    {
        InventorySlots[] currentInventorySlots = FindObjectsOfType<InventorySlots>();

        if (currentInventorySlots.Length > 0)
        {
            foreach (InventorySlots inventorySlot in currentInventorySlots)
            {
                inventorySlot.onSlotPurchaseAttempt -= PurchaseSlot;
            }
        }
    }
}
