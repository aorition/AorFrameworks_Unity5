using System;
using System.Collections.Generic;

namespace AorBaseUtility.Extends
{

    public static class ListEntends
    {
        /// <summary>
        /// 克隆一个List
        /// </summary>
        public static List<T> Clone<T>(this List<T> list) where T : class
        {
            if (list == null) return null;
            List<T> t = new List<T>();
            int i, length = list.Count;
            for (i = 0; i < length; i++)
            {
                T item = list[i] as T;
                t.Add(item);
            }
            return t;
        }
    }
}
