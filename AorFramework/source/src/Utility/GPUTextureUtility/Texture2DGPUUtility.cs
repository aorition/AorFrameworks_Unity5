using System;
using System.Collections;
using UnityEngine;

public class Texture2DGPUUtility
{


    public static void Process(RenderTexture target, Material mat, Action<Texture2D> finishCallback) {

        GPUTextureUtils.Instacne.StartCoroutine(doRt2T2dProcess(target, mat, finishCallback));

    }
    
    private static IEnumerator doRt2T2dProcess(RenderTexture target, Material mat, Action<Texture2D> finishCallback) {

        GPUTextureUtils.Process(target, mat);
        yield return new WaitForEndOfFrame();

        if(finishCallback != null)
        {
            int w = target.width;
            int h = target.height;
            Texture2D t2d = new Texture2D(w, h, TextureFormat.RGBA32, true);
            RenderTexture.active = target;
            t2d.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            t2d.Apply(true);

            finishCallback(t2d);
        }

    }


}