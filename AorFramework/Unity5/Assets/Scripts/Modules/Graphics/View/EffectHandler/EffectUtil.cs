using System;
using UnityEngine;


namespace YoukiaUnity.View
{
    /// <summary>
    /// 特效挂载类型
    /// </summary>
    public enum EffectPivotType
    {
        Follow, //完全跟随挂载点的位置和方向
        World, //生成特效特效时,位置和方向与挂载点一致
        WorldPos,//生成特效特效时,位置与挂载点一致(不包括方向)
        FollowGround, //该特效跟随挂载点所指向的地面位置
        WorldGround //生成特效与挂载点所指向的地面位置
    }

    /// <summary>
    /// 特效销毁类型
    /// </summary>
    public enum EffectDisposeType
    {
        Despawn, //放入缓冲池
        destroy, //销毁
        hidden //隐藏
    }

    /// <summary>
    /// 特效控制器
    /// </summary>
    public class EffectUtil
    {

        public static void EffectDispose(EffectDescript ed)
        {

            switch (ed.effectDisposeType)
            {
                case EffectDisposeType.Despawn:
                    //TODO 需求回收装置
                    GameObject.Destroy(ed.gameObject);
                    // YKApplication.Instance.GetManager<ResourcesManager>().PoolMg.PutInPool(ed.gameObject.GetComponent<ResourceRefKeeper>());
                    break;
                case EffectDisposeType.hidden:
                    ed.gameObject.SetActive(false);
                    break;
                default:
                    if (Application.isEditor)
                    {
                        GameObject.DestroyImmediate(ed.gameObject);
                    }
                    else
                    {
                        GameObject.Destroy(ed.gameObject);
                    }
                    break;
            }
        }
    }
}


