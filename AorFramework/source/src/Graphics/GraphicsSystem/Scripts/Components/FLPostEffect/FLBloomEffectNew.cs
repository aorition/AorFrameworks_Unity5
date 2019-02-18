#pragma warning disable
using Framework.Graphic.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic.Effect
{
    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    public class FLBloomEffectNew : FLEffectBase
    {
        #region Public Properties

        /// Prefilter threshold (gamma-encoded)
        /// Filters out pixels under this level of brightness.
        public float thresholdGamma
        {
            get { return Mathf.Max(_threshold, 0); }
            set { _threshold = value; }
        }

        /// Prefilter threshold (linearly-encoded)
        /// Filters out pixels under this level of brightness.
        public float thresholdLinear
        {
            get { return GammaToLinear(thresholdGamma); }
            set { _threshold = LinearToGamma(value); }
        }

        [SerializeField]
        [Tooltip("Filters out pixels under this level of brightness.")]
        float _threshold = 0.95f;

        /// Soft-knee coefficient
        /// Makes transition between under/over-threshold gradual.
        public float softKnee
        {
            get { return _softKnee; }
            set { _softKnee = value; }
        }

        [SerializeField, Range(0, 1)]
        [Tooltip("Makes transition between under/over-threshold gradual.")]
        float _softKnee = 0.5f;

        /// Bloom radius
        /// Changes extent of veiling effects in a screen
        /// resolution-independent fashion.
        public float radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        [SerializeField, Range(1, 7)]
        [Tooltip("Changes extent of veiling effects\n" +
                 "in a screen resolution-independent fashion.")]
        float _radius = 2.5f;

        /// Bloom intensity
        /// Blend factor of the result image.
        public float intensity
        {
            get { return Mathf.Max(_intensity, 0); }
            set { _intensity = value; }
        }

        [SerializeField]
        [Tooltip("Blend factor of the result image.")]
        float _intensity = 0.8f;

        /// High quality mode
        /// Controls filter quality and buffer resolution.
        public bool highQuality
        {
            get { return _highQuality; }
            set { _highQuality = value; }
        }

        [SerializeField]
        [Tooltip("Controls filter quality and buffer resolution.")]
        bool _highQuality = true;

        /// Anti-flicker filter
        /// Reduces flashing noise with an additional filter.
        [SerializeField]
        [Tooltip("Reduces flashing noise with an additional filter.")]
        bool _antiFlicker = true;

        public bool antiFlicker
        {
            get { return _antiFlicker; }
            set { _antiFlicker = value; }
        }

        #endregion

        #region Private Members

        [SerializeField, HideInInspector]

        const int kMaxIterations = 16;
        RenderTexture[] _blurBuffer1 = new RenderTexture[kMaxIterations];
        RenderTexture[] _blurBuffer2 = new RenderTexture[kMaxIterations];

        float LinearToGamma(float x)
        {
#if UNITY_5_3_OR_NEWER
            return Mathf.LinearToGammaSpace(x);
#else
            if (x <= 0.0031308f)
                return 12.92f * x;
            else
                return 1.055f * Mathf.Pow(x, 1 / 2.4f) - 0.055f;
#endif
        }

        float GammaToLinear(float x)
        {
#if UNITY_5_3_OR_NEWER
            return Mathf.GammaToLinearSpace(x);
#else
            if (x <= 0.04045f)
                return x / 12.92f;
            else
                return Mathf.Pow((x + 0.055f) / 1.055f, 2.4f);
#endif
        }

        #endregion
        
        private RenderTexture rt;

        public string ScriptName
        {
            get { return "BloomEffectNew"; }
        }

        /// <summary>
        /// 清理部分渲染贴图,下次再用会重新创建
        /// </summary>
        public void Clear()
        {
            // release the temporary buffers
            for (var i = 0; i < kMaxIterations; i++)
            {
                if (_blurBuffer1[i] != null)
                {
                    //RenderTexture.ReleaseTemporary(_blurBuffer1[i]);
                    RenderTextureUtility.Release(_blurBuffer1[i]);
                    _blurBuffer1[i] = null;
                }

                if (_blurBuffer2[i] != null)
                {
                    //RenderTexture.ReleaseTemporary(_blurBuffer2[i]);
                    RenderTextureUtility.Release(_blurBuffer2[i]);
                    _blurBuffer2[i] = null;
                }
            }

            if (rt != null) {
                //RenderTexture.ReleaseTemporary(rt);
                RenderTextureUtility.Release(rt);
                rt = null;
            }
        }

        protected override void init()
        {
            base.init();
            //填充后处理用Shader&Material
            if (renderMat == null)
            {
                renderMat = new Material(ShaderBridge.Find("Hidden/Kino/Bloom"));
            }
            //填充默认渲染等级值
            if (m_RenderLevel == 0) m_RenderLevel = 100;
        }

        protected override void OnDisable()
        {
            if (GraphicsManager.IsInit())
                GraphicsManager.Instance.RemovePostEffectComponent(this);
            Clear();
        }

        protected override void render(RenderTexture src, RenderTexture dst)
        {
            //if (!Application.isPlaying) return;

            base.render(src, dst);

            if (renderMat == null || renderMat.shader == null)
            {
                Debug.Log("this is a null material");
                return;
            }


            Material _material = renderMat;

            var useRGBM = Application.isMobilePlatform;

            // source texture size
            var tw = src.width;
            var th = src.height;

            // halve the texture size for the low quality mode
            if (!_highQuality)
            {
                tw /= 4;
                th /= 4;
            }

            // blur buffer format
            var rtFormat = useRGBM ?
                RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;

            // determine the iteration count
            var logh = Mathf.Log(th, 2) + _radius - 8;
            var logh_i = (int)logh;
            var iterations = Mathf.Clamp(logh_i, 1, kMaxIterations);

            // update the shader properties
            var lthresh = thresholdLinear;
            _material.SetFloat("_Threshold", lthresh);

            var knee = lthresh * _softKnee + 1e-5f;
            var curve = new Vector3(lthresh - knee, knee * 2, 0.25f / knee);
            _material.SetVector("_Curve", curve);

            var pfo = !_highQuality && _antiFlicker;
            _material.SetFloat("_PrefilterOffs", pfo ? -0.5f : 0.0f);

            _material.SetFloat("_SampleScale", 0.5f + logh - logh_i);
            _material.SetFloat("_Intensity", intensity);


            // downsample
            if (rt == null)
            {
                //rt = RenderTexture.GetTemporary(tw, th, 0, rtFormat); //src.format
                                                                      //rt.filterMode = FilterMode.Bilinear;
                rt = RenderTextureUtility.New(tw, th, 0, rtFormat);
            }

            // prefilter pass
            var pass = _antiFlicker ? 1 : 0;
            Graphics.Blit(src, rt, _material, pass);

            // construct a mip pyramid
            var last = rt;
            for (var level = 0; level < iterations; level++)
            {
                if (_blurBuffer1[level] == null)
                {
                    //_blurBuffer1[level] = RenderTexture.GetTemporary(
                    //    last.width / 2, last.height / 2, 0, rtFormat
                    //    );

                    _blurBuffer1[level] = RenderTextureUtility.New(last.width / 2, last.height / 2, 0, rtFormat);
                }

                pass = (level == 0) ? (_antiFlicker ? 3 : 2) : 4;
                Graphics.Blit(last, _blurBuffer1[level], _material, pass);

                last = _blurBuffer1[level];
            }

            // upsample and combine loop
            for (var level = iterations - 2; level >= 0; level--)
            {
                var basetex = _blurBuffer1[level];
                _material.SetTexture("_BaseTex", basetex);

                if (_blurBuffer2[level] == null)
                {
                    //_blurBuffer2[level] = RenderTexture.GetTemporary(
                    //    basetex.width, basetex.height, 0, rtFormat
                    //);
                    _blurBuffer2[level] = RenderTextureUtility.New(basetex.width, basetex.height, 0, rtFormat);
                }

                pass = _highQuality ? 6 : 5;
                Graphics.Blit(last, _blurBuffer2[level], _material, pass);
                last = _blurBuffer2[level];
            }

            // finish process
            _material.SetTexture("_BaseTex", src);
            pass = _highQuality ? 8 : 7;
            Graphics.Blit(last, dst, _material, pass);

        }



    }
}


