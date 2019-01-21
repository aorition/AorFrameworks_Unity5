using System;
using System.Collections.Generic;
using System.Reflection;
using AorBaseUtility;
using Framework;
using UnityEditor;
using UnityEngine;


namespace Framework.Editor
{

    /// <summary>
    /// 提供Editor下常用功能静态方法
    /// </summary>
    public class EditorPlusMethods
    {

        //-------------------------- GUIContents Define ---------------------

        private static GUIContent m_CompilingLabel;
        private static GUIContent CompilingLabel
        {
            get
            {
                if (m_CompilingLabel == null)
                {
                    m_CompilingLabel = new GUIContent("Compiling Please Wait...", "正在编译中 ...");
                }
                return m_CompilingLabel;
            }
        }



        //-------------------------- GUIContents Define ----------------  end

        private static EditorApplication.CallbackFunction _UDDoOnce;
        private static Action _UDDoOnceDos;
        /// <summary>
        /// 编辑器在下一次Update时调用
        /// </summary>
        public static void NextEditorApplicationUpdateDo(Action doSomething)
        {

            _UDDoOnceDos += doSomething;
            if (_UDDoOnce == null)
            {
                _UDDoOnce = () =>
                {
                    if (_UDDoOnceDos != null)
                    {
                        Action doing = _UDDoOnceDos;
                        doing();
                        _UDDoOnceDos = null;
                    }
                    EditorApplication.update -= _UDDoOnce;
                    _UDDoOnce = null;
                };
                EditorApplication.update += _UDDoOnce;
            }


        }

        /// <summary>
        /// 在 Project 中创建一个ScriptableObject子类文件
        /// </summary>
        /// <typeparam name="T">ScriptableObject子类</typeparam>
        public static T CreateAssetFile<T>(string assetName) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
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
            if (GUILayout.Button("立即写入数据到文件 (Save To Asset Immediate)", GUILayout.Height(26)))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
            GUI.color = Color.white;
        }

        /// <summary>
        /// 如果正在编译阶段 则绘制 提示UI  
        /// @@@ 建议所有EditorWindow.OnGUI都配备此段代码
        /// </summary>
        public static bool Draw_isCompilingUI()
        {
            if (EditorApplication.isCompiling)
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(CompilingLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 如果正在编译阶段 则绘制 提示 Notification UI  
        /// @@@ 建议所有EditorWindow.OnGUI都配备此段代码
        /// </summary>
        public static bool Draw_isCompilingNotification (EditorWindow editorWindow) {

            #region 正在编译提示
            if (EditorApplication.isCompiling)
            {
                editorWindow.ShowNotification(new GUIContent("Compiling Please Wait..."));
                editorWindow.Repaint();
                return true;
            }
            editorWindow.RemoveNotification();
            #endregion
            return false;
        }




        /// <summary>
        /// 绘制 测试窗口大小信息
        /// 
        /// @ 要求必须写在GUI绘制代码的末尾.
        /// @@ 测试信息显示会占用10像素的屏幕高度
        /// 
        /// </summary>
        public static void Draw_DebugWindowSizeUI()
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(0,0, Screen.width,24), "WindowSize > width : " + Screen.width + " , height : " + Screen.height);
            GUI.color = Color.white;
        }

        /// <summary>
        /// 修改Texture对应TextureImporter的设置
        /// </summary>
        public static void SetTextureImportSetting(Texture texture, bool isReadable, TextureImporterFormat textureImporterFormat, TextureImporterCompression textureCompression, TextureWrapMode wrapMode, TextureImporterType textureType, TextureImporterNPOTScale nPOTScale, MipMapSetting mipMapSettin, bool sRGBTexture = true, int anisoLevel = 1)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            if (!string.IsNullOrEmpty(path))
            {
                SetTextureImportSetting(path, isReadable, textureImporterFormat, textureCompression, wrapMode, textureType, nPOTScale, mipMapSettin, sRGBTexture, anisoLevel);
            }
            else
            {
                Debug.LogError("EditorPlusMethods.SetTextureImportSetting Error :: can not get the texture path -> " + texture);
            }
        }
        /// <summary>
        /// 修改Texture对应TextureImporter的设置
        /// </summary>
        public static void SetTextureImportSetting(string path, bool isReadable, TextureImporterFormat textureImporterFormat, TextureImporterCompression textureCompression, TextureWrapMode wrapMode, TextureImporterType textureType, TextureImporterNPOTScale nPOTScale, MipMapSetting mipMapSetting, bool sRGBTexture = true, int anisoLevel = 1)
        {
            TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
            if (importer)
            {
                TextureImporterPlatformSettings settings = importer.GetDefaultPlatformTextureSettings();
                settings.format = textureImporterFormat;
                settings.textureCompression = textureCompression;

                importer.isReadable = isReadable;
                importer.textureType = textureType;
                importer.wrapMode = wrapMode;

                //TextureImporterPlatformSettings中已经设置过了,这里就不需要再设值了.
                //                importer.textureCompression = textureCompression;

                importer.sRGBTexture = sRGBTexture;
                importer.npotScale = nPOTScale;
                importer.anisoLevel = anisoLevel;

                if (mipMapSetting.isInit)
                {
                    importer.mipmapEnabled = true;
                    importer.mipMapBias = mipMapSetting.mipMapBias;
                    importer.mipmapFilter = mipMapSetting.mipFilter;
                    importer.mipmapFadeDistanceStart = mipMapSetting.mipmapFadeDistanceStart;
                    importer.mipmapFadeDistanceEnd = mipMapSetting.mipmapFadeDistanceEnd;
                    importer.borderMipmap = mipMapSetting.borderMipmap;
                }
                else
                {
                    importer.mipmapEnabled = false;
                }

                importer.SetPlatformTextureSettings(settings);

                importer.SaveAndReimport();
            }
            else
            {
                Debug.LogError("EditorPlusMethods.SetTextureImportSetting Error :: can not get TextureImporter at the path -> " + path);
            }
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

        #region 已废弃的API

        /// <summary>
        /// 创建制定对象的撤消点
        /// </summary>
        [Obsolete("对于Undo的再封装不灵活,没实际意义")]
        public static void RegisterUndo(string name, params UnityEngine.Object[] objects)
        {
            if (objects != null && objects.Length > 0)
            {
                UnityEditor.Undo.RecordObjects(objects, name);

                foreach (UnityEngine.Object obj in objects)
                {
                    if (obj == null) continue;
                    EditorUtility.SetDirty(obj);
                }
            }
        }

        [Obsolete("并没有存在的意义")]
        public static void SaveAssetFileInProject(string dir, string assetName, ScriptableObject fileObj)
        {
            string name = assetName + ".asset";
            string path = dir + "/" + name;
            SaveAssetFileInProject(path, fileObj);
        }

        [Obsolete("并没有存在的意义")]
        public static void SaveAssetFileInProject(string path, ScriptableObject fileObj)
        {
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(fileObj, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Obsolete("并没有存在的意义")]
        public static void SavePNGFileInProject(string path, byte[] bytes)
        {
            AorIO.SaveBytesToFile(path, bytes);
        }

        #endregion

    }

}
