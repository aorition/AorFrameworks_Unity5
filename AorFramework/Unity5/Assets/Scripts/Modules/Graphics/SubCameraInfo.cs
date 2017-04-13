using System;
using System.Collections;
using UnityEngine;
using YoukiaUnity.Graphics;

namespace AorFramework.module
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
            base.Launcher();
            _camera = GetComponent<Camera>();
            _camera.enabled = false;

            GraphicsManager.GetInstance().RegisterSubCamera(this);



        }

        void OnDestroy()
        {
            if (GraphicsManager.isInited)
                GraphicsManager.GetInstance().RemoveSubCamera(this);

        }
    }
}


