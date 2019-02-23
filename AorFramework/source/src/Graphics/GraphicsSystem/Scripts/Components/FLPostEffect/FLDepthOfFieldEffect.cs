#pragma warning disable
using Framework.Graphic.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic.Effect
{
    [ExecuteInEditMode]
    public class FLDepthOfFieldEffect : FLEffectBase
    {

        public string ScriptName
        {
            get { return "DepthOfField"; }
        }

        //[Range(0.0f, 100.0f)]
        public float focalDistance = 10.0f;
        //[Range(0.0f, 100.0f)]
        public float nearBlurScale = 25.0f;
        //[Range(0.0f, 1000.0f)]
        public float farBlurScale = 25.0f;
        //分辨率  
        public int downSample = 2;
        //采样率  
        public int samplerScale = 2;

        public Transform FocalDistanceTarget;

        private Camera _mainCam = null;
        public Camera MainCam
        {
            get
            {
                if (!_mainCam)
                    _mainCam = GetComponent<Camera>();

                if (Application.isPlaying)
                {
                    if(_mainCam && _mainCam.enabled)
                    {
                        return _mainCam;
                    }else if (GraphicsManager.IsInit())
                    {
                        return GraphicsManager.Instance.MainCamera;
                    }
                }

                return _mainCam;
            }
        }

        protected override void init()
        {
            base.init();
            //填充后处理用Shader&Material
            if (renderMat == null)
            {
                Shader shader = ShaderBridge.Find("Hidden/PostEffect/DepthOfField");
                if (shader)
                    renderMat = new Material(shader);
                else
                    Debug.LogError("** FLDepthOfFieldEffect.init() Error :: Can not find \"Hidden/PostEffect/DepthOfField\" Shader .");
            }
            //填充默认渲染等级值
            if (m_RenderLevel == 0) m_RenderLevel = 125;

            //maincam的depthTextureMode是通过位运算开启与关闭的  
            MainCam.depthTextureMode |= DepthTextureMode.Depth;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            MainCam.depthTextureMode &= ~DepthTextureMode.Depth;
        }

        private void Update()
        {
            if(MainCam && FocalDistanceTarget)
            {
                focalDistance = Vector3.Distance(FocalDistanceTarget.position, MainCam.transform.position);
            }
        }

        protected override void render(RenderTexture src, RenderTexture dst)
        {
            base.render(src, dst);

            if (renderMat && MainCam)
            {
                //首先将我们设置的焦点限制在远近裁剪面之间  
                Mathf.Clamp(focalDistance, MainCam.nearClipPlane, MainCam.farClipPlane);

                //申请两块RT，并且分辨率按照downSameple降低  
                RenderTexture temp1 = RenderTexture.GetTemporary(src.width >> downSample, src.height >> downSample, 0, src.format);
                RenderTexture temp2 = RenderTexture.GetTemporary(src.width >> downSample, src.height >> downSample, 0, src.format);

                //直接将场景图拷贝到低分辨率的RT上达到降分辨率的效果  
                Graphics.Blit(src, temp1);

                //高斯模糊，两次模糊，横向纵向，使用pass0进行高斯模糊  
                renderMat.SetVector("_offsets", new Vector4(0, samplerScale, 0, 0));
                Graphics.Blit(temp1, temp2, renderMat, 0);
                renderMat.SetVector("_offsets", new Vector4(samplerScale, 0, 0, 0));
                Graphics.Blit(temp2, temp1, renderMat, 0);

                //景深操作，景深需要两的模糊效果图我们通过_BlurTex变量传入shader  
                renderMat.SetTexture("_BlurTex", temp1);
                //设置shader的参数，主要是焦点和远近模糊的权重，权重可以控制插值时使用模糊图片的权重  
                renderMat.SetFloat("_focalDistance", FocalDistance01(focalDistance));
                renderMat.SetFloat("_nearBlurScale", nearBlurScale);
                renderMat.SetFloat("_farBlurScale", farBlurScale);

                //使用pass1进行景深效果计算，清晰场景图直接从src输入到shader的_MainTex中  
                Graphics.Blit(src, dst, renderMat, 1);

                //释放申请的RT  
                RenderTexture.ReleaseTemporary(temp1);
                RenderTexture.ReleaseTemporary(temp2);
            }
            else
            {
                Graphics.Blit(src, dst);
            }

        }

        //计算设置的焦点被转换到01空间中的距离，以便shader中通过这个01空间的焦点距离与depth比较  
        private float FocalDistance01(float distance)
        {
            return MainCam.WorldToViewportPoint((distance - MainCam.nearClipPlane) * MainCam.transform.forward + MainCam.transform.position).z / (MainCam.farClipPlane - MainCam.nearClipPlane);
        }

    }
}


