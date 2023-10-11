using GameData;
using LabData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// [Singleton] 
/// 管理框架的生命週期
/// </summary>
public class GameApplication : MonoSingleton<GameApplication>
{
    private bool _isOnApplicationQuit;
    private bool _isDisposeCompleted;
    
    /// <summary>
    /// 继承Manager的集合
    /// </summary>
    private List<IGameManager> _gameManagers;
    public static bool IsVR = false;

    private void Awake()
    {
        var applicationConfig = LabTools.GetConfig<ApplicationConfig>();
        XRSettings.enabled = applicationConfig.IsOpenVR;
        IsVR = applicationConfig.IsOpenVR;
        DontDestroyOnLoad(this);
        GameApplicationInit();
                
        if (applicationConfig.OneSelf)
        {            
            OneSelfStart();
        }        
        else
        {
            ExternalStart();
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void GameApplicationInit()
    {
        _isOnApplicationQuit = false;
        GameEventCenter.Init();
        _gameManagers = FindObjectsOfType<MonoBehaviour>()
            .OfType<IGameManager>()
            .ToList()
            .OrderBy(m => m.Weight)
            .ToList();
        _gameManagers.ForEach(p =>
        {
            p.ManagerInit();          
        });
    }

    /// <summary>
    /// 外部啟動：解析命令列丟來的指令後直接進 Main Scene
    /// </summary>
    private void ExternalStart()
    {
        // 拿命令列丟來的指令
        string[] arguments = Environment.GetCommandLineArgs();

        // 直接把第一個引數解析成 GameFlowData !? 太暴力了吧
        GameDataManager.FlowData = LabTools.GetDataByString<GameFlowData>(arguments[1]);

        // 
        GameDataManager.LabDataManager.LabDataCollectInit(()=> GameDataManager.FlowData.UserId);
        GameSceneManager.Instance.Change2MainScene();
    }

    /// <summary>
    /// 自身啟動：切換到 MainUI
    /// </summary>
    private void OneSelfStart()
    {
        GameSceneManager.Instance.Change2MainUI();
    }

    /// <summary>
    /// 銷毀！！
    /// </summary>
    public void GameApplicationDispose()
    {
        GameEventCenter.RemoveAllEvents();
        StartCoroutine(GameApplicationDisposeEnumerator());
    }

    private IEnumerator GameApplicationDisposeEnumerator()
    {
        if (_gameManagers.Count <= 0)
        {
            yield break;
        }
        
        for (int i = 0; i < _gameManagers.Count; i++)
        {
            yield return StartCoroutine(_gameManagers[i].ManagerDispose());
        }

        _gameManagers.Clear();
        _isDisposeCompleted = true;
        yield return null;
    }

    /// <summary>
    /// 等待 Dispose 結束才 Application.Quit
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitforQuitGame()
    {
        yield return new WaitUntil(() => _isDisposeCompleted);
        Application.Quit();
    }

    /// <summary>
    /// Unity MonoBehaviour 特殊函式：當關閉時呼叫
    /// </summary>
    protected void OnApplicationQuit()
    {
        if (!_isOnApplicationQuit)
        {
            GameApplicationDispose();
            _isOnApplicationQuit = true;
            StartCoroutine(WaitforQuitGame());
        }
        OnApplicationQuit();
    }
}


