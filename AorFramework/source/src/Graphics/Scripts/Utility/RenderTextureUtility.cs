using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic.Utility
{
    public class RenderTextureUtility
    {

        public static Action<Exception> OnThowError;

        public struct FormatInfo
        {

            public FormatInfo(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
            {
                this.width = width;
                this.height = height;
                this.depthBuffer = depthBuffer;
                this.format = format;
                this.readWrite = readWrite;
                this.antiAliasing = antiAliasing;
            }

            public int width;
            public int height;
            public int depthBuffer;
            public RenderTextureFormat format;
            public RenderTextureReadWrite readWrite;
            //抗锯齿 必须是 1,2,4 or 8
            public int antiAliasing;
        }

        public static FormatInfo GetSimpleFormatInfo(int width, int height)
        {
            return new FormatInfo(width, height, 16, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default, 1);
        }

        public static FormatInfo GetNormalFormatInfo(int width, int height)
        {
            return new FormatInfo(width, height, 32, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 1);
        }

        public static FormatInfo GetNormalFormatInfo(int width, int height, int antiAliasing)
        {
            return new FormatInfo(width, height, 32, RenderTextureFormat.Default, RenderTextureReadWrite.Default, antiAliasing);
        }

        public static FormatInfo GetNormalHDRFormatInfo(int width, int height)
        {
            return new FormatInfo(width, height, 32, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Default, 1);
        }
        
        public static FormatInfo GetFullFormatInfo(int width, int height)
        {
            return new FormatInfo(width, height, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
        }

        public static FormatInfo GetFull64FormatInfo(int width, int height)
        {
            return new FormatInfo(width, height, 64, RenderTextureFormat.ARGB64, RenderTextureReadWrite.Default, 1);
        }

        public static FormatInfo GetCustomFormatInfo(int width, int height, int depthBuffer, RenderTextureFormat format,
            RenderTextureReadWrite readWrite, int antiAliasing)
        {
            return new FormatInfo(width, height, depthBuffer, format, readWrite, antiAliasing);
        }

        //----------------------------------------

        private static int _RequestRTNum = 0;
        public static int RequestRTNum
        {
            get
            {
                return _RequestRTNum;
            }
        }

        public static RenderTexture New(RenderTextureUtility.FormatInfo info)
        {
            _RequestRTNum ++;
            return RenderTexture.GetTemporary(info.width, info.height, info.depthBuffer, info.format, info.readWrite, info.antiAliasing);
        }

        public static void Release(RenderTexture renderTexture)
        {
            _RequestRTNum --;
            if (_RequestRTNum < 0 && OnThowError != null)
            {
                OnThowError(new Exception("RenderTexture exceeds the lower limit! Some RenderTexture has not been created using the New method, but it is disposed using the Release method."));
            }
            RenderTexture.ReleaseTemporary(renderTexture);
        }

    }
}
