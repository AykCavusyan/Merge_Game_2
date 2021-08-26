using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VisualEffectsCanDrop : MonoBehaviour
{
    private ParticleSystem particles;
    public EffectType effectType;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        particles.Stop();
    }


    private void OnEnable()
    {
        MasterEventListener.Instance.OnPossibleDropEffectMasterEvent += SetEffectState;
        ItemBag.Instance.OnGameItemCreated += StartListeningToGameItem;
        MasterEventListener.Instance.OnDestroyedMasterEvent += StopListeningToGameItem;
        //MasterEventListener.Instance.OnMerged += StopEffectOnMerge;
    }

    private void OnDisable()
    {
        MasterEventListener.Instance.OnPossibleDropEffectMasterEvent -= SetEffectState;
        ItemBag.Instance.OnGameItemCreated -= StartListeningToGameItem;
        MasterEventListener.Instance.OnDestroyedMasterEvent -= StopListeningToGameItem;

        GameItems[] gameItems = FindObjectsOfType<GameItems>();
        if (gameItems.Length > 0)
        {
            foreach (GameItems gameItem in gameItems)
            {
                gameItem.OnEndDragHandler -= StopEffectOnEndDrag;
            }
        }
        //MasterEventListener.Instance.OnMerged -= StopEffectOnMerge; ;
    }

    public enum EffectType
    {
        overInvetory,
        overGameItem,
        none,
    }

    void StartListeningToGameItem(object sender, ItemBag.OnGameItemCreatedEventArgs e)
    {
        e.gameItem.OnEndDragHandler += StopEffectOnEndDrag;
    }

    void StopListeningToGameItem(object sender, GameItems.OnItemDestroyedEventArgs e)
    {
        e.gameItems.OnEndDragHandler -= StopEffectOnEndDrag;
    }


    void SetEffectState(object sender, GameItems.OnPossibleDropEffectsEventArgs e)
    {
        if (e.canPlay == false && particles.isStopped == true) return;
        else if (e.canPlay == false && particles.isPlaying == true && effectType == e.effectType)
        {
            particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            effectType = EffectType.none;
        }
        else if (e.canPlay == true && particles.isPlaying == true && particles.transform.position == e.effectLocation) return;
        else if (e.canPlay == true && particles.isStopped == true)
        {
            particles.transform.position = e.effectLocation;
            particles.Play();
            effectType = e.effectType;
        }
    }

    void StopEffectOnEndDrag(PointerEventData pointerevent, bool endBool, bool isInsidePowerUpPanel)
    {
        particles.Stop(withChildren:true, stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear );
        effectType = EffectType.none;
    }
}
