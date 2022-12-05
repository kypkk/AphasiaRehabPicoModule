using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 見 GameEntityController 
/// </summary>
public interface IGameEntityController 
{
    List<GameEntityBase> GameEntities { get; set; }
    void EntityBuild(GameEntityBase @base);
    void EntityDestroy(GameEntityBase @base);
}
