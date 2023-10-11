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
    public static ILabDataManager LabDataManager => LabDataTestComponent.LabDataManager;

    /// <summary>
    /// 遊戲數據
    /// </summary>
    public static GameFlowData FlowData { get; set; } = new GameFlowData();

    int IGameManager.Weight => GlobalData.GameDataManagerWeight;

    void IGameManager.ManagerInit()
    {
        // LabDataManager = new LabDataManager();
        Debug.Log("LabDataManager init");
    }

    IEnumerator IGameManager.ManagerDispose()
    {
        LabDataManager.LabDataDispose();
        yield return null;
    }
}
