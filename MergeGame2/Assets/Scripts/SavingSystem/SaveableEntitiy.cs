using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class SaveableEntitiy : MonoBehaviour
{
    [SerializeField] string uniqueIdentifier = "";
    static Dictionary<string, SaveableEntitiy> identitiyDict = new Dictionary<string, SaveableEntitiy>();

    public object CaptureState()
    {
        Dictionary<string, object> saveEntitiesDict = new Dictionary<string, object>();

        foreach (ISaveable saveableScript in GetComponents<ISaveable>())
        {
            saveEntitiesDict[saveableScript.GetType().ToString()] = saveableScript.CaptureState();
        }

        return saveEntitiesDict;
    }

    public void RestoreState(object saveEntitiesDictIN)
    {
        Dictionary<string, object> restoreEntitiesDict = (Dictionary<string, object>)saveEntitiesDictIN ;
        foreach (ISaveable loadable in GetComponents<ISaveable>())
        {
            string loadabletypeString = loadable.GetType().ToString();
            if (restoreEntitiesDict.ContainsKey(loadabletypeString))
            {
                loadable.RestoreState(restoreEntitiesDict[loadabletypeString]);
            }
        }
    }


#if UNITY_EDITOR
    private void Update()
    {
        if (Application.IsPlaying(gameObject)) return;
        if (string.IsNullOrEmpty(gameObject.scene.path)) return;

        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty serializedProperty = serializedObject.FindProperty("uniqueIdentifier");

        if (string.IsNullOrEmpty(serializedProperty.stringValue) || !IsUnique(serializedProperty.stringValue))
        {
            serializedProperty.stringValue = System.Guid.NewGuid().ToString();
            serializedObject.ApplyModifiedProperties();
        }

        identitiyDict[serializedProperty.stringValue] = this;
    }
#endif

    private bool IsUnique (string serializedPropertyName)
    {
        if (!identitiyDict.ContainsKey(serializedPropertyName)) return true;
        
        if (identitiyDict[serializedPropertyName] == this) return true;

        if (identitiyDict[serializedPropertyName] == null)
        {
            identitiyDict.Remove(serializedPropertyName); return true;
        }

        if (identitiyDict[serializedPropertyName].GetUniqueIdentifier() != serializedPropertyName)
        {
            identitiyDict.Remove(serializedPropertyName); return true;
        }
        return false;

    }

    public string GetUniqueIdentifier()
    {
        return uniqueIdentifier;
    }
}
