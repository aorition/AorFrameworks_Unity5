using System;
using System.Collections.Generic;
using System.Threading;

namespace YoukiaCore
{
    /// <summary>
    /// 类说明：定时器管理器
    /// 作者：刘耀鑫
    /// </summary>
    public class TimerManager
    {

        /// <summary>
        /// 定时器列表
        /// </summary>
        //        private static YKList<Timer> _timerList = new YKList<Timer>();
        private static List<Timer> _timerList = new List<Timer>();
        /// <summary>
        /// 时间获取类
        /// </summary>
        private static IGetTime _getTime;


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="getTime">时间获取</param>
        public static void Init(IGetTime getTime)
        {
            _getTime = getTime;
            if (_getTime is SystemGetTime)
            {
                Thread t = new Thread(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(1);
                        Update();
                    }
                });
                t.IsBackground = true;
                t.Start();
            }
        }

        /// <summary>
        /// 添加定时器
        /// </summary>
        /// <param name="call">回调函数</param>
        /// <param name="delay">延迟时间</param>
        /// <param name="once">是否只执行一次</param>
        public static void AddTimer(float delay, Action call, bool once = true, bool ignoreScale = false)
        {
            Timer timer = new Timer(call, delay, once, ignoreScale);
            timer.Reset(_getTime);
            _timerList.Add(timer);
        }

        /// <summary>
        /// 移出定时器
        /// </summary>
        /// <param name="call">回调函数</param>
        public static void RemoveTimer(Action call)
        {
            for (int i = _timerList.Count - 1; i >= 0; i--)
            {
                if (_timerList[i].Call == call)
                {
                    _timerList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public static void Update()
        {
            for (int i = _timerList.Count - 1; i >= 0; i--)
            {
                if (_timerList[i].IsReset(_getTime))
                {
                    if (_timerList[i].Call != null)
                    {
                        _timerList[i].Call();
                    }

                    if(_timerList[i]==null)
                        continue;

                    if (_timerList[i].Once)
                    {
                        _timerList.RemoveAt(i);
                    }
                    else
                    {
                        _timerList[i].Reset(_getTime);
                    }
                }
            }
        }

        /// <summary>
        /// 注销定时器;
        /// </summary>
        public static void Dispose()
        {
            for (int i = _timerList.Count - 1; i >= 0; i--)
            {
                _timerList.RemoveAt(i);
            }
        }

    }
}
