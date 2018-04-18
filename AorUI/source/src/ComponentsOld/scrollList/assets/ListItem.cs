using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using ExoticUnity.GUI.AorUI.Core;
using ExoticUnity.GUI.AorUI.Debug;

namespace ExoticUnity.GUI.AorUI.Components.Assets
{

    public class ListItem : AorUIComponent
    {


        public class ListItemOnActivedUnityEvent : UnityEvent<ListItem, bool> { }

        //----------------------------------------------

        protected override void Initialization()
        {

            if (CustomInitListItem != null)
            {
                CustomInitListItem(this);
            }
            base.Initialization();
        }

        private bool _isActived = false; //是否是激活状态(表现形式可以为高亮)
        public bool isActived
        {
            get { return _isActived; }
            set
            {
                _isActived = value;
                if (_isStarted)
                {
                    setItemActived(_isActived);
                }
            }
        }

        //是否是激活(/高亮)状态的实现方法
        protected void setItemActived(bool b)
        {

            if (m_onActivedUnityEvent != null)
            {
                m_onActivedUnityEvent.Invoke(this, b);
            }

            //自定actived的实现
            if (CustomActiveChange != null)
            {
                CustomActiveChange(this);
                return;
            }

            //默认actived的实现
            Button btn = GetComponent<Button>();
            if (btn != null)
            {
                if (b)
                {
                    btn.interactable = false;
                }
                else
                {
                    btn.interactable = true;
                }
            }
        }

        //list Data
        protected object _data;
        //list id
        public int id;

        public object getRawData()
        {
            return _data;
        }

        public T getData<T>() where T : class
        {
            try
            {
                if (typeof(T) == typeof(string))
                {
                    return _data.ToString() as T;
                }

                return _data as T;
            }
            catch (Exception ex)
            {
                UiLog.Error("ListItem.getData Error : ", ex);
                return null;
            }
        }

        /// <summary>
        /// 设置ListItem的原始数据
        /// </summary>
        /// <param name="data"></param>
        public void setData(object data)
        {
            this._data = data;
            _isDirty = true;
        }

        protected override void DrawUI()
        {
            drawListItem();
            base.DrawUI();
        }

        //绘制ItemList,重写此方法实现不同的显示绘制
        protected virtual void drawListItem()
        {

            if (CustomDrawListItem != null)
            {
                CustomDrawListItem(this);
                return;
            }

            //默认绘制方法
            Text t = this.GetComponentInChildren<Text>();
            if (t != null)
            {
                t.text = _data.ToString();
            }

        }

        protected override void OnDestroy()
        {

            CustomInitListItem = null;
            CustomDrawListItem = null;
            CustomActiveChange = null;

            if (m_onActivedUnityEvent != null)
            {
                m_onActivedUnityEvent.RemoveAllListeners();
                m_onActivedUnityEvent = null;
            }

            base.OnDestroy();
        }

        //---------------------------------------- 接口 / 事件定义

        /// <summary>
        /// <接口> 自定义初始化实现
        /// </summary>
        public Action<ListItem> CustomInitListItem;

        /// <summary>
        /// <接口> 自定义绘制实现 
        /// 
        /// </summary>
        public Action<ListItem> CustomDrawListItem;

        /// <summary>
        /// <接口> 自定义Actived实现
        /// </summary>
        public Action<ListItem> CustomActiveChange;

        /// <summary>
        /// <事件> isActived属性改变时触发
        /// </summary>
        [SerializeField]
        private ListItemOnActivedUnityEvent m_onActivedUnityEvent = new ListItemOnActivedUnityEvent();
        public ListItemOnActivedUnityEvent onActived
        {
            get { return m_onActivedUnityEvent; }
            set { m_onActivedUnityEvent = value; }
        }
    }
}
