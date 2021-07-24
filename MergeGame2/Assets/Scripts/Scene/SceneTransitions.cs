using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitions : MonoBehaviour
{
    private static SceneTransitions _instance;
    public static SceneTransitions Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    private GameObject sceneTransitionCanvas;
    public Animator circleTransition; //private yapýlacak
    public GameObject sceneLoadingScreen;
    public Text sceneLoadingPercentage;

    private void Awake()
    {
        if (_instance != null && _instance != this)
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

        sceneTransitionCanvas = transform.GetChild(0).gameObject;
        circleTransition = sceneTransitionCanvas.transform.GetChild(0).GetComponent<Animator>();
        sceneLoadingScreen = sceneTransitionCanvas.transform.GetChild(1).gameObject;
        sceneLoadingPercentage = sceneLoadingScreen.transform.GetChild(1).GetComponent<Text>();

        SetSceneTransitionCanvas(false);
    }

   

    public void SetSceneTransitionCanvas(bool isActiveIN)
    {
        sceneTransitionCanvas.SetActive(isActiveIN);
    }


}
