using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class GanglionController : MonoBehaviour
{
    private AndroidJavaClass unityClass;
    private AndroidJavaObject unityActivity;
    private AndroidJavaObject _pluginInstance;
    public bool connectionStatus = false;
    private int[] impedanceValues = { 100, 100, 100, 100, 100 };
    private double[] eegValues = { 0, 0, 0, 0 };

    public void IntializePlugin(string pluginName)
    {
        unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        _pluginInstance = new AndroidJavaObject(pluginName);
        if (_pluginInstance == null)
            Debug.Log("Plugin Error");
        _pluginInstance.CallStatic("receiveUnityActivity", unityActivity);
    }

    public void ReceiveData(string num) // called by Android plugin
    {
        // Convert to Dictionary
        var values = JsonConvert.DeserializeObject<Dictionary<string, Double>>(num);
        eegValues[0] = values["ch1_1"];
        eegValues[1] = values["ch2_1"];
        eegValues[2] = values["ch3_1"];
        eegValues[3] = values["ch4_1"];

        // foreach (KeyValuePair<string, Double> kvp in values)
        // {
        //     Debug.Log($"name = {kvp.Key}, val = {kvp.Value}");
        // }
        // Debug.Log("Record");

        EEGDataClass temp1 = new EEGDataClass(values["ch1_1"].ToString(), values["ch2_1"].ToString(), values["ch3_1"].ToString(), values["ch4_1"].ToString());
        EEGDataClass temp2 = new EEGDataClass(values["ch1_2"].ToString(), values["ch2_2"].ToString(), values["ch3_2"].ToString(), values["ch4_2"].ToString());

        SaveLocalFile.Instance.Save("/eeg/", temp1);
        SaveLocalFile.Instance.Save("/eeg/", temp2);
    }

    public void ReceiveImpedance(string val) // called by Android plugin
    {
        var values = JsonConvert.DeserializeObject<Dictionary<Int32, Int32>>(val);
        foreach (KeyValuePair<Int32, Int32> kvp in values)
        {
            impedanceValues[kvp.Key] = kvp.Value;
            Debug.Log($"name = impedance {kvp.Key}, val = {kvp.Value}");
        }

    }

    public double GetEegData(int ch)
    {
        // Debug.Log("GetEegData: " + eegValues[ch]);
        return eegValues[ch];
    }
    public double GetImpedanceData(int ch)
    {
        return impedanceValues[ch];
    }
    public void InitGanglion()
    {
        Debug.Log("call Init");
        if (_pluginInstance != null)
        {
            Debug.Log(_pluginInstance);
            _pluginInstance.Call("Init");
        }
    }

    public void StreamData()
    {
        Debug.Log("call StreamData");
        if (_pluginInstance != null)
        {
            _pluginInstance.Call("StreamData");
        }
    }
    public void StreamImpedance()
    {
        Debug.Log("call StreamImpedance");
        if (_pluginInstance != null)
        {
            _pluginInstance.Call("StreamImpedance");
        }
    }

    private void GetGanglionStatus()
    {
        if (_pluginInstance != null)
        {
            connectionStatus = _pluginInstance.Get<bool>("mConnected");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        IntializePlugin("com.okt.ganglionplugin.PluginInstance");
        InvokeRepeating("GetGanglionStatus", 0f, 1f); // Regularly check status
    }

    // Update is called once per frame
    void Update()
    {
    }
}
