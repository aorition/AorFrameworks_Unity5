using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using AorBaseUtility;
using Framework.module;
using DG.Tweening;
using UnityEngine.UI;
using YoukiaCore;
//using YoukiaUnity.App;
using YoukiaUnity.Graphics.FastShadowProjector;
using YoukiaUnity.Misc;
//using YoukiaUnity.Resource;

namespace YoukiaUnity.Graphics
{
    public class GraphicsLauncher : MonoBehaviour
    {

        void setupGraphic()
        {
            //建立管理器
            GraphicsManager.CreateInstance();
            //设置绘制卡
            GraphicsManager.GetInstance().SetupDrawCrad = () =>
            {
                BaseDrawCard baseCard = new BaseDrawCard();
                GraphicsManager.GetInstance().DrawCard = baseCard;
            };
            //初始化
            GraphicsManager.GetInstance().Init(null, () =>
            {
                Launcher();
            });
        }

        void Awake()
        {
            Debug.Log(name);
            if (GraphicsManager.GetInstance() != null)
            {
                Launcher();
            }
            else if (!GraphicsManager.isInited)
            {
                StartAfterGraphicMgr(this, () =>
                {

                    setupGraphic();

                });

            }
            else
            {
                setupGraphic();
            }


        }

        protected virtual void Launcher()
        {


        }

        public static void StartAfterGraphicMgr(MonoBehaviour mono, Action finish)
        {
            if (GraphicsManager.isInited)
            {
                finish();
            }
            else
            {
                mono.StartCoroutine((IEnumerator)waitForMgr(finish));
            }

        }


        private static IEnumerator waitForMgr(Action finish)
        {

            while (GraphicsManager.isInited)
            {

                yield return 1;
            }

            finish();


        }
    }

    /// <summary>
    /// 图形管理器，提供图形性能选项
    /// </summary>
    public class GraphicsManager : SingletonManager<GraphicsManager>
    {
        /// <summary>
        /// 便捷访问
        /// </summary>
        /// <param name="notbeNULL">Ture:会保证返回一个GraphicsManager,False:在GraphicsManager没有被创建之前调用(或已删除)会返回NULL</param>
        /// <returns></returns>
        public static GraphicsManager GetInstance(bool notbeNULL = false)
        {
            return SingletonManager<GraphicsManager>.GetInstance(notbeNULL);

//            GraphicsManager gmr = Transform.FindObjectOfType<GraphicsManager>();
//            if (gmr != null)
//            {
//                return gmr;
//            }
//            else
//            {
//                GameObject instGo = GameObject.Find("GraphicsManager");
//                if (instGo != null)
//                {
//                    gmr = instGo.GetComponent<GraphicsManager>();
//                
//                    if (gmr != null)
//                    {
//                        return gmr;
//                    }
//                }
//            }
//
//            if (notbeNULL)
//            {
//                return GraphicsManager.CreateInstance();
//            }
//            else
//            {
//                return null;
//            }
        }

        public enum eCameraOpenState
        {
            None,
            KeepBlack,
            KeepWhite,
            AutoFadeIn,
        }

        protected class CameraParma
        {
            public float NearClip;
            public float FarClip;
            public float FogDestance;
            public float FogDestiy;

            public float VolumeFogOffset;
            public float VolumeFogDestiy;

        }

        public LightInfo SunLight;
        public Camera MainCamera;
        public CameraAnimation _mainCameraAnimation;
        public bool IsRenderToTexture;
        public IGraphicDraw DrawCard;

        private AtmosphereManager AtmosphereMgr;
        internal GlobalProjectorManager ShadowMgr;
        internal bool StopRender = false;

        private FPS Fps;
        private Vector2 screenSize;
        private Camera UIcamera;

        private Color cameraBackColor;

        private CameraParma TempCameraParma;
        private CameraParma ActiveCameraParma;
        private CameraParma ConfigCameraParma;


        //安装渲染效果卡
        public CallBack SetupDrawCrad;


        //抗锯齿 1 2 4 8
        public int AntiAliasing;

        public string FPS
        {

            get { return Fps.currFPS.ToString(); }

        }

        public Material RenderMaterial
        {

            get { return DrawCard.GetMaterial(IsRenderToTexture); }

        }

