using System;
using System.Collections.Generic;
using UnityEngine;

namespace AorFramework.AorTile3D.runtime
{
    public class AorTile3DManager
    {

        #region static 方法集

        public static Action<string> onManagerError;
        public static void ThrowError(string errorMess)
        {
            if (onManagerError != null)
            {
                onManagerError(errorMess);
            }
        }

        public static void ThrowError(Exception error)
        {
            if (onManagerError != null)
            {
                onManagerError(error.Message);
            }
        }

        private static AorTile3DManager _instance;

        public static AorTile3DManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AorTile3DManager();
                }
                return _instance;
            }
        }

        public static void Dispose()
        {
            //
        }

        #endregion

        #region 接口集合

        /// <summary>
        /// 销毁场景时触发
        /// </summary>
        public Action onDisposeScene;

        /// <summary>
        /// 创建场景时触发
        /// </summary>
        public Action onCreateScene;

        /// <summary>
        /// 当NCamera Active 发生改变
        /// </summary>
        public Action<bool> onNCameraActiveChanged;
        private void _onNCameraActiveChanged(bool active)
        {
            if (onNCameraActiveChanged != null)
                onNCameraActiveChanged(active);
        }

        #endregion

        public bool isCreatedTCamera = false;
        public bool isCreatedNCamera = false;

        private AorTile3DManager()
        {
        }

        private AorTile3DScene _currentScene;
        public AorTile3DScene currentScene
        {
            get { return _currentScene;}
        }

        private AorTile3DSceneController _sceneController;
        public AorTile3DSceneController sceneController
        {
            get { return _sceneController; }
        }

        /// <summary>
        /// 创建 scene
        /// </summary>
        public void setupTile3DScene(AorTile3DScene scene, AorTile3DSceneController controller)
        {
            _currentScene = scene;
            _sceneController = controller;
            _sceneController.onNCameraActiveChanged += _onNCameraActiveChanged;
            if (onCreateScene != null) onCreateScene();
        }
        
        /// <summary>
        /// 销毁当前 scene
        /// </summary>
        public void DisposeCurrentScene()
        {
            if (_currentScene != null)
            {
                _currentScene.Dispose();
                _currentScene = null;
                _sceneController.Dispose();
                _sceneController = null;
                if (onDisposeScene != null) onDisposeScene();
            }
        }

        public void Update()
        {
            if(_currentScene != null) _currentScene.Update();
        }
    }
}
