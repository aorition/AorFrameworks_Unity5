using System;
using System.Collections.Generic;
using AorFramework.AorTile3D.runtime;
using UnityEngine;

namespace AorFramework.AorTile3D.runtime
{
    public class TileCameraHandler : MonoBehaviour
    {
        #region 接口集合

        /// <summary>
        /// 当 相机PovitIndex 发生改变时触发
        /// </summary>
        public Action<int[]> onPovitIndexChange;

        /// <summary>
        /// 当 mouse0 按压在某个TileIndex时触发
        /// </summary>
        public Action<int[]> onPressOnIndex;

        /// <summary>
        /// 当鼠标按下时触发, 参数: 键位id,Ctrl,Alt,Shift
        /// </summary>
        public Action<int,bool,bool,bool> onMouseDown;
        /// <summary>
        /// 当鼠标弹起时触发, 参数: 键位id,Ctrl,Alt,Shift
        /// </summary>
        public Action<int, bool, bool, bool> onMouseUp;
        /// <summary>
        /// 当鼠标按住时持续触发, 参数: 键位id,Ctrl,Alt,Shift
        /// </summary>
        public Action<int, bool, bool, bool> onMouseLoop;

        /// <summary>
        /// 当鼠标单击时触发, 参数: 键位id,Ctrl,Alt,Shift
        /// </summary>
        public Action<int, bool, bool, bool> onMouseClick;
        
        /// <summary>
        /// 当鼠标滚轮触发, 参数: 滚动数值,Ctrl,Alt,Shift
        /// </summary>
        public Action<float, bool, bool, bool> onMouseScrollWheel;

        #endregion

        #region Define

        protected const float MaxEffectiveTPF = 1.0f;

        //eCamera Fov 可调整范围限定

        protected Vector2 FovZoomLimit = new Vector2(1, 179);
        protected Vector2 ZoomLimit = new Vector2(1, 1000);

        protected float[] _clickDelayLimit = {1f, 1f, 1f};

        public bool UseDefaultMouseInteractive = true;

        public float RotationSpeed = 5;
        public float MoveSpeed = 25;
        public float ZoomSpeed = 2;

        public Vector3 DefaultCameraRotate = new Vector3(45, 0, 0);
        public Vector3 DefaultCmaeraZoom = new Vector3(0, 0, 10);
        public float DefaultCameraFOV = 60;

        public float DefaultCameraOrthographicSize = 5;

        #endregion
        
        protected enum InputState
        {
            MouseLeft,
            MouseRight,
            MouseMidden,

            ScrollWheel,
            
            Count
        }
        
        protected Transform _rotateTransform;
        protected Transform _zoomTransform;

        protected Camera _camera;
        public new Camera camera
        {
            get
            {
                return _camera;
            }
        }

        protected Vector3 _upVector;

        protected float[] _mouseDownDelayTime;
        protected Vector3[] _mousePosCache;
        protected Vector3[] _MouseRayPosCache;
        protected Vector3[] _TransformPosCache;

        // private bool[] _states;

        protected int[] _povitIdx;
        private int[] _povitIdxCache;
        private int[] _pressIdxCache;

        protected float[] _inputStates;

        protected AorTile3DScene _scene;

        protected bool _UseFixedUpdate = true;