        public GlobalProjectorManager.ShadowResolutions _ShadowResolution = GlobalProjectorManager.ShadowResolutions.Normal_512;
        /// <summary>
        /// 阴影质量
        /// </summary>
        public GlobalProjectorManager.ShadowResolutions ShadowResolution
        {

            get { return _ShadowResolution; }
            set
            {


                if (_ShadowResolution != value)
                {
                    _ShadowResolution = value;
                    ShadowMgr.GlobalShadowResolution = (int)value;
                }

            }

        }



        private GlobalProjectorManager.ShadowType _Shadow = GlobalProjectorManager.ShadowType.None;
        /// <summary>
        /// 是否开阴影
        /// </summary>
        public GlobalProjectorManager.ShadowType ShadowType
        {

            get { return _Shadow; }
            set
            {


                if (_Shadow != value)
                {
                    _Shadow = value;
                    ShadowMgr.GlobalShadowType = value;
                }

            }

        }

        [SerializeField]
        [HideInInspector]
        private bool _showLandscape = true;

        /// <summary>
        /// 是否一直重绘远景
        /// </summary>
        public bool AlwayReDrawFarTexture = false;

        /// <summary>
        /// 绘制精度
        /// </summary>

        public enum RtSize
        {
            /// <summary>
            /// 原始大小
            /// </summary>
            Full = 1,
            /// <summary>
            /// 对半
            /// </summary>
            Half = 2,
            /// <summary>
            /// 四分之一
            /// </summary>
            Quarter = 4,
        }



        /// <summary>
        /// 摄像机远裁面
        /// </summary>
        public float CameraFarClip
        {

            get { return ActiveCameraParma.FarClip; }
            set
            {
                if (!Application.isPlaying)
                    return;

                ActiveCameraParma.FarClip = value;
                ModifyCameraClip();
            }

        }


        void OnDestroy()
        {
            _isInited = false;
        }


        /// <summary>
        /// 摄像机近裁面
        /// </summary>
        public float CameraNearClip
        {

            get { return ActiveCameraParma.NearClip; }
            set
            {


                if (!Application.isPlaying)
                    return;

                ActiveCameraParma.NearClip = value;
                ModifyCameraClip();
            }

        }


        //   private float _CameraMiddleClip;


        /// <summary>
        /// 大气雾距离
        /// </summary>

        public float FogDestance
        {
            get { return ActiveCameraParma.FogDestance; }
            set
            {

                if (!Application.isPlaying)
                    return;

                ActiveCameraParma.FogDestance = value;
                ModifyCameraClip();

                //AtmosphereMgr.FogDestance = value;
            }
        }

        /// <summary>
        /// 体积雾偏移
        /// </summary>

        public float VolumeFogOffset
        {
            get { return ActiveCameraParma.VolumeFogOffset; }
            set
            {

                if (!Application.isPlaying)
                    return;

                ActiveCameraParma.VolumeFogOffset = value;
                ModifyCameraClip();

                //AtmosphereMgr.FogDestance = value;
            }
        }

        /// <summary>
        /// 体积雾密度
        /// </summary>

        public float VolumeFogDestiy
        {
            get { return ActiveCameraParma.VolumeFogDestiy; }
            set
            {

                if (!Application.isPlaying)
                    return;

                ActiveCameraParma.VolumeFogDestiy = value;
                ModifyCameraClip();

                //AtmosphereMgr.FogDestance = value;
            }
        }





        public void SetCameraBackColor(Color col)
        {
            cameraBackColor = col;
            if (_CurrentSubCamera != null)
            {
                _CurrentSubCamera.camera.backgroundColor = cameraBackColor;
            }
        }

