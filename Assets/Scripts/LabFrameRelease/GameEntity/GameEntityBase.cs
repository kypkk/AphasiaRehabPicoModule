using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 遊戲中的實體
/// 遊戲中所用到的物體 (如 Player, Npc, etc.) 這些物體都要繼承此腳本。
/// </summary>
public abstract class GameEntityBase : MonoBehaviour
{
    // public string Describe;

    protected IGameEntityController EntityController;

    public void Awake()
    {
        EntityController = GameEntityManager.Instance.EntityController;
        EntityController.EntityBuild(this);
    }
        
    public virtual void EntityInit()
    {
        // 
    }

    /// <summary>
    /// 丟棄這個 Entity (尚未 Destroy)
    /// </summary>
    public virtual void EntityDispose()
    {

    }

    /// <summary>
    /// Destroy 這個 Enitity
    /// </summary>
    public virtual void EntityDestroy()
    {
        EntityController.EntityDestroy(this);        
    }
   
}
