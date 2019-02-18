#pragma warning disable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic
{
    /// <summary>
    /// 标准PostEffect控制器
    /// </summary>
    [AddComponentMenu("")] //禁止此脚本放入ComponentMenu中
    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    public class FLPostEffectController : MonoBehaviour
    {
        
        private RenderTexture newrt = null;
        private RenderTexture last = null;
        private RenderTexture org = null;

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            int count = GraphicsManager.Instance.m_peclist.Count;

            RenderTexture orgSrc = src;
//            IFLPostEffectComponent currentRender;

            if (count > 1)
            {
                //暂时不要深度
                org = RenderTexture.GetTemporary(src.width, src.height, 0);
                newrt = org;
                for (int i = 0; i < count; i++)
                {
                    if (GraphicsManager.Instance.m_peclist[i].IsActive)
                        GraphicsManager.Instance.m_peclist[i].RenderImage(orgSrc, newrt);

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
                if (GraphicsManager.Instance.m_peclist[0].IsActive)
                    GraphicsManager.Instance.m_peclist[0].RenderImage(src, dest);

            }
            else
            {
                //just in case
                UnityEngine.Graphics.Blit(src, dest);
            }
        }


    }

}
