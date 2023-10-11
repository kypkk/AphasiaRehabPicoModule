/// 此為新腳本，用以紀錄使用者在看什麼物件
/// by JCxYIS
/// 20220517


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataSync;
using LabData;

public class EyeFocusData : LabDataBase
{
    /// <summary>
    /// 注視目標物
    /// </summary>
    /// <value></value>
    public string FocusName { get; set; }

    /// <summary>
    /// 3D 注視點
    /// </summary>
    /// <value></value>
    public string FocusPoint { get; set; }

    /// <summary>
    /// 注視法向量
    /// </summary>
    /// <value></value>
    public string FocusNormal { get; set;}

    public EyeFocusData(string focusName, string focusPoint, string focusNormal)
    {
        FocusName = focusName;
        FocusPoint = focusPoint;
        FocusNormal = focusNormal;
    }
}