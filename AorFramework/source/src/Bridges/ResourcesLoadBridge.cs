using System;
using System.Collections.Generic;
using UnityEngine;

namespace AorFrameworks
{
    /// <summary>
    /// 提供载入资源的桥接功能
    /// </summary>
    public class ResourcesLoadBridge
    {

        #region LoadSprite

        /// <summary>
        /// 加载 Sprite 注入方法
        /// </summary>
        public static Action<string, Action<Sprite>> CustomLoadSprite;

        public static Action<string, Action<Sprite>, object[]> CustomLoadSpriteWithParams;

        /// <summary>
        /// 加载 Sprite
        /// 以 Resource/ 为起始目录
        /// </summary>
        public static void LoadSprite(string path, Action<Sprite> finishCallback)
        {
            if (CustomLoadSprite != null)
            {
                CustomLoadSprite(path, finishCallback);
                return;
            }

            //Default
            _defaultLoadSprite(path, finishCallback);
        }

        public static void LoadSprite(string path, Action<Sprite> finishCallback, params object[] param)
        {

            if (CustomLoadSpriteWithParams != null)
            {
                CustomLoadSpriteWithParams(path, finishCallback, param);
                return;
            }

            //Default
            _defaultLoadSprite(path, finishCallback);
        }

        private static void _defaultLoadSprite(string path, Action<Sprite> finishCallback)
        {
            //Default
            bool useSt = path.Contains("@");
            string absPath;
            string spName = string.Empty;
            if (useSt)
            {
                string[] sp = path.Split('@');
                absPath = sp[0];
                spName = sp[1];
            }
            else
            {
                absPath = path;
            }

            Sprite[] assets = Resources.LoadAll<Sprite>(absPath);
            if (assets != null && assets.Length > 0)
            {
                Sprite tar;
                if (useSt)
                {
                    for (int i = 0; i < assets.Length; i++)
                    {
                        if (spName.Equals(assets[i].name))
                        {
                            tar = assets[0];
                            finishCallback(tar);
                            return;
                        }
                    }
                }
                else
                {
                    tar = assets[0];
                    finishCallback(tar);
                    return;
                }
            }
            finishCallback(null);
        }

        #endregion

        #region LoadTextrue

        /// <summary>
        /// 加载Texture资源 注入方法
        /// </summary>
        public static Action<string, Action<Texture>> CustomLoadTexture;

        public static Action<string, Action<Texture>, object[]> CustomLoadTextureWithParams;

        /// <summary>
        /// 加载Texture资源
        /// 以 Resource/ 为起始目录
        /// </summary>
        public static void LoadTextrue(string path, Action<Texture> finishCallback)
        {
            if (CustomLoadTexture != null)
            {
                CustomLoadTexture(path, finishCallback);
                return;
            }

            //Default
            _defaultLoadTextrue(path, finishCallback);
        }

        public static void LoadTextrue(string path, Action<Texture> finishCallback, params object[] param)
        {
            if (CustomLoadTextureWithParams != null)
            {
                CustomLoadTextureWithParams(path, finishCallback, param);
                return;
            }

            //Default
            _defaultLoadTextrue(path, finishCallback);
        }

        public static void _defaultLoadTextrue(string path, Action<Texture> finishCallback)
        {
            //Default
            Texture asset = Resources.Load<Texture>(path);
            if (asset)
            {
                finishCallback(asset);
                return;
            }

            finishCallback(null);
        }

        #endregion

        #region Load By Type

        /// <summary>
        /// Load 加载UnityEngine.Object(非实例化) 注入方法
        /// </summary>
        public static Action<string, Type, Action<Type,UnityEngine.Object>> CustomLoad;

        public static Action<string, Type, Action<Type,UnityEngine.Object>, object[]> CustomLoadWithParams;

        /// <summary>
        /// Load 加载UnityEngine.Object(非实例化)
        /// 以 Resource/ 为起始目录
        /// </summary>
        public static void Load(string path, Type objectType, Action<Type, UnityEngine.Object> finishCallback)
        {

            if (CustomLoad != null)
            {
                CustomLoad(path, objectType, finishCallback);
                return;
            }

            //Default
            _defaultLoad(path, objectType, finishCallback);
        }

        public static void LoadPrefab(string path, Type objectType, Action<Type, UnityEngine.Object> finishCallback, params object[] param)
        {

            if (CustomLoadWithParams != null)
            {
                CustomLoadWithParams(path, objectType, finishCallback, param);
                return;
            }

            //Default
            _defaultLoad(path, objectType, finishCallback);
        }

        public static void _defaultLoad(string path, Type objectType, Action<Type, UnityEngine.Object> finishCallback)
        {
            //Default
            UnityEngine.Object obj = Resources.Load(path, objectType);
            if (finishCallback != null)
            {
                finishCallback(objectType, obj);
            }
        }

        #endregion

        #region LoadPrefabAsset

        /// <summary>
        /// LoadPrefabAsset 载入预制体资源(非实例化) 注入方法
        /// </summary>
        public static Action<string, Action<GameObject>> CustomLoadPrefabAsset;

