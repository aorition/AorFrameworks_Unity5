using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace YoukiaUnity.Graphics
{


    /// <summary>
    /// 绘制卡接口类
    /// </summary>
    public interface IGraphicDraw
    {
        //天空雾气渲染之后
        void OnSkyRenderAfter(RenderTexture mainRt);

        //半透明处理之后
        void OnPreEffectAfter(RenderTexture mainRt);

        void OnEffectRenderAfter(RenderTexture mainRt);

        void OnPostEffectRenderAfter(RenderTexture mainRt);

        Material GetMaterial(bool rttMode);
        void OnBlitEnd();
    }


}
