#pragma warning disable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Graphic.Utility;


namespace Framework.Graphic.Effect
{

    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    public class FLGaussBlurEffect : FLEffectBase
    {

        public static void CreateGaussImage(Texture src, ref RenderTexture dst, float power = 1f)
        {
            if (!dst || !src)
                return;

            Material mat = new Material(ShaderBridge.Find("Hidden/PostEffect/GaussBlur"));

            mat.SetFloat("_Offset", power * 0.01f);

            //RenderTexture tex = RenderTexture.GetTemporary(src.width / 4, src.height / 4, 0);
            RenderTexture tex = RenderTextureUtility.New(src.width / 4, src.height / 4, 0);
            //RenderTexture tex2 = RenderTexture.GetTemporary(src.width / 4, src.height / 4, 0);
            RenderTexture tex2 = RenderTextureUtility.New(src.width / 4, src.height / 4, 0);

            Graphics.Blit(src, tex, mat);
            Graphics.Blit(tex, tex2, mat);
            Graphics.Blit(tex2, tex, mat);
            Graphics.Blit(tex, dst, mat);

            //tex.Release();
            RenderTextureUtility.Release(tex);
            //tex2.Release();
            RenderTextureUtility.Release(tex2);
            Destroy(mat);

        }

        //-------------------------------------------------

        public float Power = 1f;

        protected override void init()
        {
            base.init();
            //填充后处理用Shader&Material
            if (renderMat == null)
            {
                renderMat = new Material(ShaderBridge.Find("Hidden/PostEffect/GaussBlur"));
            }
            //填充默认渲染等级值
            if (m_RenderLevel == 0) m_RenderLevel = 50;
        }

        private RenderTexture _temp;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_temp)
            {
                RenderTextureUtility.Release(_temp);
                _temp = null;
            }
        }

        protected override void render(RenderTexture src, RenderTexture dst)
        {
            base.render(src, dst);

            renderMat.SetFloat("_Offset", Power * 0.01f);
            
            if (!_temp)
            {
                //_temp = RenderTexture.GetTemporary(src.width / 4, src.height / 4, src.depth);
                _temp = RenderTextureUtility.New(src.width / 4, src.height / 4, src.depth);
            }
            Graphics.Blit(src, _temp, renderMat);
            Graphics.Blit(_temp, src, renderMat);
            // Graphics.Blit(tex2, tex, renderMat);
            Graphics.Blit(src, dst, renderMat);

            //_temp.Release();
            // tex2.Release();
        }

    }

}


