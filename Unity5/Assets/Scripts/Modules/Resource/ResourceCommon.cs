//#define Resource_UseLog

using AorBaseUtility;
using UnityEngine;
using YoukiaCore;

namespace YoukiaUnity.Resource
{

    /// <summary>
    /// 资源管理器的常用类
    /// </summary>
    public class ResourceCommon
    {

        /// <summary>
        /// 默认的包路径
        /// </summary>
        public static string assetbundleFilePath = Application.dataPath + "/StreamingAssets/";
        /// <summary>
        /// 包后缀
        /// </summary>
        public const string assetbundleFileSuffix = ".bytes";



        /// <summary>
        /// 获取资源文件名
        /// </summary>
        /// <param name="resPathName">传入的路径应为不带后缀格式</param>
        /// <returns></returns>
        public static string GetResourceName(string resPathName)
        {
            int index = resPathName.LastIndexOf("/");
            if (index == -1)
                return resPathName;
            else
            {
                return resPathName.Substring(index + 1, resPathName.Length - index - 1);
            }
        }

        /// <summary>
        /// 获得文件名
        /// </summary>
        /// <param name="filePath">实际的路径</param>
        /// <param name="removeSuffix">是否要返回后缀</param>
        /// <returns></returns>
        public static string GetFileName(string filePath, bool removeSuffix)
        {
            if (removeSuffix)
            {
                string path = filePath.Replace("\\", "/");
                int index = path.LastIndexOf("/");
                if (-1 == index)
                {
                    Log.Error("输入路径中没有/");
                    return "";
                }

                int index2 = path.LastIndexOf(".");
                if (-1 == index2)
                {
                    Log.Error("输入路径中没有.");
                    return "";
                }

                return path.Substring(index + 1, index2 - index - 1);
            }
            else
            {
                string path = filePath.Replace("\\", "/");
                int index = path.LastIndexOf("/");
                if (-1 == index)
                {
                    Log.Error("输入路径中没有/");
                    return "";
                }

                return path.Substring(index + 1, path.Length - index - 1);
            }
        }


        /// <summary>
        /// 获得文件路径中的文件夹路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFolder(string path)
        {
            path = path.Replace("\\", "/");
            int index = path.LastIndexOf("/");
            if (-1 == index)
            {
                Log.Warning("输入路径中没有文件夹");
                return "";
            }

            return path.Substring(0, index + 1);
        }

        /// <summary>
        /// 获得文件后缀名
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileSuffix(string filePath)
        {
            int index = filePath.LastIndexOf(".");
            if (-1 == index)
            {
                Log.Error("改文件无后缀!");
                return "";
            }

            return filePath.Substring(index + 1, filePath.Length - index - 1);
        }




        /// <summary>
        /// 根据平台获得对应的包内流媒体文件路径
        /// </summary>
        /// <param name="p_filename">资源路径，不带后缀</param>
        /// <returns></returns>
        public static string GetStreamingAssetsPath(string p_filename)
        {

            string suffix = "";

            if (!string.IsNullOrEmpty(p_filename))
                suffix = ResourceCommon.assetbundleFileSuffix;

            string _strPath = "";
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                _strPath = "file://" + Application.streamingAssetsPath + "/" + p_filename + suffix;
            else if (Application.platform == RuntimePlatform.Android)
            {
                _strPath = Application.streamingAssetsPath + "/" + p_filename + suffix;
            }
            else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.IPhonePlayer)
                _strPath = "file://" + Application.streamingAssetsPath + "/" + p_filename + suffix;

            return _strPath;
        }


        /// <summary>
        /// 获得外部更新文件夹路径
        /// </summary>
        public static string GetURLPath(string p_filename, string PreFix, string Suffix)
        {
            string path;
            path = "";

            if (Application.platform == RuntimePlatform.OSXEditor)
                path = Application.persistentDataPath + "/" + p_filename;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
                path = ResourcesManager.RESROOT + p_filename;


            if (Application.platform == RuntimePlatform.WindowsEditor)
                path = Application.dataPath + "/StreamingAssets/" + p_filename;

            if (Application.platform == RuntimePlatform.WindowsPlayer)
                path = Application.dataPath + "/StreamingAssets/" + p_filename;

            if (Application.platform == RuntimePlatform.Android)
                path = ResourcesManager.RESROOT + p_filename;

            path = PreFix + path;
            path = path + Suffix;

            return path;
        }
    }

}