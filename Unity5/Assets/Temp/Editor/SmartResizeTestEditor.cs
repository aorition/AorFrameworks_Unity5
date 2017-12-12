using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SmartResizeTest))]
public class SmartResizeTestEditor : Editor
{


    private SmartResizeTest _target;
    private RectTransform _rt;

    private void Awake()
    {
        _target = target as SmartResizeTest;
        _rt = _target.GetComponent<RectTransform>();
    }

    private Vector2 _pivot;
    private Vector2 _anchorMin;
    private Vector2 _anchorMax;

    private Vector2 _anchoredPos;
    private Vector2 _sizeDelta;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //

        GUILayout.BeginVertical("box");
        
        _anchoredPos = EditorGUILayout.Vector2Field("Anchored Pos", _anchoredPos);
        _sizeDelta = EditorGUILayout.Vector2Field("Size Delta", _sizeDelta);

        GUILayout.Space(5);
        
        _anchorMin = EditorGUILayout.Vector2Field("Anchor Min", _anchorMin);
        _anchorMax = EditorGUILayout.Vector2Field("Anchor Max", _anchorMax);

        GUILayout.Space(5);

        _pivot = EditorGUILayout.Vector2Field("Pivot", _pivot);

        GUILayout.Space(10);

        if (GUILayout.Button("SmartResize"))
        {
            _rt.SmartResize(_pivot, _anchorMin, _anchorMax, _anchoredPos, _sizeDelta);
        }

        if (GUILayout.Button("GetSmartRect"))
        {
            Vector3 a;
            Vector2 s;
            _rt.GetSmartRect(out a, out s);
            Debug.Log("******* >> [" + a.x + "," + a.y + "," + a.z
                    + "],[" + s.x + "," + s.y + "]"
                );
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("SetPovit"))
        {
            _rt.pivot = _pivot;
        }
        if (GUILayout.Button("SetSmartPovit"))
        {
            _rt.SmartPivot(_pivot);
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

    }
}
