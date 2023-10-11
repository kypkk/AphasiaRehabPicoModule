using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EEGDataClass
{

    //public double ch1 { get; set; }
    //public double ch2 { get; set; }
    //public double ch3 { get; set; }
    //public double ch4 { get; set; }
    public string ch1 { get; set; }
    public string ch2 { get; set; }
    public string ch3 { get; set; }
    public string ch4 { get; set; }
    public string timeStamp { get; set; }

    public EEGDataClass() {
        ch1 = ch2 = ch3 = ch4 = "";
        timeStamp = "";
    }
    public EEGDataClass(string _ch1, string _ch2, string _ch3, string _ch4,string _timeStamp) { 
        ch1 = _ch1;
        ch2 = _ch2;
        ch3 = _ch3;
        ch4 = _ch4;
        timeStamp = _timeStamp;
    }
    public EEGDataClass(string _ch1, string _ch2, string _ch3, string _ch4)
    {
        ch1 = _ch1;
        ch2 = _ch2;
        ch3 = _ch3;
        ch4 = _ch4;
        timeStamp = DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss-fff"); 
    }
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
