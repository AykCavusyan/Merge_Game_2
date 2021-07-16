using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingSystem : MonoBehaviour
{
   


    private void CaptureState()
    {
        Dictionary<string, object> saveableEntitiesDict = new Dictionary<string, object>();

        foreach (SaveableEntitiy savEntity in FindObjectsOfType<SaveableEntitiy>())
        {
            saveableEntitiesDict[savEntity.GetUniqueIdentifier()] = savEntity.CaptureState();
        }
    }

    private void RestoreState(Dictionary<string,object> saveableEntitiesDictIN)
    {
        foreach (KeyValuePair<string,object> pairs in saveableEntitiesDictIN)
        {
            if (pairs.Value is GameItems)
            {
                GameObject newGameItem = new GameObject();
            }
        }
    }

}
