using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameItems : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler,IEndDragHandler, IDragHandler, IPointerDownHandler,IPointerUpHandler
{

    public event Action<PointerEventData> OnBeginDragHandler;
    public event Action<PointerEventData> OnDragHandler;
    public event Action<PointerEventData, bool> OnEndDragHandler;

    public event EventHandler<OnMergedEventArgs> OnMerged;
    public class OnMergedEventArgs : EventArgs
    {
        public Vector3 mergePos;
        public Sprite sprite;
        public int itemLevel;
    }

    private Canvas canvas;

    public List<MergeConditions> mergeCondisitons = new List<MergeConditions>();

    public bool followCursor { get; set; } = true;
    public Vector3 startPosition;
    public bool canDrag { get; set; } = true;
    public GameSlots initialGameSlot;

    //private ItemBag itemBag;

    //false olacak ve startta tipe göre true olacak
    private bool isSpawner = true;
    private bool canSpawn = false;
    public bool isMoving = false;
    public GameObject player;

    //public GameItems gameItemExisting;
    //public GameItems gameItemDragged;

    private RectTransform rectTransform;

    public int itemLevel;
    public Item.ItemGenre itemGenre;
    public Item.ItemType itemType;

     private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        //itemBag = GameObject.Find("Button").GetComponent<ItemBag>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    private void OnEnable()
    {
      Init();
    }

    private void Start()
    {
        rectTransform.localScale = new Vector3(1, 1, 1);
    }

    IEnumerator DownsizeItemOnClick(Vector2 oldSize)
    {
        Vector2 scaleFactor = new Vector2(.8f, .8f);

        float lerpDuration = .2f;
        float elapsedTime = 0f;

        while(elapsedTime < lerpDuration)
        {
            rectTransform.sizeDelta = Vector2.Lerp(oldSize, oldSize * scaleFactor, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.sizeDelta = oldSize * scaleFactor;
        StartCoroutine(UpsizeItem(oldSize, lerpDuration));
    }

    IEnumerator UpsizeItem(Vector2 oldSize, float lerpDuration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < lerpDuration)
        {
            rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, oldSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rectTransform.sizeDelta = oldSize;

    }


    void Init()
    {
        if (ItemBag.Instance == null)
        {
            Instantiate(player);
        }
    }


    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
        

        Vector2 oldsize = rectTransform.sizeDelta;
        StartCoroutine(DownsizeItemOnClick(oldsize));
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag)
        {
            return;
        }

        rectTransform.SetAsLastSibling();
        isMoving = true;
        initialGameSlot.DischargeSlot();
        OnBeginDragHandler?.Invoke(eventData);

        #region
        //var results = new List<RaycastResult>();
        //EventSystem.current.RaycastAll(eventData , results);

        //initialGameSlot = null;

        //foreach (var result in results)
        //{
        //    initialGameSlot = result.gameObject.GetComponent<GameSlots>();        
        //}


        //Debug.Log(eventData);
        //canvasGroup.blocksRaycasts = false;
        //canvasGroup.alpha = .6f;
        #endregion
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
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag)
        {
            return;
        }

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        GameSlots gameSlot = null;
        //gameItemExisting = null;

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
                
                //initialGameSlot.DischargeSlot();
                gameSlot.Drop(this);

                //initialGameSlot.canDrop = true;

                OnEndDragHandler?.Invoke(eventData, true);

                return;
            }

            else if (gameSlot.Accepts(this) && gameSlot.canDrop == false)
            {

                if (gameSlot.containedItem != null && AcceptMerge(this, gameSlot))
                {
                    Vector3 mergePosition = gameSlot.containedItem.transform.position;
                    GameItems mergedItem = Merge(this, gameSlot, this.itemLevel);
                    gameSlot.Drop(mergedItem, mergePosition);
                    
                    //initialGameSlot.DischargeSlot();

                    OnEndDragHandler?.Invoke(eventData, true);

                    // sprite kýsýmý sadeleþebilir !!!
                    OnMerged?.Invoke(this, new OnMergedEventArgs { mergePos = gameSlot.containedItem.transform.position, sprite = gameSlot.containedItem.GetComponent<Image>().sprite, itemLevel = itemLevel });

                    return;
                }
                else
                {
                    initialGameSlot.Drop(gameSlot.containedItem.GetComponent<GameItems>());
                    gameSlot.Drop(this);
                    

                    OnEndDragHandler?.Invoke(eventData, true);


                    return;
                }
            }
        }
        Debug.Log("The Item SLot Container is already full or invalid position");
        initialGameSlot.Drop(this);

        //rectTransform.anchoredPosition = startPosition;
        
        OnEndDragHandler?.Invoke(eventData, false);
    }

    //            var results2 = new List<RaycastResult>();
    //            EventSystem.current.RaycastAll(eventData, results2);
    //            Debug.Log(results2);

    //            gameItemExisting = null;

    //            foreach (var result in results2)
    //            {
                    
    //                if( result.gameObject.GetInstanceID() == this.gameObject.GetInstanceID())
    //                {
    //                    continue;
    //                }
    //                gameItemExisting = result.gameObject.GetComponent<GameItems>();

    //                if (gameItemExisting != null)
    //                {
    //                    break;
    //                }
    //            }
    //            if (gameItemExisting != null && AcceptMerge(gameItemDragged, gameItemExisting))
    //            {


    //                GameItems mergedItem = Merge(gameItemDragged, gameItemExisting, gameItemDragged.itemLevel);

    //                gameSlot.Drop(mergedItem, gameItemExisting.transform.position);
    //                initialGameSlot.canDrop = true;
    //                OnEndDragHandler?.Invoke(eventData, true);

    //                // sprite kýsýmý sadeleþebilir !!!
    //                OnMerged?.Invoke(this, new OnMergedEventArgs { mergePos = gameItemExisting.transform.position, sprite=gameItemExisting.GetComponent<Image>().sprite, itemLevel = itemLevel });

    //                return;
    //            }
    //            else
    //            {
                   
    //                gameSlot.Drop(gameItemDragged);
    //                initialGameSlot.Drop(gameItemExisting);
                    
    //                return ;
    //            }
                
    //        }
    //    }

    //    Debug.Log("The Item SLot Container is already full or invalid position");
    //    rectTransform.anchoredPosition = startPosition;
    //    OnEndDragHandler?.Invoke(eventData, false);

    //    //Debug.Log(eventData);
    //    //canvasGroup.blocksRaycasts = true;
    //    //canvasGroup.alpha = 1f;
    //}

    public bool AcceptMerge(GameItems gameItemDragged, GameSlots gameSlot )
    {
        if (gameItemDragged.gameObject.tag == gameSlot.containedItem.tag)
        {
            return true;
        }
        else
        {
            return false;
        }        
    }

    public GameItems Merge(GameItems gameItemDragged, GameSlots gameSlot, int itemLevel)
    {
        GameItems mergedItem = ItemBag.Instance.GenerateItem( gameItemDragged.itemGenre, gameItemDragged.itemLevel +1 ).GetComponent<GameItems>();
        mergedItem.transform.localScale = default(Vector3);
        Destroy(gameItemDragged.gameObject);
        Destroy(gameSlot.containedItem.gameObject);

        return mergedItem;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine(InputListener()); 
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (canSpawn == true)
        {
            if (isSpawner == true)
            {
                ItemBag.Instance.AddGeneratedItem(itemGenre, transform.position);

                Debug.Log(transform.position + "gameitem transform position");
                Debug.Log(initialGameSlot.transform.position + "gameslot transform position");
                Debug.Log(rectTransform.anchoredPosition + "gameitem anchored position");
                Debug.Log(transform.localPosition + "gameitem localposition");
            }
        }
    }

    IEnumerator InputListener()
    {
        float validClickLimit = .35f;
        float timeElapsed = 0;

        while (timeElapsed < validClickLimit && isMoving == false)
        {
            canSpawn = true;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canSpawn = false;
    }
}
