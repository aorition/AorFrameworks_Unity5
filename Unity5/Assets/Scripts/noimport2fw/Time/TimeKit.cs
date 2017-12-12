using System;
using System.Collections.Generic;

namespace YoukiaCore
{
    /// <summary>
    /// 类说明：时间工具
    /// 作者：刘耀鑫
    /// </summary>
    public class TimeKit
    {
        private static readonly DateTime initTime = new DateTime(1970, 1, 1, 8, 0, 0);
       
        // 服务器游戏开始时间
        public static int loginServerTime;

        /// <summary>
        /// 获取当前时间的时间戳
        /// </summary>
        public static long GetNowUnixTime()
        {
            return loginServerTime + (long)ConnectManager.manager().RealtimeSinceStartup;
        }

        /// <summary>
        /// 获取当前时间datetime对象
        /// </summary>
        public static DateTime GetNowDateTime()
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return dtStart.Add(new TimeSpan(GetNowUnixTime() * 10000000));
        }

        /// <summary>
        /// 得到当月的第一天
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(int Year, int Month)
        {
            return Convert.ToDateTime(Year.ToString() + "-" + Month.ToString() + "-1");
        }

        /// <summary>
        /// 得到当月的最后一天
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(int Year, int Month)
        {
            int Days = DateTime.DaysInMonth(Year, Month);
            return Convert.ToDateTime(Year.ToString() + "-" + Month.ToString() + "-" + Days.ToString());

        }

        /** 得到校正后时间，毫秒为单位 */
        public static long GetMillisTime()
        {
            return GetNowUnixTime() * 1000;
        }

        /** 得到校正后时间，秒为单位 */
        public static int GetSecondTime()
        {
            return (int)GetNowUnixTime();
        }

        //得到DateTime   从1970.1.1 8:00 开始 秒
        public static DateTime GetDateTime(int time = 0)
        {
            if (time == 0) time = GetSecondTime();
            return initTime.AddSeconds(time);
        }

        //得到DateTime   从1970.1.1 8:00 开始 毫秒
        public static DateTime GetDateTimeMillis(long time)
        {
            return initTime.AddMilliseconds(time);
        }

        //得到DateTime   从1970.1.1 8:00 开始 毫秒
        public static DateTime GetDateTimeMin(int time)
        {
            return initTime.AddMinutes(time / 60);
        }

        /// <summary>
        /// 得到当前时间的时间戳，未做时间修正，功能开发不能用
        /// </summary>
        /// <returns>长整型时间</returns>
        public static long CurrentTimeMillis()
        {
            return Convert.ToInt64(DateTime.Now.Subtract(initTime).TotalMilliseconds);
        }
    }
}
