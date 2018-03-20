using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoukiaUnity.Graphics;

public class CameraController : MonoBehaviour
{

    private Camera _camera;
    public Action OnCameraPreRender;

    void OnPreRender()
    {
        if (_camera == null)
            _camera = GetComponent<Camera>();

        if (!_camera.orthographic)
            GraphicsManager.GetInstance().CameraSetPerspective(_camera);

        if (OnCameraPreRender != null)
            OnCameraPreRender();

    }




}
