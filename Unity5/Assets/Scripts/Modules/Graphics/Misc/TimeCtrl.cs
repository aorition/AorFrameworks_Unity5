using System;
using UnityEngine;
//using YoukiaUnity.App;

namespace YoukiaUnity.Misc
{
    /// <summary>
    /// 时间控制器，通过此脚本控制游戏时间的快慢
    /// </summary>
    public class TimeCtrl : MonoBehaviour
    {
        public float FixSpeed = 1;

        /// <summary>
        /// 是否用曲线来调整速度，勾选后固定速度无效
        /// </summary>
        [Tooltip("用曲线来调整速度，勾选后固定速度无效")]
        public bool UseTimeCurve;
        /// <summary>
        /// 时间曲线
        /// </summary>
        public AnimationCurve TimeCurve;
        float lifeTime;

        void OnEnable()
        {
            lifeTime = 0;
        }


        void Update()
        {
            lifeTime += Time.unscaledDeltaTime;

            if (UseTimeCurve)
                Time.timeScale = Mathf.Max(0, TimeCurve.Evaluate(lifeTime));
            else
                Time.timeScale = FixSpeed;
        }
    }

}

