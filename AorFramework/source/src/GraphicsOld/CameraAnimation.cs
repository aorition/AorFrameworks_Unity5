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

        public enum FadeColorType
        {
            Dark,
            Light,
        }

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
//            Debug.Log("********************* Black *************************");
//            GlobalEvent.dispatch(eEventName.BlackScreenFadeOut, false, 0f);
//            if (anim != null) anim.Kill(true);
//            fadeState = eFadeState.FadeEnd;
//            Exposure = 0;
//            MaskColor = new Color(0, 0, 0, 0);
//            setShader();
        }


        public void White()
        {
//            Debug.Log("********************* White *************************");
//            GlobalEvent.dispatch(eEventName.BlackScreenFadeOut, true, 0f);
            //            if (anim != null) anim.Kill(true);
            //            fadeState = eFadeState.FadeEnd;
            //            Exposure = 1;
            //            MaskColor = new Color(1, 1, 1, 1);
            //            setShader();
        }

        public void Normal()
        {
//            Debug.Log("********************* Normal *************************");
//            GlobalEvent.dispatch(eEventName.BlackScreenFadeIN, true, 0f);
            if (anim != null) anim.Kill(true);
            fadeState = eFadeState.FadeEnd;
            Exposure = 1;
            MaskColor = new Color(0, 0, 0, 0);
            setShader();
        }


        public void OnSwitch(GraphicsManager.eCameraOpenState state)
        {
            switch (state)
            {

                case GraphicsManager.eCameraOpenState.AutoFadeIn:
                    mgr._mainCameraAnimation.SmoothFadeIn(FadeColorType.Dark);
                    break;
                    //
                case GraphicsManager.eCameraOpenState.KeepNormal:
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

        

        /// <summary>
        /// 渐入
        /// </summary>
        /// <param name="handle">完成后的回调</param>
        public void SmoothFadeIn(FadeColorType colorType, float time = 0.5f, TweenCallback handle = null)
        {

//            if (Main.inst != null && GameSetModel.inst.optimum == 1)
//            {
//                Debug.Log("********************* eEventName.BlackScreenFadeIN : time = " + time + " *************************");
//                GlobalEvent.dispatch(eEventName.BlackScreenFadeIN, (colorType == FadeColorType.Light), time, handle);
                return;
//            }
//
//            anim.Kill(true);
//
//            if (colorType == FadeColorType.Dark)
//            {
//                
//                fadeState = eFadeState.Fading;
//                MaskColor = Color.black;
//                Exposure = 0;
//                setShader();
//
//                anim = DOTween.To(() => Exposure, x => Exposure = x, 1, time).SetEase(Ease.InQuad).OnUpdate(() =>
//                {
//                    setShader();
//                }).OnComplete(() =>
//                {
//                    fadeState = eFadeState.FadeEnd;
//                    if (handle != null)
//                    {
//                        handle();
//                    }
//                });
//
//            }
//            else
//            {
//                
//                fadeState = eFadeState.Fading;
//                MaskColor = Color.white;
//                Exposure = 1;
//                float tc = 1f;
//                setShader();
//
//                anim = DOTween.To(() => tc, x => tc = x, 0, time).SetEase(Ease.InQuad).OnUpdate(() =>
//                {
//                    MaskColor = new Color(tc, tc, tc, 1);
//                    setShader();
//                }).OnComplete(() =>
//                {
//                    fadeState = eFadeState.FadeEnd;
//                    if (handle != null)
//                    {
//                        handle();
//                    }
//                });
//
//            }

        }

        /// <summary>
        /// 渐出
        /// </summary>
        /// <param name="FadeOutColor">淡出颜色</param>
        /// <param name="time">淡出时间</param>
        /// <param name="handle">完成后的回调</param>
        public void SmoothFadeOut(FadeColorType colorType, float time = 0.5f, TweenCallback handle = null)
        {
//
//            if (Main.inst != null && GameSetModel.inst.optimum == 1)
//            {
//                Debug.Log("********************* eEventName.BlackScreenFadeOut : time = "+ time + " *************************");
//                GlobalEvent.dispatch(eEventName.BlackScreenFadeOut, (colorType == FadeColorType.Light), time, handle);
                return;
//            }

//            anim.Kill(true);
//
//            if (colorType == FadeColorType.Dark)
//            {
//
//                fadeState = eFadeState.Fading;
//                MaskColor = Color.black;
//                Exposure = 1;
//                setShader();
//
//                anim = DOTween.To(() => Exposure, x => Exposure = x, 0, time)
//                .SetEase(Ease.InQuad)
//                .OnComplete(() =>
//                {
//                    fadeState = eFadeState.FadeEnd;
//                    if (handle != null)
//                    {
//                        handle();
//                    }
//                });
//
//            }
//            else
//            {
//
//                fadeState = eFadeState.Fading;
//                MaskColor = Color.black;
//                float tc = 0;
//                Exposure = 1;
//                setShader();
//
//                anim = DOTween.To(() => tc, x => tc = x, 1, time)
//                .SetEase(Ease.InQuad)
//                .OnUpdate(() =>
//                {
//                    MaskColor = new Color(tc, tc, tc, 1);
//                    setShader();
//                }).OnComplete(() =>
//                {
//                    fadeState = eFadeState.FadeEnd;
//                    if (handle != null)
//                    {
//                        handle();
//                    }
//                });
//
//            }
        }

    }

}

