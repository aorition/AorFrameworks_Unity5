using System;
using System.Collections.Generic;
using UnityEngine;


namespace Framework.Graphic.FastShadowProjector
{
    public interface IProjector
    {
        bool IsLight { get; set; }
        Vector3 GlobalProjectionDir { get; set; }
        int GlobalShadowResolution { get; set; }
        FastShadowProjectorManager.ShadowType Type { get; }

        Bounds GetBounds();
        void SetVisible(bool visible);
        void OnPreRenderShadowProjector(Camera camera);
    }

}


