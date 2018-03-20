using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleSystem;
using UnityEngine;
using YoukiaUnity.Misc;
using YoukiaUnity.View;

public class MousePicker : MonoBehaviour
{
    private Camera _camera;
    private Transform _target;
    private float _oldMouseX;
    private float _oldMouseY;

    protected void Start()
    {
        _camera = GetComponent<Camera>();
    }


    protected void Update()
    {
        if (!_camera)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 posN = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.nearClipPlane);
            Vector3 posF = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.farClipPlane);

            posN = _camera.ScreenToWorldPoint(posN);
            posF = _camera.ScreenToWorldPoint(posF);

            Ray ray = new Ray(posN, (posF - posN).normalized);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject target = hit.collider.gameObject;
                ObjectView view = target.GetComponent<ObjectView>();

                while (view == null && target.transform.parent != null)
                {
                    target = target.transform.parent.gameObject;
                    view = target.GetComponent<ObjectView>();
                }

                if (view != null)
                {
                   Utils.GetRole(view,_target);
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            _target = null;
        }
        if (_target != null)
        {
            if (_target != null)
            {
                Vector3 wTargetPos = _target.position;
                Vector3 sTargetPos = _camera.WorldToScreenPoint(wTargetPos);

                Vector2 targetPos = new Vector2(sTargetPos.x, sTargetPos.y);
                Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                float dis = Vector2.Distance(mousePos, targetPos);
                if (dis > 5)
                {
                    Vector2 sDir = (mousePos - targetPos).normalized;
                    Vector4 wDir = _camera.cameraToWorldMatrix* _camera.projectionMatrix * (new Vector4(sDir.x, sDir.y, 0, 0));
                    _target.position = _target.position + new Vector3(wDir.x, 0, wDir.z).normalized * (Math.Min(Time.deltaTime, 0.1f) * 20);
                }
            }
        }
    }
}
