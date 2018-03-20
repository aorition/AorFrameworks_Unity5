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
        void OnSkyRenderAfter(ref RenderTexture mainRt);

        //半透明处理之后
        void OnPreEffectAfter(ref RenderTexture mainRt);

        void OnEffectRenderAfter(ref RenderTexture mainRt);

        void OnPostEffectRenderAfter(ref RenderTexture mainRt);

        void OnFinalRenderAfter(ref RenderTexture mainRt);
        Material GetMaterial( );
        void OnSettingUpdate();
        void OnBlitEnd();
    }


}
