using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aor扩展方法封装集合
/// </summary>
public static class GameObjectExtends
{
    public static void Dispose(this GameObject obj)
    {
        if (Application.isEditor)
        {
            GameObject.DestroyImmediate(obj);
        }
        else
        {
            GameObject.Destroy(obj);
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

    /// <summary>
    /// 递归子节点设置Layer
    /// </summary>
    public static void SetLayer(this GameObject obj, int layer)
    {
        obj.transform.SetLayer(layer);
    }
    
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
    /// 获取当前Transform对象上挂载的接口集合
    /// 
    ///默认API扩展，GetComponentsInChildren
    /// 
    /// </summary>
    public static List<T> GetInterfacesListInChlidren<T>(this GameObject obj) where T : class
    {
        return obj.transform.GetInterfacesListInChlidren<T>();
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
    public static List<T> GetInterfacesListInParent<T>(this GameObject obj) where T : class
    {
        return obj.transform.GetInterfacesListInParent<T>();
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
    public static List<T> FindInterfaceListInChildren<T>(this GameObject obj, bool incudeSelf = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
    {
        return obj.transform.FindInterfaceListInChildren<T>(incudeSelf, filter, doSomething);
    }

    /// <summary>
    /// 返回当前节点以下的Interface,包含隐藏或者未激活的节点
    /// <param name="incudeSelf">是否包含自身节点</param>
    /// <param name="filter">过滤器：返回True为通过过滤</param>
    /// <param name="doSomething">遍历时附加行为</param>
    /// </summary>
    public static T[] FindInterfacesInChildren<T>(this GameObject obj, bool incudeSelf = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
    {
        return obj.transform.FindInterfacesInChildren<T>(incudeSelf, filter, doSomething);
    }

    /// <summary>
    /// 返回Root节点以下的所有Interface,包含隐藏或者未激活的节点
    /// <param name="incudeRoot">是否包含Root节点</param>
    /// <param name="filter">过滤器：返回True为通过过滤</param>
    /// <param name="doSomething">遍历时附加行为</param>
    /// </summary>
    public static List<T> FindAllInterfaceList<T>(this GameObject obj, bool incudeRoot = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
    {
        return obj.transform.FindAllInterfaceList<T>(incudeRoot, filter, doSomething);
    }

    /// <summary>
    /// 返回Root节点以下的所有Interface,包含隐藏或者未激活的节点
    /// <param name="incudeRoot">是否包含Root节点</param>
    /// <param name="filter">过滤器：返回True为通过过滤</param>
    /// <param name="doSomething">遍历时附加行为</param>
    /// </summary>
    public static T[] FindAllInterfaces<T>(this GameObject obj, bool incudeRoot = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
    {
        return obj.transform.FindAllInterfaces<T>(incudeRoot, filter, doSomething);
    }

    /// <summary>
    /// 查找或者创建Component(当前Component在当前节点对象找不到,则在当前对象上创建Component)
    /// </summary>
    public static T AddMissingComponent<T>(this GameObject obj) where T : Component
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
    public static List<T> FindAllComponentList<T>(this GameObject obj, bool incudeRoot = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
    {
        return obj.transform.FindAllComponentList<T>(incudeRoot, filter, doSomething);
    }

    /// <summary>
    /// 返回Root节点以下的所有Component,包含隐藏或者未激活的节点
    /// <typeparam name="T">Component</typeparam>
    /// <param name="incudeRoot">是否包含Root节点</param>
    /// <param name="filter">过滤器：返回True为通过过滤</param>
    /// <param name="doSomething">遍历时附加行为</param>
    /// </summary>
    public static T[] FindAllComponents<T>(this GameObject obj, bool incudeRoot = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
    {
        return obj.transform.FindAllComponents<T>(incudeRoot, filter, doSomething);
    }


    /// <summary>
    /// 按照节点顺序返回所有子节点的Component,包含隐藏或者未激活的节点;
    /// </summary>
    /// <typeparam name="T">Component</typeparam>
    /// <param name="incudeSelf">是否包含自身节点</param>
    /// <param name="filter">过滤器：返回True为通过过滤</param>
    /// <param name="doSomething">遍历时附加行为</param>
    /// <returns></returns>
    public static List<T> FindComponentListInChildren<T>(this GameObject obj, bool incudeSelf = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
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
    public static T[] FindComponentsInChildren<T>(this GameObject obj, bool incudeSelf = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
    {
        return obj.transform.FindComponentsInChildren<T>(incudeSelf, filter, doSomething);
    }

}
