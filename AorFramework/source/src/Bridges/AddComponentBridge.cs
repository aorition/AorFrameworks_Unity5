using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 提供AddComponent的桥接功能
    /// 
    /// (尚未验证此桥接类的实用性)
    /// 
    /// </summary>
    public class AddComponentBridge
    {

        public static Func<GameObject, string, Component> AddComponentCustom;
        public static Component AddComponent(GameObject go, string componentName)
        {
            if (AddComponentCustom != null)
            {
                return AddComponentCustom(go, componentName);
            }

            //default:
            Assembly ab = Assembly.GetExecutingAssembly();
            Type bType = ab.GetType(componentName);
            return go.AddComponent(bType);
        }

        public static T AddComponent<T>(GameObject go, string componentName) where T : Component
        {
            if (AddComponentCustom != null)
            {
                return (T)AddComponentCustom(go, componentName);
            }

            //default:
            Assembly ab = Assembly.GetExecutingAssembly();
            Type bType = ab.GetType(componentName);
            return go.AddComponent(bType) as T;
        }

        public static Func<GameObject, Assembly,string, Component> AddComponentInAssemblyCustom;
        public static Component AddComponent(GameObject go, Assembly assembly, string componentName)
        {
            if (AddComponentInAssemblyCustom != null)
            {
                return AddComponentInAssemblyCustom(go, assembly, componentName);
            }

            //default:
            Type bType = assembly.GetType(componentName);
            return go.AddComponent(bType);
        }

        public static T AddComponent<T>(GameObject go, Assembly assembly, string componentName) where T : Component
        {
            if (AddComponentInAssemblyCustom != null)
            {
                return (T)AddComponentInAssemblyCustom(go, assembly, componentName);
            }

            //default:
            Type bType = assembly.GetType(componentName);
            return go.AddComponent(bType) as T;
        }

    }
}
