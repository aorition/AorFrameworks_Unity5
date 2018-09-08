using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{

    public class EditorAssetInfo
    {

        private static string m_fullPathHeader;
        private static string FullPathHeader
        {
            get
            {
                if (string.IsNullOrEmpty(m_fullPathHeader))
                {
                    m_fullPathHeader = Application.dataPath.Replace("Assets", "");
                }
                return m_fullPathHeader;
            }
        }

        /// <summary>
        /// Assets路径转Resoures路径
        /// </summary>
        public static string AssetPathToResourcePath(string path)
        {
            int index = path.LastIndexOf("resources/", StringComparison.OrdinalIgnoreCase); //忽略大小写
            path = path.Substring(index.Equals(-1) ? 0 : index + 10);
            int num = path.LastIndexOf('.');
            if (num != -1) path = path.Substring(0, num);
            return path;
        }

        /// <summary>
        /// 在Selection中获取EditorAssetInfo列表,包含选中的文件夹下的所有文件
        /// </summary>
        public static List<EditorAssetInfo> FindEditorAssetInfosInSelection()
        {
            List<EditorAssetInfo> targetList = new List<EditorAssetInfo>();

            UnityEngine.Object[] obj = Selection.objects;
            if (obj != null && obj.Length > 0)
            {

                int i, len = obj.Length;
                for (i = 0; i < len; i++)
                {
                    EditorAssetInfo info = new EditorAssetInfo(obj[i]);
                    if (string.IsNullOrEmpty(info.suffix))
                    {
                        //目录
                        if (Directory.Exists(info.FullPath))
                        {
                            List<EditorAssetInfo> getInfos = FindEditorAssetInfoInPath(info.FullPath);
                            if (getInfos != null && getInfos.Count > 0)
                            {
                                int v, vlen = getInfos.Count;
                                for (v = 0; v < vlen; v++)
                                {
                                    if (!targetList.Contains(getInfos[v]))
                                    {
                                        targetList.Add(getInfos[v]);
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        //文件
                        if (!targetList.Contains(info))
                        {
                            targetList.Add(info);
                        }
                    }
                }

                if (targetList.Count > 0)
                {
                    return targetList;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取输入路径之下的所有EditorAssetInfo
        /// </summary>
        /// <param name="fullPath">完整路径</param>
        /// <param name="loadAsset">是否自动加载Asset</param>
        /// <returns></returns>
        public static List<EditorAssetInfo> FindEditorAssetInfoInPath(string fullPath, bool loadAsset = false)
        {
            return FindEditorAssetInfoInPath(fullPath, "*.*", loadAsset);
        }

        /// <summary>
        /// 获取输入路径之下的所有EditorAssetInfo
        /// </summary>
        /// <param name="fullPath">完整路径</param>
        /// <param name="searchPattern">搜索模式匹配的名称</param>
        /// <param name="loadAsset">是否自动加载Asset</param>
        /// <returns></returns>
        public static List<EditorAssetInfo> FindEditorAssetInfoInPath(string fullPath, string searchPattern, bool loadAsset = false)
        {

            if (!Directory.Exists(fullPath)) return null;

            List<EditorAssetInfo> list = new List<EditorAssetInfo>();
            string[] files = Directory.GetFiles(fullPath, searchPattern, SearchOption.AllDirectories);
            if (files.Length > 0)
            {

                int i, len = files.Length;
                for (i = 0; i < len; i++)
                {
                    string filePath = files[i].Replace("\\", "/");
                    string assetPath = filePath.Replace(FullPathHeader, "");
                    if (loadAsset)
                    {
                        UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
                        if (asset != null)
                        {
                            EditorAssetInfo info = new EditorAssetInfo(asset);
                            list.Add(info);
                        }
                    }
                    else
                    {
                        EditorAssetInfo info = new EditorAssetInfo(assetPath);
                        list.Add(info);
                    }
                }
                return list;
            }
            return null;
        }

        public EditorAssetInfo(string assetPath)
        {
            _init(assetPath);
        }

        /// <summary>
        /// 是否自动加载Asset
        /// </summary>
        public EditorAssetInfo(string assetPath, bool loadAsset)
        {
            if (loadAsset)
            {
                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                if (asset)
                {
                    _asset = asset;
                    _init(assetPath);
                }
            }
            else
            {
                _init(assetPath);
            }
        }

        public EditorAssetInfo(UnityEngine.Object asset)
        {
            _asset = asset;
            if (_asset == null) return;
            _init(AssetDatabase.GetAssetPath(_asset));
        }

        ~EditorAssetInfo()
        {
            _asset = null;
        }

        public bool isInit { get; private set; }

        private void _init(string inputPath)
        {
            path = inputPath.Replace("\\", "/"); 
            name = path.Substring(path.LastIndexOf('/') + 1);
            if (path.LastIndexOf('.') != -1)
            {
                suffix = path.Substring(path.LastIndexOf('.')).ToLower();
                if (name.LastIndexOf('.') != -1)
                {
                    name = name.Substring(0, name.LastIndexOf('.'));
                }
            }
            else
            {
                suffix = string.Empty;
            }
            if (path.LastIndexOf('/') != -1)
            {
                dirPath = string.IsNullOrEmpty(suffix) ? path : path.Substring(0, path.LastIndexOf('/'));
            }
            else
            {
                dirPath = string.IsNullOrEmpty(suffix) ? path : "/";
            }

            isInit = true;
        }

        public override bool Equals(object obj)
        {
            EditorAssetInfo i = obj as EditorAssetInfo;
            if (i == null)
            {
                return false;
            }
            else
            {
                if (i.name == this.name && i.dirPath == this.dirPath && i.suffix == this.suffix)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private UnityEngine.Object _asset;
        public UnityEngine.Object asset
        {
            get { return _asset; }
        }

        /// <summary>
        /// 文件路径(Assets格式,包含后缀名)
        /// </summary>
        public string path { get; private set; }

        /// <summary>
        /// 文件名(不包含后缀名)
        /// </summary>
        public string name { get; private set; }
        /// <summary>
        /// 后缀名(以.开始)
        /// </summary>
        public string suffix { get; private set; }

        /// <summary>
        /// 后缀名(以.开始) (兼容window后续名的惯用写法)
        /// </summary>
        public string extension => suffix;

        /// <summary>
        ///所在文件夹的路径(Assets格式)
        /// </summary>
        public string dirPath { get; private set; }

        /// <summary>
        /// 完整文件路径(包含全路径,文件名,后缀名)
        /// </summary>
        public string fullPath
        {
            get { return FullPathHeader + path; }
        }

        /// <summary>
        /// 文件所在文件夹名称(仅仅是文件夹名称)
        /// </summary>
        public string dirName
        {
            get
            {
                return dirPath.Substring(dirPath.LastIndexOf('/') + 1);
            }
        }

        /// <summary>
        /// 文件所在上级文件夹路径(Assets格式)
        /// </summary>
        public string parentDirPath
        {
            get { return dirPath.Substring(0, dirPath.LastIndexOf('/')); }
        }

        [Obsolete("统一字段首字都为小写,已由fullPath替代")]
        public string FullPath
        {
            get { return fullPath; }
        }

    }
}