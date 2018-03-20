using System;
using UnityEngine;

namespace YoukiaUnity.CinemaSystem
{
    /// <summary>
    /// 模拟编辑器摄像机移动类
    /// </summary>
    public class FlyCamera : MonoBehaviour
    {
        private const float MaxEffectiveTPF = 1.0f;

        protected enum FlyCameraAction
        {
            //Rotation
            Left,
            Right,
            Up,
            Down,

            //Translation
            Forward,
            Backword,
            Leftword,
            Rightword,
            Rise,
            Lower,

            //Zoom
            ZoomIn,
            ZoomOut,

            Count
        }

        public float MoveSpeed = 15;
        public float RotationSpeed = 1;
        public float ZoomSpeed = 1;

        private Vector3 _upVector;
        private float[] _actionStatus;

        private int _oldMouseX;
        private int _oldMouseY;
        private bool _rightMouseDown = false;

//        private Camera _camera;
        // Use this for initialization
        void Start() {
//            _camera = GetComponent<Camera>();
            _upVector = Vector3.up;
            _actionStatus = new float[(int)FlyCameraAction.Count];
        }

        // Update is called once per frame
        void Update()
        {
            _keyPressed(this);
            _keyReleased(this);
            _mouseMoved(this, (int)Input.mousePosition.x, (int)Input.mousePosition.y);
            _wheelScrooled(this);

            float effectiveTPF = Math.Min(Time.deltaTime, MaxEffectiveTPF);

            if (_actionStatus[(int)FlyCameraAction.Left] != 0)
            {
                _rotate_camera(transform, -_actionStatus[(int)FlyCameraAction.Left] * RotationSpeed, Vector3.up, _upVector);
                _actionStatus[(int)FlyCameraAction.Left] = 0;
            }
            if (_actionStatus[(int)FlyCameraAction.Right] != 0)
            {
                _rotate_camera(transform, _actionStatus[(int)FlyCameraAction.Right] * RotationSpeed, Vector3.up, _upVector);
                _actionStatus[(int)FlyCameraAction.Right] = 0;
            }
            if (_actionStatus[(int)FlyCameraAction.Up] != 0)
            {
                Vector3 camDir = transform.forward;
                float dotvy = Vector3.Dot(Vector3.up, camDir);
                Vector3 axis = Vector3.Cross(camDir, _upVector).normalized;
                if (dotvy > -0.99f)
                {
                    _rotate_camera(transform, -_actionStatus[(int)FlyCameraAction.Up] * RotationSpeed, axis, _upVector);
                }
                _actionStatus[(int)FlyCameraAction.Up] = 0;
            }
            if (_actionStatus[(int)FlyCameraAction.Down] != 0)
            {
                Vector3 camDir = transform.forward;
                float dotvy = Vector3.Dot(Vector3.up, camDir);
                Vector3 axis = Vector3.Cross(camDir, _upVector).normalized;
                if (dotvy < 0.99f)
                {
                    _rotate_camera(transform, _actionStatus[(int)FlyCameraAction.Down] * RotationSpeed, axis, _upVector);
                }
                _actionStatus[(int)FlyCameraAction.Down] = 0;
            }
            if (_actionStatus[(int)FlyCameraAction.Forward] != 0)
            {
                Vector3 direction = transform.forward;
                _move_camera(transform, _actionStatus[(int)FlyCameraAction.Forward] * MoveSpeed * effectiveTPF, direction);
            }
            if (_actionStatus[(int)FlyCameraAction.Backword] != 0)
            {
                Vector3 direction = transform.forward;
                _move_camera(transform, -_actionStatus[(int)FlyCameraAction.Backword] * MoveSpeed * effectiveTPF, direction);
            }
            if (_actionStatus[(int)FlyCameraAction.Leftword] != 0)
            {
                Vector3 direction = transform.forward;
                Vector3 up = transform.up;
                Vector3 left = Vector3.Cross(up, direction);
                _move_camera(transform, -_actionStatus[(int)FlyCameraAction.Leftword] * MoveSpeed * effectiveTPF, left);
            }
            if (_actionStatus[(int)FlyCameraAction.Rightword] != 0)
            {
                Vector3 direction = transform.forward;
                Vector3 up = transform.up;
                Vector3 left = Vector3.Cross(up, direction);
                _move_camera(transform, _actionStatus[(int)FlyCameraAction.Rightword] * MoveSpeed * effectiveTPF, left);
            }
            if (_actionStatus[(int)FlyCameraAction.Rise] != 0)
            {
                Vector3 up = Vector3.up;
                _move_camera(transform, _actionStatus[(int)FlyCameraAction.Rise] * MoveSpeed * effectiveTPF, up);
            }
            if (_actionStatus[(int)FlyCameraAction.Lower] != 0)
            {
                Vector3 up = Vector3.up;
                _move_camera(transform, -_actionStatus[(int)FlyCameraAction.Lower] * MoveSpeed * effectiveTPF, up);
            }
            if (_actionStatus[(int)FlyCameraAction.ZoomIn] != 0)
            {
                _zoom_camera(transform, -_actionStatus[(int)FlyCameraAction.ZoomIn] * ZoomSpeed);
                _actionStatus[(int)FlyCameraAction.ZoomIn] = 0;
            }
            if (_actionStatus[(int)FlyCameraAction.ZoomOut] != 0)
            {
                _zoom_camera(transform, _actionStatus[(int)FlyCameraAction.ZoomOut] * ZoomSpeed);
                _actionStatus[(int)FlyCameraAction.ZoomOut] = 0;
            }
        }

