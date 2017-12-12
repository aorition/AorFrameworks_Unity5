using UnityEditor;
using UnityEngine;
using System.Collections;
using AorFramework.module;
using YoukiaUnity.CinemaSystem;
using YoukiaUnity.Graphics;


[CustomEditor(typeof(SubCameraInfo))]
public class SubCameraInfoEditor : Editor
{


    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        SerializedObject m_Object = new SerializedObject(target);

        EditorGUILayout.HelpBox("摄像机", MessageType.Warning);


        GraphicsManager.eCameraOpenState state = (GraphicsManager.eCameraOpenState)EditorGUILayout.EnumPopup(new GUIContent("开机状态", " 开机状态"), (target as SubCameraInfo).OpenState);

        (target as SubCameraInfo).OpenState = state;
        EditorGUILayout.PropertyField(m_Object.FindProperty("_Level"), new GUIContent("优先级", "优先级高的摄像机优先渲染"));

        EditorGUILayout.PropertyField(m_Object.FindProperty("OverrideGraphicSetting"), new GUIContent("覆盖图形参数", "覆盖一些参数利于表现"));



        if (m_Object.FindProperty("OverrideGraphicSetting").boolValue)
        {
            EditorGUILayout.PropertyField(m_Object.FindProperty("OverrideNearClip"), new GUIContent("覆盖摄像机近裁面", "覆盖一些参数利于表现"));
            EditorGUILayout.PropertyField(m_Object.FindProperty("OverrideFarClip"), new GUIContent("覆盖摄像机远裁面", "覆盖一些参数利于表现"));
            EditorGUILayout.PropertyField(m_Object.FindProperty("OverrideFogDestance"), new GUIContent("覆盖大气雾距离", "覆盖一些参数利于表现"));
            EditorGUILayout.PropertyField(m_Object.FindProperty("OverrideFogDestiy"), new GUIContent("覆盖大气雾浓度", "覆盖一些参数利于表现"));
        }

        SubCameraInfo cam = target as SubCameraInfo;

        m_Object.ApplyModifiedProperties();

    }
}
