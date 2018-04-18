using System;
using System.Collections;
using UnityEngine.EventSystems;

namespace Framework.UI
{
    /// <summary>
    /// AorUI组件基类
    /// AorUIComponent负责组织显示逻辑,提供统一的Dirty标记和刷新逻辑.
    /// 
    /// @ 请使用Initialization替代Start方法
    /// @ 请使用OnUpdate替代Update方法
    /// 
    /// </summary>
    public class AorUIComponent : UIBehaviour
    {

        /// <summary>
        /// 更新标记
        /// </summary>
        protected bool _isDirty;

        /// <summary>
        /// initialization的完毕标记.
        /// </summary>
        protected bool _isInit;

        /// <summary>
        /// DrawUI方法的完成标记.
        /// 
        /// </summary>
//        protected bool _isDrawUIDone;

        /// <summary>
        /// **** 绘制/更新UI显示的核心方法
        /// </summary>
        protected virtual void DrawUI()
        {

        }

        protected override void Awake()
        {
            base.Awake();
            OnAwake();
        }
        /// <summary>
        /// Awake阶段调用
        /// </summary>
        protected virtual void OnAwake()
        {

        }

        /// <summary>
        /// 请使用Initialization替代Start方法
        /// </summary>
        protected sealed override void Start()
        {
            base.Start();
            _checkInit();
        }

        /// <summary>
        /// 初始化检查,如果_isInit标记为false则执行Initialization;
        /// </summary>
        protected void _checkInit()
        {
            if (!_isInit)
            {
                _isInit = Initialization(); ;
            }
        }

        /// <summary>
        /// 初始化方法 (Start阶段)
        /// </summary>
        /// <returns>是否初始化成功</returns>
        protected virtual bool Initialization()
        {
            //通常在初始化中会标记_isDirty=true来触发DrawUI方法来绘制UI
            _isDirty = true; 
            return true;
        }

        private void Update()
        {

            if (_isDirty)
            {
//                _isDrawUIDone = false; //标记开始绘制UI
                DrawUI();
//                _isDrawUIDone = true;
                _isDirty = false;
            }

            OnUpdate();
        }

        /// <summary>
        /// 替代Update方法,每帧调用
        /// </summary>
        protected virtual void OnUpdate()
        {
            //
        }

    }
}
