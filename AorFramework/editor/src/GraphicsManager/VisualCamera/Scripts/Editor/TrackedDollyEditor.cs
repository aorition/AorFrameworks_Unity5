using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Graphic.Editor
{
    [CustomEditor(typeof(TrackedDolly))]
    internal sealed class TrackedDollyEditor : BaseEditor<TrackedDolly>
    {
        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            switch (Target.m_CameraUp)
            {
                default:
                    break;
                case TrackedDolly.CameraUpMode.PathNoRoll:
                case TrackedDolly.CameraUpMode.FollowTargetNoRoll:
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    break;
                case TrackedDolly.CameraUpMode.Default:
                    excluded.Add(FieldPath(x => x.m_PitchDamping));
                    excluded.Add(FieldPath(x => x.m_YawDamping));
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    break;
            }
            return excluded;
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            if (Target.m_Path == null)
                EditorGUILayout.HelpBox("A Path is required", MessageType.Warning);
            if (Target.m_AutoDolly.m_Enabled && Target.FollowTarget == null)
                EditorGUILayout.HelpBox("AutoDolly requires a Follow Target", MessageType.Warning);
            DrawRemainingPropertiesInInspector();
        }

        [DrawGizmo(GizmoType.Active | GizmoType.InSelectionHierarchy, typeof(TrackedDolly))]
        private static void DrawTrackeDollyGizmos(TrackedDolly target, GizmoType selectionType)
        {
            if (target.IsValid)
            {
                GEVPathBase path = target.m_Path;
                if (path != null)
                {
                    GEVPathEditor.DrawPathGizmo(path, path.m_Appearance.pathColor);
                    Vector3 pos = path.EvaluatePositionAtUnit(target.m_PathPosition, target.m_PositionUnits);
                    Color oldColor = Gizmos.color;
                    
                    //Todo ???
//                    Gizmos.color = CinemachineCore.Instance.IsLive(target.VirtualCamera)
//                        ? CinemachineSettings.CinemachineCoreSettings.ActiveGizmoColour
//                        : CinemachineSettings.CinemachineCoreSettings.InactiveGizmoColour;
                    Gizmos.DrawLine(pos, target.transform.position);
                    Gizmos.color = oldColor;
                }
            }
        }
    }
}
