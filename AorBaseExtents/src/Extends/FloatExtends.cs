using System;

namespace AorBaseUtility.Extends
{
    public static class FloatExtends
    {

        public const float Epsilon = 0.0001f;

        /// <summary>
        /// 可控的误差比较
        /// </summary>
        /// <param name="f1">浮点源1</param>
        /// <param name="f2">浮点源2</param>
        /// <param name="epsilon">公差范围</param>
        /// <returns>是否在误差内</returns>
        public static bool FloatEquel(this float f1, float f2, float epsilon = float.Epsilon)
        {
            return Math.Abs(f1 - f2) < epsilon;
        }

    }
}
