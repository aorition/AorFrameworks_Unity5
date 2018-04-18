using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using DG.Tweening;

using System;
using System.Collections.Generic;

using ExoticUnity.App;
using ExoticUnity.GUI.AorUI.Animtion;
using ExoticUnity.GUI.AorUI.Components.Assets;
using ExoticUnity.GUI.AorUI.Core;
using ExoticUnity.GUI.AorUI.Debug;
using ExoticUnity.GUI.AorUI.events;

namespace ExoticUnity.GUI.AorUI.Components
{
    public class ComboBox : AorUIComponent
    {

        [Serializable]
        public class CombboBoxEvent : UnityEvent { }

        [Serializable]
        public class ComboBoxSelectChangedEvent : UnityEvent<int, object> { }

        [Serializable]
        public class CombboBoxClickEvent : UnityEvent<GameObject, PointerEventData> { }

        //---------------------------------------

        /// <summary>
        /// 当前选择的对象id
        /// </summary>
        public int selectedID = 0;

        /// <summary>
        /// 是否一直处于开启状态
        /// </summary>
        public bool AlwaysOpend = false;

        [SerializeField]
        protected float _listBackgroundHeight = -1f;

        /// <summary>
        /// 定义列表背景的高度.如果保持默认值为-1,即使用组件的列表背景对象的实际高度值,否则视为指定.
        /// </summary>
        public float ListBackgroundHeight {
            get { return _listBackgroundHeight; }
            set {
                if (value >= 0 && value != _listBackgroundHeight) {
                    _listBackgroundHeight = value;
                    _isDirty = true;
                }
            }
        }

        [SerializeField]
        private float _listPosOffset = 5f;
        /// <summary>
        /// 列表位置偏移,(默认偏移5个像素)
        /// </summary>
        public float ListPosOffset {
            get { return _listPosOffset; }
            set {
                if (value >= 0 && value != _listPosOffset) {
                    _listPosOffset = value;
                    _isDirty = true;
                }
            }
        }

        //------------------------------------ 子组件

        /// <summary>
        /// 挂载依赖的子组件(_scrollList);
        /// </summary>
        public RectTransform ScrollList;
        private ScrollList _scrollList;

        public RectTransform Background;

        public RectTransform SelectedItemPos;

        public RectTransform ScrollListBackground;

        public RectTransform InteractiveButton;

        private Button _ItBtn;
        //------------------------------------ 子组件 end
        
        //----------------------------- Event interface

        /// <summary>
        /// <接口> 自定义绘制ListItem的实现
        /// </summary>
        private Action<ListItem> m_CustomDrawListItem;
        public Action<ListItem> CustomDrawListItem {
            get {
              return  m_CustomDrawListItem;
            }
            set {
                m_CustomDrawListItem = value;
                if (_scrollList != null) {
                    _scrollList.CustomDrawListItem = value;
                }
            }
        }

        /// <summary>
        /// <事件> 当选择对象发生改变时触发事件委托
        /// </summary>
        [SerializeField]
        private ComboBoxSelectChangedEvent m_onSelectChanged = new ComboBoxSelectChangedEvent();
        public ComboBoxSelectChangedEvent onSelectChanged {
            get { return m_onSelectChanged; }
            set { m_onSelectChanged = value; }
        }

        /// <summary>
        ///<事件> 当ComboBox被点击(不包含下拉列表区域)触发事件委托
        /// </summary>
        [SerializeField]
        private CombboBoxClickEvent m_onComboBoxClicked = new CombboBoxClickEvent();
        public CombboBoxClickEvent onComboBoxClicked {
            get { return m_onComboBoxClicked; }
            set { m_onComboBoxClicked = value; }
        }

        /// <summary>
        /// <事件> 当ComboBox的下拉列表被打开时触发事件委托
        /// </summary>
        [SerializeField]
        private CombboBoxEvent m_onComboBoxOpen = new CombboBoxEvent();
        public CombboBoxEvent onComboBoxOpen {
            get { return m_onComboBoxOpen; }
            set { m_onComboBoxOpen = value; }
        }


        /// <summary>
        /// <事件> 当ComboBox的下拉列表被关闭时触发事件委托
        /// </summary>
        [SerializeField]
        private CombboBoxEvent m_onComboBoxClose = new CombboBoxEvent();
        public CombboBoxEvent onComboBoxClose {
            get { return m_onComboBoxClose; }
            set { m_onComboBoxClose = value; }
        }

        /// <summary>
        ///<接口> 可以通过此委托,重写ComboBox的打开下拉列表的动画;
        /// </summary>
        public Action<ComboBox> CustomOpenAnimation;

        /// <summary>
        ///<接口> 可以通过此委托,重写ComboBox的关闭下拉列表的动画;
        /// </summary>
        public Action<ComboBox> CustomCloseAnimation;
        
        private ListItem _selectItem;
        public ListItem SelectItem {
            get { return _selectItem; }
        }

