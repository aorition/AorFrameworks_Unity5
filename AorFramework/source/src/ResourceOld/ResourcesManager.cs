using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YoukiaCore;
using YoukiaUnity.Misc;
using Object = UnityEngine.Object;

namespace YoukiaUnity.Resource
{
    /// <summary>
    /// 1 放在工程目录Assets\Resources\下
    /// 2 挂在scene中名为ResourcesManager的gameObject上
    /// </summary>
    public class ResourcesManager : SingletonManager<ResourcesManager>
    {

        public enum eResManagerState
        {
            NoStart,
            Prepare,
            Ready,
        }

        /// <summary>
        /// 交叉引用的AB包释放规则类型
        /// </summary>
        public enum eCrossRefBundleReleaseType
        {
            NeverRelease,//加载的镜像在资源引用期间内不释放，只释放无交叉引用资源的图片！ 内存占用高，效率好
            ReleaseNoParentTextureAB,// 只有无引用图片会释放AB包
            //    ReloadWhenUse,//加载的镜像在资源加载后卸载，后面的资源需要引用再次加载，内存占用中，过程中会经常www加载镜像
            //    AlwayRelease,//加载的镜像在资源读取后立即释放，后面加载的资源会丢失引用，需要自己管理关联。内存小。效率待评估
        }

        /// <summary>
        /// SDK提供的外部资源文件路径，热更用
        /// </summary>
        public static string RESROOT;

        /// <summary>
        /// 调试模式
        /// </summary>
        public bool DebugMode = false;

        /// <summary>
        /// 是否通过assetbundle加载资源
        /// </summary>
        public static bool UsedAssetBundle = false;

        /// <summary>
        /// 交叉引用的AB包释放规则，目前只有无引用图片会释放AB包
        /// </summary>
        public eCrossRefBundleReleaseType CrossRefBundleReleaseType = eCrossRefBundleReleaseType.ReleaseNoParentTextureAB;


        /// <summary>
        /// 缓冲池
        /// </summary>
        public PoolManager PoolMg;


        /// <summary>
        /// 最大线程数
        /// </summary>
        public int MaxMultiThread = 20;

        /// <summary>
        /// 当前已缓存的资源数量
        /// </summary>
        public string ResourceUnitCount;

        /// <summary>
        /// 当前已缓存的资源内存大小
        /// </summary>
        public int LoadMemorySize = 0;

        /// <summary>
        /// 0引用的缓存区大小 单位k
        /// </summary>
        public int MaxZeroRefCacheSize = 20480;


        /// <summary>
        /// 运行时0引用的缓存区大小
        /// </summary>
        public int CurrentZeroRefCacheSize = 0;

        /// <summary>
        /// 处理中的请求数
        /// </summary>
        public int RequestCount = 0;


        /// <summary>
        /// IO进程数
        /// </summary>
        public int ProcessCount = 0;

        /// <summary>
        ///详细的已缓存资源信息，需要开启DebugMode
        /// </summary>
        public string[] AssetsDebug;

        /// <summary>
        /// 资源请求完成后的回调
        /// </summary>
        /// <param name="obj"></param>
        //  public delegate void YoukiaCore.Callback(object obj);

        public delegate void InitFinish();


        //脚本保持防止脚本序列化错误
        private ResourceRef scriptsKeeper;
        private eResManagerState mInit = eResManagerState.NoStart;


        private List<Request> processRequest = new List<Request>();
        private Queue<Request> mAllRequests = new Queue<Request>();
        private static readonly List<ResourceUnit> DestoryQueue = new List<ResourceUnit>();


        //加载的资源信息
        private readonly Dictionary<string, ResourceUnit> _mLoadedResourceUnit = new Dictionary<string, ResourceUnit>();

        private Dictionary<string, AssetInfo> _fileInfos;
        private readonly Dictionary<string, Process> _processPool = new Dictionary<string, Process>();


        //根据assetinfo分解Process
        internal object CreateProcess(string path)
        {
            AssetInfo info = GetInfo(path);
            if (info == null)
                return null;

