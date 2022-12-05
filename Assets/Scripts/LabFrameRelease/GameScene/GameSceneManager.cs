using GameData;
using LabData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoSingleton<GameSceneManager>, IGameManager
{
    /// <summary>
    /// 目前正在載入場景的工作
    /// </summary>
    private AsyncOperation _operation;

    /// <summary>
    /// 目前場景
    /// </summary>
    /// <value></value>
    public Scene CurrentScene { get; private set; }

    int IGameManager.Weight => GlobalData.GameSceneManagerWeight;


    /// <summary>
    /// 切換場景
    /// </summary>
    /// <param name="onComplete">自訂完成後要做的事</param>
    /// <param name="sceneName">要切到哪個場景？</param>
    /// <param name="mode">一般而言就是 SINGLE，除非你要疊加場景</param>
    public void ChangeScene(List<Action> onComplete, string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        _operation = SceneManager.LoadSceneAsync(sceneName, mode);
        _operation.completed += (AsyncOperation obj) =>
        {
            OnSceneChangeCompleted();
            onComplete?.ForEach(p => p.Invoke());
        };
        _operation.allowSceneActivation = true;
    }

    /// <summary>
    /// 完成後要做的事 (basic)
    /// </summary>
    private void OnSceneChangeCompleted()
    {
        _operation = null;
        CurrentScene = SceneManager.GetActiveScene();
        Debug.Log("已進入場景 "+CurrentScene);
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// 切換到 UI SCENE
    /// </summary>
    public void Change2MainUI()
    {
        ChangeScene(new List<Action>()
        {
            GameUIManager.Instance.StartMainUiLogic
        }, GlobalData.MainUiScene);
    }

    /// <summary>
    /// 切換到 MAIN SCENE
    /// </summary>
    public void Change2MainScene()
    {
        if (GameApplication.IsVR)
        {
            ChangeScene(new List<Action>()
            {
                GameUIManager.Instance.StartMainUiLogic,
                GameEntityManager.Instance.SetSceneEntity,
                GameTaskManager.Instance.StartGameTask,
            }, GlobalData.MainScene+"_VR"); // FIXME hardcoded vr scene
        }
        else
        {
            ChangeScene(new List<Action>()
            {
                GameUIManager.Instance.StartMainUiLogic,
                GameEntityManager.Instance.SetSceneEntity,
                GameTaskManager.Instance.StartGameTask,
            }, GlobalData.MainScene);
        }
    }

    void IGameManager.ManagerInit()
    {
        
    }

    IEnumerator IGameManager.ManagerDispose()
    {
        yield return null;
    }
}
