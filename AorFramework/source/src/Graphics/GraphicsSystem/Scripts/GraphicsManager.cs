using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Extends;
using Framework.Graphic.Utility;
using Framework.Utility;
using UnityEngine;

namespace Framework.Graphic
{
    //[ExecuteInEditMode]
    public class GraphicsManager : ManagerBase
    {

        //========= Manager 模版 =============================
        //
        // 基于MonoBehavior的Manager类 需要遵循的约定:
        //
        // *. 采用_instance字段保存静态单例.
        // *. 非自启动Manager必须提供CreateInstance静态方法.
        // *. 提供Request静态方法.
        // *. 提供IsInit静态方法判定改Manager是否初始化
        // *. 须Awake中调用ManagerBase.VerifyUniqueOnInit验证单例唯一
        // *. 须Awake中调用ManagerBase.VerifyUniqueOnInit验证单例唯一
        //
        //=============== 基于MonoBehavior的Manager====================

        #region 废弃的方法

        [Obsolete("小写instance不符合规范,已由Instance代替")]
        public static GraphicsManager instance
        {
            get
            {
                return _instance;
            }
        }

        [Obsolete("GetInstance语意有歧义,已由CreateInstance代替")]
        public static GraphicsManager GetInstance(Transform parenTransform = null)
        {
            return CreateInstance(parenTransform);
        }

        [Obsolete("已由Request方法代替")]
        public static void RequestGraphicsManager(Action GraphicsManagerIniteDoSh)
        {
            Request(GraphicsManagerIniteDoSh);
        }

        [Obsolete("小写registerSubSettingData不符合规范,已由RegisterSubSettingData代替")]
        public void registerSubSettingData<T>(ScriptableObject data) where T : ScriptableObject
        {
            RegisterSubSettingData<T>(data);
        }

        [Obsolete("小写getSubSettingData不符合规范,已由GetSubSettingData代替")]
        public T getSubSettingData<T>() where T : ScriptableObject
        {
            return GetSubSettingData<T>();
        }

        [Obsolete("小写registerVisualCamera不符合规范,已由RegisterVisualCamera代替")]
        public void registerVisualCamera(VisualCamera cam)
        {
            RegisterVisualCamera(cam);
        }

        [Obsolete("小写unregisterVisualCamera不符合规范,已由UnregisterVisualCamera代替")]
        public void unregisterVisualCamera(VisualCamera cam)
        {
            UnregisterVisualCamera(cam);
        }

        #endregion

        //@@@ 静态方法实现区

        private static string _NameDefine = "GraphicsManager";

        private static GraphicsManager _instance;
        public static GraphicsManager Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 创建带有独立GameObject的Instance
        /// </summary>
        public static GraphicsManager CreateInstance(Transform parenTransform = null)
        {
            return ManagerBase.CreateInstance<GraphicsManager>(ref _instance, _NameDefine, parenTransform);
        }

        /// <summary>
        /// 在目标GameObject上的创建Instance
        /// </summary>
        public static GraphicsManager CreateInstanceOnGameObject(GameObject target)
        {
            return ManagerBase.CreateInstanceOnGameObject<GraphicsManager>(ref _instance, target);
        }

        public static void Request(Action GraphicsManagerIniteDoSh)
        {
            CreateInstance();
            ManagerBase.Request(ref _instance, GraphicsManagerIniteDoSh);
        }

        public static bool IsInit()
        {
            return ManagerBase.VerifyIsInit(ref _instance);
        }

        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        //@@@ override方法实现区

        protected override void Awake()
        {
            base.Awake();
            ManagerBase.VerifyUniqueOnInit(ref _instance,this, () =>
            {
                gameObject.name = _NameDefine;
            });
        }

        protected override void OnUnSetupedStart()
        {
            //加载默认描述文件进行初始化
            GraphicsSettingAsset def = ScriptableObject.CreateInstance<GraphicsSettingAsset>();
            def.MainCamDesInfo = GCamGDesInfo.Main();
            Setup(def);
        }


