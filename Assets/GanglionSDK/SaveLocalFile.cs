using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;

public class SaveLocalFile : MonoBehaviour
{
    private static SaveLocalFile s_Instance;
    public static SaveLocalFile Instance
    {
        get
        {
            if (s_Instance != null)
            {
                Debug.Log("DEBUG: PrefabSingleton [case1]: returning loaded singleton");
                return s_Instance;      // Early return the created instance 
            }

            // Find the active singleton already created
            // reference: https://docs.unity3d.com/ScriptReference/Object.FindObjectOfType.html
            s_Instance = FindObjectOfType<SaveLocalFile>();       // note: this is use during the Awake() logic
            if (s_Instance != null)
            {
                Debug.Log("DEBUG: PrefabSingleton [case2]: Found the active object in memory");
                return s_Instance;
            }

            CreateDefault();     // create new game object 

            return s_Instance;
        }
    }
    public string time { get; set; } = "";
    static void CreateDefault()
    {
        SaveLocalFile prefab = Resources.Load<SaveLocalFile>("Prefab/SaveLocalFile");
        s_Instance = Instantiate(prefab);       // No need to care about the position, rotation, ...
        s_Instance.gameObject.name = "PrefabSingleton";
        SaveLocalFile.Instance.time = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
    }

    public static DirectoryInfo SafeCreateDirectory(string path)
    {
        //Generate if you don't check if the directory exists
        if (Directory.Exists(path))
        {
            return null;
        }
        return Directory.CreateDirectory(path);
    }

    public bool isAlive()
    {
        if (s_Instance != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //public void Save(string Directory_path, string data)
    //{
    //    //Data storage
    //    SafeCreateDirectory(Application.persistentDataPath + "/" + Directory_path);
    //    string json = JsonUtility.ToJson(data);
    //    var Writer = new StreamWriter(Application.persistentDataPath + "/" + Directory_path + "/date.json");
    //    Writer.Write(json);
    //    Writer.Flush();
    //    Writer.Close();
    //}
    public void Save(string Directory_path, EEGDataClass data)
    {
        if (time == "")
        {
            time = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");

        }
        string filename = time + "_" + GameDataManager.FlowData.UserId +  "_eeg.json";
        Debug.Log(filename);
        //Data storage
        SafeCreateDirectory(Application.persistentDataPath + "/" + Directory_path);
        // Debug.Log("EEG get: " + data.ch1 + data.ch2 + data.ch3 + data.ch4);

        string jsonData = JsonConvert.SerializeObject(data);
        Debug.Log(jsonData);
        var Writer = new StreamWriter(Application.persistentDataPath + "/" + Directory_path + "/" + filename, append: true);
        Writer.Write(jsonData);
        Writer.Write("\r\n");
        Writer.Flush();
        Writer.Close();
        //File.WriteAllText(Application.persistentDataPath + "/" + Directory_path + "/" + filename, jsonData);
    }
    /*public void Save(string Directory_path, EyeDataClass data)
    {
        if (time == "")
        {
            time = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");

        }
        string filename = time + "_eye.json";
        Debug.Log(filename);
        SafeCreateDirectory(Application.persistentDataPath + "/" + Directory_path);
        string jsonData = JsonConvert.SerializeObject(data);
        var Writer = new StreamWriter(Application.persistentDataPath + "/" + Directory_path + "/" + filename, append: true);
        Writer.Write(jsonData);
        Writer.Write("\r\n");
        Writer.Flush();
        Writer.Close();
    }*/
    void Awake()
    {
        // Debug.Log("DEBUG: PrefabSingleton: Awake() begin. " + InfoGameObject());
        if (Instance != this)
        {
            // Debug.Log("DEBUG: PrefabSingleton: will destroy the extra gameObject " + InfoGameObject());
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);      // note: s_Instance become null when switch scene if not place this line of code
    }
    void Start()
    {
        Debug.Log("DEBUG: PrefabSingleton: Start() begin. " /*+ InfoGameObject()*/);
    }
    private void Update()
    {
        //Debug.Log("singleton object:" + isAlive());
    }

}


