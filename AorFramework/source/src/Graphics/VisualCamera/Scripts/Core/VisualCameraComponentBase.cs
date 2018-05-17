using System;
using System.Collections.Generic;
using AorBaseUtility;
using UnityEngine;

namespace Framework.Graphic
{

    public abstract class VisualCameraComponentBase : MonoBehaviour
    {

        protected const float Epsilon = FloatExtends.Epsilon;

        private VisualCameraExtension _extension;
        public VisualCameraExtension extension
        {
            get
            {
                if (!_extension)
                {
                    _extension = GetComponent<VisualCameraExtension>();
                }
                return _extension;
            }
        }

        public Transform FollowTarget
        {
            get
            {
                if (extension)
                {
                    return extension.FollowTarget;
                }
                return null;
            }
        }

        public Transform LookAtTarget
        {
            get
            {
                if (extension)
                {
                    return extension.LookAtTarget;
                }
                return null;
            }
        }

        /// <summary>Returns true if this object is enabled and set up to produce results.</summary>
        public abstract bool IsValid { get; }

        /// <summary>Mutates the camera state.  This state will later be applied to the camera.</summary>
        /// <param name="vcam">Input VisualCamera that must be mutated</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than 0)</param>
        public abstract void MutateCameraState(float deltaTime);

    }
}

