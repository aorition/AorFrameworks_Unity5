using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Framework.Graphic.Editor
{
    [CustomEditor(typeof(OrbitalTransposer))]
    internal class CinemachineOrbitalTransposerEditor : BaseEditor<OrbitalTransposer>
    {
        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            if (Target.m_HeadingIsSlave)
            {
                excluded.Add(FieldPath(x => x.m_FollowOffset));
                excluded.Add(FieldPath(x => x.m_BindingMode));
                excluded.Add(FieldPath(x => x.m_Heading));
                excluded.Add(FieldPath(x => x.m_XAxis));
                excluded.Add(FieldPath(x => x.m_RecenterToTargetHeading));
            }
            switch (Target.m_BindingMode)
            {
                default:
                case Transposer.BindingMode.LockToTarget:
                    break;
                case Transposer.BindingMode.LockToTargetNoRoll:
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    break;
                case Transposer.BindingMode.LockToTargetWithWorldUp:
                    excluded.Add(FieldPath(x => x.m_PitchDamping));
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    break;
                case Transposer.BindingMode.LockToTargetOnAssign:
                case Transposer.BindingMode.WorldSpace:
                    excluded.Add(FieldPath(x => x.m_PitchDamping));
                    excluded.Add(FieldPath(x => x.m_YawDamping));
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    break;
                case Transposer.BindingMode.SimpleFollowWithWorldUp:
                    excluded.Add(FieldPath(x => x.m_XDamping));
                    excluded.Add(FieldPath(x => x.m_PitchDamping));
                    excluded.Add(FieldPath(x => x.m_YawDamping));
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    excluded.Add(FieldPath(x => x.m_Heading));
                    excluded.Add(FieldPath(x => x.m_RecenterToTargetHeading));
                    break;
            }
            return excluded;
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            if (Target.FollowTarget == null)
                EditorGUILayout.HelpBox(
                    "Orbital Transposer requires a Follow target.", 
                    MessageType.Warning);
            DrawRemainingPropertiesInInspector();
        }

        [DrawGizmo(GizmoType.Active | GizmoType.Selected, typeof(OrbitalTransposer))]
        static void DrawTransposerGizmos(OrbitalTransposer target, GizmoType selectionType)
        {
            if (target.IsValid)
            {
                Color originalGizmoColour = Gizmos.color;

                //Todo ???
//                Gizmos.color = CinemachineCore.Instance.IsLive(target.VirtualCamera)
//                    ? CinemachineSettings.CinemachineCoreSettings.ActiveGizmoColour
//                    : CinemachineSettings.CinemachineCoreSettings.InactiveGizmoColour;

                Vector3 up = Vector3.up;

                //Todo ???
//                CinemachineBrain brain = CinemachineCore.Instance.FindPotentialTargetBrain(target.VirtualCamera);
//                if (brain != null)
//                    up = brain.DefaultWorldUp;
                Vector3 pos = target.FollowTarget.position;

                Quaternion orient = target.GetReferenceOrientation(up);
                up = orient * Vector3.up;
                DrawCircleAtPointWithRadius
                    (pos + up * target.m_FollowOffset.y, orient, target.m_FollowOffset.z);

                Gizmos.color = originalGizmoColour;
            }
        }

        internal static void DrawCircleAtPointWithRadius(Vector3 point, Quaternion orient, float radius)
        {
            Matrix4x4 prevMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(point, orient, radius * Vector3.one);

            const int kNumPoints = 25;
            Vector3 currPoint = Vector3.forward;
            Quaternion rot = Quaternion.AngleAxis(360f / (float)kNumPoints, Vector3.up);
            for (int i = 0; i < kNumPoints + 1; ++i)
            {
                Vector3 nextPoint = rot * currPoint;
                Gizmos.DrawLine(currPoint, nextPoint);
                currPoint = nextPoint;
            }
            Gizmos.matrix = prevMatrix;
        }
    }
}
