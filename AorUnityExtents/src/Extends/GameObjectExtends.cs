using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Framework.Extends
{

    /// <summary>
    /// Aor扩展方法封装集合
    /// </summary>
    public static class GameObjectExtends
    {
        public static void Dispose(this GameObject obj)
        {
            if (Application.isPlaying)
            {
                GameObject.Destroy(obj);
            }
            else
            {
                GameObject.DestroyImmediate(obj);
            }
        }

        public static void DisposeChildren(this GameObject obj)
        {
            obj.transform.DisposeChildren();
        }

        /// <summary>
        /// 获取对象在Hierarchy中的节点路径
        /// </summary>
        public static string getHierarchyPath(this GameObject obj)
        {
            return obj.transform.getHierarchyPath();
        }

        #region Layer

        /// <summary>
        /// 递归子节点设置Layer
        /// </summary>
        public static void SetLayer(this GameObject obj, int layer, int ignoreLayer)
        {
            obj.transform.SetLayer(layer, ignoreLayer);
        }

        /// <summary>
        /// 递归子节点设置Layer
        /// </summary>
        public static void SetLayer(this GameObject obj, int layer, Func<Transform, bool> filter = null,
            Action<Transform> doSomething = null)
        {
            obj.transform.SetLayer(layer, filter, doSomething);
        }

        #endregion

        #region Interface 

        /// <summary>
        /// 获取当前Transform对象上挂载的接口
        /// </summary>
        public static T GetInterface<T>(this GameObject obj) where T : class
        {
            return obj.transform.GetInterface<T>();
        }

        /// <summary>
        /// 获取当前Transform对象上挂载的接口集合
        /// </summary>
        public static List<T> GetInterfacesList<T>(this GameObject obj) where T : class
        {
            return obj.transform.GetInterfacesList<T>();
        }

        /// <summary>
        /// 获取当前Transform对象上挂载的接口集合
        /// </summary>
        public static T[] GetInterfaces<T>(this GameObject obj) where T : class
        {
            return obj.transform.GetInterfaces<T>();
        }

        /// <summary>
        /// 获取当前Transform对象或者子节点上挂载的接口集合
        /// 
        ///默认API扩展，GetComponentsInChildren 
        /// 
        /// </summary>
        public static T GetInterfaceInChlidren<T>(this GameObject obj) where T : class
        {
            return obj.transform.GetInterfaceInChlidren<T>();
        }

        /// <summary>
        /// 获取当前Transform父集上挂载的接口
        /// 
        /// 默认API扩展，GetComponentInParent
        /// 
        /// </summary>
        public static T GetInterfaceInParent<T>(this GameObject obj) where T : class
        {
            return obj.transform.GetInterfaceInParent<T>();
        }

        /// <summary>
        /// 获取当前Transform子集上挂载的接口
        /// 包含隐藏或者未激活的节点
        /// </summary>
        public static T FindInterfaceInChildren<T>(this GameObject obj, bool incudeSelf = false) where T : class
        {
            return obj.transform.FindInterfaceInChildren<T>(incudeSelf);
        }

        /// <summary>
        /// 获取当前Transform父集上挂载的接口
        /// 包含隐藏或者未激活的节点
        /// </summary
        public static T FindInterfaceInParent<T>(this GameObject obj, bool incudeSelf = false) where T : class
        {
            return obj.transform.FindInterfaceInParent<T>(incudeSelf);
        }

        /// <summary>
        /// 获取当前Transform对象上挂载的接口集合
        /// 
        ///默认API扩展，GetComponentsInChildren
        /// 
        /// </summary>
        public static List<T> GetInterfaceListInChlidren<T>(this GameObject obj) where T : class
        {
            return obj.transform.GetInterfaceListInChlidren<T>();
        }

        /// <summary>
        /// 获取当前Transform对象上挂载的接口集合
        /// 
        ///默认API扩展，GetComponentsInChildren
        /// 
        /// </summary>
        public static T[] GetInterfacesInChlidren<T>(this GameObject obj) where T : class
        {
            return obj.transform.GetInterfacesInChlidren<T>();
        }

        /// <summary>
        /// 获取当前Transform对象上挂载的接口集合
        /// 
        /// 默认API扩展，搜索精度同GetComponentsInParent
        /// 
        /// </summary>
        public static List<T> GetInterfaceListInParent<T>(this GameObject obj) where T : class
        {
            return obj.transform.GetInterfaceListInParent<T>();
        }

        /// <summary>
        /// 获取当前Transform对象上挂载的接口集合
        /// 
        /// 默认API扩展，搜索精度同GetComponentsInParent
        /// 
        /// </summary>
        public static T[] GetInterfacesInParent<T>(this GameObject obj) where T : class
        {
            return obj.transform.GetInterfacesInParent<T>();
        }

        /// <summary>
        /// 返回当前节点以下的Interface,包含隐藏或者未激活的节点
        /// <param name="incudeSelf">是否包含自身节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// </summary>
        public static List<T> FindInterfaceListInChildren<T>(this GameObject obj, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
        {
            return obj.transform.FindInterfaceListInChildren<T>(incudeSelf, filter, doSomething);
        }

        /// <summary>
        /// 返回当前节点以下的Interface,包含隐藏或者未激活的节点
        /// <param name="incudeSelf">是否包含自身节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// </summary>
        public static T[] FindInterfacesInChildren<T>(this GameObject obj, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
        {
            return obj.transform.FindInterfacesInChildren<T>(incudeSelf, filter, doSomething);
        }

        /// <summary>
        /// 返回当前节点以下的Interface,包含隐藏或者未激活的节点
        /// <param name="incudeSelf">是否包含自身节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// </summary>
        public static List<T> FindInterfaceListInParent<T>(this GameObject obj, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
        {
            return obj.transform.FindInterfaceListInParent<T>(incudeSelf, filter, doSomething);
        }

        /// <summary>
        /// 返回当前节点以下的Interface,包含隐藏或者未激活的节点
        /// <param name="incudeSelf">是否包含自身节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// </summary>
        public static T[] FindInterfacesInParent<T>(this GameObject obj, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
        {
            return obj.transform.FindInterfacesInParent<T>(incudeSelf, filter, doSomething);
        }

        /// <summary>
        /// 返回Root节点以下的所有Interface,包含隐藏或者未激活的节点
        /// <param name="incudeRoot">是否包含Root节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// </summary>
        public static List<T> FindAllInterfaceList<T>(this GameObject obj, Func<T, bool> filter = null,
            Action<T> doSomething = null) where T : class
        {
            return obj.transform.FindAllInterfaceList<T>(filter, doSomething);
        }

        /// <summary>
        /// 返回Root节点以下的所有Interface,包含隐藏或者未激活的节点
        /// <param name="incudeRoot">是否包含Root节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// </summary>
        public static T[] FindAllInterfaces<T>(this GameObject obj, Func<T, bool> filter = null,
            Action<T> doSomething = null) where T : class
        {
            return obj.transform.FindAllInterfaces<T>(filter, doSomething);
        }

        #endregion

        #region Component

        /// <summary>
        /// 查找或者创建Component(当前Component在当前节点对象找不到,则在当前对象上创建Component)
        /// </summary>
        public static T GetOrCreateComponent<T>(this GameObject obj) where T : Component
        {
            T cp = obj.GetComponent<T>();
            if (!cp)
            {
                cp = obj.AddComponent<T>();
            }
            return cp;
        }

        /// <summary>
        /// 返回Root节点以下的所有Component,包含隐藏或者未激活的节点
        /// <typeparam name="T">Component</typeparam>
        /// <param name="incudeRoot">是否包含Root节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// </summary>
        public static List<T> FindAllComponentList<T>(this GameObject obj, Func<T, bool> filter = null,
            Action<T> doSomething = null) where T : Component
        {
            return obj.transform.FindAllComponentList<T>(filter, doSomething);
        }

        /// <summary>
        /// 返回Root节点以下的所有Component,包含隐藏或者未激活的节点
        /// <typeparam name="T">Component</typeparam>
        /// <param name="incudeRoot">是否包含Root节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// </summary>
        public static T[] FindAllComponents<T>(this GameObject obj, Func<T, bool> filter = null,
            Action<T> doSomething = null) where T : Component
        {
            return obj.transform.FindAllComponents<T>(filter, doSomething);
        }


        /// <summary>
        /// 按照节点顺序返回所有子节点的Component,包含隐藏或者未激活的节点;
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        /// <param name="incudeSelf">是否包含自身节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// <returns></returns>
        public static List<T> FindComponentListInChildren<T>(this GameObject obj, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
        {
            return obj.transform.FindComponentListInChildren<T>(incudeSelf, filter, doSomething);
        }

        /// <summary>
        /// 按照节点顺序返回所有子节点的Component,包含隐藏或者未激活的节点;
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        /// <param name="incudeSelf">是否包含自身节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// <returns></returns>
        public static T[] FindComponentsInChildren<T>(this GameObject obj, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
        {
            return obj.transform.FindComponentsInChildren<T>(incudeSelf, filter, doSomething);
        }

        /// <summary>
        /// 按照节点顺序返回所有上级节点的Component,包含隐藏或者未激活的节点;
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        /// <param name="incudeSelf">是否包含自身节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// <returns></returns>
        public static List<T> FindComponentListInParent<T>(this GameObject obj, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
        {
            return obj.transform.FindComponentListInParent<T>(incudeSelf, filter, doSomething);
        }

        /// <summary>
        /// 按照节点顺序返回所有上级节点的Component,包含隐藏或者未激活的节点;
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        /// <param name="incudeSelf">是否包含自身节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// <returns></returns>
        public static T[] FindComponentsInParent<T>(this GameObject obj, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
        {
            return obj.transform.FindComponentsInParent(incudeSelf, filter, doSomething);
        }

        /// <summary>
        /// 向上级节点查找组件,并返回第一个找到的组件,包含隐藏或者未激活的节点;
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        public static T FindComponentInParent<T>(this GameObject obj, bool incudeSelf = false) where T : Component
        {
            return obj.transform.FindComponentInParent<T>(incudeSelf);
        }

        /// <summary>
        /// 向下级节点查找组件,并返回第一个找到的组件,包含隐藏或者未激活的节点;
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        public static T FindComponentInChildren<T>(this GameObject obj, bool incudeSelf = false) where T : Component
        {
            return obj.transform.FindComponentInChildren<T>(incudeSelf);
        }



        #endregion

    }

}


