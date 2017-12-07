using System;
using System.Collections.Generic;
using UnityEngine;

namespace AorFramework.AorTile3D.runtime
{
    public class AorTile3DSceneController
    {

        #region 接口集合

        /// <summary>
        /// 当NCamera Active 发生改变
        /// </summary>
        public Action<bool> onNCameraActiveChanged;

        /// <summary>
        /// 当边框发生改变时触发
        /// </summary>
        public Action<AorTile3DScene> onBorderChange;
        private void _onBorderChange(AorTile3DScene scene)
        {
            if (onBorderChange != null) onBorderChange(scene);
        }

        /// <summary>
        /// 当地图数据发生改变时触发
        /// </summary>
        public Action<TileMapData> onMapDataChange;
        private void _onMapDataChange(TileMapData data)
        {
            if (onMapDataChange != null) onMapDataChange(data);
        }

        /// <summary>
        /// 当地图相机轴点Index发生改变时触发
        /// </summary>
        public Action<int[]> onPovitIndexChange;
        private void _onPovitIndexChange(int[] ints)
        {
            //设置Scene.BorderCenter
            _scene.SetBorderCenter(ints);
            if (onPovitIndexChange != null)
            {
                onPovitIndexChange(ints);
            }
        }

        /// <summary>
        /// 当鼠标按压Index区域发生改变时触发
        /// </summary>
        public Action<int[]> onPressOnIndex;
        private void _onPressOnIndex(int[] ints)
        {
            if (onPressOnIndex != null)
            {
                onPressOnIndex(ints);
            }
        }

        #endregion

        /// <summary>
        /// 创建SceneController
        /// </summary>
        public AorTile3DSceneController(AorTile3DScene scene, Transform sceneRoot)
        {
            _scene = scene;
            _scene.onMapDataChange += _onMapDataChange;
            _scene.onBorderChange += _onBorderChange;

            _sceneRoot = sceneRoot;
            
            rebuildTilesLayer();
        }

        public void Dispose()
        {
            if (_scene != null)
            {
                _scene.onMapDataChange -= _onMapDataChange;
                _scene.onBorderChange -= _onBorderChange;
                _scene = null;
            }

            if (_TCamera)
            {
                _TCamera.onPovitIndexChange -= _onPovitIndexChange;
                _TCamera.onPressOnIndex -= _onPressOnIndex;
                _TCamera = null;
            }

            if (_cameraRoot != null)
            {
                GameObject delR = _cameraRoot.gameObject;
                GameObject.Destroy(delR);
                _cameraRoot = null;
            }

            if (_tilesLayer != null)
            {
                GameObject delT = _tilesLayer.gameObject;
                GameObject.Destroy(delT);
                _tilesLayer = null;
            }

            //接口注销
            onBorderChange = null;
            onMapDataChange = null;
            onPovitIndexChange = null;
            onPressOnIndex = null;
            onNCameraActiveChanged = null;
        }

        private AorTile3DScene _scene;
        public AorTile3DScene scene
        {
            get { return _scene; }
        }

        private Transform _sceneRoot;
        public Transform sceneRoot
        {
            get { return _sceneRoot; }
        }

        private Transform _tilesLayer;
        public Transform tilesLayer
        {
            get { return _tilesLayer; }
        }

        private Transform _cameraRoot;
        public Transform cameraRoot
        {
            get { return _cameraRoot; }
        }

        private TileCameraHandler _TCamera;
        public TileCameraHandler TCameraHandler
        {
            get
            {
                if (!_TCamera) _TCamera = _CreateRECamera();
                return _TCamera;
            }
        }

        private NavigatorCameraHandler _NCmaera;
        public NavigatorCameraHandler NCameraHandler
        {
            get
            {
                if (!_NCmaera) _NCmaera = _CreateNRCamera();
                return _NCmaera;
            }
        }
        
        private TileCameraHandler _CreateRECamera()
        {

            _CreateECameraRoot();

            GameObject cr = new GameObject("TCameraRoot");
            cr.transform.SetParent(_cameraRoot, false);

            TileCameraHandler tCamera = cr.AddComponent<TileCameraHandler>();
            tCamera.onPovitIndexChange = _onPovitIndexChange;
            tCamera.onPressOnIndex = _onPressOnIndex;

            return tCamera;
        }

        private NavigatorCameraHandler _CreateNRCamera()
        {

            _CreateECameraRoot();

            GameObject cr = new GameObject("NCameraRoot");
            cr.transform.SetParent(_cameraRoot, false);
            NavigatorCameraHandler nCmaera = cr.AddComponent<NavigatorCameraHandler>();
            return nCmaera;
        }

        private void _CreateECameraRoot()
        {
            if (!_cameraRoot)
            {
                _cameraRoot = new GameObject("eCameraRoot").transform;
                _cameraRoot.SetParent(sceneRoot, false);
                _cameraRoot.SetAsFirstSibling();
            }
        }

        public void rebuildTilesLayer()
        {
            if (_tilesLayer)
            {
                GameObject del = _tilesLayer.gameObject;
                GameObject.Destroy(del);
                _tilesLayer = null;
            }

            _tilesLayer = new GameObject("tiles").transform;
            _tilesLayer.SetParent(sceneRoot, false);
        }

        public void SetNCameraActive(bool active)
        {

            if (NCameraHandler.gameObject.activeSelf != active) _NCmaera.gameObject.SetActive(active);
            if (onNCameraActiveChanged != null) onNCameraActiveChanged(active);

        }

        public void MoveTCameraTo(int u, int v, int w)
        {
            _TCamera.SetPosition(new Vector3(
                (float)u * scene.mapData.tileSize[0] + scene.mapData.tileSize[0] / 2,
                (float)w * scene.mapData.tileSize[2],
                (float)v * scene.mapData.tileSize[1] + scene.mapData.tileSize[1] / 2
                ));
        }

        public void RotateTCameraTo(Vector3 eulerAngles)
        {
            _TCamera.SetRotate(eulerAngles);
        }

        public void ZoomTCameraTo(Vector3 zoomAndOffest)
        {
            _TCamera.SetZoomAndOffset(zoomAndOffest);
        }

    }
}
