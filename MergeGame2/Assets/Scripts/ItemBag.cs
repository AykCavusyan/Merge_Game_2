using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public sealed class ItemBag : MonoBehaviour
{

    private static ItemBag _instance;
    public static ItemBag Instance { get { return _instance; } }

    private static readonly object _lock = new object();


    //private GameObject player;
    public Item item;
    public GameObject panel_Gameslots;
    public GameObject canvas;

    public event EventHandler<OnGameItemCreatedEventArgs> OnGameItemCreated;
    public class OnGameItemCreatedEventArgs
    {
        public GameItems gameItem;
    }

    //public GameObject[] gameSlots;
    //public GameObject newGameItemIdentified;

    private void Awake()
    {
        if ( _instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = this;
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }

        //player = GameObject.FindGameObjectWithTag("Player");
        canvas = GameObject.Find("Canvas");
        panel_Gameslots = GameObject.Find("Panel_GameSlots");
    }
    //private void OnEnable()
    //{
    //    //Init();
    //}

    //private void Start()
    //{
        
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GranaryAddGeneratedItem();
        }

        else if (Input.GetKeyDown(KeyCode.O))
        {
            ArmoryAddGeneratedItem();
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            AddGeneratedItem(Item.ItemGenre.Star);
        }
    }

    //void Init()
    //{
    //     if (SlotsCounter.Instance == null)
    //    {
    //        Instantiate(player);
    //    }
    //}

    //public void IdentifyItem(GameObject newGameItem)
    //{
    //    newGameItemIdentified = newGameItem;
    //}

    public GameSlots FindEmptySlotPosition()
    {
        GameSlots currentEmptyGameSlot = null;

        if(SlotsCounter.Instance.emptySlots.Count > 0)
        { 
            currentEmptyGameSlot = SlotsCounter.Instance.emptySlots[UnityEngine.Random.Range(0, SlotsCounter.Instance.emptySlots.Count)].GetComponent<GameSlots>();
        }
        return currentEmptyGameSlot;
        #region
        //for (int i = UnityEngine.Random.Range(0,gameSlots.Length) ; i < gameSlots.Length; i++)
        //{
        //    Debug.Log("calling i" +i);
        //    if (gameSlots[i].GetComponent<GameSlots>().canDrop)
        //    {  
        //     return i;
        //    }

        //    if (i == gameSlots.Length -1)
        //    {
        //        for (int j = i; j >= 0; j--)
        //        {
        //            Debug.Log("calling j " + j);
        //            if (gameSlots[j].GetComponent<GameSlots>().canDrop)
        //            {
        //                return j;
        //            }

        //        }
        //    }
        //}
        //return -1;
        #endregion
    }


    public void GranaryAddGeneratedItem()
    {
        AddGeneratedItem(Item.ItemGenre.Meals);
    }

    public void ArmoryAddGeneratedItem()
    {
        AddGeneratedItem(Item.ItemGenre.Armor);
    }



    public GameObject GenerateItem(Item.ItemGenre itemGenre , int itemLevel = 1, bool isRewardPanelItem = false)
    {
        item = new Item(itemGenre, itemLevel, isRewardPanelItem);

        GameObject newGameItem = new GameObject();
        
        newGameItem.transform.SetParent(panel_Gameslots.transform);
        //newGameItem.AddComponent<Image>().sprite = item.GetSprite(item.itemType);
        newGameItem.AddComponent<GameItems>().CreateGameItem(item.itemLevel, item.itemGenre, item.itemType, item.givesXP, item.isSpawner, item.isCollectible, item.xpValue, item.itemPanelID, item.isQuestItem, item.isRewardPanelItem);
        
        if (item.givesXP == true)
        {
            AddGeneratedItem(Item.ItemGenre.Star);
        }
        
        //newGameItem.layer = 5;

        OnGameItemCreated?.Invoke(this, new OnGameItemCreatedEventArgs { gameItem = newGameItem.GetComponent<GameItems>() });

        return newGameItem;
    }



    public void AddGeneratedItem(Item.ItemGenre itemGenre, Vector3 itemGeneratedPosition =default(Vector3))
     {
        GameSlots currentEmptyGameSlot = FindEmptySlotPosition();
        if (currentEmptyGameSlot != null)
        {
            GameObject newGameItem = GenerateItem(itemGenre);
            newGameItem.name = "GameItem" + 1;
            currentEmptyGameSlot.Drop(newGameItem.GetComponent<GameItems>(), itemGeneratedPosition);

           
        }
        else
        {
            Debug.Log("There is no place left!");
        }

        #region
        //int i = FindEmptySlotPosition();

        //   if (i >= 0 && i <= gameSlots.Length)
        //   {

        //       GameObject newGameItem = GenerateItem(itemGenre);
        //       newGameItem.name = "GameItem" + i + 1;
        //       gameSlots[i].GetComponent<GameSlots>().Drop(newGameItem.GetComponent<GameItems>(), transform.position);
        //       IdentifyItem(newGameItem);


        //       //would be much better if made with an event - need to und how to pass the variable 
        //       //gameSlots[i].GetComponent<GameSlots>().canDrop = false;

        //       inventory.AddItemToList(item);
        //   }
        //   else
        //   {
        //    Debug.Log("There is no place left!");
        //   }
        #endregion
    }


}

     

