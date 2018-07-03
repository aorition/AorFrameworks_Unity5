using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public class UIManager : ManagerBase
    {

        //@@@ 静态方法实现区

        private static string _NameDefine = "__AorUISystem";

        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 创建带有独立GameObject的Instance
        /// </summary>
        public static UIManager CreateInstance(Transform parenTransform = null)
        {
            return ManagerBase.CreateInstance<UIManager>(ref _instance, _NameDefine, parenTransform);
        }

        /// <summary>
        /// 在目标GameObject上的创建Instance
        /// </summary>
        public static UIManager CreateInstanceOnGameObject(GameObject target)
        {
            return ManagerBase.CreateInstanceOnGameObject<UIManager>(ref _instance, target);
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
            ManagerBase.VerifyUniqueOnInit(ref _instance, this, () =>
            {
                gameObject.name = _NameDefine;
            });
        }

        protected override void OnUnSetupedStart()
        {
            if (!_isSetuped && !_isInit)
            {
                //加载默认描述文件进行初始化
                setup(UIManagerSettingAsset.Normal());
            }
            else if (_isSetuped && !_isInit)
            {
                __init();
            }
        }
        
        protected override void OnDestroy()
        {
            //
            base.OnDestroy();
            ManagerBase.VerifyUniqueOnDispose(ref _instance, this);
        }

        protected override void init()
        {

            if (_info.InfoList != null)
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

        }

        //=======================================================================

        #region setup方法集

        public void setup(UIManagerSettingAsset settingAsset)
        {
            if (!settingAsset) return;
            _info = settingAsset;
            _isSetuped = true;
            init();
        }

        #endregion

        protected UIManagerSettingAsset _info;
        protected readonly Dictionary<string, Canvas> _CanvasDic = new Dictionary<string, Canvas>();
        protected readonly Dictionary<string, Dictionary<string,RectTransform>> _LayerDic = new Dictionary<string, Dictionary<string, RectTransform>>();

        #region Canvas 方法集

        public void AddCanvas(UICanvasInfo info)
        {
            if (!_CanvasDic.ContainsKey(info.Name))
            {

                GameObject go = new GameObject(info.Name);
                go.layer = 5;
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
                    camera.depth = info.CameraInfo.Depth;
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
                canvas.planeDistance = info.PlaneDistance;

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
//            if (_CanvasDic.ContainsKey(name))
//            {
//                return _CanvasDic[name];
//            }
//            else
//            {
//                return null;
//            }
            
            //??? 这个是否效率很高点?
            Canvas result = null;
            _CanvasDic.TryGetValue(name, out result);
            return result;
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

        #region Canvas Camera 方法集

        /// <summary>
        /// 获取Canvas的支持相机(如果有)
        /// </summary>
        public Camera GetCanvasCamera(string name)
        {
            Canvas canvas = GetCanvas(name);
            if (canvas) return canvas.worldCamera;
            return null;
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

                    Vector2 pivot = Vector2.zero;
                    switch (layerInfo.PovitState)
                    {
                        case UILayerPovitState.TopLeft:
                            pivot = new Vector2(0, 1);
                            break;
                        case UILayerPovitState.Top:
                            pivot = new Vector2(0.5f, 1);
                            break;
                        case UILayerPovitState.TopRight:
                            pivot = new Vector2(1, 1);
                            break;
                        case UILayerPovitState.Left:
                            pivot = new Vector2(0, 0.5f);
                            break;
                        case UILayerPovitState.Right:
                            pivot = new Vector2(1, 0.5f);
                            break;
                        case UILayerPovitState.BottomLeft:
                            pivot = new Vector2(0, 0);
                            break;
                        case UILayerPovitState.Bottom:
                            pivot = new Vector2(0.5f, 0);
                            break;
                        case UILayerPovitState.BottomRight:
                            pivot = new Vector2(1f, 0);
                            break;
                        default:
                            pivot = new Vector2(0.5f, 0.5f);
                            break;
                    }

                    RectTransform rt = UIRuntimeUtility.CreateUI_base(layerInfo.Name, parentCanvas.transform, 0, 0, 0, 0, 0, 0, 1, 1, pivot.x, pivot.y);

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
            //            if (_CanvasDic.ContainsKey(canvasName))
            //            {
            //                if (_LayerDic[canvasName].ContainsKey(layerName))
            //                {
            //                    return _LayerDic[canvasName][layerName];
            //                }
            //            }
            //            return null;

            //??? 是否效率会高一点??
            if (_CanvasDic.ContainsKey(canvasName))
            {
                RectTransform result = null;
                _LayerDic[canvasName].TryGetValue(layerName, out result);
                return result;
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
