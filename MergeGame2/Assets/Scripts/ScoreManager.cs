using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    #region
    //public static ScoreManager instance;
    //public GameObject[] gameSlots;
    //private MasterEventListener masterEventListener;
    #endregion

    public GameObject player;
    private  TMP_Text scoreText;
    private int score;

    public event EventHandler<OnScoreUpdateEventHandler> OnScoreUpdate;
    public class OnScoreUpdateEventHandler : EventArgs
    {
        public int score;
    }

    void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        player = GameObject.FindGameObjectWithTag("Player");

        #region
        //masterEventListener = GameObject.FindGameObjectWithTag("Player").GetComponent<MasterEventListener>();
        #endregion
    }

    void OnEnable()
    {
        #region
        //gameSlots = GameObject.FindGameObjectsWithTag("Container");
        //for (int i = 0; i < gameSlots.Length; i++)
        //{
        //    gameSlots[i].GetComponent<GameSlots>().OnDropped += OnGameItemAdded;
        //}
        #endregion

        Init();
        MasterEventListener.Instance.OnMerged += UpdateText;
    }

     void OnDisable()
    {
        #region
        //gameSlots = GameObject.FindGameObjectsWithTag("Container");
        //for (int i = 0; i < gameSlots.Length; i++)
        //{
        //    gameSlots[i].GetComponent<GameSlots>().OnDropped -= OnGameItemAdded;
        //}
        #endregion

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

     void Start()
    {
        score = 0;
        scoreText.text = "SCORE : " + score;
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
