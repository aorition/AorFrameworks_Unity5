using UnityEngine;
using YoukiaCore;
using YoukiaUnity.Graphics;
using YoukiaUnity.Graphics.FastShadowProjector;

namespace YoukiaUnity
{
    /// <summary>
    /// 显示配置类
    /// </summary>
    public class DisplayConfig {

        //--------------------- GraphicsManager -------------
        [ConfigComment("开启辉光")]
        public bool EnableBloom;

        [ConfigComment("开启后期特效")]
        public bool EnablePostEffect;



        [ConfigComment("后期图大小")]
        public GraphicsManager.RtSize PostEffectRenderSize= GraphicsManager.RtSize.Quarter;


        [ConfigComment("品质等级")]
        public  int GraphicQuality;
        [ConfigComment("阴影类型")]
        public  GlobalProjectorManager.ShadowType ShadowType= GlobalProjectorManager.ShadowType.None;
        [ConfigComment("阴影质量")]
        public  GlobalProjectorManager.ShadowResolutions ShadowResolutions = GlobalProjectorManager.ShadowResolutions.VeryLow_128;
 
        [ConfigComment("大气雾距离")]
        public  float FogDistance;
        [ConfigComment("大气雾密度")]
        public  float FogDestiy;
 


        //--------------------- GraphicsManager --------- end

    }

}

