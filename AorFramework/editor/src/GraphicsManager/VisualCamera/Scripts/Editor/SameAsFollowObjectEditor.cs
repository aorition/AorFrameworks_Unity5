using UnityEditor;
using UnityEngine;

namespace Framework.Graphic.Editor
{
    [CustomEditor(typeof(SameAsFollowObject))]
    public sealed class SameAsFollowObjectEditor : BaseEditor<SameAsFollowObject>
    {
        public override void OnInspectorGUI()
        {
            BeginInspector();
            if (Target.FollowTarget == null)
                EditorGUILayout.HelpBox(
                    "Same As Follow Object requires a Follow Target.  Change Body to Do Nothing if you don't want a Follow target.",
                    MessageType.Warning);
            EditorGUI.BeginChangeCheck();
            GUI.enabled = false;
            EditorGUILayout.LabelField(" ", "Same As Follow Object has no settings", EditorStyles.miniLabel);
            GUI.enabled = true;
            DrawRemainingPropertiesInInspector();
        }
    }
}
