#pragma warning disable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AorBaseUtility.Extends;

namespace Framework.Graphic.editor
{
    [CustomEditor(typeof(FLEffectBase),true)]
    public class FLEffectBaseEditor : UnityEditor.Editor
    {

        private FLEffectBase _target;
        private void Awake()
        {
            _target = target as FLEffectBase;
        }

        private bool m_levelChange = false;

        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();

            GUILayout.Space(10);

            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);

                int level = (int)_target.GetNonPublicField("m_RenderLevel");
                if (m_levelChange)
                {
                    GUILayout.BeginHorizontal();
                    {

                        GUI.color = Color.white;

                        int nLevel = EditorGUILayout.IntField("Render Level", level);
                        if (!nLevel.Equals(level)) _target.Level = nLevel;
                        if (GUILayout.Button("Done", GUILayout.Width(80)))
                        {
                            m_levelChange = false;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {

                    GUILayout.BeginHorizontal();
                    {

                        GUI.color = Color.gray;

                        GUILayout.Label("Render Level");
                        GUILayout.Label(level.ToString());
                        if (GUILayout.Button("change?", GUILayout.Width(80)))
                        {
                            m_levelChange = true;
                        }
                    }
                    GUILayout.EndHorizontal();

                }

                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
 
        }

    }
}
