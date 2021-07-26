using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSelector : MonoBehaviour
{
    public GameItems selectedItem { get; private set; }
    private GameObject selectionSquare;
    private Inventory inventory;

    public event EventHandler<OnGameItemSelectedEventArgs> OnGameItemSelected;
    public event EventHandler<OnGameItemSelectedEventArgs> OnGameItemDeSelected;
    public class OnGameItemSelectedEventArgs
    {
        public Item.ItemType itemType;
        public int goldValue;
    }

    
    private void Awake()
    {
        selectionSquare = GameObject.Find("SelectionSquare");
        selectionSquare.SetActive(false);

        inventory = GameObject.FindObjectOfType<Inventory>();
    }

    private void OnEnable()
    {
        MasterEventListener.Instance.OnDestroyedMasterEvent += DisableSelectionForDestroyedItem;
        ItemBag.Instance.OnGameItemCreated += StartListeningGameItem;
        inventory.OnInventorySlotCreated += StartListeningInventorySlot;
    }

    private void OnDisable()
    {
        MasterEventListener.Instance.OnDestroyedMasterEvent -= DisableSelectionForDestroyedItem;
        ItemBag.Instance.OnGameItemCreated -= StartListeningGameItem;
        if(!inventory) inventory.OnInventorySlotCreated -= StartListeningInventorySlot;

        GameItems[] gameItems = FindObjectsOfType<GameItems>();
        if (gameItems.Length > 0)
        {
            foreach (GameItems gameItem in gameItems)
            {
                if (gameItem)
                {
                    gameItem.OnGameItemClicked -= EnableSelection;
                    gameItem.OnBeginDragHandler -= PauseSelectionOnItemMove;
                    gameItem.OnEndDragHandler -= ResumeSelectionOnItemRelease;
                    gameItem.OnMerged -= SelectNextMergedItem;
                }
            }
        }

        InventorySlots[] inventorySlots = FindObjectsOfType<InventorySlots>();
        if (inventorySlots.Length > 0)
        {
            foreach (InventorySlots inventorySlot in inventorySlots)
            {
                if (inventorySlot)
                {
                    inventorySlot.onInventoryPlacedItem -= DisableSelectionToInventoryDrop;
                    inventorySlot.onInventoryRemovedItem -= EnableSelectionFromInventoryDrop;
                }
            }
        }
    }

    void StartListeningGameItem(object sender, ItemBag.OnGameItemCreatedEventArgs e )
    {
        e.gameItem.OnGameItemClicked += EnableSelection;
        e.gameItem.OnBeginDragHandler += PauseSelectionOnItemMove; 
        e.gameItem.OnEndDragHandler += ResumeSelectionOnItemRelease; 
        e.gameItem.OnMerged += SelectNextMergedItem; 
    }

    void StopListeningGameItem(GameItems gameItemIN)
    {
        gameItemIN.OnGameItemClicked -= EnableSelection;
        gameItemIN.OnBeginDragHandler -= PauseSelectionOnItemMove;
        gameItemIN.OnEndDragHandler -= ResumeSelectionOnItemRelease;
        gameItemIN.OnMerged -= SelectNextMergedItem;
    }

    void StartListeningInventorySlot(object sender, Inventory.OnInventorySlotCreatedEventArgs e)
    {
        e.newActiveOrInactiveInventorySlot.onInventoryPlacedItem += DisableSelectionToInventoryDrop;
        e.newActiveOrInactiveInventorySlot.onInventoryRemovedItem += EnableSelectionFromInventoryDrop;
    }


    public void EnableSelection(object sender, GameItems.OnGameItemClickedEventArgs e)
    {

        GameItems gameItemToSelect = (GameItems)sender;

        {
            Debug.Log("trying to select");
            if (selectedItem == null || gameItemToSelect.initialGameSlot != selectedItem.initialGameSlot)
            {
                selectedItem = gameItemToSelect;
                selectionSquare.transform.SetParent(gameItemToSelect.initialGameSlot.transform);
                selectionSquare.transform.position = gameItemToSelect.initialGameSlot.transform.position;
                selectionSquare.SetActive(true);

                OnGameItemSelected?.Invoke(gameItemToSelect, new OnGameItemSelectedEventArgs { itemType = gameItemToSelect.itemType, goldValue = gameItemToSelect.goldValue });

            }
        }
    }

    private void DisableSelectionForDestroyedItem(object sender, GameItems.OnItemDestroyedEventArgs e )
    {
        if(selectedItem == e.gameItems)
        {
            StopListeningGameItem(selectedItem);
            DisableItemSelector(selectedItem);
        } 
    }

    private void ResumeSelectionOnItemRelease(PointerEventData pointerEventData, bool canEnd)
    {
        selectionSquare.transform.SetParent(selectedItem.initialGameSlot.transform);
        selectionSquare.transform.position = selectedItem.initialGameSlot.transform.position;
        selectionSquare.SetActive(true);

    }

    private void PauseSelectionOnItemMove(PointerEventData pointerEventData)
    {
        selectionSquare.SetActive(false);
    }

    void DisableSelectionToInventoryDrop(object sender, InventorySlots.onInventoryItemModificationEventArgs e)
    {
        DisableItemSelector(e.gameItem);
    }

    void EnableSelectionFromInventoryDrop(object sender, InventorySlots.onInventoryItemModificationEventArgs e)
    {
        EnableItemSelector(e.gameItem);
    }

    void SelectNextMergedItem(object sender, GameItems.OnMergedEventArgs e)
    {
        EnableItemSelector(e.mergedItem);
    }

    void EnableItemSelector(GameItems gameItemIN)
    {
        selectedItem = gameItemIN;
        selectionSquare.transform.SetParent(gameItemIN.initialGameSlot.transform);
        selectionSquare.transform.position = gameItemIN.initialGameSlot.transform.position;
        selectionSquare.SetActive(true);

        OnGameItemSelected?.Invoke(gameItemIN, new OnGameItemSelectedEventArgs { itemType = gameItemIN.itemType, goldValue = gameItemIN.goldValue });
    }

    void DisableItemSelector(GameItems selectedItemIN)
    {      
        selectedItem = null;
        selectionSquare.SetActive(false);

        OnGameItemDeSelected?.Invoke(this, null);
    }
}
