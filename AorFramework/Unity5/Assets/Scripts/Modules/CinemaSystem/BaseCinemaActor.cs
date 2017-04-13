using System;
using AorFramework.module;
using UnityEngine;
using YoukiaCore;
using YoukiaUnity.Graphics.FastShadowProjector;
//using YoukiaUnity.App;
using YoukiaUnity.View;

namespace YoukiaUnity.CinemaSystem
{

    /// <summary>
    /// 角色的接口实现类,描述演员如何进入角色以及表演过程
    /// </summary>
    public class BaseCinemaActor : ICinemaActor, IViewController
    {
        /// <summary>
        /// 需要进行表演的模型
        /// </summary>
        public ObjectView roleView;
        public ICinemaBridge bridge;
//        private AnimationClipConfig cfg;

        private void _checkCharacterUseModelShadowProjector(bool used, GameObject target)
        {
            if (used)
            {
                ModelShadowProjector MSP = target.GetComponentInChildren<ModelShadowProjector>();
                if (MSP == null)
                {
                    target.AddComponent<ModelShadowProjector>();
                }
            }
            else
            {
                ModelShadowProjector MSP = target.GetComponentInChildren<ModelShadowProjector>();
                if (MSP != null)
                {
                    MSP.enabled = false;
                }
            }
        }

        public virtual void Initialization(CinemaCharacter character, ICinemaBridge _bridge)
        {
            bridge = _bridge;

            if (character.HideBindingActorPrefab && !string.IsNullOrEmpty(character.ActorID))
            {
                ObjectView hv = bridge.LoadPlayer(character.ActorID);
                if (hv != null)
                {
                    _bridge.addHidePrefab(hv.gameObject);
                }
            }

            //预定的演员上场
            if (!character.UseActorIDPreBinding && !string.IsNullOrEmpty(character.ActorLoadPath))
            {

                CinemaClip.LoadPrefab(character.ActorLoadPath, (go) =>
                {
                    //预定的演员上场
                    character.CreateBySelf = true;
                    SimpleCinemaView view = SimpleCinemaView.Create(go);
                    View = view;
                    view.gameObject.SetActive(true);
                    view.gameObject.transform.parent = character.transform;
                    view.gameObject.transform.localPosition = Vector3.zero;
                    view.gameObject.transform.localEulerAngles = Vector3.zero;


                    character.name = View.name;
                    _checkCharacterUseModelShadowProjector(character.UseModelShadowProjector, view.gameObject);
                });

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

                        CinemaClip.LoadPrefab(character.ActorLoadPath, (o) =>
                        {

                            character.CreateBySelf = true;
                            SimpleCinemaView view = SimpleCinemaView.Create(o);
                            View = view;
                            view.gameObject.SetActive(true);
                            view.gameObject.transform.parent = character.transform;
                            view.gameObject.transform.localPosition = Vector3.zero;
                            view.gameObject.transform.localEulerAngles = Vector3.zero;

                                character.name = View.name;
                                _checkCharacterUseModelShadowProjector(character.UseModelShadowProjector,
                                    view.gameObject);

                        });

                    }
                    return;
                }

                }

                character.name = View.name;
                _checkCharacterUseModelShadowProjector(character.UseModelShadowProjector, View.gameObject);

            }

        }


        public void SetVisble(bool bo)
        {
            if (View != null)
                View.SetVisible(bo);
        }
        
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

            character.Currentbehavior = character.BehaviorIDToBehaviorName();

            ActorPlay(character.Currentbehavior);

            if (!character.CreateBySelf && View != null && View.gameObject != null)
            {


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
				View.PlayAnimtation(AnimName, WrapMode.Loop);
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




        public ObjectView View
        {
            get { return roleView; }

            set { roleView = value; }
        }
    }
}


