using System;
using System.Collections.Generic;
using AorBaseUtility;
using Framework;
using Framework.Graphic;
using Framework.Graphic.Utility;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VisualCameraExtension))]
public class VisualCameraExtensionEditor : Editor
{

    private List<Type> _bodyComps;
    private List<string> _bodyCompNames;
    private List<Type> _aimComps;
    private List<string> _aimCompNames;
    private List<Type> _noiseComps;
    private List<string> _noiseCompNames;

    private VisualCameraExtension _target;
    private bool isInit = false;

    private void init()
    {
        _target = target as VisualCameraExtension;
        _target.ref_InvokeMethod_Inst_NonPublic("Awake", null);

        //
        _bodyComps = new List<Type>();
        _bodyCompNames = new List<string>();
        _bodyCompNames.Add("Do nothing");
        _aimComps = new List<Type>();
        _aimCompNames = new List<string>();
        _aimCompNames.Add("Do nothing");
        _noiseComps = new List<Type>();
        _noiseCompNames = new List<string>();
        _noiseCompNames.Add("none");
        //
        var allExtensions = EditorReflectionHelpers.GetTypesInAllLoadedAssemblies(
                            (Type t) => t.IsSubclassOf(typeof(VisualCameraComponentBase)));
        foreach (Type t in allExtensions)
        {
            if (typeof(IVisualCameraBodyComponent).IsAssignableFrom(t))
            {
                _bodyComps.Add(t);
                _bodyCompNames.Add(t.Name);
            }
            if (typeof(IVisualCameraAimComponent).IsAssignableFrom(t))
            {
                _aimComps.Add(t);
                _aimCompNames.Add(t.Name);
            }
            if (typeof(IVisualCameraNoiseComponent).IsAssignableFrom(t))
            {
                _noiseComps.Add(t);
                _noiseCompNames.Add(t.Name);
            }
        }
        isInit = true;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!isInit) init();

        GUILayout.Space(5);
        IVisualCameraBodyComponent body = _target.gameObject.GetInterface<IVisualCameraBodyComponent>();
        draw_BodyComponent_Ctrl(body);
        GUILayout.Space(8);
        IVisualCameraAimComponent aim = _target.gameObject.GetInterface<IVisualCameraAimComponent>();
        draw_AimComponent_Ctrl(aim);
        GUILayout.Space(8);
        IVisualCameraNoiseComponent noise = _target.gameObject.GetInterface<IVisualCameraNoiseComponent>();
        draw_NoiseComponent_Ctrl(noise);
        GUILayout.Space(5);
    }

    private bool _body_ctrl_togle = true;
    private void draw_BodyComponent_Ctrl(IVisualCameraBodyComponent body)
    {

        GUILayout.BeginVertical("box");

        _body_ctrl_togle = EditorGUILayout.Foldout(_body_ctrl_togle, "  Body");

        if (_body_ctrl_togle)
        {

            int sidx;
            if (body == null)
            {
                sidx = 0;
            }
            else
            {
                sidx = _bodyComps.FindIndex(b => body.GetType().Name.Equals(b.Name)) + 1;
            }
            int nSidx = EditorGUILayout.Popup("Method ",sidx, _bodyCompNames.ToArray());
            if (!nSidx.Equals(sidx))
            {
                VisualCameraExtension.ClearAllBodyComponets(_target.gameObject);
                //
                if (nSidx > 0)
                {
                    Component comp = _target.gameObject.AddComponent(_bodyComps[nSidx - 1]);
                    comp.hideFlags = HideFlags.HideInInspector;
                    body = (IVisualCameraBodyComponent) comp;
                }
            }

            if (body != null)
            {
                GUILayout.Space(5);
                draw_subComponetInspector(body as Component);
            }
        }

        GUILayout.EndVertical();
    }

    private bool _aim_ctrl_togle = true;
    private void draw_AimComponent_Ctrl(IVisualCameraAimComponent aim)
    {

        GUILayout.BeginVertical("box");

        _aim_ctrl_togle = EditorGUILayout.Foldout(_aim_ctrl_togle, "  Aim ");

        if (_aim_ctrl_togle)
        {

            int sidx;
            if (aim == null)
            {
                sidx = 0;
            }
            else
            {
                sidx = _aimComps.FindIndex(b => aim.GetType().Name.Equals(b.Name)) + 1;
            }
            int nSidx = EditorGUILayout.Popup("Method ", sidx, _aimCompNames.ToArray());
            if (!nSidx.Equals(sidx))
            {
                VisualCameraExtension.ClearAllAimComponets(_target.gameObject);
                //
                if (nSidx > 0)
                {
                    Component comp = _target.gameObject.AddComponent(_aimComps[nSidx - 1]);
                    comp.hideFlags = HideFlags.HideInInspector;
                    aim = (IVisualCameraAimComponent)comp;
                }
            }

            if (aim != null)
            {
                GUILayout.Space(5);
                draw_subComponetInspector(aim as Component);
            }
        }

        GUILayout.EndVertical();

    }

    private bool _noise_ctrl_togle = true;
    private void draw_NoiseComponent_Ctrl(IVisualCameraNoiseComponent noise)
    {

        GUILayout.BeginVertical("box");

        _noise_ctrl_togle = EditorGUILayout.Foldout(_noise_ctrl_togle, "  Noise ");

        if (_noise_ctrl_togle)
        {

            int sidx;
            if (noise == null)
            {
                sidx = 0;
            }
            else
            {
                sidx = _noiseComps.FindIndex(b => noise.GetType().Name.Equals(b.Name)) + 1;
            }
            int nSidx = EditorGUILayout.Popup("Method ", sidx, _noiseCompNames.ToArray());
            if (!nSidx.Equals(sidx))
            {
                VisualCameraExtension.ClearAllNoiseComponets(_target.gameObject);
                //
                if (nSidx > 0)
                {
                    Component comp = _target.gameObject.AddComponent(_noiseComps[nSidx - 1]);
                    comp.hideFlags = HideFlags.HideInInspector;
                    noise = (IVisualCameraNoiseComponent)comp;
                }
            }

            if (noise != null)
            {
                GUILayout.Space(5);
                draw_subComponetInspector(noise as Component);
            }
        }

        GUILayout.EndVertical();

    }
    
    
    private void draw_subComponetInspector(Component component)
    {
        if (!component) return;
        GUILayout.Space(5);

        Editor e = Editor.CreateEditor(component);
        e.OnInspectorGUI();

        GUILayout.Space(5);
    }

}
