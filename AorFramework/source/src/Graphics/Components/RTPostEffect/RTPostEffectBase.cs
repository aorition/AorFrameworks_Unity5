using System;
using UnityEngine;

namespace Framework.Graphic
{

    /// <summary>
    /// 后处理效果基类. 依赖RenderTextureCombine脚本在同一个GameObject上.
    /// </summary>
    public class RTPostEffectBase : MonoBehaviour
    {

        [SerializeField]
        protected RenderTextureCombine _RTcombine;

        protected virtual void Awake()
        {

            if (!_RTcombine)
            {
                _RTcombine = GetComponent<RenderTextureCombine>();
            }

            if (_RTcombine)
            {
                _RTcombine._RTPostEffectBase = this;
            }
        }
        
        /// <summary>
        /// 后处理主函数
        /// </summary>
        /// <param name="src">RT输入</param>
        /// <param name="dest">RT输出</param>
        public virtual void PostEffectDo(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest);
        }

    }
}
