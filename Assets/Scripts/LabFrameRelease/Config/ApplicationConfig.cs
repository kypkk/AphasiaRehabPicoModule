using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 遊戲的啟動資訊
/// </summary>
[SerializeField]
public class ApplicationConfig 
{
    public bool IsOpenVR { get; set; } = false;

    /// <summary>
    /// 啟動並進入MainUI or 讀引數
    /// </summary>
    /// <value></value>
    public bool OneSelf { get; set; } = true;
}
