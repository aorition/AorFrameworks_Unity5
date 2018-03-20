using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoukiaUnity.Graphics;

public class InversionEffect : GraphicsLauncher, IPostEffect
{
    [Range(0,1)]
    public float inversionPower = 1;
    public Color inversionColor = Color.white;
    [Range(0, 1)]
    public float grayAmount = 0;

    private Material renderMat;
    private bool isInit;


    protected override void AfterLauncher()
    {
        isInit = true;

    }


    void OnEnable()
    {
        if (isInit)
        {
            addEffect();
        }
        else
        {
            GraphicsLauncher.StartAfterGraphicMgr(this, addEffect);
        }

    }

    void addEffect()
    {
        if (renderMat == null)
        {
            renderMat = new Material(Shader.Find("Hidden/PostEffect/inversion"));

        }


        if (GraphicsManager.GetInstance() != null && GraphicsManager.GetInstance().DrawCard != null)
        {


            (GraphicsManager.GetInstance().DrawCard as BaseDrawCard).AddEffect(this);



        }
    }

    void OnDisable()
    {
        if (GraphicsManager.GetInstance() != null && GraphicsManager.GetInstance().DrawCard != null)
        {
            (GraphicsManager.GetInstance().DrawCard as BaseDrawCard).PostEffects.Remove(this);
        }
        if(renderMat)
            DestroyImmediate(renderMat);
    }


    public void Render(ref RenderTexture SrcRT,ref RenderTexture DstRT)
    {
        if (renderMat == null || renderMat.shader == null)
            return;

        renderMat.SetFloat("_InversionPower", inversionPower);
        renderMat.SetColor("_InversionColor", inversionColor);
        renderMat.SetFloat("_GrayAmount", grayAmount);

        Graphics.Blit(SrcRT, DstRT, renderMat);


    }
 
}
