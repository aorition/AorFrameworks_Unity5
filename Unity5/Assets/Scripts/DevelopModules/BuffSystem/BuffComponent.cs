using System;

public class BuffComponent
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

    public Action<IBuffTarget> OnBuffAwake;
    public virtual void m_OnBuffAwake(IBuffTarget target)
    {
        if (OnBuffAwake != null) OnBuffAwake(target);
    }

    protected bool _isStarted = false;
    public Action<IBuffTarget> OnBuffStart;
    public virtual void m_OnBuffStart(IBuffTarget target) {
        if (OnBuffStart != null) OnBuffStart(target);
        //Todo Buff开始实现
    }

    public Action<IBuffTarget> OnBuffEffect;
    public virtual void m_DoBuffEffect(float deltaTime, IBuffTarget target) {
        if (Dead || !Enable) return;
        if (!_isStarted) {
            m_OnBuffStart(target);
            _isStarted = true;
        }
        if (OnBuffEffect != null) OnBuffEffect(target);

        //默认 生存时间判断
        verifySurvive(deltaTime);

        //Todo 这里扩写Buff的具体作用 ...
    }

    protected void verifySurvive(float deltaTime)
    {
        Survive -= deltaTime;
        if (Survive <= 0) Dead = true;
    }

    public Action<IBuffTarget> OnBuffFinish;
    public virtual void m_OnBuffFinish(IBuffTarget target) {
        if (OnBuffFinish != null) OnBuffFinish(target);
        //Todo buff结束实现
    }
    public Action<IBuffTarget> OnBuffRemoved;
    public virtual void m_OnBuffRemoved(IBuffTarget target) {
        if (OnBuffRemoved != null) OnBuffRemoved(target);
        //
    }

}