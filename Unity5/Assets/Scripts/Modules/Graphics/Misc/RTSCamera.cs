using System;
using System.Collections.Generic;
using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    private const float MaxEffectiveTPF = 1.0f;
    private const float Cos22_5 = 0.92388f;
    private const float Cos67_5 = 0.382683f;
    private const float Cos112_5 = -0.382683f;
    private const float Cos157_5 = -0.92388f;

    private float[] _actionStatus;
    private Camera _camera;

    public float ScrollSpeed = 5;
    public float ZoomSpeed = 5;

    protected enum RTSCameraAction
    {
        //Move
        Left,
        Right,
        Up,
        Down,
        LeftUp,
        RightUp,
        LeftDown,
        RightDown,

        //Zoom
        ZoomIn,
        ZoomOut,

        Count
    }

    void Start()
    {
        _camera = GetComponent<Camera>();
        _actionStatus = new float[(int) RTSCameraAction.Count];
    }

    void Update()
    {
        for (int i = 0; i < (int) RTSCameraAction.Count; i++)
        {
            _actionStatus[i] = 0;
        }
        _mouseMoved((int) Input.mousePosition.x, (int) Input.mousePosition.y);
        _wheelScrooled();
    }

    void _mouseMoved(int x, int y)
    {
        float dx = x/(float) Screen.width;
        float dy = y/(float) Screen.height;

        if (dx < 0.05f || dy < 0.05f || dx >= 0.95f || dy >= 0.95f)
        {
            Vector2 dir = new Vector2(dx - 0.5f, dy - 0.5f);
            dir.Normalize();
            float dot = Vector2.Dot(dir, Vector2.up);
            float effectiveTPF = Math.Min(Time.deltaTime, MaxEffectiveTPF);

            if (dot > Cos22_5)
            {
                dir = Vector2.up;
            }
            else if (dot > Cos67_5)
            {
                dir = new Vector2(1 * Math.Sign(dir.x), 1).normalized;
            }
            else if (dot > Cos112_5)
            {
                dir = new Vector2(1 * Math.Sign(dir.x), 0).normalized;
            }
            else if (dot > Cos157_5)
            {
                dir = new Vector2(1 * Math.Sign(dir.x), -1).normalized;
            }
            else
            {
                dir = Vector2.down;
            }

            Vector4 wDir = _camera.cameraToWorldMatrix * _camera.projectionMatrix * (new Vector4(dir.x, dir.y, 0, 0));
            Vector3 axis = new Vector3(wDir.x, 0, wDir.z).normalized;

            _move_camera(ScrollSpeed * effectiveTPF, axis);
        }
    }

    void _wheelScrooled()
    {
        float amount = Input.GetAxis("Mouse ScrollWheel");
        float effectiveTPF = Math.Min(Time.deltaTime, MaxEffectiveTPF);

        if (amount > 0)
        {
            _zoom_camera(1* ZoomSpeed * effectiveTPF);
        }
        else if (amount < 0)
        {
            _zoom_camera(-1 * ZoomSpeed * effectiveTPF);
        }
    }

    public void _zoom_camera(float value)
    {
        _camera.transform.position += _camera.transform.forward * value * 3;
    }

    private void _move_camera(float value, Vector3 axis)
    {
        Vector3 vec;
        Vector3 vecSrc = _camera.transform.position;
        vec.x = axis.x*value;
        vec.y = axis.y*value;
        vec.z = axis.z*value;
        vec += vecSrc;
        _camera.transform.position = vec;
    }
}