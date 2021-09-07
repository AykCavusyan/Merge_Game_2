using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumeratorMovement : MonoBehaviour
{
    //private RectTransform previousAmountRect;
    //private RectTransform currentAmountRect;
    //private RectTransform nextAmountRect;


    //private Text previousAmountText;
    //private Text currentAmountText;
    //private Text nextAmountText;

    //private RectTransform maskContainer;
    //private GUI_PowerUpText GUIPowerUptext;
    private static IEnumerator runningCoroutineExisting = null;
    private static Queue<IEnumerator> coroutineQueueExistingAmount = new Queue<IEnumerator>();
    private bool cr_Running_Existing = false;
    private RectTransform existingAmountText_ParentContainer_Rect;

    //public Dictionary<int, (RectTransform, Vector2)> _dictExistingAmountSlots { get; set; } = new Dictionary<int, (RectTransform, Vector2)>();

    private void Awake()
    {
        //maskContainer = transform.GetChild(0).GetComponent<RectTransform>();

        //previousAmountRect = maskContainer.GetChild(0).GetComponent<RectTransform>();
        //currentAmountRect = maskContainer.GetChild(1).GetComponent<RectTransform>();
        //nextAmountRect = maskContainer.GetChild(2).GetComponent<RectTransform>();

        //previousAmountText = maskContainer.GetChild(0).GetComponent<Text>();
        //currentAmountText = maskContainer.GetChild(1).GetComponent<Text>();
        //nextAmountText = maskContainer.GetChild(2).GetComponent<Text>();

        //GUIPowerUptext = GetComponent<GUI_PowerUpText>();

        existingAmountText_ParentContainer_Rect = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        
    }

    //public void PopulateDictionary (int slotNoIN, (RectTransform slotRtIN, Vector2 slotPosIN) tupleIN)
    //{
    //    _dictExistingAmountSlots.Add(slotNoIN, (tupleIN.slotRtIN, tupleIN.slotPosIN));
    //}


    public void SetTextsAndMove(int currentItemAmountIN, int newAmountIN)
    {

        int childCount = existingAmountText_ParentContainer_Rect.childCount;
        RectTransform targetSlot = null;
        RectTransform previousSlot = null;

        for (int i = 0; i < childCount; i++)
        {
            int slotAmountToMatch = int.Parse(existingAmountText_ParentContainer_Rect.GetChild(i).GetComponent<Text>().text);
          
            if(slotAmountToMatch == currentItemAmountIN)
            {
                previousSlot = existingAmountText_ParentContainer_Rect.GetChild(i).GetComponent<RectTransform>();                
            }

            else if (slotAmountToMatch == newAmountIN)
            {
                targetSlot = existingAmountText_ParentContainer_Rect.GetChild(i).GetComponent<RectTransform>();                
            }
           
        }
        //if (previousSlot == targetSlot) return;
        if (previousSlot == null || targetSlot == null) return;

        else if (runningCoroutineExisting == null && cr_Running_Existing == false)
        {
            runningCoroutineExisting = MoveTextsEnum(previousSlot, targetSlot);
            StartCoroutine(runningCoroutineExisting);
        }
        else
        {
            coroutineQueueExistingAmount.Enqueue(MoveTextsEnum(previousSlot, targetSlot));
        }
        
        //StartCoroutine(MoveTextsEnum(newAmountIN));
    }



    IEnumerator MoveTextsEnum(RectTransform previousSlotIN, RectTransform targetSlotIN)
    {
        cr_Running_Existing = true;

        float lerpDuration = .12f;
        float elapsedTime = 0f;

        Vector2 originalPosition = existingAmountText_ParentContainer_Rect.anchoredPosition;
        Vector2 lerpAmount = new Vector2(0, previousSlotIN.anchoredPosition.y - targetSlotIN.anchoredPosition.y);

        while (elapsedTime < lerpDuration)
        {
            existingAmountText_ParentContainer_Rect.anchoredPosition = Vector2.Lerp(originalPosition, originalPosition - (lerpAmount / 2), elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        Vector2 newPositioncontainer = originalPosition - lerpAmount / 5;
        existingAmountText_ParentContainer_Rect.anchoredPosition = newPositioncontainer;
        elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            existingAmountText_ParentContainer_Rect.anchoredPosition = Vector2.Lerp(newPositioncontainer, originalPosition + lerpAmount + (lerpAmount/2), elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        newPositioncontainer = originalPosition + lerpAmount + (lerpAmount / 3);
        existingAmountText_ParentContainer_Rect.anchoredPosition = newPositioncontainer;
        elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            existingAmountText_ParentContainer_Rect.anchoredPosition = Vector2.Lerp(newPositioncontainer, originalPosition + lerpAmount, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        existingAmountText_ParentContainer_Rect.anchoredPosition = originalPosition + lerpAmount;

        //Vector2 lerpAmount = new Vector2(_dictExistingAmountSlots[newAmountIN].Item2.x, _dictExistingAmountSlots[newAmountIN].Item2.y * -1);

        //while (elapsedTime < lerpDuration)
        //{
           
        //    foreach (KeyValuePair<int, (RectTransform, Vector2)> pair in _dictExistingAmountSlots)
        //    {               
        //        pair.Value.Item1.anchoredPosition = Vector2.Lerp(pair.Value.Item2, pair.Value.Item2 + lerpAmount, elapsedTime / lerpDuration);
        //    }
        //    elapsedTime += Time.deltaTime;
        //    yield return null;
        //}

        //for (int i = 0; i < _dictExistingAmountSlots.Count; i++)
        //{
        //    _dictExistingAmountSlots[i].Item1.anchoredPosition = _dictExistingAmountSlots[i].Item2 + lerpAmount;
        //    var newTupleValueSlot = (_dictExistingAmountSlots[i].Item1, _dictExistingAmountSlots[i].Item1.anchoredPosition);

        //    _dictExistingAmountSlots[i] = newTupleValueSlot;
        //}
        cr_Running_Existing = false;

        DequeueExisting();
    }

    private void DequeueExisting()
    {
        runningCoroutineExisting = null;
        if (coroutineQueueExistingAmount.Count > 0)
        {
            runningCoroutineExisting = coroutineQueueExistingAmount.Dequeue();
            StartCoroutine(runningCoroutineExisting);
        }
    }

}
