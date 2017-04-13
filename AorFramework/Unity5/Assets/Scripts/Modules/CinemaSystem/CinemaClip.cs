using System;
using System.Collections.Generic;
using System.Reflection;
using AorFramework.module;
using UnityEngine;
using YoukiaUnity;
using YoukiaCore;
//using YoukiaUnity.App;
using YoukiaUnity.Graphics;
//using YoukiaUnity.Resource;

namespace YoukiaUnity.CinemaSystem
{

    /// <summary>
    /// 电影剪辑类
    /// </summary>
//    [RequireComponent(typeof(AnimLinkage))]
    public class CinemaClip : GraphicsLauncher
    {

        /// <summary>
        /// CinemaClip专用加载预制体注入方法
        /// </summary>
        public static System.Action<string, Action<GameObject>> LoadPrefabCustom;

        /// <summary>
        /// CinemaClip 加载接口
        /// </summary>
        /// <param name="path">CinemaClip加载path</param>
        /// <param name="loadedCallback">>加载成功后需要执行的回调(传递实例后的加载对象)</param>
        public static void LoadPrefab(string path, Action<GameObject> loadedCallback)
        {

            GameObject go;
            if (LoadPrefabCustom != null && loadedCallback != null)
            {
                LoadPrefabCustom(path, loadedCallback);
                return;
            }

            //default:
            GameObject asset = Resources.Load<GameObject>(path);
            if (!asset) return;
            go = Instantiate(asset);
            go.name = asset.name;
            if (loadedCallback != null)
            {
                loadedCallback(go);
            }
        }

        /// <summary>
        /// CinemaClip专用卸载预制体注入方法
        /// </summary>
        public static Action<GameObject> UnloadPrefabCustom;

        /// <summary>
        /// CinemaClip 卸载接口
        /// </summary>
        public static void UnloadPrefab(GameObject prefab)
        {
            if (UnloadPrefabCustom != null)
            {
                UnloadPrefabCustom(prefab);
                return;
            }

            //default:
            if (Application.isPlaying)
            {
                GameObject.Destroy(prefab);
            }
            else
            {
                GameObject.DestroyImmediate(prefab);
            }

        }

        /// <summary>
        /// CinemaClip专用加载预制体注入方法
        /// </summary>
        public static Action<string> PlayAudioCustom;

        /// <summary>
        /// CinemaClip专用播放声音接口
        /// </summary>
        /// <param name="path"></param>
        public static void PlayAudio(string path)
        {
            if (PlayAudioCustom != null)
            {
                PlayAudioCustom(path);
                return;
            }

            //default:
            
            //... ...

        }

        /// <summary>
        /// CinemaClip专用加载预制体注入方法
        /// </summary>
        public static Action<string> PlayBGMCustom;

        public static void PlayBGM(string path)
        {

            if (PlayBGMCustom != null)
            {
                PlayBGMCustom(path);
            }

            //default:

            //... ...

        }

        public string BridgeAutoLoadKey;

        public ICinemaBridge Bridge;

        /// <summary>
        /// 标识此CinemaClip是否保持在世界坐标原点上。
        /// </summary>
        public bool KeepRootPosToOrigin = true;

        /// <summary>
        /// 是否此Clip为 多动画线CinemaClip
        /// </summary>
        public bool IsMutiLineClip = false;

        /// <summary>
        /// 本次影片的角色;
        /// </summary>
        public List<CinemaCharacter> ActorList = new List<CinemaCharacter>();

        /// <summary>
        /// 本次剪辑的摄影机列表
        /// </summary>
        public List<GameObject> CameraList = new List<GameObject>();

        /// <summary>
        /// 所有动画
        /// </summary>
        public List<Behaviour> AnimList = new List<Behaviour>();

//        private AnimLinkage _AnimLinkage;
//        public AnimLinkage AnimLinkage
//        {
//            get
//            {
//                if (_AnimLinkage == null)
//                {
//                    _AnimLinkage = GetComponent<AnimLinkage>();
//                }
//                return _AnimLinkage;
//            }
//        }

        public Animator CinemaClipAnimator
        {
            get
            {

                if (_CinemaClipAnimator == null)
                    _CinemaClipAnimator = GetComponent<Animator>();

                return _CinemaClipAnimator;

            }
        }
        private Animator _CinemaClipAnimator;

        /// <summary>
        /// 自动开始播放剪辑
        /// </summary>
        public bool AutoPlay = true;

        private bool isPlaying;


        /// <summary>
        /// 自动找到子节点内的角色(编辑器用)
        /// </summary>
        public void LinkActor(Action finish)
        {

            ActorList.Clear();
            linkAllActor(transform, finish);

        }

