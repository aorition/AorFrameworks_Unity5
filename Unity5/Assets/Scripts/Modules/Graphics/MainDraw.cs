using UnityEngine;
//using YoukiaUnity.App;
//using YoukiaUnity.GUI.AorUI;


namespace YoukiaUnity.Graphics
{
    /// <summary>
    /// 显示器绘制类
    /// </summary>
    public class MainDraw : MonoBehaviour
    {



        public Material lastMaterial
        {

            get
            {
                //                return YKApplication.Instance.GetManager<GraphicsManager>().RenderMaterial;
                if (mgr != null)
                {
                    return mgr.RenderMaterial;
                }
                return null;
            }

        }
        public int DrawPassCount = 0;
        public Material blackMaterial;
        private GraphicsManager mgr;

        /// <summary>
        /// 初始化方法
        /// </summary>
        public void Init()
        {

            //Unity 5 不在提供通过字符串创建Material的构造了.
            //            blackMaterial = new Material("Shader \"Hidden/Alpha\" {" + "SubShader {" + " Pass {" +
            //                 " ZTest Always Cull Off ZWrite Off" + " Blend One Zero" + " Color (0,0,0,1)" + " }}}");

            blackMaterial = new Material(Shader.Find("Hidden/Alpha"));

            SetMaskColor(new Color(0, 0, 0, 1));
            SetExposure(1);

            //             mgr = YKApplication.Instance.GetManager<GraphicsManager>();
            mgr = GameObject.Find("GraphicsManager").GetComponent<GraphicsManager>();

        }


        /// <summary>
        /// 设置遮盖颜色
        /// </summary>
        /// <param name="color">遮盖颜色</param>
        public void SetMaskColor(Color color)
        {
            if (lastMaterial != null)
            {
                lastMaterial.SetColor("_MaskColor", color);
            }

        }

        /// <summary>
        /// 设置曝光度
        /// </summary>
        /// <param name="exp">曝光度</param>
        public void SetExposure(float exp)
        {
            if (lastMaterial != null)
            {
                lastMaterial.SetFloat("_Exposure", exp);
            }

        }


        void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
        {


            if (mgr == null || mgr.MainRt == null)
            {
                //UnityEngine.Graphics.Blit(null, destTexture, blackMaterial);
                return;
            }

            if (mgr.CurrentSubCamera == null)
            {
                UnityEngine.Graphics.Blit(null, destTexture, blackMaterial);
                return;
            }

            UnityEngine.Graphics.SetRenderTarget(mgr.MainRt);

            if (mgr != null && GraphicsManager.isInited && mgr.MainCamera != null)
            {

                if (mgr.StopRender)
                {
                    UnityEngine.Graphics.Blit(mgr.MainRt, destTexture, lastMaterial);
                    return;
                }


                mgr.MainCamera.Render();
                mgr.ShadowMgr.ShadowUpdate();

                if (mgr.CharacterCamera != null)
                    mgr.CharacterCamera.Render();

                if (mgr.SkyCamera != null)
                    mgr.SkyCamera.Render();

                //雾效之后
                mgr.DrawCard.OnSkyRenderAfter(mgr.MainRt);

                //半透明物体延迟到这里处理
                if (mgr.PreEffectCamera != null)
                    mgr.PreEffectCamera.Render();

                mgr.DrawCard.OnPreEffectAfter(mgr.MainRt);

                //正常特效
                if (mgr.EffectCamera != null)
                    mgr.EffectCamera.Render();

                mgr.DrawCard.OnEffectRenderAfter(mgr.MainRt);

                //需要空气扭曲的特效
                if (mgr.PostEffectCamera != null)
                    mgr.PostEffectCamera.Render();


                mgr.DrawCard.OnPostEffectRenderAfter(mgr.MainRt);



                if (mgr.UIimage != null)
                {
                    //绘制到UI RT模式提前结束,后期在RawImage的shader中进行
                    return;
                }

                //  UnityEngine.Graphics.Blit(mgr.MainRt, destTexture, lastMaterial);
                if (lastMaterial != null)
                    lastMaterial.SetTexture("_MainTex", mgr.MainRt);

                UnityEngine.Graphics.SetRenderTarget(null);
                //绘制到屏幕
                GL.PushMatrix();
                lastMaterial.SetPass(DrawPassCount);
                GL.LoadOrtho();
                GL.Begin(GL.QUADS);
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(0.0f, 1.0f, 0.1f);
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, 0.1f);
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(1.0f, 0.0f, 0.1f);
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(0.0f, 0.0f, 0.1f);
                GL.End();
                GL.PopMatrix();


                mgr.DrawCard.OnBlitEnd();
            }


        }




    }


}


