using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEventCenter
{
    /// <summary>
    /// 所有的 Event
    /// </summary>
    private static Dictionary<string, List<Delegate>> _gameEvents;
    private static bool _isCenterInit;
    
    /// <summary>
    /// Init
    /// </summary>
    public static void Init()
    {
        if (_isCenterInit)
        {
            return;
        }
        _gameEvents = new Dictionary<string, List<Delegate>>();
        _isCenterInit = true;
    }

    /// <summary>
    /// 開始監聽一個事件 (callback 帶參數)
    /// </summary>
    /// <param name="eventName">事件名稱</param>
    /// <param name="callback">當事件被觸發時，要做的事情</param>
    /// <typeparam name="T">要什麼參數自己填，記得呼叫的時候也要 call 對</typeparam>
    public static void AddEvent<T>(string eventName, Action<T> callback)
    {
        // FIXME 看能不能直接呼叫 AddEvent()
        // eventName 已存在，添加至該委派
        if (_gameEvents.TryGetValue(eventName, out List<Delegate> actions))
        {
            actions.Add(callback);
        }
        // eventName 不存在，加一個
        else
        {
            actions = new List<Delegate> { callback };
            _gameEvents.Add(eventName, actions);
        }
    }

    /// <summary>
    /// 開始監聽一個事件
    /// </summary>
    /// <param name="eventName">事件名稱</param>
    /// <param name="callback">當事件被觸發時，要做的事情</param>
    public static void AddEvent(string eventName, Action callback)
    {
        // eventName 已存在，添加至該委派
        if (_gameEvents.TryGetValue(eventName, out List<Delegate> actions))
        {
            actions.Add(callback);
        }
        // eventName 不存在，加一個
        else
        {
            actions = new List<Delegate> { callback };
            _gameEvents.Add(eventName, actions);
        }
    }

    /// <summary>
    /// FIXME
    /// 停止監聽一個事件 
    /// </summary>
    /// <param name="eventName">事件名稱</param>
    /// <param name="callback">監聽函式</param>
    /// <typeparam name="T"></typeparam>
    public static void RemoveEvent<T>(string eventName, Action<T> callback)
    {
        if (!_gameEvents.TryGetValue(eventName, out List<Delegate> actions)) 
        {
            Debug.Log($"Event {eventName} 不存在");
            return;
        }

        if(!actions.Contains(callback))
        {
            Debug.Log($"Event {eventName} 不存在此監聽者");
            return;
        }

        actions.Remove(callback);

        // 如果這個事件沒有監聽者了，刪掉這個事件！
        if (actions.Count == 0)
        {
            _gameEvents.Remove(eventName);
        }
    }

    /// <summary>
    /// 停止監聽一個事件
    /// </summary>
    /// <param name="eventName">事件名稱</param>
    /// <param name="callback">監聽函式</param>
    /// <typeparam name="T"></typeparam>
    public static void RemoveEvent(string eventName, Action callback)
    {
        if (!_gameEvents.TryGetValue(eventName, out List<Delegate> actions)) 
        {
            Debug.Log($"Event {eventName} 不存在");
            return;
        }

        if(!actions.Contains(callback))
        {
            Debug.Log($"Event {eventName} 不存在此監聽者");
            return;
        }

        actions.Remove(callback);

        // 如果這個事件沒有監聽者了，刪掉這個事件！
        if (actions.Count == 0)
        {
            _gameEvents.Remove(eventName);
        }
    }

    /// <summary>
    /// 觸發一個事件
    /// </summary>
    /// <param name="eventName">事件名稱</param>
    /// <param name="args">參數要填對喔喔喔</param>
    public static void DispatchEvent<T>(string eventName, T args)
    {
        if (!_gameEvents.TryGetValue(eventName, out var actions))
        {
            throw new NotImplementedException($"Event {eventName} 不存在！");
        }

        foreach (var action in actions)
        {
            ((Action<T>)action).Invoke(args); // faster!
            // action.DynamicInvoke(args); // slow
        }
    }

    /// <summary>
    /// 觸發一個事件
    /// </summary>
    /// <param name="eventName">事件名稱</param>
    /// <param name="args">參數要填對喔喔喔</param>
    public static void DispatchEvent(string eventName)
    {
        if (!_gameEvents.TryGetValue(eventName, out var actions))
        {
            throw new NotImplementedException($"Event {eventName} 不存在！");
        }

        foreach (var action in actions)
        {
            ((Action)action).Invoke();
        }
    }

    /// <summary>
    /// 事件全部清掉AAAAA
    /// </summary>
    public static void RemoveAllEvents()
    {
        _gameEvents?.Clear();
        //_gameEvents = null;
        // _isCenterInit = false;
    }
}
