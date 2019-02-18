using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic.Utility
{
    public class RenderTextureUtility
    {

        public static Action<Exception> OnThowError;

        public enum FormatDef
        {
            Default = RenderTextureFormat.Default,
            DefaultHDR = RenderTextureFormat.DefaultHDR,
            Depth = RenderTextureFormat.Depth,
            Normal = RenderTextureFormat.ARGB32,
            ColorBuffer = RenderTextureFormat.RGB565,
            Half = RenderTextureFormat.ARGBHalf,
            Float = RenderTextureFormat.ARGBFloat
        }

        public enum DepthDef
        {
            NoDepth = 0,
            D16 = 16,
            D24 = 24,
            D32 = 32
        }

        public enum AntiAliasingDef
        {
            NoAntiAliasing = 1,
            X2 = 2,
            X4 = 4,
            X8 = 8
        }

        private static int m_RequestRTNum = 0;
        public static int RequestRTNum
        {
            get
            {
                return m_RequestRTNum;
            }
        }

        public static RenderTexture New(int width, int height)
        {
            m_RequestRTNum++;
            return RenderTexture.GetTemporary(width, height);
        }

        public static RenderTexture New(int width, int height, int depthBuffer)
        {
            m_RequestRTNum++;
            return RenderTexture.GetTemporary(width, height, depthBuffer);
        }

        public static RenderTexture New(int width, int height, int depthBuffer, RenderTextureFormat format)
        {
            m_RequestRTNum++;
            return RenderTexture.GetTemporary(width, height, depthBuffer, format);
        }

        public static RenderTexture New(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite)
        {
            m_RequestRTNum++;
            return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite);
        }

        public static RenderTexture New(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
        {
            m_RequestRTNum++;
            return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing);
        }

        public static void Release(RenderTexture renderTexture)
        {
            m_RequestRTNum --;
            if (m_RequestRTNum < 0 && OnThowError != null)
            {
                OnThowError(new Exception("RenderTexture exceeds the lower limit! Some RenderTexture has not been created using the New method, but it is disposed using the Release method."));
            }
            RenderTexture.ReleaseTemporary(renderTexture);
        }

        private static Material m_ReverseMat;
        private static Material ReverseMat {
            get {
                if (!m_ReverseMat)
                {
                    Shader shader = ShaderBridge.Find("Hidden/PostEffect/Reverse");
                    if (!shader)
                    {
                        OnThowError(new Exception("** RenderTextureUtility.create ReverseMat Error :: Can not Find the Shader : Hidden/PostEffect/Reverse , create ReverseMat fail."));
                    }
                    else
                    {
                        m_ReverseMat = new Material(shader);
                    }
                }
                return m_ReverseMat;
            }
        }

        public static void Copy(RenderTexture srcRt, RenderTexture tarRt, bool reverse = false)
        {
            if (reverse)
            {
                UnityEngine.Graphics.Blit(srcRt, tarRt, ReverseMat);
            }
            else
            {
                UnityEngine.Graphics.Blit(srcRt, tarRt);
            }
        }

    }
}
