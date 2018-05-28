using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Framework.Graphic.Editor
{
    [InitializeOnLoad]
    public class EditorVisualCameraManager
    {

        private static int initNum = 0;
        static EditorVisualCameraManager()
        {
            if (initNum == 0)
            {
                EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGui;
                EditorApplication.update += Update;
            }
            initNum ++;
        }



        private static readonly List<VisualCamera> _visualCameras = new List<VisualCamera>();
        public static List<VisualCamera> visualCameras
        {
            get { return _visualCameras; }
        }
        private static VisualCamera _currentVisualCamera;
        public static VisualCamera currentVisualCamera
        {
            get { return _currentVisualCamera; }
        }

        private static readonly List<VisualCamera> _renewList = new List<VisualCamera>();
        private static void Update()
        {

            if (Application.isPlaying)
            {
                return;
            }

            _renewList.Clear();
            foreach (VisualCamera visualCamera in _visualCameras)
            {
                if (VaildvisualCamera(visualCamera)) {
                    _renewList.Add(visualCamera);
                }
            }

            _visualCameras.Clear();
            _visualCameras.AddRange(_renewList);

            _SortAndGetCurrentVisualCamera();

            foreach (VisualCamera visualCamera in _visualCameras)
            {
                if (visualCamera == _currentVisualCamera)
                {
                    if(!visualCamera.CrrentCamera.enabled)
                        visualCamera.CrrentCamera.enabled = true;
                }
                else
                {
                    if (visualCamera.CrrentCamera.enabled)
                        visualCamera.CrrentCamera.enabled = false;
                }
            }
        }

        private static bool VaildvisualCamera(VisualCamera visualCamera)
        {
            return (visualCamera && visualCamera.gameObject && visualCamera.CrrentCamera);
        }

        private static void HierarchyWindowItemOnGui(int instanceId, Rect selectionRect)
        {

            if (Application.isPlaying)
            {
                return;
            }

            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (!gameObject) return;

            VisualCamera visualCamera = gameObject.GetComponent<VisualCamera>();
            if (visualCamera)
            {
                if (!_visualCameras.Contains(visualCamera) && VaildvisualCamera(visualCamera))
                {
                    visualCamera.CrrentCamera.enabled = false;
                    _visualCameras.Add(visualCamera);
                }
            }
        }

        private static void _SortAndGetCurrentVisualCamera()
        {
            if (_visualCameras.Count == 0)
            {
                _currentVisualCamera = null;
                return;
            }

            _visualCameras.Sort((a, b) =>
            {
                if (a.Level > b.Level)
                {
                    return 1;
                }
                else if (a.Level < b.Level)
                {
                    return -1;
                }
                return 0;
            });
            _visualCameras.Reverse();

            int idx = _visualCameras.FindIndex(v => v.Solo);
            _currentVisualCamera = idx.Equals(-1) ? _visualCameras[0] : _visualCameras[idx];
        }

    }
}
