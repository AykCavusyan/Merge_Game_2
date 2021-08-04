using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Rewards : MonoBehaviour
{
    private Transform inner_Panel_Container;
    private Transform visualEffects_Container;
    private GameObject player;
    private Button_Claim claimButton;
    private bool cr_Running;
    private ParticleSystem currentParticle;
    private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    public int currentLevelToClaim { get; private set; }
    private int slotIDNumber = 0;
    private List<int> levelsToClaim = new List<int>();

   
    public Dictionary<int, List<Item.ItemGenre>> rewardsDict = new Dictionary<int, List<Item.ItemGenre>>();
    private List<GameObject> currentRewardSlots = new List<GameObject>();

    private int playerLevel;

    public event Action<GameItems> OnRewardItemGiven;
    public event Action<List<int>> OnListOfRewardLevelToClaimUpdated;

    private void Awake()
    {
        inner_Panel_Container = transform.GetChild(2);
        visualEffects_Container = transform.GetChild(3);
        claimButton = transform.GetChild(4).GetComponent<Button_Claim>();
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    private void OnEnable()
    {    
        Init();
        PlayerInfo.Instance.OnLevelTextChanged += UpdateRewardsDict;
        claimButton.OnClaimed += ClaimReward;
    }

    private void OnDisable()
    {
        PlayerInfo.Instance.OnLevelTextChanged -= UpdateRewardsDict;
        claimButton.OnClaimed -= ClaimReward;

    }


    public void ConfigPanel(List<int> levelsToClaimIN, int playerLevelIN)
    {
        playerLevel = playerLevelIN; 
        levelsToClaim = levelsToClaimIN;
        if(levelsToClaimIN.Count == 0) levelsToClaim.Add(1);
        GenerateRewardsDict();
        InstantiateSlots();
    }

    void Init()
    {
        if (PlayerInfo.Instance == null)
        {
            Instantiate(player);
        }
    }


    void GenerateRewardsDict()
    {
        foreach (int level in levelsToClaim)
        {
            var newLevelRewardsList = new Rewards_LevelUp(level);
            rewardsDict.Add(level, newLevelRewardsList.rewardList);
        }
    }

    void UpdateLevelsToClaimList(int levelToClaimIN)
    {
        if (!levelsToClaim.Contains(levelToClaimIN)) levelsToClaim.Add(levelToClaimIN);
        else if (levelsToClaim.Contains(levelToClaimIN)) levelsToClaim.Remove(levelToClaimIN);

        OnListOfRewardLevelToClaimUpdated?.Invoke(levelsToClaim);
    }

    void UpdateRewardsDict(object sender, PlayerInfo.OnLevelChangedEventArgs e)
    {
        playerLevel = PlayerInfo.Instance.currentLevel;
        var newLevelRewardsList = new Rewards_LevelUp(playerLevel);
        rewardsDict.Add(playerLevel, newLevelRewardsList.rewardList);

        UpdateLevelsToClaimList(playerLevel);
        //rewardsList.Add(newLevelRewardsList.rewardList);
    }

    void InstantiateSlots()
    {
        slotIDNumber = 0;
        currentLevelToClaim = rewardsDict.Keys.Min();
        Debug.Log(currentLevelToClaim);
       // for (int i = 0; i < rewardsList[0].Count; i++)
        for (int i = 0; i < rewardsDict[currentLevelToClaim].Count; i++)
        {
            slotIDNumber++;
            currentRewardSlots.Add(CreateNewSLot(slotIDNumber,rewardsDict[currentLevelToClaim] ,i));
        }
        DisableInactiveParticles(rewardsDict[currentLevelToClaim]);
        DisableChildrenImages();
    }


    private GameObject CreateNewSLot(int slotIDNumberIN,List<Item.ItemGenre> list ,int listIndex)
    {
        GameObject currentNewSlot = Instantiate(Resources.Load<GameObject>("Prefabs/" + "SlotRewards"));
       
        currentNewSlot.transform.SetParent(inner_Panel_Container, false);
        currentNewSlot.AddComponent<RewardSlots>().slotIDNumber = slotIDNumberIN;

        if (particleSystems.Count >= listIndex + 1 && particleSystems[listIndex] != null)
        {
            currentParticle = particleSystems[listIndex];
        }
        else 
        {
            currentParticle = Instantiate(Resources.Load<ParticleSystem>("Prefabs/" + "ParticleEffects/" + "CFX4 Hit Paint C (Cyan)"));
            particleSystems.Add(currentParticle);
        }

        currentParticle.transform.SetParent(visualEffects_Container, false);
        currentParticle.GetComponent<RectTransform>().localScale = (inner_Panel_Container.GetComponent<GridLayoutGroup>().cellSize)/1.7f;

        currentNewSlot.GetComponent<RewardSlots>().SetupParticleSystem(currentParticle);

        GameObject itemToAdd = ItemBag.Instance.GenerateItem(list[listIndex], currentLevelToClaim, true);
        currentNewSlot.GetComponent<RewardSlots>().Drop(itemToAdd.GetComponent<GameItems>());

        
        return currentNewSlot;
    }
    void DisableInactiveParticles(List<Item.ItemGenre> listIN)
    {
        if (particleSystems.Count > listIN.Count)
        {
            int difference = particleSystems.Count - listIN.Count;
            for (int i = 0; i < difference; i++)
            {
                Destroy(particleSystems[listIN.Count + i].gameObject);
                particleSystems.RemoveAt(listIN.Count + i); // bunu da ha sonra active deacactivate ile yaparýz BELKÝ
            }
        }
    }


    void DisableChildrenImages()
    {
        var childrenToDisable = inner_Panel_Container.GetComponentsInChildren<Image>();
        foreach (var childToDisable in childrenToDisable)
        {
            childToDisable.enabled = false;
        }
    }

    void ClaimReward(int i)
    {
        StartCoroutine(ClaimRewardProcess());
    }

    IEnumerator ClaimRewardProcess()
    {
        TransferClaimedItems();
        StartCoroutine(DestroySlots());
        while (cr_Running == true)
        {
            yield return null;
        }
        RemoveClaimedListFromDict();
        InstantiateSlots();
    }

    void TransferClaimedItems()
    {
        for (int i = 0; i < currentRewardSlots.Count; i++)
        {
            GameItems itemtoTransfer = currentRewardSlots[i].GetComponent<RewardSlots>().containedItem;
            itemtoTransfer.CheckIfRewardItemIsQuestItem();

            OnRewardItemGiven?.Invoke(itemtoTransfer);

            PlayerInfo.Instance.emptySlots.First().Drop (itemtoTransfer);
        }
    }

    IEnumerator DestroySlots()
    {
        cr_Running = true;

        for (int i = 0; i < currentRewardSlots.Count; i++)
        {
            currentRewardSlots[i].GetComponent<RewardSlots>().DeplaceSlots(null,new Panel_Invetory.OnPanelStateChangeEventArgs { isAnimatable = true });
        }
        while (currentRewardSlots[currentRewardSlots.Count - 1].GetComponent<RewardSlots>().cr_Runnning == true)
        {
            yield return null;
        }
        for (int i = 0; i < currentRewardSlots.Count; i++)
        {
            Destroy(currentRewardSlots[i]);
        }

        currentRewardSlots.Clear();

        cr_Running = false;

    }

    void RemoveClaimedListFromDict()
    {
        rewardsDict.Remove(currentLevelToClaim);

        UpdateLevelsToClaimList(currentLevelToClaim);
    }

    //public object CaptureState()
    //{
    //    List<int> _levelsToClaim = levelsToClaim;

    //    return _levelsToClaim;
    //}

    //public void RestoreState (object state)
    //{
    //    levelsToClaim = (List<int>)state;
    //}

}
