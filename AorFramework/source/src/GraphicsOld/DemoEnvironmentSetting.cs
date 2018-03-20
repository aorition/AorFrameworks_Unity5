using System;
using UnityEngine;
using UnityEngine.Rendering;
using YoukiaUnity.Graphics;
using YoukiaUnity.Scene;

//环境配置
public class DemoEnvironmentSetting : EnvironmentSetting
{

    [NonSerialized]
    public float BloomThreshhold = 2.8f;
    [NonSerialized]
    public float BloomIntensity = 0.1f;
    [NonSerialized]
    public float BloomBlurSize = 3f;

    //hide
    private DemoDrawCard.Resolution _resolution = DemoDrawCard.Resolution.High;


    public override void EnvironmentUpdate(bool force)
    {
        base.EnvironmentUpdate(force);



        DemoDrawCard Dcard = GraphicsManager.GetInstance().DrawCard as DemoDrawCard;

        //单元测试
        if (Dcard == null)
        {
            DemoDrawCard baseCard = new DemoDrawCard();
            GraphicsManager.GetInstance().DrawCard = baseCard;
            Dcard = baseCard;
            GraphicsManager.GetInstance().SetExposure(1);
        }


        Dcard.BloomCheck();
        Dcard.BlurThreshhold = BloomThreshhold;
        Dcard.BlurIntensity = BloomIntensity;
        Dcard.blurSize = BloomBlurSize;
        Dcard.resolution = _resolution;

        Shader.SetGlobalFloat("_HdrIntensity", 0);


    }

}

