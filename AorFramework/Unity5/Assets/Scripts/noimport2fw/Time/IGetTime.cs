using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YoukiaCore
{
    /// <summary>
    /// 接口说明：Timer获取时间接口
    /// 作者：刘耀鑫
    /// </summary>
    public interface IGetTime
    {
        /// <summary>
        /// 获取时间
        /// </summary>
        /// <returns>秒</returns>
        float GetTime();

        /// <summary>
        /// 获取未缩放时间
        /// </summary>
        /// <returns>秒</returns>
        float GetUnscaledTime();
    }
}
