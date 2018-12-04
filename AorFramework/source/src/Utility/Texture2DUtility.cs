using UnityEngine;

using System;
using Framework.Extends;

namespace Framework.Utility
{

    public class Texture2DUtility
    {

        private static int _baseGraySize = 64;

        private static Texture2D _defaultGraylTex;
        private static Texture2D DefaultGraylTex
        {
            get
            {
                if (!_defaultGraylTex)
                {
                    _defaultGraylTex = new Texture2D(_baseGraySize, _baseGraySize, TextureFormat.RGBA32, true);
                    Color[] colors = new Color[_baseGraySize * _baseGraySize];
                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i] = new Color(0.5f, 0.5f, 0.5f, 1f);
                    }
                    _defaultGraylTex.SetPixels(colors);
                    _defaultGraylTex.Apply();
                }
                return _defaultGraylTex;
            }
        }

        private static Texture2D _defaultNormalTex;
        public static Texture2D DefaultNormalTex
        {
            get
            {
                if (!_defaultNormalTex)
                {
                    _defaultNormalTex = new Texture2D(16, 16, TextureFormat.RGB24, false);
                    _defaultNormalTex.name = "DefaultNormal";
                    _defaultNormalTex.wrapMode = TextureWrapMode.Repeat;
                    int len = 256;
                    Color32[] color32s = new Color32[len];
                    for (int i = 0; i < len; i++)
                    {
                        color32s[i] = new Color32(125, 125, 255, 255);
                    }
                    _defaultNormalTex.SetPixels32(color32s);
                }
                return _defaultNormalTex;
            }
        }

        /// <summary>
        /// 分割图片颜色 (grayscaleTex : 去色,平衡亮度; midcolorTex : 中间色 )
        /// </summary>
        public static void GrayscaleTex(Texture2D tex, out Texture2D grayscaleTex, out Texture2D midcolorTex, int mipmapLevel = 5)
        {

            if (tex.width != tex.height)
            {
                Debug.LogWarning("*** Texture2DUtility.GrayscaleTex 需要源Texture2D为正方形贴图(最好是2的N次幂).");
                grayscaleTex = null;
                midcolorTex = null;
                return;
            }

            if (tex.mipmapCount == 0)
            {
                Debug.LogWarning("*** Texture2DUtility.GrayscaleTex 需要源Texture2D 开启Mipmap.");
                grayscaleTex = null;
                midcolorTex = null;
                return;
            }

            Color[] colors = tex.GetPixels();
            Color[] mColors = tex.GetPixels(mipmapLevel);

            int bd = (int)Mathf.Sqrt((float)mColors.Length);

            grayscaleTex = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, true);

            midcolorTex = new Texture2D(bd, bd, TextureFormat.RGBA32, true);
            midcolorTex.SetPixels(mColors);
            midcolorTex.Apply(true);

            Color[] gColors = new Color[colors.Length];
            int i = 0;
            float nv, nu;
            for (int v = 0; v < tex.height; v++)
            {
                for (int u = 0; u < tex.width; u++)
                {
                    i = v * tex.width + u;
                    nv = v / tex.height;
                    nu = u / tex.width;
                    gColors[i] = colors[i].Decolorization().Divide(midcolorTex.GetPixelBilinear(nu, nv).Decolorization());
                }
            }
            grayscaleTex.SetPixels(gColors);
            grayscaleTex.Apply(true);

        }

        #region Save Texture

        public static void SaveTexture2DToPNG(string path, Texture2D t2d)
        {

            try
            {
                byte[] bytes = t2d.EncodeToPNG();
                AorBaseUtility.AorIO.SaveBytesToFile(path, bytes);
            }
            catch (Exception ex)
            {
                //
            }

        }

        public static void SaveTexture2DToJPG(string path, Texture2D t2d)
        {

            try
            {
                byte[] bytes = t2d.EncodeToJPG();
                AorBaseUtility.AorIO.SaveBytesToFile(path, bytes);
            }
            catch (Exception ex)
            {
                //
            }

        }

        public static void SaveTexture2DToEXR(string path, Texture2D t2d)
        {

            try
            {
                byte[] bytes = t2d.EncodeToEXR();
                AorBaseUtility.AorIO.SaveBytesToFile(path, bytes);
            }
            catch (Exception ex)
            {
                //
            }

        }

        #endregion

    }
}