using System;
using System.Collections;
using UnityEngine;
using YoukiaUnity.Graphics;

namespace YoukiaUnity.CinemaSystem
{
    /// <summary>
    /// 摄像机系统基类
    /// </summary>
    public class SubCameraInfo : GraphicsLauncher
    {

        /// <summary>
        /// 切换到该摄像机时是否由黑到亮过渡
        /// </summary>
        public GraphicsManager.eCameraOpenState OpenState = GraphicsManager.eCameraOpenState.None;

        /// <summary>
        /// 场景遮罩
        /// </summary>
        public bool EnableSceneMask;

        /// <summary>
        /// 运行时相机运动插值(0为不插值)
        /// </summary>
        public float PosInterpolation = 0;

        [SerializeField]
        private int _Level;
        /// <summary>
        /// 优先级,同时存在的摄像机取 高优先级的
        /// </summary>
        public int Level
        {

            get { return _Level; }
            set
            {
                _Level = Mathf.Max(0, value);

                if (GraphicsManager.isInited)
                    GraphicsManager.GetInstance().UpdateCurrentSubCamera();

            }

        }


        /// <summary>
        /// 覆盖图形参数
        /// </summary>
        public bool OverrideGraphicSetting;
        public float OverrideNearClip = -1;
        public float OverrideFarClip = -1;
        public float OverrideFogDestance = -1;
        public float OverrideFogDestiy = -1;
        public float OverrideVolumeFogOffset = -1;
        public float OverrideVolumeFogDestiy = -1;

        private Camera _camera;
        public Camera camera
        {
            get { return _camera; }
        }

        //        public override void OnScriptCoverFinish()
        protected override void Launcher()
        {
            _camera = GetComponent<Camera>();
            _camera.enabled = false;
        }

        void OnEnable()
        {
            if (!_isStarted) return;
            GraphicsManager.GetInstance().RegisterSubCamera(this);
        }

        void OnDisable()
        {
            if (GraphicsManager.isInited)
                GraphicsManager.GetInstance().RemoveSubCamera(this);
        }

        private bool _isStarted = false;

        void Start()
        {

            if (!_camera)
            {
                _camera = GetComponent<Camera>();
                _camera.enabled = false;
            }

            if (GraphicsManager.isInited)
                GraphicsManager.GetInstance().RegisterSubCamera(this);

            _isStarted = true;
        }
    }
}


