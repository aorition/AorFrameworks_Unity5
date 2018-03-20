using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Framework.Graphic.Editor
{
    [CustomEditor(typeof(Transposer))]
    internal sealed class TransposerEditor : BaseEditor<Transposer>
    {
        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
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
                    break;
            }
            return excluded;
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            if (Target.FollowTarget == null)
                EditorGUILayout.HelpBox(
                    "Transposer requires a Follow Target.  Change Body to Do Nothing if you don't want a Follow target.",
                    MessageType.Warning);
            DrawRemainingPropertiesInInspector();
        }

//        [DrawGizmo(GizmoType.Active | GizmoType.Selected, typeof(Transposer))]
//        static void DrawTransposerGizmos(Transposer target, GizmoType selectionType)
//        {
//            if (target.IsValid)
//            {
//                Color originalGizmoColour = Gizmos.color;
//                //Todo ???
////                Gizmos.color = CinemachineCore.Instance.IsLive(target.VirtualCamera)
////                    ? CinemachineSettings.CinemachineCoreSettings.ActiveGizmoColour
////                    : CinemachineSettings.CinemachineCoreSettings.InactiveGizmoColour;
//
//                Vector3 up = Vector3.up;
//
//                //Todo ???
////                CinemachineBrain brain = CinemachineCore.Instance.FindPotentialTargetBrain(target.VirtualCamera);
////                if (brain != null)
////                    up = brain.DefaultWorldUp;
//
//                Vector3 targetPos = target.FollowTarget.position;
//                Vector3 desiredPos = target.GeTargetCameraPosition(up);
//                Gizmos.DrawLine(targetPos, desiredPos);
//                Gizmos.DrawWireSphere(desiredPos, HandleUtility.GetHandleSize(desiredPos) / 20);
//                Gizmos.color = originalGizmoColour;
//            }
//        }
    }
}
