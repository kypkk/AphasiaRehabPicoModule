using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EEGSend : MonoBehaviour
{
    private GanglionController _ganglionController;

    // Start is called before the first frame update
    void Start()
    {
        _ganglionController = GameObject.Find("GanglionController").GetComponent<GanglionController>();
        _ganglionController.StreamData();    
    }

    // Update is called once per frame
    void Update()
    {
        //recordEEG from VEP_Task.cs by PlayerPrefs

        //Debug.Log("Record");
        //EEGDataClass temp = new EEGDataClass(_ganglionController.GetEegData(0).ToString(), _ganglionController.GetEegData(1).ToString(), _ganglionController.GetEegData(2).ToString(), _ganglionController.GetEegData(3).ToString());
        //SaveLocalFile.Instance.Save("/eeg/", temp);
        
    }
}