            Process mainp;

            //已有资源
            if (_mLoadedResourceUnit.ContainsKey(path))
            {
                return _mLoadedResourceUnit[path];
            }
            //如果在处理中
            else if (_processPool.ContainsKey(path))
            {
                mainp = _processPool[path];
                mainp.Info = info;
            }
            else
            {
                //新建线程
                ResourceUnit unit = new ResourceUnit(path, info);
                mainp = new Process(unit, this);

                _processPool.Add(unit.mPath, mainp);
                mainp.Info = info;

                //处理依赖
                if (info != null && info.AllDependencies != null && info.AllDependencies.Count > 0)
                {
                    List<Process> childProcess = new List<Process>();
                    for (int i = 0; i < info.AllDependencies.Count; i++)
                    {
                        string each = info.AllDependencies[i];


                        object obj = CreateProcess(each);

                        if (obj is ResourceUnit)
                        {
                            mainp.unit.DependencesAsset.Add((obj as ResourceUnit).resourceRef);
                        }
                        if (obj is Process)
                        {
                            Process child = obj as Process;

                            //子线程不能有自己
                            if (child.Path == mainp.Path)
                            {
                                Log.Error("死循环依赖引用:"+ child.Path);
                                continue;
                            }
                           

                            //保持回调唯一
                            if (!child.FinishCallBack.Contains(mainp.childFinish))
                                child.FinishCallBack.Add(mainp.childFinish);

                            childProcess.Add(child);
                            mainp.unit.DependencesAsset.Add((child as Process).unit.resourceRef);
                        }



                    }


                    mainp.depProcess = childProcess.ToArray();
                }

            }

            return mainp;
        }

        /// <summary>
        /// 调试输出,需要debugModel=true
        /// </summary>
        public void Debug(bool logOut)
        {

            AssetsDebug = new string[_mLoadedResourceUnit.Count];
            int index = 0;
            foreach (KeyValuePair<string, ResourceUnit> each in _mLoadedResourceUnit)
            {
                if (DestoryQueue.Contains(each.Value))
                {
                    AssetsDebug[index] = "[删除队列]  " + " 引用计数:" + each.Value.ReferenceCount + " " + each.Key;

                }
                else if (each.Value.mAsset == null && each.Value.ReferenceCount == 0)
                {
                    AssetsDebug[index] = "[读取错误]  " + each.Key;
                }
                else if (each.Value.mAssetBundle != null && each.Value.ReferenceCount != 0)
                {
                    AssetsDebug[index] = "[正常引用]  " + " 引用计数:" + each.Value.ReferenceCount + " " + each.Key;

                }
                else if (each.Value.mAssetBundle == null && each.Value.ReferenceCount > 0)
                {
                    AssetsDebug[index] = "[包已卸载]  " + " 引用计数:" + each.Value.ReferenceCount + " " + each.Key;

                }
                else
                {
                    AssetsDebug[index] = "[未知状态]  " + " 引用计数:" + each.Value.ReferenceCount + " " + each.Key;
                }


                index++;
            }

            if (logOut)
            {
                for (int i = 0; i < AssetsDebug.Length; i++)
                {
                    UnityEngine.Debug.Log(AssetsDebug[i]);
                }

            }
        }


        /// <summary>
        /// MainFest内容转换为AssetInfo
        /// </summary>
        /// <param name="list">输入的源数据</param>
        public void ToAssetInfo(List<List<string>> list)
        {
            List<string> Path = list[0];
            List<string> RefCount = list[1];
            List<string> Guid = list[2];
            List<string> Dep = list[3];
            List<string> type = list[4];
            List<string> Size = list[5];

            Dictionary<string, AssetInfo> Infos = new Dictionary<string, AssetInfo>();
            for (int i = 0; i < Path.Count; i++)
            {
                AssetInfo info = new AssetInfo();
                info.Path = Path[i];
                info.Guid = Guid[i];
                info.refCount = int.Parse(RefCount[i]);
                info.ResType = type[i];
                info.Size = int.Parse(Size[i]);

                info.AllDependencies = new List<string>();

                if (string.IsNullOrEmpty(Dep[i]) == false)
                {
                    string[] sub = Dep[i].Split(',');
                    foreach (string each in sub)
                    {
                        info.AllDependencies.Add(each);
                    }

                }

                Infos.Add(info.PathWithOutSuffix, info);
            }

            _fileInfos = Infos;
        }

