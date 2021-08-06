using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualEffectsGather : MonoBehaviour
{
    private ParticleSystem particles;
    private ParticleSystem.Particle[] allParticles ;
    private int allParticlesCount = 0 ;
    [SerializeField]private Item.ItemType itemType;
    private Item.ItemGenre itemGenre;
    private Sprite sprite;

    private float lerpDuration = .25f;
    private GameObject levelBar;
    private GameObject goldBar;
    private Transform target;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        particles.Stop();

        allParticles = new ParticleSystem.Particle[particles.main.maxParticles];
        levelBar = GameObject.Find("Level_Icon");
        goldBar = GameObject.Find("Gold_Icon");
        sprite = ItemAssets.Instance.GetAssetSprite(itemType);

    }

    private void OnEnable()
    {
        MasterEventListener.Instance.OnItemCollectted += SetTargetAndPlayCollect;
        QuestManager.Instance.OnQuestCompleted += SetTargetAndPlayQuest;
    }

    private void OnDisable()
    {
        MasterEventListener.Instance.OnItemCollectted -= SetTargetAndPlayCollect;
        QuestManager.Instance.OnQuestCompleted -= SetTargetAndPlayQuest;
    }

    void SetItemGenre(Item.ItemType itemTypeIN)
    {
        foreach (var item in collection)
        {

        }
    }

    private void LateUpdate()
    {

        if (target && particles && particles.isPlaying)
        {
            allParticlesCount = particles.GetParticles(allParticles);

            for (int i = 0; i < allParticlesCount; i++)
            {
                if(allParticles[i].remainingLifetime <= allParticles[i].startLifetime / 1.2f)
                {

                    allParticles[i].position = Vector2.Lerp(allParticles[i].position, target.position, lerpDuration);
                }
            }
            
            particles.SetParticles(allParticles, allParticlesCount);
 
        }
    }

    private void SetTargetAndPlayCollect(object sender, GameItems.OnItemCollectedEventArgs e)
    {
        Debug.Log("item explode event listened" + sender);
        GameItems gameItem = (GameItems)sender;
       if(gameItem.itemGenre == Item.ItemGenre.)

        //if (e.ItemType != itemType) return;

        particles.textureSheetAnimation.SetSprite(0, sprite);
       
        if (e.itemPanelID != default(int) && e.itemPanelID == 1)
        {
            target = levelBar.transform;
        }
        else if (e.itemPanelID != default(int) && e.itemPanelID == 2) 
        {
            target = goldBar.transform;
        }
        //particles.transform.SetParent(target.transform.parent);
        particles.transform.position = e.position;
   
        StartCoroutine(ExplodeEnum(GetParticleAmount(e.itemLevel)));
        
        //particles.Play();
        //MoveParticles();
    }

    private void SetTargetAndPlayQuest(object sender, QuestManager.OnQuestAddRemoveEventArgs e)
    {
        if(itemType == Item.ItemType.Star_1)
        {
            if (e.quest.questXPReward != default(int))
            {
                particles.textureSheetAnimation.SetSprite(0, sprite);
                target = levelBar.transform;
                particles.transform.position = e.button_CompleteQuest.transform.position;

                StartCoroutine(ExplodeEnum(e.quest.questXPReward));
            }
        }
        else if (itemType == Item.ItemType.Gold_1)
        {
            if (e.quest.questGoldReward != default(int))
            {
                particles.textureSheetAnimation.SetSprite(0, sprite);
                target = goldBar.transform;
                particles.transform.position = e.button_CompleteQuest.transform.position;

                StartCoroutine(ExplodeEnum(e.quest.questGoldReward));
            }
        }
        
    }

    //private void GroupSetTargetAndPlay(object sender, QuestManager.OnQuestAddRemoveEventArgs e)
    //{
    //    List<Sprite> questXPandGoldRewards = new List<Sprite>();
    //    if (e.quest.questXPReward != default(int))
    //    {
    //        Sprite starSprite = ItemAssets.Instance.GetAssetSprite(Item.ItemType.Star_1);
    //        for (int i = 0; i < e.quest.questXPReward; i++)
    //        {
    //            questXPandGoldRewards.Add(starSprite);
    //        }
    //        //particles.textureSheetAnimation.SetSprite(0, ItemAssets.Instance.GetAssetSprite(Item.ItemType.Star_1));
    //        //target = levelBar.transform;
    //        //particles.transform.position = e.button_CompleteQuest.transform.position;

    //        //StartCoroutine(ExplodeEnum(e.quest.questXPReward));
    //    }

    //    if (e.quest.questGoldReward != default(int))
    //    {
    //        Sprite goldSprite = ItemAssets.Instance.GetAssetSprite(Item.ItemType.Gold_1);
    //        for (int i = 0; i < e.quest.questXPReward; i++)
    //        {
    //            questXPandGoldRewards.Add(goldSprite);
    //        }

    //        //particles.textureSheetAnimation.SetSprite(0, ItemAssets.Instance.GetAssetSprite(Item.ItemType.Gold_1));
    //        //target = goldBar.transform;
    //        particles.transform.position = e.button_CompleteQuest.transform.position;

    //        StartCoroutine(GroupExplodeEnum(questXPandGoldRewards));
    //    }

    //    //List<int> questXPandGoldRewards = new List<int>();
    //    //if (e.quest.questXPReward != default(int)) questXPandGoldRewards.Add(e.quest.questXPReward);
    //    //if (e.quest.questGoldReward != default(int)) questXPandGoldRewards.Add(e.quest.questGoldReward);
    //}


    IEnumerator ExplodeEnum(int particleAmount)
    {
        //if (particleAmount > 30) particleAmount = 30;
        Debug.Log("explode");
        for (int i = 0; i < particleAmount; i++)
        {
            particles.Emit(1);
            yield return new WaitForSeconds(.03f);
        }
        //particles.Emit(particleAmount);

        //yield return new WaitForSeconds(.5f);
    }

    //IEnumerator GroupExplodeEnum(List<Sprite> questXPandGoldRewardsIN)
    //{
    //    foreach (Sprite sprite in questXPandGoldRewardsIN)
    //    {
    //        if (sprite == ItemAssets.Instance.GetAssetSprite(Item.ItemType.Star_1)) particles.randomSeed = 1;
    //        if (sprite == ItemAssets.Instance.GetAssetSprite(Item.ItemType.Gold_1)) particles.randomSeed = 2;
    //        particles.textureSheetAnimation.SetSprite(0, sprite);
    //        particles.Emit(1);

    //        yield return new WaitForSeconds(.005f);
    //    }
    //}

    private int GetParticleAmount(int itemLevelIN)
    {
        switch (itemLevelIN)
        {
            case 1: return 3;
            case 2: return 6;
            case 3: return 9;
            case 4: return 12;
            case 5: return 15;
            default: return 0;
        }
    }

    //IEnumerator MoveParticlesEnum(ParticleSystem.Particle[] allParticlesIN)
    //{
    //    float t = 0f;
    //    while (t < 1f)
    //    {
    //        allParticlesCount = particles.GetParticles(allParticlesIN);
    //        for (int i = 0; i < allParticlesCount; i++)
    //        {
    //            allParticlesIN[i].position = Vector3.Lerp(allParticlesIN[i].position, target.position, t);
    //        }
    //        t += 0.01f;
    //        particles.SetParticles(allParticlesIN, allParticlesCount);

    //        yield return null;
    //    }
    //}


    //void MoveParticles()
    //{
        
        

    //    for (int i = 0; i < allParticlesCount; i++)
    //    {
    //        StartCoroutine(MoveParticlesEnum(allParticles[i]));
    //    }
    //}

    //IEnumerator MoveParticlesEnum(ParticleSystem.Particle particleIN)
    //{
    //    while (allParticlesCount == 0)
    //    {
    //        continue;
    //    }

    //    Vector2 originalPosition = particleIN.position;
    //    float elapsedTime = 0;

    //    while (particleIN.position != target.position)
    //    {
    //        particleIN.position = Vector2.Lerp(originalPosition, target.position, elapsedTime / lerpDuration);
    //        elapsedTime += Time.deltaTime;

    //        yield return null;
    //    }
    //    particleIN.position = target.position;
    //    particles.SetParticles(allParticles, allParticlesCount);
    //}

}
