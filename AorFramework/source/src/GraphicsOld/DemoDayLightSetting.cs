using System;
using System.Collections.Generic;
using UnityEngine;
using YoukiaUnity.Graphics;

public class DemoDayLightSetting : DayLightSetting
{

    public bool HdrEnable = true;

    public AnimationCurve HDR_WhiteBalance = AnimationCurve.EaseInOut(0, 0f, 1f, 0f);
    public AnimationCurve HDR_MiddleGrey = AnimationCurve.EaseInOut(0, 1f, 1f, 1f);
    public AnimationCurve HDR_EnvironmentIntensity = AnimationCurve.EaseInOut(0, 0f, 1f, 0f);

    public bool BloomEnable = true;

    public AnimationCurve Bloom_Threshold = AnimationCurve.EaseInOut(0, 5f, 1f, 5f);
    public AnimationCurve Bloom_intensity = AnimationCurve.EaseInOut(0, 0.03f, 1f, 0.03f);
    public AnimationCurve Bloom_BlurSize = AnimationCurve.EaseInOut(0, 3f, 1f, 3f);

    public override void ApplyTimeInEnvironmentSetting()
    {

        if (LinkEnvironmentSetting != null && (LinkEnvironmentSetting is DemoEnvironmentSetting))
        {

            float fp = CurrentTimeForDay / 24.0f;

            DemoEnvironmentSetting DES = (DemoEnvironmentSetting)LinkEnvironmentSetting;


            if (BloomEnable)
            {
                DES.BloomThreshhold = Bloom_Threshold.Evaluate(fp);
                DES.BloomIntensity = Bloom_intensity.Evaluate(fp);
                DES.BloomBlurSize = Bloom_BlurSize.Evaluate(fp);
            }

        }

        base.ApplyTimeInEnvironmentSetting();
    }
}
