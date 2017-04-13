using System;
using System.Collections.Generic;
using UnityEngine;

namespace AorFramework
{
    /// <summary>
    /// 框架主板
    /// </summary>
    public class AorFacede
    {

        private static AorFacede _instance;
        public static AorFacede Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AorFacede();
                }
                return _instance;
            }
        }

#region ========================== 管理器方法 ========================== 

        private Dictionary<Type, IManager> _managerDic = new Dictionary<Type, IManager>();


        /// <summary>
        /// 注册管理器
        /// </summary>
        public bool RegisterManager(IManager manager)
        {
            Type mt = manager.GetType();
            if (!_managerDic.ContainsKey(mt))
            {
                _managerDic.Add(mt, manager);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除管理器
        /// </summary>
        public bool UnregisterManager<T>()
        {
            Type mt = typeof(T);
            if (_managerDic.ContainsKey(mt))
            {
                _managerDic[mt].Dispose();
                return _managerDic.Remove(mt);
            }
            return false;
        }
        public bool UnregisterManager(IManager manager)
        {
            Type mt = manager.GetType();
            if (_managerDic.ContainsKey(mt))
            {
                manager.Dispose();
                return _managerDic.Remove(mt);
            }
            return false;
        }

        /// <summary>
        /// 获取管理器
        /// </summary>
        public T GetManager<T>() where T : IManager
        {
            Type mt = typeof (T);
            if (_managerDic.ContainsKey(mt))
            {
                T t = (T) _managerDic[mt];
                return t;
            }
            return default(T);
        }

        #endregion

#region ========================== AssetLoad 资源下载方法和注入接口 ========================== 

        /// <summary>
        /// 加载资源--注入方法
        /// </summary>
        public Action<string, Type, Action<UnityEngine.Object,object[]>> LoadAssetCustomFunc;
        /// <summary>
        /// 加载资源 （Textrue2D, Material, TextAsset, Audio, etc ...）
        /// <param name="path">(Resources)路径</param>
        /// <param name="objType">资源类型</param>
        /// <param name="loadFinishCallback">加载完成回调</param>
        /// </summary>
        public void LoadAsset(string path, Type objType, Action<UnityEngine.Object,object[]> loadFinishCallback)
        {
            
            if (LoadAssetCustomFunc != null)
            {
                LoadAssetCustomFunc(path, objType, loadFinishCallback);
                return;
            }

            //Default :
            UnityEngine.Object asset = Resources.Load(path, objType);
            if (asset)
            {
                loadFinishCallback(asset, null);
            }
        }

        /// <summary>
        /// 加载预制体--注入方法
        /// </summary>
        public Action<string, Action<GameObject,object[]>> LoadPrefabCustomFunc;
        /// <summary>
        /// 加载预制体
        /// </summary>
        /// <param name="path">(Resources)路径</param>
        /// <param name="loadFinishCallback">加载完成回调</param>
        public void LoadPrefab(string path, Action<GameObject,object[]> loadFinishCallback)
        {
            if (LoadPrefabCustomFunc != null)
            {
                LoadPrefabCustomFunc(path, loadFinishCallback);
                return;
            }

            //Default :
            GameObject assetGameObject = Resources.Load<GameObject>(path);
            if (assetGameObject)
            {
                loadFinishCallback(assetGameObject, null);
            }
        }

        /// <summary>
        /// 加载Sprite集合--注入方法
        /// </summary>
        public Action<string, Action<Sprite[], object[]>> LoadAllSpritesCustomFunc;
        /// <summary>
        /// 加载Sprite集合
        /// </summary>
        /// <param name="path">(Resources)路径</param>
        /// <param name="loadFinishCallback">加载完成回调</param>
        public void LoadAllSprites(string path, Action<Sprite[], object[]> loadFinishCallback)
        {
            if (LoadAllSpritesCustomFunc != null)
            {
                LoadAllSpritesCustomFunc(path, loadFinishCallback);
                return;
            }

            //Default:
            Sprite[] allSprites = Resources.LoadAll<Sprite>(path);
            loadFinishCallback(allSprites, null);
        }

        /// <summary>
        /// 加载Sprite--注入方法
        /// </summary>
        public Action<string, string, Action<Sprite, object[]>> LoadSpriteCustomFunc;
        /// <summary>
        /// 加载Sprite
        /// </summary>
        /// <param name="path">(Resources)路径</param>
        /// <param name="spriteName">(Resources)sprite名称</param>
        /// <param name="loadFinishCallback">加载完成回调</param>
        public void LoadSprite(string path, string spriteName, Action<Sprite, object[]> loadFinishCallback)
        {
            if (LoadSpriteCustomFunc != null)
            {
                LoadSpriteCustomFunc(path, spriteName, loadFinishCallback);
                return;
            }

            //Default:
            LoadAllSprites(path, (allSprites, param) =>
            {
                if (allSprites != null && allSprites.Length > 0)
                {

                    int i, len = allSprites.Length;
                    for (i = 0; i < len; i++)
                    {

                        if (allSprites[i].name == spriteName)
                        {
                            Sprite s = allSprites[i];
                            loadFinishCallback(s, null);
                            return;
                        }

                    }

                }

                loadFinishCallback(null, null);
            });

        }

        /// <summary>
        /// 加载Sprite
        /// </summary>
        /// <param name="pathAtSpName">(Resources)路径@sprite名称</param>
        /// <param name="loadFinishCallback">加载完成回调</param>
        public void LoadSprite(string pathAtSpName, Action<Sprite, object[]> loadFinishCallback)
        {
            if (pathAtSpName.Contains("@"))
            {
                string[] psn = pathAtSpName.Split('@');
                LoadSprite(psn[0],psn[1], loadFinishCallback);
            }
            else
            {
                LoadAllSprites(pathAtSpName, (sprites, param) =>
                {
                    if (sprites != null && sprites.Length > 0)
                    {
                        loadFinishCallback(sprites[0], null);
                        return;
                    }
                    loadFinishCallback(null, null);
                });
            }
        }

#endregion



    }


}
