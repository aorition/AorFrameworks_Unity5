using System;
using System.Collections.Generic;

using Assets.AorTile3D.Scripts.runtimeEditor;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AorTile3DEditorUIView))]
public class AorTile3DEditorUIViewEditor : Editor
{

    private int _subUIId = 0;
    private bool _enableUI = false;
    private string[] _SubUILabels;

    private AorTile3DEditorUIView _target;

    private void Awake()
    {

        _target = target as AorTile3DEditorUIView;

        int i, len = (int) AorTile3DEditorUIView.ATDESubDefine.Counts;
        _SubUILabels = new string[len + 1];
        _SubUILabels[0] = "null";
        for (i = 0; i < len; i++)
        {
            _SubUILabels[i+1] = ((AorTile3DEditorUIView.ATDESubDefine)i).ToString();
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //
        if (Application.isPlaying)
        {

            GUILayout.Space(20);
            
            GUILayout.Label("RunTime Testing");
            GUILayout.BeginVertical("box");
            GUILayout.Space(10);
            GUILayout.BeginVertical("box");
            GUILayout.Space(5);
            GUILayout.Label("SubUI Enabled Switch:");
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            _subUIId = EditorGUILayout.Popup(_subUIId, _SubUILabels);
            if (_subUIId > 0)
            {
                //int[] _UIStates
                int[] uiStates = (int[]) _target.ref_GetField_Inst_NonPublic("_UIStates");
                _enableUI = uiStates[_subUIId - 1] != 0;

                bool nEnable = EditorGUILayout.ToggleLeft("isEnabled", _enableUI);
                if (nEnable != _enableUI)
                {
                    _target.setUIEnable((AorTile3DEditorUIView.ATDESubDefine)(_subUIId - 1), nEnable);
                }

            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            GUILayout.EndVertical();

        }
        
    }
}
