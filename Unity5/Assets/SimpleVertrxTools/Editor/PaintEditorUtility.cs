using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.editor
{
    /// <summary>
    /// 这里的静态方法 基本是从 UnityEditor.TerrainInspector 里搬过来的GUI绘制封装方法
    /// </summary>
    public class PaintEditorUtility
    {
        public class Styles
        {
            public GUIStyle gridList = (GUIStyle)"GridList";
            public GUIStyle gridListText = (GUIStyle)"GridListText";
            public GUIStyle label = (GUIStyle)"RightLabel";
            public GUIStyle largeSquare = (GUIStyle)"Button";
            public GUIStyle command = (GUIStyle)"Command";
            public Texture settingsIcon = EditorGUIUtility.IconContent("SettingsIcon").image;
        }

        private static Styles m_styles;
        public static Styles styles
        {
            get
            {
                if(m_styles == null) m_styles = new Styles();
                return m_styles;
            }
        }

        public static int AspectSelectionGrid(float rectWidth, int selected, GUIContent[] GUIContents, int approxSize, GUIStyle style, string emptyString, out bool doubleClick)
        {
            GUILayout.BeginVertical((GUIStyle)"box", GUILayout.MinHeight(10f));
            int nSelected = 0;
            doubleClick = false;
            if (GUIContents != null && GUIContents.Length > 0)
            {
                int uNum = Mathf.FloorToInt((rectWidth - 20f) / approxSize); //一行有几个?
                int vNum = (int)Mathf.Ceil((float)GUIContents.Length / uNum); //有几行?
                Rect aspectRect = GUILayoutUtility.GetAspectRect((float)uNum / vNum);
                Event current = Event.current;
                if (current.type == EventType.MouseDown && current.clickCount == 2 && aspectRect.Contains(current.mousePosition))
                {
                    doubleClick = true;
                    current.Use();
                }
                nSelected = GUI.SelectionGrid(aspectRect, Math.Min(selected, GUIContents.Length - 1), GUIContents, uNum, style);
            }
            else
                GUILayout.Label(emptyString);
            GUILayout.EndVertical();
            return nSelected;
        }

        public static int AspectSelectionGrid(float rectWidth, int selected, Texture[] textures, int approxSize, GUIStyle style, string emptyString, out bool doubleClick)
        {
            GUILayout.BeginVertical((GUIStyle)"box", GUILayout.MinHeight(10f));
            int nSelected = 0;
            doubleClick = false;
            if (textures != null && textures.Length > 0)
            {
                int uNum = Mathf.FloorToInt((rectWidth - 20f) / approxSize); //一行有几个?
                int vNum = (int)Mathf.Ceil((float)textures.Length / uNum); //有几行?
                Rect aspectRect = GUILayoutUtility.GetAspectRect((float)uNum / vNum);
                Event current = Event.current;
                if (current.type == EventType.MouseDown && current.clickCount == 2 && aspectRect.Contains(current.mousePosition))
                {
                    doubleClick = true;
                    current.Use();
                }
                nSelected = GUI.SelectionGrid(aspectRect, Math.Min(selected, textures.Length - 1), textures, uNum, style);
            }
            else
                GUILayout.Label(emptyString);
            GUILayout.EndVertical();
            return nSelected;
        }

        private static Rect GetBrushAspectRect(float rectWidth, int elementCount, int approxSize, int extraLineHeight, out int xCount)
        {
            xCount = (int)Mathf.Ceil((rectWidth - 20f) / (float)approxSize);
            int num = elementCount / xCount;
            if (elementCount % xCount != 0)
                ++num;
            Rect aspectRect = GUILayoutUtility.GetAspectRect((float)xCount / (float)num);
            Rect rect = GUILayoutUtility.GetRect(10f, (float)(extraLineHeight * num));
            aspectRect.height += rect.height;
            return aspectRect;
        }

        public static int AspectSelectionGridImageAndText(float rectWidth, int selected, GUIContent[] textures, int approxSize, GUIStyle style, string emptyString, out bool doubleClick)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MinHeight(10f));
            int nSelected = 0;
            doubleClick = false;
            if (textures.Length != 0)
            {
                int xCount = 0;
                Rect brushAspectRect = GetBrushAspectRect(rectWidth,textures.Length, approxSize, 12, out xCount);
                Event current = Event.current;
                if (current.type == EventType.MouseDown && current.clickCount == 2 && brushAspectRect.Contains(current.mousePosition))
                {
                    doubleClick = true;
                    current.Use();
                }
                nSelected = GUI.SelectionGrid(brushAspectRect, Math.Min(selected, textures.Length - 1), textures, xCount, style);
            }
            else
                GUILayout.Label(emptyString);
            GUILayout.EndVertical();
            return nSelected;
        }

        public static void MenuButton(GUIContent title, string menuName, UnityEngine.Object context, int userData)
        {
            GUIContent content = new GUIContent(title.text, styles.settingsIcon, title.tooltip);
            Rect rect = GUILayoutUtility.GetRect(content, styles.largeSquare);
//            if (!GUI.Button(rect, content, styles.largeSquare))
//                return;
            MenuCommand command = new MenuCommand(context, userData);
            EditorUtility.DisplayPopupMenu(new Rect(rect.x, rect.y, 0.0f, 0.0f), menuName, command);
        }

    }
}