        void getBridge()
        {
            MonoBehaviour[] list = gameObject.GetComponents<MonoBehaviour>();
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] is ICinemaBridge)
                {
                    Bridge = list[i] as ICinemaBridge;
                    return;
                }
            }

            if (!string.IsNullOrEmpty(BridgeAutoLoadKey))
            {
                Assembly assembly = Assembly.GetAssembly(typeof(CinemaClip));
                Type bType = assembly.GetType(BridgeAutoLoadKey);
                Component bridge = gameObject.GetComponent(bType);
                if (bridge == null)
                {
                    bridge = gameObject.AddComponent(bType);
                    if (bridge == null)
                    {
                        Bridge = gameObject.AddComponent<BaseCinemaBridge>();
                        return;
                    }
                }

                if (bridge is ICinemaBridge)
                {
                    Bridge = (ICinemaBridge)bridge;
                    return;
                }

            }

            Bridge = gameObject.AddComponent<BaseCinemaBridge>();
        }


        private void Start()
        {

            StartAfterGraphicMgr(this, () =>
             {
                 getBridge();
                 LinkActor(() =>
                 {

                     LinkCamera();

                     if (Bridge != null)
                     {
                         Bridge.OnClipStart();
                     }

                     if (AutoPlay)
                     {
                         Play();
                     }
                    
                 });
                
             });
            
        }

        private GraphicsManager _graphicsManager;



        protected override void Launcher()
        {
            base.Launcher();
            _graphicsManager = GraphicsManager.GetInstance();
            LinkAnim();
            setAnimsEnable(false);

        }

        void setAnimsEnable(bool enable)
        {
            for (int i = 0; i < AnimList.Count; i++)
            {
                if (AnimList[i] != null)
                {
                    AnimList[i].enabled = enable;

                }
            }
        }

        void linkAllAnim(Transform tran)
        {
//            Animator[] ats = tran.GetComponentsInChildren<Animator>();
            Animator ta = tran.GetComponent<Animator>();
            if (ta) AnimList.Add(ta);

            List<Animator> ats = tran.FindComponentListInChildren<Animator>();
            for (int i = 0; i < ats.Count; i++)
            {
                Animator animator = ats[i];
                AnimList.Add(animator);
            }

			Animation tb = tran.GetComponent<Animation>();
            if (tb) AnimList.Add(tb);

            List<Animation> ans = tran.FindComponentListInChildren<Animation>();
            for (int i = 0; i < ans.Count; i++)
            {
                AnimList.Add(ans[i]);
            }

        }

        void loadActorLoop(CinemaCharacter[] characters, int index, Action finish)
        {

           // if (Bridge == null) return;

            Bridge.InitActor(characters[index], () =>
            {

                index += 1;
                if (index >= characters.Length)
                {
                    if (finish != null)
                        finish();
                }
                else
                {

                    loadActorLoop(characters, index, finish);

                }


            });

        }

        void linkAllActor(Transform tran, Action finish)
        {
            //            CinemaCharacter[] characters = tran.GetComponentsInChildren<CinemaCharacter>();

            List<CinemaCharacter> characters = tran.FindComponentListInChildren<CinemaCharacter>();

            for (int i = 0; i < characters.Count; i++)
            {
                ActorList.Add(characters[i]);
            }


            if (Application.isPlaying && characters.Count > 0)
            {

                loadActorLoop(characters.ToArray(), 0, finish);

            }

        }

        /// <summary>
        /// 自动找到子节点内的摄像机(编辑器用)
        /// </summary>
        public void LinkCamera()
        {

            CameraList.Clear();
            linkAllCamera(transform);


        }
        /// <summary>
        /// 自动找到子节点内的动画(编辑器用)
        /// </summary>
        public void LinkAnim()
        {

            AnimList.Clear();
            linkAllAnim(transform);

        }


        void linkAllCamera(Transform tran)
        {

            foreach (Transform each in tran)
            {
                if (each.GetComponent<SubCameraInfo>() != null)
                {
                    CameraList.Add(each.gameObject);

                }

                if (each.childCount > 0)
                    linkAllCamera(each);


            }

        }
 
        public CinemaCharacter GetActorByName(string _name)
        {

            for (int i = 0; i < ActorList.Count; i++)
            {
                if (ActorList[i].name == _name)
                {
                    return ActorList[i];
                }

            }

            return null;
        }


        private string CurrentClipName;

        /// <summary>
        /// 马上播放该剪辑
        /// </summary>
        public void Play(string clipName = "")
//        public void Play()
        {
//
            if (!string.IsNullOrEmpty(clipName))
                CurrentClipName = clipName;

            if (Bridge == null)
            {
                return;
            }

            Bridge.OnClipPlay();
            CinemaClipAnimator.Play(CurrentClipName);
            //this.AnimLinkage.Play();

            
            setAnimsEnable(true);

            isPlaying = true;
        }






        private void OnDestroy()
        {
            if (GraphicsManager.isInited)
            {
                //  _graphicsManager.UpdateCurrentSubCamera();
                Bridge.OnClipEnd();
            }


        }
    }

}


