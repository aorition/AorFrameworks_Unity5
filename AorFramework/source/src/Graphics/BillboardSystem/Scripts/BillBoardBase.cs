using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic
{
    public class BillBoardBase : MonoBehaviour
    {

        [SerializeField]
        private Material _material;

        [Tooltip("启用相机视框检查")]
        public bool UseInViewCheck = true;

        [Tooltip("是否始终面对摄像机")]
        public bool Forward = false;

        [Tooltip("使用Sprite的中点作为支点,否则使用底端中点作为支点")]
        [SerializeField]
        [SetProperty("UseCenterPovit")]
        private bool _UseCenterPovit = false;

        public bool UseCenterPovit
        {
            get { return _UseCenterPovit; }
            set
            {
                _UseCenterPovit = value;
                _createVertices();
            }
        }

        [Tooltip("锁定旋转到Y轴")]
        public bool LockToY = false;

        [Tooltip("终面对摄像机时, LookAt方式面对摄像机,否则使用矩阵投射方式面对摄像机(二者效果有细微的区别)")]
        public bool UseLookAtMethod = true;

        [HideInInspector]
        public Vector3[] Vertices;
        [HideInInspector]
        public Color[] Colors;
        [HideInInspector]
        public Vector2[] TexCoords;

        [Tooltip("渲染排序深度(使用没有Ztest材质时起效)")]
        public float Depth = -1;

        public Material Material
        {
            get { return _material; }
            set
            {
                bool isAdd = _material != null;
                _material = value;
                if (!isAdd && isActiveAndEnabled)
                {
                    OnEnable();
                }
            }
        }

        private void _createVertices()
        {

            if (UseCenterPovit)
            {
                Vertices = new[]
                {
                    new Vector3(-0.5f, -0.5f, 0),
                    new Vector3(-0.5f, 0.5f, 0),
                    new Vector3(0.5f, 0.5f, 0),
                    new Vector3(0.5f, -0.5f, 0)
                };
            }
            else
            {
                Vertices = new[]
                {
                    new Vector3(-0.5f, 0f, 0),
                    new Vector3(-0.5f, 1f, 0),
                    new Vector3(0.5f, 1f, 0),
                    new Vector3(0.5f, 0f, 0)
                };
            }
        }

        protected virtual void Awake()
        {

            _createVertices();

            TexCoords = new[]
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
            };
            Colors = new[]
            {
                new Color(1, 1, 1, 1),
                new Color(1, 1, 1, 1),
                new Color(1, 1, 1, 1),
                new Color(1, 1, 1, 1)
            };
        }

        private bool _isAddInManager = false;
        protected virtual void OnEnable()
        {
            //在启用视框检测时不会在OnEnable注册
            if (!UseInViewCheck && Material)
            {
                BillBoardManager.Request(() =>
                {
                    BillBoardManager.Instance.AddSprite(this);
                });
            }
        }

        protected virtual void OnDisable()
        {
            if (_isAddInManager && BillBoardManager.IsInit())
            {
                BillBoardManager.Instance.RemoveSprite(this);
                _isAddInManager = false;
            }
        }

        protected virtual void Update()
        {

            if (!Material || !UseInViewCheck) return;

            if (!_isAddInManager && CheckInCamera())
            {
                BillBoardManager.Request(() =>
                {
                    BillBoardManager.Instance.AddSprite(this);
                });
                _isAddInManager = true;
            }

            if (_isAddInManager && !CheckInCamera() && BillBoardManager.IsInit())
            {
                BillBoardManager.Instance.RemoveSprite(this);
                _isAddInManager = false;
            }

        }

        protected virtual bool CheckInCamera()
        {
            return IsInView(transform.rotation*Vertices[0] + transform.position)
                   || IsInView(transform.rotation*Vertices[1] + transform.position)
                   || IsInView(transform.rotation*Vertices[2] + transform.position)
                   || IsInView(transform.rotation*Vertices[3] + transform.position);
        }

        protected bool IsInView(Vector3 worldPos)
        {
            if (!BillBoardManager.MainCamera) return false;
            Camera mainCamera = BillBoardManager.MainCamera;
            Transform camTransform = mainCamera.transform;

            float distance = Vector3.Distance(transform.position, camTransform.position);
            if (distance > mainCamera.farClipPlane || distance < mainCamera.nearClipPlane) return false;

            Vector2 viewPos = mainCamera.WorldToViewportPoint(worldPos);
            Vector3 dir = (worldPos - camTransform.position).normalized;
            float dot = Vector3.Dot(camTransform.forward, dir);     //判断物体是否在相机前面  

            if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
                return true;
            else
                return false;
        }

    }
}
