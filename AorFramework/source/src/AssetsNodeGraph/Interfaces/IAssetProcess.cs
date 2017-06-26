using System;
using System.Collections.Generic;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public interface IAssetProcess
    {
        /// <summary>
        /// 重置IPrefabProcess以便于初始化一系列标记/变量
        /// </summary>
        void Reset();

        /// <summary>
        /// 处理结果描述
        /// </summary>
        string ResultInfoDescribe();

        /// <summary>
        /// NodeGraph预制体自定义处理方法
        /// </summary>
        /// <returns>返回true为通过Process</returns>
        bool Process(string path, ref List<string> ResultInfoList);
    }
}
