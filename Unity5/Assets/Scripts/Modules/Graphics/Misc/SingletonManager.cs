using System;
using System.Collections.Generic;
using UnityEngine;

namespace YoukiaUnity.Misc
{
    /// <summary>
    /// 单利管理器基类
    /// </summary>
    /// <typeparam name="T">管理器类型</typeparam>
    public class SingletonManager<T> : MonoBehaviour
         where T : Component
    {

        private static Dictionary<string, Component> _SingletonDic;

        /// <summary>
        /// 获得当前类的实例
        /// </summary>
        public static T CreateInstance()
        {

            if (_SingletonDic == null)
            {
                _SingletonDic = new Dictionary<string, Component>();
            }

            string key = typeof(T).Name;

            if (_SingletonDic.ContainsKey(key))
            {
                return _SingletonDic[key] as T;
            }
            else
            {
                GameObject obj = new GameObject();
                obj.name = key;
                if (Application.isPlaying)
                    DontDestroyOnLoad(obj);
                Component cp = obj.AddComponent(typeof(T));
                _SingletonDic.Add(key, cp);
                return cp as T;
            }

        }

        public static T GetInstance(bool notbeNULL = false)
        {
            if (notbeNULL)
            {
                return CreateInstance();
            }
            else
            {
                if (_SingletonDic == null) return null;

                string key = typeof(T).Name;
                if (_SingletonDic.ContainsKey(key))
                {
                    return _SingletonDic[key] as T;
                }
                else
                {
                    return null;
                }
            }
        }

    }

}