        protected override void OnDestroy()
        {
            OnMainCameraInited = null;
            OnSubCameraInited = null;
            //
            base.OnDestroy();
            ManagerBase.VerifyUniqueOnDispose(ref _instance, this);
        }

        protected override void init()
        {

            _initExHandleEvents();

            UseFixedUpdate = _GCGinfo.UseFixedUpdate;
            AllowVisualCameraParamCover = _GCGinfo.AllowVCamParaCover;

            //自动占用MainCamera
            _mainCamera = Camera.main;
            if (!_mainCamera)
            {
                _mainCamera = new GameObject().AddComponent<Camera>();
                _mainCamera.gameObject.tag = "MainCamera";
                _mainCamera.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            }

            _mainCamera.gameObject.SetActive(false);
            if (transform.parent) _mainCamera.transform.SetParent(transform.parent, false);

            //            if (Application.isPlaying && _dontDestroyOnLoad && !_parentTransform)
            //            {
            //                GameObject.DontDestroyOnLoad(_mainCamera.gameObject);
            //            }

            GraphicsCamUtility.ApplyDesDataToCameraFormDesInfo(_mainCamera, _GCGinfo.MainCamDesInfo);

            if (_GCGinfo.SubCamGDesInfos != null && _GCGinfo.SubCamGDesInfos.Count > 0)
            {

                for (int i = 0; i < _GCGinfo.SubCamGDesInfos.Count; i++)
                {
                    GCamGDesInfo info = _GCGinfo.SubCamGDesInfos[i];
                    Camera cam = new GameObject(info.name).AddComponent<Camera>();
                    cam.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

                    _subCameras.Add(cam);

                    cam.transform.SetParent(_mainCamera.transform, false);

                    GraphicsCamUtility.ApplyDesDataToCameraFormDesInfo(cam, info);
                }

            }
        }

        protected override void OnAfterInit()
        {
            _mainCamera.gameObject.SetActive(true);
        }

        //=======================================================================

        //@@@ Manager功能实现区

        [Tooltip("使用FixedUpdate刷新")]
        public bool UseFixedUpdate = false;

        [Tooltip("是否允许VisualCamera使用参数覆盖")]
        public bool AllowVisualCameraParamCover = true;
        
        //------------------------------ Events

        /// <summary>
        /// 当QualitySetting的GetQualityLevel值发生改变时触发该事件
        /// </summary>
        public Action<int> OnQualityLevelChanged;
        /// <summary>
        /// 当主相机初始化完毕时调用该事件
        /// </summary>
        public Action<Camera, GCamGDesInfo> OnMainCameraInited;
        /// <summary>
        /// 当Sub相机初始化完毕时调用该事件
        /// </summary>
        public Action<Camera, GCamGDesInfo> OnSubCameraInited;

        //------------------------------

        /// <summary>
        /// UI级效果支持RectTransform节点
        /// GraphicsManager封装的UI级效果需要填充此节点才能正常工作
        /// </summary>
        [SerializeField]
        protected RectTransform _UIEffRoot;
        public RectTransform UIEffRoot
        {
            get { return _UIEffRoot; }
            set { _UIEffRoot = value; }
        }

        [SerializeField]
        protected Camera _mainCamera;
        public Camera MainCamera
        {
            get { return _mainCamera; }
        }

        public GMEffect Effect
        {
            get
            {
                if (GMEffect.Instance == null)
                {
                    return GMEffect.GetInstance(this);
                }
                return GMEffect.Instance;
            }
        }

        private int _qualityLevelCache;

        private float _deltaTime = 0;

        protected readonly List<Camera> _subCameras = new List<Camera>();

        protected readonly List<VisualCamera> _visualCameras = new List<VisualCamera>();
        protected VisualCamera _currentVisualCamera;
        public VisualCamera CurrentVisualCamera
        {
            get { return _currentVisualCamera; }
        }

        protected GraphicsSettingAsset _GCGinfo;
        protected GCamGDesInfo mainCameraDesInfo
        {
            get { return _GCGinfo.MainCamDesInfo; }
        }

