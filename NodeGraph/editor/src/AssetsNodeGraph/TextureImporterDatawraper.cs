using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.DataWrapers
{

    /// <summary>
    /// TextureImporter Config包装类
    /// 
    /// 对口Unity 版本 ： 5.5.x
    /// 
    /// </summary>


    //    public class TextureImporterDatawraper : Config
    //    {
    //        #region inner_enum_s 
    //        //解决 GUI显示问题
    //
    //        public enum inner_TextureImporterType
    //        {
    //            Default = 0,
    //            NormalMap = 1,
    //            GUI = 2,
    //            Cookie = 4,
    //            Lightmap = 6,
    //            Cursor = 7,
    //            Sprite = 8,
    //            SingleChannel = 10,
    //        }
    //        
    //        public enum inner_TextureImporterGenerateCubemap
    //        {
    //            Spheremap = 1,
    //            Cylindrical = 2,
    //            FullCubemap = 5,
    //            AutoCubemap = 6,
    //        } 
    //
    //        #endregion
    //        
    //        public static TextureImporterDatawraper CreateFormTextureImporter(TextureImporter textureImporter)
    //        {
    //
    //            TextureImporterDatawraper dw = new TextureImporterDatawraper();
    //            dw.updateConfigValue("textureType", textureImporter.textureType.ToString());
    //            dw.updateConfigValue("allowAlphaSplitting", textureImporter.allowAlphaSplitting);
    //            dw.updateConfigValue("alphaIsTransparency", textureImporter.alphaIsTransparency);
    //            dw.updateConfigValue("alphaSource", textureImporter.alphaSource.ToString());
    //            dw.updateConfigValue("anisoLevel", textureImporter.anisoLevel);
    //            dw.updateConfigValue("borderMipmap", textureImporter.borderMipmap);
    //            dw.updateConfigValue("compressionQuality", textureImporter.compressionQuality);
    //            dw.updateConfigValue("convertToNormalmap", textureImporter.convertToNormalmap);
    //            dw.updateConfigValue("crunchedCompression", textureImporter.crunchedCompression);
    //            dw.updateConfigValue("fadeout", textureImporter.fadeout);
    //            dw.updateConfigValue("filterMode", textureImporter.filterMode.ToString());
    //            dw.updateConfigValue("generateCubemap", textureImporter.generateCubemap.ToString());
    //            dw.updateConfigValue("heightmapScale", textureImporter.heightmapScale);
    //            dw.updateConfigValue("isReadable", textureImporter.isReadable);
    //            dw.updateConfigValue("maxTextureSize", textureImporter.maxTextureSize);
    //            dw.updateConfigValue("mipMapBias", textureImporter.mipMapBias);
    //            dw.updateConfigValue("mipmapEnabled", textureImporter.mipmapEnabled);
    //            dw.updateConfigValue("mipmapFadeDistanceEnd", textureImporter.mipmapFadeDistanceEnd);
    //            dw.updateConfigValue("mipmapFadeDistanceStart", textureImporter.mipmapFadeDistanceStart);
    //            dw.updateConfigValue("mipmapFilter", textureImporter.mipmapFilter.ToString());
    //            dw.updateConfigValue("normalmapFilter", textureImporter.normalmapFilter.ToString());
    //            dw.updateConfigValue("npotScale", textureImporter.npotScale.ToString());
    //            dw.updateConfigValue("qualifiesForSpritePacking", textureImporter.qualifiesForSpritePacking);
    //            dw.updateConfigValue("spriteBorder", textureImporter.spriteBorder);
    //            dw.updateConfigValue("spritePackingTag", textureImporter.spritePackingTag);
    //            dw.updateConfigValue("spritePivot", textureImporter.spritePivot);
    //            dw.updateConfigValue("spritePixelsPerUnit", textureImporter.spritePixelsPerUnit);
    //            dw.updateConfigValue("spritesheet", textureImporter.spritesheet);
    //
    //            //处理 textureImporter.spritesheet : SpriteMetaData[]
    //            SpriteMetaData[] smd = textureImporter.spritesheet;
    //            if (smd != null && smd.Length > 0)
    //            {
    //                int i, len = smd.Length;
    //                SpriteMetaDataWraper[] smdw = new SpriteMetaDataWraper[len];
    //                
    //                for (i = 0; i < len; i++)
    //                {
    //                    smdw[i] = SpriteMetaDataWraper.CreateFormSpriteMetaData(smd[i]);
    //                }
    //
    //                dw.updateConfigValue("spritesheet", smdw);
    //
    //            }
    //            else
    //            {
    //                dw.updateConfigValue("spritesheet", null);
    //            }
    //
    //            dw.updateConfigValue("sRGBTexture", textureImporter.sRGBTexture);
    //            dw.updateConfigValue("textureCompression", (int)textureImporter.textureCompression);
    //            dw.updateConfigValue("textureShape", (int)textureImporter.textureShape);
    //            dw.updateConfigValue("wrapMode", (int)textureImporter.wrapMode);
    //
    //            return dw;
    //        }
    //
    //        public TextureImporterDatawraper(){}
    //
    //
    //        /*
    //         public enum TextureImporterType
    //        {
    //            Default = 0,
    //            [Obsolete("Use Default (UnityUpgradable) -> Default")] Image = 0,
    //            [Obsolete("Use NormalMap (UnityUpgradable) -> NormalMap")] Bump = 1,
    //            NormalMap = 1,
    //            GUI = 2,
    //            [Obsolete("Use importer.textureShape = TextureImporterShape.TextureCube")] Cubemap = 3,
    //            [Obsolete("Use a texture setup as a cubemap with glossy reflection instead")] Reflection = 3,
    //            Cookie = 4,
    //            [Obsolete("Use Default instead. All texture types now have an Advanced foldout (UnityUpgradable) -> Default")] Advanced = 5,
    //            Lightmap = 6,
    //            Cursor = 7,
    //            Sprite = 8,
    //            [Obsolete("HDRI is not supported anymore")] HDRI = 9,
    //            SingleChannel = 10,
    //        }
    //         */
    //        public readonly string textureType = "Default"; //=> TextureImporterType
    //
    //        public readonly bool allowAlphaSplitting = true;
    //        public readonly bool alphaIsTransparency = true;
    //
    //        /*
    //            public enum TextureImporterAlphaSource
    //            {
    //                None = 0,
    //                FromInput,
    //                FromGrayScale,
    //            }
    //        */
    //        public readonly string alphaSource = "None"; //=> TextureImporterAlphaSource
    //        public readonly int anisoLevel = 0;
    //        public readonly bool borderMipmap = false;
    //        public readonly int compressionQuality = 0;
    //        public readonly bool convertToNormalmap = false;
    //        public readonly bool crunchedCompression = false;
    //        public readonly bool fadeout = false;
    //
    //        /*
    //            public enum FilterMode
    //            {
    //                Point = 0,
    //                Bilinear,
    //                Trilinear,
    //            }
    //         */
    //        public readonly string filterMode = "Bilinear"; //=> (Enum)FilterMode
    //
    //        /*
    //         public enum TextureImporterGenerateCubemap
    //          {
    //                [Obsolete("This value is deprecated (use TextureImporter.textureShape instead).")] None,
    //                Spheremap,
    //                Cylindrical,
    //                [Obsolete("Obscure shperemap modes are not supported any longer (use TextureImporterGenerateCubemap.Spheremap instead).")] SimpleSpheremap,
    //                [Obsolete("Obscure shperemap modes are not supported any longer (use TextureImporterGenerateCubemap.Spheremap instead).")] NiceSpheremap,
    //                FullCubemap,
    //                AutoCubemap,
    //          } 
    //         */
    //        public readonly string generateCubemap = "Spheremap"; //=> (Enum)TextureImporterGenerateCubemap
    //        public readonly float heightmapScale = 1f;
    //        public readonly bool isReadable = false;
    //        public readonly int maxTextureSize = 4096;
    //        public readonly float mipMapBias = 0.1f; //? 默认值可能不正确
    //        public readonly bool mipmapEnabled = true;
    //        public readonly int mipmapFadeDistanceEnd = 3;
    //        public readonly int mipmapFadeDistanceStart = 1;
    //
    //        /*
    //         public enum TextureImporterMipFilter
    //        {
    //            BoxFilter,
    //            KaiserFilter,
    //        }
    //         */
    //        public readonly string mipmapFilter = "BoxFilter"; //=> (Enum)TextureImporterMipFilter
    //
    //        /* 
    //            public enum TextureImporterNormalFilter
    //            {
    //                Standard,
    //                Sobel,
    //            }
    //        */
    //        public readonly string normalmapFilter = "Standard"; //=> (Enum)TextureImporterNormalFilter
    //
    //        /*
    //         public enum TextureImporterNPOTScale
    //        {
    //            None,
    //            ToNearest,
    //            ToLarger,
    //            ToSmaller,
    //        }
    //         */
    //        public readonly string npotScale = "None"; //=> (Enum)TextureImporterNPOTScale
    //        public readonly Vector4 spriteBorder = Vector4.zero;
    //        public readonly string spritePackingTag = string.Empty;
    //        public readonly Vector2 spritePivot = Vector2.zero;
    //        public readonly float spritePixelsPerUnit = 100f;
    //        public readonly SpriteMetaDataWraper[] spritesheet;
    //        public readonly bool sRGBTexture = true;
    //
    //        /*
    //         public enum TextureImporterCompression
    //        {
    //            Uncompressed,
    //            Compressed,
    //            CompressedHQ,
    //            CompressedLQ,
    //        }
    //         */
    //        public readonly string textureCompression = "Uncompressed"; //=> (Enum)TextureImporterCompression
    //        /*
    //         public enum TextureImporterShape
    //        {
    //            Texture2D = 1,
    //            TextureCube = 2,
    //        }
    //         */
    //        public readonly string textureShape = "Texture2D"; //=> (Enum)TextureImporterShape
    //        /*
    //         public enum TextureWrapMode
    //        {
    //            Repeat,
    //            Clamp,
    //        }
    //         */
    //         public readonly string wrapMode = "Clamp"; //=> (Enum)TextureWrapMode
    //
    //        public TextureImporter ToTextureImporter()
    //        {
    //
    //            TextureImporter ti = new TextureImporter();
    //            ti.textureType = (TextureImporterType)Enum.Parse(typeof(TextureImporterType), textureType);
    //            ti.allowAlphaSplitting = allowAlphaSplitting;
    //            ti.alphaIsTransparency = alphaIsTransparency;
    //            ti.alphaSource = (TextureImporterAlphaSource)Enum.Parse(typeof(TextureImporterAlphaSource), alphaSource);
    //            ti.anisoLevel = anisoLevel;
    //            ti.borderMipmap = borderMipmap;
    //            ti.compressionQuality = compressionQuality;
    //            ti.convertToNormalmap = convertToNormalmap;
    //            ti.crunchedCompression = crunchedCompression;
    //            ti.fadeout = fadeout;
    //            ti.filterMode = (FilterMode) Enum.Parse(typeof (FilterMode), filterMode);
    //            ti.generateCubemap = (TextureImporterGenerateCubemap)Enum.Parse(typeof(TextureImporterGenerateCubemap), generateCubemap);
    //            ti.heightmapScale = heightmapScale;
    //            ti.isReadable = isReadable;
    //            ti.maxTextureSize = maxTextureSize;
    //            ti.mipMapBias = mipMapBias;
    //            ti.mipmapEnabled = mipmapEnabled;
    //            ti.mipmapFadeDistanceEnd = mipmapFadeDistanceEnd;
    //            ti.mipmapFadeDistanceStart = mipmapFadeDistanceStart;
    //            ti.mipmapFilter = (TextureImporterMipFilter)Enum.Parse(typeof(TextureImporterMipFilter), mipmapFilter);
    //            ti.normalmapFilter = (TextureImporterNormalFilter)Enum.Parse(typeof(TextureImporterNormalFilter), normalmapFilter);
    //            ti.npotScale = (TextureImporterNPOTScale)Enum.Parse(typeof(TextureImporterNPOTScale), npotScale);
    //            ti.spriteBorder = spriteBorder;
    //            ti.spritePackingTag = spritePackingTag;
    //            ti.spritePivot = spritePivot;
    //            ti.spritePixelsPerUnit = spritePixelsPerUnit;
    //
    //            if (spritesheet != null && spritesheet.Length > 0)
    //            {
    //                int i, len = spritesheet.Length;
    //                SpriteMetaData[] smd = new SpriteMetaData[len];
    //                for (i = 0; i < len; i++)
    //                {
    //                    smd[i] = spritesheet[i].ToSpriteMetaData();
    //                }
    //                ti.spritesheet = smd;
    //            }
    //            else
    //            {
    //                ti.spritesheet = null;
    //            }
    //
    //            ti.sRGBTexture = sRGBTexture;
    //            ti.textureCompression = (TextureImporterCompression)Enum.Parse(typeof(TextureImporterCompression), textureCompression);
    //            ti.textureShape = (TextureImporterShape)Enum.Parse(typeof(TextureImporterShape), textureShape);
    //            ti.wrapMode = (TextureWrapMode)Enum.Parse(typeof(TextureWrapMode), wrapMode);
    //
    //            return ti;
    //        }
    //
    //    }

    //Todo 需要重写

    public class TextureImporterDatawraper
    {

    }

}
