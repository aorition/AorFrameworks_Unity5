using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor.Utility
{
    /// <summary>
    /// 一个窗口版的TimeManager ...
    /// 
    /// @ 众亲不觉得原生TimeManager占用Inspector的行为很烦么??
    /// 
    /// </summary>
    public class TimeManagerWindow : EditorWindow
    {

        private static GUIStyle m_titleGUIStyle;
        private static GUIStyle TitleGUIStyle
        {
            get
            {
                if (m_titleGUIStyle == null)
                {
                    m_titleGUIStyle = new GUIStyle();
                    m_titleGUIStyle.fontSize = 18;
                    m_titleGUIStyle.fontStyle = FontStyle.Bold;
                    m_titleGUIStyle.alignment = TextAnchor.MiddleCenter;
                    m_titleGUIStyle.normal.textColor = Color.white;
                }
                return m_titleGUIStyle;
            }
        }

        //----------------------------------------------

        private static TimeManagerWindow _instance;

        [MenuItem("FrameworkTools/辅助工具/TimeManager")]
        public static TimeManagerWindow Init()
        {
            _instance = EditorWindow.GetWindow<TimeManagerWindow>();
            _instance.minSize = new Vector2(380, 265);
            return _instance;
        }

        private const float _default_FixedTimestep = 0.02f;
        private const float _default_MaximumAllowedTimestep = 0.3333333f;
        private const float _default_TimeScale = 1.0f;
        private const float _default_MaximumParticleTimestep = 0.03f;

        private Color _defalutColor;
        private float _FixedTimestep = 0.02f;
        private float _MaximumAllowedTimestep = 0.3333333f;
        private float _TimeScale = 1.0f;
        private float _MaximumParticleTimestep = 0.03f;

        private void OnGUI()
        {

            _FixedTimestep = Time.fixedDeltaTime;
            _MaximumAllowedTimestep = Time.maximumDeltaTime;
            _TimeScale = Time.timeScale;
            _MaximumParticleTimestep = Time.maximumParticleDeltaTime;

            // EditorPlusMethods.Draw_DebugWindowSizeUI();

            GUILayout.Space(18);

            GUILayout.Label("TimeManager", TitleGUIStyle);

            GUILayout.Space(12);

            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Fixed Timestep");
            GUILayout.Space(5);
            _FixedTimestep = EditorGUILayout.Slider(_FixedTimestep, 0.0001f, 1f);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Maximum Allowed Timestep");
            GUILayout.Space(5);
            _MaximumAllowedTimestep = EditorGUILayout.Slider(_MaximumAllowedTimestep, 0.0001f, 1f);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Time Scale");
            GUILayout.Space(5);
            _TimeScale = EditorGUILayout.Slider(_TimeScale, 0f, 10f);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Maximum Particle Timestep");
            GUILayout.Space(5);
            _MaximumParticleTimestep = EditorGUILayout.Slider(_MaximumParticleTimestep, 0.02f, 1f);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Space(12);

            GUILayout.BeginVertical("box");
            GUILayout.Space(5);

            _defalutColor = GUI.color;

            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("0X",GUILayout.Height(26)))
            {
                _TimeScale = 0;
            }
            if (GUILayout.Button("1X", GUILayout.Height(26)))
            {
                _TimeScale = 1;
            }
            if (GUILayout.Button("2X", GUILayout.Height(26)))
            {
                _TimeScale = 2;
            }
            if (GUILayout.Button("5X", GUILayout.Height(26)))
            {
                _TimeScale = 5;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUI.color = Color.grey;
            if (GUILayout.Button("", GUILayout.Height(26)))
            {

            }
            if (GUILayout.Button("", GUILayout.Height(26)))
            {

            }
            if (GUILayout.Button("", GUILayout.Height(26)))
            {

            }
            GUI.color = _defalutColor;
            if (GUILayout.Button("ResetDefault", GUILayout.Height(26)))
            {
                _FixedTimestep = _default_FixedTimestep;
                _MaximumAllowedTimestep = _default_MaximumAllowedTimestep;
                _TimeScale = _default_TimeScale;
                _MaximumParticleTimestep = _default_MaximumParticleTimestep;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.EndVertical();

            GUILayout.Space(18);

            Time.fixedDeltaTime = _FixedTimestep;
            Time.maximumDeltaTime = _MaximumAllowedTimestep;
            Time.timeScale = _TimeScale;
            Time.maximumParticleDeltaTime = _MaximumParticleTimestep;

        }

    }
}
