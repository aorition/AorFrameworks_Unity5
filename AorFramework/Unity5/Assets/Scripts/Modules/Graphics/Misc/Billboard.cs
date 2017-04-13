using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using YoukiaUnity.App;
using YoukiaUnity.Graphics;

namespace YoukiaUnity
{
    public class Billboard : MonoBehaviour {
        [SerializeField]
        private Camera _camera;
        public EBillboardType Type = EBillboardType.Camera;

        public enum EBillboardType
        {
            Camera,
            YAxis,
        }


        public Camera Camera
        {
            get
            {
//                if (YKApplication.Instance != null && YKApplication.Instance.GetManager<GraphicsManager>() != null && YKApplication.Instance.GetManager<GraphicsManager>().MainCamera != null) {
                if (_graphicsManager != null && _graphicsManager.MainCamera != null) {
//                    return YKApplication.Instance.GetManager<GraphicsManager>().MainCamera;
                    return _graphicsManager.MainCamera;
                }
                else
                {
                    return Camera.main;
                }
//                if (_camera == null)
//                {
//                    return Camera.main;
//                }
//                return _camera;
            }
            set { _camera = value; }
        }

        public Quaternion InitialRotation;

        private GraphicsManager _graphicsManager;

        protected void Awake()
        {
            if (_graphicsManager == null) {
                _graphicsManager = GameObject.Find("GraphicsManager").GetComponent<GraphicsManager>();
            }

            InitialRotation = transform.rotation;
        }

        protected void Update()
        {
            if (Camera == null)
            {
                return;
            }

            Quaternion q = Camera.transform.rotation;
            if (Type != EBillboardType.Camera)
            {
                Vector3 euler = q.eulerAngles;
                q = Quaternion.Euler(0, euler.y, euler.z);
            }
            transform.rotation = InitialRotation * q;
        }
    }
}
