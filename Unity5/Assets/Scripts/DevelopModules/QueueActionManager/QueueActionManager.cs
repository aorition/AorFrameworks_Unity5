using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 队列行为管理器
/// 
/// 提示: 原则上添加一个队列行为必须要保证在行为代码内有地方会调用到Next方法用以标识该行为完成,否则可能造成队列逻辑错误.
/// 
/// </summary>
public class QueueActionManager
{

    public static int QueueLimit = 10; //最大队列数,超过此队列数则会触发队列溢出错误机制

    private static Queue<QueueAction> _queueActions = new Queue<QueueAction>();

    private static bool _locked = false;
    public static void AddAction(string name,Action action)
    {
        AddAction(new QueueAction(name, action));
    }
    public static void AddAction(QueueAction qaction)
    {
        if (_locked)
        { 
            if(_queueActions.Count >= QueueLimit)
            {
                Debug.LogError("*** QueueActionManager.QueueOverLimit !! QueueActionManager will be reset !");
                //默认队列溢出清理行为:
                _queueActions.Clear();
                _currentAction = qaction;
                _currentAction.action();
                _locked = true;
            }
            else
            {
                _queueActions.Enqueue(qaction);
            }
        }
        else
        {
            _currentAction = qaction;
            _currentAction.action();
            _locked = true;
        }
    }

    private static QueueAction _currentAction;
    public static List<string> GetQueueNames() {
        List<string> names = new List<string>();
        if (_currentAction != null) {
            names.Add(_currentAction.name);
        }
        foreach (QueueAction item in _queueActions)
        {
            names.Add(item.name);
        }
        return names;
    }

    public static void Next() {
        if(_queueActions.Count > 0)
        {
            _currentAction = _queueActions.Dequeue();
            _currentAction.action();
        }
        else
        {
            _currentAction = null;
            _locked = false;
        }
    }

}


public class QueueAction {
    public string name;
    public Action action;

    public QueueAction(string name, Action action)
    {
        this.name = name;
        this.action = action;
    }

    ~QueueAction() {
        this.name = null;
        this.action = null;
    }

}