        private float _baseItemHeight;
        private float _baseListBackgroundHeight;

        public object[] GetAllListData() {
            if (_scrollList != null) {
                return _scrollList.GetAllListData();
            }
            return null;
        }

//        public void AddListData(object data) {
//            if (_scrollList != null) {
//                _scrollList.AddListData(data);
//            }
//            _isDirty = true;
//        }
        public void AddListDataRange(IEnumerable<object> colletion) {
            if (_scrollList != null) {
                _scrollList.AddListDataRange(colletion);
            }
            _isDirty = true;
        }
//        public void RemoveListData(object data) {
//            if (_scrollList != null) {
//                _scrollList.RemoveListData(data);
//            }
//            _isDirty = true;
//        }
        public void RemoveListDataRange(int count) {
            if (_scrollList != null) {
//                _scrollList.RemoveListDataRange(index,count);
                _scrollList.RemoveListDataRange(count);
            }
            _isDirty = true;
        }
//        public void RemoveListData(int index) {
//            if (_scrollList != null) {
//                _scrollList.RemoveListData(index);
//            }
//            _isDirty = true;
//        }

        protected override void OnUpdate() {

            bool started = _scrollList.isStarted;
            bool dirty = _isDirty;

            if (started && dirty) {
                DrawUI();
            }

            /*
            if (_scrollList._isStarted && _isDirty) {
                DrawUI();
            }*/
        }

        public override void OnAwake() {
            base.OnAwake();

            if (Background == null) {
                Background = this.transform.FindChild("Background#").GetComponent<RectTransform>();
            }
            if (InteractiveButton == null) {
                InteractiveButton = this.transform.FindChild("InteractiveBtn#").GetComponent<RectTransform>();
            }

            _ItBtn = InteractiveButton.GetComponent<Button>();

            if (ScrollListBackground == null) {
                ScrollListBackground = transform.FindChild("ScrollListBackground#").GetComponent<RectTransform>();
            }
            if (SelectedItemPos == null) {
                SelectedItemPos = this.transform.FindChild("SelectItemPos#").GetComponent<RectTransform>();
            }
            if (ScrollList == null) {
                ScrollList = transform.FindChild("ScrollList#").GetComponent<RectTransform>();
            }

            _scrollList = ScrollList.GetComponent<ScrollList>();
        }


        protected override void Initialization()
        {

            //这里必须要保证_scorlllist是激活的,因为它不激活它就不会初始化 = =b
            if (!_scrollList.gameObject.activeInHierarchy)
            {
                _scrollList.gameObject.SetActive(true);
            }

            if (_scrollList.TemplateObject == null) {
                Transform it = _scrollList.transform.GetChild(0).GetChild(0);
                if (it != null) {
                    _baseItemHeight = it.GetComponent<RectTransform>().rect.height;
                }
                else {
                    _baseItemHeight = 50f;
                }
            }
            else {
                _baseItemHeight = (_scrollList.TemplateObject as GameObject).GetComponent<RectTransform>().rect.height;
            }
            _baseListBackgroundHeight = ScrollListBackground.rect.height;

            base.Initialization();

            if (GetAllListData() != null)
            {
                DrawUI();
            }
        }

        protected override void OnDestroy() {

            m_CustomDrawListItem = null;
            CustomOpenAnimation = null;
            CustomCloseAnimation = null;

            if (m_onSelectChanged != null) {
                m_onSelectChanged.RemoveAllListeners();
                m_onSelectChanged = null;
            }

            if (m_onComboBoxClicked != null) {
                m_onComboBoxClicked.RemoveAllListeners();
                m_onComboBoxClicked = null;
            }

            if (m_onComboBoxOpen != null) {
                m_onComboBoxOpen.RemoveAllListeners();
                m_onComboBoxOpen = null;
            }

            if (m_onComboBoxClose != null) {
                m_onComboBoxClose.RemoveAllListeners();
                m_onComboBoxClose = null;
            }
                
            Background = null;
            //   _selectedData = null;
            _scrollList = null;
            _ItBtn = null;
            _scrollList = null;

            base.OnDestroy();
        }

        protected override void DrawUI() {
            

            if (_scrollList != null && CustomDrawListItem != null) {
                _scrollList.CustomDrawListItem = CustomDrawListItem;
            }

            //初始化已选择项目
            GameObject selectedItem = _scrollList.getListItemById(selectedID);
            if (selectedItem == null) {
                _isDirty = true;
                return;
            }

            this._scrollList.GetComponent<RectTransform>().anchoredPosition = new Vector2(this._scrollList.GetComponent<RectTransform>().anchoredPosition.x, -_listPosOffset);

            if (_listBackgroundHeight >= 0) {
                this.ScrollListBackground.sizeDelta = new Vector2(this.ScrollListBackground.sizeDelta.x, _listBackgroundHeight);
            }
            else {
                _listBackgroundHeight = this.ScrollListBackground.sizeDelta.y;
            }

            _scrollList.onListItemClicked = onListEvent;

            
            setSelected(selectedItem);
            if (!AlwaysOpend)
            {
                
                _scrollList.gameObject.SetActive(false);
                ScrollListBackground.gameObject.SetActive(false);

                AorUIEventListener aue = _ItBtn.GetComponent<AorUIEventListener>();
                if (aue != null && m_onComboBoxClicked != null) {
                    aue.onEventClick.AddListener((arg0, data) => {
                        m_onComboBoxClicked.Invoke(arg0, data);
                    });
                }
            }
            else
            {
                _isOpen = true;
            }

            _ItBtn.onClick.AddListener(() =>
            {
                listSwitchLogic();
            });

            base.DrawUI();
        }