        void Enqueue(ResourceUnit unit)
        {
            if (!DestoryQueue.Contains(unit))
            {
                DestoryQueue.Add(unit);

                //只计算大资源,非打包模式下为资源数量
                if (unit.DependencesAsset == null || unit.DependencesAsset.Count == 0)
                {
                    CurrentZeroRefCacheSize += (int)unit.Size;
                }

            }

        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

        }

        /// <summary>
        /// 通过路径获得一个资源,如果资源是GameObject,返回GameObject上的resourceKeeper脚本.否则返回resourceRef(引用保持器);
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="handle">回调</param>
        public void LoadObject(string path, CallBack<object> handle)
        {

            LoadObject(path, typeof(Object), handle, false);

        }


        /// <summary>
        /// 通过路径获得一个资源,如果资源是GameObject,返回GameObject上的resourceKeeper脚本.否则返回resourceRef(引用保持器);
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="handle">回调</param>
        public void LoadObject(string path,Type t, CallBack<object> handle)
        {

            LoadObject(path, t, handle, false);

        }
        /// <summary>
        /// 通过路径获得一个资源,如果资源是GameObject,返回GameObject上的resourceKeeper脚本.否则返回resourceRef(引用保持器);
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="handle">回调</param>
        /// <param name="loadAsync">是否协程读取</param>
        public void LoadObject(string path, CallBack<object> handle, bool loadAsync)
        {

            LoadObject(path, typeof(Object), handle, loadAsync);

        }

        internal void LoadScene(string path, Type resType, CallBack<object> handle)
        {
            //todo 需要改版
            LoadObject(path, resType, handle, false);

        }

        internal void LoadObject(string path, Type resType, CallBack<object> handle, bool LoadAsync, bool isCache = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.Error("**ResourcesManager** 路径为空");
                return;
            }


            //   ResourceUnit unit = new ResourceUnit(path, resType);
            //生成请求
            Request req = new Request(path, resType, handle, LoadAsync);
            req.isCache = isCache;

            //已准备好的资源直接返回
            if (_mLoadedResourceUnit.ContainsKey(path))
            {
                ResourceUnit res = _mLoadedResourceUnit[req.Path];
                req.Call(_mLoadedResourceUnit[req.Path] as ResourceUnit);
            }
            else
            {
                //装入请求队列等待读取
                mAllRequests.Enqueue(req);

            }
        }

        public void CacheObject(string path, Type resType, CallBack<object> handle, bool LoadAsync = false)
        {
            LoadObject(path, resType, handle, LoadAsync, true);
        }




        /// <summary>
        /// 初始化资源管理器
        /// </summary>
        /// <param name="callback">成功后的回调</param>

        public void Init(InitFinish callback)
        {



            if (mInit > eResManagerState.NoStart)
                return;

            PoolMg = new PoolManager();
            //使用assetbundle打包功能
            if (UsedAssetBundle)
            {
                //读取预制样本
                LoadObject("AllScripts", (scripts) =>
                {
                    scriptsKeeper = (scripts as ResourceRef);

                    //读取依赖关系网
                    LoadObject("MainFest", (unit) =>
                    {
                        GameObject tmpMainfest = (unit as ResourceRefKeeper).gameObject;
//                        tmpMainfest.transform.parent = YKApplication.Instance.GetManager<ResourcesManager>().transform;

                        tmpMainfest.SendMessage("SendAssetInfos");
                        mInit = eResManagerState.Ready;

                        if (callback != null)
                            callback();
                    });



                });
                mInit = eResManagerState.Prepare;
            }
            else
            {
                mInit = eResManagerState.Ready;
                if (callback != null)
                    callback();
            }



        }

