
using System;
using System.Collections.Generic;

public class GlobalBuffComponent
{

    /// <summary>
    /// buff名称
    /// </summary>
    public string Name = "Buff";

    /// <summary>
    /// 生存时间
    /// </summary>
    public float Survive = 1f;

    /// <summary>
    /// 起效标识
    /// </summary>
    public bool Enable = true;

    /// <summary>
    /// 标识该buff应该被清除了
    /// </summary>
    public bool Dead = false;

    public Action<List<IBuffTarget>> OnBuffAwake;
    public virtual void m_OnBuffAwake(List<IBuffTarget> list)
    {
        if (OnBuffAwake != null) OnBuffAwake(list);
        //
    }

    protected bool _isStarted = false;
    public Action<List<IBuffTarget>> OnBuffStart;
    public virtual void m_OnBuffStart(List<IBuffTarget> list) {
        if (OnBuffStart != null) OnBuffStart(list);
        //Todo Buff开始实现
    }

    public Action<List<IBuffTarget>> OnBuffEffect;
    public virtual void m_DoBuffEffect(float deltaTime, List<IBuffTarget> list) {
        if (Dead || !Enable) return;
        if (!_isStarted)
        {
            m_OnBuffStart(list);
            _isStarted = true;
        }
        if (OnBuffEffect != null) OnBuffEffect(list);

        //默认 生存时间判断
        verifySurvive(deltaTime);

        //Todo 这里扩写Buff的具体作用 ...
    }

    protected void verifySurvive(float deltaTime) {
        Survive -= deltaTime;
        if (Survive <= 0) Dead = true;
    }

    public Action<List<IBuffTarget>> OnBuffFinish;
    public virtual void m_OnBuffFinish(List<IBuffTarget> list) {
        if (OnBuffFinish != null) OnBuffFinish(list);
        //Todo buff结束实现
    }
    public Action<List<IBuffTarget>> OnBuffRemoved;
    public virtual void m_OnBuffRemoved(List<IBuffTarget> list) {
        if (OnBuffRemoved != null) OnBuffRemoved(list);
        //
    }


}