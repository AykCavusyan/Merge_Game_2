using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffectsCanDrop : MonoBehaviour
{
    private ParticleSystem particles;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        particles.Stop();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) particles.Stop();
        if (Input.GetKeyDown(KeyCode.R)) particles.Play();
    }


    private void OnEnable()
    {
        MasterEventListener.Instance.OnPossibleDropEffectMasterEvent += SetEffectState;
    }

    private void OnDisable()
    {
        MasterEventListener.Instance.OnPossibleDropEffectMasterEvent -= SetEffectState;
    }

    void SetEffectState(object sender, GameItems.OnPossibleDropEffectsEventArgs e)
    {
        if (e.canPlay == false && particles.isStopped == true) return; 
        else if (e.canPlay == false && particles.isPlaying == true) particles.Stop();

        else if (e.canPlay == true && particles.isPlaying == true && particles.transform.position == e.effectLocation) return;
        else if (e.canPlay == true && particles.isStopped == true)
        {
            particles.transform.position = e.effectLocation;
            particles.Play();
        }
    }
}
