using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using YoukiaUnity.App;

namespace YoukiaUnity.Graphics
{

    public interface IPostEffect
    {

        void Render(ref RenderTexture SrcRT, ref RenderTexture DstRT);
    }

    /// <summary>
    /// 一个基本的绘制卡,图形管理器通过自定义的绘制卡来实现各种效果
    /// </summary>
    public class BaseDrawCard : IGraphicDraw
    {
        public List<IPostEffect> PostEffects = new List<IPostEffect>();

        protected Material mat;
        //uv上下反转材质球
        protected Material reverseMat
        {
            get
            {
                if (_reverseMat == null)
                    _reverseMat = new Material(Shader.Find("Hidden/PostEffect/reverse"));

                return _reverseMat;
            }

        }

        private Material _reverseMat;
        private PostEffectDraw _postEffectDraw;

        /// <summary>
        /// OnRenderImage后期脚本,用在非RTT模式(unity经典模式)
        /// </summary>
        internal PostEffectDraw postEffectDraw
        {

            get
            {

                if (_postEffectDraw == null)
                    _postEffectDraw = GraphicsManager.GetInstance().PostEffectCamera.gameObject.GetComponent<PostEffectDraw>();

                return _postEffectDraw;
            }
        }

        /// <summary>
        /// OnRenderImage后期脚本,用在非RTT模式(unity经典模式)
        /// </summary>
        private PostEffectDraw _effectDraw;

        internal PostEffectDraw effectDraw
        {

            get
            {

                if (_effectDraw == null)
                    _effectDraw = GraphicsManager.GetInstance().EffectCamera.gameObject.GetComponent<PostEffectDraw>();

                return _effectDraw;
            }
        }

        public BaseDrawCard()
        {
            //    Shader = YKApplication.Instance.ConstShader["Hidden/PostEffect/DrawShader"];
        }

        public void AddEffect(IPostEffect postEffect)
        {

            if (!GraphicsManager.GetInstance().EnablePostEffect)
                return;

            if (!PostEffects.Contains(postEffect))
                PostEffects.Add(postEffect);



            //默认模式通过添加脚本实现onRenderImage
            setPostEffectDraw(true);
        }

        protected virtual void setPostEffectDraw(bool enable)
        {
            if (GraphicsManager.GetInstance().EnableBloom)
                postEffectDraw.enabled = true;
            else
                postEffectDraw.enabled = enable;
        }

        protected virtual void setEffectDraw(bool enable)
        {
            effectDraw.enabled = enable;

        }

        public virtual void OnBlitEnd()
        {

        }

        public virtual void OnEffectRenderAfter(ref RenderTexture mainRt)
        {

        }

        void setMat()
        {

            mat = new Material(Shader.Find("Hidden/PostEffect/DrawShader"));

            mat.SetTexture("_CurveTex", Texture2D.blackTexture);

        }

        public virtual Material GetMaterial()
        {

            if (mat == null)
            {

                setMat();
            }

            return mat;
        }

        public virtual void OnEffectRenderBefore(ref RenderTexture mainRt)
        {
            //throw new NotImplementedException();
        }

        public virtual void OnSkyRenderAfter(ref RenderTexture mainRt)
        {
            //  throw new NotImplementedException();
        }

        public virtual void OnPreEffectAfter(ref RenderTexture mainRt)
        {
            //  throw new NotImplementedException();
        }

        private HashSet<Type> renderDic = new HashSet<Type>();
        public virtual void OnPostEffectRenderAfter(ref RenderTexture mainRt)
        {



            if (PostEffects.Count > 0)
            {
                int size = (int)GraphicsManager.GetInstance().PostEffectRenderSize;
                RenderTexture rt = RenderTexture.GetTemporary(mainRt.width / size, mainRt.height / size, 0, RenderTextureFormat.ARGB32);
                
                copy(ref mainRt, ref rt);

                for (int i = 0; i < PostEffects.Count; i++)
                {

                    Type t = PostEffects[i].GetType();
                    if (!renderDic.Contains(t))
                    {
                        renderDic.Add(t);
                    }
                    else
                    {
                        continue;
                    }

                    if (PostEffects[i] != null)
                        PostEffects[i].Render(ref rt, ref mainRt);

                    //多个混合效果要把画面留下重用.不根据平台反转的话,PC上两个会导致画面上下颠倒(可用来检测效率)
                    if (PostEffects.Count > 1)
                        copy(ref mainRt, ref rt);
                    // UnityEngine.Graphics.Blit(mainRt, rt);

                }

                renderDic.Clear();

                //rt.Release(); 
                RenderTexture.ReleaseTemporary(rt);

            }
            else
            {
                setPostEffectDraw(false);
            }



        }


        void copy(ref RenderTexture mainRt, ref RenderTexture rt)
        {

#if UNITY_5_6_OR_NEWER
            //5.6 解决了编辑器上混合颠倒的问题
            UnityEngine.Graphics.Blit(mainRt, rt);
#else
            //默认后期模式需要区分平台,不然上下颠倒
            if (Application.isEditor && Application.platform==RuntimePlatform.WindowsEditor)
            {
            
                UnityEngine.Graphics.Blit(mainRt, rt, reverseMat);
            }
            else
            {
                UnityEngine.Graphics.Blit(mainRt, rt);
            }
#endif
        }

        public virtual void OnFinalRenderAfter(ref RenderTexture mainRt)
        {
            //    throw new NotImplementedException();
        }

        public virtual void OnSettingUpdate()
        {
            // throw new NotImplementedException();
        }
    }
}
