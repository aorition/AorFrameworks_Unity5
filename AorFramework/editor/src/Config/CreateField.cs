using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{

    public class CreateField : EditorWindow
    {
        internal static ConfigEditorUI ceui;
        internal static string[] idArr;
        internal static string targetId;
        private string s = "";

        void OnGUI()
        {
            if (ceui == null || idArr == null)
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

            GUILayout.BeginHorizontal();
            GUI.color = Color.white;
            GUILayout.Label("新配置id：", EditorStyles.largeLabel, GUILayout.Width(60));
            GUI.color = Color.yellow;
            s = EditorGUILayout.TextField("", s, "LargeTextField", GUILayout.Width(s.Length * 8 + 100));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUI.color = Color.white;
            if (GUILayout.Button("确认创建", "LargeButton"))
            {
                bool flag = true;
                for (int i = 0; i < idArr.Length; i++)
                {
                    if (idArr[i] == s)
                    {
                        flag = false;
                        break;
                    }
                }
                if (!flag)
                {
                    EditorUtility.DisplayDialog("创建失败", "该id已存在！", "返回");
                }
                else
                {
                    long n;
                    if (long.TryParse(s, out n))
                    {
                        string[] newStrs = new string[ceui.currentCfgFileInfo[0].Count];
                        newStrs[0] = s;
                        if (targetId == null)
                        {
                            for (int i = 1; i < newStrs.Length; i++)
                            {
                                newStrs[i] = "";
                            }
                        }
                        else
                        {
                            for (int i = 1; i < newStrs.Length; i++)
                            {
                                newStrs[i] = ceui.currentCfgFileInfo[int.Parse(targetId)][i];
                            }
                        }
                        ceui.currentCfgFileInfo.Add(newStrs.ToList());
                        ceui.StateInit();
                        int maxPage = (ceui.currentCfgFileInfo.Count - 3) % 10 == 0
                            ? (ceui.currentCfgFileInfo.Count - 3) / 10
                            : (ceui.currentCfgFileInfo.Count - 3) / 10 + 1;
                        ceui.Paging(10, maxPage);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("创建失败", "id格式不正确！", "返回");
                    }
                }
                ceui = null;
                idArr = null;
                targetId = null;
                Close();
            }
            if (GUILayout.Button("返回", "LargeButton"))
            {
                ceui = null;
                idArr = null;
                targetId = null;
                Close();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }

}

