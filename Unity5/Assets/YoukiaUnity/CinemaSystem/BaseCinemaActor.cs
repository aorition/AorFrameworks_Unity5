using System;
using AorBaseUtility;
using AorFramework;
using UnityEngine;

namespace YoukiaUnity.CinemaSystem
{

    /// <summary>
    /// 角色的接口实现类,描述演员如何进入角色以及表演过程
    /// </summary>
    public class BaseCinemaActor : ICinemaActor
    {
        /// <summary>
        /// 需要进行表演的模型
        /// </summary>
        public IGameAnimObject roleView;
        public ICinemaBridge bridge;
        public CinemaCharacter character;
         
        //        private AnimationClipConfig cfg;

//        private void _checkCharacterUseModelShadowProjector(bool used, GameObject target)
//        {
//            if (used)
//            {
//                ModelShadowProjector MSP = target.GetComponentInChildren<ModelShadowProjector>();
//                if (MSP == null)
//                {
//                    target.AddComponent<ModelShadowProjector>();
//                }
//            }
//            else
//            {
//                ModelShadowProjector MSP = target.GetComponentInChildren<ModelShadowProjector>();
//                if (MSP != null)
//                {
//                    MSP.enabled = false;
//                }
//            }
//        }

        public virtual void Initialization(CinemaCharacter character, ICinemaBridge bridge, Action finish = null)
        {

            this.bridge = bridge;
            this.character = character;


            if (character.LoadedGameObject)
            {

                View = character.LoadedGameObject.GetInterface<IGameAnimObject>();

                initObjectViewSacle();

                character.LinkSubAnimator();

                if (finish != null) finish();
                return;
            }

            if (character.HideBindingActorPrefab && !string.IsNullOrEmpty(character.ActorID))
            {
                IGameObject hv = bridge.LoadPlayer(character.ActorID);
                if (hv != null)
                {
                    bridge.addHidePrefab(hv.gameObject);
                }
            }

            //预定的演员上场
            if (!character.UseActorIDPreBinding && !string.IsNullOrEmpty(character.ActorLoadPath))
            {

                CinemaClip.LoadPrefab(character.ActorLoadPath, (go) =>
                {

                    if (go == null)
                    {
                        if (finish != null) finish();
                        return;
                    }

                    //预定的演员上场
                    character.CreateBySelf = true;
                    go.SetActive(true);
                    SimpleCinemaView view = SimpleCinemaView.Create(go);
                    View = view;
                    
                    view.gameObject.transform.parent = character.transform;
                    view.gameObject.transform.localPosition = Vector3.zero;
                    view.gameObject.transform.localEulerAngles = Vector3.zero;
                    view.gameObject.SetActive(false);

                    //单独处理角色模型缩放
                    initObjectViewSacle();

                    character.name = View.name;
                    character.LoadedGameObject = view.gameObject;
                    character.LinkSubAnimator();

//                    _checkCharacterUseModelShadowProjector(character.UseModelShadowProjector, view.gameObject);

                    if (finish != null) finish();
                },0);
                return;
            }
            else if (!string.IsNullOrEmpty(character.ActorID))
            {
                //通过定义的关键字挑选演员上场
                
                View = bridge.CheckLongTao(character);

                if (View == null)
                {
                    View = bridge.LoadPlayer(character.ActorID);

                    //解决在预关联角色模式下，通过character.ActorID不能找到角色对象的补救方法
                    if (View == null)
                    {
                        if (character.UseActorIDPreBinding && !string.IsNullOrEmpty(character.ActorLoadPath))
                        {
                            Log.Error("补救？拿资源里的人");
                            CinemaClip.LoadPrefab(character.ActorLoadPath, (o) =>
                            {

                                if (o == null)
                                {
                                    if (finish != null) finish();
                                    return;
                                }

                                o.SetActive(true);
                                character.CreateBySelf = true;
                                SimpleCinemaView view = SimpleCinemaView.Create(o);
                                View = view;
                                view.gameObject.transform.parent = character.transform;
                                view.gameObject.transform.localPosition = Vector3.zero;
                                view.gameObject.transform.localEulerAngles = Vector3.zero;

                                //单独处理角色模型缩放
                                initObjectViewSacle();

                                character.name = View.name;
                                view.gameObject.SetActive(false);
                                character.LoadedGameObject = view.gameObject;
                                character.LinkSubAnimator();

//                                _checkCharacterUseModelShadowProjector(character.UseModelShadowProjector, view.gameObject);

                                if (finish != null) finish();

                            }, 0);
                            return;
                        }
                    }
                }

                if (View != null)
                {
                    //单独处理角色模型缩放
                    initObjectViewSacle();

                    character.LinkSubAnimator();

                    character.name = View.name;
//                    _checkCharacterUseModelShadowProjector(character.UseModelShadowProjector, View.gameObject);
                }

                if (finish != null) finish();
                return;

            }

            if (finish != null) finish();
        }

