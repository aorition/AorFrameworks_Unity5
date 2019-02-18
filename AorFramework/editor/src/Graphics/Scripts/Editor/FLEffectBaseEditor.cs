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

        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();

            GUILayout.Space(10);

            int level = (int)_target.GetNonPublicField("m_RenderLevel");
            int nLevel = EditorGUILayout.IntField("Render Level", level);
            if (!nLevel.Equals(level)) _target.Level = nLevel;
            
        }

    }
}
