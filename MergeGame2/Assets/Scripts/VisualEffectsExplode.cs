using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffectsExplode : MonoBehaviour
{
    private bool isActive = true;
    private float coolDownTimer;
    private ParticleSystem particles;


    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
        particles.Stop();
    }



    public void DoEmit(Vector3 explodePosition)
    {
        Debug.Log("explodeeffect");
        particles.transform.position = explodePosition;
        particles.Emit(35);
    }
}
