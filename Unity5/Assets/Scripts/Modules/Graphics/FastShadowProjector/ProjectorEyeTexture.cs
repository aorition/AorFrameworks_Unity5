using UnityEngine;
using System.Collections;



namespace YoukiaUnity.Graphics.FastShadowProjector
{
    [System.Serializable]
    public class ProjectorEyeTexture
    {

        RenderTexture _RenderTexture;
        Texture2D _RenderTextureDummy;

        Camera _Camera;

        public ProjectorEyeTexture(Camera camera, int size)
        {
            _Camera = camera;
            _RenderTexture = null;
            _RenderTextureDummy = null;

            if (RenderTextureSupported())
            {
                if (_RenderTexture != null)
                {
                    _RenderTexture.Release();
                    _RenderTexture = null;
                }

                _RenderTexture = new RenderTexture(size, size, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                _RenderTexture.antiAliasing = 4;

                _RenderTexture.anisoLevel = 0;
                _RenderTexture.filterMode = FilterMode.Bilinear;
                _RenderTexture.useMipMap = false;

                //Unity5 版本 RT一旦被创建,_RenderTexture.useMipMap 设置就会没有效果了.
                _RenderTexture.Create();

                camera.targetTexture = _RenderTexture;
            }
            else
            {
                if (_RenderTextureDummy != null)
                {
                    _RenderTextureDummy = null;
                }

                _RenderTextureDummy = new Texture2D((int)_Camera.pixelWidth, (int)_Camera.pixelHeight, TextureFormat.ARGB32, false, true);
                _RenderTextureDummy.filterMode = FilterMode.Bilinear;
                _RenderTextureDummy.wrapMode = TextureWrapMode.Clamp;
            }
        }

        public void CleanUp()
        {
            if (_RenderTexture != null)
            {
                _RenderTexture.Release();
                _RenderTexture = null;
            }

            if (_RenderTextureDummy != null)
            {
                _RenderTextureDummy = null;
            }
        }

        public Texture GetTexture()
        {
            if (RenderTextureSupported())
            {
                return _RenderTexture;
            }
            else
            {
                return _RenderTextureDummy;
            }
        }

        public RenderTexture GetRenderTexture()
        {
            return _RenderTexture;
        }

        public void GrabScreenIfNeeded()
        {
            if (!RenderTextureSupported())
            {
                _RenderTextureDummy.ReadPixels(new Rect(0, 0, (int)_Camera.pixelWidth, (int)_Camera.pixelHeight), 0, 0, false);
                _RenderTextureDummy.Apply();
            }
        }

        public bool RenderTextureSupported()
        {
#if UNITY_5_5_OR_NEWER
            return true;
#else
            return SystemInfo.supportsRenderTextures;
#endif
        }
    }



}


