using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AorBaseUtility;
using AorFramework;
using UnityEditor;
using UnityEngine;


namespace Framework.Editor
{

    /// <summary>
    /// 提供Editor下常用功能静态方法
    /// </summary>
    public class EditorPlusMethods
    {

        /// <summary>
        /// 创建制定对象的撤消点
        /// </summary>
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

        public static void SaveAssetFileInProject(string dir,string assetName, ScriptableObject fileObj)
        {
            string name = assetName + ".asset";
            string path = dir + "/" + name;
            SaveAssetFileInProject(path, fileObj);
        }

        public static void SaveAssetFileInProject(string path, ScriptableObject fileObj)
        {
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(fileObj, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void SavePNGFileInProject(string path, byte[] bytes)
        {
            AorIO.SaveBytesToFile(path, bytes);
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

                //对JScriptableObject持久化做特殊处理
                if (target is JScriptableObject)
                {
                    JScriptableObject jso = target as JScriptableObject;
                    jso.ClearInnerJsonData();//重置_innerJsonValue
                    string json = JSONEncoder.ToJSON(jso);
                    target.SetNonPublicField("_innerJsonValue", json);
//                    if ((bool) jso.GetNonPublicField("m_justUseJsonData"))
//                    {
//                        jso.ClearSerializeData();
//                        EditorUtility.SetDirty(jso);
//                        AssetDatabase.SaveAssets();
//                        AssetDatabase.Refresh();
//                    }
                }

                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
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


    }



}
