using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ParticleReceiver : MonoBehaviour
{
    private RectTransform rt;
    private Vector2 originalSize;
    private Vector2 lerpSize;
    private float lerpDuration = .10f;

    private ParticleSystem[] particlesFromAddSlotButton;
    private ParticleSystem topPanelEffects;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        originalSize = rt.sizeDelta;
        lerpSize = new Vector2(originalSize.x * 1.2f, originalSize.y * 1.2f);
        topPanelEffects = GameObject.Find("CFX_TopPanelEffects").GetComponent<ParticleSystem>();
        topPanelEffects.Stop();
    }

    private void Start()
    {
        particlesFromAddSlotButton = new ParticleSystem[Button_AddPowerUpSlots.particleSystemAmount];

        for (int i = 0; i < particlesFromAddSlotButton.Length; i++)
        {
            particlesFromAddSlotButton[i] = Button_AddPowerUpSlots.particleSystemPool[i];
        }
    }
    public void OnParticleCollision(GameObject other)
    {       
        if (particlesFromAddSlotButton.Any(particles => particles == other.GetComponent<ParticleSystem>()))
        {
            ExecutePanelVisualEffect();
        } 
        else
        {
            ChangeSize();
        }

    }

    private void ExecutePanelVisualEffect()
    {
        topPanelEffects.transform.position = this.transform.position;
        topPanelEffects.textureSheetAnimation.SetSprite(0, GetComponent<Image>().sprite);
        topPanelEffects.Play();
    }

    void ChangeSize()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeSizeEnum());
    }

    IEnumerator ChangeSizeEnum()
    {
        float elapsedTime = 0f;
        while (elapsedTime < lerpDuration)
        {
            rt.sizeDelta = Vector2.Lerp(originalSize, lerpSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rt.sizeDelta = lerpSize;

        elapsedTime = 0f;
        while (elapsedTime < lerpDuration)
        {
            rt.sizeDelta = Vector2.Lerp(lerpSize, originalSize, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rt.sizeDelta = originalSize;
    }
}
