using AorBaseUtility.Config;
using YoukiaUnity.Graphics.FastShadowProjector;

namespace YoukiaUnity
{
    /// <summary>
    /// 显示配置类
    /// </summary>
    public class DisplayConfig : Config {

        //----------------------- AorUIManager -------------- 
        [ConfigComment("设计舞台尺寸")]
        public readonly int[] DesginStageSize;
        [ConfigComment("舞台缩放模式")]
        public readonly string ScaleMode;
        [ConfigComment("设计背景尺寸")] 
        public readonly int[] BackgroundSize;
        [ConfigComment("背景缩放模式")]
        public readonly string BGScaleMode;
        [ConfigComment("背景对齐方式")]
        public readonly string BGAlignType;
        [ConfigComment("是否隐藏背景")]
        public readonly bool isHideBackground;
        //----------------------- AorUIManager ---------- end


        //--------------------- GraphicsManager -------------
        [ConfigComment("最小裁剪")]
        public readonly float NearCameraClip;
        [ConfigComment("切换裁剪")]
        public readonly float MiddleCameraClip;
        [ConfigComment("最大裁剪")]
        public readonly float FarCameraClip;
        [ConfigComment("品质等级")]
        public readonly int GraphicQuality;
        [ConfigComment("阴影类型")]
        public readonly GlobalProjectorManager.ShadowType ShadowType= GlobalProjectorManager.ShadowType.None;
        [ConfigComment("阴影质量")]
        public readonly GlobalProjectorManager.ShadowResolutions ShadowResolutions = GlobalProjectorManager.ShadowResolutions.VeryLow_128;
 
        [ConfigComment("大气雾距离")]
        public readonly float FogDistance;
        [ConfigComment("大气雾密度")]
        public readonly float FogDestiy;
        [ConfigComment("显示分辨率")]
        public readonly int Resolution;
        [ConfigComment("抗锯齿")]
        public readonly int AntiAliasing;
        //--------------------- GraphicsManager --------- end

    }

}

