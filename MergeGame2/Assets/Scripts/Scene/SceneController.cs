using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    private static SceneController _instance;
    public static SceneController Instance { get { return _instance; } }
    private static readonly object _lock = new object();


    public event EventHandler<OnSceneLoadedEventArgs> OnSceneLoaded;
    public class OnSceneLoadedEventArgs
    {
        public SceneNames.Scene _sceneToLoad;
        public int initializeOrder;
    }

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

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Load(SceneNames.Scene.MergeScene);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Load(SceneNames.Scene.TownScene);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Load(SceneNames.Scene.WorldScene);
        }
    }

    public void Load()
    {
        StartCoroutine(LoadScene(SceneNames.Scene.MergeScene));
    }

    public void Load(SceneNames.Scene sceneToLoad)
    {
        StartCoroutine(LoadScene(sceneToLoad));
    }

    IEnumerator LoadScene(SceneNames.Scene sceneToLoadIN)
    {
        Animator animator = SceneTransitions.Instance.circleTransition;

        SceneTransitions.Instance.SetSceneTransitionCanvas(true);
        
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 || animator.IsInTransition(0))
        {

            yield return null;
        }

        SceneTransitions.Instance.sceneLoadingScreen.SetActive(true);

        string _targetSceneName = sceneToLoadIN.ToString();
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_targetSceneName);

        while (!asyncOperation.isDone)
        {
            SceneTransitions.Instance.sceneLoadingPercentage.text = asyncOperation.progress.ToString("F0");

            yield return null;
        }

        int initializeOrder = 1;
        foreach (IInitializerScript initializerScript in GameObject.Find("Player").GetComponents<IInitializerScript>())
        {
            OnSceneLoaded?.Invoke(this, new OnSceneLoadedEventArgs { _sceneToLoad = sceneToLoadIN , initializeOrder =initializeOrder});
            initializeOrder++;
        }

        yield return new WaitForSeconds(1f);
        SceneTransitions.Instance.sceneLoadingScreen.SetActive(false);

        animator.SetTrigger("Finish");

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("ReposState") || animator.IsInTransition(0))
        {
            
            yield return null;
        }

        SceneTransitions.Instance.SetSceneTransitionCanvas(false) ;
    }

  
}