        public static Action<string, Action<GameObject>, object[]> CustomLoadPrefabAssetWithParams;

        /// <summary>
        ///  LoadPrefabAsset 载入预制体资源(非实例化)
        /// 以 Resource/ 为起始目录
        /// </summary>
        public static void LoadPrefabAsset(string path, Action<GameObject> finishCallback)
        {
            if (CustomLoadPrefabAsset != null)
            {
                CustomLoadPrefabAsset(path, finishCallback);
                return;
            }

            //Default
            _defaultLoadPrefabAsset(path, finishCallback);
        }

        public static void LoadPrefabAsset(string path, Action<GameObject> finishCallback, params object[] param)
        {
            if (CustomLoadPrefabAssetWithParams != null)
            {
                CustomLoadPrefabAssetWithParams(path, finishCallback, param);
                return;
            }

            //Default
            _defaultLoadPrefabAsset(path, finishCallback);
        }

        private static void _defaultLoadPrefabAsset(string path, Action<GameObject> finishCallback)
        {
            //Default
            GameObject asset = Resources.Load<GameObject>(path);
            if (asset)
            {
                finishCallback(asset);
                return;
            }
            finishCallback(null);
        }

        #endregion

        #region LoadScriptableObject

        /// <summary>
        /// LoadPrefabAsset 载入预制体资源(非实例化) 注入方法
        /// </summary>
        public static Action<string, Action<ScriptableObject>> CustomLoadScriptableObject;

        public static Action<string, Action<ScriptableObject>, object[]> CustomLoadScriptableObjectWithParams;

        /// <summary>
        ///  LoadPrefabAsset 载入预制体资源(非实例化)
        /// 以 Resource/ 为起始目录
        /// </summary>
        public static void LoadScriptableObject(string path, Action<ScriptableObject> finishCallback)
        {
            if (CustomLoadPrefabAsset != null)
            {
                CustomLoadScriptableObject(path, finishCallback);
                return;
            }

            //Default
            _defaultLoadScriptableObject(path, finishCallback);
        }

        public static void LoadScriptableObject(string path, Action<ScriptableObject> finishCallback, params object[] param)
        {
            if (CustomLoadScriptableObjectWithParams != null)
            {
                CustomLoadScriptableObjectWithParams(path, finishCallback, param);
                return;
            }

            //Default
            _defaultLoadScriptableObject(path, finishCallback);
        }

        public static void _defaultLoadScriptableObject(string path, Action<ScriptableObject> finishCallback)
        {
            //Default
            ScriptableObject data = Resources.Load<ScriptableObject>(path);
            if (data)
            {
                finishCallback(data);
                return;
            }

            finishCallback(null);
        }

        #endregion

        #region LoadPrefab

        /// <summary>
        /// LoadPrefab 载入预制体(实例化) 注入方法
        /// </summary>
        public static Action<string, Action<GameObject>> CustomLoadPrefab;

        public static Action<string, Action<GameObject>, object[]> CustomLoadPrefabWithParams;

        /// <summary>
        /// LoadPrefab 载入预制体(实例化)
        /// 以 Resource/ 为起始目录
        /// </summary>
        public static void LoadPrefab(string path, Action<GameObject> finishCallback)
        {

            if (CustomLoadPrefab != null)
            {
                CustomLoadPrefab(path, finishCallback);
                return;
            }

            //Default
            _defaultLoadPrefab(path, finishCallback);
        }

        public static void LoadPrefab(string path, Action<GameObject> finishCallback, params object[] param)
        {

            if (CustomLoadPrefabWithParams != null)
            {
                CustomLoadPrefabWithParams(path, finishCallback, param);
                return;
            }

            //Default
            _defaultLoadPrefab(path, finishCallback);
        }

        public static void _defaultLoadPrefab(string path, Action<GameObject> finishCallback)
        {
            //Default
            LoadPrefabAsset(path, (asset) =>
            {
                if (asset)
                {
                    GameObject ins = GameObject.Instantiate(asset);
                    ins.name = asset.name;
                    finishCallback(ins);
                    return;
                }

                finishCallback(null);
            });

        }

        #endregion

        #region UnLoadPrefab

        /// <summary>
        /// 卸载 预制体(实例化) 注入方法
        /// </summary>
        public static Action<GameObject> CustomUnLoadPrefab;

        public static Action<GameObject, object[]> CustomUnLoadPrefabWithParams;

        /// <summary>
        /// 卸载 预制体(实例化)
        /// </summary>
        public static void UnLoadPrefab(GameObject prefab)
        {

            if (CustomUnLoadPrefab != null)
            {
                CustomUnLoadPrefab(prefab);
                return;
            }

            //Default
            _defaultUnLoadPrefab(prefab);
        }

        public static void UnLoadPrefab(GameObject prefab, params object[] param)
        {

            if (CustomUnLoadPrefabWithParams != null)
            {
                CustomUnLoadPrefabWithParams(prefab, param);
                return;
            }

            //Default
            _defaultUnLoadPrefab(prefab);
        }

        public static void _defaultUnLoadPrefab(GameObject prefab)
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
        
    }
}