        void removeFinishRequest()
        {
            if (processRequest.Count == 0)
                return;


            List<Request> rqList = new List<Request>();
            for (int i = 0; i < processRequest.Count; i++)
            {
                if (processRequest[i].isDone)
                {
                    rqList.Add(processRequest[i]);
                }
            }


            if (rqList.Count == 0)
                return;


            for (int i = 0; i < rqList.Count; i++)
            {
                processRequest.Remove(rqList[i]);
            }

        }

        void removeFinishProcess()
        {

            if (_processPool.Count == 0)
                return;


            List<Process> rqList = new List<Process>();
            foreach (KeyValuePair<string, Process> each in _processPool)
            {
                if (each.Value.state == AsyncState.Done)
                {
                    rqList.Add(each.Value);
                }
            }

            if (rqList.Count == 0)
                return;


            for (int i = 0; i < rqList.Count; i++)
            {
                _processPool.Remove(rqList[i].Path);
            }

        }

        void Update()
        {
            if (mInit < eResManagerState.Prepare)
                return;

            if (DebugMode)
                Debug(false);


            ResourceRef currentLoadUnit = null;


            removeFinishRequest();
            removeFinishProcess();

            if (mAllRequests.Count > 0)
            {



                if (_processPool.Count < MaxMultiThread)
                {

                    for (int i = 0; i < MaxMultiThread - _processPool.Count; i++)
                    {
                        handleRequest();
                    }
                }

            }


            //移除资源部分代码

            int len = DestoryQueue.Count;
            if (len > 0)
            {
                for (int i = 0; i < len; i++)
                {
                    if (DestoryQueue == null || i >= DestoryQueue.Count)
                        break;

                    ResourceUnit Unit = DestoryQueue[i];


                    // 大资源特殊处理
                    if (UsedAssetBundle && (Unit.DependencesAsset == null || Unit.DependencesAsset.Count == 0))
                    {
                        if (CurrentZeroRefCacheSize >= MaxZeroRefCacheSize)
                        {

                            if (Unit.isCanDestory())
                            {
                                _mLoadedResourceUnit.Remove(Unit.mPath);
                                Unit.Destory();
                                DestoryQueue.RemoveAt(i);
                                i--;
                                LoadMemorySize -= Unit.Size;
                                CurrentZeroRefCacheSize -= (int)Unit.Size;
                            }

                        }
                    }
                    else
                    {

                        if (Unit.isCanDestory() || !UsedAssetBundle)
                        {
                            _mLoadedResourceUnit.Remove(Unit.mPath);
                            LoadMemorySize -= Unit.Size;
                            Unit.Destory();
                            DestoryQueue.RemoveAt(i);
                            i--;
                        }
                    }

                }
            }


            ResourceUnitCount = _mLoadedResourceUnit.Count.ToString();
            ProcessCount = _processPool.Count;
            RequestCount = processRequest.Count;
        }

