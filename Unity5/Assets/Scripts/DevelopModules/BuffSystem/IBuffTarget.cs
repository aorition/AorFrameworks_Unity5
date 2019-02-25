public interface IBuffTarget
{
    /// <summary>
    /// 注册到管理器
    /// </summary>
    void RegisToManager();
    /// <summary>
    /// 从管理器取消祖册
    /// </summary>
    void UnregisFormManager();
    /// <summary>
    /// 刷新LocalBuff组
    /// </summary>
    /// <param name="deltaTime"></param>
    void UpdateLocalBuffs(float deltaTime);
    /// <summary>
    /// 关闭LocalBuff组
    /// </summary>
    void DisableLocalBuffs();
    /// <summary>
    /// 开启LocalBuff组
    /// </summary>
    void EnableLocalBuffs();
    /// <summary>
    /// 获取某个Buff
    /// </summary>
    /// <param name="BuffName"></param>
    /// <returns></returns>
    BuffComponent GetLocalBuff(string BuffName);
    /// <summary>
    /// 添加Buff
    /// </summary>
    /// <param name="buffComponent"></param>
    void AddLocalBuff(BuffComponent buffComponent);
    /// <summary>
    /// 移除Buff
    /// </summary>
    /// <param name="buffComponent"></param>
    void RemoveLocalBuff(BuffComponent buffComponent);
    
}