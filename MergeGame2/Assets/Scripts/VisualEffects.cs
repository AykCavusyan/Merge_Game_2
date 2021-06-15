using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffects : MonoBehaviour
{
    private ParticleSystem particles;
    public GameObject player;

    //private MasterEventListener masterEventListener;

    // public GameObject[] gameSlots;


    public void Awake()
    {

        //masterEventListener = GameObject.FindGameObjectWithTag("Player").GetComponent<MasterEventListener>();
        particles = GetComponent<ParticleSystem>();
        particles.Stop();
        player = GameObject.FindGameObjectWithTag("Player");

    }


     void OnEnable()
    {
        //gameSlots = GameObject.FindGameObjectsWithTag("Container");
        //for (int i = 0; i < gameSlots.Length; i++)
        //{
        //    gameSlots[i].GetComponent<GameSlots>().OnDropped += OnGameItemAdded;
        //}

        Init();

        MasterEventListener.Instance.OnMerged += MergeAnimation;
    }

     void OnDisable()
    {
        //gameSlots = GameObject.FindGameObjectsWithTag("Container");
        //for (int i = 0; i < gameSlots.Length; i++)
        //{
        //    gameSlots[i].GetComponent<GameSlots>().OnDropped -= OnGameItemAdded;
        //}

        MasterEventListener.Instance.OnMerged -= MergeAnimation;
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
       
    }

    //private void OnGameItemAdded(object sender, GameSlots.OnDroppedEventHandler e)
    //{
    //    e.gameItem.OnMerged += MergeAnimation;
       
    //}

    



    // Update is called once per frame
     void MergeAnimation(object sender, GameItems.OnMergedEventArgs e)
    {
        

        particles.gameObject.GetComponent<RectTransform>().position = e.mergePos;
        particles.textureSheetAnimation.SetSprite(0, e.sprite);
        particles.Play();
       


    }

   
}
