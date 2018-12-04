using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{

    private static BuffManager _instance;
    public static BuffManager Instance
    {
        get { return _instance; }
    }

    public static bool HasInstance
    {
        get { return _instance != null; }
    }

    public static BuffManager CreateInstance(GameObject target, bool autoUpdteBuff = false)
    {
        if (!_instance)
        {
            if (!target) target = new GameObject("DelayActionManager");
            _instance = target.GetComponent<BuffManager>();
            if (!_instance) _instance = target.AddComponent<BuffManager>();
            _instance.Setup(autoUpdteBuff);
        }
        return _instance;
    }

    //-------------------------------------------------

    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            GameObject.Destroy(this);
        }
    }

    private bool _isInit = false;
    public bool isInit { get { return _isInit; } }
    public void Setup(bool autoUpdteBuff = false) {
        AutoUpdteBuff = autoUpdteBuff;
    }

    public bool AutoUpdteBuff = false;
    private void Update()
    {
        if (AutoUpdteBuff) UpdateAllBuff(Time.deltaTime);
    }

    private List<GlobalBuffComponent> _buffComponents = new List<GlobalBuffComponent>();
    private List<GlobalBuffComponent> _globalDelTmp = new List<GlobalBuffComponent>();
    public void AddGlobalBuffComponent(GlobalBuffComponent buff)
    {
        _buffComponents.Add(buff);
        buff.m_OnBuffAwake(_buffTargetList);
    }

    public void RemoveGlobalBuffComponent(GlobalBuffComponent buff)
    {
        _buffComponents.Remove(buff);
        if (buff.Survive > 0 && !buff.Dead)
        {
            buff.m_OnBuffRemoved(_buffTargetList);
        }
    }

    public GlobalBuffComponent GetGlobalBuff(string BuffName)
    {
        if (_buffComponents.Count > 0) return _buffComponents.Find(b => b.Name == BuffName);
        return null;
    }

    public List<GlobalBuffComponent> GetGlobalBuffList() {
        return _buffComponents;
    }

    private readonly List<IBuffTarget> _buffTargetList = new List<IBuffTarget>();
    public List<IBuffTarget> BuffTargetList { get { return _buffTargetList; } }

    public void RegistBuffTarget(IBuffTarget buffTarget) {
        if (!_buffTargetList.Contains(buffTarget)) _buffTargetList.Add(buffTarget);
    }

    public void UnregistBuffTarget(IBuffTarget buffTarget)
    {
        if (_buffTargetList.Contains(buffTarget)) _buffTargetList.Remove(buffTarget);
    }

    public void UpdateAllBuff(float deltaTime)
    {
        UpdateGlobalBuff(deltaTime);
        UpdateLocalBuff(deltaTime);
    }

    /// <summary>
    /// 更新全局buff
    /// </summary>
    public void UpdateGlobalBuff(float deltaTime)
    {
        for (int i = 0; i < _buffComponents.Count; i++)
        {
            GlobalBuffComponent bc = _buffComponents[i];
            if (bc.Enable)
            {
                bc.m_DoBuffEffect(deltaTime, _buffTargetList);
                if (bc.Dead) _globalDelTmp.Add(bc);
            }
        }
        if (_globalDelTmp.Count > 0)
        {
            for (int j = 0; j < _globalDelTmp.Count; j++)
            {
                _buffComponents.Remove(_globalDelTmp[j]);
                _globalDelTmp[j].m_OnBuffFinish(_buffTargetList);
            }
            _globalDelTmp.Clear();
        }
    }

    public void UpdateLocalBuff(float deltaTime) {
        for (int i = 0; i < _buffTargetList.Count; i++)
        {
            _buffTargetList[i].UpdateLocalBuffs(deltaTime);
        }
    }

}