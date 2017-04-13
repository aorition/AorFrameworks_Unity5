using UnityEngine;
using UnityEngine.Rendering;
using YoukiaUnity.Graphics;
using YoukiaUnity.Scene;

//环境配置
public class DemoEnvironmentSetting : EnvironmentSetting
{




    public bool Hdr = true;
    public bool Bloom = true;
    public float HDR_WhiteBalance = 0f;
    public float HDR_MiddleGrey = 0.4f;
    public float HDR_EnvironmentIntensity = 0;



    private float HDR_BloomThreshhold = 5f;
    private float HDR_BloomIntensity = 0.1f;
    private float HDR_BloomBlurSize = 3f;


    private float NoHDR_SkyIntensity = 1;
    private float NoHDR_BloomThreshhold = 5f;
    private float NoHDR_BloomIntensity = 0.1f;
    private float NoHDR_BloomBlurSize = 3f;

    //hide
    private DemoDrawCard.Resolution _resolution = DemoDrawCard.Resolution.High;
    [Range(1, 4)]
    private int _blurIterations = 2;





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


        Dcard.Hdr = Hdr;
        Dcard.UseBloom = Bloom;
        Dcard.blurIterations = _blurIterations;
        Dcard.resolution = _resolution;


        if (Dcard.Hdr)
        {

            Shader.SetGlobalFloat("_HdrIntensity", HDR_EnvironmentIntensity);

            Dcard.middleGrey = HDR_MiddleGrey;
            Dcard.whiteBalance = HDR_WhiteBalance;
            Dcard.threshhold = HDR_BloomThreshhold;
            Dcard.intensity = HDR_BloomIntensity;
            Dcard.blurSize = HDR_BloomBlurSize;

        }
        else
        {

            Shader.SetGlobalFloat("_HdrIntensity", 0);
            gameObject.GetComponent<Renderer>().material.SetFloat("_Exposure", NoHDR_SkyIntensity);

            Dcard.threshhold = NoHDR_BloomThreshhold;
            Dcard.intensity = NoHDR_BloomIntensity;
            Dcard.whiteBalance = 0;
        }

    }

}

