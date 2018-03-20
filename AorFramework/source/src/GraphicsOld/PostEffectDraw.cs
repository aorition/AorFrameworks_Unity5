using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YoukiaUnity.Graphics
{
    public class PostEffectDraw : MonoBehaviour
    {
        internal GraphicsManager.eCameraType CameraType;
        internal GraphicsManager Mgr;
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {

            switch (CameraType)
            {
                case GraphicsManager.eCameraType.Effect:
                    Mgr.DrawCard.OnEffectRenderAfter(ref source);
                    UnityEngine.Graphics.Blit(source, destination);
                    break;
                case GraphicsManager.eCameraType.PostEffect:
                    Mgr.DrawCard.OnPostEffectRenderAfter(ref source);
                    if (Mgr.EnableBloom)
                        UnityEngine.Graphics.Blit(source, destination, Mgr.RenderMaterial, 0);
                    else
                        UnityEngine.Graphics.Blit(source, destination);
                    break;
 
            }


            //   GraphicsManager.GetInstance().DrawCard.OnPostEffectRenderAfter(ref source);
            //  UnityEngine.Graphics.Blit(source, destination);
        }
    }
}