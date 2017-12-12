using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
//using YoukiaUnity.App;
using YoukiaUnity.CinemaSystem;
using YoukiaUnity.Graphics;


namespace YoukiaUnity.Graphics
{
    public class CameraAnimation : MonoBehaviour

    {



        /// <summary>
        /// 遮罩颜色
        /// </summary>
        [HideInInspector]
        public Color MaskColor = new Color(0, 0, 0, 0);

        /// <summary>
        /// 亮度
        /// </summary>
        [HideInInspector]
        public float Exposure = 1;

        private GraphicsManager mgr;
        private Tweener anim;

        public eFadeState fadeState = eFadeState.NoStart;

        public enum eFadeState
        {
            NoStart,
            Fading,
            FadeEnd,
        }

        protected TweenCallback OnComplete;


        private void Awake()
        {
            if (mgr == null)
            {
//                mgr = YKApplication.Instance.GetManager<GraphicsManager>();
                mgr = GameObject.Find("GraphicsManager").GetComponent<GraphicsManager>();
            }
        }



        // Update is called once per frame
        public void AnimUpdate()
        {

            if (fadeState == eFadeState.NoStart)
            {
                Exposure = 0;
            }
            else if (fadeState == eFadeState.Fading)
            {
                setShader();
            }
        }



        private void setShader()
        {


            if (mgr != null)
            {
                mgr.SetMaskColor(MaskColor);
                mgr.SetExposure(Exposure);
            }

        }


        public void Black()
        {
            fadeState = eFadeState.FadeEnd;
            Exposure = 0;
            MaskColor = new Color(0, 0, 0, 0);
            setShader();
        }


        public void White()
        {
            fadeState = eFadeState.FadeEnd;
            Exposure = 0;
            MaskColor = new Color(1, 0, 0, 0);
            setShader();
        }



        public void OnSwitch(GraphicsManager.eCameraOpenState state)
        {
            switch (state)
            {

                case GraphicsManager.eCameraOpenState.AutoFadeIn:
                    mgr._mainCameraAnimation.SmoothFadeIn();
                    break;
                case GraphicsManager.eCameraOpenState.None:
                    mgr._mainCameraAnimation.Normal();
                    break;
                case GraphicsManager.eCameraOpenState.KeepBlack:
                    mgr._mainCameraAnimation.Black();
                    break;
                case GraphicsManager.eCameraOpenState.KeepWhite:
                    mgr._mainCameraAnimation.White();
                    break;

            }

        }

        public void Normal()
        {
            fadeState = eFadeState.FadeEnd;
            Exposure = 1;
            MaskColor = new Color(0, 0, 0, 0);
            setShader();
        }


        /// <summary>
        /// 渐进
        /// </summary>
        /// <param name="handle">完成后的回调</param>
        public void SmoothFadeIn(TweenCallback handle = null)
        {


            anim.Kill(true);
            fadeState = eFadeState.Fading;
            MaskColor = new Color(0, 0, 0, 0);
            Exposure = 0;
            setShader();


            anim = DOTween.To(() => Exposure, x => Exposure = x, 1, 0.5f).SetEase(Ease.InQuad).OnUpdate(() =>
            {
                setShader();
            }).OnComplete(() =>
          {
              fadeState = eFadeState.FadeEnd;

              if (handle != null)
              {

                  handle();
              }
          });


        }

        /// <summary>
        /// 渐出(变暗)
        /// </summary>
        /// <param name="time">淡出时间</param>
        /// <param name="handle">完成后的回调</param>
        public void SmoothFadeOut(float time = 0.5f, TweenCallback handle = null)
        {
            SmoothFadeOut(new Color(0, 0, 0, 0), time, handle);
        }

        /// <summary>
        /// 渐出(变亮)
        /// </summary>
        /// <param name="time">淡出时间</param>
        /// <param name="handle">完成后的回调</param>
        public void SmoothFadeOutLight(float time = 0.5f, TweenCallback handle = null)
        {
            SmoothFadeOut(new Color(1, 1, 1, 1), time, handle);
        }

        /// <summary>
        /// 渐出
        /// </summary>
        /// <param name="FadeOutColor">淡出颜色</param>
        /// <param name="time">淡出时间</param>
        /// <param name="handle">完成后的回调</param>
        public void SmoothFadeOut(Color FadeOutColor, float time = 0.5f, TweenCallback handle = null)
        {
            anim.Kill(true);

            fadeState = eFadeState.Fading;
            MaskColor = FadeOutColor;
            Exposure = 1;
            setShader();

            anim = DOTween.To(() => Exposure, x => Exposure = x, 0, time)
                .SetEase(Ease.InQuad)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    fadeState = eFadeState.FadeEnd;

                    if (handle != null)
                    {

                        handle();
                    }
                });


        }

    }

}

