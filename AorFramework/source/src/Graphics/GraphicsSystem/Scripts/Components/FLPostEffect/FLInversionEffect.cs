﻿#pragma warning disable
using System;
using System.Collections;
using UnityEngine;

namespace Framework.Graphic.Effect
{

    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    public class FLInversionEffect : FLEffectBase
    {
        [Range(0, 1)]
        public float inversionPower = 1;
        public Color inversionColor = Color.white;
        [Range(0, 1)]
        public float grayAmount = 0;

        public string ScriptName
        {
            get { return "InversionEffect"; }
        }

        protected override void init()
        {
            base.init();
            if (renderMat == null)
            {
                renderMat = new Material(ShaderBridge.Find("Hidden/PostEffect/Inversion"));
            }
            //填充默认渲染等级值
            if (m_RenderLevel == 0) m_RenderLevel = 25;
        }
        
        protected override void render(RenderTexture src, RenderTexture dst)
        {
            base.render(src, dst);

            renderMat.SetFloat("_InversionPower", inversionPower);
            renderMat.SetColor("_InversionColor", inversionColor);
            renderMat.SetFloat("_GrayAmount", grayAmount);

            Graphics.Blit(src, dst, renderMat);


        }

    }

}


