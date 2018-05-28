using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace Framework.Graphic
{

    //[ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class VisualCamera : MonoBehaviour
    {

        public bool OverrideClearFlags = false;

        public bool OverrideBackground = false;

        public bool OverrideOrthographic = false;

        public bool OverrideFieldOfView = true;

        public bool OverrideNearPlane = true;
        public bool OverrideFarPlane = true;

        public bool OverrideRenderingPath = false;

        public bool OverrideOcclusionCulling = false;
        public bool OverrideAllowHDR = false;
        public bool OverrideAllowMSAA = false;

        public bool IgnoreInterpolationOnce = true;

        [SerializeField]
        [Range(0,1)]
        private float _interpolation = 0;
        /// <summary>
        /// 运行时相机运动插值(设置范围 0-1, 大于1时会产生奇怪的运动, 0为不插值)
        /// </summary>
        public float Interpolation
        {
            get { return _interpolation; }
            set
            {
                float v = Mathf.Max(0, value);
                if (!_interpolation.Equals(v))
                {
                    _interpolation = v;
                }
            }
        }

        private Camera _currentCam;
        public Camera CrrentCamera
        {
            get
            {
                if (!_currentCam) _currentCam = GetComponent<Camera>();
                return _currentCam;
            }
        }

        [SerializeField]
        private float _level;
        public float Level
        {
            get { return _level; }
            set {
                if (!_level.Equals(value))
                {
                    _level = value;
                    if (GraphicsManager.IsInit())
                        GraphicsManager.Instance.RefreshCurrentVisualCamera();
                }   
            }
        }

        [SerializeField]
        private bool _solo;
        public bool Solo
        {
            get { return _solo; }
            set
            {
                if (!_solo.Equals(value))
                {
                    _solo = value;
                    if (GraphicsManager.IsInit())
                        GraphicsManager.Instance.RefreshCurrentVisualCamera();
                }
            }
        }
        
        private VisualCameraExtension _extension;
        public VisualCameraExtension extension 
        {
            get {
                if (!_extension)
                {
                    _extension = GetComponent<VisualCameraExtension>();
//                    if (!_extension) _extension = gameObject.AddComponent<VisualCameraExtension>();
                }
                return _extension;
            }
        }
        
        public void UpdateExtension(float deltaTime)
        {
            if(extension && _extension.enabled) _extension.UpdateExtension(deltaTime);
        }

        protected void Awake()
        {
            if (gameObject.tag == "MainCamera")
            {
                Log.Error("*** VisualCamera不能定义Tag为MainCamera!");
                GameObject.Destroy(this);
            }
        }
        protected void OnEnable()
        {
            if (!CrrentCamera) return;
            GraphicsManager.Request(() =>
            {
                GraphicsManager.Instance.RegisterVisualCamera(this);
                CrrentCamera.enabled = false;
            });
        }

        protected void OnDisable()
        {
            if (!CrrentCamera) return;
            if(GraphicsManager.IsInit())
                GraphicsManager.Instance.UnregisterVisualCamera(this);
        }

        private bool _isDestroyd = false;
        protected void OnDestroy()
        {
            _isDestroyd = true;
        }
    }
}


