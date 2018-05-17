using UnityEngine;
using System;
using System.Collections;

namespace Framework.Graphic.FastShadowProjector
{
    public class GlobalProjectorLightCamera : MonoBehaviour
    {

        public Action PreCullCallback;
        public Action PostRenderCallback;

        void OnPreCull()
        {
            if (PreCullCallback != null)
            {
                PreCullCallback();
            }
        }

        void OnPostRender()
        {
            if (PostRenderCallback != null)
            {
                PostRenderCallback();
            }
        }
    }
}