        /// <summary>
        /// 调整裁剪面
        /// </summary>
        private void ModifyCameraClip()
        {
            if (CurrentSubCamera != null)
            {

                if (CurrentSubCamera.OverrideGraphicSetting)
                {
                    TempCameraParma = new CameraParma();

                    if (CurrentSubCamera.OverrideNearClip != -1)
                        TempCameraParma.NearClip = CurrentSubCamera.OverrideNearClip;
                    else
                        TempCameraParma.NearClip = ActiveCameraParma.NearClip;


                    if (CurrentSubCamera.OverrideFarClip != -1)
                        TempCameraParma.FarClip = CurrentSubCamera.OverrideFarClip;
                    else
                        TempCameraParma.FarClip = ActiveCameraParma.FarClip;


                    if (CurrentSubCamera.OverrideFogDestance != -1)
                        TempCameraParma.FogDestance = CurrentSubCamera.OverrideFogDestance;
                    else
                        TempCameraParma.FogDestance = ActiveCameraParma.FogDestance;

                    if (CurrentSubCamera.OverrideFogDestiy != -1)
                        TempCameraParma.FogDestiy = CurrentSubCamera.OverrideFogDestiy;
                    else
                        TempCameraParma.FogDestiy = ActiveCameraParma.FogDestiy;

                    if (CurrentSubCamera.OverrideVolumeFogOffset != -1)
                        TempCameraParma.VolumeFogOffset = CurrentSubCamera.OverrideVolumeFogOffset;
                    else
                        TempCameraParma.VolumeFogOffset = ActiveCameraParma.VolumeFogOffset;

                    if (CurrentSubCamera.OverrideVolumeFogDestiy != -1)
                        TempCameraParma.VolumeFogDestiy = CurrentSubCamera.OverrideVolumeFogDestiy;
                    else
                        TempCameraParma.VolumeFogDestiy = ActiveCameraParma.VolumeFogDestiy;


                    ActiveCameraParma = TempCameraParma;
                }
                else
                {
                    ActiveCameraParma = ConfigCameraParma;
                }


                if (SkyCamera == null)
                {
                    CreateCharacterCamera();
                    CreateSkyCamera();
                    CreateEffectCamera(ref PreEffectCamera, "preEffect");
                    CreateEffectCamera(ref EffectCamera, "effect");
                    CreateEffectCamera(ref PostEffectCamera, "postEffect");


                }


                CurrentSubCamera.camera.nearClipPlane = ActiveCameraParma.NearClip;
                CurrentSubCamera.camera.clearFlags = CameraClearFlags.SolidColor;
                CurrentSubCamera.camera.backgroundColor = cameraBackColor;
                CurrentSubCamera.camera.nearClipPlane = ActiveCameraParma.NearClip;
                CurrentSubCamera.camera.farClipPlane = ActiveCameraParma.FarClip;

                AtmosphereMgr.FogDestance = ActiveCameraParma.FogDestance;
                AtmosphereMgr.FogDestiy = ActiveCameraParma.FogDestiy;
                AtmosphereMgr.VolumeFogOffset = ActiveCameraParma.VolumeFogOffset;
                AtmosphereMgr.VolumeFogDestiy = ActiveCameraParma.VolumeFogDestiy;

                setClipPlane(CharacterCamera);
                setClipPlane(PreEffectCamera);
                setClipPlane(EffectCamera);
                setClipPlane(PostEffectCamera);

            }
        }

        //可自定义到任何一个渲染位置的黑屏

        private GameObject _darkOcclusion;
        private float _darkDuration;
        private float _darkLum;
        private float _darkFade;
        private Material _darkMaterial;

        private void darkOcclusionUpdate()
        {
            if (_darkOcclusion != null)
            {
                if (_darkDuration > 0)
                {
                    if (!_darkOcclusion.activeInHierarchy)
                    {
                        _darkOcclusion.SetActive(true);
                    }
                    float lum = _darkMaterial.GetFloat("_Lum");
                    lum = YKMath.Lerp(lum, _darkLum, Time.deltaTime / _darkFade);
                    _darkMaterial.SetFloat("_Lum", lum);
                    _darkDuration -= Time.deltaTime;
                }
                else if (_darkOcclusion.activeInHierarchy)
                {
                    _darkLum = 1;
                    float lum = _darkMaterial.GetFloat("_Lum");
                    lum = YKMath.Lerp(lum, _darkLum, Time.deltaTime / _darkFade);
                    if (lum > 0.95f)
                    {
                        _darkOcclusion.SetActive(false);
                    }
                    else
                    {
                        _darkMaterial.SetFloat("_Lum", lum);
                    }
                }
            }

        }

