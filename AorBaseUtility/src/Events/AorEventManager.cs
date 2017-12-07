using System;
using System.Collections.Generic;

namespace AorBaseUtility
{
    
    public delegate void AEventAction(object target, params object[] args);

    public struct AEventInfo
    {
        public AEventInfo(AEventAction action, bool once)
        {
            this.Action = action;
            this.once = once;
        }

        public bool once;
        public AEventAction Action;
    }
    
    /// <summary>
    /// 基础的事件管理器
    /// </summary>
    public class AEventManager
    {

        #region static 方法集

        public static Action<Exception> onManagerError;

        public static void ThrowError(Exception error)
        {
            if (onManagerError != null)
            {
                onManagerError(error);
            }
        }

        #endregion

        private static AEventManager _instance;
        public static AEventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AEventManager();
                }
                return _instance;
            }
        }
        
        //事件池
        //<事件标识,<TypeName,>>
        private  readonly Dictionary<string, Dictionary<object, AEventInfo>> _eventInfoDic = new Dictionary<string, Dictionary<object, AEventInfo>>(); 

        private AEventManager()
        {
        }

        /// <summary>
        /// 销毁，回收处理
        /// </summary>
        public virtual void Dispose()
        {
            onManagerError = null;
            _eventInfoDic.Clear();
            if (_instance != null)
            {
                _instance = null;
            }
        }

        /// <summary>
        /// 添加一个事件监听
        /// </summary>
        /// <param name="target">监听对象</param>
        /// <param name="EventType">事件类型标识</param>
        /// <param name="action">事件触发回调</param>
        public virtual void AddEvent(object target, string EventType, AEventAction action)
        {
            if (_eventInfoDic.ContainsKey(EventType))
            {
                if (_eventInfoDic[EventType].ContainsKey(target))
                {
                    _eventInfoDic[EventType][target] = new AEventInfo(action, false);
                }
                else
                {
                    _eventInfoDic[EventType].Add(target, new AEventInfo(action, false));
                }
            }
            else
            {
                _eventInfoDic.Add(EventType, new Dictionary<object, AEventInfo>());
                _eventInfoDic[EventType].Add(target, new AEventInfo(action, false));
            }
        }

        /// <summary>
        /// 移除一个事件监听
        /// </summary>
        /// <param name="target">监听对象</param>
        /// <param name="EventType">事件类型标识</param>
        public virtual void RemoveEvent(object target, string EventType)
        {
            if (_eventInfoDic.ContainsKey(EventType))
            {
                if (_eventInfoDic[EventType].ContainsKey(target))
                {
                    _eventInfoDic[EventType].Remove(target);
                    if (_eventInfoDic[EventType].Count == 0)
                    {
                        _eventInfoDic.Remove(EventType);
                    }
                }
            }
        }

        /// <summary>
        /// 移除某个事件类型标识的监听
        /// </summary>
        /// <param name="EventType">事件类型标识</param>
        public virtual void RemoveEvent(string EventType)
        {
            if (_eventInfoDic.ContainsKey(EventType))
            {
                _eventInfoDic.Remove(EventType);
            }
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="target">监听对象</param>
        /// <param name="EventType">事件类型标识</param>
        /// <param name="args">事件参数传递</param>
        public virtual void DispathEvent(object target, string EventType, params object[] args)
        {
            if (_eventInfoDic.ContainsKey(EventType))
            {
                if (_eventInfoDic[EventType].ContainsKey(target))
                {

                    AEventInfo info = _eventInfoDic[EventType][target];
                    try
                    {
                        info.Action(target, args);
                    }
                    catch (Exception err1)
                    {
                        ThrowError(err1);
                    }


                    if (info.once)
                    {
                        _eventInfoDic[EventType].Remove(target);
                        if (_eventInfoDic[EventType].Count == 0)
                        {
                            _eventInfoDic.Remove(EventType);
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// 快速调用的转接类
    /// </summary>
    public class AEvents
    {

        /// <summary>
        /// 添加一个事件监听
        /// </summary>
        /// <param name="target">监听对象</param>
        /// <param name="EventType">事件类型标识</param>
        /// <param name="action">事件触发回调</param>
        public static void AddEvent(object target, string EventType, AEventAction action)
        {
            AEventManager.Instance.AddEvent(target, EventType, action);
        }

        /// <summary>
        /// 移除一个事件监听
        /// </summary>
        /// <param name="target">监听对象</param>
        /// <param name="EventType">事件类型标识</param>
        public static void RemoveEvent(object target, string EventType)
        {
            AEventManager.Instance.RemoveEvent(target, EventType);
        }

        /// <summary>
        /// 移除某个事件类型标识的监听
        /// </summary>
        /// <param name="EventType">事件类型标识</param>
        public static void RemoveEvent(string EventType)
        {
            AEventManager.Instance.RemoveEvent(EventType);
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="target">监听对象</param>
        /// <param name="EventType">事件类型标识</param>
        /// <param name="args">事件参数传递</param>
        public static void DispathEvent(object target, string EventType, params object[] args)
        {
            AEventManager.Instance.DispathEvent(target, EventType, args);
        }
    }

}
