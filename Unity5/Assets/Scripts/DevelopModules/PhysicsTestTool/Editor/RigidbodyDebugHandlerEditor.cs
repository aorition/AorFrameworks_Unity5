using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Framework.Utility.Editor
{
    [CustomEditor(typeof(RigidbodyDebugHandler))]
    public class RigidbodyDebugHandlerEditor : UnityEditor.Editor
    {

        private Vector3 _dir;
        private RigidbodyDebugHandler _target;
        private void Awake()
        {
            _target = target as RigidbodyDebugHandler;
            _dir = _target.transform.position + _target.direction;

        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                GameObject.Destroy(_target);
            }
            else
            {
                GameObject.DestroyImmediate(_target);
            }
        }

        private void OnSceneGUI()
        {
            HandleUtility.AddDefaultControl(0);
            
            Vector3 os = _target.transform.position + _dir;
            Vector3 nos = Handles.PositionHandle(os, Quaternion.identity);
            _dir = nos - _target.transform.position;

            _target.direction = Vector3.Normalize(_dir);

            Handles.color = Color.red;
            Handles.DrawLine(nos, _target.transform.position);

            float dis = Vector3.Distance(nos, _target.transform.position);

            _target.velocity = dis*_target.force * _target.direction;

            Vector3 center = Vector3.Lerp(nos, _target.transform.position, 0.5f);
            Handles.Label(center, "velocity : " + _target.velocity);

        }

    }
}


