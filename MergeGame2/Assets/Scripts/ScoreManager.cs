using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private Text scoreText;

    // Start is called before the first frame update
    void Awake ()
    {
        scoreText = GetComponent<Text>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText()
    {
        scoreText.text = ("update");
    }


}
