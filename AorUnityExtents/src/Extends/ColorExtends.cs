using System;
using UnityEngine;

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

    /// <summary>
    /// 两色插值
    /// </summary>
    public static Color ColorLerpUnclamped(Color c1, Color c2, float value, bool keepAlpha = false)
    {
        return keepAlpha ? new Color(c1.r + (c2.r - c1.r) * value,
                          c1.g + (c2.g - c1.g) * value,
                          c1.b + (c2.b - c1.b) * value,
                          c1.a)
                          :
                           new Color(c1.r + (c2.r - c1.r) * value,
                          c1.g + (c2.g - c1.g) * value,
                          c1.b + (c2.b - c1.b) * value,
                          c1.a + (c2.a - c1.a) * value);
    }

    #region 基础计算

    /// <summary>
    /// 加
    /// </summary>
    public static Color Add(this Color c1, Color c2, bool keepAlpha = false)
    {
        return keepAlpha ? new Color(c1.r + c2.r, c1.g + c2.g, c1.b + c2.b, c1.a) : new Color(c1.r + c2.r, c1.g + c2.g, c1.b + c2.b, Mathf.Clamp(c1.a + c2.a, 0, 1f));
    }
    public static Color Add(this Color c1, float f, bool keepAlpha = false)
    {
        return keepAlpha ? new Color(c1.r + f, c1.g + f, c1.b + f, c1.a) : new Color(c1.r + f, c1.g + f, c1.b + f, Mathf.Clamp(c1.a + f, 0, 1f));
    }
    
    /// <summary>
    /// 减
    /// </summary>
    public static Color Subtract(this Color c1, Color c2, bool keepAlpha = false)
    {
        return keepAlpha ? new Color(c1.r - c2.r, c1.g - c2.g, c1.b - c2.b, c1.a) : new Color(c1.r - c2.r, c1.g - c2.g, c1.b - c2.b, Mathf.Clamp(c1.a - c2.a, 0, 1f));
    }
    public static Color Subtract(this Color c1, float f, bool keepAlpha = false)
    {
        return keepAlpha ? new Color(c1.r - f, c1.g - f, c1.b - f, c1.a) : new Color(c1.r - f, c1.g - f, c1.b - f, Mathf.Clamp(c1.a - f, 0, 1f));
    }

    /// <summary>
    /// 乘以
    /// </summary>
    public static Color Multiply(this Color c1, Color c2, bool keepAlpha = false)
    {
        return keepAlpha ? new Color(c1.r * c2.r, c1.g * c2.g, c1.b * c2.b, c1.a) : new Color(c1.r * c2.r, c1.g * c2.g, c1.b * c2.b, Mathf.Clamp(c1.a * c2.a, 0, 1f));
    }
    public static Color Multiply(this Color c1, float f, bool keepAlpha = false)
    {
        return keepAlpha ? new Color(c1.r * f, c1.g * f, c1.b * f, c1.a) : new Color(c1.r * f, c1.g * f, c1.b * f, Mathf.Clamp(c1.a * f, 0, 1f));
    }

    /// <summary>
    /// 除以
    /// </summary>
    public static Color Divide(this Color c1, Color c2, bool keepAlpha = false)
    {
        return keepAlpha ? new Color(c1.r / c2.r, c1.g / c2.g, c1.b / c2.b, c1.a) : new Color(c1.r / c2.r, c1.g / c2.g, c1.b / c2.b, Mathf.Clamp(c1.a / c2.a, 0, 1f));
    }
    public static Color Divide(this Color c1, float f, bool keepAlpha = false)
    {
        return keepAlpha ? new Color(c1.r / f, c1.g / f, c1.b / f, c1.a) : new Color(c1.r / f, c1.g / f, c1.b / f, Mathf.Clamp(c1.a / f, 0, 1f));
    }

    /// <summary>
    /// 去色
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color Decolorization(this Color color)
    {
        float lum = (Mathf.Max(color.r, color.b) + Mathf.Max(color.r, color.g)) * 0.5f;
        return new Color(lum, lum, lum, color.a);
    }

    #endregion

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

