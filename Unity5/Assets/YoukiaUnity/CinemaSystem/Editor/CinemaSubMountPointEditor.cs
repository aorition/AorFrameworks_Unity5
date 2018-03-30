using System;
using System.Collections.Generic;
using AorBaseUtility;
using UnityEditor;
using UnityEngine;
using YoukiaUnity.CinemaSystem;

[CustomEditor(typeof(CinemaSubMountPoint))]
public class CinemaSubMountPointEditor : Editor
{

    private CinemaSubMountPoint _target;

    private void Awake()
    {
        _target = target as CinemaSubMountPoint;
    }

    private Transform subMountPoint = null;

    public override void OnInspectorGUI()
    {
        if (_target == null) return;

        if (Application.isPlaying)
        {
            base.OnInspectorGUI();
            return;
        }

        serializedObject.Update();

        Transform mountRoot = EditorGUILayout.ObjectField(new GUIContent("设置绑定点根节点"),serializedObject.FindProperty("_mountRoot").objectReferenceValue,typeof(Transform),true) as Transform;
        if (mountRoot != serializedObject.FindProperty("_mountRoot").objectReferenceValue)
        {
            _target.ref_SetField_Inst_NonPublic("_isInit", false);
            serializedObject.FindProperty("_mountRoot").objectReferenceValue = mountRoot;
            serializedObject.FindProperty("_subMountPointPath").stringValue = "";

            Repaint();
        }

        if (mountRoot)
        {

            if (!subMountPoint && !string.IsNullOrEmpty(serializedObject.FindProperty("_subMountPointPath").stringValue))
            {
                subMountPoint = _target.getSubMountPoint(serializedObject.FindProperty("_subMountPointPath").stringValue);
            }

            if (!string.IsNullOrEmpty(serializedObject.FindProperty("_subMountPointPath").stringValue))
            {
                GUILayout.Label("Linked Sub Mount Point : " + serializedObject.FindProperty("_subMountPointPath").stringValue);
            }

            Transform n_sub = EditorGUILayout.ObjectField(new GUIContent("设置绑定点"), subMountPoint, typeof(Transform), true) as Transform;
            if (n_sub != subMountPoint)
            {
                subMountPoint = n_sub;

                if (n_sub)
                {
                    string path = _getSubPath(mountRoot, n_sub);
                    serializedObject.FindProperty("_subMountPointPath").stringValue = path;
                }

            }
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("FollowSubMountPoint"),new GUIContent("挂点跟随绑定点"));

        serializedObject.ApplyModifiedProperties();

        if (serializedObject.FindProperty("FollowSubMountPoint").boolValue)
        {
            GUI.color = Color.yellow;
            GUILayout.Label("注意！你已启用挂点跟随功能！ 请确保此挂点没有K动画！！！");
            GUI.color = Color.white;
        }

    }

    private string _getSubPath(Transform root, Transform sub, string p = "")
    {
        string s;
        if (string.IsNullOrEmpty(p))
        {
            s = sub.gameObject.name;
        }
        else
        {
            s = sub.gameObject.name + "/" + p;
        }

        if (!sub.parent || sub.parent.gameObject.name == root.gameObject.name)
        {
            return s;
        }
        else
        {
            return _getSubPath(root, sub.parent, s);
        }
    }

}
