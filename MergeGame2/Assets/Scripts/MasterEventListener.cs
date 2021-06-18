using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MasterEventListener : MonoBehaviour
{
    private static MasterEventListener _instance;

    public static MasterEventListener Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    private GameObject[] gameSlots;
        
    public event EventHandler<GameItems.OnMergedEventArgs> OnMerged;

    #region
    //private VisualEffects visualEffects;
    //private ScoreManager scoreManager;
    #endregion
    void Awake()
    {
    
        if (_instance != null & _instance != this)
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

      gameSlots = GameObject.FindGameObjectsWithTag("Container");

      #region
        //visualEffects = GameObject.FindGameObjectWithTag("ParticleSystem").GetComponent<VisualEffects>() ;
        //scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
        //Debug.Log("static working");
        #endregion
    }
    void OnEnable()
    {
       for (int i = 0; i < gameSlots.Length; i++)
       {
          gameSlots[i].GetComponent<GameSlots>().OnDropped += OnGameItemAdded;
       }
    }

    void OnDisable()
    {
       if (gameSlots != null)
       {
          for (int i = 0; i < gameSlots.Length; i++)
          {
             gameSlots[i].GetComponent<GameSlots>().OnDropped -= OnGameItemAdded;
          }
       }
    }
      
    void OnGameItemAdded(object sender, GameSlots.OnDroppedEventHandler e)
    {
        e.gameItem.OnMerged += MergeEvent;
    }

    void MergeEvent(object sender, GameItems.OnMergedEventArgs e)
    {
       OnMerged?.Invoke(sender, e);
    }
    
}
