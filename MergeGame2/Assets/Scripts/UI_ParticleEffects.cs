using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ParticleEffects : MonoBehaviour
{
    private ParticleSystem particles;
    private Panel_BackgroundPanelHolder panelBackGroundHolder;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        particles.Stop();
        panelBackGroundHolder = transform.parent.GetComponent<Panel_BackgroundPanelHolder>();
    }

    private void OnEnable()
    {
        panelBackGroundHolder.OnenableVisibility += PlayAnimation;
        panelBackGroundHolder.OnDisableVisibility += StopAnimation;
    }

    private void OnDisable()
    {
        panelBackGroundHolder.OnenableVisibility -= PlayAnimation;
        panelBackGroundHolder.OnDisableVisibility -= StopAnimation;
    }

    void PlayAnimation(object sender, Panel_BackgroundPanelHolder.OnEnableVisibilityEventArgs e)
    {
        particles.Play();
    }

    void StopAnimation(object sender, Panel_BackgroundPanelHolder.OnDisableVisibilityEventArgs e)
    {
        particles.Stop();
    }
}