        static bool isBaseRes(string path)
        {

            if (path == "MainFest" || path == "AllScripts" || path == "DllLinker")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        AssetInfo GetInfo(string path)
        {
            if (!UsedAssetBundle)
            {

                AssetInfo resInfo = new AssetInfo();
                resInfo.Path = path;
                resInfo.Guid = path;

                //todo 同文件夹下同名不同后缀有风险
//                resInfo.ResType = "GameObject";
                resInfo.ResType = "UnityEngine.Object";
                return resInfo;

            }

            if (isBaseRes(path))
            {
                AssetInfo baseInfo = new AssetInfo();
                baseInfo.Path = path;
                baseInfo.Guid = path;
                baseInfo.ResType = "GameObject";
                return baseInfo;
            }

            if (_fileInfos != null && _fileInfos.ContainsKey(path))
                return _fileInfos[path];
            else
            {
                return null;
            }

        }

        //处理请求
        private void handleRequest()
        {
            if (mAllRequests.Count == 0)
                return;

            Request req = mAllRequests.Dequeue();



            object res = CreateProcess(req.Path);
            if (res == null)
            {
                Log.Error("**ResourceManager: " + req.Path + "没有找到!");
            }
            else if (res is ResourceUnit)
            {
                //返回资源直接回调
                req.OnFinish(res as ResourceUnit);
            }
            else if (res is Process)
            {

                //返回进度
                Process mainProcess = res as Process;
                if (mainProcess != null)
                {
                    req.AddProcess(mainProcess);
                    processRequest.Add(req);
                }
            }

        }

        internal enum AsyncState
        {
            Nostart,
            Loading,
            LoadEnd,
            Reading,
            Done,
        }


        /// <summary>
        /// 引用技术器,每new一个ResourceRef，资源单位的引用计数+1
        /// </summary>
        public class ResourceRef : object
        {
            internal ResourceUnit resUnit;

            /// <summary>
            /// 资源类型
            /// </summary>
            public Type ResourceType
            {

                get { return resUnit.ResourceType; }
            }

            internal ResourceRef(ResourceUnit unit)
            {
                resUnit = unit;
                resUnit.addReferenceCount();
            }

            /// <summary>
            /// 资源
            /// </summary>
            public Object Asset
            {
                get
                {
                    return resUnit.mAsset;
                }
            }




            internal bool isDone
            {
                get
                {
                    return resUnit.isDone;
                }
            }

            ~ResourceRef()
            {
                resUnit.reduceReferenceCount();
            }


            public string Debug()
            {
                return resUnit.Debug();
            }

            public Sprite[] GetAllSprites()
            {
                if (resUnit.mAssetBundle == null && !ResourcesManager.UsedAssetBundle)
                {
                    //默认下载Sprite行为
                    Sprite[] sprites = Resources.LoadAll<Sprite>(resUnit.mPath);
                    if (sprites != null || sprites.Length == 0)
                    {
                        return sprites;
                    }
                    return null;
                }
                else
                {
                    List<Sprite> sl = new List<Sprite>();
                    for (int i = 0; i < resUnit.all.Length; i++)
                    {
                        if (resUnit.all[i].GetType() == typeof(Sprite))
                        {
                            sl.Add(resUnit.all[i] as Sprite);
                        }
                    }
                    if (sl.Count > 0)
                    {
                        return sl.ToArray();
                    }
                    else
                    {
                        return null;
                    }
                }
            }


            private static Dictionary<string, Sprite[]> tmpCache = new Dictionary<string, Sprite[]>();

            public Sprite GetSprite(string name)
            {
                if (resUnit.mAssetBundle == null && !ResourcesManager.UsedAssetBundle)
                {
                    Sprite[] sprites = null;
                    if (tmpCache.ContainsKey(resUnit.mPath))
                    {
                        sprites = tmpCache[resUnit.mPath];
                    }
                    else
                    {
                        //默认下载Sprite行为
                        sprites = Resources.LoadAll<Sprite>(resUnit.mPath);
                        tmpCache.Add(resUnit.mPath, sprites);
                    }

                    if (sprites != null || sprites.Length == 0)
                    {

                        foreach (Sprite sprite in sprites)
                        {
                            if (sprite.name == name)
                            {

                                return sprite;
                            }
                        }

                    }

                    return null;

                }
                else
                {
                    for (int i = 0; i < resUnit.all.Length; i++)
                    {
                        if (resUnit.all[i].name == name && resUnit.all[i].GetType() == typeof(Sprite))
                        {
                            return resUnit.all[i] as Sprite;
                        }

                    }

                    return null;
                }


            }


        }


        internal class ResourceUnit
        {
            internal string mPath;
            private List<ResourceRef> mDependencesAsset;//依赖的下级资源，也就是增加了下级的应用计数
            private int mReferenceCount;//上级引用计数
            private AsyncState _state;
            internal Type ResourceType;
            internal int Size;//大小

            internal float loadTime;
            internal float _startTime;
            //   internal float _LastNoRefTime;//最后一次无引用的时间

            internal Object[] all;
            //交叉引用数，打包时确定的
            private int CrossRefCount;

            internal Object mAsset;
            internal AssetBundle mAssetBundle;

            internal ResourceRef resourceRef
            {

                get
                {
                    return new ResourceRef(this);
                }

            }


            /// <summary>
            /// 尝试丢AssetBundle包，实验性功能
            /// </summary>
            internal void UnloadAssetBundle()
            {

                if (CanUnloadAssetBundle())
                {
                    if (mAssetBundle != null)
                    {
                        Log.Info(mPath + " 丢掉了AssetBundle包");
                        mAssetBundle.Unload(false);
                        mAssetBundle = null;
                    }
                }

            }


            private bool CanUnloadAssetBundle()
            {
                if (mPath == "AllScripts")
                    return false;
                //无引用数并且是图片
                if (CrossRefCount == 0 && (ResourceType == typeof(Texture2D) || ResourceType == typeof(Texture) || ResourceType == typeof(Cubemap)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }


            internal AsyncState State
            {
                get { return _state; }
                set
                {
                    _state = value;
                    if (_state == AsyncState.Done)
                    {
                        loadTime = Time.realtimeSinceStartup - _startTime;
                    }
                    else if (_state == AsyncState.Loading)
                    {
                        _startTime = Time.realtimeSinceStartup;
                    }

                }
            }



            internal void InputResource(AssetBundle Bundle, Object asset, int refCount)
            {
                mAsset = asset;
                mAssetBundle = Bundle;
                CrossRefCount = refCount;

                if (Bundle != null)
//                    all = Bundle.LoadAll();
                    all = Bundle.LoadAllAssets();
                State = AsyncState.Done;

            }


            internal ResourceUnit(string path, AssetInfo info)
            {
                mPath = path;
                Size = info.Size;
                mDependencesAsset = new List<ResourceRef>();
                _startTime = Time.realtimeSinceStartup;
                State = AsyncState.Nostart;
                State = AsyncState.Nostart;

                //缺失YKApplication
                //                if (YKApplication.Instance.UnityEngineAssembly != null)
                //                {
                //                    ResourceType = YKApplication.Instance.UnityEngineAssembly.GetType("UnityEngine." + info.ResType);
                //                }
                //                else
                //                {
                //                    Assembly ams = YKApplication.Instance.UnityEngineAssembly;
                //                    ResourceType = ams.GetType("UnityEngine." + info.ResType);
                //                }

                if (ResourceType == null)
                {
                    ResourceType = typeof(Object);
                }


            }

            internal bool isDone
            {

                get { return _state == AsyncState.Done; }

            }
            internal bool AllDependencesAssetDone()
            {

                for (int i = 0; i < mDependencesAsset.Count; i++)
                {
                    if (!mDependencesAsset[i].isDone)
                        return false;

                }

                return true;

            }

            internal List<ResourceRef> DependencesAsset
            {
                get
                {
                    return mDependencesAsset;
                }

                set
                {
                    foreach (ResourceRef asset in value)
                    {
                        mDependencesAsset.Add(asset);
                    }
                }
            }



            internal int ReferenceCount
            {
                get
                {
                    return mReferenceCount;
                }
            }


            internal string Debug()
            {

                string info = GetHashCode() + "," + mPath + ",类型:" + ResourceType.Name + ",引用计数:" + mReferenceCount + "|";
                foreach (ResourceRef ru in mDependencesAsset)
                {
                    info += ru.Debug();
                }
                return info;
            }

            internal void addReferenceCount()
            {
                ++mReferenceCount;

            }



            internal void reduceReferenceCount()
            {
                --mReferenceCount;


                if (mReferenceCount <= 0)
                {
                    //  _LastNoRefTime = Time.realtimeSinceStartup;
                    //   UnityEngine.Debug.Log(mPath + "最后一次无引用时间:" + _LastNoRefTime);
                    //缺失YKApplication
                    //                    YKApplication.Instance.GetManager<ResourcesManager>().Enqueue(this);

                }


            }

            internal bool isCanDestory() { return (mReferenceCount <= 0 && isDone); }


            /// <summary>
            /// 释放整个资源单位
            /// </summary>
            internal void Destory()
            {
                //缺失YKApplication
                //                if (YKApplication.Instance.GetManager<ResourcesManager>().DebugMode && mReferenceCount > 0)
                //                {
                //                    Log.Warning("**** 错误的移除资源 " + mPath);
                //                }


                if (!ResourcesManager.UsedAssetBundle) {
                    return;
                }


                Log.Warning("**** 移除资源 " + mPath);
                mDependencesAsset.Clear();


                if (mAssetBundle != null)
                {
                    //全丢
                    mAssetBundle.Unload(true);
                }
                else if (mAsset != null)
                {
                    //清Asset
                    Resources.UnloadAsset(mAsset);

                }


                mAssetBundle = null;



            }
        }

        /// <summary>
        /// 资源请求类
        /// </summary>
        internal class Request
        {
            //请求资源相对Assets/完整路径名称
            internal string Path;
            internal CallBack<object> mHandle;
            //是否用异步
            internal bool LoadAsync;
            internal bool isCache = false;
            internal Process process;
            //请求是否完成
            internal bool isDone;
            internal Request(string fileName, Type resourceType, CallBack<object> handle, bool Async)
            {
                LoadAsync = Async;
                Path = fileName;
                mHandle = handle;
            }

            internal void Call(ResourceUnit unit)
            {
                if (isCache)
                {

                    if (mHandle != null)
                        mHandle(unit.resourceRef);
                }
                else
                {
                    if (unit.mAsset == null)
                    {
                        if (mHandle != null)
                        {
                             Log.Error(unit.mPath + "资源没找到");
                             mHandle(null);
                        }


                    }
                    else if (unit.mAsset is GameObject)
                    {
                        GameObject go = Instantiate(unit.mAsset) as GameObject;
                        go.name = ResourceCommon.GetResourceName(Path);
                        ResourceRefKeeper keeper = go.AddComponent<ResourceRefKeeper>();
                        keeper.ResRef = unit.resourceRef;
                        keeper.RefID = unit.resourceRef.GetHashCode();
                        if (mHandle != null)
                            mHandle(keeper);
                    }
                    else
                    {
                        if (mHandle != null)
                            mHandle(unit.resourceRef);

                    }



                }


            }

            internal void OnFinish(ResourceUnit unit)
            {
                Call(unit);
                unit.UnloadAssetBundle();
                isDone = true;
            }

            internal void AddProcess(Process p)
            {
                process = p;
                process.FinishCallBack.Add(OnFinish);
                process.TryLoad();
            }
        }

        internal class Process
        {
            internal ResourcesManager mgr;
            internal string Path;
            internal Process[] depProcess;
            internal ResourceUnit unit;

            private int childCount
            {
                get
                {
                    if (depProcess == null)
                    {
                        return 0;
                    }

                    return depProcess.Length;


                }
            }

            private int loadedCount;
            /// <summary>
            /// 谨慎引用,非打包模式下为空
            /// </summary>
            internal AssetInfo Info;

            internal AsyncState state;
            internal bool LoadAsync;
            private List<CallBack<ResourceUnit>> _FinishCallBack;

            internal List<CallBack<ResourceUnit>> FinishCallBack
            {
                get
                {
                    if (_FinishCallBack == null)
                        _FinishCallBack = new List<CallBack<ResourceUnit>>();

                    return _FinishCallBack;
                }
                set { _FinishCallBack = value; }

            }


            internal void childFinish(ResourceUnit unitReady)
            {

                //加完成度
                loadedCount += 1;
                //  UnityEngine.Debug.Log("child:" + unitReady.mPath + " ok!!!,  count:" + loadedCount + ":" + childCount);
                TryLoad();
            }

            public Process(ResourceUnit unit, ResourcesManager manager)
            {
                Path = unit.mPath;
                this.unit = unit;
                loadedCount = 0;
                mgr = manager;
            }

            internal void TryLoad()
            {
                if (loadedCount >= childCount)
                {
                    if (state == AsyncState.Nostart)
                    {
                        state = AsyncState.Loading;
                        //缺失YKApplication
                        //                        YKApplication.Instance.GetManager<ResourcesManager>().StartCoroutine(_load(Path));
                    }
                }
                else
                {
                    if (depProcess == null)
                        return;

                    for (int i = 0; i < depProcess.Length; i++)
                    {
                        if (depProcess[i].state == AsyncState.Nostart)
                        {
                            depProcess[i].TryLoad();
                        }
                    }
                    // child tryLoad;
                }
            }


            void finish()
            {
                //  UnityEngine.Debug.Log(Path);
                state = AsyncState.Done;

                for (int i = 0; i < FinishCallBack.Count; i++)
                {
                    FinishCallBack[i](unit);
                }

                //clear
                FinishCallBack.Clear();
                loadedCount = 0;
                depProcess = null;

                mgr._mLoadedResourceUnit.Add(unit.mPath, unit);
                mgr.LoadMemorySize += unit.Size;
                unit = null;
                Info = null;


            }

            private IEnumerator _load(string path)
            {


                if (UsedAssetBundle == false)
                {
                    //非打包模式直读
                    UnityEngine.Object res = Resources.Load(path, this.unit.ResourceType);
                    this.unit.InputResource(null, res, 0);
                    this.unit.State = AsyncState.Done;
                    //非打包模式算数量
                    this.unit.Size = 1;
                    finish();
                    yield break;
                }

                string finalPath = ResourceCommon.GetURLPath("StreamingResources/" + Info.LoadPath, "", ResourceCommon.assetbundleFileSuffix);

                if (File.Exists(finalPath) == false)
                    finalPath = ResourceCommon.GetStreamingAssetsPath("StreamingResources/" + Info.LoadPath);
                else
                {
                    finalPath = "file://" + finalPath;
                }
                WWW www = null;

                //   UnityEngine.Debug.Log(finalPath);
                if (unit.ResourceType == typeof(Texture2D) || unit.ResourceType == typeof(AudioClip) || unit.ResourceType == typeof(AnimationClip))
                {
                    //   www = new WWW(finalPath);
                    //bug 贴图加载后显示不正确
                    www = WWW.LoadFromCacheOrDownload(finalPath, 0);
                }
                else
                {
                    www = new WWW(finalPath);
                }
                www.threadPriority = ThreadPriority.High;


                unit.State = AsyncState.Loading;
                yield return www;
                unit.State = AsyncState.LoadEnd;

                if (path == "AllScripts")
                {
                    //保持asset不空以回调
                    unit.InputResource(www.assetBundle, www.assetBundle, Info.refCount);
                    afterLoad(www);
                    yield break;
                }


                Object asset = null;

                if (!LoadAsync)
                {

                    string tmp = ResourceCommon.GetResourceName(path);
//                    asset = www.assetBundle.Load(tmp, unit.ResourceType);
                    asset = www.assetBundle.LoadAsset(tmp, unit.ResourceType);

                }
                else
                {
//                    AssetBundleRequest asyncOperation = www.assetBundle.LoadAsync(ResourceCommon.GetResourceName(path), unit.ResourceType);
                    AssetBundleRequest asyncOperation = www.assetBundle.LoadAssetAsync(ResourceCommon.GetResourceName(path), unit.ResourceType);
                    unit.State = AsyncState.Reading;
                    yield return asyncOperation;
                    asset = asyncOperation.asset;
                }



                if (!asset)
                {
                    Log.Error("**ResourcesManager**   " + path + "_loadAsync failed ");

                }

                if (asset.GetType() == typeof(Material) && Application.isEditor)
                {
                    Material mat = asset as Material;
                    mat.shader = Shader.Find(mat.shader.name);
                }



                unit.InputResource(www.assetBundle, asset, Info.refCount);
                afterLoad(www);

            }

            void afterLoad(WWW www)
            {
                www.Dispose();
                finish();
            }

        }
    }
}