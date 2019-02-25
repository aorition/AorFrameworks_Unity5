using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Modules.SpringBone.Editor
{
    //[CanEditMultipleObjects]
    [CustomEditor(typeof(SpringBone))]
    public class SpringBoneEditor : UnityEditor.Editor
    {

        private SpringBone _target;
        private SpringBoneEditon _editon;
        private void Awake()
        {
            _target = target as SpringBone;
            _editon = _target.gameObject.GetComponent<SpringBoneEditon>();
        }

        private void OnDestroy()
        {
            //防止SpringBoneEditon泄漏.并且SpringBoneEditon随SpringBone一同被销毁
            if (!_target && _editon)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(_editon);
                }
                else
                {
                    GameObject.DestroyImmediate(_editon);
                }
            }
        }

        public override void OnInspectorGUI()
        {

            var so = new SerializedObject(_target);

            GUILayout.Space(12);

            _draw_singleEdit_UI(so);

            GUILayout.Space(12);

            so.ApplyModifiedProperties();

            GUILayout.Space(12);

            GUI.color = Color.yellow;
            if (GUILayout.Button("Open MutiEdit Window", GUILayout.Height(36)))
            {
                SpringBoneEditWindow.init(_target.transform.root ? _target.transform.root : _target.transform);
            }
            GUI.color = Color.white;
            GUILayout.Space(12);

        }

        private void _draw_singleEdit_UI(SerializedObject so)
        {

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            so.FindProperty("LockRotaX").boolValue = EditorGUILayout.ToggleLeft("Lock X", so.FindProperty("LockRotaX").boolValue, GUILayout.Width(80));
            GUILayout.FlexibleSpace();
            so.FindProperty("LockRotaY").boolValue = EditorGUILayout.ToggleLeft("Lock Y", so.FindProperty("LockRotaY").boolValue, GUILayout.Width(80));
            GUILayout.FlexibleSpace();
            so.FindProperty("LockRotaZ").boolValue = EditorGUILayout.ToggleLeft("Lock Z", so.FindProperty("LockRotaZ").boolValue, GUILayout.Width(80));
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            EditorGUILayout.PropertyField(so.FindProperty("springEnd"));

            GUILayout.Space(5);

            EditorGUILayout.HelpBox("If you have don't have other(e.g. Animator) controlling rotation of this gameobject, enable this to fix its rotation. Otherwise don't use it.", MessageType.Info);

            GUILayout.Space(5);

            EditorGUILayout.PropertyField(so.FindProperty("useSpecifiedRotation"), new GUIContent("Use custom rotation?"));
            if (_target.useSpecifiedRotation)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(so.FindProperty("customRotation"));
                if (GUILayout.Button("Copy current rotation"))
                {
                    _target.customRotation = _target.transform.localRotation.eulerAngles;
                }
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(5);

            EditorGUILayout.LabelField("Forces");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(so.FindProperty("stiffness"));
            EditorGUILayout.PropertyField(so.FindProperty("bounciness"));
            EditorGUILayout.PropertyField(so.FindProperty("dampness"));
            EditorGUI.indentLevel--;

            GUILayout.Space(5);

            GUILayout.EndVertical();

        }

        private void OnSceneGUI()
        {
            var t = target as SpringBone;
            var so = new SerializedObject(t);
            Handles.DrawDottedLine(t.transform.position, t.transform.TransformPoint(t.springEnd), 4.0f);
            var currentPos = t.transform.TransformPoint(t.springEnd);
            var size = HandleUtility.GetHandleSize(currentPos) * 0.2f;
            EditorGUI.BeginChangeCheck();
            var movedPos = Handles.FreeMoveHandle(currentPos, Quaternion.identity, size, Vector3.one * 0.5f, Handles.SphereHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                so.FindProperty("springEnd").vector3Value =
                        t.transform.InverseTransformPoint(movedPos);
                so.ApplyModifiedProperties();
            }
        }
    }

}

