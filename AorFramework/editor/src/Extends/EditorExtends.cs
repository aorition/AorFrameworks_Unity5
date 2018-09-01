using System;
using System.Collections.Generic;

using UnityEditor;

namespace Framework.Editor
{
    /// <summary>
    /// Editor常用扩展方法
    /// </summary>
    public static class EditorExtends
    {

        /// <summary>
        /// 获取当前编辑焦点是否在AnimationWindow窗口上
        /// </summary>
        public static bool isFoucsAnimtionWindow(this UnityEditor.Editor editor)
        {
            EditorWindow window = EditorWindow.focusedWindow;
            if (window == null)
                return false;
            Type t = window.GetType();
            if (t.Name == "AnimationWindow")
            {
                return true;
            }
            return false;
        }

    }
}


