using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffects : MonoBehaviour
{
    private ParticleSystem particles;
    public GameObject player;

    #region
    //private MasterEventListener masterEventListener;
    // public GameObject[] gameSlots;
    #endregion

    public void Awake()
    {
        #region
        //masterEventListener = GameObject.FindGameObjectWithTag("Player").GetComponent<MasterEventListener>();
        #endregion

        particles = GetComponent<ParticleSystem>();
        particles.Stop();
        player = GameObject.FindGameObjectWithTag("Player");
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
        MasterEventListener.Instance.OnMerged += MergeAnimation;
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

        MasterEventListener.Instance.OnMerged -= MergeAnimation;
    }

    void Init()
    {
        if (MasterEventListener.Instance == null)
        {
            Instantiate(player);
        }
        else
        {
        }
    }

     void MergeAnimation(object sender, GameItems.OnMergedEventArgs e)
    {
        
        particles.gameObject.GetComponent<RectTransform>().position = e.mergePos;
        particles.textureSheetAnimation.SetSprite(0, e.sprite);
        particles.Play();
    }

   
    public void UpperButtonAnimation(Vector3 positionIN, Sprite spriteIN)
    {
        particles.transform.position = positionIN;
        particles.textureSheetAnimation.SetSprite(0, spriteIN);
        particles.Play();
    }
}
