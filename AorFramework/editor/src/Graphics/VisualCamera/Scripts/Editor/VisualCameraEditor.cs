﻿using System.Collections;
using AorBaseUtility.Extends;
using Framework.Graphic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VisualCamera))]
public class VisualCameraEditor : Editor
{

    private VisualCamera _target;
    private void Awake()
    {
        _target = target as VisualCamera;

        //移除默认Camera上不需要的脚本
        if (Application.isEditor && !Application.isPlaying)
        {

            GUILayer cuilayer = _target.GetComponent<GUILayer>();
            if (cuilayer) DestroyImmediate(cuilayer);

            FlareLayer flarelayer = _target.GetComponent<FlareLayer>();
            if (flarelayer) DestroyImmediate(flarelayer);

            AudioListener audioListener = _target.GetComponent<AudioListener>();
            if (audioListener) DestroyImmediate(audioListener);

            if (_target.gameObject.tag == "MainCamera")
            {
                Debug.LogWarning("*** VisualCamera不能定义Tag为MainCamera! : 已替换为 Untagged.");
                _target.gameObject.tag = "Untagged";
            }

        }

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUILayout.Space(5);
        draw_SoloAndLevelField();
        GUILayout.Space(8);
        draw_InterpolationField();
        GUILayout.Space(8);
        draw_OverrideFields();
        GUILayout.Space(8);
        draw_extensionBtn();
        GUILayout.Space(5);
        serializedObject.ApplyModifiedProperties();
    }

    private void draw_SoloAndLevelField()
    {

        GUILayout.BeginVertical("box");

        GUILayout.Space(5);

        //反射模式下不相应Animation面板的帧记录.. 

        //        bool solo = (bool)_target.ref_GetField_Inst_NonPublic("_solo");
        //        bool nSolo = EditorGUILayout.ToggleLeft(new GUIContent("SOLO", "独占模式"), solo);
        //        if (!nSolo.Equals(solo))
        //        {
        //            if (Application.isPlaying)
        //            {
        //                _target.Solo = nSolo;
        //            }
        //            else
        //            {
        //                _target.ref_SetField_Inst_NonPublic("_solo", nSolo);
        //            }
        //            _isDirty = true;
        //        }

        //        EditorGUILayout.PropertyField(serializedObject.FindProperty("_solo"), new GUIContent("SOLO", "独占模式"));
        bool solo = serializedObject.FindProperty("_solo").boolValue;
        bool nSolo = EditorGUILayout.Toggle(new GUIContent("SOLO", "独占模式"), solo);
        if (!nSolo.Equals(solo))
        {
            _target.Solo = nSolo;
        }


        GUILayout.Space(5);

        //        int level = (int)_target.ref_GetField_Inst_NonPublic("_level");
        //        int nLevel = EditorGUILayout.IntField(new GUIContent("优先级"), level);
        //        if (!nLevel.Equals(level))
        //        {
        //            if (Application.isPlaying)
        //            {
        //                _target.Level = nLevel;
        //            }
        //            else
        //            {
        //                _target.ref_SetField_Inst_NonPublic("_level", nLevel);
        //            }
        //        }

        //  EditorGUILayout.PropertyField(serializedObject.FindProperty("_level"), new GUIContent("优先级"));
        float level = serializedObject.FindProperty("_level").floatValue;
        float nlevel = EditorGUILayout.FloatField(new GUIContent("优先级"), level);
        if (!nlevel.Equals(level))
        {
            _target.Level = nlevel;
        }

        GUILayout.Space(5);

        GUILayout.EndVertical();

    }

