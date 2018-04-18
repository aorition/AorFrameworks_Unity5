using System;
using UnityEngine;

namespace Framework
{
    public class ShaderBridge
    {
        /// <summary>
        /// 查找Shader的注入方法
        /// </summary>
        public static Func<string, Shader> CustomFind;
        /// <summary>
        /// 查找Shader的桥接方法
        /// </summary>
        /// <param name="shaderName"></param>
        /// <param name="callback"></param>
        public static Shader Find(string shaderName)
        {
            if (CustomFind != null)
            {
                return CustomFind(shaderName);
            }

            //default:
            return Shader.Find(shaderName);
        }


        /// <summary>
        /// 查找Shader的注入方法 (异步)
        /// </summary>
        public static Action<string, Action<Shader>> CustomFindAsync;
        /// <summary>
        /// 查找Shader的桥接方法 (异步)
        /// </summary>
        public static void FindAsync(string shaderName, Action<Shader> callBack)
        {
            if (CustomFind != null)
            {
                CustomFindAsync(shaderName, callBack);
                return;
            }

            //default:
            callBack(Shader.Find(shaderName));
        }

    }
}
