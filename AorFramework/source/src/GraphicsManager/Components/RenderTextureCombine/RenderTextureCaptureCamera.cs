using System;
using System.Collections.Generic;
using Framework.Graphic.Utility;
using UnityEngine;

namespace Framework.Graphic
{
    public class RenderTextureCaptureCamera : MonoBehaviour, IGMPostEffectComponent
    {
        
        public RenderTextureCombine rtCombine;

        private Camera _target;
        private void Awake()
        {
            _target = GetComponent<Camera>();
            _target.enabled = false;
        }

        private void OnEnable()
        {
            GraphicsManager.RequestGraphicsManager(() =>
            {
                GraphicsManager.instance.AddPostEffectComponent(this);
            });
        }

        private void OnDisable()
        {
            if(GraphicsManager.instance)
                GraphicsManager.instance.RemovePostEffectComponent(this);
        }

        public void UpdateGMPostEffect()
        {
            if (!rtCombine.renderTexture)
            {
                //                rtCombine.renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
                rtCombine.renderTexture = RenderTextureUtility.New(RenderTextureUtility.GetNormalHDRFormatInfo(Screen.width, Screen.height));
                _target.targetTexture = rtCombine.renderTexture;
            }

            _target.Render();
        }
        
    }
}
