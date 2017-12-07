using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aor扩展方法封装集合
/// 
/// 
/// **  原生 GetComponentsInChildren   方法，会包含自身节点上挂在的Component；
///          GetComponentsInParent     方法，会包含自身节点上挂在的Component；
/// 
/// </summary>
public static class TransformExtends
{
    public static void Dispose(this Transform trans) {
        trans.gameObject.Dispose();
    }

    public static void DisposeChildren(this Transform tran)
    {
        List<GameObject> dels = new List<GameObject>();
        int i, len = tran.childCount;
        for (i = 0; i < len; i++)
        {
            GameObject d = tran.GetChild(i).gameObject;
            dels.Add(d);
        }
        len = dels.Count;
        for (i = 0; i < len; i++)
        {
            dels[i].Dispose();
        }
    }

    /// <summary>
    /// 获取对象在Hierarchy中的节点路径
    /// </summary>
    public static string getHierarchyPath(this Transform tran) {
        return _getHierarchPathLoop(tran);
    }
    private static string _getHierarchPathLoop(Transform t, string path = null) {
        if (string.IsNullOrEmpty(path)) {
            path = t.gameObject.name;
        }
        else {
            path = t.gameObject.name + "/" + path;
        }

        if (t.parent != null) {
            return _getHierarchPathLoop(t.parent, path);
        }
        else {
            return path;
        }
    }

    /// <summary>
    /// 递归子节点设置Layer
    /// <param name="filter">过滤器：返回True为通过过滤</param>
    /// <param name="doSomething">遍历时附加行为</param>
    /// </summary>
    public static void SetLayer(this Transform tran, int layer, Func<Transform, bool> filter = null, Action<Transform> doSomething = null)
    {
        if (filter != null)
        {
            if (filter(tran))
            {
                tran.gameObject.layer = layer;
                if (doSomething != null) doSomething(tran);
            }
        }
        else
        {
            tran.gameObject.layer = layer;
            if (doSomething != null) doSomething(tran);
        }
        if (tran.childCount > 0)
        {

            int i, len = tran.childCount;
            for (i = 0; i < len; i++)
            {
                Transform sub = tran.GetChild(i);
                sub.SetLayer(layer, filter, doSomething);
            }
        }
    }

