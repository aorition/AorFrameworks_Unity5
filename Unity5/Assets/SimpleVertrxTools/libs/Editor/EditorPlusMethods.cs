using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace Framework.editor
{

    /// <summary>
    /// 提供Editor下常用功能静态方法
    /// </summary>
    public class EditorPlusMethods
    {

        /// <summary>
        /// 创建制定对象的撤消点
        /// </summary>
        //        public static void RegisterUndo(string name, params UnityEngine.Object[] objects)
        //        {
        //            if (objects != null && objects.Length > 0)
        //            {
        //                UnityEditor.Undo.RecordObjects(objects, name);
        //
        //                foreach (UnityEngine.Object obj in objects)
        //                {
        //                    if (obj == null) continue;
        //                    EditorUtility.SetDirty(obj);
        //                }
        //            }
        //        }

        /// <summary>
        /// 获取Inspector的ActiveEditorTracker, 可以监视Inspector的状态.
        /// </summary>
        public static ActiveEditorTracker GetCurrentInspectorTracker()
        {
            System.Type typeInspector = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
            EditorWindow w = EditorWindow.focusedWindow;
            if (!typeInspector.IsInstanceOfType(w))
            {
                w = null;
                UnityEngine.Object[] objs = Resources.FindObjectsOfTypeAll(typeInspector);
                if (objs.Length > 0) w = objs[0] as EditorWindow;
            }
            if (w != null)
            {
                ActiveEditorTracker tracker = typeInspector.InvokeMember("GetTracker", BindingFlags.InvokeMethod, null, w, null) as ActiveEditorTracker;
                return tracker;
            }
            return null;
        }

        /// <summary>
        /// 编辑器在下一次Update时调用
        /// </summary>
        public static void NextEditorApplicationUpdateDo(Action doSomething)
        {
            EditorApplication.CallbackFunction UDDoOnce = null;
            UDDoOnce = () =>
            {
                EditorApplication.update -= UDDoOnce;
                //
                if (doSomething != null) doSomething();
            };
            EditorApplication.update += UDDoOnce;
        }

        /// <summary>
        /// 在 Project 中创建一个ScriptableObject子类文件
        /// </summary>
        /// <typeparam name="T">ScriptableObject子类</typeparam>
        public static T CreateAssetFile<T>(string assetName) where T : ScriptableObject
        {
            string dir;
            string name = assetName + ".asset";
            if (Selection.objects == null || Selection.objects.Length == 0)
            {
                dir = "Assets";
            }
            else
            {

                string dataPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
                if (Selection.objects[0] is DefaultAsset)
                {
                    dir = dataPath;
                }
                else
                {
                    if (string.IsNullOrEmpty(dataPath))
                    {
                        dir = "Assets";
                    }
                    else
                    {
                        dir = new EditorAssetInfo(dataPath).dirPath;
                    }
                }
            }
            string path = dir + "/" + name;
            return CreateAssetFileWithPath<T>(path);
        }

        public static T CreateAssetFileWithPath<T>(string path) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }

        /// <summary>
        /// 绘制 按钮-> <立即写入修改数据到文件>
        /// 
        /// @@@ 建议所有.Asset文件的Editor都配备此段代码
        /// 
        /// </summary>
        /// <param name="target">Editor.target</param>
        public static void Draw_AssetFileApplyImmediateUI(UnityEngine.Object target)
        {
            GUILayout.Space(13);
            GUI.color = Color.yellow;
            if (GUILayout.Button(new GUIContent("Save Data To Asset Immediate", "立即将修改的数据保存到文件"), GUILayout.Height(26)))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
            GUI.color = Color.white;
        }

        // ------------------------------------------ UsedTags 

        #region 计数方法集合

        private static Dictionary<string, int> UsedTags;

        /// <summary>
        /// 计数Tag机制
        /// 
        /// 添加一个计数
        /// 
        /// </summary>
        public static int AddUsedTag(string tag)
        {
            if (UsedTags == null)
            {
                UsedTags = new Dictionary<string, int>();
                UsedTags.Add(tag, 1);
                return UsedTags[tag];
            }

            if (UsedTags.ContainsKey(tag))
            {
                UsedTags[tag]++;
                return UsedTags[tag];
            }
            else
            {
                UsedTags.Add(tag, 1);
                return UsedTags[tag];
            }
        }

        public static int SubUsedTag(string tag)
        {
            if (UsedTags == null)
            {
                return -1;
            }
            if (UsedTags.ContainsKey(tag))
            {
                UsedTags[tag]--;
                return UsedTags[tag];
            }
            else
            {
                return -1;
            }
        }

        public static int UsedTagCount(string tag)
        {
            if (UsedTags == null)
            {
                return 0;
            }
            if (UsedTags.ContainsKey(tag))
            {
                return UsedTags[tag];
            }
            else
            {
                return 0;
            }
        }

        #endregion

        // ------------------------------------------ UsedTags End

        // ------------------------------------------ PlusDefindWindow 

        #region 关于Unity编辑界面预制窗口的方法集合

        public enum PlusDefindWindow
        {
            AnimationWindow,
        }

        private static string _getPlusDefindWindowFullName(PlusDefindWindow defind)
        {
            switch (defind)
            {
                case PlusDefindWindow.AnimationWindow:
                    return "UnityEditor.AnimationWindow";
            }
            return null;
        }

        public static EditorWindow GetPlusDefindWindow(PlusDefindWindow defind)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(EditorWindow));
            string fullName = _getPlusDefindWindowFullName(defind);
            if (string.IsNullOrEmpty(fullName)) return null;
            Type t = assembly.GetType(fullName);
            if (t == null) return null;
            EditorWindow aw = EditorWindow.GetWindow(t);
            if (aw == null) return null;
            return aw;
        }

        #endregion

        // ------------------------------------------ PlusDefindWindow End


    }

}
