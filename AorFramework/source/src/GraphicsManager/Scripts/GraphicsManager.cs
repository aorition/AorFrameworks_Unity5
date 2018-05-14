using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Graphic.Utility;
using UnityEngine;

namespace Framework.Graphic
{

    //[ExecuteInEditMode]
    public class GraphicsManager : MonoBehaviour
    {

        //========= Manager 模版 =============================
        //
        // 基于MonoBehavior的Manager类 需要遵循的约定:
        //
        // 1. 采用_instance字段保存静态单例.
        // 2. 提供 _findOrCreateXXX的私有静态方法来实现查找或者创建承载该Manager的GameObject的方法
        // 3. 非自启动Manager必须提供CreateInstance静态方法.
        // 4. 提供Request静态方法.
        // 5. 提供IsInit静态方法判定改Manager是否初始化
        // 6. MonoBehaviour.Awake中必须加入单例限制代码
        //
        //=============== 基于MonoBehavior的Manager====================

        private static string _NameDefine = "GraphicsManager";

        private static GraphicsManager _findOrCreateGraphicsManager(Transform parenTransform = null)
        {
            GameObject go = null;
            if (parenTransform)
            {
                Transform t = parenTransform.Find(_NameDefine);
                if (t) go = t.gameObject;
            }

            if(!go) go = GameObject.Find(_NameDefine);

            if (go)
            {

                if (parenTransform) go.transform.SetParent(parenTransform, false);
                GraphicsManager gm = go.GetComponent<GraphicsManager>();
                if (gm)
                {
                    return gm;
                }
                else
                {
                    return go.AddComponent<GraphicsManager>();
                }
            }
            else
            {
                go = new GameObject(_NameDefine);
                if (parenTransform) go.transform.SetParent(parenTransform, false);
//                if (Application.isPlaying && _dontDestroyOnLoad && !_parentTransform) GameObject.DontDestroyOnLoad(go);
                return go.AddComponent<GraphicsManager>();
            }
        }

        private static GraphicsManager _instance;

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

        public static GraphicsManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public static GraphicsManager CreateInstance(Transform parenTransform = null)
        {
            if (_instance == null)
            {
                _instance = _findOrCreateGraphicsManager(parenTransform);
                _instance.parentTransform = parenTransform;
            }
            else if (parenTransform)
            {
                _instance.transform.SetParent(parenTransform, false);
                _instance.parentTransform = parenTransform;
            }
            return _instance;
        }

        public static void Request(Action GraphicsManagerIniteDoSh)
        {
            CreateInstance().AddGraphicsManagerInited(GraphicsManagerIniteDoSh);
        }

        public static bool IsInit()
        {
            return _instance && _instance._isInit;
        }

        //=====================================================

        [Tooltip("使用FixedUpdate刷新")]
        public bool UseFixedUpdate = false;

        [Tooltip("是否允许VisualCamera使用参数覆盖")]
        public bool AllowVisualCameraParamCover = true;

        [Tooltip("在一次Update中忽略缓动插值")]
        public bool IgnoreInterpolationOnce = false;

        protected Transform _parentTransform;
        public Transform parentTransform
        {
            set { _parentTransform = value; }
        }

        protected bool _isSetuped = false;
        protected bool _isInit = false;

        protected Action _AfterInitDo;
        public void AddGraphicsManagerInited(Action doSh)
        {
            if (_isInit)
            {
                doSh();
            }
            else
            {
                _AfterInitDo += doSh;
            }
        }

        public Action<Camera, GCamGDesInfo> OnMainCameraInited;
        public Action<Camera, GCamGDesInfo> OnSubCameraInited;

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

        private float _deltaTime = 0;

        protected readonly List<Camera> _subCameras = new List<Camera>();

