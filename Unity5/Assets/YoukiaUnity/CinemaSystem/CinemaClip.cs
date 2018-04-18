using System;
using System.Collections.Generic;
using System.Reflection;
using Framework;
using Framework.Graphic;
using UnityEngine;
using YoukiaUnity;

//using YoukiaUnity.Resource;

namespace YoukiaUnity.CinemaSystem
{

    /// <summary>
    /// 电影剪辑类
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CinemaClip : MonoBehaviour
    {

        public static bool UsedTempScene = false;

        /// <summary>
        /// 重置Clip
        /// </summary>
        /// <param name="clip"></param>
        public static void resetClip(CinemaClip clip)
        {
            int i;
            int len;

            //卸载所有加载的角色
            if (clip.ActorList != null && clip.ActorList.Count > 0)
            {
                len = clip.ActorList.Count;
                for (i = 0; i < len; i++)
                {
                    CinemaCharacter character = clip.ActorList[i];
                    if (character)
                    {
                        if (character.LoadedGameObject)
                        {
                            GameObject unInstallObj = character.LoadedGameObject;
                            SimpleCinemaView view = unInstallObj.GetComponent<SimpleCinemaView>();
                            if (view)
                            {
                                CinemaClip.UnloadPrefab(view.rootObject);
                                GameObject.Destroy(unInstallObj);
                            }
                        }
                        character.Reset();
                    }
                }
            }

            //卸载所有加载的动态挂点
            if (clip.SubMountPointsList != null && clip.SubMountPointsList.Count > 0)
            {
                len = clip.SubMountPointsList.Count;
                for (i = 0; i < len; i++)
                {
                    CinemaSubMountPoint subMountPoint = clip.SubMountPointsList[i];
                    if (subMountPoint)
                    {
                        subMountPoint.reset();
                    }
                }
            }

            //卸载所有加载的特效
            if (clip.EffectList != null && clip.EffectList.Count > 0)
            {
                len = clip.EffectList.Count;
                for (i = 0; i < len; i++)
                {
                    CinemaEffect effect = clip.EffectList[i];
                    if (effect)
                    {
                        if (effect.LoadedGameObject)
                        {
                            GameObject unInstallObj = effect.LoadedGameObject;
                            CinemaClip.UnloadPrefab(unInstallObj);
                        }
                        effect.Reset();
                    }
                }
            }

            //卸载所有加载的物件
            if (clip.ObjectList != null && clip.ObjectList.Count > 0)
            {
                len = clip.ObjectList.Count;
                for (i = 0; i < len; i++)
                {
                    CinemaObject cinemaObject = clip.ObjectList[i];
                    if (cinemaObject)
                    {
                        if (cinemaObject.LoadedGameObject)
                        {
                            GameObject unInstallObj = cinemaObject.LoadedGameObject;
                            CinemaClip.UnloadPrefab(unInstallObj);
                        }
                        cinemaObject.Reset();
                    }
                }
            }
        }

        private bool _isStarted = false;

        public static void LoadTexture(string path, Action<Texture> loadedCallback, params object[] param)
        {
            if (param != null && param.Length > 0)
            {
                ResourcesLoadBridge.LoadTextrue(path, loadedCallback, param);
            }
            else
            {
                ResourcesLoadBridge.LoadTextrue(path, loadedCallback);
            }
        }

        /// <summary>
        /// CinemaClip 加载接口
        /// </summary>
        /// <param name="path">CinemaClip加载path</param>
        /// <param name="loadedCallback">>加载成功后需要执行的回调(传递实例后的加载对象)</param>
        public static void LoadPrefab(string path, Action<GameObject> loadedCallback, params object[] param)
        {
            if (param != null && param.Length > 0)
            {
                ResourcesLoadBridge.LoadPrefab(path, loadedCallback, param);
            }
            else
            {
                ResourcesLoadBridge.LoadPrefab(path, loadedCallback);
            }
        }

        public static Func<GameObject> HideCurrentSceneCustom;
        public static GameObject HideCurrentScene()
        {
            if (HideCurrentSceneCustom != null)
            {
                return HideCurrentSceneCustom();
            }

            //default:
            //todo 隐藏当前场景
            return null;
        }

        public static Action<string, Action> ChangeToTempSceneCustom;
        public static void ChangeToTempScene(string sceneName, Action finishCallback)
        {
            if (UsedTempScene) return;

            UsedTempScene = true;

            if (ChangeToTempSceneCustom != null)
            {
                ChangeToTempSceneCustom(sceneName, finishCallback);
                return;
            }

            //default:
            //todo 加载临时场景
        }

        public static Action<Action> RestoreCurrentSceneCustom;
        public static void RestoreCurrentScene(Action finishCallback)
        {

            UsedTempScene = false;

            if (RestoreCurrentSceneCustom != null)
            {
                RestoreCurrentSceneCustom(finishCallback);
                return;
            }

            //default:
            //todo 恢复当前场景
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

                CinemaClip clip = prefab.GetComponent<CinemaClip>();
                if (clip)
                {
                    prefab.SetActive(false);
                }
                else
                {
                       GameObject.Destroy(prefab);
                }
            }
            else
            {
                GameObject.DestroyImmediate(prefab);
            }

        }

