using System;
using UnityEngine;

public class DelayActionBase
{

    public DelayActionBase(string name, Action action)
    {
        this.dead = false;
        this.hash = GetHashCode();
        this.action = action;
    }
    
    public string name { get; protected set; }
    public int hash { get; protected set; }

    /// <summary>
    /// 初始化 必要数据
    /// </summary>
    public virtual void Init() {
        //
    }

    /// <summary>
    /// Update
    /// </summary>
    public virtual void Update()
    {
        //
    }
    public bool dead { get; protected set; }
    public Action action { get; protected set; }

}