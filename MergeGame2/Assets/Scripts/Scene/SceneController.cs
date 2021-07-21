using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static SceneController _instance;
    public static SceneController Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    public event EventHandler<OnSceneLoadedEventArgs> OnSceneLoaded;
    public class OnSceneLoadedEventArgs
    {
        public string _sceneName;
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

    public void Load()
    {
        string _targetSceneName = SceneNames.Scene.MergeScene.ToString();
        StartCoroutine(LoadScene(_targetSceneName));
    }

    IEnumerator LoadScene(string sceneNameEnumIN)
    {
        string _targetSceneName = sceneNameEnumIN.ToString();
        
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_targetSceneName);

        while (!asyncOperation.isDone)
        {
            Debug.Log(asyncOperation.progress);

            yield return null;
        }

        OnSceneLoaded?.Invoke(this, new OnSceneLoadedEventArgs { _sceneName = sceneNameEnumIN });
    }

  
}
