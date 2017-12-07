using System;
using System.Collections.Generic;
using UnityEngine;

namespace AorFrameworks
{
    /// <summary>
    /// 提供载入资源的桥接功能
    /// </summary>
    public class AorResourceLoader
    {
        /// <summary>
        /// 加载 Sprite 注入方法
        /// </summary>
        public static Action<string, Action<Sprite>> CustomLoadSprite;
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

        /// <summary>
        /// 加载Texture资源 注入方法
        /// </summary>
        public static Action<string, Action<Texture>> CustomLoadTexture;
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
            Texture asset = Resources.Load<Texture>(path);
            if (asset)
            {
                finishCallback(asset);
            }
            finishCallback(null);

        }

        /// <summary>
        /// LoadPrefabAsset 载入预制体资源(非实例化) 注入方法
        /// </summary>
        public static Action<string, Action<GameObject>> CustomLoadPrefabAsset;
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
            GameObject asset = Resources.Load<GameObject>(path);
            if (asset)
            {
                finishCallback(asset);
                return;
            }

            finishCallback(null);
        }
        /// <summary>
        /// LoadPrefab 载入预制体(实例化) 注入方法
        /// </summary>
        public static Action<string, Action<GameObject>> CustomLoadPrefab;
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
        /// <summary>
        /// 卸载 预制体(实例化) 注入方法
        /// </summary>
        public static Action<GameObject> CustomUnLoadPrefab;
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
            if (!Application.isPlaying && Application.isEditor)
            {
                GameObject.DestroyImmediate(prefab);
            }
            else
            {
                GameObject.Destroy(prefab);
            }
        }

    }
}
