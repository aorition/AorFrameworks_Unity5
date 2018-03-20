using UnityEngine;

namespace Framework.Graphic
{
    /// <summary>
    /// This is a CinemachineComponent in the Aim section of the component pipeline.
    /// Its job is to aim the camera hard at the LookAt target.
    /// </summary>
    [AddComponentMenu("")] // Don't display in add component menu
    public class HardLookAt : VisualCameraComponentBase, IVisualCameraAimComponent
    {
        /// <summary>True if component is enabled and has a LookAt defined</summary>
        public override bool IsValid { get { return this != null && enabled && LookAtTarget != null; } }

        /// <summary>Applies the composer rules and orients the camera accordingly</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">Used for calculating damping.  If less than
        /// zero, then target will snap to the center of the dead zone.</param>
        public override void MutateCameraState(float deltaTime)
        {
            if (IsValid && extension.HasLookAt)
            {
                Vector3 dir = (extension.ReferenceLookAt - extension.CorrectedPosition);
                if (dir.magnitude > Epsilon)
                {
                    if (Vector3.Cross(dir.normalized, extension.ReferenceUp).magnitude < Epsilon)
                        extension.RawOrientation = Quaternion.FromToRotation(Vector3.forward, dir);
                    else
                        extension.RawOrientation = Quaternion.LookRotation(dir, extension.ReferenceUp);
                }
            }
        }
    }
}

