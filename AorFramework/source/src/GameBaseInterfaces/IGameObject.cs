using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 一个Game物体的基础接口
    /// </summary>
    public interface IGameObject
    {
        /// <summary>
        /// 物体名称
        /// </summary>
        string name { get; set; }
        /// <summary>
        /// 物体标签
        /// </summary>
        string tag { get; set; }
        /// <summary>
        /// 物件的Transform
        /// </summary>
        Transform transform { get; }
        /// <summary>
        /// 物件的GameObject
        /// </summary>
        GameObject gameObject { get; }
        

        /// <summary>
        /// 物件状态标识
        /// </summary>
        string state { get; set; }
        /// <summary>
        /// 是否可见
        /// </summary>
        void SetVisible(bool visble);

    }
}