        protected readonly List<VisualCamera> _visualCameras = new List<VisualCamera>();
        protected VisualCamera _currentVisualCamera;

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
            init();
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

//                if (_mainCamera && _GCGinfo)
//                {
                    //                    GraphicsCamUtility.ApplayCamParamToCamera(_mainCamera, _GCGinfo);
//                    GraphicsCamUtility.ApplyDesDataToCameraFormDesInfo(_mainCamera, _GCGinfo.MainCamDesInfo);
//                }
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
                if (a.Level > b.Level)
                {
                    return -1;
                }
                else if (a.Level < b.Level)
                {
                    return 1;
                }
                return 0;
            });

            int idx = _visualCameras.FindIndex(v => v.Solo);
            _currentVisualCamera = idx.Equals(-1) ? _visualCameras[0] : _visualCameras[idx];

            Camera.SetupCurrent(_currentVisualCamera.CrrentCamera);

        }

        public void RefreshCurrentVisualCamera()
        {
            _SortAndGetCurrentVisualCamera();
            _updateMainCameraPos();
        }

        protected void Awake()
        {
            //单例限制
            if (_instance != null && _instance != this)
            {
                GameObject.Destroy(this);
            }else if (_instance == null)
            {
                _instance = this;
                gameObject.name = _NameDefine;
            }
        }

        protected void Start()
        {
            if (!_isSetuped && !_isInit)
            {
                //加载默认描述文件进行初始化
                GraphicsSettingAsset def = ScriptableObject.CreateInstance<GraphicsSettingAsset>();
                def.MainCamDesInfo = GCamGDesInfo.Main();
                Setup(def);
            }
            else if (_isSetuped && !_isInit)
            {
                init();
            }
        }

        protected void OnDestroy()
        {

            OnMainCameraInited = null;
            OnSubCameraInited = null;
            _AfterInitDo = null;

            if (Instance != null && Instance == this)
            {
                _instance = null;
            }
        }

        private Vector3 _baseMainCameraPos;
        private Quaternion _baseMainCameraRotate;
        protected void _updateMainCameraPos()
        {
            if (_currentVisualCamera && _mainCamera)
            {

                if (IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0))
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

            }

            //强制设置false
            IgnoreInterpolationOnce = false;
        }

        protected void init()
        {

            if (_isInit) return;

            UseFixedUpdate = _GCGinfo.UseFixedUpdate;
            AllowVisualCameraParamCover = _GCGinfo.AllowVCamParaCover;

            //自动占用MainCamera
            _mainCamera = Camera.main;
            if (!_mainCamera)
            {
                _mainCamera = new GameObject().AddComponent<Camera>();
                _mainCamera.gameObject.tag = "MainCamera";
            }

            _mainCamera.gameObject.SetActive(false);
            if (_parentTransform) _mainCamera.transform.SetParent(_parentTransform, false);

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
                    _subCameras.Add(cam);

                    cam.transform.SetParent(_mainCamera.transform, false);

                    GraphicsCamUtility.ApplyDesDataToCameraFormDesInfo(cam, info);
                }

            }

            _isInit = true;
            StartCoroutine(_afterInitRun());
        }

        IEnumerator _afterInitRun()
        {
            yield return new WaitForEndOfFrame();
            if (_AfterInitDo != null)
            {
                Action tmpDo = _AfterInitDo;
                tmpDo();
                _AfterInitDo = null;
            }
            _mainCamera.gameObject.SetActive(true);
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
            _updateMainCameraPos();
            //
            _RefreshPostEffectComponents();
        }

        public void AddPostEffectComponent(IGMPostEffectComponent component)
        {
            if (!_postEffectComponents.Contains(component))
            {
                _postEffectComponents.Add(component);
            }
        }

        public void RemovePostEffectComponent(IGMPostEffectComponent component)
        {
            if (_postEffectComponents.Contains(component))
            {
                _postEffectComponents.Remove(component);
            }
        }

        protected readonly List<IGMPostEffectComponent> _postEffectComponents = new List<IGMPostEffectComponent>();

        protected void _RefreshPostEffectComponents()
        {
            for (int i = 0; i < _postEffectComponents.Count; i++)
            {
                _postEffectComponents[i].UpdateGMPostEffect();
            }
        }

        protected void process()
        {

            if (!_mainCamera) return;
            
            //更新所有VisualCamera附加计算
            for (int i = 0; i < _visualCameras.Count; i++)
            {
                _visualCameras[i].UpdateExtension(_deltaTime);
            }

            //刷新相机组优先级
            _SortAndGetCurrentVisualCamera();

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
                    _mainCamera.backgroundColor = (IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                        _currentVisualCamera.CrrentCamera.backgroundColor :
                        Color.Lerp(_mainCamera.backgroundColor, _currentVisualCamera.CrrentCamera.backgroundColor, _currentVisualCamera.Interpolation);
                }
                else
                {
                    _mainCamera.backgroundColor = (IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                        mainCameraDesInfo.lensSetting.BackgroundColor :
                        Color.Lerp(_mainCamera.backgroundColor, mainCameraDesInfo.lensSetting.BackgroundColor, _currentVisualCamera.Interpolation);
                }

                if (_currentVisualCamera.OverrideOrthographic)
                {
                    _mainCamera.orthographic = _currentVisualCamera.CrrentCamera.orthographic;
                    _mainCamera.orthographicSize = (IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ? 
                        _currentVisualCamera.CrrentCamera.orthographicSize :
                        Mathf.Lerp(_mainCamera.orthographicSize, _currentVisualCamera.CrrentCamera.orthographicSize, _currentVisualCamera.Interpolation);
                }
                else
                {
                    _mainCamera.orthographic = mainCameraDesInfo.lensSetting.isOrthographicCamera;
                    _mainCamera.orthographicSize = (IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                                            mainCameraDesInfo.lensSetting.OrthographicSize :
                                            Mathf.Lerp(_mainCamera.orthographicSize, mainCameraDesInfo.lensSetting.OrthographicSize, _currentVisualCamera.Interpolation);
                }

                if (_currentVisualCamera.OverrideFieldOfView)
                {
                    _mainCamera.fieldOfView = (IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ? 
                        _currentVisualCamera.CrrentCamera.fieldOfView :
                        Mathf.Lerp(_mainCamera.fieldOfView, _currentVisualCamera.CrrentCamera.fieldOfView, _currentVisualCamera.Interpolation);
                }
                else
                {
                    _mainCamera.fieldOfView = (IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                        mainCameraDesInfo.lensSetting.FieldOfView :
                        Mathf.Lerp(_mainCamera.fieldOfView, mainCameraDesInfo.lensSetting.FieldOfView, _currentVisualCamera.Interpolation);
                }

                if (_currentVisualCamera.OverrideNearPlane)
                {
                    _mainCamera.nearClipPlane = (IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                        _currentVisualCamera.CrrentCamera.nearClipPlane :
                        Mathf.Lerp(_mainCamera.nearClipPlane, _currentVisualCamera.CrrentCamera.nearClipPlane, _currentVisualCamera.Interpolation);
                }
                else
                {
                    _mainCamera.nearClipPlane = (IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                        mainCameraDesInfo.lensSetting.NearClipPlane :
                        Mathf.Lerp(_mainCamera.nearClipPlane, mainCameraDesInfo.lensSetting.NearClipPlane, _currentVisualCamera.Interpolation);
                }

                if (_currentVisualCamera.OverrideFarPlane)
                {
                    _mainCamera.farClipPlane = (IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
                        _currentVisualCamera.CrrentCamera.farClipPlane :
                        Mathf.Lerp(_mainCamera.farClipPlane, _currentVisualCamera.CrrentCamera.farClipPlane, _currentVisualCamera.Interpolation);
                }
                else
                {
                    _mainCamera.farClipPlane = (IgnoreInterpolationOnce || _currentVisualCamera.Interpolation.Equals(0)) ?
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
            }

        }

    }

}

