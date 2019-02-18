#pragma warning disable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic.Effect
{
    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    public class FLRadialBlurEffect : FLEffectBase
    {

        public int Power = 10;
        public Transform FocusTarget;
        public Vector3 FocusPos = Vector3.zero;
        protected override void init()
        {
            base.init();
            if (renderMat == null)
            {
                FocusPos = transform.position;
                renderMat = new Material(ShaderBridge.Find("Hidden/PostEffect/RadialBlur"));
            }
            //填充默认渲染等级值
            if (m_RenderLevel == 0) m_RenderLevel = 75;
        }

        protected override void render(RenderTexture src, RenderTexture dst)
        {
            base.render(src, dst);

            renderMat.SetFloat("_Level", Power);

            Vector3 pos = _CulScreenPoint(FocusTarget ? FocusTarget.position : FocusPos);

            renderMat.SetFloat("_CenterX", pos.x / Screen.width);
            renderMat.SetFloat("_CenterY", pos.y / Screen.height);

            Graphics.Blit(src, dst, renderMat);

        }

        private Vector3 _CulScreenPoint(Vector3 pos)
        {
            if(GraphicsManager.IsInit()) return GraphicsManager.Instance.MainCamera.WorldToScreenPoint(pos);
            if(Camera.main) return Camera.main.WorldToScreenPoint(pos);
            if(Camera.current) return Camera.current.WorldToScreenPoint(pos);
            return new Vector3(Screen.width / 2, Screen.height / 2, 0);
        }

    }
}


