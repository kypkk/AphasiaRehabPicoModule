using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DataSync;
using GameData;
using LabData;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 有關 GameData 都是這裡在管 (除了 ApplicationConfig)
/// </summary>
public class GameDataManager : MonoSingleton<GameDataManager>, IGameManager
{
    /// <summary>
    /// LabData
    /// </summary>
    public static ILabDataManager LabDataManager { get; set; }

    /// <summary>
    /// 遊戲數據
    /// </summary>
    public static GameFlowData FlowData { get; set; }

    int IGameManager.Weight => GlobalData.GameDataManagerWeight;

    void IGameManager.ManagerInit()
    {
        LabDataManager = new LabDataManager();
    }

    IEnumerator IGameManager.ManagerDispose()
    {
        LabDataManager.LabDataDispose();
        yield return null;
    }
}