        #region setup方法集

        public void Setup(GraphicsSettingAsset GCGinfo)
        {
            if (!GCGinfo) return;
            _GCGinfo = GCGinfo;
            _isSetuped = true;
            __init();
        }

        public void Setup(GraphicsSettingAsset GCGinfo, RectTransform UIEffectRoot)
        {
            _UIEffRoot = UIEffectRoot;
            Setup(GCGinfo);
        }

        public void Setup(GraphicsSettingAsset GCGinfo, Camera mainCamera)
        {
            Setup(GCGinfo);
            _mainCamera = mainCamera;
        }

        public void Setup(GraphicsSettingAsset GCGinfo, Camera mainCamera, RectTransform UIEffectRoot)
        {
            _UIEffRoot = UIEffectRoot;
            Setup(GCGinfo, mainCamera);
        }

        #endregion

        /// <summary>
        /// 停止/启动渲染流
        /// </summary>
        public void SetRendering(bool isRendering)
        {
            if (_mainCamera)
            {
                if (_mainCamera.enabled != isRendering) _mainCamera.enabled = isRendering;
            }
            foreach (Camera subCamera in _subCameras)
            {
                if (subCamera.enabled != isRendering) subCamera.enabled = isRendering;
            }
        }

        protected Dictionary<Type, ScriptableObject> _SubSettingDic = new Dictionary<Type, ScriptableObject>();
        public void RegisterSubSettingData<T>(ScriptableObject data) where T : ScriptableObject
        {
            Type t = typeof(T);
            if (!_SubSettingDic.ContainsKey(t))
            {
                _SubSettingDic.Add(t, data);
            }
        }

        public T GetSubSettingData<T>() where T : ScriptableObject
        {
            Type t = typeof(T);
            if (_SubSettingDic.ContainsKey(t))
            {
                return _SubSettingDic[t] as T;
            }
            return null;
        }

        public Camera GetSubCamera(string name)
        {
            return _subCameras.Find(s => s.name.Equals(name));
        }

        public int GetSubCameraLength()
        {
            return _subCameras.Count;
        }

        public string[] GetSubCameraNames()
        {
            int len = _subCameras.Count;
            string[] li = new string[len];
            for (int i = 0; i < len; i++)
            {
                li[i] = _subCameras[i].name;
            }
            return li;
        }

        public void AddSubCamera(Camera cam)
        {
            if (!_subCameras.Contains(cam))
            {
                _subCameras.Add(cam);
                cam.transform.SetParent(_mainCamera.transform, false);
            }
        }

        public void RemoveSubCamera(Camera cam)
        {
            if (_subCameras.Contains(cam))
            {
                _subCameras.Remove(cam);
                cam.gameObject.Dispose();
            }
        }

        //----------------------------------

        public void RegisterVisualCamera(VisualCamera cam)
        {
            if (!_visualCameras.Contains(cam))
            {
                _visualCameras.Add(cam);
            }
            RefreshCurrentVisualCamera();
        }

        public void UnregisterVisualCamera(VisualCamera cam)
        {
            if (_visualCameras.Contains(cam))
            {
                _visualCameras.Remove(cam);
            }
            RefreshCurrentVisualCamera();
        }


        private void _SortAndGetCurrentVisualCamera()
        {

            if (_visualCameras.Count == 0)
            {
                _currentVisualCamera = null;

                if (_mainCamera && _mainCamera.gameObject.activeSelf)
                {
                    _mainCamera.gameObject.SetActive(false);
                }
                return;
            }
            else
            {
                if (_mainCamera && !_mainCamera.gameObject.activeSelf)
                {
                    _mainCamera.gameObject.SetActive(true);
                }
            }

            _visualCameras.Sort((a, b) =>
            {
                //小->大
                if (a.Level > b.Level)
                {
                    return 1;
                }
                else if (a.Level < b.Level)
                {
                    return -1;
                }
                return 0;
            });
            //大->小
            _visualCameras.Reverse();

            int idx = _visualCameras.FindIndex(v => v.Solo);
            _currentVisualCamera = idx.Equals(-1) ? _visualCameras[0] : _visualCameras[idx];

            Camera.SetupCurrent(_currentVisualCamera.CrrentCamera);

        }

