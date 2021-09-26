using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpText_Floating : MonoBehaviour
{
    private int popupTextPoolAmount = 20;
    private GameObject[] popupTextPool;
    private float lerpDuration = .35f;
    private Color positiveColor = new Color (0.9921569f, 0.7960785f, 0.2588235f, 0);
    private Color negativeColor = new Color(0.9921569f, 0.3167791f, 0.2588235f, 0);

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        PlayerInfo.Instance.OnPopupTextDisplay += DisplayPopupText;
    }

    private void OnDisable()
    {
        PlayerInfo.Instance.OnPopupTextDisplay -= DisplayPopupText;
    }

    private void Start()
    {
        InitializePopuptextPool();
    }

    private void InitializePopuptextPool()
    {
        popupTextPool = new GameObject[popupTextPoolAmount];

        for (int i = 0; i < popupTextPoolAmount; i++)
        {
            GameObject popupText = Instantiate(Resources.Load<GameObject>("Prefabs/" + "PopupText_Effect"));
            popupText.transform.SetParent(this.transform);
            popupText.transform.position = this.transform.position;
            popupText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); 
           

            popupTextPool[i] = popupText;
            
            popupText.gameObject.SetActive(false);       
        }
    }

    private void DisplayPopupText(object sender, MasterEventListener.OnPopupTextEventArgs e)
    {
        TextMeshProUGUI popupText = ChoosePopupTextFromPool();
        popupText.text = e.amount > 0 ? string.Concat("+", e.amount.ToString()) : e.amount.ToString();
        Debug.Log(popupText.text);
        Color popupColor = e.amount <= 0 ? negativeColor: positiveColor;
        Vector3 lerpedPos = new Vector3(e.originalPosition.x, e.originalPosition.y + (1 * e.travelDirection), e.originalPosition.z);

        StartCoroutine(MovePopupTextEnumerator(popupText, e.originalPosition, lerpedPos, popupColor));
    }



    private TextMeshProUGUI ChoosePopupTextFromPool()
    {
        for (int i = 0; i < popupTextPoolAmount; i++)
        {
            if (!popupTextPool[i].gameObject.activeSelf)
            {
                popupTextPool[i].gameObject.SetActive(true);

                return popupTextPool[i].GetComponent<TextMeshProUGUI>();
                
            }
        }
        Debug.Log("no text available");
        return null;
    }

    private IEnumerator MovePopupTextEnumerator(TextMeshProUGUI popupText, Vector3 originalPos, Vector3 lerpedPos, Color color)
    {
        float elapsedTime = 0f;
        Color lerpColor = new Color(color.r, color.g, color.b, 1);

        while (elapsedTime < lerpDuration)
        {
            Debug.Log(originalPos);
            Debug.Log(lerpedPos);
            popupText.transform.position = Vector3.Lerp(originalPos, lerpedPos, elapsedTime / lerpDuration);
            popupText.color = Color.Lerp(color, lerpColor, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        
    }
}
