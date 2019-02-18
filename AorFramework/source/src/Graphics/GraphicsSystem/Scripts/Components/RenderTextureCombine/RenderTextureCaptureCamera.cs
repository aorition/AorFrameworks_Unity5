using System;
using System.Collections.Generic;
using Framework.Graphic.Utility;
using UnityEngine;

namespace Framework.Graphic
{
    /// <summary>
    /// 基于IRTPostEffectComponent
    /// </summary>
    public class RenderTextureCaptureCamera : MonoBehaviour, IRTPostEffectComponent
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
            GraphicsManager.Request(() =>
            {
                GraphicsManager.Instance.AddPostEffectComponent(this);
            });
        }

        private void OnDisable()
        {
            if(GraphicsManager.IsInit())
                GraphicsManager.Instance.RemovePostEffectComponent(this);
        }

        public void UpdateGMPostEffect()
        {
            if (!rtCombine.renderTexture)
            {
                //                rtCombine.renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
                rtCombine.renderTexture = RenderTextureUtility.New(Screen.width, Screen.height, (int)RenderTextureUtility.DepthDef.NoDepth);
                _target.targetTexture = rtCombine.renderTexture;
            }
            _target.Render();
        }
        
    }
}
