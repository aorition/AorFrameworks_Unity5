using UnityEngine;

namespace Framework.Graphic
{
    /// <summary>
    /// This is a CinemachineComponent in the Aim section of the component pipeline.
    /// Its job is to place the camera on the Follow Target.
    /// </summary>
    [AddComponentMenu("")] // Don't display in add component menu
    public class HardLockToTarget : VisualCameraComponentBase, IVisualCameraBodyComponent
    {
        /// <summary>True if component is enabled and has a LookAt defined</summary>
        public override bool IsValid { get { return this != null && enabled && FollowTarget != null; } }

        /// <summary>Applies the composer rules and orients the camera accordingly</summary>
        public override void MutateCameraState(float deltaTime)
        {
            if (IsValid)
                extension.RawPosition = FollowTarget.position;
        }
    }
}

