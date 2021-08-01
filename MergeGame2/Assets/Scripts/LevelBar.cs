using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBar : MonoBehaviour
{
    private GameObject player;
    private Image bar;
    private Text levelText;
    private GameObject levelIcon;
    private ParticleSystem particles;
    private Button_Claim button_Claim;

    private bool cr_Running = false;

    private static IEnumerator runningCoroutine = null;
    private static Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();


    private void Awake()
    {  
        player = GameObject.FindGameObjectWithTag("Player");
        bar = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        levelIcon = transform.GetChild(0).gameObject;
        levelText = levelIcon.transform.GetChild(0).GetComponent<Text>();

        button_Claim = GameObject.Find("LevelUp_Button_levelPanel_Parent").GetComponent<Button_Claim>();
        particles = transform.GetChild(2).GetComponent<ParticleSystem>();
        particles.Stop();

        EnableParticleSystem(null,null);
    }

    private void OnEnable()
    {
        Init();
        PlayerInfo.Instance.OnLevelTextChanged += UpdateText;
        PlayerInfo.Instance.OnLevelTextChanged += EnableParticleSystem;
        button_Claim.OnClaimed += DisableParticleSystem;
        PlayerInfo.Instance.OnLevelNumberChanged += UpdateBarFill;
        PlayerInfo.Instance.OnResetBar += ResetBarFill;
    }

    private void OnDisable()
    {
        PlayerInfo.Instance.OnLevelTextChanged -= UpdateText;
        PlayerInfo.Instance.OnLevelTextChanged -= EnableParticleSystem;
        button_Claim.OnClaimed -= DisableParticleSystem;
        PlayerInfo.Instance.OnLevelNumberChanged -= UpdateBarFill;
        PlayerInfo.Instance.OnResetBar -= ResetBarFill;
    }

    void Init()
    {
        if(PlayerInfo.Instance == null)
        {
            Instantiate(player); 
        }
    }

    void EnableParticleSystem(object sender, PlayerInfo.OnLevelChangedEventArgs e)
    {
        int currentLevel ;
        if (e == null) currentLevel = PlayerInfo.Instance.currentLevel;
        else currentLevel = int.Parse(e.levelText);

        Debug.Log(PlayerInfo.Instance.lastClaimedLevel);
        Debug.Log(currentLevel);

        if (currentLevel > PlayerInfo.Instance.lastClaimedLevel) particles.Play(); 
    }

    void DisableParticleSystem(int lastClaimedLevelIN)
    {
        if (PlayerInfo.Instance.currentLevel <= lastClaimedLevelIN) particles.Stop();
    }

    void Start()
    {
        int currentXP = PlayerInfo.Instance.currentXP;
        int xpToNextLevel = PlayerInfo.Instance.XPToNextLevel;

        bar.fillAmount = (float)currentXP / xpToNextLevel;
        levelText.text = PlayerInfo.Instance.currentLevel.ToString();
    }

    void UpdateText(object sender, PlayerInfo.OnLevelChangedEventArgs e)
    {
        if(runningCoroutine == null && cr_Running == false)
        {
            runningCoroutine = UpdateTextEnum(e.levelText);
            StartCoroutine(runningCoroutine);
        }
        else
        {
            coroutineQueue.Enqueue(UpdateTextEnum(e.levelText));
        }
    }

    IEnumerator UpdateTextEnum(string level)
    {
        cr_Running = true;
        levelText.text = level;
        yield return null;

        cr_Running = false;
        
        Dequeue();
    }


    public void UpdateBarFill(object sender, PlayerInfo.OnLevelChangedEventArgs e)
    {
        if (runningCoroutine == null && cr_Running ==false)
        {
            runningCoroutine = UpdateBarFillEnum(e.xpToNextLevel, e.currentXP);
            StartCoroutine(runningCoroutine);
        }
        else
        {
            Debug.Log("coroutine queued");
            coroutineQueue.Enqueue(UpdateBarFillEnum(e.xpToNextLevel, e.currentXP));
        }
    }


    IEnumerator UpdateBarFillEnum (int xpToNextLevel , int currentXP )
    {
        cr_Running = true;

        float elapsedTime = 0;

        float originalAmount = bar.fillAmount;
        float lerpAmount = (float)currentXP / xpToNextLevel;
        float lerpDuration =  lerpAmount - originalAmount;

        while (elapsedTime < lerpDuration)
       {
            bar.fillAmount = Mathf.Lerp(originalAmount, lerpAmount, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
       }
        
       bar.fillAmount = lerpAmount;

       cr_Running = false;

       Dequeue();

    }

    public void ResetBarFill(object sender, PlayerInfo.OnLevelChangedEventArgs e)
    {

        if (runningCoroutine == null && cr_Running == false)
        {
            runningCoroutine = ResetBarFillEnum();
            StartCoroutine(runningCoroutine);
        }
        else
        {
            Debug.Log("coroutine queued");
            coroutineQueue.Enqueue(ResetBarFillEnum());
        }
    }

    IEnumerator ResetBarFillEnum()
    {

        cr_Running = true;

        Debug.Log("bar fill amount reached");
        bar.fillAmount = 0f;

        yield return null;

        cr_Running = false;

        Dequeue();


    }

    void Dequeue()
    {
        runningCoroutine = null;
        if (coroutineQueue.Count > 0)
        {
            runningCoroutine = coroutineQueue.Dequeue();
            StartCoroutine(runningCoroutine);
        }
    }

}



