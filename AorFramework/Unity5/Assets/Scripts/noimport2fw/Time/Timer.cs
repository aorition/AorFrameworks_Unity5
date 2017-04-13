using System;
using System.Collections.Generic;

namespace YoukiaCore
{
    /// <summary>
    /// 类说明：定时器
    /// 作者：刘耀鑫
    /// </summary>
    public class Timer
    {

        /// <summary>
        /// 回调函数
        /// </summary>
        private Action _call;
        /// <summary>
        /// 延迟时间：秒
        /// </summary>
        private float _delay;
        /// <summary>
        /// 结束时间
        /// </summary>
        private float _endTime;
        /// <summary>
        /// 是否执行一次
        /// </summary>
        private bool _once;
        /// <summary>
        /// 忽略缩放
        /// </summary>
        private bool _ignoreScale;

        /// <summary>
        /// 回调函数
        /// </summary>
        public Action Call
        {
            get
            {
                return _call;
            }
        }

        /// <summary>
        /// 延迟时间：秒
        /// </summary>
        public float Delay
        {
            get
            {
                return _delay;
            }
        }

        /// <summary>
        /// 是否执行一次
        /// </summary>
        public bool Once
        {
            get
            {
                return _once;
            }
        }

        /// <summary>
        /// 忽略缩放
        /// </summary>
        public bool IgnoreScale
        {
            get
            {
                return _ignoreScale;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Timer(Action call, float delay, bool once = false, bool ignoreScale = true)
        {
            this._call = call;
            this._delay = delay;
            this._once = once;
            this._ignoreScale = ignoreScale;
        }

        /// <summary>
        /// 重置，重新计算时长
        /// </summary>
        public void Reset(IGetTime getTime)
        {
            _endTime = (_ignoreScale ? getTime.GetUnscaledTime() : getTime.GetTime()) + _delay;
        }

        /// <summary>
        /// 是否可以重置
        /// </summary>
        /// <returns></returns>
        public bool IsReset(IGetTime getTime)
        {
            return _endTime <= (_ignoreScale ? getTime.GetUnscaledTime() : getTime.GetTime());
        }
        
    }
}
