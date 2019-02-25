using System.Collections;
using System.Collections.Generic;
using Framework.Extends;
using UnityEngine;

namespace Modules.SpringBone
{

    public class SpringBone : MonoBehaviour
    {
        
        public Vector3 springEnd = Vector3.left;

        private float springLength
        {
            get
            {
                return springEnd.magnitude;
            }
        }

        public bool LockRotaX;
        public bool LockRotaY;
        public bool LockRotaZ;

        //
        private SpringBone parentBone;

        public bool useSpecifiedRotation;

        public Vector3 customRotation;

        public float stiffness = 1.0f;

        public float bounciness = 40.0f;

        [Range(0.0f, 0.9f)]
        public float dampness = 0.1f;

        // Use this for initialization
        void Start()
        {
            currentTipPos = transform.TransformPoint(springEnd);
            if (!parentBone && transform.parent)
            {
                parentBone = transform.FindComponentInParent<SpringBone>();
            }

            _lockEulerCache = transform.localEulerAngles;
        }

        private bool updated = false;
        // Update is called once per frame
        void Update()
        {
            updated = false;
        }

        private void LateUpdate()
        {
            UpdateSpring();

            //Debug
            if (isDebug)
                _draw_debug();
        }

        //hack by Aor
        private bool isDebug;
        private void _draw_debug()
        {
            Debug.DrawLine(currentTipPos - Vector3.right * 1, currentTipPos + Vector3.right * 1);
            Debug.DrawLine(currentTipPos - Vector3.up * 1, currentTipPos + Vector3.up * 1);
            Debug.DrawLine(currentTipPos - Vector3.forward * 1, currentTipPos + Vector3.forward * 1);
        }

        private Vector3 currentTipPos;
        private Vector3 velocity;   //current velocity of tip.

        //hack by Aor
        private Vector3 _lockEulerCache;

        private void UpdateSpring()
        {
            if (updated)
                return;
            if (parentBone != null)
                parentBone.UpdateSpring();     //make sure update is from parent to child. ( or things will just mess up  :D)

            updated = true;

            var lastFrameTip = currentTipPos;

            if (useSpecifiedRotation)
                transform.localRotation = Quaternion.Euler(customRotation);

            currentTipPos = transform.TransformPoint(springEnd);

            var force = bounciness * (currentTipPos - lastFrameTip);  //spring force.

            force += stiffness * (currentTipPos - transform.position).normalized;  //stiffness

            force -= dampness * velocity;               //damp force. 

            velocity += force * Time.deltaTime;       //v = v0 + at. we don't need integration here, you won't notice any "wrong".

            currentTipPos = lastFrameTip + velocity * Time.deltaTime; //s = s0 + vt

            currentTipPos = springLength * (currentTipPos - transform.position).normalized + transform.position;    //clamp length.

            transform.rotation = Quaternion.FromToRotation(transform.TransformDirection(springEnd), (currentTipPos - transform.position).normalized) * transform.rotation;

            //hack by Aor
            if (LockRotaX || LockRotaY || LockRotaZ)
            {
                Vector3 last = transform.localEulerAngles;
                transform.localEulerAngles = new Vector3(LockRotaX ? _lockEulerCache.x : last.x, LockRotaY ? _lockEulerCache.y : last.y, LockRotaZ ? _lockEulerCache.z : last.z);
            }

        }
    }

}


