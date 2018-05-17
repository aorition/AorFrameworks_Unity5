using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic
{
    /// <summary>
    /// 标准PostEffect组件接口
    /// </summary>
    public interface IFLPostEffectComponent
    {

        string ScriptName { get; }
        int Level { get; }
        void RenderImage(RenderTexture src, RenderTexture dest);
        bool IsActive { get; }
    }
}