        public void RefreshCurrentVisualCamera()
        {
            _SortAndGetCurrentVisualCamera();
            _updateMainCameraPos(false);
        }

        private Vector3 _baseMainCameraPos;
        private Quaternion _baseMainCameraRotate;
        protected void _updateMainCameraPos(bool closeIIO)
        {
            if (_currentVisualCamera && _mainCamera)
            {

                if (_currentVisualCamera.IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0))
                {
                    _baseMainCameraRotate = _currentVisualCamera.transform.rotation;
                    _baseMainCameraPos = _currentVisualCamera.transform.position;

                }
                else
                {
                    _baseMainCameraRotate = Quaternion.Lerp(_mainCamera.transform.rotation, _currentVisualCamera.transform.rotation, _currentVisualCamera.Interpolation);
                    _baseMainCameraPos = Vector3.Lerp(_mainCamera.transform.position, _currentVisualCamera.transform.position, _currentVisualCamera.Interpolation);
                }

                _mainCamera.transform.rotation = _baseMainCameraRotate;
                _mainCamera.transform.position = _baseMainCameraPos + Effect.CamShakeOffset;

                //强制设置false
                if (closeIIO)
                {
                    _currentVisualCamera.IgnoreInterpolationOnce = false;
                }
            }
        }

        protected void FixedUpdate()
        {
            if (!_isInit || !UseFixedUpdate) return;
            _deltaTime = Time.fixedDeltaTime;
            process();
        }

        protected void Update()
        {
            if (!_isInit || UseFixedUpdate) return;
            _deltaTime = Time.deltaTime;
            process();
        }

        protected void LateUpdate()
        {
            if (!_isInit) return;
            _updateMainCameraPos(true);
            //
            _RefreshPostEffectComponents();
        }

        public void AddPostEffectComponent(IRTPostEffectComponent component)
        {
            if (!m_postRTEffectComponents.Contains(component))
            {
                m_postRTEffectComponents.Add(component);
            }
        }

        public void RemovePostEffectComponent(IRTPostEffectComponent component)
        {
            if (m_postRTEffectComponents.Contains(component))
            {
                m_postRTEffectComponents.Remove(component);
            }
        }

        protected readonly List<IRTPostEffectComponent> m_postRTEffectComponents = new List<IRTPostEffectComponent>();

        protected void _RefreshPostEffectComponents()
        {
            for (int i = 0; i < m_postRTEffectComponents.Count; i++)
            {
                m_postRTEffectComponents[i].UpdateGMPostEffect();
            }
        }

