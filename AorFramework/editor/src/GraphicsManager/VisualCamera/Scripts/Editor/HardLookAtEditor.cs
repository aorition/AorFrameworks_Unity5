using UnityEditor;
using UnityEngine;

namespace Framework.Graphic.Editor
{
    [CustomEditor(typeof(HardLookAt))]
//    public sealed class HardLookAtEditor : BaseEditor<HardLookAt>
    public sealed class HardLookAtEditor : UnityEditor.Editor
    {

        private HardLookAt Target;

        public override void OnInspectorGUI()
        {

            Target = target as HardLookAt;

            //BeginInspector();
            if (Target.LookAtTarget == null)
                EditorGUILayout.HelpBox(
                    "Hard Look At requires a Look At Target.  Change Body to Do Nothing if you don't want a Look At Target.",
                    MessageType.Warning);
            EditorGUI.BeginChangeCheck();
            GUI.enabled = false;
            EditorGUILayout.LabelField(" ", "Hard Look At has no settings", EditorStyles.miniLabel);
            GUI.enabled = true;
            //DrawRemainingPropertiesInInspector();
        }
    }
}
