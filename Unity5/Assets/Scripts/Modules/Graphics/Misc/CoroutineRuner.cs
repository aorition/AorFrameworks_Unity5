using System;
using System.Collections;
using UnityEngine;

namespace YoukiaUnity.Framework.Misc
{
    public class CoroutineRuner : MonoBehaviour
    {

        private bool _isStarted;

        void Start() {
            if (initDoFunc != null)
            {
                initDoFunc();
            }
        }

        public Action initDoFunc;
        
        void OnDestroy() {
            initDoFunc = null;
        }

    }
}
