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
        /// 运行时如果该Effect挂载到某个角色节点下，该字段则记录这个角色节点
        /// </summary>
        [HideInInspector] public GameObject EffectRootGameObject;

        /// <summary>
        /// 运行时记录，该Effect注册的LayerMask
        /// </summary>
        [HideInInspector] public int registLayerMask;

        /// <summary>
        /// 运行时标记，该Effect是否在HideLayer中
        /// </summary>
        [HideInInspector] public bool isInHideLayer = false;

        //------------------------------------------------------------

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

