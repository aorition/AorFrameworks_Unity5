using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.editor
{

    public class EditorAssetInfo
    {

        /// <summary>
        /// Assets路径转Resoures路径
        /// </summary>
        public static string AssetPathToResourcePath(string path)
        {
            if (path.LastIndexOf('.') != -1)
            {
                path = path.Substring(0, path.LastIndexOf('.'));
            }
            return path.Replace("Assets/", "");
        }

        /// <summary>
        /// Resoures路径转Assets路径
        /// 
        /// ** suffix形参 必须以 . 开头
        /// 
        /// </summary>
        public static string ResourcePathToAssetPath(string path, string suffix)
        {
            return "Assets/" + path + suffix;
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
        public static List<EditorAssetInfo> FindEditorAssetInfoInPath(string fullPath,string searchPattern, bool loadAsset = false)
        {

            if (!Directory.Exists(fullPath)) return null;

            List<EditorAssetInfo> list = new List<EditorAssetInfo>();
            string[] files = Directory.GetFiles(fullPath, searchPattern, SearchOption.AllDirectories);
            if (files != null && files.Length > 0)
            {

                int i, len = files.Length;
                for (i = 0; i < len; i++)
                {
                    string filePath = files[i].Replace("\\", "/");
                    string assetPath = filePath.Replace(Application.dataPath + "/", "Assets/");
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
            _path = assetPath;
            _name = _path.Substring(_path.LastIndexOf('/') + 1);
            if (_path.LastIndexOf('.') != -1)
            {
                _suffix = _path.Substring(_path.LastIndexOf('.')).ToLower();
                if (_name.LastIndexOf('.') != -1)
                {
                    _name = _name.Substring(0, _name.LastIndexOf('.'));
                }
            }
            else
            {
                _suffix = "";
            }
            if (_path.LastIndexOf('/') != -1)
            {
                _dirPath = _path.Substring(0, _path.LastIndexOf('/'));
            }
            else
            {
                _dirPath = "/";
            }
        }

        public EditorAssetInfo(UnityEngine.Object asset)
        {
            _asset = asset;
            update();
        }

        ~EditorAssetInfo()
        {
            _asset = null;
        }

        public void update()
        {

            if (_asset == null) return;

            //FBX路径(包含文件名和后缀)
            _path = AssetDatabase.GetAssetPath(_asset);
            _name = _asset.name;
            if (_path.LastIndexOf('.') != -1)
            {
                _suffix = _path.Substring(_path.LastIndexOf('.')).ToLower();
            }
            else
            {
                _suffix = "";
            }
            if (_path.LastIndexOf('/') != -1)
            {
                _dirPath = _path.Substring(0, _path.LastIndexOf('/'));
            }
            else
            {
                _dirPath = "/";
            }
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

        private string _name;
        /// <summary>
        /// 文件名(不包含后缀名)
        /// </summary>
        public string name
        {
            get { return _name; }
        }

        private string _suffix;
        /// <summary>
        /// 后缀名(以.开始)
        /// </summary>
        public string suffix
        {
            get { return _suffix; }
        }

        private string _dirPath;
        /// <summary>
        /// 文件夹路径(以Assets开头)
        /// </summary>
        public string dirPath
        {
            get { return _dirPath; }
        }

        /// <summary>
        /// 文件夹路径(以Resources开头)
        /// </summary>
        public string resDirPath {
            get
            {
                if (_path.Contains("Assets/Resources/"))
                {
                    return _dirPath.Replace("Assets/", "");
                }
                return null;
            }
        }

        private string _path;
        /// <summary>
        /// 以Assets开头的文件路径(包含后缀名)
        /// </summary>
        public string path
        {
            get { return _path; }
        }

        /// <summary>
        /// 完整文件路径(包含全路径,文件名,后缀名)
        /// </summary>
        public string FullPath
        {
            get { return Application.dataPath.Replace("Assets", "") + _path; }
        }

        /// <summary>
        /// 以Resources开头的文件路径(不包含后缀名)
        /// </summary>
        public string resPath
        {
            get
            {
                if (_path.Contains("Assets/Resources/"))
                {
                    return _dirPath.Replace("Assets/", "") + "/" + _name;
                }
                return null;
            }
        }

        /// <summary>
        /// 文件所在文件夹名称(以Assets开头)
        /// </summary>
        public string dirName
        {
            get
            {
                return _dirPath.Substring(_dirPath.LastIndexOf('/') + 1);
                ;
            }
        }

        /// <summary>
        /// 文件所在上级文件夹路径(已Assets开头)
        /// </summary>
        public string parentDirPath
        {
            get { return _dirPath.Substring(0, _dirPath.LastIndexOf('/')); }
        }

    }
}