using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSelector : MonoBehaviour
{
    public GameItems selectedItem { get; private set; }
    private GameObject selectionSquare;

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
    }

    private void OnEnable()
    {
        MasterEventListener.Instance.OnDestroyedMasterEvent += DisableSelection;
        ItemBag.Instance.OnGameItemCreated += StartListeningGameItem;
    }

    private void OnDisable()
    {
        MasterEventListener.Instance.OnDestroyedMasterEvent -= DisableSelection;
        ItemBag.Instance.OnGameItemCreated -= StartListeningGameItem;
    }

    void StartListeningGameItem(object sender, ItemBag.OnGameItemCreatedEventArgs e )
    {
        e.gameItem.OnGameItemClicked += EnableSelection;
        e.gameItem.OnBeginDragHandler += DisableSelectionOnMove; //bundan unsubscribe olmak LAZIM !!!!
        e.gameItem.OnEndDragHandler += EnableSelectionOnStopMove; //bundan unsubscribe olmak LAZIM !!!!
    }

    void StopListeningGameItem(GameItems gameItemIN)
    {
        gameItemIN.OnGameItemClicked -= EnableSelection;      
    }

    private void DisableSelection(object sender, GameItems.OnItemDestroyedEventArgs e )
    {
        if(selectedItem == e.gameItems)
        {
            StopListeningGameItem(e.gameItems);
            selectedItem = null;
            selectionSquare.SetActive(false);
            OnGameItemDeSelected?.Invoke(this, null);
        } 
    }

    private void DisableSelectionOnMove(PointerEventData pointerEventData)
    {
        selectionSquare.SetActive(false);
    }

    private void EnableSelectionOnStopMove(PointerEventData pointerEventData, bool canEnd)
    {
        selectionSquare.transform.SetParent(selectedItem.initialGameSlot.transform);
        selectionSquare.transform.position = selectedItem.initialGameSlot.transform.position;
        selectionSquare.SetActive(true);
    }

    public void EnableSelection(object sender, GameItems.OnGameItemClickedEventArgs e)
    {

        GameItems gameItemToSelect = (GameItems)sender;

        {
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
}
