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


    private void OnEnable()
    {
        MasterEventListener.Instance.OnPossibleDropEffectMasterEvent += SetEffectState;
        MasterEventListener.Instance.OnMerged += StopEffectOnMerge;
    }

    private void OnDisable()
    {
        MasterEventListener.Instance.OnPossibleDropEffectMasterEvent -= SetEffectState;
        MasterEventListener.Instance.OnMerged -= StopEffectOnMerge; ;
    }

    void SetEffectState(object sender, GameItems.OnPossibleDropEffectsEventArgs e)
    {
        if (e.canPlay == false && particles.isStopped == true) return; 
        else if (e.canPlay == false && particles.isPlaying == true) particles.Stop(true,ParticleSystemStopBehavior.StopEmitting);

        else if (e.canPlay == true && particles.isPlaying == true && particles.transform.position == e.effectLocation) return;
        else if (e.canPlay == true && particles.isStopped == true)
        {
            particles.transform.position = e.effectLocation;
            particles.Play();
        }
    }

    void StopEffectOnMerge(object sender, GameItems.OnMergedEventArgs e)
    {
        particles.Stop(withChildren:true, stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear );
    }
}
