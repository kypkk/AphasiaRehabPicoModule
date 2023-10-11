using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSync;

public class GameStageData : LabDataBase
{
    /// <summary>
    /// 對了嗎？
    /// </summary>
    public bool IsCorrect;

    /// <summary>
    /// 此題用時
    /// </summary>
    public float TimeUsed;

    public string Question;
}
