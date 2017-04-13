using System;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// 简化原有GUILayout区块语句的语法糖
/// </summary>
public static class AorGUILayout
{

    #region BeginArea = > Area

    public static void Area(Rect screenRect, Action innerDraw)
    {
        GUILayout.BeginArea(screenRect);
        innerDraw();
        GUILayout.EndArea();
    }

    public static void Area(Rect screenRect, GUIContent content, Action innerDraw)
    {
        GUILayout.BeginArea(screenRect, content);
        innerDraw();
        GUILayout.EndArea();
    }

    public static void Area(Rect screenRect, GUIContent content, GUIStyle style, Action innerDraw)
    {
        GUILayout.BeginArea(screenRect, content, style);
        innerDraw();
        GUILayout.EndArea();
    }

    public static void Area(Rect screenRect, GUIStyle style, Action innerDraw)
    {
        GUILayout.BeginArea(screenRect, style);
        innerDraw();
        GUILayout.EndArea();
    }

    public static void Area(Rect screenRect, Texture image, Action innerDraw)
    {
        GUILayout.BeginArea(screenRect, image);
        innerDraw();
        GUILayout.EndArea();
    }

    public static void Area(Rect screenRect, Texture image, GUIStyle style, Action innerDraw)
    {
        GUILayout.BeginArea(screenRect, image, style);
        innerDraw();
        GUILayout.EndArea();
    }

    public static void Area(Rect screenRect, string text, Action innerDraw)
    {
        GUILayout.BeginArea(screenRect, text);
        innerDraw();
        GUILayout.EndArea();
    }

    public static void Area(Rect screenRect, string text, GUIStyle style, Action innerDraw)
    {
        GUILayout.BeginArea(screenRect, text, style);
        innerDraw();
        GUILayout.EndArea();
    }

    #endregion

    #region BeginHorizontal = > Horizontal
    public static void Horizontal(Action innerDraw, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(options);
        innerDraw();
        GUILayout.EndHorizontal();
    }
    public static void Horizontal(GUIContent content, GUIStyle style, Action innerDraw, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(content, style, options);
        innerDraw();
        GUILayout.EndHorizontal();
    }
    public static void Horizontal(GUIStyle style, Action innerDraw, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(style, options);
        innerDraw();
        GUILayout.EndHorizontal();
    }
    public static void Horizontal(string text, GUIStyle style, Action innerDraw, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(text, style, options);
        innerDraw();
        GUILayout.EndHorizontal();
    }
    public static void Horizontal(Texture image, GUIStyle style, Action innerDraw, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(image, style, options);
        innerDraw();
        GUILayout.EndHorizontal();
    }
    #endregion

    #region BeginVertical = > Vertical
    public static void Vertical(Action innerDraw, params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(options);
        innerDraw();
        GUILayout.EndVertical();
    }
    public static void Vertical(GUIContent content, GUIStyle style, Action innerDraw, params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(content, style, options);
        innerDraw();
        GUILayout.EndVertical();
    }
    public static void Vertical(GUIStyle style, Action innerDraw, params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(style, options);
        innerDraw();
        GUILayout.EndVertical();
    }
    public static void Vertical(string text, GUIStyle style, Action innerDraw, params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(text, style, options);
        innerDraw();
        GUILayout.EndVertical();
    }
    public static void Vertical(Texture image, GUIStyle style, Action innerDraw, params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(image, style, options);
        innerDraw();
        GUILayout.EndVertical();
    }
    #endregion

    #region BeginScrollView = > ScrollView
    public static Vector2 ScrollView(Vector2 scrollPosition, GUIStyle horizontalScrollBar, GUIStyle verticalScrollBar, Action<Vector2> innerDraw, params GUILayoutOption[] options)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, horizontalScrollBar, verticalScrollBar, options);
        innerDraw(scrollPosition);
        GUILayout.EndScrollView();
        return scrollPosition;
    }

    public static Vector2 ScrollView(Vector2 scrollPosition, GUIStyle style, Action<Vector2> innerDraw)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, style);
        innerDraw(scrollPosition);
        GUILayout.EndScrollView();
        return scrollPosition;
    }

    public static Vector2 ScrollView(Vector2 scrollPosition, GUIStyle style, Action<Vector2> innerDraw, params GUILayoutOption[] options)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, style, options);
        innerDraw(scrollPosition);
        GUILayout.EndScrollView();
        return scrollPosition;
    }

    public static Vector2 ScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollBar, GUIStyle verticalScrollBar, GUIStyle background, Action<Vector2> innerDraw, params GUILayoutOption[] options)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollBar, verticalScrollBar, background, options);
        innerDraw(scrollPosition);
        GUILayout.EndScrollView();
        return scrollPosition;
    }

    public static Vector2 ScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollBar, GUIStyle verticalScrollBar, Action<Vector2> innerDraw, params GUILayoutOption[] options)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollBar, verticalScrollBar, options);
        innerDraw(scrollPosition);
        GUILayout.EndScrollView();
        return scrollPosition;
    }

    public static Vector2 ScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, Action<Vector2> innerDraw, params GUILayoutOption[] options)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, options);
        innerDraw(scrollPosition);
        GUILayout.EndScrollView();
        return scrollPosition;
    }

    public static Vector2 ScrollView(Vector2 scrollPosition, Action<Vector2> innerDraw, params GUILayoutOption[] options)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, options);
        innerDraw(scrollPosition);
        GUILayout.EndScrollView();
        return scrollPosition;
    }

    #endregion

}

