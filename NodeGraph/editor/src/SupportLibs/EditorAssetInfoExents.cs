using System;
using System.Collections.Generic;

namespace NodeGraph.SupportLib
{

    public static class EditorAssetInfoExents
    {
        /// <summary>
        /// 过滤,输出满足条件的EditorAssetInfo
        /// </summary>
        public static EditorAssetInfo[] filter(this EditorAssetInfo[] e, Func<EditorAssetInfo, bool> fil)
        {
            List<EditorAssetInfo> t = new List<EditorAssetInfo>();
            int i, len = e.Length;
            for (i = 0; i < len; i++)
            {
                if (fil(e[i]))
                {
                    t.Add(e[i]);
                }
            }

            if (t.Count > 0)
            {
                return t.ToArray();
            }
            return null;
        }

        /// <summary>
        /// 过滤,输出满足条件的EditorAssetInfo
        /// </summary>
        public static List<EditorAssetInfo> filter(this List<EditorAssetInfo> e, Func<EditorAssetInfo, bool> fil)
        {
            List<EditorAssetInfo> t = new List<EditorAssetInfo>();
            int i, len = e.Count;
            for (i = 0; i < len; i++)
            {
                if (fil(e[i]))
                {
                    t.Add(e[i]);
                }
            }

            if (t.Count > 0)
            {
                return t;
            }
            return null;
        }

        /// <summary>
        /// 输入指定类型的Assets
        /// </summary>
        public static T[] ToAssets<T>(this List<EditorAssetInfo> e) where T : UnityEngine.Object
        {
            return e.ToArray().ToAssets<T>();
        }

        /// <summary>
        /// 输入指定类型的Assets
        /// </summary>
        public static T[] ToAssets<T>(this EditorAssetInfo[] e) where T : UnityEngine.Object
        {

            if (e.Length > 0)
            {

                List<T> tList = new List<T>();

                int i, len = e.Length;
                for (i = 0; i < len; i++)
                {
                    if (e[i].asset is T)
                    {
                        tList.Add(e[i].asset as T);
                    }
                }

                if (tList.Count > 0)
                {
                    return tList.ToArray();
                }

            }

            return null;
        }


    }
}