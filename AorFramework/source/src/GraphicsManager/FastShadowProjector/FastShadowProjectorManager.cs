﻿using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Framework.Graphic.FastShadowProjector
{

    public class FastShadowProjectorManager : MonoBehaviour
    {

        private static bool _dontDestroyOnLoad = true;
        private static Transform _parentTransform = null;

        private static FastShadowProjectorManager _instance;
        public static FastShadowProjectorManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private static FastShadowProjectorManager _findOrCreateGlobalProjectorManager()
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

                if (_parentTransform) go.transform.SetParent(_parentTransform, false);
                if (Application.isPlaying && _dontDestroyOnLoad && !_parentTransform) GameObject.DontDestroyOnLoad(go);

                FastShadowProjectorManager gm = go.GetComponent<FastShadowProjectorManager>();
                if (gm)
                {
                    return gm;
                }
                else
                {
                    return go.AddComponent<FastShadowProjectorManager>();
                }
            }
            else
            {
                go = new GameObject(_NameDefine);
                if (_parentTransform) go.transform.SetParent(_parentTransform, false);
                if (Application.isPlaying && _dontDestroyOnLoad && !_parentTransform) GameObject.DontDestroyOnLoad(go);
                return go.AddComponent<FastShadowProjectorManager>();
            }
        }

        public static FastShadowProjectorManager CreateInstance(Transform parenTransform = null, bool dontDestroyOnLoad = true)
        {

            _parentTransform = parenTransform;
            _dontDestroyOnLoad = dontDestroyOnLoad;

            if (_instance == null)
            {
                _instance = _findOrCreateGlobalProjectorManager();
            }

            return _instance;
        }

        public static void Request(Action GraphicsManagerIniteDoSh)
        {
            CreateInstance().AddGlobalProjectorManagerInited(GraphicsManagerIniteDoSh);
        }

        public static bool IsInit()
        {
            return _instance && _instance._isInit;
        }

        //============================================

        public enum ProjectionCulling
        {
            None,
            ProjectorBounds,
            ProjectionVolumeBounds
        }

        public enum ShadowType
        {
            None,
            FuzzyProjection,
            ModelProjection,
            ShadowMap
        }

        public enum ShadowResolutions
        {
            VeryLow_128 = 0,
            Low_256,
            Normal_512,
            High_1024,
            VeryHigh_2048,
        }

        private bool _AlphaMode;
        public bool AlphaMode
        {

            set
            {
                _AlphaMode = value;

                if (_ProjectorCamera == null)
                    return;

                if (_AlphaMode)
                {

                    _ProjectorCamera.backgroundColor = new Color(0, 0, 0, 0);
                }

                else
                {
                    _ProjectorCamera.backgroundColor = new Color(1, 1, 1, 1);
                }
            }

            get { return _AlphaMode; }

        }

        /// <summary>
        /// 阴影单独渲染
        /// </summary>
        public Camera RenderTextureCamera;

        ProjectorEyeTexture _Tex;
        ProjectorEyeTexture _TexLight;
        public Camera viewCamera;
        [HideInInspector]
        public Material _ProjectorMaterialShadow;
        [HideInInspector]
        public Material _ProjectorMaterialShadowAlpha;
        [HideInInspector]
        public Material _ProjectorMaterialLight;

        private Material _PreShadowMaterialAlpha;

        private Material _PreShadowMaterial;


        public Material PreShadowMaterial
        {
            get
            {
                if (AlphaMode)
                    return _PreShadowMaterialAlpha;
                else

                    return _PreShadowMaterial;

            }

        }

        Matrix4x4 _ProjectorMatrix;
        Matrix4x4 _BiasMatrix;
        Matrix4x4 _ViewMatrix;
        Matrix4x4 _BPV;
        Matrix4x4 _ModelMatrix;
        Matrix4x4 _FinalMatrix;
        //Matrix4x4 _FinalClipMatrix;

        Vector3[] points = new Vector3[8];

        MaterialPropertyBlock _MBP;

        int[] _ShadowResolutions = new int[] { 128, 256, 512, 1024, 2048 };

        public const string _NameDefine = "_FSPGlobalProjectorManager";
        public static readonly string GlobalProjectorLayer = "GlobalProjectorLayer";

        public Vector3 GlobalProjectionDir
        {
            set
            {
                if (_GlobalProjectionDir != value)
                {
                    _GlobalProjectionDir = value;
                    OnProjectionDirChange();
                }
            }

            get
            {
                return _GlobalProjectionDir;
            }
        }
        Vector3 _GlobalProjectionDir = new Vector3(90, 0, 0);

        [HideInInspector]
        public float ZClipFar = 30;

        [HideInInspector]
        public bool FixedProjection = false;            //关闭此项可以提高ShadowMap的分辨率利用率,但是在需要精细阴影的时候会因为场景中物体的运动导致阴影抖动

        public int GlobalShadowResolution
        {
            set
            {
                if (_GlobalShadowResolution != value)
                {
                    _GlobalShadowResolution = value;
                    OnShadowResolutionChange();
                }
            }

            get
            {
                return _GlobalShadowResolution;
            }
        }
        int _GlobalShadowResolution = 1;

        public ProjectionCulling GlobalShadowCullingMode
        {
            set
            {
                _GlobalShadowCullingMode = value;
            }

            get
            {
                return _GlobalShadowCullingMode;
            }
        }

        ProjectionCulling _GlobalShadowCullingMode = ProjectionCulling.None;

        public ShadowType GlobalShadowType
        {
            set
            {
                _GlobalShadowType = value;
            }

            get
            {
                return _GlobalShadowType;
            }
        }

        [SerializeField]
        ShadowType _GlobalShadowType = ShadowType.ModelProjection;

        public float GlobalCutOffDistance
        {
            set
            {
                _GlobalCutOffDistance = value;
            }

            get
            {
                return _GlobalCutOffDistance;
            }
        }

        float _GlobalCutOffDistance = 1000.0f;

        bool _renderShadows = true;

        public bool ShadowsOn
        {
            set
            {
                _renderShadows = value;
            }
            get
            {
                return _renderShadows;
            }
        }

        Camera _ProjectorCamera;
        Camera _ProjectorCameraLight;

        List<IProjector> _ShadowProjectors;
        List<IProjector> _LightProjectors;
        List<ShadowReceiver> _ShadowReceivers;
        List<ShadowTrigger> _ShadowTriggers;

        Texture2D _textureRead;

        bool _shouldCheckTriggers = false;

        //Plane[] _mainCameraPlains;
        //bool _cameraPlainsCalculated;
        //Bounds _projectorBounds;

        protected int projectorCount = 0;

        protected Action _AfterInitDo;
        public void AddGlobalProjectorManagerInited(Action doSh)
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

        protected bool _isInit = false;


        //初始化逻辑数据
        public void Setup()
        {

            if (_isInit) return;

            gameObject.layer = LayerMask.NameToLayer(GlobalProjectorLayer);

            _ProjectorMaterialShadow = new Material(ShaderBridge.Find("Fast Shadow Projector/Multiply"));
            _ProjectorMaterialShadowAlpha = new Material(ShaderBridge.Find("Fast Shadow Projector/MultiplyAlpha"));
            _ProjectorMaterialLight = new Material(ShaderBridge.Find("Fast Shadow Projector/Add"));

            _PreShadowMaterialAlpha = new Material(ShaderBridge.Find("Fast Shadow Projector/PreModelAlpha"));
            _PreShadowMaterial = new Material(ShaderBridge.Find("Fast Shadow Projector/PreModel"));
            _PreShadowMaterial.SetFloat("_Intensity", 0.5f);

            _ProjectorCamera = gameObject.AddComponent<Camera>();
            _ProjectorCamera.clearFlags = CameraClearFlags.SolidColor;
            _ProjectorCamera.backgroundColor = new Color(1, 1, 1, 1);
            _ProjectorCamera.cullingMask = 1 << LayerMask.NameToLayer(FastShadowProjectorManager.GlobalProjectorLayer);
            _ProjectorCamera.orthographic = true;
            _ProjectorCamera.nearClipPlane = -1000;
            _ProjectorCamera.farClipPlane = 1000;
            _ProjectorCamera.orthographicSize = 100;
            _ProjectorCamera.aspect = 1.0f;
            _ProjectorCamera.depth = Single.MinValue;
            _ProjectorCamera.transform.position = new Vector3(0, 10, 0);

            _BiasMatrix = new Matrix4x4();
            _BiasMatrix.SetRow(0, new Vector4(0.5f, 0.0f, 0.0f, 0.5f));
            _BiasMatrix.SetRow(1, new Vector4(0.0f, 0.5f, 0.0f, 0.5f));
            _BiasMatrix.SetRow(2, new Vector4(0.0f, 0.0f, 0.5f, 0.5f));
            _BiasMatrix.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

            _ProjectorMatrix = new Matrix4x4();

            _MBP = new MaterialPropertyBlock();

            _ShadowProjectors = new List<IProjector>();
            _LightProjectors = new List<IProjector>();
            _ShadowReceivers = new List<ShadowReceiver>();
            _ShadowTriggers = new List<ShadowTrigger>();

            CreateLightCamera();

            _ProjectorCamera.enabled = false;
            _ProjectorCameraLight.enabled = false;

            if (_LightProjectors.Count > 0)
            {
                CreateProjectorEyeTexture();
            }
            else
            {
                CreateProjectorEyeTexture(true, false);
            }

            _textureRead = new Texture2D(256, 1, TextureFormat.ARGB32, false);

            //_projectorBounds = new Bounds();
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

        void CreateLightCamera()
        {
            GameObject cameraGameObject = new GameObject("_lightCamera");
            FastShadowProjectorLightCamera lightCamera;

            cameraGameObject.transform.parent = transform;
            lightCamera = cameraGameObject.AddComponent<FastShadowProjectorLightCamera>();
            lightCamera.PreCullCallback = OnLightPreCull;
            lightCamera.PostRenderCallback = OnLightPostRender;

            _ProjectorCameraLight = cameraGameObject.AddComponent<Camera>();
            _ProjectorCameraLight.CopyFrom(_ProjectorCamera);
            _ProjectorCameraLight.backgroundColor = new Color32(0, 0, 0, 0);
            _ProjectorCameraLight.depth = -2;

            _ProjectorCameraLight.enabled = false;
        }

        void Awake()
        {
            //单例限制
            if (_instance != null && _instance != this)
            {
                GameObject.Destroy(this);
            }
            else if (_instance == null)
            {
                _instance = this;
                gameObject.name = _NameDefine;
            }
        }

        protected void OnEnable()
        {
            OnProjectionDirChange();
        }

        public Texture GetShadowTexture()
        {
            return _Tex.GetTexture();
        }

        public void AddProjector(IProjector projector)
        {
            bool added = false;

            if (!projector.IsLight)
            {
                if (_TexLight == null)
                {
                    CreateProjectorEyeTexture(false, true);
                }

                if (!_ShadowProjectors.Contains(projector))
                {
                    _ShadowProjectors.Add(projector);
                    added = true;

                    if (_ProjectorCamera.enabled == false)
                    {
                        _ProjectorCamera.enabled = true;
                    }
                }
            }
            else
            {
                if (_Tex == null)
                {
                    CreateProjectorEyeTexture(true, false);
                }
                if (!_LightProjectors.Contains(projector))
                {
                    _LightProjectors.Add(projector);
                    added = true;

                    if (_ProjectorCameraLight.enabled == false)
                    {
                        _ProjectorCameraLight.enabled = true;
                    }
                }
            }

            //if (added && projector.GlobalProjectionDir != _GlobalProjectionDir)
            //{
            //GlobalProjectionDir = projector.GlobalProjectionDir;
            //}

            //if (added && projector.GlobalShadowCullingMode != _GlobalShadowCullingMode)
            //{
            //    _GlobalShadowCullingMode = projector.GlobalShadowCullingMode;
            //}

            if (added && projector.GlobalShadowResolution != _GlobalShadowResolution)
            {
                _GlobalShadowResolution = projector.GlobalShadowResolution;
            }

        }

        public void RemoveProjector(IProjector projector)
        {
            if (!projector.IsLight)
            {
                if (_ShadowProjectors.Contains(projector))
                {
                    _ShadowProjectors.Remove(projector);

                    if (_ShadowProjectors.Count == 0)
                    {
                        _ProjectorCamera.enabled = false;
                    }
                }
            }
            else
            {
                if (_LightProjectors.Contains(projector))
                {
                    _LightProjectors.Remove(projector);

                    if (_LightProjectors.Count == 0)
                    {
                        _ProjectorCameraLight.enabled = false;
                    }
                }
            }

        }

        public void AddReceiver(ShadowReceiver receiver)
        {
            if (!_ShadowReceivers.Contains(receiver) && receiver.enabled)
            {
                CheckForTerrain(receiver);
                _ShadowReceivers.Add(receiver);
            }
        }

        void CheckForTerrain(ShadowReceiver receiver)
        {
#if !UNITY_3_5 && !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0 && !UNITY_3_0_0
            if (receiver.IsTerrain())
            {
                if (receiver.GetTerrain().materialTemplate != null)
                {
                    receiver.GetTerrain().materialTemplate.SetTexture("_ShadowTex", _Tex.GetTexture());
                }
            }
#endif
        }

        public void AddShadowTrigger(ShadowTrigger trigger)
        {
            if (!_ShadowTriggers.Contains(trigger))
            {
                _ShadowTriggers.Add(trigger);
            }
        }

        public void RemoveShadowTrigger(ShadowTrigger trigger)
        {
            if (_ShadowTriggers.Contains(trigger))
            {
                _ShadowTriggers.Remove(trigger);
            }
        }

        public void RemoveReceiver(ShadowReceiver receiver)
        {
            if (_ShadowReceivers.Contains(receiver))
            {
                _ShadowReceivers.Remove(receiver);
            }
        }

        void OnProjectionDirChange()
        {
            if (_ProjectorCamera != null)
            {
                _ProjectorCamera.transform.rotation = Quaternion.Euler(_GlobalProjectionDir);
            }
        }

        void OnShadowResolutionChange()
        {
            if (_LightProjectors.Count > 0)
            {
                CreateProjectorEyeTexture();
            }
            else
            {
                CreateProjectorEyeTexture(true, false);
            }
        }

        void CreateProjectorEyeTexture()
        {
            CreateProjectorEyeTexture(true, true);
        }

        void CreateProjectorEyeTexture(bool shadow, bool light)
        {
            if (shadow)
            {
                if (_Tex != null)
                {
                    _Tex.CleanUp();
                }

                _Tex = new ProjectorEyeTexture(_ProjectorCamera, _ShadowResolutions[_GlobalShadowResolution]);
                _ProjectorMaterialShadow.SetTexture("_ShadowTex", _Tex.GetTexture());
                _ProjectorMaterialShadowAlpha.SetTexture("_ShadowTex", _Tex.GetTexture());
            }

            if (light)
            {
                if (_TexLight != null)
                {
                    _TexLight.CleanUp();
                }

                _TexLight = new ProjectorEyeTexture(_ProjectorCameraLight, _ShadowResolutions[_GlobalShadowResolution]);
                _ProjectorMaterialLight.SetTexture("_ShadowTex", _TexLight.GetTexture());
            }
        }

        public void SetMainCamera(Camera view)
        {
            viewCamera = view;
        }

        public Camera getMainCamera()
        {
            return viewCamera;
        }
        void CalculateShadowBounds(Camera targetCamera, List<IProjector> projectors)
        {
            updateProjectionCamera(viewCamera ?? Camera.main, targetCamera, projectors, _ShadowResolutions[_GlobalShadowResolution], _ShadowResolutions[_GlobalShadowResolution], ZClipFar == 0 ? targetCamera.farClipPlane : ZClipFar);
        }

        protected void getFrustumPoints(Camera cam, float zFar, Vector3[] result)
        {
            if (cam == null)
                return;
            uint i;
            Matrix4x4 viewProjMatrixInverse;
            float a, b, projZFar;
            viewProjMatrixInverse = cam.projectionMatrix * cam.worldToCameraMatrix;
            viewProjMatrixInverse = viewProjMatrixInverse.inverse;
            a = -(cam.farClipPlane + cam.nearClipPlane) / (cam.farClipPlane - cam.nearClipPlane);
            b = -(2 * cam.farClipPlane * cam.nearClipPlane) / (cam.farClipPlane - cam.nearClipPlane);
            projZFar = -(a * zFar - b) / zFar;
            result[0].Set(-1, -1, -1);
            result[1].Set(1, -1, -1);
            result[2].Set(1, 1, -1);
            result[3].Set(-1, 1, -1);
            result[4].Set(-1, -1, projZFar);
            result[5].Set(1, -1, projZFar);
            result[6].Set(1, 1, projZFar);
            result[7].Set(-1, 1, projZFar);
            for (i = 0; i < 8; i++)
            {
                result[i] = viewProjMatrixInverse.MultiplyPoint(result[i]);
            }
        }

        protected void fitShadowToPixel(ref Bounds shadowAABB, float smWidth, float smHeight)
        {
            float worldUnitsPrePixelX = (shadowAABB.max.x - shadowAABB.min.x) / smWidth;
            float worldUnitsPrePixelY = (shadowAABB.max.y - shadowAABB.min.y) / smHeight;

            float maxX = 0;
            float minX = 0;
            float maxY = 0;
            float minY = 0;

            if (!worldUnitsPrePixelX.Equals(0))
            {
                maxX = Mathf.Floor(shadowAABB.max.x / worldUnitsPrePixelX) * worldUnitsPrePixelX;
                minX = Mathf.Floor(shadowAABB.min.x / worldUnitsPrePixelX) * worldUnitsPrePixelX;
            }
            if (!worldUnitsPrePixelY.Equals(0))
            {
                maxY = Mathf.Floor(shadowAABB.max.y / worldUnitsPrePixelY) * worldUnitsPrePixelY;
                minY = Mathf.Floor(shadowAABB.min.y / worldUnitsPrePixelY) * worldUnitsPrePixelY;
            }

            shadowAABB.max = new Vector3(maxX, maxY, shadowAABB.max.z);
            shadowAABB.min = new Vector3(minX, minY, shadowAABB.min.z);
        }
        public static void SetFrustum(Camera cam, float left, float right, float bottom, float top, float near, float far, bool isOrtho)
        {
            Matrix4x4 projMat = new Matrix4x4();
            if (isOrtho)
            {
                float dx = 1.0f / (right - left);
                float dy = 1.0f / (top - bottom);
                float dz = 1.0f / (near - far);

                projMat = new Matrix4x4();
                projMat.m00 = 2 * dx;
                projMat.m11 = 2 * dy;
                projMat.m22 = 2 * dz;
                projMat.m03 = -dx * (left + right);
                projMat.m13 = -dy * (bottom + top);
                projMat.m23 = dz * (near + far);
                projMat.m33 = 1.0f;
            }
            else
            {
                float dx = 1.0f / (right - left);
                float dy = 1.0f / (top - bottom);
                float dz = 1.0f / (near - far);

                projMat.m00 = 2 * near * dx;
                projMat.m11 = 2 * near * dy;
                projMat.m02 = dx * (right + left);
                projMat.m12 = dy * (bottom + top);
                projMat.m22 = dz * (far + near);
                projMat.m32 = -1;
                projMat.m23 = 2 * dz * near * far;
            }

            cam.projectionMatrix = projMat;
        }
        public Bounds Transform(Bounds bounds, Matrix4x4 matrix)
        {
            Vector3 p0 = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z);
            Vector3 p1 = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z);
            Vector3 p2 = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z);
            Vector3 p3 = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z);
            Vector3 p4 = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z);
            Vector3 p5 = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
            Vector3 p6 = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z);
            Vector3 p7 = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);

            bounds.center = matrix.MultiplyPoint(p0);
            bounds.size = Vector3.zero;
            bounds.Encapsulate(matrix.MultiplyPoint(p1));
            bounds.Encapsulate(matrix.MultiplyPoint(p2));
            bounds.Encapsulate(matrix.MultiplyPoint(p3));
            bounds.Encapsulate(matrix.MultiplyPoint(p4));
            bounds.Encapsulate(matrix.MultiplyPoint(p5));
            bounds.Encapsulate(matrix.MultiplyPoint(p6));
            bounds.Encapsulate(matrix.MultiplyPoint(p7));

            return bounds;
        }
        protected void updateProjectionCamera(Camera viewCam, Camera shadowCam, List<IProjector> projectors, float smWidth, float smHeight, float zFar)
        {
            Vector3 scale, offset;
            Bounds shadowAABB = new Bounds(), casterAABB = new Bounds(), frustumAABB = new Bounds();
            Matrix4x4 viewMat;
            Matrix4x4 projMat;
            Matrix4x4 cropMat;
            SetFrustum(shadowCam, -1, 1, -1, 1, -1, 1, true);
            getFrustumPoints(viewCam, zFar, points);
            viewMat = shadowCam.worldToCameraMatrix;
            for (int i = 0; i < 8; i++)
            {
                //points[i] = viewMat.MultiplyPoint(points[i]);

                if (i == 0)
                {
                    frustumAABB.center = points[i];
                    frustumAABB.size = Vector3.zero;
                }
                else
                {
                    frustumAABB.Encapsulate(points[i]);
                }
            }
            shadowCam.transform.position = new Vector3(frustumAABB.center.x, shadowCam.transform.position.y, frustumAABB.center.z);
            frustumAABB = Transform(frustumAABB, viewMat);
            frustumAABB.SetMinMax(new Vector3(frustumAABB.min.x, frustumAABB.min.y, frustumAABB.min.z - 40),             //可随根据需要改变
                new Vector3(frustumAABB.max.x, frustumAABB.max.y, frustumAABB.max.z + 200));
            projectorCount = 0;
            for (int i = 0; i < projectors.Count; i++)
            {
                IProjector projector = projectors[i];
                Bounds bounds;
                Bounds tempBounds = projector.GetBounds();
                tempBounds = Transform(tempBounds, viewMat);

                if (projector is ModelShadowProjector && !(projector as ModelShadowProjector).InCamera && !_AlphaMode)
                {
                    projector.SetVisible(false);
                    continue;
                }

                if (_GlobalShadowType == projector.Type && frustumAABB.Intersects(bounds = tempBounds))
                {
                    if (projectorCount == 0)
                    {
                        casterAABB = bounds;
                    }
                    else
                    {
                        casterAABB.Encapsulate(bounds);
                    }
                    projector.SetVisible(true);
                    projectorCount++;
                }
                else
                {
                    projector.SetVisible(false);
                }
            }
            //TODO:这里可以继续裁剪Receiver,并逆向裁剪Caster进一步精确结果,目前因为只有一块地表投影专用Mesh,故无需这样做

            if (projectorCount == 0)
            {
                return;
            }

            if (FixedProjection)
            {
                shadowAABB = frustumAABB;
            }
            else
            {
                shadowAABB.min = Max(casterAABB.min, frustumAABB.min);
                shadowAABB.max = Min(casterAABB.max, frustumAABB.max);
                shadowAABB.SetMinMax(new Vector3(shadowAABB.min.x, shadowAABB.min.y, shadowAABB.min.z - 5),             //可随根据需要改变
                new Vector3(shadowAABB.max.x, shadowAABB.max.y, shadowAABB.max.z + 5));
            }
            fitShadowToPixel(ref shadowAABB, smWidth, smHeight);
            scale.x = 2.0f / (shadowAABB.max.x - shadowAABB.min.x);
            scale.y = 2.0f / (shadowAABB.max.y - shadowAABB.min.y);
            offset.x = -0.5f * (shadowAABB.max.x + shadowAABB.min.x) * scale.x;
            offset.y = -0.5f * (shadowAABB.max.y + shadowAABB.min.y) * scale.y;
            scale.z = 1.0f / (shadowAABB.max.z - shadowAABB.min.z);
            offset.z = shadowAABB.min.z * scale.z;
            cropMat = Matrix4x4.identity;
            cropMat.m00 = scale.x;
            cropMat.m03 = offset.x;
            cropMat.m11 = scale.y;
            cropMat.m13 = offset.y;
            cropMat.m22 = scale.z;
            cropMat.m23 = offset.z;
            projMat = shadowCam.projectionMatrix;
            shadowCam.projectionMatrix = cropMat * projMat;
        }
        public Vector3 Min(Vector3 v0, Vector3 v1)
        {
            return new Vector3
            {
                x = Mathf.Min(v0.x, v1.x),
                y = Mathf.Min(v0.y, v1.y),
                z = Mathf.Min(v0.z, v1.z)
            };
        }
        public Vector3 Max(Vector3 v0, Vector3 v1)
        {
            return new Vector3
            {
                x = Mathf.Max(v0.x, v1.x),
                y = Mathf.Max(v0.y, v1.y),
                z = Mathf.Max(v0.z, v1.z)
            };
        }
        //void CheckMainCameraPlains()
        //{
        //    if (!_cameraPlainsCalculated)
        //    {
        //        _mainCameraPlains = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        //        _cameraPlainsCalculated = true;
        //    }
        //}

        //bool IsProjectionVolumeVisible(Plane[] planes, ShadowProjector projector)
        //{
        //    float boundSize = 1000000.0f;

        //    Vector3 center = projector.GetShadowPos() + GlobalProjectionDir.normalized * (boundSize * 0.5f);
        //    Vector3 size = new Vector3(Mathf.Abs(GlobalProjectionDir.normalized.x), Mathf.Abs(GlobalProjectionDir.normalized.y), Mathf.Abs(GlobalProjectionDir.normalized.z)) * boundSize;
        //    Bounds bounds = new Bounds(center, size);

        //    float shadowSize = projector.GetShadowWorldSize();

        //    bounds.Encapsulate(new Bounds(projector.GetShadowPos(), new Vector3(shadowSize, shadowSize, shadowSize)));

        //    return GeometryUtility.TestPlanesAABB(planes, bounds);
        //}

        public void SetTriggerTexPixel(Vector3 point, bool checkShadow, bool checkLight, int triggerID)
        {
            if (checkShadow)
            {
                SetTriggerTexPixel(_ProjectorCamera, _Tex, point, triggerID);
            }
            else
            {
                if (checkLight)
                {
                    SetTriggerTexPixel(_ProjectorCameraLight, _TexLight, point, triggerID);
                }
            }
        }

        void SetTriggerTexPixel(Camera camera, ProjectorEyeTexture tex, Vector3 point, int triggerID)
        {
            Vector3 screenPoint = camera.WorldToScreenPoint(point);

            Texture texture = (Texture)tex.GetTexture();

            int x = (int)((screenPoint.x / camera.pixelWidth) * texture.width);
            int y = (int)((screenPoint.y / camera.pixelHeight) * texture.height);

            if (x < 0.0f || y < 0.0f || x >= texture.width || y >= texture.height)
            {
                return;
            }

            Color pixelColor;
            if (!tex.RenderTextureSupported())
            {
                pixelColor = ((Texture2D)texture).GetPixel(x, y);
                _textureRead.SetPixel(triggerID, 0, pixelColor);
            }
            else
            {
                RenderTexture.active = tex.GetRenderTexture();
                _textureRead.ReadPixels(new Rect(x, y, 1, 1), triggerID, 0, false);
            }

        }

        void Update()
        {
            ShadowReceiver receiver;

            //            if (_ProjectorMaterialShadow != null && _Tex != null)
            //                _ProjectorMaterialShadow.SetTexture("_ShadowTex", _Tex.GetTexture());

            if (null != _ShadowReceivers)
            {
                for (int i = 0; i < _ShadowReceivers.Count; i++)
                {
                    receiver = _ShadowReceivers[i];
                    if (receiver.IsTerrain())
                    {
                        receiver.GetTerrain().materialTemplate = null;
                    }
                }
            }
        }


        void LateUpdate()
        {
            if (!_renderShadows)
            {
                return;
            }

            _shouldCheckTriggers = !_shouldCheckTriggers;
            if (AlphaMode)
                RenderProjectors(_ProjectorCamera, _ShadowProjectors, _ProjectorMaterialShadowAlpha);
            else
                RenderProjectors(_ProjectorCamera, _ShadowProjectors, _ProjectorMaterialShadow);

            RenderProjectors(_ProjectorCameraLight, _LightProjectors, _ProjectorMaterialLight);
        }

        void RenderProjectors(Camera targetCamera, List<IProjector> projectors, Material material)
        {
            if (!_renderShadows || null == targetCamera)
            {
                return;
            }

            if (projectors.Count > 0 && _ShadowReceivers.Count > 0)
            {
                if (_GlobalShadowType == ShadowType.FuzzyProjection)
                {
                    _ProjectorCamera.transform.rotation = new Quaternion(0.7f, 0, 0, 0.7f);
                    _GlobalProjectionDir = new Vector3(0.0f, -1.0f, 0.0f);
                }

                CalculateShadowBounds(targetCamera, projectors);

                float n = targetCamera.nearClipPlane;
                float f = targetCamera.farClipPlane;
                float r = targetCamera.orthographicSize;
                float t = targetCamera.orthographicSize;

                _ProjectorMatrix.SetRow(0, new Vector4(1 / r, 0.0f, 0.0f, 0));
                _ProjectorMatrix.SetRow(1, new Vector4(0.0f, 1 / t, 0.0f, 0));
                _ProjectorMatrix.SetRow(2, new Vector4(0.0f, 0.0f, -2 / (f - n), 0));
                _ProjectorMatrix.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

                _ProjectorMatrix = targetCamera.projectionMatrix;

                _ViewMatrix = targetCamera.transform.localToWorldMatrix.inverse;

                _BPV = _BiasMatrix * _ProjectorMatrix * _ViewMatrix;

                Render(material);
            }
        }

        void Render(Material material)
        {
            if (!_renderShadows || projectorCount == 0 || _ShadowReceivers == null || _ShadowReceivers.Count == 0)
            {
                return;
            }

            bool useMBP = true; // WP8 doesn't support MBP's correctly - only one receiver will work for now.

#if UNITY_WP8
		useMBP = false;
#endif

            ShadowReceiver receiver;

            for (int i = 0; i < _ShadowReceivers.Count; i++)
            {
                receiver = _ShadowReceivers[i];
                _ModelMatrix = receiver.transform.localToWorldMatrix;
                _FinalMatrix = _BPV * _ModelMatrix;

                if (receiver.IsTerrain())
                {
                    ApplyTerrainTextureMatrix(receiver);
                }
                else
                {

                    if (useMBP)
                    {

                        _MBP.Clear();
                        _MBP.AddMatrix("_GlobalProjector", _FinalMatrix);
                        //_MBP.AddMatrix("_GlobalProjectorClip", _FinalClipMatrix);
                        if (_ShadowReceivers[i].GetMesh() != null)
                        {
                            for (int n = 0; n < _ShadowReceivers[i].GetMesh().subMeshCount; n++)
                            {
                                if (RenderTextureCamera == null)
                                {
                                    UnityEngine.Graphics.DrawMesh(_ShadowReceivers[i].GetMesh(), _ModelMatrix, material,
                                        LayerMask.NameToLayer("Default"), null, n, _MBP);

                                }
                                else
                                {
                                    UnityEngine.Graphics.DrawMesh(_ShadowReceivers[i].GetMesh(), _ModelMatrix, material,
         LayerMask.NameToLayer("shadow"), RenderTextureCamera, n, _MBP);

                                }
                            }
                        }

                    }
                    else
                    {
                        material.SetMatrix("_GlobalProjector", _FinalMatrix);
                        //material.SetMatrix("_GlobalProjector", _FinalClipMatrix);

                        UnityEngine.Graphics.DrawMesh(_ShadowReceivers[i].GetMesh(), _ModelMatrix, material, LayerMask.NameToLayer("Default"));
                    }
                }
            }
        }

        void CheckTriggers(bool shadow)
        {
            if (!_shouldCheckTriggers)
            {
                return;
            }

            RaycastHit raycastHit;

            for (int n = 0; n < _ShadowTriggers.Count; n++)
            {
                if (Physics.Raycast(new Ray(_ShadowTriggers[n].transform.position, _instance.GlobalProjectionDir), out raycastHit))
                {
                    if (_ShadowTriggers[n].DetectShadow && shadow)
                    {
                        SetTriggerTexPixel(_ProjectorCamera, _Tex, raycastHit.point, n);
                    }
                    else if (_ShadowTriggers[n].DetectLight && !shadow)
                    {
                        SetTriggerTexPixel(_ProjectorCameraLight, _TexLight, raycastHit.point, n);
                    }
                }
            }

            _textureRead.Apply();

            Color[] pixels;
            pixels = _textureRead.GetPixels(0, 0, _ShadowTriggers.Count, 1);

            for (int n = 0; n < _ShadowTriggers.Count; n++)
            {

                if (_ShadowTriggers[n].DetectShadow && shadow)
                {
                    _ShadowTriggers[n].OnTriggerCheckDone(pixels[n].a > 0.0f ? true : false);
                }
                else if (_ShadowTriggers[n].DetectLight && !shadow)
                {
                    _ShadowTriggers[n].OnTriggerCheckDone(pixels[n].a > 0.0f ? true : false);
                }

            }
        }

        void ApplyTerrainTextureMatrix(ShadowReceiver receiver)
        {

            if (receiver._terrainMaterial != null)
            {
                receiver._terrainMaterial.SetMatrix("_GlobalProjector", _FinalMatrix);
                //receiver._terrainMaterial.SetMatrix("_GlobalProjectorClip", _FinalClipMatrix);
            }

            receiver.GetTerrain().materialTemplate = receiver._terrainMaterial;

        }

        void OnPreCull()
        {
            for (int i = 0; i < _ShadowProjectors.Count; i++)
            {
                _ShadowProjectors[i].SetVisible(true);
                _ShadowProjectors[i].OnPreRenderShadowProjector(_ProjectorCamera);
            }
        }

        void OnPostRender()
        {
            //_Tex.GrabScreenIfNeeded();
            //CheckTriggers(true);

            for (int i = 0; i < _ShadowProjectors.Count; i++)
            {
                _ShadowProjectors[i].SetVisible(false);
            }
        }

        void OnLightPreCull()
        {
            for (int i = 0; i < _LightProjectors.Count; i++)
            {
                _LightProjectors[i].SetVisible(true);
                _LightProjectors[i].OnPreRenderShadowProjector(_ProjectorCameraLight);
            }
        }

        void OnLightPostRender()
        {
            _TexLight.GrabScreenIfNeeded();
            CheckTriggers(false);

            for (int i = 0; i < _LightProjectors.Count; i++)
            {
                _LightProjectors[i].SetVisible(false);
            }
        }
    }

}

