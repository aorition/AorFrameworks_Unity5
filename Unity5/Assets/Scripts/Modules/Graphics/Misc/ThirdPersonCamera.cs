using System;
using AorBaseUtility;
using UnityEngine;
using YoukiaCore;

public class ThirdPersonCamera : MonoBehaviour
{
    public enum ThirdPersonAction
    {
        //Rotation
        Left,
        Right,
        Up,
        Down,
        //Zoom
        ZoomIn,
        ZoomOut,
        Count
    }

    private float[] _actionStatus;

    private Camera _camera;
    private Vector3 _localPosition = new Vector3(0, 0, -5);

    private Quaternion _localRotation = Quaternion.identity;
    private int _oldMouseX;
    private int _oldMouseY;
    private bool _rightMouseDown;
    private Vector3 _upVector;
    public GameObject Focus;
    public float RotationSpeed = 1;
    public float ZoomSpeed = 1;
    public float MinHeigthOffset = 1;
    public Vector3 FocusOffset = new Vector3(0, 1, 0);
    public Vector2 ZoomNearFar = new Vector2(1, 30);

    protected void Start()
    {
        _camera = GetComponent<Camera>();
        _upVector = Vector3.up;
        _actionStatus = new float[(int) ThirdPersonAction.Count];
    }

    public void setFocusRole(GameObject role)
    {
        Focus = role;
    }
    protected void Update()
    {
        if (!Focus)
        {
            return;
        }

        _mouseMoved((int) Input.mousePosition.x, (int) Input.mousePosition.y);
        _wheelScrooled();

        if (_actionStatus[(int) ThirdPersonAction.Left] != 0)
        {
            _rotateCamera(-_actionStatus[(int) ThirdPersonAction.Left]*RotationSpeed, Vector3.up, _upVector);
            _actionStatus[(int) ThirdPersonAction.Left] = 0;
        }
        if (_actionStatus[(int) ThirdPersonAction.Right] != 0)
        {
            _rotateCamera(_actionStatus[(int) ThirdPersonAction.Right]*RotationSpeed, Vector3.up, _upVector);
            _actionStatus[(int) ThirdPersonAction.Right] = 0;
        }
        if (_actionStatus[(int) ThirdPersonAction.Up] != 0)
        {
            var camDir = _camera.transform.forward;
            var dotvy = Vector3.Dot(Vector3.up, camDir);
            var axis = Vector3.Cross(camDir, _upVector).normalized;
            if (dotvy > -0.99f)
            {
                _rotateCamera(-_actionStatus[(int) ThirdPersonAction.Up]*RotationSpeed, axis, _upVector);
            }
            _actionStatus[(int) ThirdPersonAction.Up] = 0;
        }
        if (_actionStatus[(int) ThirdPersonAction.Down] != 0)
        {
            var camDir = transform.forward;
            var dotvy = Vector3.Dot(Vector3.up, camDir);
            var axis = Vector3.Cross(camDir, _upVector).normalized;
            if (dotvy < 0.99f)
            {
                _rotateCamera(_actionStatus[(int) ThirdPersonAction.Down]*RotationSpeed, axis, _upVector);
            }
            _actionStatus[(int) ThirdPersonAction.Down] = 0;
        }
        if (_actionStatus[(int) ThirdPersonAction.ZoomIn] != 0)
        {
            _zoomCamera(_actionStatus[(int) ThirdPersonAction.ZoomIn]*ZoomSpeed);
            _actionStatus[(int) ThirdPersonAction.ZoomIn] = 0;
        }
        if (_actionStatus[(int) ThirdPersonAction.ZoomOut] != 0)
        {
            _zoomCamera(-_actionStatus[(int) ThirdPersonAction.ZoomOut]*ZoomSpeed);
            _actionStatus[(int) ThirdPersonAction.ZoomOut] = 0;
        }

        _applyTransform();
    }

    private void _applyTransform()
    {
        var parentTranslation = Focus.transform.position;
        var parentRotation = _localRotation;

        var localTranslation = _localPosition + FocusOffset;
        var localRotation = Quaternion.identity;

        var worldRotation = parentRotation*localRotation;
        var worldTranslation = localTranslation;
        worldTranslation = worldRotation*worldTranslation;

        if (MinHeigthOffset != 0 && worldTranslation.y < (FocusOffset.y - MinHeigthOffset))
        {
            float scaleRatio = MinHeigthOffset/(FocusOffset.y - worldTranslation.y);

            worldTranslation = worldTranslation*scaleRatio;
            worldTranslation = worldTranslation + parentTranslation;
        }
        else
        {
            worldTranslation = worldTranslation + parentTranslation;
        }

        _camera.transform.position = worldTranslation;
        _camera.transform.rotation = worldRotation;
    }

    public void _cameraAction(ThirdPersonAction action, float value)
    {
        _actionStatus[(int) action] = value;
    }

    private void _rotateCamera(float value, Vector3 axis, Vector3 up)
    {
        var quatSrc = _localRotation;
        var quat = Quaternion.AngleAxis(value, axis);
        _localRotation = quat*quatSrc;
        var direction = _localRotation*Vector3.forward;
        var left = Vector3.Cross(up, direction).normalized;
        var fixedUp = Vector3.Cross(direction, left);
        _localRotation = Quaternion.LookRotation(direction, fixedUp);
    }

    private void _zoomCamera(float value)
    {
        var zoom = 1/(1 + value);

        _localPosition.Set(0, 0, YKMath.Clamp(_localPosition.z*zoom, -ZoomNearFar.y, -ZoomNearFar.x));
    }

    private void _mouseMoved(int x, int y)
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            _rightMouseDown = true;
            _oldMouseX = x;
            _oldMouseY = y;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            _rightMouseDown = false;
        }
        float aspect = 1; //flyCam._camera.aspect;
        if (_rightMouseDown && (_oldMouseX != x || _oldMouseY != y))
        {
            var offsetX = x - _oldMouseX;
            var offsetY = y - _oldMouseY;
            if (offsetX > 0)
            {
                _cameraAction(ThirdPersonAction.Right, offsetX/10.0f);
            }
            else
            {
                _cameraAction(ThirdPersonAction.Left, -offsetX/10.0f);
            }
            if (offsetY > 0)
            {
                _cameraAction(ThirdPersonAction.Down, offsetY/10.0f/aspect);
            }
            else
            {
                _cameraAction(ThirdPersonAction.Up, -offsetY/10.0f/aspect);
            }
        }
        _oldMouseX = x;
        _oldMouseY = y;
    }

    private void _wheelScrooled()
    {
        var amount = Input.GetAxis("Mouse ScrollWheel");

        if (amount > 0)
        {
            _cameraAction(ThirdPersonAction.ZoomIn, amount);
        }
        else if (amount < 0)
        {
            _cameraAction(ThirdPersonAction.ZoomOut, -amount);
        }
    }
}