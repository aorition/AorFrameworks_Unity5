using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utility
{
    /// <summary>
    /// 当前相机计算得出的Aspect将适配fixAspect的宽度(以此消除在不同屏幕分辨率下该相机的视觉差异)
    /// </summary>
    public class CorrectingAspect : MonoBehaviour
    {

        public float DesignScreenWidth = 1280;
        public float DesignScreenHeight = 720;

        private Matrix4x4 m_projectionMatrix = new Matrix4x4();

        private Camera _camera;
        private void OnEnable()
        {
            _camera = GetComponent<Camera>();
        }

        private void OnDisable()
        {
            if (_camera)
            {
                _camera.ResetProjectionMatrix();
            }
        }

        private void OnPreRender()
        {
            CameraMathUtility.CorrectingAspect(DesignScreenWidth/ DesignScreenHeight,_camera.fieldOfView ,_camera.nearClipPlane, _camera.farClipPlane, _camera.orthographic,ref m_projectionMatrix);
            _camera.projectionMatrix = m_projectionMatrix;
        }
    }
}
