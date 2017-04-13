using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CreateSpecialField : EditorWindow
{
    internal static string baseTypeName;
    internal static string targetId;
    private string[] typeNames;
    private Type[] types;
    private int ind = 0;
    
    void Awake()
    {
        types = GenerateConfig.GetRelationalTypeArray(baseTypeName);
        typeNames = new string[types.Length];
        for (int i = 0; i < typeNames.Length; i++)
        {
            typeNames[i] = types[i].Name;
        }
    }

    void OnGUI()
    {
        if (types == null || typeNames == null || targetId == null || baseTypeName == null)
            return;
        GUILayout.BeginVertical();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUI.color = Color.white;
        GUILayout.Label("选择要创建的配置类：", EditorStyles.largeLabel);
        GUI.color = Color.green;
        ind = EditorGUILayout.Popup("", ind, typeNames, "LargePopup", GUILayout.Width(typeNames[ind].Length * 8 + 20));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUI.color = Color.white;
        if (GUILayout.Button("确认创建", "LargeButton"))
        {
            ConfigEditorUI ceui = CreateInstance<ConfigEditorUI>();
            ceui.title = typeNames[ind];
            ceui.typePool = types[ind];
            ceui.Init();
            
            string[] newStrs = new string[ceui.currentCfgFileInfo[0].Count];
            newStrs[0] = targetId;
            for (int i = 1; i < newStrs.Length; i++)
            {
                newStrs[i] = "";
            }
            ceui.currentCfgFileInfo.Add(newStrs.ToList());
            
            ceui.StateInit();
            ceui.Paging(1, ceui.currentCfgFileInfo.Count - 2);
            ceui.Show();
            SetNull();
        }
        if (GUILayout.Button("返回", "LargeButton"))
        {
            SetNull();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    void SetNull()
    {
        types = null;
        typeNames = null;
        baseTypeName = null;
        targetId = null;
        Close();
    }
}
