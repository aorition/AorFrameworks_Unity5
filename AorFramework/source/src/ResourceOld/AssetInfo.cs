using System.Collections.Generic;
using UnityEngine;

namespace YoukiaUnity.Resource
{
    /// <summary>
    /// 资源文件详细信息记录器
    /// </summary>
    public class AssetInfo
    {

        /// <summary>
        /// 文件路径相对Resources 
        /// </summary>
        public string Path;

        /// <summary>
        /// 包体大小=bundleSize*4
        /// </summary>
        public int Size=10;


        /// <summary>
        /// 层数
        /// </summary>
        public int Level;
        /// <summary>
        /// GUID
        /// </summary>
        public string Guid;
        /// <summary>
        /// 资源类型，同名文件时用
        /// </summary>   
        public string ResType;
        /// <summary>
        /// 所有下级依赖
        /// </summary>
        public List<string> AllDependencies;

        /// <summary>
        /// 交叉引用计数
        /// </summary>
        public int refCount = 0;
        private string _directories = "-1";


        /// <summary>
        /// 返回从Assets开始长路径
        /// </summary>
        public string longPath
        {

            get
            {

                return "Assets/Resources/" + Path;

            }

        }
        /// <summary>
        /// 返回从Assets开始的AB包保存路径，包名为GUID
        /// </summary>
        public string SavePath
        {

            get
            {
                string tmp = "Assets/StreamingAssets/StreamingResources/" + Directories + Guid;
                return tmp;
            }


        }
        /// <summary>
        /// 返回计算机绝对路径，包名为GUID
        /// </summary>
        public string URLSavePath
        {

            get
            {
                string tmp = Application.dataPath + "/StreamingAssets/StreamingResources/" + Directories + Guid;
                return tmp;
            }


        }


        /// <summary>
        /// 返回ab包的读取路径
        /// </summary>
        public string LoadPath
        {

            get
            {
                string tmp = Directories + Guid;
                return tmp;
            }


        }

        /// <summary>
        /// 返回存放的文件夹路径
        /// </summary>
        public string Directories
        {

            get
            {
                if (_directories == "-1")
                {
                    string path = Path.Replace("\\", "/");
                    int index = path.LastIndexOf("/");
                    if (-1 == index)
                        return "";
                    _directories = path.Substring(0, index + 1);

                }

                return _directories;
            }


        }


        /// <summary>
        /// 不包含后缀的原始路径,从Resources文件夹内开始
        /// </summary>
        public string PathWithOutSuffix
        {
            get
            {
                return Path.Substring(0, Path.Length - Suffix.Length - 1);
            }
        }


        /// <summary>
        /// 文件名
        /// </summary>
        public string Name
        {

            get
            {

                return ResourceCommon.GetFileName(Path, false);

            }

        }

        public string NameWithOutSuffix {

            get { return ResourceCommon.GetFileName(Path, true); }

        }

        /// <summary>
        /// 后缀名
        /// </summary>
        public string Suffix
        {

            get
            {

                return ResourceCommon.GetFileSuffix(Path);

            }

        }


    }

}