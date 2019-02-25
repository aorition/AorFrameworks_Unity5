using AorBaseUtility.Config;

namespace YoukiaUnity
{
    /// <summary>
    /// 动画剪辑配置类
    /// </summary>
    [System.Serializable]
    public class AnimationClipConfig : TConfig
    {
        [ConfigComment("动画名称")]
        public readonly string AnimationName;
    }

}

