using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_AddPowerUpSlots : MonoBehaviour, IPointerDownHandler
{
    int slotCost = 10;
    Panel_PowerUpItems panelPowerUpitems;

    public static int particleSystemAmount { get; private set; } = 4;
    public static ParticleSystem[] particleSystemPool { get; private set; }
    GameObject goldIcon;
    float lerpDuration = .275f;

    private bool canClick = true;
    private GameObject UI_Effects_Panel;
    float minX;
    float maxX;
    float minY;
    float maxY;

    public static event EventHandler<MasterEventListener.OnFinancialEvent> onPowerUpSlotBought;


    private void Awake()
    {
        panelPowerUpitems = transform.parent.GetChild(this.transform.GetSiblingIndex() - 2).GetComponent<Panel_PowerUpItems>();
        goldIcon = GameObject.Find("Gold_Icon");
        UI_Effects_Panel = GameObject.Find("UI_Effects");
    }

    private void Start()
    {
        InitialParticleSystemSetup();
        SetMinMaxLerpValues();
    }
    private void SetMinMaxLerpValues()
    {
        Vector3[] worldCorners = new Vector3[4];
        UI_Effects_Panel.GetComponent<RectTransform>().GetWorldCorners(worldCorners);
        minX = worldCorners[0].x;
        minY = worldCorners[0].y;
        maxX = worldCorners[2].x;
        maxY = worldCorners[2].y;
    }


    private void InitialParticleSystemSetup()
    {
        particleSystemPool = new ParticleSystem[particleSystemAmount];

        for (int i = 0; i < particleSystemAmount; i++)
        {
            ParticleSystem particleSystem = Instantiate(Resources.Load<ParticleSystem>("Prefabs/" + "ParticleEffects/" + "GlowEdge"));
            particleSystem.transform.SetParent(UI_Effects_Panel.transform);
            particleSystem.transform.position = this.transform.position;
            particleSystem.Stop();

            particleSystemPool[i] = particleSystem;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(PlayerInfo.Instance.currentGold >= slotCost && canClick)        
        {
            panelPowerUpitems.AddExtraSlots();
            ParticleSystem particlesystem = ChooseParticleSystem();
            StartCoroutine(MoveParticleSystemEnumerator(particlesystem));
            onPowerUpSlotBought?.Invoke(this, new MasterEventListener.OnFinancialEvent { powerUpSlotCost = slotCost });
        }
        else
        {
            Debug.Log("not enough gold to buy more slots ");
            // display popup about not enough gold
        }
    }

    private ParticleSystem ChooseParticleSystem()
    {
        foreach (ParticleSystem particleSystem in particleSystemPool)
        {
            if (!particleSystem.isPlaying)
            {
                return particleSystem;
            }
        }

        Debug.Log("no particle system available");      
        StartCoroutine(ClickLock());
        return null;
    }


    private IEnumerator MoveParticleSystemEnumerator(ParticleSystem particleSystem)
    {
        if (!canClick) yield break;

        float elapsedTime = 0f;
        particleSystem.Play();
        Vector2 randomPos = new Vector2(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY));

        while (elapsedTime < lerpDuration)
        {
            Vector2 pointABposition = Vector2.Lerp(this.transform.position, randomPos, elapsedTime / lerpDuration);
            Vector2 pointBCposition = Vector2.Lerp(randomPos, goldIcon.transform.position , elapsedTime / lerpDuration); 

            particleSystem.transform.position = Vector2.Lerp(pointABposition, pointBCposition, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        particleSystem.transform.position = goldIcon.transform.position;
        goldIcon.GetComponent<ParticleReceiver>().OnParticleCollision(particleSystem.gameObject);
        particleSystem.Stop();

        particleSystem.transform.position = this.transform.position;

    }

    private IEnumerator ClickLock()
    {
        while (!particleSystemPool.Any(particlesystem => particlesystem.isPlaying == false))
        {
            canClick = false;
            yield return null;
        }

        canClick = true;
    }
}
