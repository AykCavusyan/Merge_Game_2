using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpText_Floating : MonoBehaviour
{
    private int popupTextPoolAmount = 20;
    private GameObject[] popupTextPool;

    private void Awake()
    {
        
    }

    private void Start()
    {
        InitializePopuptextPool();
        ChoosePopupTextFromPool();
    }

    private void InitializePopuptextPool()
    {
        popupTextPool = new GameObject[popupTextPoolAmount];

        for (int i = 0; i < popupTextPoolAmount; i++)
        {
            GameObject popupText = Instantiate(Resources.Load<GameObject>("Prefabs/" + "PopupText_Effect"));
            popupText.transform.SetParent(this.transform);
            popupText.transform.position = this.transform.position;

            popupTextPool[i] = popupText;
            
            popupText.gameObject.SetActive(false);       
        }
    }


    private TextMeshPro ChoosePopupTextFromPool()
    {
        for (int i = 0; i < popupTextPoolAmount; i++)
        {
            if (!popupTextPool[i].gameObject.activeSelf)
            {
                popupTextPool[i].gameObject.SetActive(true);

                return popupTextPool[i].GetComponent<TextMeshPro>();
            }
        }
        Debug.Log("no text available");
        return null;
    }

    private IEnumerator MovePopupTextEnumerator()
    {
        float elapsedTime = 0f;

        yield return null;
    }
}
