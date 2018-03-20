using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using YoukiaCore;
//using YoukiaUnity.App;
using YoukiaUnity.CinemaSystem;
using YoukiaUnity.Graphics.FastShadowProjector;
using YoukiaUnity.Misc;

//using YoukiaUnity.Resource;

namespace YoukiaUnity.Graphics
{

    /// <summary>
    /// 图形管理器，提供图形性能选项
    /// </summary>
    public class GraphicsManager : SingletonManager<GraphicsManager>
    {

        /*
         
            CreateEffectCamera(ref PreEffectCamera, "preEffect");
            CreateEffectCamera(ref EffectCamera, "effect");
            CreateEffectCamera(ref PostEffectCamera, "postEffect");

         */

        public const string EffectLayerDefine = "effect";
        public const string PreEffectLayerDefine = "preEffect";
        public const string PostEffectLayerDefine = "postEffect";

        /// <summary>
        /// 便捷访问
        /// </summary>
        /// <param name="notbeNULL">Ture:会保证返回一个GraphicsManager,False:在GraphicsManager没有被创建之前调用(或已删除)会返回NULL</param>
        /// <returns></returns>
        public static GraphicsManager GetInstance(bool notbeNULL = false)
        {
            return SingletonManager<GraphicsManager>.GetInstance(notbeNULL);
        }

        public enum eCameraOpenState
        {
            None,
            KeepNormal,
            KeepBlack,
            KeepWhite,
            AutoFadeIn,
        }

        public enum eCameraType
        {
            Default,
            Character,
            Sky,
            PreEffect,
            Effect,
            PostEffect,
            FinalCamera,
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


        /// <summary>
        /// 开启后期特效
        /// </summary>
        public bool EnablePostEffect
        {
            get { return _EnablePostEffect; }
        }

        public bool _EnablePostEffect = false;

        /// <summary>
        /// 开启辉光
        /// </summary>
        public bool EnableBloom
        {
            get { return _EnableBloom; }
        }

        public bool _EnableBloom = false;


        public IGraphicDraw DrawCard;

        private AtmosphereManager AtmosphereMgr;
        internal GlobalProjectorManager ShadowMgr;

        internal bool StopRender
        {

            get { return _StopRender; }
            set
            {
                _StopRender = value;



            }
        }
        private bool _StopRender = false;
        private FPS Fps;

        private Camera UIcamera;

        private Color cameraBackColor;

        private CameraParma TempCameraParma;
        private CameraParma ActiveCameraParma;
        private CameraParma ConfigCameraParma;

        /*
                    internal Camera PreEffectCamera;
                    internal Camera EffectCamera;
                    internal Camera PostEffectCamera; 
        */

        //安装渲染效果卡
        public CallBack SetupDrawCrad;


        //抗锯齿 1 2 4 8
        public int AntiAliasing = 1;

        public string FPS
        {
            get { return Fps.currFPS.ToString(); }
        }

        public Material RenderMaterial
        {
            get { return DrawCard.GetMaterial(); }
        }

        public GlobalProjectorManager.ShadowResolutions _ShadowResolution = GlobalProjectorManager.ShadowResolutions.Low_256;

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
                if (ShadowMgr != null)
                {
                    _Shadow = value;
                    ShadowMgr.GlobalShadowType = value;
                }
            }
        }