        protected void process()
        {

            //捕获事件
            _catchExHandleEvents();

            if (!_mainCamera) return;
            
            //更新所有VisualCamera附加计算
            for (int i = 0; i < _visualCameras.Count; i++)
            {
                _visualCameras[i].UpdateExtension(_deltaTime);
            }

            //刷新相机组优先级 (优化性能:: 在保证VisualCamera的注册,反注册,相关数据更改时已经手动调用RefreshCurrentVisualCamera方法,因此这里可以不进行排序)
            //_SortAndGetCurrentVisualCamera();

            //MainCamera参数匹配
            if (_currentVisualCamera && AllowVisualCameraParamCover)
            {
                if (_currentVisualCamera.OverrideClearFlags)
                {
                    _mainCamera.clearFlags = _currentVisualCamera.CrrentCamera.clearFlags;
                }
                else
                {
                    _mainCamera.clearFlags = mainCameraDesInfo.lensSetting.ClearFlags;
                }

                if (_currentVisualCamera.OverrideBackground)
                {
                    _mainCamera.backgroundColor = (_currentVisualCamera.IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                        _currentVisualCamera.CrrentCamera.backgroundColor :
                        Color.Lerp(_mainCamera.backgroundColor, _currentVisualCamera.CrrentCamera.backgroundColor, _currentVisualCamera.Interpolation);
                }
                else
                {
                    _mainCamera.backgroundColor = (_currentVisualCamera.IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                        mainCameraDesInfo.lensSetting.BackgroundColor :
                        Color.Lerp(_mainCamera.backgroundColor, mainCameraDesInfo.lensSetting.BackgroundColor, _currentVisualCamera.Interpolation);
                }

                if (_currentVisualCamera.OverrideOrthographic)
                {
                    _mainCamera.orthographic = _currentVisualCamera.CrrentCamera.orthographic;
                    _mainCamera.orthographicSize = (_currentVisualCamera.IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ? 
                        _currentVisualCamera.CrrentCamera.orthographicSize :
                        Mathf.Lerp(_mainCamera.orthographicSize, _currentVisualCamera.CrrentCamera.orthographicSize, _currentVisualCamera.Interpolation);
                }
                else
                {
                    _mainCamera.orthographic = mainCameraDesInfo.lensSetting.isOrthographicCamera;
                    _mainCamera.orthographicSize = (_currentVisualCamera.IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                                            mainCameraDesInfo.lensSetting.OrthographicSize :
                                            Mathf.Lerp(_mainCamera.orthographicSize, mainCameraDesInfo.lensSetting.OrthographicSize, _currentVisualCamera.Interpolation);
                }

                if (_currentVisualCamera.OverrideFieldOfView)
                {
                    _mainCamera.fieldOfView = (_currentVisualCamera.IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ? 
                        _currentVisualCamera.CrrentCamera.fieldOfView :
                        Mathf.Lerp(_mainCamera.fieldOfView, _currentVisualCamera.CrrentCamera.fieldOfView, _currentVisualCamera.Interpolation);
                }
                else
                {
                    _mainCamera.fieldOfView = (_currentVisualCamera.IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                        mainCameraDesInfo.lensSetting.FieldOfView :
                        Mathf.Lerp(_mainCamera.fieldOfView, mainCameraDesInfo.lensSetting.FieldOfView, _currentVisualCamera.Interpolation);
                }

                if (_currentVisualCamera.OverrideNearPlane)
                {
                    _mainCamera.nearClipPlane = (_currentVisualCamera.IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                        _currentVisualCamera.CrrentCamera.nearClipPlane :
                        Mathf.Lerp(_mainCamera.nearClipPlane, _currentVisualCamera.CrrentCamera.nearClipPlane, _currentVisualCamera.Interpolation);
                }
                else
                {
                    _mainCamera.nearClipPlane = (_currentVisualCamera.IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                        mainCameraDesInfo.lensSetting.NearClipPlane :
                        Mathf.Lerp(_mainCamera.nearClipPlane, mainCameraDesInfo.lensSetting.NearClipPlane, _currentVisualCamera.Interpolation);
                }

                if (_currentVisualCamera.OverrideFarPlane)
                {
                    _mainCamera.farClipPlane = (_currentVisualCamera.IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                        _currentVisualCamera.CrrentCamera.farClipPlane :
                        Mathf.Lerp(_mainCamera.farClipPlane, _currentVisualCamera.CrrentCamera.farClipPlane, _currentVisualCamera.Interpolation);
                }
                else
                {
                    _mainCamera.farClipPlane = (_currentVisualCamera.IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                                            mainCameraDesInfo.lensSetting.FarClipPlane :
                                            Mathf.Lerp(_mainCamera.farClipPlane, mainCameraDesInfo.lensSetting.FarClipPlane, _currentVisualCamera.Interpolation);
                }
                
                _mainCamera.renderingPath = _currentVisualCamera.OverrideRenderingPath ? _currentVisualCamera.CrrentCamera.renderingPath : mainCameraDesInfo.lensSetting.RenderingPath;
                _mainCamera.useOcclusionCulling = _currentVisualCamera.OverrideOcclusionCulling ? _currentVisualCamera.CrrentCamera.useOcclusionCulling : mainCameraDesInfo.lensSetting.UseOcclusionCulling;
                _mainCamera.allowHDR = _currentVisualCamera.OverrideAllowHDR ? _currentVisualCamera.CrrentCamera.allowHDR : mainCameraDesInfo.lensSetting.AllowHDR;
                _mainCamera.allowMSAA = _currentVisualCamera.OverrideAllowMSAA ? _currentVisualCamera.CrrentCamera.allowMSAA : mainCameraDesInfo.lensSetting.AllowMSAA;
            }

            //子相机参数对齐
            for (int i = 0; i < _subCameras.Count; i++)
            {
                Camera sub = _subCameras[i];

                if (sub.renderingPath != _mainCamera.renderingPath)
                {
                    sub.renderingPath = _mainCamera.renderingPath;
                }

                sub.orthographicSize = _mainCamera.orthographicSize;
                sub.orthographic = _mainCamera.orthographic;
                sub.nearClipPlane = _mainCamera.nearClipPlane;
                sub.farClipPlane = _mainCamera.farClipPlane;
                sub.fieldOfView = _mainCamera.fieldOfView;

                sub.useOcclusionCulling = _mainCamera.useOcclusionCulling;
                sub.allowHDR = _mainCamera.allowHDR;
                sub.allowMSAA = _mainCamera.allowMSAA;

                sub.projectionMatrix = _mainCamera.projectionMatrix;

            }
        }

        //------

        protected void _initExHandleEvents()
        {
            //初始化 QualityLevel缓存
            _qualityLevelCache = QualitySettings.GetQualityLevel();
        }

        protected void _catchExHandleEvents()
        {
            int q = QualitySettings.GetQualityLevel();
            if (!_qualityLevelCache.Equals(q))
            {
                _qualityLevelCache = q;
                if (OnQualityLevelChanged != null)
                {
                    OnQualityLevelChanged(q);
                }
            }
        }

        internal void SortEffectComponents()
        {
            //更新list排序
            m_peclist.Sort((a, b) =>
            {
                if (a.Level > b.Level)
                {
                    return 1;
                }
                else if (a.Level < b.Level)
                {
                    return -1;
                }
                return 0;
            });
            m_peclist.Reverse();
        }

        internal FLPostEffectController FLPostEffectController;
        internal readonly Dictionary<string, List<IFLPostEffectComponent>> m_postFLEffectComponents = new Dictionary<string, List<IFLPostEffectComponent>>();
        internal readonly List<IFLPostEffectComponent> m_peclist = new List<IFLPostEffectComponent>();

        internal void PostEffectComponentsToList()
        {
            m_peclist.Clear();
            foreach (List<IFLPostEffectComponent> list in m_postFLEffectComponents.Values)
            {
                m_peclist.Add(list[0]);
            }
        }

        public void AddPostEffectComponent(IFLPostEffectComponent component)
        {
            string key = component.ScriptName;
            if (!m_postFLEffectComponents.ContainsKey(key))
            {
                List<IFLPostEffectComponent> list = new List<IFLPostEffectComponent>();
                list.Add(component);
                m_postFLEffectComponents.Add(component.ScriptName, list);

            }
            else
            {
                m_postFLEffectComponents[key].Insert(0, component);
            }

            PostEffectComponentsToList();
            SortEffectComponents();

            if (FLPostEffectController && m_peclist.Count > 0)
            {
                FLPostEffectController.enabled = true;
            }
        }

        public void RemovePostEffectComponent(IFLPostEffectComponent component)
        {
            string key = component.ScriptName;
            if (m_postFLEffectComponents.ContainsKey(key))
            {

                m_postFLEffectComponents[key].Remove(component);

                if (m_postFLEffectComponents[key].Count == 0)
                {
                    m_postFLEffectComponents.Remove(component.ScriptName);
                }

                PostEffectComponentsToList();
                SortEffectComponents();

                if (FLPostEffectController && m_peclist.Count == 0)
                {
                    FLPostEffectController.enabled = false;
                }
            }
        }

    }

}

