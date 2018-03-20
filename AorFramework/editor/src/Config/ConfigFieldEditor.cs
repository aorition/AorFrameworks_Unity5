using System;
using System.Collections.Generic;
using AorBaseUtility;
using AorBaseUtility.Config;
using UnityEditor;
using UnityEngine;

public class ConfigFieldEditor : EditorWindow
{
    internal static int Index = -1;
    internal static string[] efs;
    internal static ConfigEditorUI ceui;
    private string fieldType;
    private string fieldTip;
    private string fieldName;
    private int ind;

    void Awake()
    {
        if (Index < 0 || ceui.typePool == null || efs == null)
            return;
        fieldType = fieldTip = fieldName = "";
    }

    void OnGUI()
    {
        if (ceui == null)
            return;
        
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUI.color = Color.white;
        GUILayout.Label("配置表名：", EditorStyles.largeLabel, GUILayout.Width(60));
        GUI.color = Color.cyan;
        GUILayout.Label(ceui.typePool.Name, EditorStyles.largeLabel, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUI.color = Color.green;
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("字段类型 ： " + fieldType, EditorStyles.largeLabel, GUILayout.Width(fieldType.Length * 8 + 100));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("字段说明 ： ", "说明里请不要包含换行符"), EditorStyles.largeLabel, GUILayout.Width(60));
        fieldTip = EditorGUILayout.TextField("", fieldTip, "LargeTextField", GUILayout.Width(fieldTip.Length * 8 + 100));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("字段名 ： " + fieldName, EditorStyles.largeLabel, GUILayout.Width(fieldName.Length * 8 + 100));
        for (int i = 0; i < efs.Length; i++)
        {
            if (fieldName == efs[i])
            {
                ind = i;
                Attribute[] a = Attribute.GetCustomAttributes(ceui.typePool.GetField(efs[ind]));
                fieldTip = a.Length != 0 && a[0].GetType() == typeof(ConfigCommentAttribute) ? (string)a[0].GetType().GetField("comment").GetValue(a[0]) : fieldTip;
                break;
            }
        }
        ind = EditorGUILayout.Popup("", ind, efs, "LargePopup", GUILayout.Width(efs[ind].Length * 8 + 20));
        fieldName = efs[ind];
        fieldType = GenerateConfig.RuntimeTypeChangeToTypeName(ceui.typePool.GetField(efs[ind]).FieldType);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        GUI.color = Color.white;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("确认添加", "LargeButton", GUILayout.Width(100)))
        {
            ceui.currentCfgFileInfo[0].Insert(Index, fieldType);
            ceui.currentCfgFileInfo[1].Insert(Index, fieldTip);
            ceui.currentCfgFileInfo[2].Insert(Index, fieldName);
            for (int i = 3; i < ceui.currentCfgFileInfo.Count; i++)
            {
                ceui.currentCfgFileInfo[i].Insert(Index, "");
            }
            ceui.StateInit();
            ceui.TypeInit();
            Close();
            return;
        }
        GUILayout.Space(20);
        if (GUILayout.Button("取消", "LargeButton", GUILayout.Width(100)))
        {
            Close();
            return;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}