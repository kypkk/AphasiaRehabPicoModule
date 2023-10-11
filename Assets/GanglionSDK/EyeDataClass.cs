//using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeDataClass 
{ 

    //public Matrix4x4 HeadPos { get; set; }
    public SerializableVector3 EyeGazePoint { get; set; }
    public SerializableVector3 EyeGazeVector { get; set; }
    public float LeftEyeGazeOpenness { get; set; }
    public float RightEyeGazeOpenness { get; set; }
    public SerializableVector3 LeftEyePosition { get; set; }
    public SerializableVector3 RightEyePosition { get; set; }
    public string timeStamp { get; set; }

    public EyeDataClass()
    {
        //HeadPos = new Matrix4x4();
        EyeGazePoint = new SerializableVector3(new Vector3());
        EyeGazeVector = new SerializableVector3(new Vector3());
        LeftEyePosition = new SerializableVector3(new Vector3());
        RightEyePosition = new SerializableVector3(new Vector3());
        LeftEyeGazeOpenness = -1.0f;
        RightEyeGazeOpenness = -1.0f;
        timeStamp = "";
    }
    public EyeDataClass(/*Matrix4x4 _HP,*/ SerializableVector3 _EGP, SerializableVector3 _EGV, SerializableVector3 _LEP, SerializableVector3 _REP,float _LEGO,float _REGO,string _timeStamp)
    {
        //HeadPos = _HP;
        EyeGazePoint =_EGP;
        EyeGazeVector = _EGV;
        LeftEyePosition = _LEP;
        RightEyePosition = _REP;
        LeftEyeGazeOpenness = _LEGO;
        RightEyeGazeOpenness = _REGO;
        timeStamp=_timeStamp;
    }
    public EyeDataClass(/*Matrix4x4 _HP,*/ SerializableVector3 _EGP, SerializableVector3 _EGV, SerializableVector3 _LEP, SerializableVector3 _REP, float _LEGO, float _REGO)
    {
        //HeadPos = _HP;
        EyeGazePoint = _EGP;
        EyeGazeVector = _EGV;
        LeftEyePosition = _LEP;
        RightEyePosition = _REP;
        LeftEyeGazeOpenness = _LEGO;
        RightEyeGazeOpenness = _REGO;
        timeStamp = DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss-fff");
    }
}
