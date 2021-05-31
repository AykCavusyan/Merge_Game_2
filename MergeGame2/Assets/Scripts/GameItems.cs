using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameItems : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler,IEndDragHandler, IDragHandler
{

    public event Action<PointerEventData> OnBeginDragHandler;
    public event Action<PointerEventData> OnDragHandler;
    public event Action<PointerEventData, bool> OnEndDragHandler;

    public event EventHandler<OnMergedEventArgs> OnMerged;
    public class OnMergedEventArgs : EventArgs
    {
        public Vector3 mergePos;
        public Sprite sprite;
    }


    public List<MergeConditions> mergeCondisitons = new List<MergeConditions>();

    public bool followCursor { get; set; } = true;
    public Vector3 startPosition;
    public bool canDrag { get; set; } = true;
    private GameSlots initialGameSlot;

    private ItemBag itemBag;

    public GameItems gameItemExisting;
    public GameItems gameItemDragged;

    private RectTransform rectTransform;


     private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gameItemDragged = this;
        itemBag = GameObject.Find("Button").GetComponent<ItemBag>();
    }

    private void Start()
    {
        rectTransform.localScale = new Vector3(1, 1, 1);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("key pressed");
            
        }
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag)
        {
            return;
        }

        // Puts each dragged item as last sibling on canvas order
        rectTransform.SetAsLastSibling();

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        initialGameSlot = null;
        
        foreach (var result in results)
        {
            initialGameSlot = result.gameObject.GetComponent<GameSlots>();
            
        }

        OnBeginDragHandler?.Invoke(eventData);

        //Debug.Log(eventData);
        //canvasGroup.blocksRaycasts = false;
        //canvasGroup.alpha = .6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag)
        {
            return;
        }

        OnDragHandler?.Invoke(eventData);

        if (followCursor)
        {
            rectTransform.anchoredPosition += eventData.delta;
        }
        
        //this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag)
        {
            return;
        }

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        Debug.Log(results);


        GameSlots gameSlot = null;
        gameItemExisting = null;

        foreach (var result in results)
        {
            gameSlot = result.gameObject.GetComponent<GameSlots>();

            if (gameSlot != null)
            {
                break;
            }
        }

        if (gameSlot != null)
        {
            if (gameSlot.Accepts(this) && gameSlot.canDrop == true)
            {
                    gameSlot.Drop(this);
                    initialGameSlot.canDrop = true;
                    OnEndDragHandler?.Invoke(eventData, true);

                return;
            }

            else if (gameSlot.Accepts(this) && gameSlot.canDrop == false)
            {
                var results2 = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results2);
                Debug.Log(results2);

                gameItemExisting = null;

                foreach (var result in results2)
                {
                    
                    if( result.gameObject.GetInstanceID() == this.gameObject.GetInstanceID())
                    {
                        continue;
                    }
                    gameItemExisting = result.gameObject.GetComponent<GameItems>();

                    if (gameItemExisting != null)
                    {
                        break;
                    }
                }
                if (gameItemExisting != null && AcceptMerge(gameItemDragged, gameItemExisting))
                {


                    GameItems mergedItem = Merge(gameItemDragged, gameItemExisting);

                    gameSlot.Drop(mergedItem);
                    initialGameSlot.canDrop = true;
                    OnEndDragHandler?.Invoke(eventData, true);

                    // sprite kısımı sadeleşebilir !!!
                    OnMerged?.Invoke(this, new OnMergedEventArgs { mergePos = gameItemExisting.transform.position, sprite=gameItemExisting.GetComponent<Image>().sprite });

                    return;
                }
                
            }
        }

        Debug.Log("The Item SLot Container is already full or invalid position");
        rectTransform.anchoredPosition = startPosition;
        OnEndDragHandler?.Invoke(eventData, false);

        //Debug.Log(eventData);
        //canvasGroup.blocksRaycasts = true;
        //canvasGroup.alpha = 1f;
    }

    public bool AcceptMerge(GameItems gameItemDragged, GameItems gameItemExisting)
    {

        if (gameItemDragged.gameObject.tag == gameItemExisting.gameObject.tag)
        {
            return true;
            
        }
        else
        {
            return false;
        }
        
    }

    public GameItems Merge(GameItems aaa , GameItems bbb)
    {
        GameItems mergedItem = itemBag.GenerateItem(gameItemDragged.gameObject, gameItemExisting.gameObject).GetComponent<GameItems>();
        Destroy(gameItemDragged.gameObject);
        Destroy(gameItemExisting.gameObject);

        return mergedItem;
    }
}
