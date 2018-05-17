using UnityEngine;

namespace Framework.Graphic
{
    /// <summary>
    /// This is a CinemachineComponent in the Aim section of the component pipeline.
    /// Its job is to aim the camera hard at the LookAt target.
    /// </summary>
    [AddComponentMenu("")] // Don't display in add component menu
    public class SameAsFollowObject : VisualCameraComponentBase, IVisualCameraAimComponent
    {
        /// <summary>True if component is enabled and has a Follow target defined</summary>
        public override bool IsValid { get { return enabled && FollowTarget != null; } }
        
        /// <summary>Applies the composer rules and orients the camera accordingly</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">Used for calculating damping.  If less than
        /// zero, then target will snap to the center of the dead zone.</param>
        public override void MutateCameraState(float deltaTime)
        {
            if (IsValid)
                extension.RawOrientation = FollowTarget.transform.rotation;
        }
    }
}

