using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{

    /// <summary>
    /// 默认Resource异步加载处理器
    /// </summary>
    public class LoadAsyncHandler : MonoBehaviour
    {

        private readonly Queue<string> _pathQueue = new Queue<string>();
        private readonly Queue<Type> _typeQueue = new Queue<Type>();
        private readonly Queue<Action<UnityEngine.Object, object[]>> _callbackQueue = new Queue<Action<Object, object[]>>();
        private readonly Queue<object[]> _parmsQueue = new Queue<object[]>();

        private bool _loading = false;

        private void OnEnable()
        {
            _init();
        }

        private void OnDisable()
        {
            _loading = false;
        }

        private void OnDestroy()
        {
            _pathQueue.Clear();
            _typeQueue.Clear();
            _callbackQueue.Clear();
            _parmsQueue.Clear();
        }

        private void _init()
        {
            if (!_loading && _pathQueue.Count > 0)
            {
                StartCoroutine(loadEnumerator());
                _loading = true;
            }
        }

        IEnumerator loadEnumerator()
        {
            while (true)
            {
                if (_pathQueue.Count == 0)
                {
                    _loading = false;
                    break;
                }

                string path = _pathQueue.Dequeue();
                Type type = _typeQueue.Dequeue();
                Action<UnityEngine.Object, object[]> callback = _callbackQueue.Dequeue();
                object[] parms = _parmsQueue.Dequeue();

                var request = Resources.LoadAsync(path, type);

                yield return request;

                if (request.isDone)
                {
                    if (callback != null)
                    {
                        callback(request.asset, parms);
                    }
                }

            }
        }

        public void LoadAsync(string path, Type objectType, Action<UnityEngine.Object, object[]> finishCallback, params object[] param)
        {
            _pathQueue.Enqueue(path);
            _typeQueue.Enqueue(objectType);
            _callbackQueue.Enqueue(finishCallback);
            _parmsQueue.Enqueue(param);

            _init();
        }

    }

    /// <summary>
    /// 提供载入资源的桥接功能
    /// </summary>
    public class ResourcesLoadBridge
    {
        
        //------------------------------------- 

        #region Load By Type (基础桥接)

        /// <summary>
        /// 加载UnityEngine.Object(非实例化) 注入方法
        /// 注意:在LoadMethodHook已注入的情况下 Load方法和LoadAsync方法都会调用LoadMethodHook来实现方法.
        /// </summary>
        public static Action<string, Type, Action<UnityEngine.Object, object[]>, object[]> LoadMethodHook;

        /// <summary>
        /// Load 加载UnityEngine.Object(非实例化)(默认行为是同步) 
        /// 以 Resource/ 为起始目录
        /// 特殊处理加载Sprite行为
        /// 默认加载行为的param第一个参数填true,则启用异步加载
        /// </summary>
        public static void Load(string path, Type objectType, Action<UnityEngine.Object, object[]> finishCallback, params object[] param)
        {
            if (objectType == typeof (Sprite))
            {
                string absPath = path;
                string spName = string.Empty;
                //sprite 特殊处理
                bool useSt = path.Contains("@");
                if (useSt)
                {
                    string[] sp = path.Split('@');
                    absPath = sp[0];
                    spName = sp[1];
                }
                LoadAll(absPath,objectType, (objs, param2) =>
                {
                    UnityEngine.Object[] assets = objs;
                    if (assets != null && assets.Length > 0)
                    {
                        if (string.IsNullOrEmpty(spName))
                        {
                            finishCallback(assets[0], param2);
                        }
                        else
                        {
                            for (int i = 0; i < assets.Length; i++)
                            {
                                if (assets[i].name == spName)
                                {
                                    finishCallback(assets[i], param2);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        finishCallback(null, param2);
                    }
                }, param);
            }
            else
            {
                if (LoadMethodHook != null)
                {
                    LoadMethodHook(path, objectType, finishCallback, param);
                    return;
                }
                //Default
                _defaultLoad(path, objectType, finishCallback, param);
            }
        }
        private static void _defaultLoad(string path, Type objectType, Action<UnityEngine.Object, object[]> finishCallback, params object[] param)
        {
            //异步加载参数判断
            if (param != null && param.Length > 0)
            {
                if ((bool)param[0])
                {
                    _defaultLoadAsync(path, objectType, finishCallback);
                    return;
                }
            }

            //Default
            UnityEngine.Object obj = Resources.Load(path, objectType);
            if (finishCallback != null)
            {
                finishCallback(obj, null);
            }
        }

        //------------------ LoadAsync ------------------

        private static string _lahGoLabelDefine = "LoadAsyncHandler";

        private static GameObject m_loadAsyncHandlerGo;
        protected static GameObject LoadAsyncHandlerGo
        {
            get
            {
                if (!m_loadAsyncHandlerGo)
                {
                    m_loadAsyncHandlerGo = GameObject.FindWithTag(_lahGoLabelDefine);
                    if (!m_loadAsyncHandlerGo)
                    {
                        m_loadAsyncHandlerGo = new GameObject(_lahGoLabelDefine);
                        m_loadAsyncHandlerGo.name = _lahGoLabelDefine;
                        m_loadAsyncHandlerGo.tag = _lahGoLabelDefine;
                    }
                }
                return m_loadAsyncHandlerGo;
            }
        }

        private static LoadAsyncHandler m_loadAsyncHandler;
        protected static LoadAsyncHandler LoadAsyncHandler
        {
            get
            {
                if (!m_loadAsyncHandler)
                {
                    m_loadAsyncHandler = LoadAsyncHandlerGo.GetComponent<LoadAsyncHandler>();
                    if (!m_loadAsyncHandler)
                    {
                        m_loadAsyncHandler = LoadAsyncHandlerGo.AddComponent<LoadAsyncHandler>();
                    }
                }
                return m_loadAsyncHandler;
            }
        }

        private static void _defaultLoadAsync(string path, Type objectType, Action<UnityEngine.Object, object[]> finishCallback, params object[] param)
        {
            //Default
            LoadAsyncHandler.LoadAsync(path, objectType, finishCallback, param);
        }

        //------------------ LoadAsync -------------- end

        #endregion

        #region LoadAll By Type (基础桥接)

        /// <summary>
        ///加载UnityEngine.Object(非实例化) 注入方法
        /// </summary>
        public static Action<string, Type, Action<UnityEngine.Object[], object[]>, object[]> LoadAllMethodHook;

        /// <summary>
        /// Load 加载UnityEngine.Object(非实例化)
        /// 以 Resource/ 为起始目录
        /// </summary>
        public static void LoadAll(string path, Type objectType, Action<UnityEngine.Object[], object[]> finishCallback, params object[] param)
        {

            if (LoadAllMethodHook != null)
            {
                LoadAllMethodHook(path, objectType, finishCallback, param);
                return;
            }

            //Default
            _defaultLoadAll(path, objectType, finishCallback, param);
        }
        private static void _defaultLoadAll(string path, Type objectType, Action<UnityEngine.Object[], object[]> finishCallback, params object[] param)
        {
            UnityEngine.Object[] assets = Resources.LoadAll(path, objectType);
            if (assets != null && assets.Length > 0)
            {
                if (finishCallback != null)
                {
                    finishCallback(assets, param);
                    return;
                }
            }
            if (finishCallback != null)
            {
                finishCallback(null, param);
            }
        }

        #endregion

        //-------------------------------------

        public static void Load<T>(string path, Action<T, object[]> finishCallback, params object[] param) where T : UnityEngine.Object
        {
            Load(path, typeof(T), (obj, param2) =>
            {
                if (obj != null)
                {
                    if (finishCallback != null)
                    {
                        finishCallback((T)obj, param2);
                        return;
                    }
                }
                if (finishCallback != null)
                {
                    finishCallback(null, param2);
                }

            }, param);
        }

        public static void LoadAll<T>(string path, Action<T[], object[]> finishCallback, params object[] param)
            where T : UnityEngine.Object
        {
            LoadAll(path, typeof(T), (objs, param2) =>
            {
                if (objs != null && objs.Length > 0)
                {
                    T[] pks = new T[objs.Length];
                    for (int i = 0; i < objs.Length; i++)
                    {
                        pks[i] = objs[i] as T;
                    }
                    if (finishCallback != null)
                    {
                        finishCallback(pks, param2);
                        return;
                    }
                }

                if (finishCallback != null)
                {
                    finishCallback(null, param2);
                }
            }, param);
        }

        #region LoadPrefab

        /// <summary>
        /// LoadPrefab 载入预制体(实例化) 注入方法,方便实现对象池设计实现
        /// </summary>
        public static Action<string, Action<GameObject, object[]>, object[]> LoadPrefabHook;

        /// <summary>
        /// LoadPrefab 载入预制体(实例化)
        /// 以 Resource/ 为起始目录
        /// </summary>
        public static void LoadPrefab(string path, Action<GameObject, object[]> finishCallback, params object[] param)
        {

            if (LoadPrefabHook != null)
            {
                LoadPrefabHook(path, finishCallback, param);
                return;
            }

            //Default
            Load<GameObject>(path, (asset, param2) =>
            {
                if (asset)
                {
                    GameObject ins = GameObject.Instantiate(asset);
                    ins.name = asset.name;
                    if (finishCallback != null)
                    {
                        finishCallback(ins, param2);
                    }
                    return;
                }
                if (finishCallback != null)
                {
                    finishCallback(null, param2);
                }
            }, param);
        }

        #endregion

        #region UnLoadPrefab

        /// <summary>
        /// 卸载 预制体(实例化) 注入方法
        /// </summary>
        public static Action<GameObject, object[]> UnLoadPrefabHook;

        /// <summary>
        /// 卸载 预制体(实例化)
        /// </summary>
        public static void UnLoadPrefab(GameObject prefab, params object[] param)
        {
            if (UnLoadPrefabHook != null)
            {
                UnLoadPrefabHook(prefab, param);
                return;
            }
            //Default
            _defaultUnLoadPrefab(prefab);
        }
        private static void _defaultUnLoadPrefab(GameObject prefab)
        {
            //Default
            if (!Application.isPlaying && Application.isEditor)
            {
                GameObject.DestroyImmediate(prefab);
            }
            else
            {
                GameObject.Destroy(prefab);
            }
        }

        #endregion

        //---------------------------------------------------

        #region 弃用方法和字段
        [Obsolete("多余的封装方法. Using Load<T>")]
        public static void LoadSprite(string path, Action<Sprite, object[]> finishCallback, params object[] param)
        {
            Load<Sprite>(path, finishCallback, param);
        }
        [Obsolete("多余的封装方法. Using Load<T>")]
        public static void LoadTextrue(string path, Action<Texture2D, object[]> finishCallback, params object[] param)
        {
            Load<Texture2D>(path, finishCallback, param);
        }

        public static void LoadPrefabAsset(string path, Action<GameObject, object[]> finishCallback, params object[] param)
        {
            Load<GameObject>(path, finishCallback, param);
        }
        [Obsolete("多余的封装方法. Using Load<T>")]
        public static void LoadScriptableObject(string path, Action<ScriptableObject, object[]> finishCallback, params object[] param)
        {
            Load<ScriptableObject>(path, finishCallback, param);
        }
        [Obsolete("Using LoadMethodHook")]
        public static Action<string, Type, Action<UnityEngine.Object, object[]>, object[]> CustomLoad
        {
            get { return LoadMethodHook; }
            set { LoadMethodHook = value; }
        }
        [Obsolete("Using LoadAllMethodHook")]
        public static Action<string, Type, Action<UnityEngine.Object[], object[]>, object[]> CustomLoadAll
        {
            get { return LoadAllMethodHook; }
            set { LoadAllMethodHook = value; }
        }

        #endregion

    }
}
