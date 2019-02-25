using System;
using System.Collections.Generic;
using Framework;
using Framework.Extends;
using UnityEngine;

namespace Modules.SpringBone 
{
    public class SpringBoneEditon : MonoBehaviour, IEditorOnlyScript
    {

        private void Awake()
        {
            if (Application.isPlaying && !Application.isEditor)
            {
                GameObject.Destroy(this);
            }
        }

        public List<SpringBone> SpringBones = new List<SpringBone>();
        public void FillSpringBones()
        {
            SpringBones = transform.FindComponentListInChildren<SpringBone>(true);
        }

        public bool LockRotaX;
        public bool LockRotaY;
        public bool LockRotaZ;

        public Vector3 springEnd;

        public bool useSpecifiedRotation;

        public Vector3 customRotation;

        public bool useMutiEditForces;

        public float stiffness = 1.0f;
        public AnimationCurve stiffnessCureve = new AnimationCurve(new Keyframe[]{new Keyframe(0,1), new Keyframe(1, 1)});

        public float bounciness = 40.0f;
        public AnimationCurve bouncinessCureve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) });

        [Range(0.0f, 0.9f)]
        public float dampness = 0.1f;
        public AnimationCurve dampnessCureve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) });

    }
}
