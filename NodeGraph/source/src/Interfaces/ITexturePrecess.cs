using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.NodeGraph
{
    public interface ITexturePrecess
    {
        /// <summary>
        /// 处理结果描述
        /// </summary>
        string ResultInfoDescribe();

        /// <summary>
        /// NodeGraph Texture自定义处理方法
        /// </summary>
        /// <param name="texture">输入的Texture</param>
        /// <returns>返回true为通过Process</returns>
        bool TextureProcess(Texture texture, ref List<string> ResultInfoList);

    }
}
