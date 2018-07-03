using System;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    [Serializable]
    public struct MipMapSetting
    {

        public static MipMapSetting NoMipMap()
        {
            return new MipMapSetting();
        }

        public static MipMapSetting Default()
        {
            return new MipMapSetting(0.1f, TextureImporterMipFilter.BoxFilter, 0, 0, false);
        }

        public MipMapSetting(float mipMapBias, TextureImporterMipFilter mipFilter, int mipmapFadeDistanceStart, int mipmapFadeDistanceEnd, bool borderMipmap)
        {
            this.mipMapBias = mipMapBias;
            this.mipFilter = mipFilter;
            this.mipmapFadeDistanceStart = mipmapFadeDistanceStart;
            this.mipmapFadeDistanceEnd = mipmapFadeDistanceEnd;
            this.borderMipmap = borderMipmap;
            this._isInit = true;
        }

        [SerializeField]
        private bool _isInit;
        public bool isInit
        {
            get { return _isInit; }
        }

        public float mipMapBias;
        public TextureImporterMipFilter mipFilter;
        public int mipmapFadeDistanceStart;
        public int mipmapFadeDistanceEnd;
        public bool borderMipmap;
    }
}