        private void setSelected(GameObject go)
        {
            if (_selectItem == null)
            {
                GameObject selectItemObj = Instantiate(_scrollList.TemplateObject) as GameObject;
                selectItemObj.name = "SelectedItem#";
                selectItemObj.transform.SetParent(SelectedItemPos, false);

                _selectItem = selectItemObj.GetComponent<ListItem>();
                if (_selectItem == null) {
                    _selectItem = selectItemObj.AddComponent<ListItem>();
                }

                Button del = selectItemObj.GetComponent<Button>();
                if (del != null)
                {
                    if (Application.isEditor) {
                        GameObject.DestroyImmediate(del);
                    }
                    else {
                        GameObject.Destroy(del);
                    }
                }

            }
            
            ListItem goitem = go.GetComponent<ListItem>();
            selectedID = _selectItem.id = goitem.id;

            if (CustomDrawListItem != null) {
                _selectItem.CustomDrawListItem = CustomDrawListItem;
            }

            _selectItem.setData(goitem.getRawData());
            _scrollList.setItemActivedByID(selectedID);
        }



        private void onListEvent(GameObject go, PointerEventData ped)
        {
            // Debug.UiLog("onListEvent ----> " + go.name);
            setSelected(go);
            listSwitchLogic();
            if (m_onSelectChanged != null)
            {
                ListItem li = go.GetComponent<ListItem>();
                int id = li.id;
                object obj = li.getRawData();
                m_onSelectChanged.Invoke(id, obj);
            }
        }

        /// <summary>
        /// 此方法让ComboBox在open / close之间切换
        /// </summary>
        public void listSwitchLogic()
        {
            if (!AlwaysOpend)
            {
                if (_isOpen)
                {
                    closeList();
                }
                else
                {
                    openList();
                }
            }
        }

        private bool _isOpen = false;
        public bool isOpen
        {
            get { return _isOpen; }
        }


        private void openList()
        {
            _isOpen = true;

            if (onComboBoxOpen != null)
            {
                onComboBoxOpen.Invoke();
            }

            if (CustomOpenAnimation != null)
            {
                CustomOpenAnimation(this);
                return;
            }
            
            ScrollListBackground.sizeDelta = new Vector2(ScrollListBackground.sizeDelta.x, 0f);
            ScrollListBackground.gameObject.SetActive(true);
            Vector2 bgh = new Vector2(ScrollListBackground.sizeDelta.x, _baseListBackgroundHeight);

            DOTween.To(() => ScrollListBackground.sizeDelta, x => ScrollListBackground.sizeDelta = x, bgh, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _scrollList.gameObject.SetActive(true);
                AorUIAnimator ar = _scrollList.GetComponent<AorUIAnimator>();
                if (ar != null)
                {
                    ar.fadeIN();
                }
            });
        }

        private void closeList()
        {
            _isOpen = false;

            if (onComboBoxClose != null)
            {
                onComboBoxClose.Invoke();
            }

            if (CustomCloseAnimation != null)
            {
                CustomCloseAnimation(this);
                return;
            }

            AorUIAnimator ar = _scrollList.GetComponent<AorUIAnimator>();
            if (ar != null)
            {
                _scrollList.GetComponent<AorUIAnimator>().fadeOUT(() =>
                {
                    if (_scrollList.gameObject.activeInHierarchy)
                    {
                        _scrollList.gameObject.SetActive(false);
                    }
                    Vector2 bgh = new Vector2(ScrollListBackground.sizeDelta.x, 0f);
                    DOTween.To(() => ScrollListBackground.sizeDelta, x => ScrollListBackground.sizeDelta = x, bgh, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        ScrollListBackground.gameObject.SetActive(false);
                    });
                });
            }
            else
            {
                if (_scrollList.gameObject.activeInHierarchy)
                {
                    _scrollList.gameObject.SetActive(false);
                }
                Vector2 bgh = new Vector2(ScrollListBackground.sizeDelta.x, 0f);
                DOTween.To(() => ScrollListBackground.sizeDelta, x => ScrollListBackground.sizeDelta = x, bgh, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    ScrollListBackground.gameObject.SetActive(false);
                });
            }
        }
    }
}