        /// <summary>
        /// CinemaClip专用加载预制体注入方法
        /// </summary>
        public static Action<string, bool> PlayAudioCustom;

        /// <summary>
        /// CinemaClip专用播放声音接口
        /// </summary>
        /// <param name="path"></param>
        public static void PlayAudio(string path, bool isLoop)
        {
            if (PlayAudioCustom != null)
            {
                PlayAudioCustom(path, isLoop);
                return;
            }

            //default:

            //... ...

        }

        /// <summary>
        /// CinemaClip专用加载预制体注入方法
        /// </summary>
        public static Action<string> StopAudioCustom;

        public static void StopAudio(string path)
        {
            if (StopAudioCustom != null)
            {
                StopAudioCustom(path);
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

        public static Action<string> StopBGMCustom;
        public static void StopBGM(string path)
        {

            if (StopBGMCustom != null)
            {
                StopBGMCustom(path);
            }

            //default:

            //... ...

        }

        //        public string BridgeAutoLoadAssembly;

        public string BridgeAutoLoadKey;

        public string BridgeAutoLoadValueString;

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
        /// 标识 此Clip在 低质量渲染模式下 会自动隐藏当前场景
        /// </summary>
        public bool HideCurrentSceneOnLowRenderQuality = false;

        /// <summary>
        /// 本次影片的角色;
        /// </summary>
        [NonSerialized] public List<CinemaCharacter> ActorList;

        /// <summary>
        /// 本次影片的特效
        /// </summary>
        [NonSerialized]public List<CinemaEffect> EffectList;

        [NonSerialized]public List<CinemaObject> ObjectList;

        [NonSerialized] public List<CinemaSubMountPoint> SubMountPointsList;

        /// <summary>
        /// 本次剪辑的摄影机列表
        /// </summary>
        [NonSerialized] public List<GameObject> CameraList;

        /// <summary>
        /// 所有动画
        /// </summary>
        [NonSerialized] public List<Behaviour> AnimList;


        private Dictionary<string, bool> _SubsActiveDic;

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

        private Action _onInitComplete;
        private bool _initCompleteRunOnce;

        public void setInitComplete(Action action, bool runonce = true)
        {
            _onInitComplete = action;
            _initCompleteRunOnce = runonce;
        }

        private Animator _CinemaClipAnimator;

        /// <summary>
        /// 自动开始播放剪辑
        /// </summary>
        public bool AutoPlay = true;

        private bool isPlaying;

        void getBridge()
        {
            Assembly _crrAss = Assembly.GetAssembly(GetType());
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
                //                Component bridge = YKBridgeUtils.AddDllScript(gameObject, BridgeAutoLoadKey);
                Component bridge = AddComponentBridge.AddComponent(gameObject, _crrAss, BridgeAutoLoadKey);
             
                if (bridge == null)
                {
                    //                    YKBridgeUtils.AddDllScript(gameObject, "BaseCinemaBridge");
//                    bridge = AddComponentBridge.AddComponent(gameObject, _crrAss, "BaseCinemaBridge");
                    bridge = gameObject.AddComponent<BaseCinemaBridge>();
                    return;
                }

                if (bridge is ICinemaBridge)
                {
                    Bridge = (ICinemaBridge)bridge;

                    if (!string.IsNullOrEmpty(BridgeAutoLoadValueString))
                    {
                        Bridge.setupValues(BridgeAutoLoadValueString);
                    }
                    return;
                }

            }

            Bridge = gameObject.AddComponent<BaseCinemaBridge>();
        }


        private void OnEnable()
        {
            if (!_isStarted) return;
            //恢复初始化Active状态
            resetActivesByDic(_SubsActiveDic, transform, transform);

            //
            init();
        }

        private string _currentHiddenSceneName;
        private GameObject _currentHiddenScene;

        private void OnDisable()
        {
            //必要的清理
            setLoadedGameObjectsActiveFalse();
            setAnimsEnable(false);

            if (HideCurrentSceneOnLowRenderQuality)
            {
                if (_currentHiddenScene && !_currentHiddenScene.activeInHierarchy)
                {
                    _currentHiddenScene.SetActive(true);
                }

            }

        }

        private void init()
        {

            //GraphicsManager.GetInstance()._mainCameraAnimation.Normal();

            LinkActor(() =>
            {
                LinkEffects(() =>
                {
                    LinkObjects(() =>
                    {
                        LinkSubMountPoints(() =>
                        {

//                            LinkCamera();

                            if (Bridge != null)
                            {
                                Bridge.OnClipStart();
                            }

                            _isStarted = true;

                            if (_onInitComplete != null)
                            {
                                if (_initCompleteRunOnce)
                                {
                                    Action tmp = _onInitComplete;
                                    _onInitComplete = null;
                                    tmp();
                                }
                                else
                                {
                                    _onInitComplete();
                                }
                            }

                            if (CinemaClipAnimator && AutoPlay)
                            {
                                Play();
                            }
                        });

                    });

                });

            });
        }

        private string _getInnerHPath(Transform t, Transform root, string cache = "")
        {
            if (t == root)
            {
                return t.gameObject.name + (string.IsNullOrEmpty(cache) ? "" : "/" + cache);
            }

            cache = t.gameObject.name + (string.IsNullOrEmpty(cache) ? "" : "/" + cache);
            if (t.parent)
            {
                return _getInnerHPath(t.parent, root, cache);
            }
            return cache;
        }

        private void setupActiveDic(Dictionary<string, bool> dic, Transform t, Transform root)
        {
            string innerHPath = _getInnerHPath(t, root);
            if (!dic.ContainsKey(innerHPath))
            {
                dic.Add(innerHPath, t.gameObject.activeSelf);
            }

            if (t.childCount > 0)
            {
                int i, len = t.childCount;
                for (i = 0; i < len; i++)
                {
                    Transform subT = t.GetChild(i);
                    setupActiveDic(dic, subT, root);
                }
            }
        }

        private void resetActivesByDic(Dictionary<string, bool> dic, Transform t,Transform root)
        {
            string innerHPath = _getInnerHPath(t, root);
            if (dic.ContainsKey(innerHPath))
            {
                t.gameObject.SetActive(dic[innerHPath]);
            }

            if (t.childCount > 0)
            {
                int i, len = t.childCount;
                for (i = 0; i < len; i++)
                {
                    Transform subT = t.GetChild(i);
                    resetActivesByDic(dic, subT, root);
                }
            }
        }

        void Awake()
        {
            GraphicsManager.RequestGraphicsManager(() =>
            {
                //建立初始化时Active图 ...
                _SubsActiveDic = new Dictionary<string, bool>();
                setupActiveDic(_SubsActiveDic, transform, transform);
                //_graphicsManager = GraphicsManager.GetInstance();
                getBridge();
                LinkAnim();
                setAnimsEnable(false);

                init();

            });
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

        void linkAllAnimtor(Transform tran)
        {
            List<Animator> ats = tran.FindComponentListInChildren<Animator>(true);

            if (ats == null || ats.Count == 0) return;

            for (int i = 0; i < ats.Count; i++)
            {
                Animator animator = ats[i];
                AnimList.Add(animator);
            }

            //已不在支持Animation
//            Animation tb = tran.GetComponent<Animation>();
//            if (tb) AnimList.Add(tb);
//
//            List<Animation> ans = tran.FindComponentListInChildren<Animation>();
//            for (int i = 0; i < ans.Count; i++)
//            {
//                AnimList.Add(ans[i]);
//            }

        }

        #region LinkActor

        /// <summary>
        /// 自动找到子节点内的角色(编辑器用)
        /// </summary>
        public void LinkActor(Action finish)
        {
            if (ActorList == null)
            {
                ActorList = new List<CinemaCharacter>();
            }
            else
            {
                ActorList.Clear();
            }
            linkAllActor(transform, finish);

        }

        void linkAllActor(Transform tran, Action finish)
        {
            //CinemaCharacter[] characters = tran.GetComponentsInChildren<CinemaCharacter>();

            List<CinemaCharacter> characters = tran.FindComponentListInChildren<CinemaCharacter>();

            if (characters == null || characters.Count == 0)
            {
                if (finish != null) finish();
                return;
            }

            for (int i = 0; i < characters.Count; i++)
            {
                ActorList.Add(characters[i]);
            }


            if (Application.isPlaying && characters.Count > 0)
            {
                loadActorLoop(characters.ToArray(), 0, finish);
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

        #endregion

        #region LinkEffect

        public void LinkEffects(Action finish)
        {
            if (EffectList == null)
            {
                EffectList = new List<CinemaEffect>();
            }
            else
            {
                EffectList.Clear();
            }
            linkAllEffects(transform, finish);
        }

        void linkAllEffects(Transform tran, Action finish)
        {
            List<CinemaEffect> effects = tran.FindComponentListInChildren<CinemaEffect>();

            if (effects == null || effects.Count == 0)
            {
                if (finish != null) finish();
                return;
            }

            for (int i = 0; i < effects.Count; i++)
            {
                EffectList.Add(effects[i]);
            }


            if (Application.isPlaying && effects.Count > 0)
            {

                loadEffectLoop(effects.ToArray(), 0, finish);
            }
        }

        void loadEffectLoop(CinemaEffect[] effects, int index, Action finish)
        {
            if (index >= effects.Length)
            {
                if (finish != null)
                {
                    finish();
                }
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(effects[index].EffectLoadPath))
                {
                    index += 1;
                    loadEffectLoop(effects, index, finish);
                }
                else if (effects[index].LoadedGameObject != null)
                {
                    effects[index].LoadedGameObject.transform.localRotation = Quaternion.identity;
                    effects[index].LoadedGameObject.transform.localPosition = Vector3.zero;
                    effects[index].LoadedGameObject.transform.localScale = new Vector3(effects[index].EffectScale, effects[index].EffectScale,
                        effects[index].EffectScale);

                    effects[index].LoadedGameObject.transform.SetParent(effects[index].transform, false);

                    if (isPlaying)
                    {
                        effects[index].LoadedGameObject.SetActive(true);
                    }
                    else
                    {
                        effects[index].LoadedGameObject.SetActive(false);
                    }

                    effects[index].LinkSubAnims();

                    index += 1;
                    loadEffectLoop(effects, index, finish);
                }
                else{
                    LoadPrefab(effects[index].EffectLoadPath, (obj) =>
                    {
                        
                        if (obj != null)
                        {

                            obj.transform.localRotation = Quaternion.identity;
                            obj.transform.localPosition = Vector3.zero;
                            obj.transform.localScale = new Vector3(effects[index].EffectScale,
                                effects[index].EffectScale,
                                effects[index].EffectScale);

                            obj.transform.SetParent(effects[index].transform, false);

                            if (isPlaying)
                            {
                                obj.SetActive(true);
                            }
                            else
                            {
                                obj.SetActive(false);
                            }
                            effects[index].LoadedGameObject = obj;
                            effects[index].LinkSubAnims();

                        }

                        index += 1;
                        loadEffectLoop(effects, index, finish);

                    });
                }
            }
        }

        #endregion

        #region LinkObject

        public void LinkObjects(Action finish)
        {
            if (ObjectList == null)
            {
                ObjectList = new List<CinemaObject>();
            }
            else
            {
                ObjectList.Clear();
            }
            linkAllObjects(transform, finish);
        }

        //linkAllObjects
        void linkAllObjects(Transform tran, Action finish)
        {
            List<CinemaObject> objects = tran.FindComponentListInChildren<CinemaObject>();

            if (objects == null || objects.Count == 0)
            {
                if (finish != null) finish();
                return;
            }

            for (int i = 0; i < objects.Count; i++)
            {
                ObjectList.Add(objects[i]);
            }


            if (Application.isPlaying && objects.Count > 0)
            {

                loadObjectLoop(objects.ToArray(), 0, finish);
            }
        }

        void loadObjectLoop(CinemaObject[] objects, int index, Action finish)
        {
            if (index >= objects.Length)
            {
                if (finish != null)
                {
                    finish();
                }
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(objects[index].ObjectLoadPath))
                {
                    index += 1;
                    loadObjectLoop(objects, index, finish);
                }
                else if (objects[index].LoadedGameObject)
                {
                    objects[index].LoadedGameObject.transform.localRotation = Quaternion.identity;
                    objects[index].LoadedGameObject.transform.localPosition = Vector3.zero;
                    objects[index].LoadedGameObject.transform.localScale = new Vector3(objects[index].ObjectScale, objects[index].ObjectScale,
                        objects[index].ObjectScale);

                    objects[index].LoadedGameObject.transform.SetParent(objects[index].transform, false);


                    if (isPlaying)
                    {
                        objects[index].LoadedGameObject.SetActive(true);
                    }
                    else
                    {
                        objects[index].LoadedGameObject.SetActive(false);
                    }
                    

                    index += 1;
                    loadObjectLoop(objects, index, finish);
                }
                else
                {
                    LoadPrefab(objects[index].ObjectLoadPath, (obj) =>
                    {

                        obj.transform.localRotation = Quaternion.identity;
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localScale = new Vector3(objects[index].ObjectScale, objects[index].ObjectScale,
                            objects[index].ObjectScale);

                        obj.transform.SetParent(objects[index].transform, false);

                        if (isPlaying)
                        {
                            obj.SetActive(true);
                        }
                        else
                        {
                            obj.SetActive(false);
                        }
                        objects[index].LoadedGameObject = obj;

                        index += 1;
                        loadObjectLoop(objects, index, finish);

                    });
                }
            }
        }

        #endregion

        #region LinkSubMountPoint

        public void LinkSubMountPoints(Action finish)
        {
            if (SubMountPointsList == null)
            {
                SubMountPointsList = new List<CinemaSubMountPoint>();
            }
            else
            {
                SubMountPointsList.Clear();
            }
            linkAllSubMountPoints(transform, finish);
        }

        void linkAllSubMountPoints(Transform tran, Action finish)
        {
            List<CinemaSubMountPoint> subMountPoints = tran.FindComponentListInChildren<CinemaSubMountPoint>();

            if (subMountPoints == null || subMountPoints.Count == 0)
            {
                if (finish != null) finish();
                return;
            }

            for (int i = 0; i < subMountPoints.Count; i++)
            {
                if (!subMountPoints[i].isInit)
                {
                    subMountPoints[i].init();
                }
                SubMountPointsList.Add(subMountPoints[i]);
            }

            if (finish != null) finish();
        }

        #endregion

//        /// <summary>
//        /// 自动找到子节点内的摄像机(编辑器用)
//        /// </summary>
//        public void LinkCamera()
//        {
//            if (CameraList == null)
//            {
//                CameraList = new List<GameObject>();
//            }
//            else
//            {
//                CameraList.Clear();
//            }
//            linkAllCamera(transform);
//        }

        /// <summary>
        /// 自动找到子节点内的动画(编辑器用)
        /// </summary>
        public void LinkAnim()
        {
            if (AnimList == null)
            {
                AnimList = new List<Behaviour>();
            }
            else
            {
                AnimList.Clear();
            }
            linkAllAnimtor(transform);
        }


//        void linkAllCamera(Transform tran)
//        {
//
//            foreach (Transform each in tran)
//            {
//                if (each.GetComponent<SubCameraInfo>() != null)
//                {
//                    CameraList.Add(each.gameObject);
//
//                }
//
//                if (each.childCount > 0)
//                    linkAllCamera(each);
//            }
//
//        }

        public CinemaCharacter GetActorByName(string _name)
        {

            for (int i = 0; i < ActorList.Count; i++)
            {
                if (ActorList[i].gameObject.name == _name)
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

            if (HideCurrentSceneOnLowRenderQuality)
            {
                _currentHiddenScene = HideCurrentScene();
            }

            setLoadedGameObjectsActiveTrue();

            Bridge.OnClipPlay();
            CinemaClipAnimator.Play(CurrentClipName, 0, 0);
            //this.AnimLinkage.Play();

            setAnimsEnable(true);

            isPlaying = true;
        }

        private void setLoadedGameObjectsActiveTrue()
        {
            int i, len;
            if (ActorList != null && ActorList.Count > 0)
            {
                len = ActorList.Count;
                for (i = 0; i < len; i++)
                {
                    CinemaCharacter cc = ActorList[i];
                    if (cc && cc.LoadedGameObject && !cc.LoadedGameObject.activeSelf)
                    {
                        cc.LoadedGameObject.SetActive(true);
                    }
                }
            }

            if (EffectList != null && EffectList.Count > 0)
            {
                len = EffectList.Count;
                for (i = 0; i < len; i++)
                {
                    CinemaEffect ce = EffectList[i];
                    if (ce && ce.LoadedGameObject && !ce.LoadedGameObject.activeSelf)
                    {
                        ce.LoadedGameObject.SetActive(true);
                    }
                }
            }

            if (ObjectList != null && ObjectList.Count > 0)
            {
                len = ObjectList.Count;
                for (i = 0; i < len; i++)
                {
                    CinemaObject co = ObjectList[i];
                    if (co && co.LoadedGameObject && !co.LoadedGameObject.activeSelf)
                    {
                        co.LoadedGameObject.SetActive(true);
                    }
                }
            }

        }

        private void setLoadedGameObjectsActiveFalse()
        {
            int i, len;
            if (ActorList != null && ActorList.Count > 0)
            {
                len = ActorList.Count;
                for (i = 0; i < len; i++)
                {
                    CinemaCharacter cc = ActorList[i];
                    if (cc && cc.LoadedGameObject && cc.LoadedGameObject.activeSelf)
                    {
                        cc.LoadedGameObject.SetActive(false);
                    }
                }
            }

            if (EffectList != null && EffectList.Count > 0)
            {
                len = EffectList.Count;
                for (i = 0; i < len; i++)
                {
                    CinemaEffect ce = EffectList[i];
                    if (ce && ce.LoadedGameObject && ce.gameObject.activeSelf)
                    {
                        ce.LoadedGameObject.SetActive(false);
                    }
                }
            }

            if (ObjectList != null && ObjectList.Count > 0)
            {
                len = ObjectList.Count;
                for (i = 0; i < len; i++)
                {
                    CinemaObject co = ObjectList[i];
                    if (co && co.LoadedGameObject && co.LoadedGameObject.activeSelf)
                    {
                        co.LoadedGameObject.SetActive(false);
                    }
                }
            }

        }

    }

}


