using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SavingSystem : MonoBehaviour
{
    //private GameObject panel_GameSlot;
    private GameObject player;
    private Dictionary<string, object> saveableEntitiesDict = new Dictionary<string, object>();

    private void Start()
    {
        
        //panel_GameSlot = ItemBag.Instance.panel_Gameslots;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SaveFile();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            LoadFile();
        }
    }
    private string GetPathFromSaveFile(string saveName)
    {
        return Path.Combine(Application.persistentDataPath, saveName + ".sav");
    }

    private void SaveFile()
    {
        string path = GetPathFromSaveFile("newsavegame");
        CaptureState();
        SerializeData(path);
    }

    public void LoadFile()
    {
        Debug.Log("load is receiving click");


        string path = GetPathFromSaveFile("newsavegame");  
        RestoreState(DeserializeData(path));
    }

    private void SerializeData(string savePath)
    {
        Debug.Log("saving to" + savePath);
        using(FileStream stream = File.Open(savePath, FileMode.OpenOrCreate))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, saveableEntitiesDict);
        }
    }

    private Dictionary<string, object> DeserializeData(string savePath)
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("savegame not found");
            return new Dictionary<string, object>();
            
        }
        using(FileStream stream = File.Open(savePath, FileMode.Open))
        {
            Debug.Log("savegame  found");

            BinaryFormatter formatter = new BinaryFormatter();
            return (Dictionary<string,object>)formatter.Deserialize(stream);
        }

    }

    private void CaptureState()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        foreach (SaveableEntitiy savEntity in player.GetComponents<SaveableEntitiy>())
        {
            saveableEntitiesDict[savEntity.GetUniqueIdentifier()] = savEntity.CaptureState();
        }

        //testDict = saveableEntitiesDict;
    }

    private void RestoreState(Dictionary<string,object> saveableEntitiesDictIN)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        Debug.Log("restore state is working");

        foreach (SaveableEntitiy savEntity in player.GetComponents<SaveableEntitiy>())
        {
            string id = savEntity.GetUniqueIdentifier(); Debug.Log(id);
            if (saveableEntitiesDictIN.ContainsKey(id))
            {
                savEntity.RestoreState(saveableEntitiesDictIN[id]); Debug.Log(saveableEntitiesDictIN[id]);
            }
        }

        //foreach (KeyValuePair<string,object> pairs in saveableEntitiesDictIN)
        //{
          
        //    foreach (KeyValuePair<string,object> pair in (Dictionary<string,object>)pairs.Value)
        //    {
        //        if(string.Equals(pair.Key, "GameItems"))
        //        {

        //            GameObject gameItemtoLoad = new GameObject();
        //            gameItemtoLoad.transform.SetParent(panel_GameSlot.transform);
        //            gameItemtoLoad.AddComponent<GameItems>().RestoreState(pair.Value);

        //        }
        //    } 
        //}
    }

}
