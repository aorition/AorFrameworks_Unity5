using UnityEngine;
using UnityEditor;

/// <summary>
/// [参考]
/// 封装 EditorGUI 一部分绘制方法
/// (摘抄至 NGUI)
/// </summary>
public static class NGUIEditorGUIUtility
{
    static Texture2D mContrastTex;

    static public Texture2D blankTexture
    {
        get
        {
            return UnityEditor.EditorGUIUtility.whiteTexture;
        }
    }

    static public Texture2D contrastTexture
    {
        get
        {
            if (mContrastTex == null) mContrastTex = CreateCheckerTex(
                new Color(0f, 0.0f, 0f, 0.5f),
                new Color(1f, 1f, 1f, 0.5f));
            return mContrastTex;
        }
    }

    static public void DrawSeparator()
    {
        GUILayout.Space(12f);

        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = blankTexture;
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.color = new Color(0f, 0f, 0f, 0.25f);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
            GUI.color = Color.white;
        }
    }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>
    static public bool DrawHeader(string text) { return DrawHeader(text, text, false, false); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>
    static public bool DrawHeader(string text, string key) { return DrawHeader(text, key, false, false); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, bool detailed) { return DrawHeader(text, text, detailed, !detailed); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
    {
        bool state = EditorPrefs.GetBool(key, true);

        if (!minimalistic) GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUI.changed = false;

        if (minimalistic)
        {
            if (state) text = "\u25BC" + (char)0x200a + text;
            else text = "\u25BA" + (char)0x200a + text;

            GUILayout.BeginHorizontal();
            GUI.contentColor = UnityEditor.EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
            if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
        else
        {
            text = "<b><size=11>" + text + "</size></b>";
            if (state) text = "\u25BC " + text;
            else text = "\u25BA " + text;
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
        }

        if (GUI.changed) EditorPrefs.SetBool(key, state);

        if (!minimalistic) GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }

    static public void BeginContents() { BeginContents(false); }

    static bool mEndHorizontal = false;

    /// <summary>
    /// Begin drawing the content area.
    /// </summary>

    static public void BeginContents(bool minimalistic)
    {
        if (!minimalistic)
        {
            mEndHorizontal = true;
            GUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
        }
        else
        {
            mEndHorizontal = false;
            EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
            GUILayout.Space(10f);
        }
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    /// <summary>
    /// End drawing the content area.
    /// </summary>

    static public void EndContents()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (mEndHorizontal)
        {
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(3f);
    }

    static public void DrawOutline(Rect rect)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = contrastTexture;
            GUI.color = Color.white;
            DrawTiledTexture(new Rect(rect.xMin, rect.yMax, 1f, -rect.height), tex);
            DrawTiledTexture(new Rect(rect.xMax, rect.yMax, 1f, -rect.height), tex);
            DrawTiledTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
            DrawTiledTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
        }
    }

    /// <summary>
    /// Draw a single-pixel outline around the specified rectangle.
    /// </summary>

    static public void DrawOutline(Rect rect, Color color)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = blankTexture;
            GUI.color = color;
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1f, rect.height), tex);
            GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, 1f, rect.height), tex);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
            GUI.color = Color.white;
        }
    }

    /// <summary>
    /// Draw a selection outline around the specified rectangle.
    /// </summary>

    static public void DrawOutline(Rect rect, Rect relative, Color color)
    {
        if (Event.current.type == EventType.Repaint)
        {
            // Calculate where the outer rectangle would be
            float x = rect.xMin + rect.width * relative.xMin;
            float y = rect.yMax - rect.height * relative.yMin;
            float width = rect.width * relative.width;
            float height = -rect.height * relative.height;
            relative = new Rect(x, y, width, height);

            // Draw the selection
            DrawOutline(relative, color);
        }
    }

    /// <summary>
    /// Draw a selection outline around the specified rectangle.
    /// </summary>

    static public void DrawOutline(Rect rect, Rect relative)
    {
        if (Event.current.type == EventType.Repaint)
        {
            // Calculate where the outer rectangle would be
            float x = rect.xMin + rect.width * relative.xMin;
            float y = rect.yMax - rect.height * relative.yMin;
            float width = rect.width * relative.width;
            float height = -rect.height * relative.height;
            relative = new Rect(x, y, width, height);

            // Draw the selection
            DrawOutline(relative);
        }
    }

    /// <summary>
    /// Draw a 9-sliced outline.
    /// </summary>

    static public void DrawOutline(Rect rect, Rect outer, Rect inner)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Color green = new Color(0.4f, 1f, 0f, 1f);

            DrawOutline(rect, new Rect(outer.x, inner.y, outer.width, inner.height));
            DrawOutline(rect, new Rect(inner.x, outer.y, inner.width, outer.height));
            DrawOutline(rect, outer, green);
        }
    }

    static Texture2D CreateCheckerTex(Color c0, Color c1)
    {
        Texture2D tex = new Texture2D(16, 16);
        tex.name = "[Generated] Checker Texture";

        for (int y = 0; y < 8; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c1);
        for (int y = 8; y < 16; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c0);
        for (int y = 0; y < 8; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c0);
        for (int y = 8; y < 16; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c1);

        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return tex;
    }

    static public void DrawTiledTexture(Rect rect, Texture tex)
    {
        GUI.BeginGroup(rect);
        {
            int width = Mathf.RoundToInt(rect.width);
            int height = Mathf.RoundToInt(rect.height);

            for (int y = 0; y < height; y += tex.height)
            {
                for (int x = 0; x < width; x += tex.width)
                {
                    GUI.DrawTexture(new Rect(x, y, tex.width, tex.height), tex);
                }
            }
        }
        GUI.EndGroup();
    }
}