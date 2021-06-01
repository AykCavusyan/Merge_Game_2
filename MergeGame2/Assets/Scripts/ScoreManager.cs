using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    //public static ScoreManager instance;
    public GameObject[] gameSlots;


    public TMP_Text scoreText;
    private int score;

    //
    private int maxScoreForCurrentLevel;

    private void OnEnable()
    {
        gameSlots = GameObject.FindGameObjectsWithTag("Container");
        for (int i = 0; i < gameSlots.Length; i++)
        {
            gameSlots[i].GetComponent<GameSlots>().OnDropped += OnGameItemAdded;
        }
    }

    private void OnDisable()
    {
        gameSlots = GameObject.FindGameObjectsWithTag("Container");
        for (int i = 0; i < gameSlots.Length; i++)
        {
            gameSlots[i].GetComponent<GameSlots>().OnDropped -= OnGameItemAdded;
        }
    }

    // Start is called before the first frame update
    void Awake ()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        score = 0;
        scoreText.text = "SCORE : " + score;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGameItemAdded(object sender, GameSlots.OnDroppedEventHandler e)
    {
        e.gameItem.OnMerged += UpdateText;
    }

    private void UpdateText(object sender, GameItems.OnMergedEventArgs e)
    {
        score++;
        scoreText.text = "SCORE : "  + score;
            ;
    }


}
