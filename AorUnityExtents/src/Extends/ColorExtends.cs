using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Extends
{

    public static class ColorExtends
    {
        /// <summary>
        /// 转换为Color32
        /// </summary>
        public static Color32 ToColor32(this Color c)
        {
            return new Color32((byte)(c.r * 255), (byte)(c.g * 255), (byte)(c.b * 255), (byte)(c.a * 255));
        }

        /// <summary>
        /// 转换为Color
        /// </summary>
        public static Color ToColor(this Color32 c)
        {
            return new Color(c.r / 255f, c.g / 255f, c.b / 255f, c.a / 255f);
        }


        #region ToHtmlString

        public static string ToHtmlStringRGB(this Color32 c)
        {
            return ColorUtility.ToHtmlStringRGB(c.ToColor());
        }

        public static string ToHtmlStringRGB(this Color c)
        {
            return ColorUtility.ToHtmlStringRGB(c);
        }

        public static string ToHtmlStringRGBA(this Color32 c)
        {
            return ColorUtility.ToHtmlStringRGBA(c.ToColor());
        }

        public static string ToHtmlStringRGBA(this Color c)
        {
            return ColorUtility.ToHtmlStringRGBA(c);
        }

        #endregion


    }


}


