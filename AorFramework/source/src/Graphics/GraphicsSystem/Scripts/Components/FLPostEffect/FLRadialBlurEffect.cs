using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using Framework.Graphic;
using UnityEngine;


public class FLRadialBlurEffect : FLEffectBase
{

    public int Power = 10;
    public Vector3 targetPos=Vector3.zero;
    protected override void init()
    {
        base.init();
        if (renderMat == null)
        {
            targetPos = transform.position;
            renderMat = new Material(ShaderBridge.Find("Hidden/PostEffect/RadiaBlur"));

        }
    }

    protected override void render(RenderTexture src, RenderTexture dst)
    {
        base.render(src, dst);

        renderMat.SetFloat("_Level", Power);

        Vector3 pos = GraphicsManager.Instance.MainCamera.WorldToScreenPoint(targetPos);

        renderMat.SetFloat("_CenterX", pos.x / Screen.width);
        renderMat.SetFloat("_CenterY", pos.y / Screen.height);

        Graphics.Blit(src, dst, renderMat);

    }

}
