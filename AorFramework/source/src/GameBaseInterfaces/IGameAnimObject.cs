using System;
using System.Collections.Generic;
using UnityEngine;

namespace AorFramework
{

    /// <summary>
    /// 定义一个带有动画的Game物件
    /// </summary>
    public interface IGameAnimObject : IGameObject
    {

        /// <summary>
        /// 获取当前播放动画的标识 (标识一般指能够直接Play调用的名称)
        /// </summary>
        string CurrentAnimtation { get; }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="anim">动画标识</param>
        /// <param name="wrap">动画WrapMode</param>
        /// <param name="speed">速度</param>
        /// <param name="normalizedTime">速度</param>
        void PlayAnimtation(string anim, WrapMode wrap, float speed, float normalizedTime = 0);

    }
}