        static void _rotate_camera(Transform cam, float value, Vector3 axis, Vector3 up)
        {
            Quaternion quatSrc = cam.rotation;
            Quaternion quat = Quaternion.AngleAxis(value, axis);
            quat = quat * quatSrc;
            cam.rotation = quat;
            Vector3 direction = cam.forward;
            Vector3 left = Vector3.Cross(up, direction).normalized;
            Vector3 fixedUp = Vector3.Cross(direction, left);
            cam.up = fixedUp;
            cam.forward = direction;
        }

        static void _move_camera(Transform cam, float value, Vector3 axis)
        {
            Vector3 vec;
            Vector3 vecSrc = cam.position;
            vec.x = axis.x * value;
            vec.y = axis.y * value;
            vec.z = axis.z * value;
            vec += vecSrc;
            cam.position = vec;
        }

        static void _zoom_camera(Transform cam, float value)
        {
            Camera c = cam.GetComponent<Camera>();
            if (!c)
            {
                return;
            }

            float zoom = 1.0f + 1.0f / 5.0f * value;
            if (c.orthographic)
            {
                c.orthographicSize = c.orthographicSize * zoom;
            }
            else
            {
                c.fieldOfView = c.fieldOfView * zoom;
            }
        }

        static void _flyCameraAction(FlyCamera flyCam, FlyCameraAction action, float value)
        {
            flyCam._actionStatus[(int)action] = value;
        }

        private void _keyPressed(FlyCamera flyCam)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                _flyCameraAction(flyCam, FlyCameraAction.Forward, 1);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                _flyCameraAction(flyCam, FlyCameraAction.Backword, 1);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                _flyCameraAction(flyCam, FlyCameraAction.Leftword, 1);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                _flyCameraAction(flyCam, FlyCameraAction.Rightword, 1);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _flyCameraAction(flyCam, FlyCameraAction.Lower, 1);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                _flyCameraAction(flyCam, FlyCameraAction.Rise, 1);
            }
        }

        private void _keyReleased(FlyCamera flyCam)
        {
            if (Input.GetKeyUp(KeyCode.W))
            {
                _flyCameraAction(flyCam, FlyCameraAction.Forward, 0);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                _flyCameraAction(flyCam, FlyCameraAction.Backword, 0);
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                _flyCameraAction(flyCam, FlyCameraAction.Leftword, 0);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                _flyCameraAction(flyCam, FlyCameraAction.Rightword, 0);
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                _flyCameraAction(flyCam, FlyCameraAction.Lower, 0);
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                _flyCameraAction(flyCam, FlyCameraAction.Rise, 0);
            }
        }

        static void _mouseMoved(FlyCamera flyCam, int x, int y)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                flyCam._rightMouseDown = true;
                flyCam._oldMouseX = x;
                flyCam._oldMouseY = y;
            }
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                flyCam._rightMouseDown = false;
            }
            float aspect = 1;//flyCam._camera.aspect;
            if (flyCam._rightMouseDown && (flyCam._oldMouseX != x || flyCam._oldMouseY != y))
            {
                int offsetX = x - flyCam._oldMouseX;
                int offsetY = y - flyCam._oldMouseY;
                if (offsetX > 0)
                {
                    _flyCameraAction(flyCam, FlyCameraAction.Right, offsetX / 10.0f);
                }
                else
                {
                    _flyCameraAction(flyCam, FlyCameraAction.Left, -offsetX / 10.0f);
                }
                if (offsetY > 0)
                {
                    _flyCameraAction(flyCam, FlyCameraAction.Down, offsetY / 10.0f / aspect);
                }
                else
                {
                    _flyCameraAction(flyCam, FlyCameraAction.Up, -offsetY / 10.0f / aspect);
                }
            }
            flyCam._oldMouseX = x;
            flyCam._oldMouseY = y;
        }

        static void _wheelScrooled(FlyCamera flyCam)
        {
            float amount = Input.GetAxis("Mouse ScrollWheel");

            if (amount > 0)
            {
                _flyCameraAction(flyCam, FlyCameraAction.ZoomIn, (float)amount);
            }
            else if (amount < 0)
            {
                _flyCameraAction(flyCam, FlyCameraAction.ZoomOut, -(float)amount);
            }
        }
    }
}


