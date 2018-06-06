using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework.Graphic.FastShadowProjector
{

    public class FSPConfigAsset : ScriptableObject
    {

        public enum ShadowResolutions
        {
            VeryLow_128 = 0,
            Low_256,
            Normal_512,
            High_1024,
            VeryHigh_2048,
            SupperHigh_4096,
        }

        public float ZClipFar = 30;
        public Vector3 ProjectionAngles = new Vector3(90, 0, 0);
        public bool FixedProjection = false;
        public ShadowResolutions ShadowResolution = ShadowResolutions.Normal_512;
    }

}


