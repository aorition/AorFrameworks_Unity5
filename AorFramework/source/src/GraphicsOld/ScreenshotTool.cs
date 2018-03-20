using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace YoukiaUnity.Graphics
{
    /// <summary>
    /// 截屏工具,添加到最后的摄像机上,Callback中取得rt
    /// </summary>
    public class ScreenshotTool : MonoBehaviour
    {
        //private RenderTexture texture;
        public Action<RenderTexture> Callback;
        //public RenderTexture GetScreenshot()
        //{
        //    return texture;
        //}

        public bool AutoDestroy = true;

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            //if (texture == null)
            //    texture = RenderTexture.GetTemporary(source.width, source.height);

            //if (GraphicsManager.GetInstance().EnableRTTMode)
            //    UnityEngine.Graphics.Blit(GraphicsManager.GetInstance().MainRt, texture);
            //else
            //    UnityEngine.Graphics.Blit(source, texture);

            if (Callback != null)
                Callback(source);


            Callback = null;

            if (AutoDestroy)
                Destroy(this);
        }

    }


}