        public void DarkOcclusion(float lum, float time, string layerName = "effect", float fade = 0.2f)
        {
            if (_darkOcclusion == null)
            {
                _darkOcclusion = GameObject.CreatePrimitive(PrimitiveType.Quad);
                _darkOcclusion.transform.SetParent(MainCamera.transform, false);
                _darkOcclusion.transform.localPosition = new Vector3(0, 0, 20);
                //                _darkMaterial = new Material(YKApplication.Instance.ConstShader["Custom/Other/DarkOcclusion"]);
                _darkMaterial = new Material(Shader.Find("Custom/Other/DarkOcclusion"));
                _darkMaterial.SetFloat("_Lum", 1);
                _darkOcclusion.GetComponent<MeshRenderer>().material = _darkMaterial;
                _darkOcclusion.layer = LayerMask.NameToLayer(layerName);
            }
            _darkLum = lum;
            _darkDuration = Math.Max(time, _darkDuration);
            _darkFade = fade;
        }




        private void setClipPlane(Camera camera)
        {
            if (camera != null)
            {
                //特效摄像机必须和主摄像机裁剪一样,否则深度出错
                camera.nearClipPlane = CurrentSubCamera.camera.nearClipPlane;
                camera.farClipPlane = CurrentSubCamera.camera.farClipPlane;
            }
        }

        /// <summary>
        /// 大气雾浓度
        /// </summary>
        public float FogDestiy
        {
            get { return ActiveCameraParma.FogDestiy; }
            set
            {

                if (!Application.isPlaying)
                    return;

                ActiveCameraParma.FogDestiy = value;
                ModifyCameraClip();

            }
        }

        /// <summary>
        /// debug模式
        /// </summary>
        public bool DebugMode = true;


        List<SubCameraInfo> SubCameraList = new List<SubCameraInfo>();

        /// <summary>
        /// 当前摄像机
        /// </summary>
        private SubCameraInfo _CurrentSubCamera;


        internal RenderTexture MainRt;

        private float MainRtReleaseTime = -1;

        internal Camera SkyCamera;
        internal Camera CharacterCamera;

        private Vector3 LastSubCameraPos;
        private Vector3 LastSubCameraRot;
        private static bool _isInited = false;

        internal MainDraw FinalDraw;
        public RawImage UIimage;





        public void SetDrawPassCount(int index)
        {
            FinalDraw.DrawPassCount = index;
        }

        [SerializeField]
        [HideInInspector]
        internal RtSize _FinalDrawRtSize;
        public RtSize FinalDrawRtSize
        {

            get { return _FinalDrawRtSize; }
            set
            {

                if (_FinalDrawRtSize == value)
                {
                    return;
                }

                _FinalDrawRtSize = value;



                if (Application.isPlaying && MainRt != null)
                {
                    DestoryMainRt();
                    CreateMainRt();
                    UpdateCameraRt();
                }

            }


        }



        /// <summary>
        /// 绘制到RT模式
        /// </summary>
        public void SetRenderTarget(RawImage image)
        {
            UIimage = image;
            UIimage.material = DrawCard.GetMaterial(true);
            IsRenderToTexture = true;

            if (MainRt != null)
            {
                UIimage.material.SetTexture("_MainTex", MainRt);
                image.texture = MainRt;
            }
        }

        /// <summary>
        /// 设置RT背景的透明度
        /// </summary>
        public void SetRenderTargetAlpha(float alpha)
        {


            if (UIimage != null)
            {
                UIimage.material.SetFloat("_AlphaAdd", alpha);
            }
        }

        /// <summary>
        /// 是否已经启动
        /// </summary>
        public static bool isInited
        {
            get { return _isInited; }
        }

        /// <summary>
        /// 当前摄像机
        /// </summary>
        public SubCameraInfo CurrentSubCamera
        {
            set
            {
                _CurrentSubCamera = value;
                if (value == null)
                    return;

                if (MainRt == null)
                {
                    CreateMainRt();
                }
                Log("CurrentSubCamera is:" + value.gameObject.name);
                _mainCameraAnimation.OnSwitch(_CurrentSubCamera.OpenState);
                ModifyCameraClip();
                UpdateCameraRt();

            }
            get
            {

                return _CurrentSubCamera;
            }
        }

