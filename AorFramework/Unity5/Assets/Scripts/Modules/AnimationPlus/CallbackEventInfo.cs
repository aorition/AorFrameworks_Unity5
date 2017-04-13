using System;
using System.Collections.Generic;

using UnityEngine;

namespace YoukiaUnity.View
{
    public class CallbackEventInfo
    {

        public GameObject target;
        public bool isPlayEnd;
        public string currentAnimName;

        public CallbackEventInfo(GameObject target, string AnimName, bool isPlayEnd)
        {
            this.target = target;
            this.currentAnimName = AnimName;
            this.isPlayEnd = isPlayEnd;
        }

    }
}

