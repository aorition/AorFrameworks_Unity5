#pragma warning disable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Framework.Graphic.Effect
{

    public class FLGaussBlurEffect : FLEffectBase
    {

        public static void CreateGaussImage(Texture2D src, ref RenderTexture dst, float power = 0.01f)
        {
            if (!dst || !src)
                return;


            Material mat = new Material(ShaderBridge.Find("Hidden/PostEffect/GaussBlur"));


            mat.SetFloat("_Offset", power);

            RenderTexture tex = RenderTexture.GetTemporary(src.width / 4, src.height / 4, 0);
            RenderTexture tex2 = RenderTexture.GetTemporary(src.width / 4, src.height / 4, 0);

            Graphics.Blit(src, tex, mat);
            Graphics.Blit(tex, tex2, mat);
            Graphics.Blit(tex2, tex, mat);
            Graphics.Blit(tex, dst, mat);

            tex.Release();
            tex2.Release();
            Destroy(mat);

        }

        //-------------------------------------------------

        public float Power = 0.01f;

        protected override void init()
        {
            base.init();
            if (renderMat == null)
            {
                renderMat = new Material(ShaderBridge.Find("Hidden/PostEffect/GaussBlur"));
            }
        }

        protected override void render(RenderTexture src, RenderTexture dst)
        {
            base.render(src, dst);

            renderMat.SetFloat("_Offset", Power);

            RenderTexture tex = RenderTexture.GetTemporary(src.width / 4, src.height / 4, src.depth);
            // RenderTexture tex2 = RenderTexture.GetTemporary(src.width / 4, src.height / 4, src.depth);

            Graphics.Blit(src, tex, renderMat);
            Graphics.Blit(tex, src, renderMat);
            // Graphics.Blit(tex2, tex, renderMat);
            Graphics.Blit(src, dst, renderMat);

            tex.Release();
            // tex2.Release();
        }

    }

}


