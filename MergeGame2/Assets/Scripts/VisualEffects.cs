using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffects : MonoBehaviour
{
    private ParticleSystem particles;
    public GameObject[] gameSlots;
    
    
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

    public void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        particles.Stop();
      

        
      
    }



    // Start is called before the first frame update
    public void Start()
    {
       
    }

    private void OnGameItemAdded(object sender, GameSlots.OnDroppedEventHandler e)
    {
        e.gameItem.OnMerged += MergeAnimation;
       
    }

    



    // Update is called once per frame
    private void MergeAnimation(object sender, GameItems.OnMergedEventArgs e)
    {
        

        particles.gameObject.GetComponent<RectTransform>().position = e.mergePos;
        particles.textureSheetAnimation.SetSprite(0, e.sprite);
        particles.Play();
       


    }

   
}
