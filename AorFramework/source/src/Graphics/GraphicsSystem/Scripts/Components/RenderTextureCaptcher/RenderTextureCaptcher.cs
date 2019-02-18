using System;
using System.Collections.Generic;
using UnityEngine;
using Framework.Graphic.Utility;

namespace Framework.Graphic
{
    /// <summary>
    /// RenderTexture抓取器, 把当前相机的渲染流数据抓取到一张RT上。
    /// </summary>
    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    [RequireComponent(typeof(Camera))]
    public class RenderTextureCaptcher : MonoBehaviour
    {

        [Space(10)]

        [Tooltip("RT质量系数(值为1等于标准屏幕大小)")]
        public float Quality = 1;

        [Space(10)]

        [Tooltip("DepthBuffer参数")]
        public int RT_DepthBuffer = 32;
        [Tooltip("抗锯齿参数(1,2,4,8)")]
        public int RT_AntiAliasing = 1;
        [Tooltip("RT格式")]
        public RenderTextureFormat RT_Format = RenderTextureFormat.DefaultHDR;

        [Space(10)]

        [SerializeField] //调试用标记
        private RenderTexture m_capturedRT;
        public RenderTexture CapturedRT {
            get { return m_capturedRT; }
        }

        private bool m_isStarted = false;

        private void OnEnable()
        {
            if (!m_isStarted) return;
            //
            m_createCapturedRT();
        }
        
        private void Start()
        {
            m_cached_values();
            m_createCapturedRT();
            m_isStarted = true;
        }

        private void OnDisable()
        {
            m_releaseCapturedRT();
        }

        private bool m_createAsTemp;

        private void m_createCapturedRT() {
            if (!m_capturedRT)
            {
                m_capturedRT = RenderTextureUtility.New((int)(m_cache_screenWidth * m_cache_quality), (int)(m_cache_screenHeight * m_cache_quality), m_cache_RT_DepthBuffer, m_cache_RT_Format, RenderTextureReadWrite.Default, m_cache_RT_AntiAliasing);
                m_createAsTemp = true;
            }
        }

        private void m_releaseCapturedRT() {
            if (m_capturedRT && m_createAsTemp)
            {
                RenderTextureUtility.Release(m_capturedRT);
                m_capturedRT = null;
                m_createAsTemp = false;
            }
        }

        private int m_cache_screenWidth;
        private int m_cache_screenHeight;
        private float m_cache_quality;
        private int m_cache_RT_AntiAliasing;
        private int m_cache_RT_DepthBuffer;
        private RenderTextureFormat m_cache_RT_Format;

        private void m_cached_values() {
            m_cache_screenWidth = Screen.width;
            m_cache_screenHeight = Screen.height;
            m_cache_quality = Quality;
            m_cache_RT_AntiAliasing = RT_AntiAliasing;
            m_cache_RT_DepthBuffer = RT_DepthBuffer;
            m_cache_RT_Format = RT_Format;
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (m_createAsTemp)
            {
                if (!m_cache_screenWidth.Equals(Screen.width)
                    || !m_cache_screenHeight.Equals(Screen.height)
                    || !m_cache_quality.Equals(Quality)
                    || !m_cache_RT_AntiAliasing.Equals(RT_AntiAliasing)
                    || !m_cache_RT_DepthBuffer.Equals(RT_DepthBuffer)
                    || !m_cache_RT_Format.Equals(RT_Format)
                )
                {
                    m_releaseCapturedRT();
                    m_cached_values();
                    m_createCapturedRT();
                }
            }

            RenderTextureUtility.Copy(src, m_capturedRT);
            UnityEngine.Graphics.Blit(src, dest);
        }

        public Texture2D GetCapturedTexture2D()
        {
            if (m_capturedRT)
            {
                RenderTexture org = RenderTexture.active;
                RenderTexture.active = m_capturedRT;
                Texture2D tex = new Texture2D(m_capturedRT.width, m_capturedRT.height,
                    (m_cache_RT_Format == RenderTextureFormat.ARGBFloat || m_cache_RT_Format == RenderTextureFormat.DefaultHDR) ? TextureFormat.RGBAFloat : TextureFormat.RGBA32
                    , false);
                tex.ReadPixels(new Rect(0, 0, m_capturedRT.width, m_capturedRT.height), 0, 0);
                tex.Apply();
                RenderTexture.active = org;
                return tex;
            }
            return null;
        }

    }
}
