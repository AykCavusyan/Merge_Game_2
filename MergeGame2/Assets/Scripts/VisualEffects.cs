using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffects : MonoBehaviour
{
    private ParticleSystem particles;


    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void Awake()
    {
        particles = GameObject.Find("Particle System").GetComponent<ParticleSystem>();
        particles.Stop();
    }



    // Start is called before the first frame update
    public void Start()
    {
        
    }

    // Update is called once per frame
    public void MergeAnimation(Vector3 pos, Sprite sprite)
    {
        particles.gameObject.GetComponent<RectTransform>().position = pos;
        particles.textureSheetAnimation.SetSprite(0, sprite);
        particles.Play();
        
    }
}
