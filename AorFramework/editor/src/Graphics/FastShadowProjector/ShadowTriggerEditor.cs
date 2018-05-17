using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Framework.Graphic.FastShadowProjector.Editor
{
    [CustomEditor(typeof(ShadowTrigger))]
    public class ShadowTriggerEditor : UnityEditor.Editor
    {


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ShadowTrigger ShadowTrigger = (ShadowTrigger)target;

            ShadowTrigger.DetectShadow = EditorGUILayout.Toggle("Detect shadow", ShadowTrigger.DetectShadow);
            ShadowTrigger.DetectLight = EditorGUILayout.Toggle("Detect light", ShadowTrigger.DetectLight);

            serializedObject.ApplyModifiedProperties();
        }
    }
}