        private void CreateSkyCamera()
        {
            Log("CreateSkyCamera ");
            GameObject tmp = new GameObject("SkyCamera");
            SkyCamera = tmp.AddComponent<Camera>();
            SkyCamera.transform.parent = transform;
            SkyCamera.transform.localPosition = _CurrentSubCamera.transform.position;
            SkyCamera.transform.localEulerAngles = _CurrentSubCamera.transform.eulerAngles;
            //  SkyCamera.targetTexture = MainRt;
            SkyCamera.fieldOfView = _CurrentSubCamera.camera.fieldOfView;
            SkyCamera.nearClipPlane = 10f;
            SkyCamera.farClipPlane = 5000;
            SkyCamera.depth = 15;
            SkyCamera.cullingMask = 1 << LayerMask.NameToLayer("sky");
            SkyCamera.clearFlags = CameraClearFlags.Nothing;
            SkyCamera.enabled = false;
            SkyCamera.hdr = true;
        }

        private void CreateCharacterCamera()
        {
            Log("CreateCharacterCamera ");
            GameObject tmp = new GameObject("CharacterCamera");
            CharacterCamera = tmp.AddComponent<Camera>();
            CharacterCamera.transform.parent = transform;
            CharacterCamera.transform.localPosition = _CurrentSubCamera.transform.position;
            CharacterCamera.transform.localEulerAngles = _CurrentSubCamera.transform.eulerAngles;
            CharacterCamera.fieldOfView = _CurrentSubCamera.camera.fieldOfView;
            CharacterCamera.depth = 10;
            CharacterCamera.cullingMask = 1 << LayerMask.NameToLayer("character");
            CharacterCamera.clearFlags = CameraClearFlags.Nothing;
            CharacterCamera.enabled = false;
            CharacterCamera.hdr = true;

        }


        internal Camera PreEffectCamera;
        internal Camera EffectCamera;
        internal Camera PostEffectCamera;


        private float CullingSize = 24;//剔除范围

        private void CreateEffectCamera(ref Camera effectCamera, string layer)
        {
            Log("CreateEffectCamera :" + layer);
            GameObject tmp = new GameObject(layer + "Camera");
            effectCamera = tmp.AddComponent<Camera>();
            // collider.enabled = false;
            effectCamera.transform.parent = transform;
            effectCamera.transform.position = _CurrentSubCamera.transform.position;
            effectCamera.transform.eulerAngles = _CurrentSubCamera.transform.eulerAngles;
            //  effectCamera.targetTexture = MainRt;
            effectCamera.clearFlags = CameraClearFlags.Nothing;
            effectCamera.fieldOfView = _CurrentSubCamera.camera.fieldOfView;
            effectCamera.depth = 20;
            effectCamera.cullingMask = 1 << LayerMask.NameToLayer(layer);
            effectCamera.enabled = false;
            effectCamera.hdr = true;


        }



        private void UpdateSkyCamera()
        {
            if (SkyCamera == null)
                return;

            SkyCamera.fieldOfView = CurrentSubCamera.camera.fieldOfView;
            SkyCamera.transform.position = _CurrentSubCamera.transform.position;
            SkyCamera.transform.eulerAngles = _CurrentSubCamera.transform.eulerAngles;

        }
        private void UpdateCharacterCamera()
        {
            if (CharacterCamera == null)
                return;

            CharacterCamera.fieldOfView = CurrentSubCamera.camera.fieldOfView;
            CharacterCamera.transform.position = _CurrentSubCamera.transform.position;
            CharacterCamera.transform.eulerAngles = _CurrentSubCamera.transform.eulerAngles;

        }



        private void UpdateEffectCamera(Camera effectCamera)
        {
            if (effectCamera == null)
                return;

            effectCamera.fieldOfView = CurrentSubCamera.camera.fieldOfView;
            effectCamera.transform.position = _CurrentSubCamera.transform.position;
            effectCamera.transform.eulerAngles = _CurrentSubCamera.transform.eulerAngles;

        }



        [SerializeField]
        [HideInInspector]
        private int _Quality = -1;


