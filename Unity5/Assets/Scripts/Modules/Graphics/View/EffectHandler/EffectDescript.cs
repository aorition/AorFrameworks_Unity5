using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;
using YoukiaCore;


namespace YoukiaUnity.View
{


    /// <summary>
    /// 特效描述
    /// </summary>
    public class EffectDescript : MonoBehaviour
    {
        /// <summary>
        /// 特效挂载点,null=创建它的人的当时世界坐标位置
        /// </summary>
        [Tooltip("挂载点")] public string EffectPivot;

        /// <summary>
        /// 特效挂载点模式
        /// </summary>
        [Tooltip("特效挂载点模式")] public EffectPivotType effectPivotType = EffectPivotType.World;

        /// <summary>
        /// 特效移除时采用的方式
        /// </summary>
        [Tooltip("特效移除时采用的方式")] public EffectDisposeType effectDisposeType = EffectDisposeType.Despawn;


        /// <summary>
        /// 特效生存时限,单位为秒,超时后自动删除
        /// </summary>
        [Tooltip("特效生存时间(秒),填负数则取动画长度作为生存时间")] public float SurvivalTime = 3f;

        public Trail[] trails;


    }


}

