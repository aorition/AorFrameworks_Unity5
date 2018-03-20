using AorBaseUtility;
using AorBaseUtility.Config;
using YoukiaCore;

namespace YoukiaUnity
{
    /// <summary>
    /// 动画剪辑配置类
    /// </summary>
    public class AnimationClipConfig : TConfig
    {
        [ConfigComment("动画名称")]
        public readonly string AnimationName;
    }

}

