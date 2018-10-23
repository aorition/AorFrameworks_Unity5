using System;
using System.Collections.Generic;
using System.Reflection;
using Framework.Utility;
using UnityEditor;
using UnityEngine;


namespace Framework.Editor.Utility
{

    [CustomEditor(typeof(SimpleTestMonoBehaviour),true)]
    public class SimpleTestEditor : UnityEditor.Editor
    {

        private SimpleTestMonoBehaviour _target;

        private readonly List<MethodInfo> m_MethodInfoList = new List<MethodInfo>();
        private string[] m_MethodNameList;

        private void _vaildmTestMethods(Type type)
        {

            MethodInfo[] infos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);
            List<string> nameList = new List<string>();
            foreach (MethodInfo methodInfo in infos)
            {
                if (methodInfo.Name.ToLower().StartsWith("test") 
                    && methodInfo.GetGenericArguments().Length == 0
                    && methodInfo.ReturnType == typeof(void)
                    )
                {
                    m_MethodInfoList.Add(methodInfo);
                    nameList.Add(methodInfo.Name);
                }
            }
            m_MethodNameList = nameList.ToArray();
        }

        private void Awake()
        {
            _target = target as SimpleTestMonoBehaviour;
            _vaildmTestMethods(target.GetType());
        }

        private int m_MethodInfoIndex = 0;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (m_MethodInfoList.Count > 0)
            {
                GUILayout.Space(16);

                GUILayout.BeginVertical("box");
                GUILayout.Space(5);

                GUILayout.Label("Test Method :: ");

                GUILayout.Space(3);
                m_MethodInfoIndex = EditorGUILayout.Popup(m_MethodInfoIndex, m_MethodNameList);
                GUILayout.Space(3);

                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button("Do Test",GUILayout.Height(40)))
                {
                    m_MethodInfoList[m_MethodInfoIndex].Invoke(_target, null);
                }
                GUI.backgroundColor = Color.white;
                GUILayout.Space(5);
                GUILayout.EndVertical();

            }

        }
    }
}
