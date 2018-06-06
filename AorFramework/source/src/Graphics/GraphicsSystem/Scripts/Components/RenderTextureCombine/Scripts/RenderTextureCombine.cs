using System;
using Framework.Graphic.Utility;
using UnityEngine;

namespace Framework.Graphic
{
    public class RenderTextureCombine : MonoBehaviour
    {

        public enum PostEffectCombineType
        {
            Normal,
            Add,
            Multiply
        }

        public static RenderTextureCombine Create(GameObject go, PostEffectCombineType type)
        {

            RenderTextureCombine combine = go.GetComponent<RenderTextureCombine>();
            if (!combine) combine = go.AddComponent<RenderTextureCombine>();

            switch (type)
            {
                case PostEffectCombineType.Multiply:
                    combine.material = new Material(ShaderBridge.Find("Hidden/PEOverrideMultiply"));
                    break;
                case PostEffectCombineType.Add:
                    combine.material = new Material(ShaderBridge.Find("Hidden/PEOverrideAdd"));
                    break;
                case PostEffectCombineType.Normal:
                    combine.material = new Material(ShaderBridge.Find("Hidden/PEOverride"));
                    break;
            }

            return combine;
        }

        private void OnDestory()
        {
            if (renderTexture)
            {
                RenderTextureUtility.Release(renderTexture);
                renderTexture = null;
            }

            if (_temp)
            {
                RenderTextureUtility.Release(_temp);
                _temp = null;
            }

            if (material) material = null;
        }

        private bool _isDirty = false;

        public void SetDirty()
        {
            _isDirty = true;
        }

        public RenderTexture renderTexture;
        public Material material;

        public float Power = 1f;
//
//    [SerializeField]
//    private bool disposeRT;

        public RTPostEffectBase _RTPostEffectBase;

        private RenderTexture _temp;
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {

            if (!material)
            {
                material = new Material(Shader.Find("Hidden/PEOverride"));
            }

            if (renderTexture)
            {

                if (_isDirty)
                {
                    renderTexture.DiscardContents();
                    _isDirty = false;
                }

                if (_RTPostEffectBase && _RTPostEffectBase.enabled)
                {
                    if (!_temp)
                    {
                        _temp = RenderTextureUtility.New(RenderTextureUtility.GetNormalHDRFormatInfo(renderTexture.width, renderTexture.height));
                    }
                    _RTPostEffectBase.PostEffectDo(renderTexture, _temp);
                    material.SetTexture("_EffectTex", _temp);
                    material.SetFloat("_Power", Power);
                    Graphics.Blit(src, dest, material);
                }
                else
                {
                    material.SetTexture("_EffectTex", renderTexture);
                    material.SetFloat("_Power", Power);
                    Graphics.Blit(src, dest, material);
                }
            }
            else
            {
                Graphics.Blit(src, dest);
            }
        }
    }
}