        private void Awake()
        {
            _povitIdx = new[] {0, 0, 0};
            _povitIdxCache = new[] {0, 0, 0};
            _pressIdxCache = new[] {-1, -1, -1}; 

            _upVector = Vector3.up;
            //_states = new bool[(int) MotionState.Count];
            _inputStates = new float[(int) InputState.Count];
            
            BoxCollider collider = transform.gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(500, 10, 500);
            collider.center = new Vector3(0,-5,0);

            GameObject r = new GameObject("TCameraRotat");
            _rotateTransform = r.transform;
            _rotateTransform.SetParent(transform, false);
            
            _eRotateTarget = DefaultCameraRotate;
            _eRotateCache = _eRotateTarget;
            _rotateTransform.localEulerAngles = _eRotateTarget;

            GameObject c = new GameObject("TCamera");
            _zoomTransform = c.transform;
            _zoomTransform.SetParent(_rotateTransform, false);
            _camera = c.AddComponent<Camera>();
            _camera.fieldOfView = DefaultCameraFOV;
            _camera.orthographicSize = DefaultCameraOrthographicSize;
            _camera.depth = 50;

            //创建相机各种状态
            //createEStatuses();

            _eZoomTarget = DefaultCmaeraZoom;
            _zoomTransform.localPosition = -_eZoomTarget;

            _mouseDownDelayTime = new float[3];

            _mousePosCache = new Vector3[3];
            _MouseRayPosCache = new Vector3[3];
            _TransformPosCache = new Vector3[3];
        }

        /// <summary>
        /// 自动setup
        /// </summary>
        private void Start()
        {
            if (_isInit) return;
            if (AorTile3DManager.Instance != null && AorTile3DManager.Instance.currentScene != null)
            {
                setup(AorTile3DManager.Instance.currentScene);
            }
            else
            {
                _camera.enabled = false;
                AorTile3DManager.ThrowError("AorTile3DManager.Instance is null or AorTile3DManager.Instance.currentScene is null. TileCameraHandler init fail!");
            }
        }

        protected bool _isInit = false;
        public void setup(AorTile3DScene scene)
        {
            _scene = scene;
            _isInit = true;
        }

        private void Update()
        {
            if (!_isInit) return;
            HandleInput(); //Input类在FixedUpdate中工作很糟糕..
            if (_UseFixedUpdate) return;
            DoProcess(Time.deltaTime);
        }

        //用LateUpdate处理相机移动,防止抖动
        private void LateUpdate()
        {
            float effectiveTPF = Math.Min(Time.deltaTime, MaxEffectiveTPF);

            _eRotateCache += (_eRotateTarget - _eRotateCache) * RotationSpeed * effectiveTPF;
            _rotateTransform.localEulerAngles = _eRotateCache;

            //
            _zoomTransform.localPosition += (-_eZoomTarget - _zoomTransform.localPosition) * ZoomSpeed * effectiveTPF;
            transform.localPosition += (_ePosTarget - transform.localPosition) * MoveSpeed * effectiveTPF;
        }

        private void FixedUpdate()
        {
            if(!_isInit || !_UseFixedUpdate) return;
            DoProcess(Time.fixedDeltaTime);
        }

        private bool _ctrl, _alt, _shift;

