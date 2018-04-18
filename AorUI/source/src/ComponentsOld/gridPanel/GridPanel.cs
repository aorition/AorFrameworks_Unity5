
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using ExoticUnity.GUI.AorUI.Components.Assets;
using ExoticUnity.GUI.AorUI.Core;
using ExoticUnity.GUI.AorUI.Debug;
using ExoticUnity.GUI.AorUI.events;
using ExoticUnity.Misc;
using Object = UnityEngine.Object;

namespace ExoticUnity.GUI.AorUI.Components {

    /// <summary>
    /// GridPanel 组件
    /// 
    /// Author : Aorition
    /// 
    /// 目前暂不支持多种数据排序方式 ... 仅使用左上角开始, 从左到右,从上到下的数据排序方式.
    /// 
    /// </summary>
    public class GridPanel : AorUIComponent {

        private struct GridPanelItemUV {
            public GridPanelItemUV(int u, int v) {
                this.U = u;
                this.V = v;
            }

            public int U;
            public int V;
        }

        /// <summary>
        /// 强制刷新显示数据. 开启后会每帧都刷新Item显示数据
        /// </summary>
        public bool ForceRefresh = false;

        private bool _DoScrollDirty = false;

        /// <summary>
        /// 显示的条数
        /// </summary>
        [SerializeField, SetProperty("SizeNumX")]
        protected int _SizeNumX = 1;
        public int SizeNumX {
            get { return _SizeNumX; }
            set {

                if (value > 0) {
                    if (Application.isEditor) {
                        _SizeNumX = value;
                        _isDirty = true;
                    }
                    else {
                        if (value != _SizeNumX) {
                            _SizeNumX = value;
                            _isDirty = true;
                        }
                    }
                }

                
            }
        }

