using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic.Effect
{
    public class FLBloomEffect : FLEffectBase
    {
        public enum Resolution
        {
            Low = 0,
            High = 1,
        }

        [Range(0.0f, 4f)]
        public float threshhold = 3f;//0.2f; 


        [Range(0.0f, 2.5f)]
        public float intensity = 0.05f;//0.75f;

        [Range(0.25f, 5.5f)]
        public float blurSize = 5f;//3f;

        public Resolution resolution = Resolution.Low;

        [Range(1, 4)]
        public int blurIterations = 1;


        private Material fastBloomMat;
        private RenderTexture rt;
        private RenderTexture rt2;

        public string ScriptName
        {
            get { return "BloomEffect"; }
        }

        /// <summary>
        /// 清理部分渲染贴图,下次再用会重新创建
        /// </summary>
        public void Clear()
        {
            rt = null;
            rt2 = null;
        }

        protected override void init()
        {
            if (fastBloomMat == null)
            {
                fastBloomMat = new Material(ShaderBridge.Find("Hidden/PostEffect/FastBloom"));
                renderMat = new Material(ShaderBridge.Find("Hidden/PostEffect/BloomDrawShader"));
            }
        }

        protected override void OnDisable()
        {
            if (GraphicsManager.IsInit())
                GraphicsManager.Instance.RemovePostEffectComponent(this);
            Clear();
        }

        protected override void render(RenderTexture src, RenderTexture dst)
        {
            base.render(src, dst);

            if (fastBloomMat == null || fastBloomMat.shader == null)
                return;

            int divider = resolution == Resolution.Low ? 8 : 4;
            float widthMod = resolution == Resolution.Low ? 0.5f : 1.0f;

            fastBloomMat.SetVector("_Parameter", new Vector4(blurSize * widthMod, 0.0f, threshhold, intensity));

            var rtW = src.width / divider;
            var rtH = src.height / divider;

            // downsample
            if (rt == null)
            {
                rt = new RenderTexture(rtW, rtH, 0, src.format);
                rt.filterMode = FilterMode.Bilinear;
            }

            Graphics.Blit(src, rt, fastBloomMat, 0);


            for (int i = 0; i < blurIterations; i++)
            {
                fastBloomMat.SetVector("_Parameter", new Vector4(blurSize * widthMod + (i * 1.0f), 0.0f, threshhold, intensity));

                // vertical blur
                if (rt2 == null)
                {
                    rt2 = new RenderTexture(rtW, rtH, 0, src.format);
                    rt2.filterMode = FilterMode.Bilinear;
                }

                Graphics.Blit(rt, rt2, fastBloomMat, 1);


                // horizontal blur
                Graphics.Blit(rt2, rt, fastBloomMat, 2);

            }

            if (renderMat != null)
            {

                renderMat.SetTexture("_CurveTex", rt);
                renderMat.SetTexture("_MainTex", src);
            }

            Graphics.Blit(src, dst, renderMat);

        }



    }
}


