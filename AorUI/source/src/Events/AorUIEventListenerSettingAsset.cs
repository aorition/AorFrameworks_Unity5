using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI
{
    public class AorUIEventListenerSettingAsset : ScriptableObject
    {

        public static AorUIEventListenerSettingAsset Default()
        {
            return ScriptableObject.CreateInstance<AorUIEventListenerSettingAsset>();
        }

        public float FPS = 60f;

        /// <summary>
        /// 点击判定距离阀值 (屏幕像素) : 按下到弹起的距离阀值,超过这个值后则不认为是Click事件;
        /// </summary>
        public float ClickDistanceThreshold = 50;

        /// <summary>
        /// 点击判定时间阀值 (毫秒) : 按下到弹起的时间限制,超过这个限制后则不触发Click事件
        /// </summary>
        public float ClickTimeThreshold = 500;

        /// <summary>
        /// 长按判定距离阀值 (屏幕像素) : 按下到弹起的距离阀值,超过这个值后则不认为是LongPress事件;
        /// </summary>
        public float LongPressDistanceThreshold = 50;

        /// <summary>
        /// 长按判定时间阀值 (毫秒) : 按下后持续时间超过此阀值时,将判定为LongPress事件;
        /// </summary>
        public float LongPressTimeThreshold = 1000;

        /// <summary>
        /// 甩出动作判定距离阀值 (屏幕像素) : 按下到弹起的距离阀值,小于此阀值则不触发Swing事件
        /// </summary>
        public float SwingDistanceThreshold = 80;

        /// <summary>
        /// 甩出动作判定时间阀值 (毫秒) : 按下到弹起的时间限制,超过这个限制后则不触发Swing事件
        /// </summary>
        public float SwingTimeThreshold = 600;
    }
}
