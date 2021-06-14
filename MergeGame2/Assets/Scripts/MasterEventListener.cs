using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterEventListener : MonoBehaviour
{
        public static MasterEventListener Instance = null;

        private GameObject[] gameSlots;
        //private VisualEffects visualEffects;
        //private ScoreManager scoreManager;

        public event EventHandler<GameItems.OnMergedEventArgs> OnMerged;


        void Awake()
        {
            if ( Instance == null)
            {
              Instance = this;
            }
            else if ( Instance != this)
            {
              Destroy(gameObject);
            }

            gameSlots = GameObject.FindGameObjectsWithTag("Container");
            //visualEffects = GameObject.FindGameObjectWithTag("ParticleSystem").GetComponent<VisualEffects>() ;
            //scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
            Debug.Log("static working");
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
            for (int i = 0; i < gameSlots.Length; i++)
            {
                gameSlots[i].GetComponent<GameSlots>().OnDropped -= OnGameItemAdded;
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
