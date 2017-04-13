using System;
using System.Collections.Generic;
using AorFrameworks.Tools;
using UnityEditor;
using UnityEngine;

namespace AorFramework.editor.tools
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TransformRecorder))]
    public class TransformRecorderEditor : Editor
    {

        public static void ClearDataCaches()
        {
            GameObject[] selects = Selection.gameObjects;

            if (selects != null && selects.Length > 0)
            {

                int i, len = selects.Length;
                for (i = 0; i < len; i++)
                {

                    TransformRecorder tr = selects[i].GetComponent<TransformRecorder>();
                    if (tr)
                    {
                        tr.ClearCacheFiles();
                    }

                }

            }
        }

        public static void LoadDataFromFile()
        {
            GameObject[] selects = Selection.gameObjects;

            if (selects != null && selects.Length > 0)
            {

                int i, len = selects.Length;
                for (i = 0; i < len; i++)
                {

                    TransformRecorder tr = selects[i].GetComponent<TransformRecorder>();
                    if (tr)
                    {
                        tr.LoadFromFile();
                    }

                }

            }

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseFixedUpdate"));
            serializedObject.ApplyModifiedProperties();

            AorGUILayout.Vertical("box", () =>
            {
                GUI.color = Color.red;
                if (GUILayout.Button("ClearDataCaches"))
                {
                    if (EditorUtility.DisplayDialog("警告", "确定清楚已生成的位置缓存文件？", "确定", "取消"))
                    {
                        ClearDataCaches();
                    }
                }
                GUI.color = Color.white;
            });

            if (GUILayout.Button("LoadDataFromFile"))
            {
                LoadDataFromFile();
            }

        }
    }
}