    private void draw_InterpolationField()
    {
        GUILayout.BeginVertical("box");

        GUILayout.Space(5);

        //        bool ignoreInterp = (bool)_target.ref_GetField_Inst_Public("IgnoreInterpolationOnFirstEnable");
        //        bool nIgnoreInterp = EditorGUILayout.ToggleLeft(new GUIContent("初始化时忽略插值 (主相机)"), ignoreInterp);
        //        if (!nIgnoreInterp.Equals(ignoreInterp))
        //        {
        //            _target.ref_SetField_Inst_Public("IgnoreInterpolationOnFirstEnable", nIgnoreInterp);
        //        }

        //EditorGUILayout.PropertyField(serializedObject.FindProperty("IgnoreInterpolationOnce"), new GUIContent("忽略一次插值?"));

        bool ignoreInterpolationOnce = serializedObject.FindProperty("_ignoreInterpolationOnce").boolValue;
        bool nIgnoreInterpolationOnce = EditorGUILayout.Toggle(new GUIContent("IIO", "忽略一次插值"), ignoreInterpolationOnce);
        if (!nIgnoreInterpolationOnce.Equals(ignoreInterpolationOnce))
        {
            _target.IgnoreInterpolationOnce = nIgnoreInterpolationOnce;
        }

        GUILayout.Space(5);

        //        float interpn = (float)_target.ref_GetField_Inst_NonPublic("_interpolation");
        //        float nInterpn = EditorGUILayout.Slider(new GUIContent("缓动插值", "虚拟相机与主相机产生插值效果, 值为0时关闭插值"), interpn, 0, 1);
        //        if (!nInterpn.Equals(interpn))
        //        {
        //            _target.ref_SetField_Inst_NonPublic("_interpolation", nInterpn);
        //        }

        //EditorGUILayout.PropertyField(serializedObject.FindProperty("_interpolation"), new GUIContent("缓动插值", "虚拟相机与主相机产生插值效果, 值为0时关闭插值"));

        float interpolation = serializedObject.FindProperty("_interpolation").floatValue;
        float nInterpolation = EditorGUILayout.Slider(new GUIContent("缓动插值", "虚拟相机与主相机产生插值效果, 值为0时关闭插值"), interpolation, 0, 1);
        if (!nInterpolation.Equals(interpolation))
        {
            _target.Interpolation = nInterpolation;
        }

        GUILayout.Space(5);

        GUILayout.EndVertical();
    }

    private void draw_OverrideFields()
    {

        GUILayout.BeginVertical("box");

        GUILayout.Space(5);

        GUILayout.BeginVertical("box");

        GUILayout.Label("相机覆盖参数设置");

        GUILayout.EndVertical();
        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        togeField("OverrideClearFlags", new GUIContent("Clear Flags"));
        togeField("OverrideBackground", new GUIContent("Background Color"));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        togeField("OverrideOrthographic", new GUIContent("Projection"));
        togeField("OverrideFieldOfView", new GUIContent("Field Of View"));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        togeField("OverrideNearPlane", new GUIContent("Near Clip Plane"));
        togeField("OverrideFarPlane", new GUIContent("Far Clip Plane"));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        togeField("OverrideRenderingPath", new GUIContent("Rendering Path"));
        togeField("OverrideOcclusionCulling", new GUIContent("Occlusion Culling"));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        togeField("OverrideAllowHDR", new GUIContent("Allow HDR"));
        togeField("OverrideAllowMSAA", new GUIContent("Allow MSAA"));
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.EndVertical();

    }

    private void draw_extensionBtn()
    {

        VisualCameraExtension ext = _target.gameObject.GetComponent<VisualCameraExtension>();
        if (!ext)
        {

            VisualCameraExtension.ClearAllSubComponets(_target.gameObject);

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);

            if (GUILayout.Button(new GUIContent("Extension", "添加扩展组件"), GUILayout.Height(30)))
            {
                ext = _target.gameObject.AddComponent<VisualCameraExtension>();

                HardLockToTarget b = _target.gameObject.AddComponent<HardLockToTarget>();
                b.hideFlags = HideFlags.HideInInspector;
                _target.ref_SetField_Inst_NonPublic("_bodyExtMethod", b);

                HardLookAt a = _target.gameObject.AddComponent<HardLookAt>();
                a.hideFlags = HideFlags.HideInInspector;
                _target.ref_SetField_Inst_NonPublic("_aimExtMethod", b);
            }

            GUILayout.Space(5);

            GUILayout.EndVertical();
        }
    }

    //--------------------------------------------------------------------

    private void togeField(string fieldName, GUIContent label)
    {
        //        bool tog = (bool)_target.ref_GetField_Inst_Public(fieldName);
        //        bool ntog = EditorGUILayout.ToggleLeft(label, tog);
        //        if (!ntog.Equals(tog))
        //        {
        //            _target.ref_SetField_Inst_Public(fieldName, ntog);
        //        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName), label);
    }

}
