using System.Collections.Generic;

public class BuffTarget : IBuffTarget
{

    public void RegisToManager() {
        BuffManager.Instance.RegistBuffTarget(this);
    }

    public void UnregisFormManager() {
        BuffManager.Instance.UnregistBuffTarget(this);
    }

    private List<BuffComponent> _localBuffs = new List<BuffComponent>();
    private List<BuffComponent> _localDelTmp = new List<BuffComponent>();
    public void UpdateLocalBuffs(float deltaTime)
    {
        for (int i = 0; i < _localBuffs.Count; i++)
        {
            BuffComponent bc = _localBuffs[i];
            if (bc.Enable)
            {
                bc.m_DoBuffEffect(deltaTime, this);
                if (bc.Dead) _localDelTmp.Add(bc);
            }
        }
        if (_localDelTmp.Count > 0)
        {
            for (int j = 0; j < _localDelTmp.Count; j++)
            {
                _localBuffs.Remove(_localDelTmp[j]);
                _localDelTmp[j].m_OnBuffFinish(this);
            }
            _localDelTmp.Clear();
        }
    }

    public void DisableLocalBuffs()
    {
        for (int i = 0; i < _localBuffs.Count; i++)
        {
            BuffComponent bc = _localBuffs[i];
            bc.Enable = false;
        }
    }
    public void EnableLocalBuffs()
    {
        for (int i = 0; i < _localBuffs.Count; i++)
        {
            BuffComponent bc = _localBuffs[i];
            bc.Enable = true;
        }
    }

    public BuffComponent GetLocalBuff(string BuffName)
    {
        if (_localBuffs.Count > 0)
        {
            return _localBuffs.Find(b => b.Name == BuffName);
        }
        return null;
    }

    public void AddLocalBuff(BuffComponent buffComponent)
    {
        _localBuffs.Add(buffComponent);
        buffComponent.m_OnBuffAwake(this);
    }
    public void RemoveLocalBuff(BuffComponent buffComponent)
    {
        _localBuffs.Remove(buffComponent);
        if (buffComponent.Survive > 0 && !buffComponent.Dead)
        {
            buffComponent.m_OnBuffRemoved(this);
        }
    }

}