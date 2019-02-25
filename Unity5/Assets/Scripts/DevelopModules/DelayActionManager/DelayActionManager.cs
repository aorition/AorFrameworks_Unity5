using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 延迟执行管理器
/// </summary>
public class DelayActionManager : MonoBehaviour
{
    private static DelayActionManager _instance;
    public static DelayActionManager Instance {
        get { return _instance; }
    }

    public static bool HasInstance {
        get { return _instance != null; }
    }

    public static DelayActionManager CreateInstance(GameObject target) {
        if (!_instance)
        {
            if (!target) target = new GameObject("DelayActionManager");
            _instance = target.GetComponent<DelayActionManager>();
            if (!_instance) _instance = target.AddComponent<DelayActionManager>();
        }
        return _instance;
    }

    //-------------------------------------------------

    private void Awake()
    {
        if (!_instance) {
            _instance = this;
        }else if(_instance != this)
        {
            GameObject.Destroy(this);
        }
    }
    
    public void AddDelayAction(DelayActionBase action) {
        if (action == null) return;
        _delayActions.Add(action);
        action.Init();
    }

    public void RemoveDelayActionByHash(int hash) {
        DelayActionBase ac = _delayActions.Find(d => d.hash == hash);
        if(ac != null) _delayActions.Remove(ac);
    }

    public void AddLoopAction(DelayActionBase action)
    {
        if (action == null) return;
        _loopActions.Add(action);
        action.Init();
    }

    public void RemoveLoopActionByHash(int hash)
    {
        DelayActionBase ac = _loopActions.Find(d => d.hash == hash);
        if (ac != null) _loopActions.Remove(ac);
    }

    private readonly List<DelayActionBase> _delayActions = new List<DelayActionBase>();
    private readonly List<DelayActionBase> _delTmp = new List<DelayActionBase>();
    private readonly List<DelayActionBase> _loopActions = new List<DelayActionBase>();

    private void Update()
    {
        if(_delayActions.Count > 0)
        {
            for (int i = 0; i < _delayActions.Count; i++)
            {
                if (_delayActions[i].dead)
                    _delTmp.Add(_delayActions[i]);
                else
                    _delayActions[i].Update();
            
            }
            if(_delTmp.Count > 0)
            {
                for (int j = 0; j < _delTmp.Count; j++)
                {
                    _delayActions.Remove(_delTmp[j]);
                }
                _delTmp.Clear();
            }

        }

        if (_loopActions.Count > 0)
        {
            for (int i = 0; i < _loopActions.Count; i++)
            {
                _loopActions[i].Update();
            }
        }
    }

}