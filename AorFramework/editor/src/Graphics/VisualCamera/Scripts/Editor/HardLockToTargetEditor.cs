using UnityEditor;
using UnityEngine;

namespace Framework.Graphic.Editor
{
    [CustomEditor(typeof(HardLockToTarget))]
    public sealed class HardLockToTargetEditor : BaseEditor<HardLockToTarget>
    {
        public override void OnInspectorGUI()
        {
            BeginInspector();
            if (Target.FollowTarget == null)
                EditorGUILayout.HelpBox(
                    "Hard Lock requires a Follow Target.  Change Body to Do Nothing if you don't want a Follow target.",
                    MessageType.Warning);
            EditorGUI.BeginChangeCheck();
            GUI.enabled = false;
            EditorGUILayout.LabelField(" ", "Hard Lock has no settings", EditorStyles.miniLabel);
            GUI.enabled = true;
            DrawRemainingPropertiesInInspector();
        }
    }
}
