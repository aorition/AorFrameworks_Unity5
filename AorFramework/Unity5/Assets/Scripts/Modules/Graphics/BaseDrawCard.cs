using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using YoukiaUnity.App;

namespace YoukiaUnity.Graphics
{

    /// <summary>
    /// 一个基本的绘制卡,图形管理器通过自定义的绘制卡来实现各种效果
    /// </summary>
    public class BaseDrawCard : IGraphicDraw
    {

        protected Material mat;

        private bool _RttMode;

        public BaseDrawCard()
        {
            //    Shader = YKApplication.Instance.ConstShader["Hidden/PostEffect/DrawShader"];
        }

        public virtual void OnBlitEnd()
        {

        }

        public virtual void OnEffectRenderAfter(RenderTexture mainRt)
        {

        }

        void setMat(bool rttMode)
        {
            if (rttMode == false)
            {
//                mat = new Material(YKApplication.Instance.ConstShader["Hidden/PostEffect/DrawShader"]);
                mat = new Material(Shader.Find("Hidden/PostEffect/DrawShader"));
            }
            else
            {
//                mat = new Material(YKApplication.Instance.ConstShader["Hidden/PostEffect/UIDrawShader"]);
                mat = new Material(Shader.Find("Hidden/PostEffect/UIDrawShader"));
            }

            mat.SetTexture("_CurveTex",Texture2D.blackTexture);

        }

        public virtual Material GetMaterial(bool rttMode)
        {

            if (mat == null)
            {

                setMat(rttMode);
            }
            else
            {
                //已有材质的情况下mode改变才new mat
                if (_RttMode != rttMode)
                {
                    setMat(rttMode);
                }

            }
            _RttMode = rttMode;
            return mat;
        }

        public virtual void OnEffectRenderBefore(RenderTexture mainRt)
        {
           //throw new NotImplementedException();
        }

        public virtual void OnSkyRenderAfter(RenderTexture mainRt)
        {
          //  throw new NotImplementedException();
        }

        public virtual void OnPreEffectAfter(RenderTexture mainRt)
        {
          //  throw new NotImplementedException();
        }

        public virtual void OnPostEffectRenderAfter(RenderTexture mainRt)
        {
            //throw new NotImplementedException();
        }
    }
}
