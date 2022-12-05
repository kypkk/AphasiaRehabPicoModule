using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityController : MonoBehaviour, IGameEntityController
{   
    /// <summary>
    /// 所有 GameEntityBase 都在這了
    /// </summary>
    /// <value></value>
    public List<GameEntityBase> GameEntities { get; set; }

    /// <summary>
    /// 新增 Enitity
    /// </summary>
    /// <param name="base"></param>
    public void EntityBuild(GameEntityBase @base)
    {
        GameEntities.Add(@base);
    }

    /// <summary>
    /// 刪掉 Entity
    /// </summary>
    /// <param name="base"></param>
    public void EntityDestroy(GameEntityBase @base)
    {
        GameEntities.Remove(@base);
        Destroy(@base.gameObject);
    }

 
}
