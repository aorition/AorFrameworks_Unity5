using System;
using System.Collections.Generic;
using UnityEngine;

namespace AorFramework.AorTile3D.runtime
{
    public class NavigatorCameraHandler : MonoBehaviour
    {

        private Camera _camera;
        public new Camera camera
        {
            get
            {
                return _camera;
            }
        }

        private AorTile3DScene _scene;

        private void Awake()
        {

            GameObject c = new GameObject("eCamera");
            c.transform.SetParent(transform, false);
            _camera = c.AddComponent<Camera>();
            _camera.transform.localPosition = new Vector3(0, 500, 0);
            _camera.transform.localEulerAngles = new Vector3(90, 0, 0);

            _camera.depth = 51;
            _camera.nearClipPlane = 1;
            _camera.farClipPlane = 1000;

            _camera.orthographic = true;
            _camera.renderingPath = RenderingPath.Forward;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = AorTile3DUtils.GetDefineBGColor();

            _camera.rect = new Rect(0.8f, 0.8f, 0.2f, 0.2f);
        }
        
        /// <summary>
        /// 自动setup
        /// </summary>
        private void Start()
        {
            if(_isInit) return;
            if (AorTile3DManager.Instance != null && AorTile3DManager.Instance.currentScene != null)
            {
                setup(AorTile3DManager.Instance.currentScene);
            }
            else
            {
                _camera.enabled = false;
                AorTile3DManager.ThrowError("AorTile3DManager.Instance is null or AorTile3DManager.Instance.currentScene is null. NavigatorCameraHandler init fail!");
            }
        }

        private bool _isInit = false;
        public void setup(AorTile3DScene secne)
        {
            _scene = secne;
            _isInit = true;
        }

        private void LateUpdate()
        {
            if (!_isInit) return;

            UpdateCSize();
            UpdateLockPos();
        }

        private void UpdateCSize()
        {
            if (!_isInit) return;
            _camera.orthographicSize = _scene.borderHalfSize[1];
        }

        private void UpdateLockPos()
        {
            if (!_isInit) return;
            Vector3 c = new Vector3(
                    _scene.borderCenter[0] * _scene.mapData.tileSize[0] - _scene.mapData.tileSize[0] / 2,
                    _scene.borderCenter[2] * _scene.mapData.tileSize[2] - _scene.mapData.tileSize[2] / 2,
                    _scene.borderCenter[1] * _scene.mapData.tileSize[1] - _scene.mapData.tileSize[1] / 2
                ) ;
            transform.localPosition = c;
        }

    }
}
