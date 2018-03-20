using System;
using System.Collections.Generic;

namespace AorBaseUtility
{
    public static class ArrayExtends
    {

        /// <summary>
        /// 克隆一个数组
        /// </summary>
        public static T[] Clone<T>(this T[] Array) where T : class
        {
            if (Array == null) return null;

            int i, len = Array.Length;
            T[] newArray = new T[len];

            for (i = 0; i < len; i++)
            {
                newArray[i] = Array[i];
            }

            return newArray;
        }

    }
}