    /// <summary>
    /// 获取当前Transform对象上挂载的接口
    /// </summary>
    public static T GetInterface<T>(this Transform tran) where T : class
    {
        if (!typeof(T).IsInterface)
        {
            return null;
        }
        //return inObj.GetComponents<Component>().OfType<T>().FirstOrDefault();
        Component[] cp = tran.gameObject.GetComponents<Component>();
        int i, length = cp.Length;
        for (i = 0; i < length; i++)
        {
            if (cp[i] is T)
            {
                T t = cp[i] as T;
                return t;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取当前Transform对象上挂载的接口集合
    /// </summary>
    public static List<T> GetInterfacesList<T>(this Transform tran) where T : class
    {
        if (!typeof(T).IsInterface)
        {
            return null;
        }
        Component[] cp = tran.gameObject.GetComponents<Component>();
        if (cp != null && cp.Length > 0)
        {
            int i, len = cp.Length;
            List<T> list = new List<T>();
            for (i = 0; i < len; i++)
            {
                if (cp[i] is T)
                {
                    T a = cp[i] as T;
                    list.Add(a);
                }
            }
            if (list != null && list.Count > 0)
            {
                return list;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取当前Transform对象上挂载的接口集合
    /// </summary>
    public static T[] GetInterfaces<T>(this Transform tran) where T : class
    {
        List<T> list = tran.GetInterfacesList<T>();
        if (list != null)
        {
            return list.ToArray();
        }
        return null;
    }

    /// <summary>
    /// 获取当前Transform对象或者子节点上挂载的接口集合
    /// 
    ///默认API扩展，GetComponentsInChildren 
    /// 
    /// </summary>
    public static T GetInterfaceInChlidren<T>(this Transform tran) where T : class
    {
        if (!typeof(T).IsInterface)
        {
            return null;
        }
        //return inObj.GetComponents<Component>().OfType<T>().FirstOrDefault();
        Component[] cp = tran.gameObject.GetComponentsInChildren<Component>();
        int i, length = cp.Length;
        for (i = 0; i < length; i++)
        {
            if (cp[i] is T)
            {
                T t = cp[i] as T;
                return t;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取当前Transform对象上挂载的接口集合
    /// 
    ///默认API扩展，GetComponentsInChildren
    /// 
    /// </summary>
    public static List<T> GetInterfacesListInChlidren<T>(this Transform tran) where T : class
    {
        if (!typeof(T).IsInterface)
        {
            return null;
        }

        Component[] cp = tran.gameObject.GetComponentsInChildren<Component>();
        
        if (cp != null && cp.Length > 0)
        {
            int i, len = cp.Length;
            List<T> list = new List<T>();
            for (i = 0; i < len; i++)
            {
                if (cp[i] is T)
                {
                    T t = cp[i] as T;
                    list.Add(t);
                }
            }
            if (list.Count > 0)
            {
                return list;
            }
        }

        return null;
    }

    /// <summary>
    /// 获取当前Transform对象上挂载的接口集合
    /// 
    ///默认API扩展，GetComponentsInChildren
    /// 
    /// </summary>
    public static T[] GetInterfacesInChlidren<T>(this Transform tran) where T : class
    {
        List<T> list = tran.GetInterfacesListInChlidren<T>();
        if (list != null)
        {
            return list.ToArray();
        }
        return null;
    }

    /// <summary>
    /// 获取当前Transform对象上挂载的接口集合
    /// 
    /// 默认API扩展，搜索精度同GetComponentsInParent
    /// 
    /// </summary>
    public static List<T> GetInterfacesListInParent<T>(this Transform tran) where T : class
    {
        if (!typeof(T).IsInterface)
        {
            return null;
        }

        Component[] cp = tran.gameObject.GetComponentsInParent<Component>();
        if (cp != null && cp.Length > 0)
        {
            int i, len = cp.Length;
            List<T> list = new List<T>();
            for (i = 0; i < len; i++)
            {
                if (cp[i] is T)
                {
                    T t = cp[i] as T;
                    list.Add(t);
                }
            }
            if (list.Count > 0)
            {
                return list;
            }
        }

        return null;
    }

    /// <summary>
    /// 获取当前Transform对象上挂载的接口集合
    /// 
    /// 默认API扩展，搜索精度同GetComponentsInParent
    /// 
    /// </summary>
    public static T[] GetInterfacesInParent<T>(this Transform tran) where T : class
    {
        List<T> list = tran.GetInterfacesListInParent<T>();
        if (list != null)
        {
            return list.ToArray();
        }
        return null;
    }

    /// <summary>
    /// 返回当前节点以下的Interface,包含隐藏或者未激活的节点
    /// <param name="incudeSelf">是否包含自身节点</param>
    /// <param name="filter">过滤器：返回True为通过过滤</param>
    /// <param name="doSomething">遍历时附加行为</param>
    /// </summary>
    public static List<T> FindInterfaceListInChildren<T>(this Transform tran, bool incudeSelf = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
    {
        if (!typeof(T).IsInterface)
        {
            return null;
        }

        List<Component> cp = tran.FindComponentListInChildren<Component>(incudeSelf);
        if (cp != null && cp.Count > 0)
        {
            List<T> list = new List<T>();
            int i, len = cp.Count;
            for (i = 0; i < len; i++)
            {
                if (cp[i] is T)
                {
                    T t = cp[i] as T;
                    if (filter != null)
                    {
                        if (filter(t))
                        {
                            list.Add(t);
                            if (doSomething != null) doSomething(t);
                        }
                    }
                    else
                    {
                        list.Add(t);
                        if (doSomething != null) doSomething(t);
                    }
                }
            }
            if (list.Count > 0)
            {
                return list;
            }
        }

        return null;
    }

    /// <summary>
    /// 返回当前节点以下的Interface,包含隐藏或者未激活的节点
    /// <param name="incudeSelf">是否包含自身节点</param>
    /// <param name="filter">过滤器：返回True为通过过滤</param>
    /// <param name="doSomething">遍历时附加行为</param>
    /// </summary>
    public static T[] FindInterfacesInChildren<T>(this Transform tran, bool incudeSelf = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
    {
        List<T> list = tran.FindInterfaceListInChildren<T>(incudeSelf, filter, doSomething);
        if (list != null)
        {
            return list.ToArray();
        }
        return null;
    }

    /// <summary>
    /// 返回Root节点以下的所有Interface,包含隐藏或者未激活的节点
    /// <param name="incudeRoot">是否包含Root节点</param>
    /// <param name="filter">过滤器：返回True为通过过滤</param>
    /// <param name="doSomething">遍历时附加行为</param>
    /// </summary>
    public static List<T> FindAllInterfaceList<T> (this Transform tran, bool incudeRoot = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
    {
        
        if (!typeof(T).IsInterface)
        {
            return null;
        }

        List<Component> cp = tran.FindAllComponentList<Component>(incudeRoot);
        if (cp != null && cp.Count > 0)
        {
            List<T> list = new List<T>();
            int i, len = cp.Count;
            for (i = 0; i < len; i++)
            {
                if (cp[i] is T)
                {
                    T t = cp[i] as T;
                    if (filter != null)
                    {
                        if (filter(t))
                        {
                            list.Add(t);
                            if (doSomething != null) doSomething(t);
                        }
                    }
                    else
                    {
                        list.Add(t);
                        if (doSomething != null) doSomething(t);
                    }
                }
            }
            if (list.Count > 0)
            {
                return list;
            }
        }
        
        return null;
    }

    /// <summary>
    /// 返回Root节点以下的所有Interface,包含隐藏或者未激活的节点
    /// <param name="incudeRoot">是否包含Root节点</param>
    /// <param name="filter">过滤器：返回True为通过过滤</param>
    /// <param name="doSomething">遍历时附加行为</param>
    /// </summary>
    public static T[] FindAllInterfaces<T>(this Transform tran, bool incudeRoot = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
    {
        List<T> list = tran.FindAllInterfaceList<T>(incudeRoot, filter, doSomething);
        if (list != null)
        {
            return list.ToArray();
        }
        return null;
    }

    /// <summary>
    /// 查找或者创建Component(当前Component在当前节点对象找不到,则在当前对象上创建Component)
    /// </summary>
    public static T AddMissingComponent<T>(this Transform tran) where T : Component
    {
        return tran.gameObject.AddMissingComponent<T>();
    }

    /// <summary>
    /// 返回Root节点以下的所有Component,包含隐藏或者未激活的节点
    /// <typeparam name="T">Component</typeparam>
    /// <param name="incudeRoot">是否包含Root节点</param>
    /// <param name="filter">过滤器：返回True为通过过滤</param>
    /// <param name="doSomething">遍历时附加行为</param>
    /// </summary>
    public static List<T> FindAllComponentList<T>(this Transform trans, bool incudeRoot = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
    {
        List<T> list = new List<T>();
        if (incudeRoot)
        {
            T cpt = trans.root.GetComponent<T>();
            if (cpt != null)
            {
                list.Add(cpt);
            }
        }
        _findComponentLoop<T>(trans, ref list, filter, doSomething);
        if (list.Count > 0)
        {
            return list;
        }
        return null;
    }

    /// <summary>
    /// 返回Root节点以下的所有Component,包含隐藏或者未激活的节点
    /// <typeparam name="T">Component</typeparam>
    /// <param name="incudeRoot">是否包含Root节点</param>
    /// <param name="filter">过滤器：返回True为通过过滤</param>
    /// <param name="doSomething">遍历时附加行为</param>
    /// </summary>
    public static T[] FindAllComponents<T>(this Transform trans, bool incudeRoot = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
    {
        List<T> list = trans.FindAllComponentList<T>(incudeRoot, filter, doSomething);
        if (list != null)
        {
            return list.ToArray();
        }
        return null;
    }


    /// <summary>
    /// 按照节点顺序返回所有子节点的Component,包含隐藏或者未激活的节点;
    /// </summary>
    /// <typeparam name="T">Component</typeparam>
    /// <param name="incudeSelf">是否包含自身节点</param>
    /// <param name="filter">过滤器：返回True为通过过滤</param>
    /// <param name="doSomething">遍历时附加行为</param>
    /// <returns></returns>
    public static List<T> FindComponentListInChildren<T>(this Transform trans, bool incudeSelf = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component {
        List<T> list = new List<T>();
        if (incudeSelf)
        {
            T cpt = trans.GetComponent<T>();
            if (cpt != null)
            {
                list.Add(cpt);
            }
        }
        _findComponentLoop<T>(trans, ref list, filter, doSomething);
        if (list.Count > 0)
        {
            return list;
        }
        return null;
    }

    /// <summary>
    /// 按照节点顺序返回所有子节点的Component,包含隐藏或者未激活的节点;
    /// </summary>
    /// <typeparam name="T">Component</typeparam>
    /// <param name="incudeSelf">是否包含自身节点</param>
    /// <param name="filter">过滤器：返回True为通过过滤</param>
    /// <param name="doSomething">遍历时附加行为</param>
    /// <returns></returns>
    public static T[] FindComponentsInChildren<T>(this Transform trans, bool incudeSelf = false, Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
    {
        List<T> list = trans.FindComponentListInChildren<T>(incudeSelf, filter, doSomething);
        if (list != null)
        {
            return list.ToArray();
        }
        return null;
    }

    private static void _findComponentLoop<T>(Transform t, ref List<T> list, Func<T,bool> filter = null, Action<T> doSomething = null) where T : Component {
        int i, len = t.childCount;
        for (i = 0; i < len; i++) {
            Transform ct = t.GetChild(i);
            T[] cpts = ct.GetComponents<T>();
            if (cpts != null && cpts.Length > 0)
            {
                int c, cLen = cpts.Length;
                for (c = 0; c < cLen; c++)
                {
                    T cpt = cpts[c];
                    if (cpt)
                    {
                        if (filter != null)
                        {
                            if (filter(cpt)) list.Add(cpt);
                            if (doSomething != null) doSomething(cpt);
                        }
                        else
                        {
                            list.Add(cpt);
                            if (doSomething != null) doSomething(cpt);
                        }
                    }
                }
            }
            if (ct.childCount > 0) {
                _findComponentLoop<T>(ct, ref list);
            }
        }
    }
}
