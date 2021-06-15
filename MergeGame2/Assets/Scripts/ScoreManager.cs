using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    //public static ScoreManager instance;
    //public GameObject[] gameSlots;
    public GameObject player;

    private  TMP_Text scoreText;
    private int score;
    //private MasterEventListener masterEventListener;
    

    public event EventHandler<OnScoreUpdateEventHandler> OnScoreUpdate;
    public class OnScoreUpdateEventHandler : EventArgs
    {
        public int score;
    }

    void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        player = GameObject.FindGameObjectWithTag("Player");
        //masterEventListener = GameObject.FindGameObjectWithTag("Player").GetComponent<MasterEventListener>();
    }

     void OnEnable()
    {
        //gameSlots = GameObject.FindGameObjectsWithTag("Container");
        //for (int i = 0; i < gameSlots.Length; i++)
        //{
        //    gameSlots[i].GetComponent<GameSlots>().OnDropped += OnGameItemAdded;
        //}

        Init();

        MasterEventListener.Instance.OnMerged += UpdateText;
    }

     void OnDisable()
    {
        //gameSlots = GameObject.FindGameObjectsWithTag("Container");
        //for (int i = 0; i < gameSlots.Length; i++)
        //{
        //    gameSlots[i].GetComponent<GameSlots>().OnDropped -= OnGameItemAdded;
        //}

        

        MasterEventListener.Instance.OnMerged -= UpdateText;

    }

    void Init()
    {
        if (MasterEventListener.Instance == null)
        {
            Debug.Log("null master event listener - instantiating");
            Instantiate(player);
        }
        else
        {
            Debug.Log("instance is already runnig");
        }
    }

    // Start is called before the first frame update
    

     void Start()
    {
        score = 0;
        scoreText.text = "SCORE : " + score;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //private void OnGameItemAdded(object sender, GameSlots.OnDroppedEventHandler e)
    //{
    //    e.gameItem.OnMerged += UpdateText;
    //}

     void UpdateText(object sender, GameItems.OnMergedEventArgs e)
    {
        int oldScore = score;
        score++;
        scoreText.text = "SCORE : "  + score;
        OnScoreUpdate?.Invoke(this, new OnScoreUpdateEventHandler { score = score - oldScore});
            
    }


}
