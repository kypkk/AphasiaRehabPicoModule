using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LabData;
using GameData;


public class GameUIManager : MonoSingleton<GameUIManager>, IGameManager
{
    int IGameManager.Weight => GlobalData.GameUIManagerWeight;

    IEnumerator IGameManager.ManagerDispose()
    {
        yield return null;
    }

    void IGameManager.ManagerInit()
    {
       
    }

    public void BindEvent()
    {
    }

    public void StartMainUiLogic()
    {

    }
}

