using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Framework.UI
{
    /// <summary>
    /// AorUI事件监听器(所有需要事件响应的UI对象都应该挂载此组件)
    /// </summary>
    public class AorUIEventListener : AorUIComponent, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler/*,IDragHandler */
    {
        [Serializable]
        public class AorUIEventListenerEvent : UnityEvent<GameObject, PointerEventData> {}

        /// <summary>
        /// 获取该AorUIEventListener对象的上一级AorUIEventListener对象(静态)
        /// </summary>
        /// <param name="target">Transform</param>
        /// <returns>AorUIEventListener</returns>
        public static AorUIEventListener getParentEventListener(Transform target) {
            if (target.parent != null) {
                return target.parent.GetComponentInParent<AorUIEventListener> ();
            }
            return null;
        }
        
        /// <summary>
        /// 配置数据
        /// </summary>
        public AorUIEventListenerSettingAsset Setting;

        /// <summary>
        /// 是否监听 Click 事件
        /// 
        /// </summary>
        public bool Click = true;
        /// <summary>
        /// 是否监听 LongPress 事件 (长按)
        /// </summary>
        public bool LongPress = false;
        /// <summary>
        /// 是否监听 Down 事件 (按下)
        /// </summary>
        public bool Down = false;
        /// <summary>
        /// 是否监听 Up 事件 (抬起)
        /// </summary>
        public bool Up = false;
        /// <summary>
        /// 是否监听 Over 事件 (移入,通常此事件用于鼠标)
        /// </summary>
        public bool Over = false;
        /// <summary>
        /// 是否监听 Out 事件 (移出,通常此事件用于鼠标)
        /// </summary>
        public bool Out = false;
        /// <summary>
        /// 是否监听 Drag 事件 (拖动)
        /// </summary>
        public bool Drag = false;
        /// /// <summary>
        /// 是否监听 Swing 事件 (滑动/快速甩动)
        /// </summary>
        public bool Swing = false;

        /// <summary>
        /// Click事件 委托
        /// AorUI事件监听器可针对某个监听对象挂载事件委托方法来处理特殊的事件响应.
        /// 挂载委托后,AorUI事件监听器仍会通知全局事件逻辑IAorUILogicInterface,请妥善处理两者的逻辑关系.
        /// </summary>
        [SerializeField]
        private AorUIEventListenerEvent m_onEventClick = new AorUIEventListenerEvent ();
        public AorUIEventListenerEvent onEventClick
        {
            get
            {
                return m_onEventClick;
            }
            set
            {
                m_onEventClick = value;
            }
        }

        /// <summary>
        /// LongPress事件 委托
        /// AorUI事件监听器可针对某个监听对象挂载事件委托方法来处理特殊的事件响应.
        /// 挂载委托后,AorUI事件监听器仍会通知全局事件逻辑IAorUILogicInterface,请妥善处理两者的逻辑关系.
        /// </summary>
        [SerializeField]
        private AorUIEventListenerEvent m_onEventLongPress = new AorUIEventListenerEvent ();
        public AorUIEventListenerEvent onEventLongPress
        {
            get
            {
                return m_onEventLongPress;
            }
            set
            {
                m_onEventLongPress = value;
            }
        }

        /// <summary>
        /// Down事件 委托
        /// AorUI事件监听器可针对某个监听对象挂载事件委托方法来处理特殊的事件响应.
        /// 挂载委托后,AorUI事件监听器仍会通知全局事件逻辑IAorUILogicInterface,请妥善处理两者的逻辑关系.
        /// </summary>
        [SerializeField]
        private AorUIEventListenerEvent m_onEventDown = new AorUIEventListenerEvent();
        public AorUIEventListenerEvent onEventDown{
            get { return m_onEventDown; }
            set { m_onEventDown = value; }
        }

        /// <summary>
        /// Up事件 委托
        /// AorUI事件监听器可针对某个监听对象挂载事件委托方法来处理特殊的事件响应.
        /// 挂载委托后,AorUI事件监听器仍会通知全局事件逻辑IAorUILogicInterface,请妥善处理两者的逻辑关系.
        /// </summary>
        [SerializeField]
        private AorUIEventListenerEvent m_onEventUp = new AorUIEventListenerEvent ();
        public AorUIEventListenerEvent onEventUp
        {
            get
            {
                return m_onEventUp;
            }
            set
            {
                m_onEventUp = value;
            }
        }

        /// <summary>
        /// Over事件 委托
        /// AorUI事件监听器可针对某个监听对象挂载事件委托方法来处理特殊的事件响应.
        /// 挂载委托后,AorUI事件监听器仍会通知全局事件逻辑IAorUILogicInterface,请妥善处理两者的逻辑关系.
        /// </summary>
        [SerializeField]
        private AorUIEventListenerEvent m_onEventOver = new AorUIEventListenerEvent ();
        public AorUIEventListenerEvent onEventOver
        {
            get
            {
                return m_onEventOver;
            }
            set
            {
                m_onEventOver = value;
            }
        }
        /// <summary>
        /// Out事件 委托
        /// AorUI事件监听器可针对某个监听对象挂载事件委托方法来处理特殊的事件响应.
        /// 挂载委托后,AorUI事件监听器仍会通知全局事件逻辑IAorUILogicInterface,请妥善处理两者的逻辑关系.
        /// </summary>
        [SerializeField]
        private AorUIEventListenerEvent m_onEventOut = new AorUIEventListenerEvent ();
        public AorUIEventListenerEvent onEventOut
        {
            get
            {
                return m_onEventOut;
            }
            set
            {
                m_onEventOut = value;
            }
        }

        /// <summary>
        /// Drag事件 委托
        /// AorUI事件监听器可针对某个监听对象挂载事件委托方法来处理特殊的事件响应.
        /// 挂载委托后,AorUI事件监听器仍会通知全局事件逻辑IAorUILogicInterface,请妥善处理两者的逻辑关系.
        /// </summary>
        [SerializeField]
        private AorUIEventListenerEvent m_onEventDrag = new AorUIEventListenerEvent ();
        public AorUIEventListenerEvent onEventDrag
        {
            get
            {
                return m_onEventDrag;
            }
            set
            {
                m_onEventDrag = value;
            }
        }
        /// <summary>
        /// Swing事件 委托
        /// AorUI事件监听器可针对某个监听对象挂载事件委托方法来处理特殊的事件响应.
        /// 挂载委托后,AorUI事件监听器仍会通知全局事件逻辑IAorUILogicInterface,请妥善处理两者的逻辑关系.
        /// </summary>
        [SerializeField]
        private AorUIEventListenerEvent m_onEventSwing = new AorUIEventListenerEvent ();
        public AorUIEventListenerEvent onEventSwing
        {
            get
            {
                return m_onEventSwing;
            }
            set
            {
                m_onEventSwing = value;
            }
        }

        private AorUIEventListener _parentEventListener;
        /// <summary>
        /// 上一级AorUIEventListener对象的实例引用
        /// </summary>
        public AorUIEventListener parentEventListener
        {
            get
            {
                return _parentEventListener;
            }
            set
            {
                _parentEventListener = value;
            }
        }

        protected override void OnAwake() {
            base.OnAwake ();

            if (m_onEventClick == null) {
                m_onEventClick = new AorUIEventListenerEvent ();
            }

            if (m_onEventDown == null) {
                m_onEventDown = new AorUIEventListenerEvent ();
            }

            if (m_onEventDrag == null) {
                m_onEventDrag = new AorUIEventListenerEvent ();
            }

            if (m_onEventLongPress == null) {
                m_onEventLongPress = new AorUIEventListenerEvent ();
            }

            if (m_onEventOut == null) {
                m_onEventOut = new AorUIEventListenerEvent ();
            }

            if (m_onEventOver == null) {
                m_onEventOver = new AorUIEventListenerEvent ();
            }

            if (m_onEventSwing == null) {
                m_onEventSwing = new AorUIEventListenerEvent ();
            }

            if (m_onEventUp == null) {
                m_onEventUp = new AorUIEventListenerEvent ();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_isInit)
            {
                StartCoroutine(Processing());
            }
        }

        // Use this for initialization
        protected override bool Initialization()
        {
            if (!Setting)
            {
                Setting = AorUIEventListenerSettingAsset.Default();
            }
            _parentEventListener = AorUIEventListener.getParentEventListener (this.transform);
            StartCoroutine(Processing());
            return true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (m_onEventSwing != null) {
                m_onEventSwing.RemoveAllListeners ();
                m_onEventSwing = null;
            }

            if (m_onEventDrag != null) {
                m_onEventDrag.RemoveAllListeners ();
                m_onEventDrag = null;
            }

            if (m_onEventOver != null) {
                m_onEventOver.RemoveAllListeners ();
                m_onEventOver = null;
            }
            
            if (m_onEventOut != null) {
                m_onEventOut.RemoveAllListeners ();
                m_onEventOut = null;
            }

            if (m_onEventClick != null) {
                m_onEventClick.RemoveAllListeners ();
                m_onEventClick = null;
            }
               
            if (m_onEventLongPress != null) {
                m_onEventLongPress.RemoveAllListeners ();
                m_onEventLongPress = null;
            }

            if (m_onEventDown != null) {
                m_onEventDown.RemoveAllListeners ();
                m_onEventDown = null;
            }

            if (m_onEventUp != null) {
                m_onEventUp.RemoveAllListeners ();
                m_onEventUp = null;
            }
        }

        private IEnumerator Processing()
        {
            while (true)
            {
                if (_isDown)
                {

                    _downValue += (Time.unscaledTime - _timeCache)*1000;
                    _timeCache = Time.unscaledTime;

                    //Drag
                    if ((Vector2.Distance(_currentEventData.position, _currentEventData.pressPosition) > Setting.ClickDistanceThreshold) || _isDraged)
                    {
                        _isDraged = true;
                        if (Drag)
                        {
                            if (onEventDrag != null && onEventDrag.GetPersistentEventCount() > 0)
                            {
                                onEventDrag.Invoke(gameObject, _currentEventData);
                            }
                        }
                        else
                        {
                            passEventDrag(_parentEventListener, gameObject, _currentEventData);
                        }
                    }
                    if (_downValue > Setting.LongPressTimeThreshold)
                    {
                        if (!_isLongPressed)
                        {
                            _isLongPressed = true;
                            if ((Vector2.Distance(_currentEventData.position, _currentEventData.pressPosition) < Setting.LongPressDistanceThreshold))
                            {
                                //LongPress
                                if (LongPress)
                                {
                                    if (onEventLongPress != null && onEventLongPress.GetPersistentEventCount() > 0)
                                    {
                                        onEventLongPress.Invoke(gameObject, _currentEventData);
                                    }
                                }
                                else
                                {
                                    passEventLongPress(_parentEventListener, gameObject, _currentEventData);
                                }
                            }
                        }
                    }
                }

                yield return new WaitForSecondsRealtime(1f/Setting.FPS);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private float _timeCache;
        private bool _isDraged = false;
        private bool _isLongPressed = false;
        private bool _isDown = false;
        private float _downValue;
        private PointerEventData _currentEventData;
        /// <summary>
        /// 原生EventSystem.Down事件处理接口实现 (若非强行事件注入,程序员通常不需要调用此方法)
        /// </summary>
        /// <param name="eventData">PointerEventData</param>
        public void OnPointerDown(PointerEventData eventData) {

            _currentEventData = eventData;
            _downValue = 0;
            _isDraged = false;
            //_isProDrag = false;
            _isLongPressed = false;

            _isDown = true;
            _timeCache = Time.unscaledTime;
            if (Down)
            {
                if (onEventDown != null && onEventDown.GetPersistentEventCount() > 0)
                {
                    onEventDown.Invoke(gameObject, eventData);
                }
            }
            else
            {
                passEventDown(_parentEventListener, gameObject, eventData);
            }
        }
        /// <summary>
        /// 原生EventSystem.Up事件处理接口实现 (若非强行事件注入,程序员通常不需要调用此方法)
        /// </summary>
        /// <param name="eventData">PointerEventData</param>
        public void OnPointerUp(PointerEventData eventData) {
            _isDown = false;
            //Up
            if (Up)
            {
                if (onEventUp != null && onEventUp.GetPersistentEventCount() > 0)
                {
                    onEventUp.Invoke(gameObject, eventData);
                }
            }
            else
            {
                passEventUp(_parentEventListener, gameObject, eventData);
            }
            //Swing
            if (Vector2.Distance(eventData.position, eventData.pressPosition) > Setting.SwingDistanceThreshold && _downValue < Setting.SwingTimeThreshold) {
                if (Swing)
                {
                    if (onEventSwing != null && onEventSwing.GetPersistentEventCount() > 0)
                    {
                        onEventSwing.Invoke(gameObject, eventData);
                    }
                }
                else
                {
                    passEventSwing(_parentEventListener, gameObject, eventData);
                }
            }
        }

        public void OnPointerClick ( PointerEventData eventData )
        {
            //Click
            if (_downValue < Setting.ClickTimeThreshold && Vector2.Distance (eventData.position, eventData.pressPosition) < Setting.ClickDistanceThreshold)
            {
                if (Click)
                {
                    if (onEventClick != null && onEventClick.GetPersistentEventCount() > 0)
                    {
                        onEventClick.Invoke(gameObject, eventData);
                    }
                }
                else
                {
                    passEventClick(_parentEventListener, gameObject, eventData);
                }
            }
        }
        /// <summary>
        /// 原生EventSystem.Enter事件处理接口实现 (若非强行事件注入,程序员通常不需要调用此方法)
        /// </summary>
        /// <param name="eventData">PointerEventData</param>
        public void OnPointerEnter(PointerEventData eventData) {
            if (Over)
            {
                if (onEventOver != null && onEventOver.GetPersistentEventCount() > 0)
                {
                    onEventOver.Invoke(this.gameObject, eventData);
                }
            }
            else
            {
                passEventOver(_parentEventListener, gameObject, eventData);
            }
        }
        /// <summary>
        /// 原生EventSystem.Exit事件处理接口实现 (若非强行事件注入,程序员通常不需要调用此方法)
        /// </summary>
        /// <param name="eventData">PointerEventData</param>
        public void OnPointerExit(PointerEventData eventData) {
            if (Out)
            {
                if (onEventOut != null && onEventOut.GetPersistentEventCount() > 0)
                {
                    onEventOut.Invoke(this.gameObject, eventData);
                }
            }
            else
            {
                passEventOut(_parentEventListener, gameObject, eventData);
            }
        }
        /*
        private bool _isProDrag = false;
        public void OnDrag(PointerEventData eventData) {
            _isProDrag = true;
        }*/

        ///----------------------------------------------------- passEvent 事件传递 方法组;
        public void passEventClick(AorUIEventListener p, GameObject go, PointerEventData eventData) {
            if (p != null) {
                if (p.Click)
                {
                    if (p.onEventClick != null && p.onEventClick.GetPersistentEventCount() > 0)
                    {
                        p.onEventClick.Invoke(go, eventData);
                    }
                }
                else
                {
                    passEventClick(p.parentEventListener, go, eventData);
                }
            }
        }
        public void passEventLongPress(AorUIEventListener p, GameObject go, PointerEventData eventData) {
            if (p != null) {
                if (p.LongPress)
                {
                    if (p.onEventLongPress != null && p.onEventLongPress.GetPersistentEventCount() > 0)
                    {
                        p.onEventLongPress.Invoke(go, eventData);
                    }
                }
                else
                {
                    passEventLongPress(p.parentEventListener, go, eventData);
                }
            }
        }
        public void passEventDown(AorUIEventListener p, GameObject go, PointerEventData eventData) {
            if (p != null) {
                if (p.Down)
                {
                    if (p.onEventDown != null && p.onEventDown.GetPersistentEventCount() > 0)
                    {
                        p.onEventDown.Invoke(p.gameObject, eventData);
                    }
                }
                else
                {
                    passEventDown(p.parentEventListener, go, eventData);
                }
            }
        }
        public void passEventUp(AorUIEventListener p, GameObject go, PointerEventData eventData) {
            if (p != null) {
                if (p.Up)
                {
                    if (p.onEventUp != null && p.onEventUp.GetPersistentEventCount() > 0)
                    {
                        p.onEventUp.Invoke(go, eventData);
                    }
                }
                else
                {
                    passEventUp(p.parentEventListener, go, eventData);
                }
            }
        }
        public void passEventOver(AorUIEventListener p, GameObject go, PointerEventData eventData) {
            if (p != null) {
                if (p.Over)
                {
                    if (p.onEventOver != null && p.onEventOver.GetPersistentEventCount() > 0)
                    {
                        p.onEventOver.Invoke(go, eventData);
                    }
                }
                else
                {
                    passEventOver(p.parentEventListener, go, eventData);
                }
            }
        }
        public void passEventOut(AorUIEventListener p, GameObject go, PointerEventData eventData) {
            if (p != null) {
                if (p.Out)
                {
                    if (p.onEventOut != null && p.onEventOut.GetPersistentEventCount() > 0)
                    {
                        p.onEventOut.Invoke(go, eventData);
                    }
                }
                else
                {
                    passEventOut(p.parentEventListener, go, eventData);
                }
            }
        }
        public void passEventDrag(AorUIEventListener p, GameObject go, PointerEventData eventData) {
            if (p != null) {
                if (p.Drag)
                {
                    if (p.onEventDrag != null && p.onEventDrag.GetPersistentEventCount() > 0)
                    {
                        p.onEventDrag.Invoke(go, eventData);
                    }
                }
                else
                {
                    passEventDrag(p.parentEventListener, go, eventData);
                }
            }
        }
        public void passEventSwing(AorUIEventListener p, GameObject go, PointerEventData eventData) {
            if (p != null) {
                if (p.Swing)
                {
                    if (p.onEventSwing != null && p.onEventSwing.GetPersistentEventCount() > 0)
                    {
                        p.onEventSwing.Invoke(go, eventData);
                    }
                }
                else
                {
                    passEventSwing(p.parentEventListener, go, eventData);
                }
            }
        }

        //已通过方式实现Drag,并未实现 IDragHandler 接口
        //        public void OnDrag ( PointerEventData eventData )
        //        {
        //            if (Drag && _isDraged)
        //            {
        //                if (onEventDrag != null)
        //                {
        //                    onEventDrag.Invoke (gameObject, _currentEventData);
        //                }
        //            }
        //            else
        //            {
        //                passEventDrag (_parentEventListener, _currentEventData);
        //            }
        //        }

    }
}