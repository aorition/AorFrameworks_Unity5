using UnityEngine;
using UnityEditor;
using System.IO;
#if UNITY_5
#else
    using System.Text.RegularExpressions;
#endif

namespace Framework.UI.Editor.Utility
{
    public class ReimportUnityEngineUI
    {

        /// <summary>
        /// 用于修复Unity编辑器有时莫名其妙丢失UnityEngine.UI的系列脚本.
        /// </summary>
        [MenuItem("FrameworkTools/杂项/重建UI索引")]
        public static void ReimportUI()
        {
#if UNITY_5
            //unity 5.0 or higher
            var path = EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/GUISystem/{1}";
            var version = string.Empty;
#else
            // unity4
            var path = EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/GUISystem/{0}/{1}";
            var version = Regex.Match(Application.unityVersion, @"^[0-9]+\.[0-9]+\.[0-9]+").Value;
#endif

            string engineDll = string.Format(path, version, "UnityEngine.UI.dll");
            string editorDll = string.Format(path, version, "Editor/UnityEditor.UI.dll");
            ReimportDll(engineDll);
            ReimportDll(editorDll);

        }
        static void ReimportDll(string path)
        {
            if (File.Exists(path))
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.DontDownloadFromCacheServer);


            }
            else
                Debug.LogError(string.Format("DLL not found {0}", path));
        }
    }
}


