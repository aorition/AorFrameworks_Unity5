using System;
using System.Collections.Generic;
using Framework.Extends;
using UnityEngine;

namespace Framework.Graphic
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(VisualCamera))]
    public class VisualCameraExtension : MonoBehaviour
    {

        public static void ClearAllBodyComponets(GameObject gameObject)
        {
            List<IVisualCameraBodyComponent> bodyExts = gameObject.GetInterfacesList<IVisualCameraBodyComponent>();
            if (bodyExts != null && bodyExts.Count > 0)
            {
                for (int i = 0; i < bodyExts.Count; i++)
                {
                    Component b = bodyExts[i] as Component;
                    if (Application.isPlaying)
                    {
                        GameObject.Destroy(b);
                    }
                    else
                    {
                        GameObject.DestroyImmediate(b);
                    }
                }
                bodyExts.Clear();
            }
        }
        public static void ClearAllAimComponets(GameObject gameObject)
        {
            List<IVisualCameraAimComponent> aimExts = gameObject.GetInterfacesList<IVisualCameraAimComponent>();
            if (aimExts != null && aimExts.Count > 0)
            {
                for (int i = 0; i < aimExts.Count; i++)
                {
                    Component b = aimExts[i] as Component;
                    if (Application.isPlaying)
                    {
                        GameObject.Destroy(b);
                    }
                    else
                    {
                        GameObject.DestroyImmediate(b);
                    }
                }
                aimExts.Clear();
            }
        }
        public static void ClearAllNoiseComponets(GameObject gameObject)
        {
            List<IVisualCameraNoiseComponent> noiseExts = gameObject.GetInterfacesList<IVisualCameraNoiseComponent>();
            if (noiseExts != null && noiseExts.Count > 0)
            {
                for (int i = 0; i < noiseExts.Count; i++)
                {
                    Component b = noiseExts[i] as Component;
                    if (Application.isPlaying)
                    {
                        GameObject.Destroy(b);
                    }
                    else
                    {
                        GameObject.DestroyImmediate(b);
                    }
                }
                noiseExts.Clear();
            }
        }

        public static void ClearAllSubComponets(GameObject gameObject)
        {
            ClearAllBodyComponets(gameObject);
            ClearAllAimComponets(gameObject);
            ClearAllNoiseComponets(gameObject);
        }

        //----------------------------------------------------------------------
        [SerializeField]
        private Transform m_LookAt;
        public Transform LookAtTarget
        {
            get { return m_LookAt; }
            set { m_LookAt = value; }
        }

        [SerializeField]
        private Transform m_Follow;
        public Transform FollowTarget
        {
            get { return m_Follow; }
            set { m_Follow = value; }
        }

        //----------------------------------------

        private VisualCamera _target;
        public VisualCamera Target
        {
            get
            {
                if (!_target)
                {
                    _target = GetComponent<VisualCamera>();
                }
                return _target;
            }
        }

        /// <summary>
        /// Which way is up.  World space unit vector.
        /// </summary>
        public Vector3 ReferenceUp { get; set; }

        /// <summary>
        /// The world space focus point of the camera.  What the camera wants to look at.
        /// There is a special constant define to represent "nothing".  Be careful to 
        /// check for that (or check the HasLookAt property).
        /// </summary>
        public Vector3 ReferenceLookAt { get; set; }

        /// <summary>
        /// This constant represents "no point in space" or "no direction".
        /// </summary>
        public static Vector3 kNoPoint = new Vector3(float.NaN, float.NaN, float.NaN);

        /// <summary>
        /// Raw (un-corrected) world space position of this camera
        /// </summary>
        public Vector3 RawPosition { get; set; }

        /// <summary>
        /// Raw (un-corrected) world space orientation of this camera
        /// </summary>
        public Quaternion RawOrientation { get; set; }

        /// <summary>This is a way for the Body component to bypass aim damping,
        /// useful for when the body need to rotate its point of view, but does not
        /// want interference from the aim damping</summary>
        internal Vector3 PositionDampingBypass { get; set; }

        /// <summary>
        /// Position correction.  This will be added to the raw position.
        /// This value doesn't get fed back into the system when calculating the next frame.
        /// Can be noise, or smoothing, or both, or something else.
        /// </summary>
        public Vector3 PositionCorrection { get; set; }

        /// <summary>
        /// Orientation correction.  This will be added to the raw orientation.
        /// This value doesn't get fed back into the system when calculating the next frame.
        /// Can be noise, or smoothing, or both, or something else.
        /// </summary>
        public Quaternion OrientationCorrection { get; set; }

        /// <summary>
        /// Position with correction applied.
        /// </summary>
        public Vector3 CorrectedPosition { get { return RawPosition + PositionCorrection; } }

        /// <summary>
        /// Orientation with correction applied.
        /// </summary>
        public Quaternion CorrectedOrientation { get { return RawOrientation * OrientationCorrection; } }

        /// <summary>
        /// Position with correction applied.  This is what the final camera gets.
        /// </summary>
        public Vector3 FinalPosition { get { return RawPosition + PositionCorrection; } }

        /// <summary>
        /// Orientation with correction and dutch applied.  This is what the final camera gets.
        /// </summary>
        public Quaternion FinalOrientation
        {
            get
            {
                //                if (Mathf.Abs(Lens.Dutch) > FloatExtends.Epsilon)
                //                    return CorrectedOrientation * Quaternion.AngleAxis(Lens.Dutch, Vector3.forward);
                return CorrectedOrientation;
            }
        }

        public Camera Lens
        {
            get
            {
                return _target.CrrentCamera;
            }
        }

        /// <summary>
        /// Returns true if this state has a valid ReferenceLookAt value.
        /// </summary>
        public bool HasLookAt { get { return ReferenceLookAt == ReferenceLookAt; } } // will be false if NaN

        private IVisualCameraBodyComponent _bodyExtMethod;
        //        public IVisualCameraBodyComponent bodyExtMethod
        //        {
        //            get
        //            {
        //                if (_bodyExtMethod == null)
        //                {
        //                    _bodyExtMethod = gameObject.GetInterface<IVisualCameraBodyComponent>();
        //                }
        //                return _bodyExtMethod;
        //            }
        //        }
        private IVisualCameraAimComponent _aimExtMethod;
        //        public IVisualCameraAimComponent aimExtMethod
        //        {
        //            get
        //            {
        //                if (_aimExtMethod == null)
        //                {
        //                    _aimExtMethod = gameObject.GetInterface<IVisualCameraAimComponent>();
        //                }
        //                return _aimExtMethod;
        //            }
        //        }
        private IVisualCameraNoiseComponent _noiseExtMethod;
        //        public IVisualCameraNoiseComponent noiseExtMethod
        //        {
        //            get
        //            {
        //                if (_noiseExtMethod == null)
        //                {
        //                    _noiseExtMethod = gameObject.GetInterface<IVisualCameraNoiseComponent>();
        //                }
        //                return _noiseExtMethod;
        //            }
        //        }

        private void Awake()
        {
            _target = GetComponent<VisualCamera>();

            _bodyExtMethod = gameObject.GetInterface<IVisualCameraBodyComponent>();
            _aimExtMethod = gameObject.GetInterface<IVisualCameraAimComponent>();
            _noiseExtMethod = gameObject.GetInterface<IVisualCameraNoiseComponent>();
        }

        private void OnDestroy()
        {
            ClearAllSubComponets(gameObject);
        }

        public void UpdateExtension(float deltaTime)
        {

            if (!_target) return;

            _PreUpdateExtension();

            if (_bodyExtMethod != null && _bodyExtMethod.IsValid) _bodyExtMethod.MutateCameraState(deltaTime);
            if (_aimExtMethod != null && _aimExtMethod.IsValid) _aimExtMethod.MutateCameraState(deltaTime);
            if (_noiseExtMethod != null && _noiseExtMethod.IsValid) _noiseExtMethod.MutateCameraState(deltaTime);

            _AfterUpdateExtension();

        }

        private void _PreUpdateExtension()
        {
            ReferenceLookAt = m_LookAt ? m_LookAt.position : Vector3.zero;

            OrientationCorrection = Quaternion.identity;
            PositionCorrection = Vector3.zero;

            RawPosition = _target.transform.position;
            RawOrientation = _target.transform.rotation;
        }

        private void _AfterUpdateExtension()
        {
            _target.transform.rotation = FinalOrientation;
            _target.transform.position = FinalPosition;
        }

    }
}
