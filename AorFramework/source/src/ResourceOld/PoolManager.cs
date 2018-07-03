using Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using YoukiaCore;
using Object = UnityEngine.Object;

namespace YoukiaUnity.Resource
{

    /// <summary>
    /// 缓存池管理器
    /// </summary>
    public class PoolManager
    {
        /// <summary>
        /// 池列表
        /// </summary>
        Dictionary<string, GameObject> Pools = new Dictionary<string, GameObject>();

        /// <summary>
        /// 池内对应的节点列表
        /// </summary>
        Dictionary<string, Dictionary<string, List<ResourceRefKeeper>>> Nodes = new Dictionary<string, Dictionary<string, List<ResourceRefKeeper>>>();
        //缓冲上限
        private int NodeLimit = 10;

        private const string DEFAULTPOOL = "defaultPool";


        /// <summary>
        /// 建立池
        /// </summary>
        /// <param name="poolName">池名字</param>
        public void CreatePool(string poolName)
        {
            GameObject pool = new GameObject(poolName);
//            pool.transform.parent = YKApplication.Instance.GetManager<ResourcesManager>().transform;
            pool.transform.localPosition = new Vector3(0, 99999, 0);
            pool.transform.localEulerAngles = Vector3.zero;
            pool.transform.localScale = Vector3.one;
            Pools.Add(poolName, pool);
            Nodes.Add(poolName, new Dictionary<string, List<ResourceRefKeeper>>());

        }

        /// <summary>
        /// 销毁池
        /// </summary>
        public void DestroyPool(string poolName)
        {

            GameObject.Destroy(Pools[poolName]);
            Pools.Remove(poolName);
            Nodes.Remove(poolName);
        }


        /// <summary>
        /// 清空池
        /// </summary>
        public void CleanPool(string poolName = DEFAULTPOOL)
        {
            if(Pools.Count==0)
                return;

            List<GameObject> objArray = new List<GameObject>();
            foreach (Transform each in Pools[poolName].transform)
            {
                objArray.Add(each.gameObject);
            }


            for (int i = 0; i < objArray.Count; i++)
            {

                GameObject.DestroyImmediate(objArray[i]);

            }

            Nodes[poolName] = new Dictionary<string, List<ResourceRefKeeper>>();


        }

        /// <summary>
        /// 从池中获得物体
        /// </summary>
        ///  <param name="path">读取路径</param>
        /// <param name="poolName">目标池</param>
        /// <param name="handle">回调</param>
        public void LoadObjectFromPool(string path, CallBack<object> handle, string poolName = DEFAULTPOOL)
        {
            ResourceRefKeeper tmp = null;

            if (Nodes.ContainsKey(poolName) == false)
            {


                if (poolName == DEFAULTPOOL)
                {
                    CreatePool(DEFAULTPOOL);
                }
                else
                {
                    Log.Error("没有对应的缓冲池");
                    return;
                }



            }



            if (Nodes[poolName].ContainsKey(path))
            {
                if (Nodes[poolName][path].Count > 0)
                {

                    tmp = Nodes[poolName][path][0];
                    Nodes[poolName][path].Remove(tmp);

                    if (Nodes[poolName][path].Count <= 0)
                        Nodes[poolName].Remove(path);

                }
            }


            if (tmp == null)
            {

//                YKApplication.Instance.GetManager<ResourcesManager>().LoadObject(path, (obj) =>
//                {
//                    handle(obj);
//                });
            }
            else
            {
                tmp.transform.SetParent(null, false);
                handle(tmp);
            }
        }

        /// <summary>
        /// 缓冲物体
        /// </summary>
        /// <param name="path">缓冲物体的路径</param>
        /// <param name="handle">回调</param>
        /// <param name="poolName">目标池</param>
        public void CacheObjToPool(string path, CallBack<Object> handle, string poolName = DEFAULTPOOL)
        {
//
//            YKApplication.Instance.GetManager<ResourcesManager>().LoadObject(path, (obj) =>
//            {
//                ResourceRefKeeper keeper = obj as ResourceRefKeeper;
//                PutInPool(keeper, poolName);
//                if (handle != null)
//                    handle(keeper);
//            });


        }

        /// <summary>
        /// 是否存在对应缓冲池
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        private bool hasPool(string poolName)
        {
            if (Nodes.ContainsKey(poolName) == false)
            {
                if (poolName == DEFAULTPOOL)
                {
                    CreatePool(DEFAULTPOOL);
                    return true;
                }
                else
                {
                    Log.Error("没有对应的缓冲池");
                    return false;
                }

            }

            return true;
        }

        /// <summary>
        /// 已实例化的物体回收到池中
        /// </summary>
        /// <param name="keeper">该物体的资源保持器,标脏物体无法入池</param>
        /// <param name="poolName">目标池,默认为DEFAULTPOOL，不能为空</param>
        public void PutInPool(ResourceRefKeeper keeper, string poolName = DEFAULTPOOL)
        {
            if (!hasPool(poolName) || keeper == null || keeper.isDirty)
                return;

//            IMonoSwitch[] ims = keeper.gameObject.GetInterfaces<IMonoSwitch>();
//            if (ims != null) {
//                foreach (IMonoSwitch im in ims) {
//                    Type t = im.GetType();
//                    im.RemoveCall(t.FullName);
//               //     t.GetMethod("RemoveCall", BindingFlags.Instance | BindingFlags.Public).Invoke(im, null);
//                }
//            }

            keeper.transform.SetParent(Pools[poolName].transform, false);

            keeper.transform.localPosition = Vector3.zero;
            keeper.gameObject.SetActive(false);

            string path = keeper.ResRef.resUnit.mPath;

            //无同路径字典，新建一个
            if (!Nodes[poolName].ContainsKey(path))
            {
                Nodes[poolName].Add(path, new List<ResourceRefKeeper>());
            }

            Nodes[poolName][path].Add(keeper);

        }


        /// <summary>
        /// 从池中删除
        /// </summary>
        public void DestoryObjectFromPool(ResourceRefKeeper keeper, string poolName = DEFAULTPOOL)
        {
            if (!hasPool(poolName) || keeper == null)
                return;

            string path = keeper.ResRef.resUnit.mPath;

            //移除
            if (Nodes[poolName].ContainsKey(path))
            {
                Nodes[poolName][path].Remove(keeper);

                if (Nodes[poolName][path].Count == 0)
                {

                    Nodes[poolName].Remove(path);
                }
            }


        }



    }


}
