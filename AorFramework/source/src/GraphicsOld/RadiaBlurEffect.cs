using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoukiaUnity.Graphics;

public class RadiaBlurEffect : GraphicsLauncher, IPostEffect
{
    public float Level = 50; //力度
    private bool isInit;

    private Material renderMat;

    protected override void AfterLauncher()
    {
        isInit = true;
 
    }

    void addEffect()
    {
        if (renderMat == null)
        {
            renderMat = new Material(Shader.Find("Hidden/PostEffect/RadiaBlur"));

        }


        if (GraphicsManager.GetInstance() != null && GraphicsManager.GetInstance().DrawCard != null)
        {


            (GraphicsManager.GetInstance().DrawCard as BaseDrawCard).AddEffect(this);



        }
    }

    void OnEnable()
    {
        if (isInit)
        {
            addEffect();
        }
        else
        {
            GraphicsLauncher.StartAfterGraphicMgr(this,addEffect);
        }




    }



    void OnDisable()
    {
        if (GraphicsManager.GetInstance() != null && GraphicsManager.GetInstance().DrawCard != null)
        {


            (GraphicsManager.GetInstance().DrawCard as BaseDrawCard).PostEffects.Remove(this);



        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
 

    public void Render(ref RenderTexture SrcRT,ref  RenderTexture DstRT)
    {
        if (renderMat == null || renderMat.shader == null)
            return;

        // _Level("Level", Range(1, 100)) = 50
        //  _CenterX("Center.x", Range(0, 1)) = 0.5
        // _CenterY("Center.y", Range(0, 1)) = 0.5
        renderMat.SetFloat("_Level", Level);

        Vector3 pos = GraphicsManager.GetInstance().CurrentSubCamera.camera.WorldToScreenPoint(transform.position);

        renderMat.SetFloat("_CenterX", pos.x / Screen.width);
        renderMat.SetFloat("_CenterY", pos.y / Screen.height);

        Graphics.Blit(SrcRT, DstRT, renderMat);


    }
}