        /// <summary>
        /// 是否渲染影子
        /// </summary>
        public void SetShadowsOn(bool isRender)
        {
            ShadowMgr.ShadowsOn = isRender;
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
        /// 图形设置改变
        /// </summary>
        /// <param name="eneryModel"></param>
        public void OnDisplaySettingChange(DisplayConfig config)
        {
            loadConfig(config);
            DrawCard.OnSettingUpdate();
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
                    if (lum > 0.95f && lum < 1.05f)
                    {
                        _darkMaterial.SetFloat("_Lum", 1);
                        _darkOcclusion.layer = LayerMask.NameToLayer("Default");
                        _darkOcclusion.SetActive(false);
                    }
                    else
                    {
                        _darkMaterial.SetFloat("_Lum", lum);
                    }
                }
            }
        }



        public void DarkOcclusion(float lum, float time, float alpha = 1, string layerName = "effect", float fade = 0.2f)
        {

            if (Main.inst == null) return;

            if (_darkOcclusion == null)
            {
                _darkOcclusion = GameObject.CreatePrimitive(PrimitiveType.Quad);
                _darkOcclusion.transform.SetParent(MainCamera.transform, false);
                _darkOcclusion.transform.localPosition = new Vector3(0, 0, 20);
                //                _darkMaterial = new Material(YKApplication.Instance.ConstShader["Custom/Other/DarkOcclusion"]);
                _darkMaterial = new Material(Shader.Find("Custom/Other/DarkOcclusion"));
                _darkMaterial.SetFloat("_Lum", 1);
                _darkMaterial.renderQueue = 3000;

                //                Utils.LoadTexture("Common/bgMask", (srcPath, path, texute) =>
                //                {
                //                    _darkMaterial.SetTexture("_MainTex", texute);
                //                });
                //                UnityEngine.Object tex = Resources.Load("Effect/test/blood003");
                _darkOcclusion.GetComponent<MeshRenderer>().material = _darkMaterial;
            }

            _darkMaterial.SetFloat("_Alpha", alpha);
            _darkMaterial.SetFloat("_Lum", 1);
            _darkLum = lum;
            _darkDuration = Math.Max(time, _darkDuration);
            _darkOcclusion.layer = LayerMask.NameToLayer(layerName);
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
        /// debug模式
        /// </summary>
        public bool DebugMode = true;


        List<SubCameraInfo> SubCameraList = new List<SubCameraInfo>();

        /// <summary>
        /// 当前摄像机
        /// </summary>
        private SubCameraInfo _CurrentSubCamera;


        public Camera CharacterCamera;


        private static bool _isInited = false;

        internal MainDraw FinalDraw;
        internal Camera FinalCamera;

        public Camera GetFinalCamera
        {
            get { return FinalCamera; }
        }

        public GraphicsManager.RtSize PostEffectRenderSize;
        //5.0已经不需要转换
        static float getHorizontalAngle(Camera camera)
        {
            float vFOVrad = camera.fieldOfView * Mathf.Deg2Rad;
            float cameraHeightAt1 = Mathf.Tan(vFOVrad * 0.5f);
            float hFOVrad = Mathf.Atan(cameraHeightAt1 * camera.aspect) * 2;
            return hFOVrad * Mathf.Rad2Deg;
        }

        public void CameraSetPerspective(Camera cam)
        {
            cameraSetPerspective(cam);
        }

        public Matrix4x4 ProjMatAdapt;

        void cameraSetPerspective(Camera cam)
        {
            //aspect: width/height
            //get hfov
            float hfov = cam.fieldOfView;
            float near = cam.nearClipPlane;
            float far = cam.farClipPlane;

            float halfHeight = Mathf.Tan(hfov * Mathf.PI / 180.0f * 0.5f) * cam.nearClipPlane;

            float fixAsp = 1280f / 720;

            float halfWidth = halfHeight * fixAsp;

            float left = -halfWidth;
            float right = halfWidth;
            float bottom = -halfHeight;
            float top = halfHeight;

            top *= fixAsp * Screen.height / Screen.width;
            bottom *= fixAsp * Screen.height / Screen.width;


            if (cam.orthographic)
            {
                float dx = 1.0f / (right - left);
                float dy = 1.0f / (top - bottom);
                float dz = 1.0f / (near - far);

                ProjMatAdapt = new Matrix4x4();
                ProjMatAdapt.m00 = 2 * dx;
                ProjMatAdapt.m11 = 2 * dy;
                ProjMatAdapt.m22 = 2 * dz;
                ProjMatAdapt.m03 = -dx * (left + right);
                ProjMatAdapt.m13 = -dy * (bottom + top);
                ProjMatAdapt.m23 = dz * (near + far);
                ProjMatAdapt.m33 = 1.0f;
            }
            else
            {
                float dx = 1.0f / (right - left);
                float dy = 1.0f / (top - bottom);
                float dz = 1.0f / (near - far);

                ProjMatAdapt.m00 = 2 * near * dx;
                ProjMatAdapt.m11 = 2 * near * dy;
                ProjMatAdapt.m02 = dx * (right + left);
                ProjMatAdapt.m12 = dy * (bottom + top);
                ProjMatAdapt.m22 = dz * (far + near);
                ProjMatAdapt.m32 = -1;
                ProjMatAdapt.m23 = 2 * dz * near * far;
            }

            cam.projectionMatrix = ProjMatAdapt;
        }

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



                if (_CurrentSubCamera.EnableSceneMask)
                    DarkOcclusion(1, 0.01f, 1);
                else
                    DarkOcclusion(1, 0.01f, -1);


                //  Log("CurrentSubCamera is:" + value.gameObject.name);
                //*** 镜头转换导致镜头State混乱，禁用自动转换State
                //                _mainCameraAnimation.OnSwitch(_CurrentSubCamera.OpenState);
                ModifyCameraClip();
                UpdateMainCamera();

            }
            get { return _CurrentSubCamera; }
        }

        private GameObject _roleLight;
        private GameObject _roleLightLookAt;

        private void CreateCharacterCamera()
        {
            GameObject tmp = new GameObject("CharacterCamera");
            CharacterCamera = tmp.AddComponent<Camera>();
            tmp.AddComponent<CameraController>();
            //            CharacterCamera.transform.parent = transform;
            CharacterCamera.transform.SetParent(MainCamera.transform, false);
            CharacterCamera.transform.localPosition = Vector3.zero;
            CharacterCamera.transform.localEulerAngles = Vector3.zero;
            //  CharacterCamera.fieldOfView = _CurrentSubCamera.camera.fieldOfView;
            CharacterCamera.depth = MainCamera.depth + 5;
            //  CharacterCamera.orthographic = _CurrentSubCamera.camera.orthographic;
            // CharacterCamera.orthographicSize = _CurrentSubCamera.camera.orthographicSize;
            CharacterCamera.cullingMask = 1 << LayerMask.NameToLayer("character");
            CharacterCamera.clearFlags = CameraClearFlags.Nothing;
            CharacterCamera.enabled = true;
            CharacterCamera.useOcclusionCulling = false;
            CharacterCamera.hdr = true;

            //characterLight

            _roleLight = new GameObject("Light_Role");

            _roleLight.transform.SetParent(transform, false);
            _roleLightLookAt = CharacterCamera.gameObject;


        }



        public Camera PreEffectCamera;
        public Camera EffectCamera;
        public Camera PostEffectCamera;

        public Camera GetPostEffectCamera
        {
            get { return PostEffectCamera; }
        }

        public Camera GetEffectCamera
        {
            get { return EffectCamera; }
        }

        private float CullingSize = 24; //剔除范围

        private void CreateEffectCamera(ref Camera effectCamera, float depth, string layer, eCameraType camType)
        {
            GameObject tmp = new GameObject(layer + "Camera");
            effectCamera = tmp.AddComponent<Camera>();
            //            effectCamera.transform.parent = transform;
            effectCamera.transform.SetParent(MainCamera.transform, false);
            effectCamera.transform.localPosition = Vector3.zero;
            effectCamera.transform.eulerAngles = Vector3.zero;
            effectCamera.clearFlags = CameraClearFlags.Nothing;
            tmp.AddComponent<CameraController>();
            // effectCamera.fieldOfView = _CurrentSubCamera.camera.fieldOfView;
            effectCamera.depth = depth;
            //            effectCamera.orthographic = _CurrentSubCamera.camera.orthographic;
            //            effectCamera.orthographicSize = _CurrentSubCamera.camera.orthographicSize;
            effectCamera.cullingMask = 1 << LayerMask.NameToLayer(layer);
            effectCamera.enabled = true;
            effectCamera.useOcclusionCulling = false;
            effectCamera.hdr = true;

            PostEffectDraw c = tmp.AddComponent<PostEffectDraw>();
            c.Mgr = this;
            c.CameraType = camType;
            c.enabled = false;
        }

        private void UpdateCameraPos(Camera camera)
        {

            if (camera == null)
                return;

            if (CurrentSubCamera)
            {
                camera.transform.position = CurrentSubCamera.PosInterpolation.Equals(0)
                                    ? _CurrentSubCamera.transform.position
                                    : camera.transform.position +
                                      (_CurrentSubCamera.transform.position - camera.transform.position) *
                                      CurrentSubCamera.PosInterpolation;
                camera.transform.eulerAngles = _CurrentSubCamera.transform.eulerAngles;
            } 
        }

        private void UpdateCamera(Camera camera)
        {
            if (camera == null)
                return;

            camera.enabled = (!_StopRender & CurrentSubCamera != null);

            if (CurrentSubCamera)
            {
                camera.fieldOfView = CurrentSubCamera.camera.fieldOfView;
                camera.orthographic = _CurrentSubCamera.camera.orthographic;
                camera.orthographicSize = _CurrentSubCamera.camera.orthographicSize;
                camera.farClipPlane = CurrentSubCamera.camera.farClipPlane;
                camera.nearClipPlane = CurrentSubCamera.camera.nearClipPlane;
            }

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
                Update();
            }
        }

        private void loadConfig(DisplayConfig config)
        {
            if (config != null)
            {
                GraphicsQuality = config.GraphicQuality;
                ShadowType = config.ShadowType;
                ShadowResolution = config.ShadowResolutions;
                PostEffectRenderSize = config.PostEffectRenderSize;
                _EnablePostEffect = config.EnablePostEffect;
                _EnableBloom = config.EnableBloom;
            }
            else
            {
                _EnableBloom = true;
                _EnablePostEffect = true;
                GraphicsQuality = 3;
                ShadowType = GlobalProjectorManager.ShadowType.ModelProjection;
                ShadowResolution = GlobalProjectorManager.ShadowResolutions.Normal_512;
                PostEffectRenderSize = RtSize.Full;
            }

        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="onFinish">成功后的回调</param>
        public void Init(DisplayConfig config, CallBack onFinish)
        {

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
            obj.AddComponent<CameraController>();
            MainCamera.depth = 0;

            MainCamera.cullingMask = 1 << LayerMask.NameToLayer("Default");
            MainCamera.enabled = true;
            MainCamera.useOcclusionCulling = false;
            MainCamera.hdr = true;

            //FinalCamera结构生成
            obj = new GameObject("FinalCamera");
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            FinalCamera = obj.AddComponent<Camera>();
            FinalCamera.clearFlags = CameraClearFlags.Nothing;
            FinalCamera.backgroundColor = Color.black;
            FinalCamera.depth = 50;
            FinalCamera.cullingMask = 0;
            FinalCamera.useOcclusionCulling = false;
            FinalDraw = obj.AddComponent<MainDraw>();
            Fps = obj.AddComponent<FPS>();

            //drawCard插入
            if (SetupDrawCrad != null)
            {
                SetupDrawCrad();
            }
            loadConfig(config);

            //序列化数据读取
            ConfigCameraParma = new CameraParma();
            FinalDrawRtSize = RtSize.Full;
            AntiAliasing = (QualitySettings.antiAliasing == 0 ? 1 : QualitySettings.antiAliasing);
            ConfigCameraParma.FarClip = 3000;
            ConfigCameraParma.NearClip = 0.1f;

            AtmosphereMgr.Init();
            ActiveCameraParma = ConfigCameraParma;
            UpdateCurrentSubCamera();
            FinalDraw.gameObject.SetActive(true);

            FinalDraw.enabled = true;
            FinalDraw.Init();

            if (CharacterCamera == null)
            {
                CreateCharacterCamera();
                CreateEffectCamera(ref PreEffectCamera, CharacterCamera.depth + 5, PreEffectLayerDefine, eCameraType.PreEffect);
                CreateEffectCamera(ref EffectCamera, CharacterCamera.depth + 10, EffectLayerDefine, eCameraType.Effect);
                CreateEffectCamera(ref PostEffectCamera, CharacterCamera.depth + 15, PostEffectLayerDefine, eCameraType.PostEffect);

            }


            //太阳光生产
            bool ns = false;
            GameObject lgo = new GameObject("SunLight");
            lgo.transform.SetParent(transform, false);
            lgo.transform.localEulerAngles = new Vector3(45, 45, 0);

            Light l = lgo.GetComponent<Light>();
            if (!l) l = lgo.AddComponent<Light>();
            l.type = LightType.Directional;
            l.intensity = 1;

            SunLight = lgo.GetComponent<LightInfo>();
            if (!SunLight) SunLight = lgo.AddComponent<LightInfo>();

            //延迟初始化阴影系统
            ShadowMgr.Initialize();
            ShadowMgr.SetMainCamera(MainCamera);
            _isInited = true;

            //初始化CameraAnimation
            _mainCameraAnimation.Normal();

            if (onFinish != null)
                onFinish();


        }

        public bool isSupportHdr()
        {
            return SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf) || SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBFloat);
        }



        private void DestoryMainRt()
        {
            if (CurrentSubCamera != null)
                CurrentSubCamera.camera.targetTexture = null;

            DestoryOtherCamera();
        }

        private void DestoryOtherCamera()
        {
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

            RenderMaterial.SetColor("_MaskColor", color);

        }

        /// <summary>
        /// 设置屏幕曝光度
        /// </summary>
        /// <param name="exp"></param>
        public void SetExposure(float exp)
        {

            RenderMaterial.SetFloat("_Exposure", exp);

        }

        public void UpdateCurrentSubCamera()
        {
            SubCameraInfo select = null;
            for (int i = 0; i < SubCameraList.Count; i++)
            {
                if (SubCameraList[i] == null)
                    continue;

                if (SubCameraList[i].gameObject.activeInHierarchy && (select == null || SubCameraList[i].Level >= select.Level))
                {
                    select = SubCameraList[i];
                }
            }

            if (select != null)
            {
                CurrentSubCamera = select;

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

                }
            }
        }


        public void UpdateMainCamera()
        {
            UpdateCamera(MainCamera);
            _mainCameraAnimation.AnimUpdate();
        }

        Vector4 rot;

        void UpdateRoleLight()
        {
            if (_roleLight != null)
            {
                Quaternion q = _roleLight.transform.rotation;
                _roleLight.transform.eulerAngles = _roleLightLookAt.transform.eulerAngles;
                _roleLight.transform.Rotate(new Vector3(5, -14, 0));


                rot.x = -_roleLight.transform.forward.x;
                rot.y = -_roleLight.transform.forward.y;
                rot.z = -_roleLight.transform.forward.z;
                rot = rot.normalized;
                rot.w = 1;

                Shader.SetGlobalColor("_RoleDirectionalLightDir", rot);
            }


        }

        public float EffectLum = 1;
        private float _applyEffectLum = 0;

        // Update is called once per frame
        void Update()
        {

            // 设置公共shader属性 _CircleTime 用于支持所有Shader动画做Time参数
            Shader.SetGlobalFloat("_CircleTime", Time.time % 10 * 0.1f);

            darkOcclusionUpdate();

            bool updatePosRequired = false;

            if (!CurrentSubCamera || !CurrentSubCamera.gameObject.activeInHierarchy)
            {
                CurrentSubCamera = null;
                UpdateCurrentSubCamera();

                updatePosRequired = true;
            }
            else if (CurrentSubCamera)
            {
                CurrentSubCamera.enabled = true;
            }

            UpdateMainCamera();
            UpdateCamera(CharacterCamera);
            UpdateCamera(PreEffectCamera);
            UpdateCamera(EffectCamera);
            UpdateCamera(PostEffectCamera);

            if (SunLight != null && ShadowMgr.GlobalProjectionDir != SunLight.transform.eulerAngles)
            {
                ShadowMgr.GlobalProjectionDir = SunLight.transform.eulerAngles;
            }

            UpdateRoleLight();

            if (EffectLum != _applyEffectLum)
            {
                Shader.SetGlobalFloat("_EffectScale", EffectLum);
                _applyEffectLum = EffectLum;
            }

            if (updatePosRequired)
            {
                UpdateCameraPos(MainCamera);
            }
        }

        
        void LateUpdate()
        {
            UpdateCameraPos(MainCamera);
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

        // <summary>
        // 当前摄像机渐出
        // (废弃) 现在调用UI层接口实现
        // </summary>
        //        public void FadeOutCurrentCamera(CameraAnimation.FadeColorType colorType, float time = 0.5f, TweenCallback callback = null)
        //        {
        //            if (_mainCameraAnimation != null)
        //                _mainCameraAnimation.SmoothFadeOut(colorType, time, callback);
        //        }

        // <summary>
        // 当前摄像机渐入
        // (废弃) 现在调用UI层接口实现
        // </summary>
        //        public void FadeInCurrentCamera(CameraAnimation.FadeColorType colorType, float time = 0.5f, TweenCallback callback = null)
        //        {
        //            if (_mainCameraAnimation != null)
        //                _mainCameraAnimation.SmoothFadeIn(colorType, time, callback);
        //        }



    }
}