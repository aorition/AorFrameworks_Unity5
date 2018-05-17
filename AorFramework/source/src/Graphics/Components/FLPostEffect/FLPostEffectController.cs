﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic
{
    /// <summary>
    /// 标准PostEffect控制器
    /// </summary>
    public class FLPostEffectController : MonoBehaviour
    {

        


        private static Material ReverseMat;
        private static void RtCopy(RenderTexture srcRt, RenderTexture tarRt, bool reverse = false)
        {
            if (reverse)
            {
                UnityEngine.Graphics.Blit(srcRt, tarRt, ReverseMat);
            }
            else
            {
                UnityEngine.Graphics.Blit(srcRt, tarRt);
            }
        }

        void OnEnable()
        {
            if (!ReverseMat)
            {
                Shader shader = ShaderBridge.Find("Hidden/PostEffect/Reverse");
                if (!shader)
                {
                    Log.Error("** FLPostEffectController.OnEnable Error :: Can not Find the Shader : Hidden/PostEffect/Reverse , create ReverseMat fail.");
                }
                ReverseMat = new Material(shader);
            }
        }

        RenderTexture newrt = null;
        RenderTexture last = null;
        RenderTexture org = null;

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            int count = GraphicsManager.Instance.peclist.Count;

            RenderTexture orgSrc = src;
//            IFLPostEffectComponent currentRender;

            if (count > 1)
            {
                //暂时不要深度
                org = RenderTexture.GetTemporary(src.width, src.height, 0);
                newrt = org;
                for (int i = 0; i < count; i++)
                {
                    if (GraphicsManager.Instance.peclist[i].IsActive)
                        GraphicsManager.Instance.peclist[i].RenderImage(orgSrc, newrt);

                    //switch
                    last = orgSrc;
                    orgSrc = newrt;
                    newrt = last;
                }
                UnityEngine.Graphics.Blit(orgSrc, dest);

                if (org != null)
                    RenderTexture.ReleaseTemporary(org);
            }
            else if (count == 1)
            {
                if (GraphicsManager.Instance.peclist[0].IsActive)
                    GraphicsManager.Instance.peclist[0].RenderImage(src, dest);

            }
            else
            {
                //just in case
                UnityEngine.Graphics.Blit(src, dest);
            }
        }


    }

}
