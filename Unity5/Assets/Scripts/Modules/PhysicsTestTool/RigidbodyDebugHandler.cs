using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utility
{
    public class RigidbodyDebugHandler : MonoBehaviour
    {
        public Vector3 direction;
        public float force = 100;
        public Vector3 velocity;

        private void Awake()
        {
            if (!Application.isEditor)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(this);
                }
                else
                {
                    GameObject.DestroyImmediate(this);
                }
            }
        }

    }
}