        private void HandleInput()
        {

            #region Ctrl/Alt/Shift

            if (Input.GetKeyDown(KeyCode.LeftCommand)
                || Input.GetKeyDown(KeyCode.RightCommand)
                || Input.GetKeyDown(KeyCode.LeftControl)
                || Input.GetKeyDown(KeyCode.RightControl)
                )
            {
                _ctrl = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftCommand)
                || Input.GetKeyUp(KeyCode.RightCommand)
                || Input.GetKeyUp(KeyCode.LeftControl)
                || Input.GetKeyUp(KeyCode.RightControl)
                )
            {
                _ctrl = false;
            }
            if (Input.GetKeyDown(KeyCode.LeftAlt)
                || Input.GetKeyDown(KeyCode.RightAlt)
                )
            {
                _alt = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftAlt)
                || Input.GetKeyUp(KeyCode.RightAlt)
                )
            {
                _alt = false;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift)
                || Input.GetKeyDown(KeyCode.RightShift)
                )
            {
                _shift = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift)
                || Input.GetKeyUp(KeyCode.RightShift)
                )
            {
                _shift = false;
            }

            #endregion
            
            #region Input Mouse 

            if (Input.GetMouseButtonDown(0))
            {
                if (_inputStates[(int) InputState.MouseLeft].Equals(0))
                {
                    _inputStates[(int) InputState.MouseLeft] = 1;
                    //
                    _mouseDownDelayTime[0] = 0;
                    OnMouseDown_L(_ctrl, _alt, _shift);
                    if (onMouseDown != null) onMouseDown(0, _ctrl, _alt, _shift);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                _inputStates[(int) InputState.MouseLeft] = 0;
                //
                OnMouseUp_L(_ctrl, _alt, _shift);
                if (_mouseDownDelayTime[0] < _clickDelayLimit[0])
                {
                    OnMouseClick_L(_ctrl, _alt, _shift);
                    if (onMouseClick != null) onMouseClick(0, _ctrl, _alt, _shift);
                }
                if (onMouseUp != null) onMouseUp(0, _ctrl, _alt, _shift);
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (_inputStates[(int) InputState.MouseRight].Equals(0))
                {
                    _inputStates[(int) InputState.MouseRight] = 1;
                    //
                    _mouseDownDelayTime[1] = 0;
                    OnMouseDown_R(_ctrl, _alt, _shift);
                    if (onMouseDown != null) onMouseDown(1, _ctrl, _alt, _shift);
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                _inputStates[(int) InputState.MouseRight] = 0;
                //
                OnMouseUp_R(_ctrl, _alt, _shift);
                if (_mouseDownDelayTime[1] < _clickDelayLimit[1])
                {
                    OnMouseClick_R(_ctrl, _alt, _shift);
                    if (onMouseClick != null) onMouseClick(1, _ctrl, _alt, _shift);
                }
                if (onMouseUp != null) onMouseUp(1, _ctrl, _alt, _shift);
            }
            if (Input.GetMouseButtonDown(2))
            {
                if (_inputStates[(int) InputState.MouseMidden].Equals(0))
                {
                    _inputStates[(int) InputState.MouseMidden] = 1;
                    //
                    _mouseDownDelayTime[2] = 0;
                    OnMouseDown_M(_ctrl, _alt, _shift);
                    if (onMouseDown != null) onMouseDown(2, _ctrl, _alt, _shift);
                }
            }
            if (Input.GetMouseButtonUp(2))
            {
                _inputStates[(int) InputState.MouseMidden] = 0;
                //
                OnMouseUp_M(_ctrl, _alt, _shift);
                if (_mouseDownDelayTime[2] < _clickDelayLimit[2])
                {
                    OnMouseClick_M(_ctrl, _alt, _shift);
                    if (onMouseClick != null) onMouseClick(2, _ctrl, _alt, _shift);
                }
                if (onMouseUp != null) onMouseUp(2, _ctrl, _alt, _shift);
            }

            float amount = Input.GetAxis("Mouse ScrollWheel");
            if (!amount.Equals(0))
            {
                _inputStates[(int)InputState.ScrollWheel] = amount;
            }
            
            #endregion


        }

        //记录相机移动的目标位置
        [SerializeField]
        private Vector3 _ePosTarget;
        public void SetPosition(Vector3 pos, bool immediately = false)
        {
            _ePosTarget = pos;
            if (immediately) transform.localPosition = pos;
        }

        [SerializeField]
        private Vector3 _eRotateTarget;
        public void SetRotate(Vector3 eulerAngles, bool immediately = false)
        {
            _eRotateTarget = eulerAngles;
            if (immediately)
            {
                _eRotateCache = eulerAngles;
                _rotateTransform.localEulerAngles = _eRotateCache;
            }

        }

        private Vector3 _eRotateCache;// 旋转比较特殊
        [SerializeField]
        private Vector3 _eZoomTarget;
        public void SetZoomAndOffset(Vector3 zoomAndOffset, bool immediately = false)
        {
            _eZoomTarget = new Vector3(zoomAndOffset.x, zoomAndOffset.y, Mathf.Clamp(zoomAndOffset.z,ZoomLimit.x,ZoomLimit.y));
            if (immediately)
            {
                _zoomTransform.localPosition = _eZoomTarget;
            }
        }

        private void DoProcess(float deltaTime)
        {
            
            //计算eCamera原点位置转换的Index
            _povitIdxCache[0] = Mathf.RoundToInt(transform.position.x / _scene.mapData.tileSize[0]);
            _povitIdxCache[1] = Mathf.RoundToInt(transform.position.z / _scene.mapData.tileSize[1]);
            _povitIdxCache[2] = Mathf.RoundToInt(transform.position.y / _scene.mapData.tileSize[2]);

            if (!_inputStates[(int) InputState.MouseLeft].Equals(0))
            {
                _mouseDownDelayTime[0] += deltaTime;
                OnMouseLoop_L(_ctrl, _alt, _shift);
                if (onMouseLoop != null) onMouseLoop(0, _ctrl, _alt, _shift);
            }

            if (!_inputStates[(int)InputState.MouseRight].Equals(0))
            {
                _mouseDownDelayTime[1] += deltaTime;
                OnMouseLoop_R(_ctrl, _alt, _shift);
                if (onMouseLoop != null) onMouseLoop(1, _ctrl, _alt, _shift);
            }

            if (!_inputStates[(int)InputState.MouseMidden].Equals(0))
            {
                _mouseDownDelayTime[2] += deltaTime;
                OnMouseLoop_M(_ctrl, _alt, _shift);
                if (onMouseLoop != null) onMouseLoop(2, _ctrl, _alt, _shift);
            }

            if (!_inputStates[(int)InputState.ScrollWheel].Equals(0))
            {
                float ScrollWheelValue = _inputStates[(int) InputState.ScrollWheel];
                OnMouseScrollWheel(ScrollWheelValue, _ctrl, _alt, _shift);
                if (onMouseScrollWheel != null) onMouseScrollWheel(ScrollWheelValue, _ctrl, _alt, _shift);
                _inputStates[(int) InputState.ScrollWheel] = 0;
            }

            if (AorTile3DManager.Instance != null)
            {

                if (!AorTile3DUtils.Int3Equals(_povitIdxCache, _povitIdx))
                {
                    AorTile3DUtils.Int3Set(_povitIdxCache, _povitIdx);
                    if (onPovitIndexChange != null)
                    {
                        onPovitIndexChange(_povitIdx);
                    }
                }

            }
        }

        protected virtual void OnMouseDown_L(bool ctrl, bool alt, bool shift)
        {
            if (!UseDefaultMouseInteractive)
            {
                return;
            }
        }
        protected virtual void OnMouseLoop_L(bool ctrl, bool alt, bool shift)
        {

            if (!UseDefaultMouseInteractive)
            {
                return;
            }

            RaycastHit hitInfo;
            Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hitInfo.collider != null && hitInfo.collider.name == transform.name)
            {
                //计算鼠标按压的Idx
                int pu = Mathf.RoundToInt(hitInfo.point.x / _scene.mapData.tileSize[0]);
                int pv = Mathf.RoundToInt(hitInfo.point.z / _scene.mapData.tileSize[1]);
                int pw = Mathf.RoundToInt(hitInfo.point.y / _scene.mapData.tileSize[2]);

                if (pu != _pressIdxCache[0] || pv != _pressIdxCache[1] || pw != _pressIdxCache[2])
                {

                    _pressIdxCache[0] = pu;
                    _pressIdxCache[1] = pv;
                    _pressIdxCache[2] = pw;

                    if (onPressOnIndex != null)
                    {
                        onPressOnIndex(_pressIdxCache);
                    }
                }
            }
        }
        protected virtual void OnMouseUp_L(bool ctrl, bool alt, bool shift)
        {
            if (!UseDefaultMouseInteractive)
            {
                return;
            }

            _pressIdxCache[0] = -1;
            _pressIdxCache[0] = -1;
            _pressIdxCache[0] = -1;
        }

        protected virtual void OnMouseClick_L(bool ctrl, bool alt, bool shift)
        {
            if (!UseDefaultMouseInteractive)
            {
                return;
            }

        }

        protected virtual void OnMouseDown_R(bool ctrl, bool alt, bool shift)
        {

            if (!UseDefaultMouseInteractive)
            {
                return;
            }

            Vector2 mp = Input.mousePosition;
            RaycastHit hit;
            Physics.Raycast(_camera.ScreenPointToRay(mp), out hit);
            if (hit.collider != null && hit.collider.name == transform.name)
            {
                _MouseRayPosCache[1] = hit.point;
            }
            _TransformPosCache[1] = transform.localPosition;
            _mousePosCache[1] = mp;
        }
        protected virtual void OnMouseLoop_R(bool ctrl, bool alt, bool shift)
        {

            if (!UseDefaultMouseInteractive)
            {
                return;
            }

            if (alt)
            {
                Vector3 screenOffset = Input.mousePosition - _mousePosCache[1];
                _eRotateTarget += new Vector3(-screenOffset.y* 0.12f, screenOffset.x*0.07f);
                _mousePosCache[1] = Input.mousePosition;
                return;
            }

            if (ctrl)
            {
                Vector3 screenOffset = Input.mousePosition - _mousePosCache[1];
                SetZoomAndOffset(_eZoomTarget + new Vector3(0, 0, -screenOffset.x * 0.05f));
                _mousePosCache[1] = Input.mousePosition;
                return;
            }

            RaycastHit hit;
            Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit);
            if (hit.collider != null && hit.collider.name == transform.name)
            {
                Vector3 nowWorldPos = hit.point - _MouseRayPosCache[1];
                _ePosTarget = _TransformPosCache[1] - nowWorldPos;
            }
        }
        protected virtual void OnMouseUp_R(bool ctrl, bool alt, bool shift)
        {
            if (!UseDefaultMouseInteractive)
            {
                return;
            }
        }

        protected virtual void OnMouseClick_R(bool ctrl, bool alt, bool shift)
        {
            if (!UseDefaultMouseInteractive)
            {
                return;
            }
        }

        protected virtual void OnMouseDown_M(bool ctrl, bool alt, bool shift)
        {
            if (!UseDefaultMouseInteractive)
            {
                return;
            }

            _mousePosCache[2] = Input.mousePosition;
        }
        protected virtual void OnMouseLoop_M(bool ctrl, bool alt, bool shift)
        {
            if (!UseDefaultMouseInteractive)
            {
                return;
            }

            Vector3 mp = Input.mousePosition;
            Vector3 screenOffset = mp - _mousePosCache[2];
            _ePosTarget += (Quaternion.Euler(new Vector3(0, _rotateTransform.localEulerAngles.y, 0)) * new Vector3(screenOffset.x, 0, screenOffset.y) * 0.001f);
           // _mousePosCache[2] = mp;
        }
        protected virtual void OnMouseUp_M(bool ctrl, bool alt, bool shift)
        {
            if (!UseDefaultMouseInteractive)
            {
                return;
            }
        }

        protected virtual void OnMouseClick_M(bool ctrl, bool alt, bool shift)
        {
            if (!UseDefaultMouseInteractive)
            {
                return;
            }

            if (alt)
            {
                _eRotateTarget = DefaultCameraRotate;
                return;
            }

            if (ctrl)
            {
                _eZoomTarget = DefaultCmaeraZoom;
            }

            if (_camera.orthographic)
            {
                _camera.orthographicSize = DefaultCameraOrthographicSize;
            }
            else
            {
                _camera.fieldOfView = DefaultCameraFOV;
            }
        }

        protected virtual void OnMouseScrollWheel(float value, bool ctrl, bool alt, bool shift)
        {

            if (!UseDefaultMouseInteractive)
            {
                return;
            }

            //_zoom_camera
            float zoom = 1.0f + 1.0f / 5.0f * value;
            if (_camera.orthographic)
            {
                _camera.orthographicSize = _camera.orthographicSize * zoom;
            }
            else
            {
                float n = _camera.fieldOfView * zoom;
                _camera.fieldOfView = Mathf.Clamp(n, FovZoomLimit.x, FovZoomLimit.y);
            }
        }

    }
}
