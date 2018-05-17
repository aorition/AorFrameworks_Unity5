using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

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
        public static void Dispose(this Transform trans)
        {
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
        public static string getHierarchyPath(this Transform tran)
        {
            return _getHierarchPathLoop(tran);
        }
        private static string _getHierarchPathLoop(Transform t, string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = t.gameObject.name;
            }
            else
            {
                path = t.gameObject.name + "/" + path;
            }

            if (t.parent != null)
            {
                return _getHierarchPathLoop(t.parent, path);
            }
            else
            {
                return path;
            }
        }

        #region Layer

        /// <summary>
        /// 递归子节点设置Layer
        /// </summary>
        /// <param name="ignoreLayer">忽略层</param>
        public static void SetLayer(this Transform tran, int layer, int ignoreLayer)
        {
            SetLayer(tran, layer, t => t.gameObject.layer != ignoreLayer);
        }

        /// <summary>
        /// 递归子节点设置Layer
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// </summary>
        public static void SetLayer(this Transform tran, int layer, Func<Transform, bool> filter = null,
            Action<Transform> doSomething = null)
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

        #endregion

        #region Interface

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
        /// 获取当前Transform父集上挂载的接口
        /// 
        /// 默认API扩展，GetComponentInParent
        /// 
        /// </summary>
        public static T GetInterfaceInParent<T>(this Transform tran) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                return null;
            }

            Component[] cp = tran.gameObject.GetComponentsInParent<Component>();
            if (cp != null && cp.Length > 0)
            {
                int i, len = cp.Length;
                for (i = 0; i < len; i++)
                {
                    if (cp[i] is T)
                    {
                        T t = cp[i] as T;
                        return t;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取当前Transform子集上挂载的接口
        /// 包含隐藏或者未激活的节点
        /// </summary>
        public static T FindInterfaceInChildren<T>(this Transform tran, bool incudeSelf = false) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                return null;
            }

            List<Component> cp = tran.FindComponentListInChildren<Component>(incudeSelf);
            if (cp != null && cp.Count > 0)
            {
                int i, len = cp.Count;
                for (i = 0; i < len; i++)
                {
                    if (cp[i] is T)
                    {
                        T t = cp[i] as T;
                        return t;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取当前Transform父集上挂载的接口
        /// 包含隐藏或者未激活的节点
        /// </summary
        public static T FindInterfaceInParent<T>(this Transform tran, bool incudeSelf = false) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                return null;
            }

            List<Component> cp = tran.FindComponentListInParent<Component>();
            if (cp != null && cp.Count > 0)
            {
                int i, len = cp.Count;
                for (i = 0; i < len; i++)
                {
                    if (cp[i] is T)
                    {
                        T t = cp[i] as T;
                        return t;
                    }
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
        public static List<T> GetInterfaceListInChlidren<T>(this Transform tran) where T : class
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
            List<T> list = tran.GetInterfaceListInChlidren<T>();
            if (list != null)
            {
                return list.ToArray();
            }
            return null;
        }

        /// <summary>
        /// 获取当前Transform对象上挂载的接口集合
        /// 
        /// 默认API扩展，GetComponentsInParent
        /// 
        /// </summary>
        public static List<T> GetInterfaceListInParent<T>(this Transform tran) where T : class
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
        /// 默认API扩展，GetComponentsInParent
        /// 
        /// </summary>
        public static T[] GetInterfacesInParent<T>(this Transform tran) where T : class
        {
            List<T> list = tran.GetInterfaceListInParent<T>();
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
        public static List<T> FindInterfaceListInChildren<T>(this Transform tran, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
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
        public static T[] FindInterfacesInChildren<T>(this Transform tran, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
        {
            List<T> list = tran.FindInterfaceListInChildren<T>(incudeSelf, filter, doSomething);
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
        public static List<T> FindInterfaceListInParent<T>(this Transform tran, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                return null;
            }

            List<Component> cp = tran.FindComponentListInParent<Component>(incudeSelf);
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
        public static T[] FindInterfacesInParent<T>(this Transform tran, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : class
        {
            List<T> list = FindInterfaceListInParent(tran, incudeSelf, filter, doSomething);
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
        public static List<T> FindAllInterfaceList<T>(this Transform tran, Func<T, bool> filter = null,
            Action<T> doSomething = null) where T : class
        {

            if (!typeof(T).IsInterface)
            {
                return null;
            }

            List<Component> cp = tran.FindAllComponentList<Component>();
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
        public static T[] FindAllInterfaces<T>(this Transform tran, Func<T, bool> filter = null,
            Action<T> doSomething = null) where T : class
        {
            List<T> list = tran.FindAllInterfaceList<T>(filter, doSomething);
            if (list != null)
            {
                return list.ToArray();
            }
            return null;
        }

        #endregion

        #region Component

        /// <summary>
        /// 查找或者创建Component(当前Component在当前节点对象找不到,则在当前对象上创建Component)
        /// </summary>
        public static T GetOrCreateComponent<T>(this Transform tran) where T : Component
        {
            return tran.gameObject.GetOrCreateComponent<T>();
        }

        /// <summary>
        /// 返回Root节点以下的所有Component,包含隐藏或者未激活的节点
        /// <typeparam name="T">Component</typeparam>
        /// <param name="incudeRoot">是否包含Root节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// </summary>
        public static List<T> FindAllComponentList<T>(this Transform trans, Func<T, bool> filter = null,
            Action<T> doSomething = null) where T : Component
        {
            return trans.root.FindComponentListInChildren(true, filter, doSomething);
        }

        /// <summary>
        /// 返回Root节点以下的所有Component,包含隐藏或者未激活的节点
        /// <typeparam name="T">Component</typeparam>
        /// <param name="incudeRoot">是否包含Root节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// </summary>
        public static T[] FindAllComponents<T>(this Transform trans, Func<T, bool> filter = null,
            Action<T> doSomething = null) where T : Component
        {
            List<T> list = FindAllComponentList<T>(trans, filter, doSomething);
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
        public static List<T> FindComponentListInChildren<T>(this Transform trans, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
        {
            List<T> list = new List<T>();
            if (incudeSelf)
            {
                T cpt = trans.GetComponent<T>();
                if (cpt != null)
                {
                    list.Add(cpt);
                }
            }
            _findComponentInChildrenLoop<T>(trans, ref list, filter, doSomething);
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
        public static T[] FindComponentsInChildren<T>(this Transform trans, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
        {
            List<T> list = trans.FindComponentListInChildren<T>(incudeSelf, filter, doSomething);
            if (list != null)
            {
                return list.ToArray();
            }
            return null;
        }

        /// <summary>
        /// 按照节点顺序返回所有上级节点的Component,包含隐藏或者未激活的节点;
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        /// <param name="incudeSelf">是否包含自身节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// <returns></returns>
        public static List<T> FindComponentListInParent<T>(this Transform trans, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
        {
            List<T> list = new List<T>();
            if (incudeSelf)
            {
                T cpt = trans.GetComponent<T>();
                if (cpt != null)
                {
                    list.Add(cpt);
                }
            }
            _findComponentInParentLoop<T>(trans, ref list, filter, doSomething);
            if (list.Count > 0)
            {
                return list;
            }
            return null;
        }

        /// <summary>
        /// 按照节点顺序返回所有上级节点的Component,包含隐藏或者未激活的节点;
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        /// <param name="incudeSelf">是否包含自身节点</param>
        /// <param name="filter">过滤器：返回True为通过过滤</param>
        /// <param name="doSomething">遍历时附加行为</param>
        /// <returns></returns>
        public static T[] FindComponentsInParent<T>(this Transform trans, bool incudeSelf = false,
            Func<T, bool> filter = null, Action<T> doSomething = null) where T : Component
        {
            List<T> list = trans.FindComponentListInParent<T>(incudeSelf, filter, doSomething);
            if (list != null)
            {
                return list.ToArray();
            }
            return null;
        }

        /// <summary>
        /// 向上级节点查找组件,并返回第一个找到的组件,包含隐藏或者未激活的节点;
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        public static T FindComponentInParent<T>(this Transform trans, bool incudeSelf = false) where T : Component
        {
            if (incudeSelf)
                return _findComponentInParent<T>(trans);

            if (trans.parent)
            {
                return _findComponentInParent<T>(trans.parent);
            }
            return null;
        }
        private static T _findComponentInParent<T>(Transform t) where T : Component
        {
            T[] cpts = t.GetComponents<T>();
            if (cpts != null && cpts.Length > 0)
            {
                int c, cLen = cpts.Length;
                for (c = 0; c < cLen; c++)
                {
                    T cpt = cpts[c];
                    if (cpt)
                    {
                        return cpt;
                    }
                }
            }
            if (t.parent)
            {
                return _findComponentInParent<T>(t.parent);
            }
            return null;
        }

        /// <summary>
        /// 向下级节点查找组件,并返回第一个找到的组件,包含隐藏或者未激活的节点;
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        public static T FindComponentInChildren<T>(this Transform trans, bool incudeSelf = false) where T : Component
        {
            if (incudeSelf)
                return _findComponentInChildren<T>(trans);

            if (trans.childCount > 0)
            {
                T findComponent;
                for (int i = 0; i < trans.childCount; i++)
                {
                    Transform ct = trans.GetChild(i);
                    findComponent = _findComponentInChildren<T>(ct);
                    if (findComponent)
                    {
                        return findComponent;
                    }
                }
            }
            return null;
        }
        private static T _findComponentInChildren<T>(Transform t) where T : Component
        {
            T[] cpts = t.GetComponents<T>();
            if (cpts != null && cpts.Length > 0)
            {
                int c, cLen = cpts.Length;
                for (c = 0; c < cLen; c++)
                {
                    T cpt = cpts[c];
                    if (cpt)
                    {
                        return cpt;
                    }
                }
            }
            if (t.childCount > 0)
            {
                T findComponent;
                for (int i = 0; i < t.childCount; i++)
                {
                    Transform ct = t.GetChild(i);
                    findComponent = _findComponentInChildren<T>(ct);
                    if (findComponent)
                    {
                        return findComponent;
                    }
                }
            }
            return null;
        }

        private static void _findComponentInChildrenLoop<T>(Transform t, ref List<T> list, Func<T, bool> filter = null,
            Action<T> doSomething = null) where T : Component
        {
            int i, len = t.childCount;
            for (i = 0; i < len; i++)
            {
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
                if (ct.childCount > 0)
                {
                    _findComponentInChildrenLoop<T>(ct, ref list);
                }
            }
        }

        private static void _findComponentInParentLoop<T>(Transform t, ref List<T> list, Func<T, bool> filter = null,
            Action<T> doSomething = null) where T : Component
        {
            if (t.parent)
            {
                Transform ct = t.parent;
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
                _findComponentInParentLoop<T>(t.parent, ref list, filter, doSomething);
            }
        }

        #endregion

    }

}


