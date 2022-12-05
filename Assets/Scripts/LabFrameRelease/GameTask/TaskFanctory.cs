using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TestGameFrame;

public class TaskFanctory 
{
    public static List<TaskBase> GetCurrentScopeTasks()
    {
        var temptasks = new List<TaskBase>
        {
            new Test_PlayerTask(),
            new Test_EnemyTask()
        };
        return temptasks;
    }
}
