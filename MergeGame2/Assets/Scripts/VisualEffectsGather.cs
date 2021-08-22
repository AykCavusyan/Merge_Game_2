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
    [SerializeField]private Item.ItemGenre itemGenre;
    private Sprite sprite;

    private float lerpSpeed = .52f;
    private GameObject levelBar;
    private GameObject goldBar;
    private Transform target;

    private Vector3 lateralOscillation;

    private VisualEffectsExplode explodeEffect;
    private List<Vector4> customData = new List<Vector4>();
    private int uniqueID;

    //private Dictionary<ParticleSystem.Particle,bool> explodeIndex = new Dictionary<ParticleSystem.Particle, bool>();


    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        particles.Stop();

        allParticles = new ParticleSystem.Particle[particles.main.maxParticles];
        levelBar = GameObject.Find("Level_Icon");
        goldBar = GameObject.Find("Gold_Icon");
        sprite = ItemAssets.Instance.GetAssetSprite(itemType);

        explodeEffect = GameObject.Find("Particle_Explode_Effect").GetComponent<VisualEffectsExplode>();

    }

    private void OnEnable()
    {
        MasterEventListener.Instance.OnItemCollectted += SetTargetAndPlayCollectibles;
        MasterEventListener.Instance.OnItemCollectted += SetTargetAndPlaySellables;
        QuestManager.Instance.OnQuestCompleted += SetTargetAndPlayQuest;
    }

    private void OnDisable()
    {
        MasterEventListener.Instance.OnItemCollectted -= SetTargetAndPlayCollectibles;
        MasterEventListener.Instance.OnItemCollectted -= SetTargetAndPlaySellables;
        QuestManager.Instance.OnQuestCompleted -= SetTargetAndPlayQuest;
    }

   

    private void LateUpdate()
    {
        if (target && particles && particles.isPlaying)
        {

            particles.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

            for (int i = 0; i < customData.Count; i++)
            {
                if(customData[i].x == 0.0f)
                {
                    customData[i] = new Vector4(++uniqueID, 1, 0, 0);
                }
            }

            particles.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

            allParticlesCount = particles.GetParticles(allParticles);
            lateralOscillation = new Vector3(Mathf.PingPong(Time.unscaledTime*5, .2f), 0,0);

            for (int i = 0; i < allParticlesCount; i++)
            {
                particles.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
                if(allParticles[i].remainingLifetime <= allParticles[i].startLifetime / Mathf.Clamp((1.2f+(i/8)),1.2f,1.65f))
                {
                    if(customData[i].y == 1)
                    {
                        explodeEffect.DoEmit(allParticles[i].position);
                        customData[i] = new Vector4(customData[i].x, 0, 0, 0);
                        particles.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
                    }

                    //Debug.Log(allParticles[i]);
                    //if (explodeIndex.ContainsKey(allParticles[i]) && explodeIndex[allParticles[i]] == true) // (explodeIndex[i] && explodeIndex[i] == true) 
                    //{
                    //    explodeEffect.DoEmit(allParticles[i].position);
                    //    explodeIndex[allParticles[i]] = false;                           //explodeIndex[i] = false;
                    //}
                    
                    allParticles[i].position = Vector2.MoveTowards(allParticles[i].position + lateralOscillation, target.position, lerpSpeed);


                }
                
            }
            
            particles.SetParticles(allParticles, allParticlesCount);
        }
    }

    private void SetTargetAndPlayCollectibles(object sender, GameItems.OnItemCollectedEventArgs e)
    {
        
        GameItems gameItem = (GameItems)sender;
        if (gameItem.itemGenre != itemGenre) return;


        particles.textureSheetAnimation.SetSprite(0, sprite);
       
        if (e.itemPanelID != default(int) && e.itemPanelID == 1)
        {
            target = levelBar.transform;
        }
        else if (e.itemPanelID != default(int) && e.itemPanelID == 2) 
        {
            target = goldBar.transform;
        }
        particles.transform.position = e.position;
   
        StartCoroutine(ExplodeEnum(GetParticleAmount(e.itemLevel)));

    }

    private void SetTargetAndPlaySellables(object sender,GameItems.OnItemCollectedEventArgs e)
    {
        GameItems gameItem = (GameItems)sender;
        if (gameItem.itemGenre == Item.ItemGenre.Star || gameItem.itemGenre == Item.ItemGenre.Gold ||itemGenre!=Item.ItemGenre.Gold) return;

        particles.textureSheetAnimation.SetSprite(0, sprite);
        target = goldBar.transform;
        particles.transform.position = e.position;

        StartCoroutine(ExplodeEnum(GetParticleAmount(e.itemLevel))); 
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
        ParticleSystem.Particle[] emittedParticleArray = new ParticleSystem.Particle[particleAmount];

        for (int i = 0; i < particleAmount; i++)
        {
            particles.Emit(1);

            //particles.GetParticles(emittedParticleArray);
            //ParticleSystem.Particle particle = emittedParticleArray[i];
            //explodeIndex.Add(particle., true);
            //if (explodeIndex.ContainsKey(i)) explodeIndex[i] = true;
            //else explodeIndex.Add(i,true);

            yield return new WaitForSeconds(.03f);
        }
        //particles.Emit(particleAmount);

        //yield return new WaitForSeconds(.5f);
    }

    //IEnumerator MoveParticlesEnum(ParticleSystem.Particle particleIN, int particleArrayLengthIN, int particleOrder)
    //{
    //    ParticleSystem.Particle[] emittedParticleArray = new ParticleSystem.Particle[particleArrayLengthIN];
    //    float elapsedTime = 0;
    //    float lerpDuration = .5f;

    //    while (particleIN.remainingLifetime > particleIN.startLifetime / 1.5)
    //    {
    //        Debug.Log(particleIN.remainingLifetime);
    //        Debug.Log(particleIN.startLifetime);
    //        particles.GetParticles(emittedParticleArray);
    //        particleIN = emittedParticleArray[particleOrder];

    //        yield return null;
    //    }

    //    while (elapsedTime < lerpDuration)
    //    {
    //        particles.GetParticles(emittedParticleArray);
    //        particleIN = emittedParticleArray[particleOrder];
    //        particleIN.position = Vector2.MoveTowards(particleIN.position, target.position, lerpSpeed);
    //        particles.SetParticles(emittedParticleArray);
    //        yield return null;

    //    }
    //}

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




    //IEnumerator CollectParticlesEnum(int i)
    //{
    //    yield return new WaitForSeconds(.5f);
    //    allParticlesCount = particles.GetParticles(allParticles);
    //    Vector3 particlePos = allParticles[i].position;
    //    float elapsedTime = 0f;

    //    while (elapsedTime < lerpDuration)
    //    {
    //        allParticlesCount = particles.GetParticles(allParticles);
    //        allParticles[i].position = Vector3.Lerp(particlePos, target.position, elapsedTime / lerpDuration);
    //        elapsedTime += Time.deltaTime;
    //        particles.SetParticles(allParticles, allParticlesCount);

    //        yield return null;
    //    }
    //    allParticles[i].position = target.position;
    //    particles.SetParticles(allParticles, allParticlesCount);
    //    onParticleCollision?.Invoke(EventArgs.Empty);
    //}

}
