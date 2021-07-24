using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public sealed class ItemBag : MonoBehaviour
{

    private static ItemBag _instance;
    public static ItemBag Instance { get { return _instance; } }

    private static readonly object _lock = new object();


    public Item item;
    public GameObject panel_Gameslots;
    public GameObject canvas;

    public event EventHandler<OnGameItemCreatedEventArgs> OnGameItemCreated;
    public class OnGameItemCreatedEventArgs
    {
        public GameItems gameItem;
    }


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
        Debug.Log("ItemBag Awoken");
    }
    
    private void OnEnable()
    {
        SceneController.Instance.OnSceneLoaded += SceneConfig;
        Debug.Log("ItemBag OnEnable Worked");
    }

    private void OnDisable()
    {
        SceneController.Instance.OnSceneLoaded -= SceneConfig;

    }

    private void SceneConfig(object sender, SceneController.OnSceneLoadedEventArgs e)
    {
        //string activeSceneName = SceneManager.GetActiveScene().name;

        if (e._sceneToLoad == SceneNames.Scene.MergeScene)
        {
            panel_Gameslots = GameObject.Find("Panel_GameSlots");
            canvas = GameObject.Find("Canvas");

            //GameObject.Find("Panel_LevelPanel").GetComponent<Rewards>().ConfigPanel();
        }
    }


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
            GameObject newGameItem = GenerateItem(Item.ItemGenre.Star);
            AddGeneratedItem(newGameItem);
        }
    }



    public GameSlots FindEmptySlotPosition()
    {
        GameSlots currentEmptyGameSlot = null;

        if(SlotsCounter.Instance.emptySlots.Count > 0)
        { 
            currentEmptyGameSlot = SlotsCounter.Instance.emptySlots[UnityEngine.Random.Range(0, SlotsCounter.Instance.emptySlots.Count)].GetComponent<GameSlots>();
        }
        return currentEmptyGameSlot;
    }


    public void GranaryAddGeneratedItem()
    {
        GameObject newGameItem = GenerateItem(Item.ItemGenre.Meals);
        AddGeneratedItem(newGameItem);
    }

    public void ArmoryAddGeneratedItem()
    {
        GameObject newGameItem = GenerateItem(Item.ItemGenre.Armor);
        AddGeneratedItem(newGameItem);
    }



    public GameObject GenerateItem(Item.ItemGenre itemGenre, int itemLevel = 1, bool isRewardPanelItem = false)
    {

        item = new Item(itemGenre, itemLevel, isRewardPanelItem);

        GameObject newGameItem = new GameObject();
        newGameItem.transform.SetParent(panel_Gameslots.transform);
        newGameItem.AddComponent<GameItems>().CreateGameItem(item.itemLevel, item.itemGenre, item.itemType, item.givesXP, item.isSpawner, item.isCollectible, item.xpValue, item.itemPanelID, item.isQuestItem, item.isRewardPanelItem);

        OnGameItemCreated?.Invoke(this, new OnGameItemCreatedEventArgs { gameItem = newGameItem.GetComponent<GameItems>() });

        Debug.Log("ITEM CREATED EVENT FIREDDDDD!!!");

        return newGameItem;
    }

    public GameObject GenerateItem(Dictionary<string, object> _dictFromItemIN)
    {

        GameObject newGameItem = new GameObject();
        newGameItem.transform.SetParent(panel_Gameslots.transform);
        newGameItem.AddComponent<GameItems>().RestoreState(_dictFromItemIN);


        OnGameItemCreated?.Invoke(this, new OnGameItemCreatedEventArgs { gameItem = newGameItem.GetComponent<GameItems>() });

        Debug.Log("ITEM CREATED EVENT FIREDDDDD!!!");

        return newGameItem;
    }


    public void AddGeneratedItem(GameObject newGameItemIN, Vector3 itemDroppedPositionIN = default(Vector3)) //Item.ItemGenre itemGenre, Vector3 itemGeneratedPosition =default(Vector3))
     {
        GameSlots currentEmptyGameSlot = FindEmptySlotPosition();
        
        if (currentEmptyGameSlot != null)
        {
            //GameObject newGameItem = GenerateItem(itemGenre);
            //newGameItem.name = "GameItem" + 1;
            currentEmptyGameSlot.Drop(newGameItemIN.GetComponent<GameItems>(), itemDroppedPositionIN); //, itemGeneratedPosition);
        }
        else
        {
            Debug.Log("There is no place left!");
        }

    }


}

     

