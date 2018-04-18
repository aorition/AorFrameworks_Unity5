using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using AorBaseUtility;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Framework.UI
{

    public enum ListDirection
    {
        horizontal,
        vertical
    }

    public class ScrollList : AorUIComponent
    {


        /// <summary>
        /// 列表方向
        /// </summary>
        [SerializeField, SetProperty("Direct")]
        protected ListDirection _Direct = ListDirection.vertical;
        public ListDirection Direct {
            get { return _Direct; }
            set {

                if (Application.isEditor)
                {
                    _Direct = value;
                    _isDirty = true;
                }
                else
                {
                    if (value != _Direct)
                    {
                        _Direct = value;
                        _isDirty = true;
                    }
                }


            }
        }

        /// <summary>
        /// 强制刷新显示数据. 开启后会每帧都刷新Item显示数据
        /// </summary>
        public bool ForceRefresh = false;

        private bool _DoScrollDirty = false;

        /// <summary>
        /// 显示的条数
        /// </summary>
        [SerializeField, SetProperty("SizeNum")]
        protected int _SizeNum = 1;
        public int SizeNum {
            get { return _SizeNum; }
            set {
                if (value > 0)
                {

                    if (Application.isEditor)
                    {
                        _SizeNum = value;
                        _isDirty = true;
                    }
                    else
                    {
                        if (value != _SizeNum)
                        {
                            _SizeNum = value;
                            _isDirty = true;
                        }
                    }


                }
            }
        }

        /// <summary>
        /// 缓存的条数
        /// </summary>
        [SerializeField, SetProperty("BufferNum")]
        protected int _BufferNum = 2;
        public int BufferNum {
            get { return _BufferNum; }
            set {
                if (value > 0)
                {
                    if (Application.isEditor)
                    {
                        _BufferNum = value;
                        _isDirty = true;
                    }
                    else
                    {
                        if (value != _BufferNum)
                        {
                            _BufferNum = value;
                            _isDirty = true;
                        }
                    }
                }
            }
        }

        [SerializeField, SetProperty("padding")]
        protected float _padding = 0f;
        public float padding {
            get { return _padding; }
            set {
                if (value > 0)
                {
                    if (Application.isEditor)
                    {
                        _padding = value;
                        _isDirty = true;
                    }
                    else
                    {
                        if (value != _padding)
                        {
                            _padding = value;
                            _isDirty = true;
                        }
                    }
                }
            }
        }

        public bool isInit
        {
            get { return _isInit; }
        }

        /// <summary>
        /// 模版对象(ListItem);
        /// </summary>
        private UnityEngine.Object _TemplateObject;

        public UnityEngine.Object TemplateObject {
            get { return _TemplateObject; }
            set {
                if (_TemplateObject != value)
                {

                    _TemplateObject = value;

                    _reset();
                }
            }
        }

        private RectTransform TemplateRT;

        [SerializeField]
        private string TemplateLoadPath;

        private ScrollRect.MovementType _initScrollRectMovementType;
        private bool _initScrollRectInertia;

        [SerializeField, SetProperty("useItemAlign")]
        private bool _useItemAlign = false;
        /// <summary>
        /// 是否启用 tiemAlign (子物件对齐) 功能
        /// 
        /// </summary>
        public bool useItemAlign {
            get { return _useItemAlign; }
            set {
                if (Application.isEditor)
                {
                    _useItemAlign = value;
                    _isDirty = true;
                }
                else
                {
                    if (value != _useItemAlign)
                    {
                        _useItemAlign = value;
                        _isDirty = true;
                    }
                }
            }
        }

        /// <summary>
        /// 纯逻辑数据
        /// </summary>
        protected List<object> _listData;

        public object[] GetAllListData()
        {
            return _listData.ToArray();
        }

        public void setListDataAsNew(IEnumerable<object> colletion, uint startIndex = 0)
        {
            _startIndex = startIndex;
            _listData = new List<object>(colletion);
            _isDirty = true;
        }

        public void ListDataClear()
        {
            if (_listData != null)
            {
                _listData.Clear();
                _isDirty = true;
            }
        }

        public void InsertListDataRangeInHead(IEnumerable<object> colletion)
        {
            List<object> old = _listData;
            _listData = new List<object>();
            _listData.AddRange(colletion);
            _listData.AddRange(old);
            _isDirty = true;
        }
        public void AddListDataRange(IEnumerable<object> colletion)
        {
            _listData.AddRange(colletion);
            _isDirty = true;
        }


        public void RemoveListDataRangeInHead(int count)
        {
            _startIndex -= (uint)count;
            _listData.RemoveRange(0, count);
            _isDirty = true;
        }
        public void RemoveListDataRange(int count)
        {
            _listData.RemoveRange(_listData.Count - count, count);
            _isDirty = true;
        }



        public void StartCreative(int dataCount, Action<ScrollListItem> customDrawListItem, string templateLoadPath = null, uint startIndex = 0, int jumpIndex = 0)
        {

            if (_listData != null)
            {
                _listData.Clear();
                _listData = null;
            }

            _listData = new List<object>();

            int i, len = dataCount;
            for (i = 0; i < len; i++)
            {
                _listData.Add(i);
            }

            CustomDrawListItem = customDrawListItem;

            if (!string.IsNullOrEmpty(templateLoadPath))
            {
                TemplateLoadPath = templateLoadPath;
            }

            if (dataCount <= 0)
            {
                return;
            }

            _startIndex = startIndex;
            _index = jumpIndex - (int)startIndex;
            if (_index < 0)
            {
                _startIndex = 0;
                _index = 0;
            }
            if (_index > dataCount - 1)
            {
                _index = dataCount - 1;
            }

            if (_isInit)
            {
                _isDirty = true;
            }
        }

        private void _reset()
        {
            if (_listItemCache != null)
            {
                int i, len = _listItemCache.Count;
                for (i = 0; i < len; i++)
                {
                    GameObject.DestroyImmediate(_listItemCache[i].gameObject);
                }
                _listItemCache.Clear();
            }
            if (_listItemTempCache != null)
            {
                _listItemTempCache.Clear();
            }
            if (_listData != null)
            {
                _listData.Clear();
            }

            if (_isInit)
            {
                _isDirty = true;
            }

        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _listData = new List<object>();
        }

        private List<ScrollListItem> _listItemCache;
        private List<ScrollListItem> _listItemTempCache;

        /// <summary>
        /// 初始化
        /// </summary>
        protected override bool Initialization()
        {

            _listItemCache = new List<ScrollListItem>();
            _listItemTempCache = new List<ScrollListItem>();
            _newIDList = new List<int>();

            _scrollRect = this.GetComponent<ScrollRect>();
            if (_scrollRect == null)
            {
                _scrollRect = gameObject.AddComponent<ScrollRect>();
            }

            _initScrollRectMovementType = _scrollRect.movementType;
            _initScrollRectInertia = _scrollRect.inertia;

            _panel = _scrollRect.content;

            if (_Direct == ListDirection.vertical)
            {
                _scrollRect.vertical = true;
                _scrollRect.horizontal = false;
            }
            else
            {
                _scrollRect.vertical = false;
                _scrollRect.horizontal = true;
            }

            setRTBaseData(_panel);

            if (!string.IsNullOrEmpty(TemplateLoadPath))
            {

                ResourcesLoadBridge.LoadPrefab(TemplateLoadPath, go =>
                {
                    go.name = "ListItemTemplate";
                    go.transform.SetParent(transform, false);
                    _TemplateObject = go;

                    //检查TemplateObject是否已经存在于Hierarchy中
                    if ((_TemplateObject as GameObject).activeInHierarchy)
                    {
                        (_TemplateObject as GameObject).SetActive(false);
                    }

                    TemplateRT = (_TemplateObject as GameObject).GetComponent<RectTransform>();

                    //强制TemplateRT设置
                    setRTBaseData(TemplateRT);

                    if (_listData.Count > 0)
                    {
                        DrawUI();
                    }

                    _scrollRect.onValueChanged.AddListener((v2) =>
                    {
                        if (!ForceRefresh)
                        {
                            _DoScrollDirty = true;
                        }
                    });

                    base.Initialization();

                });


                return false;
            }
            else
            {
                if (_TemplateObject == null)
                {
                    Transform rt = _panel.childCount > 0 ? _panel.GetChild(0) : null;
                    if (rt == null)
                    {
                        Log.Error("ScrollList.setupUI Faild :: can not find _TemplateObject");
                        if (Application.isEditor)
                        {
                            GameObject.DestroyImmediate(this);
                        }
                        else
                        {
                            GameObject.Destroy(this);
                        }
                        return false;
                    }
                    else
                    {
                        rt.SetParent(transform, false);
                        _TemplateObject = rt.gameObject;
                    }
                }
                else
                {
                    if (_panel.childCount > 0)
                    {
                        Transform rt = _panel.GetChild(0);
                        if (rt != null)
                        {
                            rt.SetParent(transform, false);
                            if (!_TemplateObject.Equals(rt.gameObject))
                            {
                                GameObject.Destroy(rt.gameObject);
                            }
                        }
                    }
                }

            }

            //检查TemplateObject是否已经存在于Hierarchy中
            if ((_TemplateObject as GameObject).activeInHierarchy)
            {
                (_TemplateObject as GameObject).SetActive(false);
            }

            TemplateRT = (_TemplateObject as GameObject).GetComponent<RectTransform>();

            //强制TemplateRT设置
            setRTBaseData(TemplateRT);

            if (_listData.Count > 0)
            {
                DrawUI();
            }

            _scrollRect.onValueChanged.AddListener((v2) =>
            {
                if (!ForceRefresh)
                {
                    _DoScrollDirty = true;
                }
            });

            return base.Initialization();
        }

        private void setRTBaseData(RectTransform rt)
        {
            if (_Direct == ListDirection.vertical)
            {
                rt.pivot = new Vector2(0, 1);
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(1, 1);

                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;

                rt.localPosition = Vector3.zero;
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(0, rt.sizeDelta.y);

            }
            else
            {
                rt.pivot = new Vector2(0, 1);
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(0, 1);

                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;

                rt.localPosition = Vector3.zero;
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 0);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CheckItemSize_bool = false;
            if (_listData != null)
                _listData.Clear();

            _listData = null;
            if (_scrollRect != null)
            {
                _scrollRect.onValueChanged.RemoveAllListeners();
                _scrollRect = null;
            }

            _panel = null;
        }

        private ScrollRect _scrollRect;
        private RectTransform _panel;

        private bool _isScroll;
        public bool isScroll {
            get { return _isScroll; }
        }

        [SerializeField, SetProperty("ItemSize")]
        private Vector2 _ItemSize;      //子元素的宽高
        public Vector2 ItemSize {
            get { return _ItemSize; }
            set {

                if (Application.isEditor)
                {
                    if (!UseTemplateItemSize)
                    {
                        if ((_Direct == ListDirection.horizontal && value.x != _ItemSize.x) || (_Direct == ListDirection.vertical && value.y != _ItemSize.y))
                        {
                            _ItemSize = value;
                            _isDirty = true;
                        }
                    }
                }
                else
                {
                    if (value != _ItemSize && !UseTemplateItemSize)
                    {
                        if ((_Direct == ListDirection.horizontal && value.x != _ItemSize.x) || (_Direct == ListDirection.vertical && value.y != _ItemSize.y))
                        {
                            _ItemSize = value;
                            _isDirty = true;
                        }
                    }
                }



            }
        }

        [SerializeField, SetProperty("UseCustomOutlineSize")]
        private bool _UseCustomOutlineSize = false;
        public bool UseCustomOutlineSize {
            get { return _UseCustomOutlineSize; }
            set {
                if (Application.isEditor)
                {
                    _UseCustomOutlineSize = value;
                    _isDirty = true;
                }
                else
                {

                    if (value != _UseCustomOutlineSize)
                    {
                        _UseCustomOutlineSize = value;
                        _isDirty = true;
                    }

                }
            }
        }

        public bool UseTemplateItemSize = true;

        [SerializeField, SetProperty("index")]
        //private int _cutNum = 0; //裁切指数
        private int _index = 0; //位移指数

        private uint _startIndex = 0; //标记逻辑数据开始index;

        public int index {
            get { return _index + (int)_startIndex; }
            set {
                if (Application.isEditor)
                {
                    _index = value - (int)_startIndex;
                    _isDirty = true;
                }
                else
                {
                    if (value - (int)_startIndex != _index)
                    {
                        _index = value - (int)_startIndex;
                        _isDirty = true;
                    }
                }
            }
        }

        private void removeUseItemAlignEvents(ScrollRect asr)
        {
//            asr.onBeginDragUnityEvent.RemoveListener(onUseItemAlign_onBeginDrag);
//            // asr.onDragUnityEvent.RemoveListener(onUseItemAlign_onDrag);
//            asr.onEndDragUnityEvent.RemoveListener(onUseItemAlign_onEndDrag);
        }

        private void addUseItemAlignEvents(ScrollRect asr)
        {

            removeUseItemAlignEvents(asr);

//            asr.onBeginDragUnityEvent.AddListener(onUseItemAlign_onBeginDrag);
//            // asr.onDragUnityEvent.AddListener(onUseItemAlign_onDrag);
//            asr.onEndDragUnityEvent.AddListener(onUseItemAlign_onEndDrag);
        }

        //[SerializeField]
        private int _itemNum; //逻辑中加载了多少条;
        protected override void DrawUI()
        {

            if (_scrollRect is ScrollRect)
            {
                ScrollRect asr = (ScrollRect)_scrollRect;

                if (_useItemAlign)
                {
                    asr.movementType = ScrollRect.MovementType.Clamped;
                    //asr.inertia = false;

                    addUseItemAlignEvents(asr);

                }
                else
                {
                    asr.movementType = _initScrollRectMovementType;
                    //                    asr.inertia = _initScrollRectInertia;

                    removeUseItemAlignEvents(asr);

                }
            }
            else
            {
                _scrollRect.movementType = _initScrollRectMovementType;
                //                _scrollRect.inertia = _initScrollRectInertia;
            }

            if (UseTemplateItemSize)
            {
                _ItemSize = new Vector2(TemplateRT.sizeDelta.x, TemplateRT.sizeDelta.y);
            }

            int i;
            _itemNum = SizeNum + BufferNum * 2;
            if (_listData.Count < _itemNum)
            {
                //数据列表实际数量少于显示数量+缓存数量(x2)
                if (_listData.Count <= SizeNum)
                {
                    //数据列表实际数量少于显示数量
                    _isScroll = false;
                }
                else
                {
                    _isScroll = true;
                }
                _itemNum = _listData.Count;
            }
            else
            {
                _isScroll = true;
            }

            //            
            int preIndex = Mathf.Clamp(_index, 0, _listData.Count - _itemNum);
            if (preIndex < 0)
            {
                preIndex = 0;
            }

            _index = Mathf.Clamp(_index, 0, _listData.Count - SizeNum);
            if (_index < 0)
            {
                _index = 0;
            }
            _indexCache = _index;

            int length = preIndex + _itemNum;
            int v = -1;
            for (i = preIndex; i < length; i++)
            {

                GameObject go;
                ScrollListItem li;

                v = i - preIndex;
                if (v < _listItemCache.Count)
                {
                    go = _listItemCache[v].gameObject;
                    li = _listItemCache[v];
                }
                else
                {
                    //new
                    go = GameObject.Instantiate(_TemplateObject) as GameObject;
                    li = go.GetComponent<ScrollListItem>();
                    if (li == null)
                    {
                        li = go.AddComponent<ScrollListItem>();
                        // li.FinishCall(li.GetType().FullName);
                    }
                    _listItemCache.Add(li);

//                    AorUIEventListener aui = li.gameObject.GetComponent<AorUIEventListener>();
//                    if (aui != null)
//                    {
//                        aui.onEventClick.AddListener(listItemClickDo);
//                    }

                    if (!go.activeInHierarchy)
                    {
                        go.SetActive(true);
                    }

                    li.transform.SetParent(_panel.transform, false);

                }
                updateListItem(i, li, true);
            }

            //移除多余Item
            if (v + 1 < _listItemCache.Count)
            {

                List<ScrollListItem> dellList = new List<ScrollListItem>();
                for (int d = (v + 1); d < _listItemCache.Count; d++)
                {
                    dellList.Add(_listItemCache[d]);
                }

                for (int f = 0; f < dellList.Count; f++)
                {
                    ScrollListItem li = dellList[f];
                    _listItemCache.Remove(li);
                    GameObject.DestroyImmediate(li.gameObject);
                }

                dellList.Clear();
                dellList = null;
            }

            _itemNum = length;

            int fitNum = Mathf.Min(_itemNum, SizeNum);

            if (_Direct == ListDirection.vertical)
            {
                _panel.anchoredPosition = new Vector2(_panel.anchoredPosition.x, _index * (_ItemSize.y + _padding));
                setPanelHeight();
                if (!_UseCustomOutlineSize)
                {
                    this.GetComponent<RectTransform>().sizeDelta = new Vector2(
                        this.GetComponent<RectTransform>().sizeDelta.x,
                        (_ItemSize.y + _padding) * (_itemNum < SizeNum ? _itemNum : SizeNum) - _padding
                        );
                }
            }
            else
            {
                _panel.anchoredPosition = new Vector2(-(_index * (_ItemSize.x + _padding)), _panel.anchoredPosition.y);
                setPanelWidth();
                if (!_UseCustomOutlineSize)
                {
                    this.GetComponent<RectTransform>().sizeDelta = new Vector2(
                        (_ItemSize.x + _padding) * (_itemNum < SizeNum ? _itemNum : SizeNum) - _padding,
                        this.GetComponent<RectTransform>().sizeDelta.y
                        );
                }
            }

            base.DrawUI();
        }

        private void updateListItem(int index, ScrollListItem li, bool updateValue)
        {

            li.id = index;

            li.gameObject.name = index.ToString();

            if (_Direct == ListDirection.vertical)
            {
                li.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(li.gameObject.GetComponent<RectTransform>().anchoredPosition.x, -(index * (_ItemSize.y + _padding)));
            }
            else
            {
                li.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(index * (_ItemSize.x + _padding), li.gameObject.GetComponent<RectTransform>().anchoredPosition.y);
            }

            if (!li.gameObject.activeInHierarchy)
            {
                li.gameObject.SetActive(true);
            }

            //setValueCode
            if (updateValue)
            {
                setItemValue(li, index);
            }
        }
        private bool CheckItemSize_bool = false;//标记当前滑动时不复位
        private float item_size2_mark;//标记当前条目高度
        private int index_size_mark;//标记当前条目索引
        /// <summary>
        /// 检查item尺寸
        /// </summary>
        public void CheckItemSize(int index01, float item_size2, bool size)
        {
            index_size_mark = index01;
            item_size2_mark = item_size2;
            CheckItemSize_bool = size;
            int i, length = _index + _SizeNum + _BufferNum;
            length = (length < _listData.Count ? length : _listData.Count);

            for (i = (_index - _BufferNum < 0 ? 0 : _index - _BufferNum); i < length; i++)
            {
                if (i < _listData.Count)
                {
                    // ListItem lii = getListItemFromCache(i);
                    ScrollListItem lii = getListItemFromCache(i, false);
                    if (lii != null)
                    {
                        UpdataItemSize(i, lii, index01, item_size2, size);
                        //updateListItem(i, lii, false);//
                        //  _listItemTempCache.Add(lii);
                    }
                    else
                    {
                        //_newIDList.Add(i);
                    }
                }
            }
        }

        /// <summary>
        /// 更新item尺寸
        /// </summary>
        private void UpdataItemSize(int index, ScrollListItem li, int InputIndex, float size_y, bool add)
        {
            size_y = add == true ? size_y : 0;
            float item_y = int.Parse(li.gameObject.name) > int.Parse(InputIndex.ToString()) ? size_y : 0;


            // li.gameObject.name = index.ToString();
            float a = _padding;
            if (_Direct == ListDirection.vertical)
            {
                li.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(li.gameObject.GetComponent<RectTransform>().anchoredPosition.x, -(index * (_ItemSize.y + _padding)) + item_y);
            }
            else
            {//暂不写横排集
                li.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(index * (_ItemSize.x + _padding), li.gameObject.GetComponent<RectTransform>().anchoredPosition.y);
            }

            if (!li.gameObject.activeInHierarchy)
            {
                li.gameObject.SetActive(true);
            }



        }


        private int _highlight = -1;
        public void setItemActivedByID(int id)
        {
            if (_highlight == id)
                return;

            if (_highlight != -1)
            {
                Transform o = _panel.FindChild(_highlight.ToString());
                if (o != null)
                {
                    o.GetComponent<ScrollListItem>().isActived = false;
                }
            }
            _highlight = id;
            Transform go = _panel.FindChild(id.ToString());
            if (go != null)
            {
                go.GetComponent<ScrollListItem>().isActived = true;
            }

        }
        /// <summary>
        /// 设置itemData
        /// </summary>
        /// <param name="t"></param>
        /// <param name="dataIndex"></param>
        protected void setItemValue(ScrollListItem li, int dataIndex)
        {
            if (CustomDrawListItem != null)
            {
                li.CustomDrawListItem = CustomDrawListItem;
            }

            li.setData(_listData[dataIndex]);
        }

        private void setPanelHeight()
        {
            //            _panel.sizeDelta = new Vector2(0, _ItemSize.y*_itemNum + (_itemNum > 0 ? (_padding*(_itemNum - 1)) : 0));
            int ldc = _listData.Count;
            _panel.sizeDelta = new Vector2(0, _ItemSize.y * ldc + (ldc > 0 ? (_padding * (ldc - 1)) : 0));
        }

        private void setPanelWidth()
        {
            //            _panel.sizeDelta = new Vector2(_ItemSize.x*_itemNum + (_itemNum > 0 ? (_padding*(_itemNum - 1)) : 0), 0);
            int ldc = _listData.Count;
            _panel.sizeDelta = new Vector2(_ItemSize.x * ldc + (ldc > 0 ? (_padding * (ldc - 1)) : 0), 0);
        }

        private int _indexCache;
        private List<int> _newIDList;

        private void DoScroll()
        {
            if (!_isScroll) return;
            if (!_scrollRect.enabled) return;

            if (_Direct == ListDirection.vertical)
            {
                _index = (int)Mathf.Round(_panel.anchoredPosition.y / (_ItemSize.y + _padding));
            }
            else
            {
                _index = -(int)Mathf.Round(_panel.anchoredPosition.x / (_ItemSize.x + _padding));
            }

            if (_indexCache == _index) return;

            if (_listItemTempCache.Count > 0)
            {
                _listItemTempCache.Clear();
            }

            if (_newIDList.Count > 0)
            {
                _newIDList.Clear();
            }

            int i, length = _index + _SizeNum + _BufferNum;
            length = (length < _listData.Count ? length : _listData.Count);
            for (i = (_index - _BufferNum < 0 ? 0 : _index - _BufferNum); i < length; i++)
            {
                if (i < _listData.Count)
                {
                    ScrollListItem li = getListItemFromCache(i);
                    if (li != null)
                    {
                        if (CheckItemSize_bool)

                            UpdataItemSize(i, li, index_size_mark, item_size2_mark, CheckItemSize_bool);
                        else

                            updateListItem(i, li, false);

                        _listItemTempCache.Add(li);
                    }
                    else
                    {
                        _newIDList.Add(i);
                    }
                }
            }

            if (_newIDList.Count > 0)
            {

                for (i = 0; i < _newIDList.Count; i++)
                {
                    ScrollListItem li;
                    int newID = _newIDList[i];
                    if (_listItemCache.Count > 0)
                    {
                        li = _listItemCache[0];
                        _listItemCache.RemoveAt(0);
                    }
                    else
                    {
                        //新增
                        GameObject go = GameObject.Instantiate(_TemplateObject) as GameObject;
                        li = go.GetComponent<ScrollListItem>();
                        if (li == null)
                        {
                            li = go.AddComponent<ScrollListItem>();
                            // li.FinishCall(li.GetType().FullName);
                        }

                        AorUIEventListener aui = li.gameObject.GetComponent<AorUIEventListener>();
                        if (aui != null)
                        {
                            aui.onEventClick.AddListener(listItemClickDo);
                        }

                        if (!go.activeInHierarchy)
                        {
                            go.SetActive(true);
                        }

                        li.transform.SetParent(_panel.transform, false);
                    }

                    updateListItem(newID, li, true);
                    _listItemTempCache.Add(li);

                }

            }

            _itemNum = length;

            ListItemTempCopy();

            _indexCache = _index;

        }

        private ScrollListItem getListItemFromCache(int index, bool delete = true)
        {
            for (int i = 0; i < _listItemCache.Count; i++)
            {
                if (_listItemCache[i].id == index)
                {
                    ScrollListItem li = _listItemCache[i];
                    if (delete)
                        _listItemCache.RemoveAt(i);
                    return li;
                }
            }
            return null;
        }

        private bool _itemAlignMoving = false;
        private bool _itemAlignCentering = false;
        private float _itemAlignThreshold = 50f;
        private float _itemAlignOffestRate = 0.1f;
        private float _itemAlignEndThreshold = 0.2f;
        private int _itemAlignTarIndex;
        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (_itemAlignMoving)
            {

                // UnityEngine.Debug.Log("****** " + _scrollRect.velocity);
                float vdis = Vector2.Distance(Vector2.zero, _scrollRect.velocity);

                if (vdis <= _itemAlignThreshold)
                {

                    _itemAlignTarIndex = _index;

                    _itemAlignMoving = false;
                    _scrollRect.inertia = false;
                    _itemAlignCentering = true;
                    //   UnityEngine.Debug.Log("vdis <= _itemAlignThreshold" + _scrollRect.velocity + " *** " + _index);
                }

            }

            if (ForceRefresh)
            {
                DoScroll();
                _DoScrollDirty = false;
            }
            else
            {
                if (_DoScrollDirty)
                {
                    DoScroll();
                    _DoScrollDirty = false;
                }
            }

            //对齐回归
            if (_itemAlignCentering)
            {
                float alignTar, offset;
                if (_Direct == ListDirection.vertical)
                {
                    alignTar = _itemAlignTarIndex * (_ItemSize.y + _padding);
                    offset = (alignTar - _panel.anchoredPosition.y) * _itemAlignOffestRate;

                    if (Mathf.Abs(offset) < _itemAlignEndThreshold)
                    {
                        _panel.anchoredPosition = new Vector2(_panel.anchoredPosition.x,
                                                                alignTar
                                                                );
                        _scrollRect.inertia = _initScrollRectInertia;
                        _itemAlignCentering = false;
                    }
                    else
                    {
                        _panel.anchoredPosition = new Vector2(_panel.anchoredPosition.x,
                                                                _panel.anchoredPosition.y + offset
                                                                );
                    }
                }
                else
                {
                    alignTar = -(_itemAlignTarIndex * (_ItemSize.x + _padding));
                    offset = (alignTar - _panel.anchoredPosition.x) * _itemAlignOffestRate;

                    if (Mathf.Abs(offset) < _itemAlignEndThreshold)
                    {
                        _panel.anchoredPosition = new Vector2(alignTar,
                                                                _panel.anchoredPosition.y
                                                                );
                        _scrollRect.inertia = _initScrollRectInertia;
                        _itemAlignCentering = false;
                    }
                    else
                    {
                        _panel.anchoredPosition = new Vector2(
                                                            _panel.anchoredPosition.x + offset,
                                                            _panel.anchoredPosition.y
                                                            );
                    }


                }
            }

        }

        private void ListItemTempCopy()
        {
            _listItemCache.AddRange(_listItemTempCache.ToArray());
            _listItemTempCache.Clear();
        }

        //根据id返回列表元素对象.
        public GameObject getListItemById(int id)
        {
//            if (!_isDrawUIDone) return null;
            /*
            GameObject go = _panel.FindChild(id.ToString()).gameObject;
            if (go != null)
            {
                return go;
            }
            return null;
            */
            int i, len = _listItemCache.Count;
            for (i = 0; i < len; i++)
            {
                if (id == _listItemCache[i].id)
                {
                    return _listItemCache[i].gameObject;
                }
            }
            return null;
        }

        private void onUseItemAlign_onBeginDrag(PointerEventData eventData)
        {
            _scrollRect.inertia = _initScrollRectInertia;
            _itemAlignCentering = false;
            _itemAlignMoving = false;
        }

        //        private void onUseItemAlign_onDrag(PointerEventData eventData) {
        //            UnityEngine.Debug.Log("****** onUseItemAlign_onDrag");
        //        }

        private void onUseItemAlign_onEndDrag(PointerEventData eventData)
        {
            //  UnityEngine.Debug.Log("****** onUseItemAlign_onEndDrag");
            _itemAlignCentering = false;
            _itemAlignMoving = true;
        }

        //----------------------------- Event interface

        /// <summary>
        /// <接口> 自定义绘制ListItem(ScrollList的子对象)的接口委托
        /// 
        /// 委托传入参数:
        /// ListItem : ListItem对象实例
        /// object : ListItem对象的data数据;
        /// 
        /// </summary>
        public Action<ScrollListItem> CustomDrawListItem;

        /// <summary>
        /// <接口> 当ListItem被点击时触发事件委托;
        /// 委托传入参数:
        /// GameObject : 被点击的ListItem.gameObject对象
        /// PointerEventData : 原生UGUI事件对象 
        /// 
        /// </summary>
        public Action<GameObject, PointerEventData> onListItemClicked;
        //目前只挂载了接收Click事件,所以只需要将移动中触发的Click事件屏蔽掉.

        //----------------------------- Event interface End

        private void listItemClickDo(GameObject go, PointerEventData ped)
        {
            if (onListItemClicked != null)
            {
                onListItemClicked(go, ped);
            }
        }

    }
}