        [SerializeField, SetProperty("SizeNumY")]
        protected int _SizeNumY = 1;
        public int SizeNumY {
            get { return _SizeNumY; }
            set {
                if (value > 0) {
                    if (Application.isEditor) {
                        _SizeNumY = value;
                        _isDirty = true;
                    } else {
                        if (value != _SizeNumY) {
                            _SizeNumY = value;
                            _isDirty = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 缓存的条数
        /// </summary>
        [SerializeField, SetProperty("BufferNumX")]
        protected int _BufferNumX = 2;
        public int BufferNumX {
            get { return _BufferNumX; }
            set {

                if (value > 0) {
                    if (Application.isEditor) {
                        _BufferNumX = value;
                        _isDirty = true;
                    } else {
                        if (value != _BufferNumX) {
                            _BufferNumX = value;
                            _isDirty = true;
                        }
                    }
                }
            }
        }

        [SerializeField, SetProperty("BufferNumY")]
        protected int _BufferNumY = 2;
        public int BufferNumY {
            get { return _BufferNumY; }
            set {

                if (value > 0) {
                    if (Application.isEditor) {
                        _BufferNumY = value;
                        _isDirty = true;
                    } else {
                        if (value != _BufferNumY) {
                            _BufferNumY = value;
                            _isDirty = true;
                        }
                    }
                }
            }
        }

        [SerializeField, SetProperty("Padding")] 
        protected Vector2 _padding;
        public Vector2 Padding {
            get { return _padding; }
            set {
                if (value.x >= 0 && value.y >= 0) {

                    if (Application.isEditor) {
                        _padding = value;
                        _isDirty = true;
                    }
                    else {
                        if (value != _padding) {
                            _padding = value;
                            _isDirty = true;
                        }
                    }

                    
                }
            }
        }

        public bool isStarted {
            get { return _isStarted; }
        }

        /// <summary>
        /// 模版对象(ListItem);
        /// </summary>
        public UnityEngine.Object TemplateObject;
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
                if (Application.isEditor) {
                    _useItemAlign = value;
                    _isDirty = true;
                }
                else {
                    if (value != _useItemAlign) {
                        _useItemAlign = value;
                        _isDirty = true;
                    }
                }
            }
        }

        private int _rowMaxCount;
        private int _gridDataCount;

        /// <summary>
        /// 纯逻辑数据
        /// </summary>
//        protected List<object> _gridData;
        // protected List<List<object>> _gridData;
        protected object[][] _gridData;


        /// <summary>
        /// 置入数据列表
        /// </summary>
        /// <param name="data"></param>
        public void setGridBoxData(object[][] data) {

            _gridData = data;

            int n = 0;
            for (int v = 0; v < _gridData.Length; v++) {
                for (int u = 0; u < data[v].Length; u++) {
                    n ++;
                }
            }

            _rowMaxCount = _gridData[0].Length;
            _gridDataCount = n;

            _isDirty = true;

        }

        /// <summary>
        /// 置入数据列表
        /// </summary>
        /// <param name="data"></param>
        public void setGridBoxData(List<List<object>> data) {

            _gridData = new object[data.Count][];
            
            int n = 0;
            for (int v = 0; v < data.Count; v++) {
                _gridData[v] = new object[data[v].Count];
                for (int u = 0; u < data[v].Count; u++) {
                    _gridData[v][u] = data[v][u];
                    n++;
                }
            }

            _rowMaxCount = _gridData[0].Length;
            _gridDataCount = n;

            _isDirty = true;

        }

        public void StartCreative(int dataCount, int maxRowNum, Action<ListItem> customDrawListItem, string templateLoadPath = null,int jumpIndexX = 0, int jumpIndexY = 0) {

            if (dataCount <= 0 || maxRowNum <= 0) {
                UiLog.Error("GridPanel.StartCreative Error :  dataCount or maxRowNum is empty !");
                return;
            }
            
            int u, v, ulenth, vlenth;
            int n = 0;
            ulenth = (maxRowNum < dataCount ? maxRowNum : dataCount);
            vlenth = Mathf.CeilToInt((float)dataCount / maxRowNum);
            _gridData = new object[vlenth][];
            for (v = 0; v < vlenth; v++) {
                int uNum = (v + 1)*maxRowNum > dataCount ? dataCount - v*maxRowNum : maxRowNum;
                _gridData[v] = new object[uNum];
                for (u = 0; u < ulenth; u++) {
                    if (n < dataCount) {
                        _gridData[v][u] = n;
                        n++;
                    }
                    else {
                        break;
                    }
                }
            }

            _rowMaxCount = _gridData[0].Length;
            _gridDataCount = dataCount;

            CustomDrawListItem = customDrawListItem;

            if (!string.IsNullOrEmpty(templateLoadPath)) {
                TemplateLoadPath = templateLoadPath;
            }

            if (dataCount <= 0) {
                return;
            }

            _indexX = Mathf.Clamp(jumpIndexX, 0, maxRowNum - 1);
            _indexY = Mathf.Clamp(jumpIndexY, 0, _gridData.Length - 1);

            if (_isStarted) {
                _isDirty = true;
            }
        }
        /*
        public override void OnAwake() {
            base.OnAwake();
        }*/

//        private List<ListItem> _listItemCache;
        private ListItem[] _listItemCache;
//        private List<ListItem> _listItemTempCache;
        private ListItem[] _listItemTempCache;
//        private List<GridPanelItemUV> _newIDList;
        private GridPanelItemUV[] _newIDList;

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void Initialization() {

//            _listItemCache = new List<ListItem>();
//            _listItemTempCache = new List<ListItem>();
//            _newIDList = new List<GridPanelItemUV>();

            _scrollRect = this.GetComponent<ScrollRect>();

            if (_scrollRect == null) {
                _scrollRect = gameObject.AddComponent<AorScrollRect>();
            }

            _initScrollRectMovementType = _scrollRect.movementType;
            _initScrollRectInertia = _scrollRect.inertia;

            _scrollRect.vertical = false;
            _scrollRect.horizontal = false;

            _panel = _scrollRect.content;

            setPanelRTBaseData(_panel,new Vector2(
                                            (_SizeNumX + _BufferNumX * 2) * (_ItemSize.x + Padding.x) - Padding.x,
                                            (_SizeNumY + _BufferNumY * 2) * (_ItemSize.y + Padding.y) - Padding.y
                                            ));

            if (!string.IsNullOrEmpty(TemplateLoadPath)) {

                AorUIAssetLoader.LoadPrefabFromPool(TemplateLoadPath, go => {
                    go.name = "ListItemTemplate";
                    go.transform.SetParent(transform, false);
                    TemplateObject = go;

                    //检查TemplateObject是否已经存在于Hierarchy中
                    if ((TemplateObject as GameObject).activeInHierarchy)
                    {
                        (TemplateObject as GameObject).SetActive(false);
                    }

                    TemplateRT = (TemplateObject as GameObject).GetComponent<RectTransform>();

                    //强制TemplateRT设置
                    setTemplateRTBaseData(TemplateRT);

                    if (_gridData.Length > 0)
                    {
                        _rowMaxCount = _gridData[0].Length;
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

                return;

            } else {
                if (TemplateObject == null) {
                    Transform rt = _panel.GetChild(0);
                    if (rt == null) {
                        UiLog.Error("ScrollList.setupUI Faild :: can not find _TemplateObject");
                        if (Application.isEditor) {
                            GameObject.DestroyImmediate(this);
                        } else {
                            GameObject.Destroy(this);
                        }
                        return;
                    } else {
                        rt.SetParent(transform, false);
                        TemplateObject = rt.gameObject;
                    }
                } else {
                    if (_panel.childCount > 0) {
                        Transform rt = _panel.GetChild(0);
                        if (rt != null) {
                            rt.SetParent(transform, false);
                            if (!TemplateObject.Equals(rt.gameObject)) {
                                GameObject.Destroy(rt.gameObject);
                            }
                        }
                    }
                }
            }

            //检查TemplateObject是否已经存在于Hierarchy中
            if ((TemplateObject as GameObject).activeInHierarchy) {
                (TemplateObject as GameObject).SetActive(false);
            }

            TemplateRT = (TemplateObject as GameObject).GetComponent<RectTransform>();

            //强制TemplateRT设置
            setTemplateRTBaseData(TemplateRT);

            if (_gridData != null && _gridData.Length > 0) {
                _rowMaxCount = _gridData[0].Length;
                _isDirty = true;
            }

            _scrollRect.onValueChanged.AddListener((v2) => {
                if (!ForceRefresh) {
                    _DoScrollDirty = true;
                }
            });

            base.Initialization();
        }

        private void setTemplateRTBaseData(RectTransform rt) {

            rt.pivot = new Vector2(0, 1);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);

            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;

            rt.localPosition = Vector3.zero;
            rt.anchoredPosition = Vector2.zero;
            //rt.sizeDelta = sizeDelta;
        }

        private void setPanelRTBaseData(RectTransform rt, Vector2 sizeDelta) {

            rt.pivot = new Vector2(0, 1);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);

            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;

            rt.localPosition = Vector3.zero;
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = sizeDelta;
        }

        protected override void OnDestroy() {
            base.OnDestroy();

            if (_scrollRect != null) {
                _scrollRect.onValueChanged.RemoveAllListeners();
                _scrollRect = null;
            }

            _panel = null;
        }

        private ScrollRect _scrollRect;
        private RectTransform _panel;

        private bool _isScrollX;
        public bool isScrollX {
            get { return _isScrollX; }
        }

        private bool _isScrollY;
        public bool isScrollY {
            get { return _isScrollY; }
        }

        [SerializeField]
        private Vector2 _ItemSize;      //子元素的宽高
        public Vector2 ItemSize {
            get { return _ItemSize; }
            set {

                if (value.x >= 0 && value.y >= 0) {

                    if (Application.isEditor) {
                        _ItemSize = value;
                        if (!UseTemplateItemSize) {
                            _isDirty = true;
                        }
                    }
                    else {
                        if (_ItemSize != value) {
                            _ItemSize = value;
                            if (!UseTemplateItemSize) {
                                _isDirty = true;
                            }
                        }
                    }

                }

            }
        }

        [SerializeField, SetProperty("UseCustomOutlineSize")]
        private bool _UseCustomOutlineSize = false;
        public bool UseCustomOutlineSize
        {
            get { return _UseCustomOutlineSize; }
            set
            {
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

        //private int _cutNum = 0; //裁切指数
        [SerializeField, SetProperty("indexX")]
        private int _indexX = 0; //位移指数
        public int indexX {
            get { return _indexX; }
            set {
                if (Application.isEditor) {
                    _indexX = value;
                    _isDirty = true;
                }
                else {
                    if (value != _indexX) {
                        _indexX = value;
                        _isDirty = true;
                    }
                }
            }
        }

        [SerializeField, SetProperty("indexY")]
        private int _indexY = 0; //位移指数
        public int indexY {
            get { return _indexY; }
            set {
                if (Application.isEditor) {
                    _indexY = value;
                    _isDirty = true;
                } else {
                    if (value != _indexY) {
                        _indexY = value;
                        _isDirty = true;
                    }
                }
            }
        }

        private void removeUseItemAlignEvents(AorScrollRect asr) {
            asr.onBeginDragUnityEvent.RemoveListener(onUseItemAlign_onBeginDrag);
            // asr.onDragUnityEvent.RemoveListener(onUseItemAlign_onDrag);
            asr.onEndDragUnityEvent.RemoveListener(onUseItemAlign_onEndDrag);
        }

        private void addUseItemAlignEvents(AorScrollRect asr) {

            removeUseItemAlignEvents(asr);

            asr.onBeginDragUnityEvent.AddListener(onUseItemAlign_onBeginDrag);
            // asr.onDragUnityEvent.AddListener(onUseItemAlign_onDrag);
            asr.onEndDragUnityEvent.AddListener(onUseItemAlign_onEndDrag);
        }

        private int _itemNumX;
        private int _itemNumY;
        
        protected override void DrawUI() {

            if (_scrollRect is AorScrollRect) {
                AorScrollRect asr = (AorScrollRect)_scrollRect;

                if (_useItemAlign) {
                    asr.movementType = ScrollRect.MovementType.Clamped;
                    //asr.inertia = false;

                    addUseItemAlignEvents(asr);

                } else {
                    asr.movementType = _initScrollRectMovementType;
                    //                    asr.inertia = _initScrollRectInertia;

                    removeUseItemAlignEvents(asr);

                }
            } else {
                _scrollRect.movementType = _initScrollRectMovementType;
                //                _scrollRect.inertia = _initScrollRectInertia;
            }

            if (UseTemplateItemSize) {
                _ItemSize = new Vector2(TemplateRT.sizeDelta.x, TemplateRT.sizeDelta.y);
            }

            _itemNumX = SizeNumX + BufferNumX*2;

            if (_rowMaxCount < _itemNumX) {
                //数据列表实际数量少于显示数量+缓存数量(x2)
                if (_rowMaxCount <= SizeNumX) {
                    //数据列表实际数量少于显示数量
                    _isScrollX = false;
                    _scrollRect.horizontal = false;
                } else {
                    _isScrollX = true;
                    _scrollRect.horizontal = true;
                }
                _itemNumX = _rowMaxCount;
            }
            else {
                _isScrollX = true;
                _scrollRect.horizontal = true;
            }

            _itemNumY = SizeNumY + BufferNumY * 2;

            if (_gridData.Length < _itemNumY) {
                //数据列表实际数量少于显示数量+缓存数量(x2)
                if (_gridData.Length <= SizeNumY) {
                    //数据列表实际数量少于显示数量
                    _scrollRect.vertical = false;
                    _isScrollY = false;
                } else {
                    _isScrollY = true;
                    _scrollRect.vertical = true;
                }
                _itemNumY = _gridData.Length;
            } else {
                _isScrollY = true;
                _scrollRect.vertical = true;
            }

            _indexX = Mathf.Clamp(_indexX, 0, _rowMaxCount - _itemNumX);
            _indexY = Mathf.Clamp(_indexY, 0, _gridData.Length - _itemNumY);

            _indexCacheX = _indexX;
            _indexCacheY = _indexY;

            List<ListItem> tempListItems = new List<ListItem>();
            if (_listItemCache != null) {
                for (int i = 0; i < _listItemCache.Length; i++) {
                    if (_listItemCache[i] != null) {
                        ListItem li = _listItemCache[i];
                        tempListItems.Add(li);
                    }
                }
            }

            int d = -1;

            int ulength = _indexX + _itemNumX, vlength = _indexY + _itemNumY;
            int u = 0, v = 0;
            for (v = _indexY; v < vlength; v++) {
                for (u = _indexX; u < ulength; u++) {
                    GameObject go;
                    ListItem li;

                    d ++;
                    if (u < _gridData[v].Length) {
                        if (d < tempListItems.Count) {
                            go = _listItemCache[d].gameObject;
                            li = _listItemCache[d];
                        }
                        else {
                            //new
                            go = AorUIAssetLoader.Instantiate(TemplateObject) as GameObject;
                            li = go.GetComponent<ListItem>();
                            if (li == null) {
                                li = go.AddComponent<ListItem>();
                                // li.FinishCall(li.GetType().FullName);
                            }
                            tempListItems.Add(li);

                            AorUIEventListener aui = li.gameObject.GetComponent<AorUIEventListener>();
                            if (aui != null) {
                                aui.onEventClick.AddListener(listItemClickDo);
                            }

                            li.transform.SetParent(_panel.transform, false);

                        }

                        if (!go.activeInHierarchy) {
                            go.SetActive(true);
                        }

                        updateListItem(u, v, li, true);
                    }
                } 
            }

            //移除多余Item
            if ((d + 1) < tempListItems.Count) {

                List<ListItem> dellList = new List<ListItem>();
                for (int dNum = (d + 1); dNum < tempListItems.Count; dNum++) {
                    dellList.Add(_listItemCache[dNum]);
                }

                for (int df = 0; df < dellList.Count; df++) {
                    ListItem li = dellList[df];
                    tempListItems.Remove(li);
                    GameObject.DestroyImmediate(li.gameObject);
                }

                dellList.Clear();
                dellList = null;
            }

            _listItemCache = tempListItems.ToArray();
            _listItemTempCache = new ListItem[_listItemCache.Length];
            _newIDList = new GridPanelItemUV[_listItemCache.Length];

            _panel.anchoredPosition = new Vector2(-(_indexX * (_ItemSize.x + _padding.x)), _indexY * (_ItemSize.y + _padding.y));
            _panel.sizeDelta = new Vector2((_ItemSize.x + _padding.x) * _rowMaxCount - _padding.x, (_ItemSize.y + _padding.y) * _gridData.Length - _padding.y);

            if (!_UseCustomOutlineSize)
            {
                this.GetComponent<RectTransform>().sizeDelta = new Vector2((_ItemSize.x + _padding.x) * _SizeNumX - _padding.x, (_ItemSize.y + _padding.y) * _SizeNumY - _padding.y);
            }
            base.DrawUI();
        }

        private void updateListItem(int x, int y, ListItem li, bool updateValue) {

            li.id = y*_rowMaxCount + x;

            li.gameObject.name = li.id.ToString();
            
            li.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                x * (_ItemSize.x + _padding.x),
                -(y * (_ItemSize.y + _padding.y))
                );

            //setValueCode
            if (updateValue) {
                setItemValue(li, x, y);
            }
        }

        private int _highlight = -1;
        public void setItemActivedByID(int id) {
            if (_highlight == id)
                return;

            if (_highlight != -1) {
                Transform o = _panel.FindChild(_highlight.ToString());
                if (o != null) {
                    o.GetComponent<ListItem>().isActived = false;
                }
            }
            _highlight = id;
            Transform go = _panel.FindChild(id.ToString());
            if (go != null) {
                go.GetComponent<ListItem>().isActived = true;
            }

        }
        /// <summary>
        /// 设置itemData
        /// </summary>
        /// <param name="t"></param>
        /// <param name="dataIndex"></param>
        protected void setItemValue(ListItem li, int x, int y) {
            if (CustomDrawListItem != null) {
                li.CustomDrawListItem = CustomDrawListItem;
            }
            li.setData(_gridData[y][x] as object);
        }

        private int _indexCacheX, _indexCacheY;

        private void DoScroll() {
            
            if (!_scrollRect.enabled) return;

            if (_isScrollX) {
                _indexX = -(int) Mathf.Round(_panel.anchoredPosition.x/(_ItemSize.x + _padding.x));
            }
            else {
                _indexX = 0;
            }

            if (_isScrollY) {
                _indexY = (int) Mathf.Round(_panel.anchoredPosition.y/(_ItemSize.y + _padding.y));
            }
            else {
                _indexY = 0;
            }

            if (_indexCacheX == _indexX && _indexCacheY == _indexY) return;

            if (!_isScrollX && !_isScrollY) return;

            int u, v;
            int ulength = _indexX + _SizeNumX + _BufferNumX, vlength = _indexY + _SizeNumY + _BufferNumY;
            ulength = (ulength < _rowMaxCount ? ulength : _rowMaxCount);
            vlength = (vlength < _gridData.Length ? vlength : _gridData.Length);
            int newIdNum = 0, tempCacheNum = 0;
            for (v = (_indexY - _BufferNumY < 0 ? 0 : _indexY - _BufferNumY); v < vlength; v++) {
                for (u = (_indexX - _BufferNumX < 0 ? 0 : _indexX - _BufferNumX); u < ulength; u++) {
                    if (u < _rowMaxCount && v < _gridData.Length) {
                        if (u < _gridData[v].Length) {
                            int id = v*_rowMaxCount + u;
                            if (id < _gridDataCount) {
                                ListItem li = getListItemFromCache(u, v);
                                if (li != null) {
                                    updateListItem(u, v, li, false);
                                    _listItemTempCache[tempCacheNum] = li;
                                    tempCacheNum ++;
                                }
                                else {
                                    _newIDList[newIdNum] = new GridPanelItemUV(u, v);
                                    newIdNum++;
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < newIdNum; i++) {
                
                GridPanelItemUV guv = _newIDList[i];
                ListItem li = findLastListItemFromCache();
                if (li != null) {
                    updateListItem(guv.U, guv.V, li, true);
                    _listItemTempCache[tempCacheNum] = li;
                    tempCacheNum ++;
                }
                else {
                    UiLog.Error("************** 计算逻辑有问题 ....");
                }

            }

            while (true) {
                ListItem li = findLastListItemFromCache();
                if (li != null) {
                    _listItemTempCache[tempCacheNum] = li;
                    tempCacheNum++;
                } else {
                    break;
                }
            }

            ListItemTempCopy();

            _indexCacheX = _indexX;
            _indexCacheY = _indexY;

        }

        private void clearTempCache() {
            int i, len = _listItemTempCache.Length;
            for (i = 0; i < len; i++) {
                _listItemTempCache[i] = null;
            }
        }

        private void ListItemTempCopy() {
            int i, len = _listItemCache.Length;
            for (i = 0; i < len; i++) {
                ListItem li = _listItemTempCache[i];
                _listItemCache[i] = li;
                _listItemTempCache[i] = null;
            }
        }

        private ListItem findLastListItemFromCache() {
            for (int i = 0; i < _listItemCache.Length; i++) {
                if (_listItemCache[i] != null) {
                    ListItem li = _listItemCache[i];
                    _listItemCache[i] = null;
                    return li;
                }
            }
            return null;
        }

        private ListItem getListItemFromCache(int u, int v) {
            int index = v * _rowMaxCount + u;
            for (int i = 0; i < _listItemCache.Length; i++) {
                if (_listItemCache[i] != null) {
                    if (_listItemCache[i].id == index) {
                        ListItem li = _listItemCache[i];
                        _listItemCache[i] = null;
                        return li;
                    }
                }
            }
            return null;
        }

        private bool _itemAlignMoving = false;
        private bool _itemAlignCentering = false;

        private float _itemAlignThreshold = 50f;
        private float _itemAlignOffestRate = 0.1f;
        private float _itemAlignEndThreshold = 0.2f;

        private int _itemAlignTarIndexX;
        private int _itemAlignTarIndexY;

        protected override void OnUpdate() {
            base.OnUpdate();

            if (_itemAlignMoving) {

                // UnityEngine.Debug.Log("****** " + _scrollRect.velocity);
                float vdis = Vector2.Distance(Vector2.zero, _scrollRect.velocity);

                if (vdis <= _itemAlignThreshold) {
                    
                    _itemAlignTarIndexX = _indexX;
                    _itemAlignTarIndexY = _indexY;

                    _itemAlignMoving = false;
                    _scrollRect.inertia = false;
                    _itemAlignCentering = true;
                    //   UnityEngine.Debug.Log("vdis <= _itemAlignThreshold" + _scrollRect.velocity + " *** " + _index);
                }

            }

            if (ForceRefresh) {
                DoScroll();
                _DoScrollDirty = false;
            } else {
                if (_DoScrollDirty) {
                    DoScroll();
                    _DoScrollDirty = false;
                }
            }

            //对齐回归
            if (_itemAlignCentering) {
                float alignTarX, offsetX, alignTarY, offsetY;

                alignTarX = -(_itemAlignTarIndexX*(_ItemSize.x + _padding.x));
                alignTarY = _itemAlignTarIndexY*(_ItemSize.y + _padding.y);

                offsetX = (alignTarX - _panel.anchoredPosition.x) * _itemAlignOffestRate;
                offsetY = (alignTarY - _panel.anchoredPosition.y) * _itemAlignOffestRate;

                if (Mathf.Abs(offsetX) < _itemAlignEndThreshold && Mathf.Abs(offsetY) < _itemAlignEndThreshold) {
                    _panel.anchoredPosition = new Vector2(  alignTarX,
                                                            alignTarY
                                                            );
                    _scrollRect.inertia = _initScrollRectInertia;
                    _itemAlignCentering = false;
                } else {
                    _panel.anchoredPosition = new Vector2(_panel.anchoredPosition.x + offsetX,
                                                            _panel.anchoredPosition.y + offsetY
                                                            );
                }

            }

        }

        //根据x,y返回列表元素对象.
        public GameObject getListItemById(int x, int y) {
            if (!_isDrawUIDone) return null;
            int id = y*_rowMaxCount + x;
            return getListItemById(id);
        }

        //根据id返回列表元素对象.
        public GameObject getListItemById(int id) {
            if (!_isDrawUIDone) return null;
            /*
            GameObject go = _panel.FindChild(id.ToString()).gameObject;
            if (go != null)
            {
                return go;
            }
            return null;
            */
            int i, len = _listItemCache.Length;
            for (i = 0; i < len; i++) {
                if (id == _listItemCache[i].id) {
                    return _listItemCache[i].gameObject;
                }
            }
            return null;
        }

        private void onUseItemAlign_onBeginDrag(PointerEventData eventData) {
            _scrollRect.inertia = _initScrollRectInertia;
            _itemAlignCentering = false;
            _itemAlignMoving = false;
        }

        //        private void onUseItemAlign_onDrag(PointerEventData eventData) {
        //            UnityEngine.Debug.Log("****** onUseItemAlign_onDrag");
        //        }

        private void onUseItemAlign_onEndDrag(PointerEventData eventData) {
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
        public Action<ListItem> CustomDrawListItem;

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

        private void listItemClickDo(GameObject go, PointerEventData ped) {
            if (!ped.IsPointerMoving()) {
                //Debug.UiLog("itemClick ----> " + go.name);
                if (onListItemClicked != null) {
                    onListItemClicked(go, ped);
                }
            }
        }

    }
}