        private void initObjectViewSacle()
        {
            View.transform.localScale = new Vector3(character.ActorScale, character.ActorScale, character.ActorScale);
        }

        public void SetVisible(bool bo)
        {
            if (View != null)
                View.SetVisible(bo);
        }

        private Animator _linkedViewAnimator;

        public virtual void OnUpdate(CinemaCharacter character)
        {
            /**
             * (废弃) 已不需要使用 AnimationClipConfig来管理BehaviorID到BehaviorName的对应关系了
            cfg = ConfigManager.Instance.Get<AnimationClipConfig>((long)character.BehaviorID);

            if (cfg == null)
            {
                Main.isDebug = true;
                ConfigBridge.ImportConfig<AnimationClipConfig>("", null);
                cfg = ConfigManager.Instance.Get<AnimationClipConfig>((long)character.BehaviorID);
            }

            if (cfg == null)
                return;
            */

            //character.Currentbehavior = cfg.AnimationName;

            SetVisible(!character.HideRole);

            character.Currentbehavior = character.BehaviorIDToBehaviorName();
            if (!character.IgnoreActorBehavior)
            {
                ActorPlay(character.Currentbehavior);
            }

            if (!character.CreateBySelf && View != null && View.gameObject != null)
            {

                if (!_linkedViewAnimator) {
                    _linkedViewAnimator = View.gameObject.GetComponentInChildren<Animator>();
                }

                _linkedViewAnimator.speed = character.Speed;

                if (character.FollowPrefab)
                {

                    //角色位置跟随演员
                    character.transform.position = View.gameObject.transform.position;
                    character.transform.eulerAngles = View.gameObject.transform.eulerAngles;
                }
                else
                {
                    //演员跟随预定的角色位置
                    View.gameObject.transform.position = character.transform.position;
                    View.gameObject.transform.eulerAngles = character.transform.eulerAngles;

                    //处理缩放
                    character.transform.localScale = new Vector3(character.ActorScale, character.ActorScale, character.ActorScale);
                    View.gameObject.transform.localScale = character.transform.lossyScale;

                }

            }


        }


        /// <summary>
        /// 播放对应的行为动作
        /// </summary>
        /// <param name="AnimName">动画名字</param>
        public void ActorPlay(string AnimName)
        {
            if (View != null && View.CurrentAnimtation != AnimName)
            {
                //View.PlayAnimtation(AnimName, WrapMode.Loop, 0);
                View.PlayAnimtation(AnimName, WrapMode.Loop, character.Speed);
            }
        }

        public void Update()
        {
            // throw new NotImplementedException();
        }

        public void OnEnable()
        {
            // throw new NotImplementedException();
        }

        public void OnDisable()
        {
            //  throw new NotImplementedException();
        }



        /// <summary>
        /// 返回改view的gamoObject
        /// </summary>
        public GameObject gameObject
        {
            get
            {
                if (View != null)
                {
                    return View.gameObject;
                }
                else
                {
                    return null;
                }

            }
        }

        public IGameAnimObject View
        {
            get { return roleView; }

            set { roleView = value; }
        }
    }
}


