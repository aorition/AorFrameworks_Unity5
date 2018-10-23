using System.Collections;
using System.Collections.Generic;
using Framework.Editor;
using UnityEngine;
using UnityEditor;
using Framework.UI;
[CustomEditor(typeof(RectTransform))]
public class AorRectTransformEditor : DecoratorEditor
{

    public AorRectTransformEditor() : base("RectTransformEditor")
    {
    }

    private RectTransform _rt;
    private RectTransform _parentRt;

    private bool _debugInfo = false;

    private void Awake()
    {
        _rt = base.target as RectTransform;
        if(!_rt.parent)return;
        _parentRt = _rt.parent.GetComponent<RectTransform>();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //

        GUILayout.Space(10);

        GUILayout.BeginVertical("box");

        if (GUILayout.Button(new GUIContent("ConvertAToR","绝对定位转换相对定位")))
        {
            _rt.convertAbsRTRelative();
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("ConvertAToRH", "绝对定位转换相对定位(横)")))
        {
            _rt.convertAbsRTRelativeH();
        }
        if (GUILayout.Button(new GUIContent("ConvertAToRV", "绝对定位转换相对定位(竖)")))
        {
            _rt.convertAbsRTRelativeV();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button(new GUIContent("ConvertRVToA", "相对定位转换绝对定位")))
        {
            _rt.convertRelativeRTAbs();
        }
        GUILayout.EndVertical();

        GUILayout.Space(10);
        GUILayout.BeginVertical("box");

        _debugInfo = EditorGUILayout.Toggle(new GUIContent("Enable Debug Info", "启用Debug信息"), _debugInfo);
        if (_debugInfo)
        {
            GUILayout.BeginVertical("box");

            Vector3 lp = _rt.localPosition;
            EditorGUILayout.Vector3Field("Local Position", lp);
            GUILayout.Space(5);
            Vector2 ap = _rt.anchoredPosition;
            EditorGUILayout.Vector2Field("Anchored Position", ap);
            Vector2 sd = _rt.sizeDelta;
            EditorGUILayout.Vector2Field("Size Delta", sd);
            GUILayout.Space(5);
            Vector2 oMin = _rt.offsetMin;
            EditorGUILayout.Vector2Field("Offset Min", oMin);
            Vector2 oMax = _rt.offsetMax;
            EditorGUILayout.Vector2Field("Offset Max", oMax);
            GUILayout.Space(10);
            Rect ct = _rt.rect;
            EditorGUILayout.RectField("Rect", ct);
            if (_parentRt)
            {
                GUILayout.Space(5);
                Rect pct = _parentRt.rect;
                EditorGUILayout.RectField("Parent Rect", pct);
            }
            GUILayout.EndVertical();
        }

        GUILayout.EndVertical();

    }


}
