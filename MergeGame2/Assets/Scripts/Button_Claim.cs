using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_Claim : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    private GameObject player;
    private GameObject parentPanel;
    private List<Item.ItemGenre> rewardList;


    private Image button;
    private Image notificationBubble;
    private Text notificationText;

    private Color buttonOriginalColor;
    private Color buttonFadedColor;

    private int oldLevel = 1; // bu saveden gelmeli
    private int newLevel;
    private int levelToClaim;
    private bool canClaim = false;

    public event Action<EventArgs> OnClaimed;


    private void Awake()
    {
        button = transform.GetChild(1).GetComponent<Image>();
        notificationBubble = transform.GetChild(3).GetComponent<Image>();
        notificationText = notificationBubble.GetComponentInChildren<Text>(true);

        notificationBubble.gameObject.SetActive(false);

        buttonOriginalColor = button.color;
        buttonFadedColor = new Color(.5f, .5f, .5f, .5f);

        player = GameObject.FindGameObjectWithTag("Player");
        parentPanel = transform.parent.gameObject;

    }

    private void OnEnable()
    {
        Init();
        PlayerInfo.Instance.OnLevelTextChanged += UpdatevelToClaim;
        

    }
    private void OnDisable()
    {
        PlayerInfo.Instance.OnLevelTextChanged -= UpdatevelToClaim;
    }


    void Init()
    {
        if (PlayerInfo.Instance == null)
        {
            Instantiate(player);
        }
    }

    private void Start()
    {
        CalculateTevelToClaim(newLevel);
    }

    void UpdatevelToClaim(object sender, PlayerInfo.OnLevelChangedEventArgs e)
    {
        newLevel = int.Parse(e.levelText);  // buna dikkat ededlim !!! // eventi direk numaraya çevirmek daha iyi olacak
        
        CalculateTevelToClaim(newLevel);
        
    }

    void CalculateTevelToClaim(int newLevelIN = 0) 
    {
        if (newLevel > oldLevel)
        {
            levelToClaim = newLevel - oldLevel;
            canClaim = true;
            SetButtonVisibility(buttonOriginalColor);
            ActivateNotificationBubble(canClaim);

            return;
        }
        canClaim = false;
        SetButtonVisibility(buttonFadedColor);
        ActivateNotificationBubble(canClaim);
    }

    void Claim()
    {
        rewardList = parentPanel.GetComponent<Rewards>().rewardsList[0];

        Debug.Log(rewardList.Count);
        Debug.Log(PlayerInfo.Instance.emptySlots.Count);

        if (levelToClaim > 0 )
        {
            if (PlayerInfo.Instance.emptySlots.Count >= rewardList.Count)
            {
                oldLevel += 1;
                CalculateTevelToClaim(newLevel);
                OnClaimed?.Invoke(EventArgs.Empty);
            }
            else
            {
                Debug.Log("no inventory slots buy more"); // and a popup will say this with button to the inventory!!
            }
        }
        
    }

    void SetButtonVisibility(Color buttonColor)
    {
        button.color = buttonColor;
    }

    void ActivateNotificationBubble(bool isVisible)
    {
        
         notificationBubble.gameObject.SetActive(isVisible);
         SetNotificationText();
        
    }

    void SetNotificationText()
    {
        notificationText.text = levelToClaim.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (canClaim == true)
        {
            Claim();
        }
    }
}