        /// <summary>
        /// 品质等级=unity设置内的品质等级
        /// </summary>
        public int GraphicsQuality
        {

            get
            {
                if (_Quality == -1)
                {
                    _Quality = QualitySettings.GetQualityLevel();
                }
                return _Quality;
            }
            set
            {
                _Quality = value;
                QualitySettings.SetQualityLevel(value);


            }

        }

        /// <summary>
        /// 注册摄像机
        /// </summary>
        /// <param name="cameraInfo">要注册的摄像机</param>
        public void RegisterSubCamera(SubCameraInfo cameraInfo)
        {
            if (!SubCameraList.Contains(cameraInfo))
            {

                SubCameraList.Add(cameraInfo);

            }
            UpdateCurrentSubCamera();
        }

        /// <summary>
        /// 移除摄像机
        /// </summary>
        /// <param name="cameraInfo"></param>
        public void RemoveSubCamera(SubCameraInfo cameraInfo)
        {
            if (gameObject == null)
                return;

            if (SubCameraList.Contains(cameraInfo))
            {
                SubCameraList.Remove(cameraInfo);
                Log("SubCamera Destory!");
            }

        }

        private void Log(string str)
        {
            if (DebugMode)
            {
                //                YoukiaCore.YoukiaCore.Log.Info(str);
                Debug.Log(str);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="onFinish">成功后的回调</param>
        public void Init(DisplayConfig config, CallBack onFinish)
        {
            //            DisplayConfig config = ConfigManager.Instance.Get<DisplayConfig>(789123);



            //FastShadowProjector结构生成
            ShadowMgr = GlobalProjectorManager.CreateInstance();
            ShadowMgr.transform.parent = transform;


            //AtmosphereManager结构生成
            GameObject obj = new GameObject("AtmosphereManager");
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            AtmosphereMgr = obj.AddComponent<AtmosphereManager>();

            //主摄像机
            obj = new GameObject("MainCamera");
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            MainCamera = obj.AddComponent<Camera>();
            _mainCameraAnimation = MainCamera.gameObject.AddComponent<CameraAnimation>();
            MainCamera.clearFlags = CameraClearFlags.Color;
            MainCamera.backgroundColor = Color.black;

            MainCamera.depth = 50;
            MainCamera.cullingMask = 1 << LayerMask.NameToLayer("Default");
            MainCamera.enabled = false;
            MainCamera.hdr = true;
            ShadowMgr.SetMainCamera(MainCamera);
            
            //FinalCamera结构生成
            obj = new GameObject("FinalCamera");
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            Camera cam = obj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Color;
            cam.backgroundColor = Color.black;
            cam.cullingMask = 0;
            FinalDraw = obj.AddComponent<MainDraw>();
            Fps = obj.AddComponent<FPS>();

            //drawCard插入
            if (SetupDrawCrad != null)
            {
                SetupDrawCrad();
            }

            //序列化数据读取
            ConfigCameraParma = new CameraParma();


            FinalDrawRtSize = config == null ? RtSize.Full : (RtSize)config.Resolution;

            ShadowType = config == null ? GlobalProjectorManager.ShadowType.ModelProjection : config.ShadowType;
            ShadowResolution = config == null ? GlobalProjectorManager.ShadowResolutions.VeryLow_128 : config.ShadowResolutions;

            AntiAliasing = config == null ? 1 : config.AntiAliasing;


            ConfigCameraParma.FogDestance = config == null ? 60 : config.FogDistance;
            ConfigCameraParma.FogDestiy = config == null ? 20 : config.FogDestiy;

            ConfigCameraParma.FarClip = config == null ? 3000 : config.FarCameraClip;
            ConfigCameraParma.NearClip = config == null ? 1f : config.NearCameraClip;

            screenSize = new Vector2(Screen.width, Screen.height);
            GraphicsQuality = config == null ? 0 : config.GraphicQuality;
            AtmosphereMgr.Init();
            ActiveCameraParma = ConfigCameraParma;
            UpdateCurrentSubCamera();
            FinalDraw.gameObject.SetActive(true);

            FinalDraw.enabled = true;
            FinalDraw.Init();
         

            //延迟初始化阴影系统
            ShadowMgr.Initialize();
            _isInited = true;

            if (onFinish != null)
                onFinish();

            //太阳光生产
            GameObject lgo = new GameObject("SunLight");
            lgo.transform.SetParent(transform, false);
            lgo.transform.localEulerAngles = new Vector3(45, 45, 0);
            Light l = lgo.AddComponent<Light>();
            l.type = LightType.Directional;
            l.intensity = 1;
            SunLight = lgo.AddComponent<LightInfo>();
            
        }

        public bool isSupportHdr()
        {
            return SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
        }

        private void CreateMainRt()
        {
            if (isSupportHdr())
                MainRt = new RenderTexture((int)screenSize.x / (int)FinalDrawRtSize, (int)screenSize.y / (int)FinalDrawRtSize, 24, RenderTextureFormat.ARGBHalf);
            else
                MainRt = new RenderTexture((int)screenSize.x / (int)FinalDrawRtSize, (int)screenSize.y / (int)FinalDrawRtSize, 24, RenderTextureFormat.ARGB32);

            MainRt.antiAliasing = AntiAliasing;
            MainRt.filterMode = FilterMode.Bilinear;

            if (UIimage != null)
            {
                UIimage.texture = MainRt;
                UIimage.material.SetTexture("_MainTex", MainRt);
            }
            Log("** 创建了新的主RenderTexture!");
        }

        private void UpdateCameraRt()
        {

            if (CurrentSubCamera != null)
            {
                setTargetTexture(MainCamera);
            }
            setTargetTexture(CharacterCamera);
            setTargetTexture(SkyCamera);
            setTargetTexture(PreEffectCamera);
            setTargetTexture(EffectCamera);
            setTargetTexture(PostEffectCamera);
        }

        void setTargetTexture(Camera camera)
        {

            if (camera != null && camera.targetTexture != MainRt)
                camera.targetTexture = MainRt;
        }

        private void DestoryMainRt()
        {
            if (CurrentSubCamera != null)
                CurrentSubCamera.camera.targetTexture = null;

            DestoryOtherCamera();

            MainRt.Release();
            MainRt = null;

        }

        private void DestoryOtherCamera()
        {
            cleanTargetTexture(SkyCamera);
            cleanTargetTexture(PreEffectCamera);
            cleanTargetTexture(EffectCamera);
            cleanTargetTexture(PostEffectCamera);
        }

        private void cleanTargetTexture(Camera camera)
        {
            if (camera != null)
                camera.targetTexture = null;
        }

        /// <summary>
        /// 设置屏幕遮罩颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetMaskColor(Color color)
        {

            if (UIimage != null)
            {
                UIimage.material.SetColor("_MaskColor", color);
            }
            else
            {
                RenderMaterial.SetColor("_MaskColor", color);
            }

        }
        /// <summary>
        /// 设置屏幕曝光度
        /// </summary>
        /// <param name="exp"></param>
        public void SetExposure(float exp)
        {
            if (UIimage != null)
            {
                UIimage.material.SetFloat("_Exposure", exp);
            }
            else
            {
                RenderMaterial.SetFloat("_Exposure", exp);
            }



        }

        public void UpdateCurrentSubCamera()
        {

            SubCameraInfo select = null;
            for (int i = 0; i < SubCameraList.Count; i++)
            {
                if (SubCameraList[i] == null)
                    continue;

                if (SubCameraList[i].gameObject.activeInHierarchy && (select == null || SubCameraList[i].Level > select.Level))
                {
                    select = SubCameraList[i];
                }

            }

            if (select != null)
            {
                CurrentSubCamera = select;
                MainRtReleaseTime = -1;
            }

        }

        /// <summary>
        /// 在摄像机列表中找到当前摄像机,并设置为激活
        /// </summary>
        /// <param name="info"></param>
        public void SetCurrentSubCamera(SubCameraInfo info)
        {
            if (info == null || info.gameObject == null || !info.gameObject.activeInHierarchy)
                return;

            for (int i = 0; i < SubCameraList.Count; i++)
            {
                if (SubCameraList[i] == null)
                    continue;

                if (SubCameraList[i] == info)
                {
                    CurrentSubCamera = info;
                    MainRtReleaseTime = -1;
                }
            }

        }

        private void UpdateMainCamera()
        {
            MainCamera.transform.position = CurrentSubCamera.transform.position;
            MainCamera.transform.eulerAngles = CurrentSubCamera.transform.eulerAngles;
            //   MainCamera.transform.localScale = CurrentSubCamera.transform.localScale;
            MainCamera.farClipPlane = CurrentSubCamera.camera.farClipPlane;
            MainCamera.nearClipPlane = CurrentSubCamera.camera.nearClipPlane;
            MainCamera.backgroundColor = CurrentSubCamera.camera.backgroundColor;
            MainCamera.clearFlags = CurrentSubCamera.camera.clearFlags;
            MainCamera.cullingMask = 1 << LayerMask.NameToLayer("Default");
            MainCamera.fieldOfView = CurrentSubCamera.camera.fieldOfView;
            MainCamera.orthographic = CurrentSubCamera.camera.orthographic;
            MainCamera.depth = CurrentSubCamera.camera.depth;

            _mainCameraAnimation.AnimUpdate();

            CurrentSubCamera.camera.enabled = false;
        }

        public float EffectLum = 1;
        private float _applyEffectLum = 0;

        void Update()
        {

            // 设置公共shader属性 _CircleTime 用于支持所有Shader动画做Time参数
            Shader.SetGlobalFloat("_CircleTime", Time.time % 10 * 0.1f);

//            UpdateRoleLight();
            if (EffectLum != _applyEffectLum)
            {
                Shader.SetGlobalFloat("_EffectScale", EffectLum);
                _applyEffectLum = EffectLum;
            }

        }


        // Update is called once per frame
        void LateUpdate()
        {
            darkOcclusionUpdate();

            if (CurrentSubCamera == null || !CurrentSubCamera.gameObject.activeInHierarchy)
            {
                CurrentSubCamera = null;
                UpdateCurrentSubCamera();

            }

            if (CurrentSubCamera == null)
            {
                if (MainCamera != null)
                {
                    MainCamera.backgroundColor = Color.black;
                    MainCamera.clearFlags = CameraClearFlags.SolidColor;
                    MainCamera.cullingMask = 0;
                }

                return;
            }
            else
            {

                UpdateMainCamera();
            }

            //  FindLowerCorners();
            //延迟释放
            if (MainRtReleaseTime > 0)
            {
                MainRtReleaseTime -= Time.deltaTime;

                if (MainRtReleaseTime <= 0)
                {
                    DestoryMainRt();
                }

            }
            else
            {
                if (CurrentSubCamera == null && MainRt != null)
                {
                    MainRtReleaseTime = 3;

                }

            }


            UpdateSkyCamera();
            UpdateCharacterCamera();
            UpdateEffectCamera(PreEffectCamera);
            UpdateEffectCamera(EffectCamera);
            UpdateEffectCamera(PostEffectCamera);


        }


        /// <summary>
        /// 抖动当前摄像机
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="power">力度</param>
        public void CameraShake(float time, float power, int vibrato = 100)
        {
            if (_CurrentSubCamera != null)
                _CurrentSubCamera.transform.DOShakePosition(time, new Vector3(power, power, 0), vibrato).SetEase(Ease.Linear);
        }



        /// <summary>
        /// 当前摄像机渐出(变暗)
        /// </summary>
        public void FadeOutCurrentCamera(float time = 0.5f)
        {
            if (_mainCameraAnimation != null)
                _mainCameraAnimation.SmoothFadeOut(time);
        }

        /// <summary>
        /// 当前摄像机渐出(变亮)
        /// </summary>
        public void FadeOutCurrentCameraLight(float time = 0.5f)
        {
            if (_mainCameraAnimation != null)
                _mainCameraAnimation.SmoothFadeOutLight(time);
        }

        /// <summary>
        /// 当前摄像机渐出(颜色)
        /// </summary>
        public void FadeOutCurrentCamera(Color fadeOutColor, float time = 0.5f)
        {
            if (_mainCameraAnimation != null)
                _mainCameraAnimation.SmoothFadeOut(fadeOutColor, time);
        }

        /// <summary>
        /// 当前摄像机渐入
        /// </summary>
        public void FadeInCurrentCamera()
        {
            if (_mainCameraAnimation != null)
                _mainCameraAnimation.SmoothFadeIn();
        }
    }

}


