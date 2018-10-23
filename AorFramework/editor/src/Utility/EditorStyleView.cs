using UnityEngine;
using UnityEditor;

namespace Framework.Editor.Utility
{

    /// <summary>
    /// 查看默认的gui skin样式
    /// </summary>
    public class EditorStyleView : EditorWindow
    {
        private Vector2 scrollPosition = Vector2.zero;
        private string search = string.Empty;

        [MenuItem("FrameworkTools/辅助工具/默认GUI样式查看器", false)]
        static void Init()
        {
            EditorWindow.GetWindow<EditorStyleView>("GUI样式查看器");
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal("HelpBox");
            GUILayout.Label("单击左侧样式将复制其名到剪贴板", "label");
            GUILayout.FlexibleSpace();
            GUILayout.Label("查找:");
            search = EditorGUILayout.TextField(search);
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            //foreach (GUIStyle style in GUI.skin.customStyles)
            foreach (GUIStyle style in GUI.skin)
            {
                //过滤
                if (style.name.ToLower().Contains(search.ToLower()))
                {
                    //设置奇偶行不同背景
                    GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                    GUILayout.Space(20);//左边留白20
                    if (GUILayout.Button(style.name, style))
                    {
                        //把名字存储在剪粘板 
                        UnityEditor.EditorGUIUtility.systemCopyBuffer = style.name; // "\"" + style.name + "\"";
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.SelectableLabel("\"" + style.name + "\"");
                    GUILayout.Space(20);//右边留白20
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
        }
    }

}

