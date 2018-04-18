using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public class AorUIManager : MonoBehaviour
    {

        private static string _NameDefine = "__AorUISystem";

        private static bool _dontDestroyOnLoad = true;
        private static Transform _parentTransform = null;

        private static AorUIManager _findOrCreateUIManager()
        {
            GameObject go = null;
            if (_parentTransform)
            {
                Transform t = _parentTransform.Find(_NameDefine);
                if (t) go = t.gameObject;
            }

            if (!go) go = GameObject.Find(_NameDefine);

            if (go)
            {
                AorUIManager gm = go.GetComponent<AorUIManager>();
                if (gm)
                {
                    return gm;
                }
                else
                {
                    return go.AddComponent<AorUIManager>();
                }
            }
            else
            {
                try
                {
                    go = new GameObject(_NameDefine);
                    if (_parentTransform) go.transform.SetParent(_parentTransform, false);
                    if (Application.isPlaying && _dontDestroyOnLoad && !_parentTransform) GameObject.DontDestroyOnLoad(go);
                    return go.AddComponent<AorUIManager>();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        private static AorUIManager _instance;
        public static AorUIManager instance
        {
            get
            {
                return _instance;
            }
        }

        public static AorUIManager GetInstance(Transform parenTransform = null, bool dontDestroyOnLoad = true)
        {

            _parentTransform = parenTransform;
            _dontDestroyOnLoad = dontDestroyOnLoad;

            if (_instance == null)
            {
                _instance = _findOrCreateUIManager();
            }
            return _instance;
        }

        public static void RequestUIManager(Action GraphicsManagerIniteDoSh)
        {
            GetInstance().AddUIManagerInited(GraphicsManagerIniteDoSh);
        }

        //=====================================================

        protected bool _isSetuped = false;
        protected bool _isInit = false;
        public bool isInit
        {
            get { return _isInit; }
        }

        protected Action _AfterInitDo;
        public void AddUIManagerInited(Action doSh)
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

        #region setup方法集

        public void setup(UIManagerSettingAsset settingAsset)
        {
            if (!settingAsset) return;
            _info = settingAsset;
            _isSetuped = true;
            init();
        }

        #endregion

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
                setup(UIManagerSettingAsset.Normal());
            }
            else if (_isSetuped && !_isInit)
            {
                init();
            }
        }
        
        protected void init()
        {

            if (_isInit) return;

            if (Application.isPlaying && _dontDestroyOnLoad && !_parentTransform)
            {
               GameObject.DontDestroyOnLoad(gameObject);
            }

            if(_info.InfoList != null)
            for (int i = 0; i < _info.InfoList.Count; i++)
            {
                UICanvasInfo canvasInfo = _info.InfoList[i];
                AddCanvas(canvasInfo);

                UILayerGroup layers = _info.LayerList[i];
                for (int j = 0; j < layers.Count; j++)
                {
                    UILayer uiLayer = layers[j];
                    AddLayer(uiLayer, canvasInfo.Name);
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
        }

        protected UIManagerSettingAsset _info;
        protected readonly Dictionary<string, Canvas> _CanvasDic = new Dictionary<string, Canvas>();
        protected readonly Dictionary<string, Dictionary<string,RectTransform>> _LayerDic = new Dictionary<string, Dictionary<string, RectTransform>>();

        #region Canvas 方法集

        public void AddCanvas(UICanvasInfo info)
        {
            if (!_CanvasDic.ContainsKey(info.Name))
            {

                GameObject go = new GameObject(info.Name);
                go.transform.SetParent(transform, false);
                Camera camera = null;
                if (info.RenderMode == RenderMode.ScreenSpaceCamera || info.RenderMode == RenderMode.WorldSpace)
                {
                    GameObject camGo =
                        new GameObject(info.Name + "_cam" + (info.RenderMode == RenderMode.WorldSpace ? "_evt" : ""));
                    camGo.transform.SetParent(transform, false);

                    camera = camGo.AddComponent<Camera>();

                    camera.clearFlags = info.CameraInfo.ClearFlags;
                    camera.backgroundColor = info.CameraInfo.Background;
                    camera.cullingMask = info.CameraInfo.CullingMask;
                    camera.orthographic = info.CameraInfo.Orthographic;
                    camera.orthographicSize = info.CameraInfo.OrthographicSize;
                    camera.fieldOfView = info.CameraInfo.FieldOfView;
                    camera.nearClipPlane = info.CameraInfo.NearClipPlane;
                    camera.farClipPlane = info.CameraInfo.FarClipPlane;
                    camera.rect = info.CameraInfo.ViewportRect;
                    camera.renderingPath = info.CameraInfo.RenderingPath;
                    camera.targetTexture = info.CameraInfo.TargetTexture;
                    camera.useOcclusionCulling = info.CameraInfo.OcclusionCulling;
                    camera.allowHDR = info.CameraInfo.AllowHDR;
                    camera.allowMSAA = info.CameraInfo.AllowMSAA;
                }

                Canvas canvas = go.AddComponent<Canvas>();

                canvas.renderMode = info.RenderMode;
                canvas.sortingOrder = info.SortOrder;
                canvas.pixelPerfect = info.PixelPerfect;
                canvas.sortingLayerName = info.SortingLayerName;
                canvas.targetDisplay = info.TargetDisplay;
                canvas.additionalShaderChannels = info.AdditionalShaderChannels;

                canvas.worldCamera = camera;

                canvas.referencePixelsPerUnit = info.ReferencePixelsPerUnit;
                canvas.scaleFactor = info.ScaleFactor;

                CanvasScaler Scaler = go.AddComponent<CanvasScaler>();
                Scaler.uiScaleMode = info.ScaleMode;
                Scaler.scaleFactor = info.ScaleFactor;
                Scaler.referencePixelsPerUnit = info.ReferencePixelsPerUnit;
                Scaler.referenceResolution = info.ReferenceResolution;
                Scaler.screenMatchMode = info.ScreenMatchMode;
                Scaler.matchWidthOrHeight = info.MatchWidthOrHeight;
                Scaler.physicalUnit = info.PhysicalUnit;
                Scaler.defaultSpriteDPI = info.DefaultSpriteDPI;

                AorGraphicRaycaster raycaster = go.AddComponent<AorGraphicRaycaster>();
                raycaster.ignoreReversedGraphics = info.IgnoreReversedGraphic;
                raycaster.blockingObjects = info.BlockingObjects;
                raycaster.BlockingMask = info.BlockingMask;

                _CanvasDic.Add(info.Name, canvas);
                _LayerDic.Add(info.Name, new Dictionary<string, RectTransform>());

            }
        }

        public List<Canvas> GetCanvasList()
        {
            List<Canvas> list = new List<Canvas>();
            foreach (Canvas canvas in _CanvasDic.Values)
            {
                list.Add(canvas);
            }
            return list;
        }

        public Canvas GetCanvas(string name)
        {
            if (_CanvasDic.ContainsKey(name))
            {
                return _CanvasDic[name];
            }
            return null;
        }

        public void RemoveCanvas(string name)
        {
            if (_CanvasDic.ContainsKey(name))
            {

                _LayerDic.Remove(name);

                Canvas canvas = _CanvasDic[name];
                _CanvasDic.Remove(name);

                if (canvas.worldCamera)
                {
                    Camera cam = canvas.worldCamera;
                    if (Application.isPlaying)
                    {
                        Destroy(cam.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(cam.gameObject);
                    }
                    canvas.worldCamera = null;
                }

                if (Application.isPlaying)
                {
                    Destroy(canvas.gameObject);
                }
                else
                {
                    DestroyImmediate(canvas.gameObject);
                }
            }
        }

        #endregion

        #region Layer 方法集

        public List<string> GetLayerNamesInAllCanvas()
        {
            List<string> canvasKeys = new List<string>(_LayerDic.Keys);
            List<string> list = new List<string>();
            for (int i = 0; i < canvasKeys.Count; i++)
            {
                list.AddRange(_LayerDic[canvasKeys[i]].Keys);
            }
            return list;
        }

        public List<string> GetLayerNames(string canvasName = "Default")
        {
            if (_CanvasDic.ContainsKey(canvasName))
            {
                List<string> list = new List<string>(_LayerDic[canvasName].Keys);
                return list;
            }
            return null;
        }

        public RectTransform AddLayer(UILayer layerInfo, string canvasName = "Default")
        {
            if (_CanvasDic.ContainsKey(canvasName))
            {
                if (!_LayerDic[canvasName].ContainsKey(layerInfo.Name))
                {

                    Canvas parentCanvas = _CanvasDic[canvasName];

                    RectTransform rt = UiRuntimeUtility.CreateUI_base(layerInfo.Name, parentCanvas.transform);

                    if (layerInfo.UseInnerCanvas)
                    {

                        Canvas canvas = rt.gameObject.AddComponent<Canvas>();
                        canvas.overrideSorting = layerInfo.OverrideSorting;
                        canvas.sortingOrder = layerInfo.SortOrder;

                        AorGraphicRaycaster graphicRaycaster = rt.gameObject.AddComponent<AorGraphicRaycaster>();
                        graphicRaycaster.ignoreReversedGraphics = layerInfo.IgnoreReversedGraphics;
                        graphicRaycaster.blockingObjects = layerInfo.BlockingObjects;
                        graphicRaycaster.BlockingMask = layerInfo.BlockingMask;

                    }

                    if (layerInfo.UseCanvasGroup)
                    {
                        CanvasGroup canvasGroup = rt.gameObject.AddComponent<CanvasGroup>();
                        canvasGroup.alpha = layerInfo.Alpha;
                        canvasGroup.interactable = layerInfo.Interactable;
                        canvasGroup.blocksRaycasts = layerInfo.BlocksRaycasts;
                        canvasGroup.ignoreParentGroups = layerInfo.IgnoreParentGroups;
                    }

                    _LayerDic[canvasName].Add(layerInfo.Name, rt);

                    return rt;
                }
            }
            return null;
        }

        public RectTransform GetLayer(string layerName, string canvasName = "Default")
        {
            if (_CanvasDic.ContainsKey(canvasName))
            {
                if (_LayerDic[canvasName].ContainsKey(layerName))
                {
                    return _LayerDic[canvasName][layerName];
                }
            }
            return null;
        }

        public void RemoveLayer(string layerName, string canvasName = "Default")
        {
            if (_CanvasDic.ContainsKey(canvasName))
            {
                if (_LayerDic[canvasName].ContainsKey(layerName))
                {
                    RectTransform rt = _LayerDic[canvasName][layerName];
                    _LayerDic[canvasName].Remove(layerName);
                    if (Application.isPlaying)
                    {
                        Destroy(rt.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(rt.gameObject);
                    }
                }
            }
        }

        #endregion
        
    }
}
