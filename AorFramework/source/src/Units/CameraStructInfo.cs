using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public struct CameraStructInfo
    {

        public static CameraStructInfo Default()
        {
            return new CameraStructInfo(CameraClearFlags.Skybox, new Color(0.0172869f, 0.3019607f, 0.4745098f, 0), -1,
                false, 5, 60f, 0.3f, 1000f, new Rect(0, 0, 1, 1), 51, RenderingPath.UsePlayerSettings, null, true, true,
                true);
        }

        public static CameraStructInfo DefaultUI()
        {
            return new CameraStructInfo(CameraClearFlags.Depth, new Color(0, 0, 0, 0), 1<<5,
                true, 10, 60f, 10, 100f, new Rect(0, 0, 1, 1), 51, RenderingPath.UsePlayerSettings, null, false, false,
                false);
        }

        public CameraStructInfo(CameraClearFlags clearFlags, Color background, int cullingMask, bool orthographic,
            float orthographicSize, float fieldOfView, float nearClipPlane, float farClipPlane, Rect viewportRect, float Depth,
            RenderingPath renderingPath, RenderTexture targetTexture, bool occlusionCulling, bool allowHdr, bool allowMsaa
            )
        {
            this.ClearFlags = clearFlags;
            this.Background = background;
            this.CullingMask = cullingMask;
            this.Orthographic = orthographic;
            this.OrthographicSize = orthographicSize;
            this.FieldOfView = fieldOfView;
            this.NearClipPlane = nearClipPlane;
            this.FarClipPlane = farClipPlane;
            this.ViewportRect = viewportRect;
            this.Depth = Depth;
            this.RenderingPath = renderingPath;
            this.TargetTexture = targetTexture;
            this.OcclusionCulling = occlusionCulling;
            this.AllowHDR = allowHdr;
            this.AllowMSAA = allowMsaa;
            this._isInit = true;
        }

        [SerializeField]
        private bool _isInit;
        public bool isInit
        {
            get { return _isInit; }
        }

        public CameraClearFlags ClearFlags;
        public Color Background;
        public int CullingMask;
        public bool Orthographic;
        public float OrthographicSize;
        public float FieldOfView;
        public float NearClipPlane;
        public float FarClipPlane;
        public Rect ViewportRect;
        public float Depth;
        public RenderingPath RenderingPath;
        public RenderTexture TargetTexture;
        public bool OcclusionCulling;
        public bool AllowHDR;
        public bool AllowMSAA;
    }
}
