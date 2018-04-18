using System;
using Framework;
using UnityEngine;

namespace YoukiaUnity.CinemaSystem
{
    public class SimpleCinemaView : MonoBehaviour, IGameAnimObject
    {

        public static SimpleCinemaView Create(GameObject obj)
        {
            GameObject baseObj = new GameObject(obj.name);
            SimpleCinemaView view = baseObj.AddComponent<SimpleCinemaView>();
            view.Setup(obj);
            return view;
        }

        public void Setup(GameObject obj)
        {
            _rootObject = obj;
            _rootObject.transform.SetParent(transform, false);
//            _modelAnims = _rootObject.GetComponentsInChildren<Animation>();
            _modelAnimators = _rootObject.GetComponentsInChildren<Animator>();
            //强制设置 cullingMode = AnimatorCullingMode.AlwaysAnimate
            if (_modelAnimators != null && _modelAnimators.Length > 0)
            {
                int i, len = _modelAnimators.Length;
                for (i = 0; i < len; i++)
                {
                    _modelAnimators[i].cullingMode = AnimatorCullingMode.AlwaysAnimate;
                }
            }
        }

        private GameObject _rootObject;
        public GameObject rootObject
        {
            get { return _rootObject; }
        }

        public string state
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string CurrentAnimtation
        {
            get { return _nowClip == null ? "" : _nowClip.name; }
        }

        //Animation
//        private Animation[] _modelAnims;
        private AnimationClip _nowClip;

        //Animator
        private Animator[] _modelAnimators;
        private int _nowStateNameHash;

        public void PlayAnimtation(string anim, WrapMode wrap, float speed, float normalizedTime = 0)
        {
            //不再支持 Animation 类型动画
//            if (_modelAnims != null && _modelAnims.Length > 0)
//            {
//
//                for (int i = 0; i < _modelAnims.Length; i++)
//                {
//
//                    AnimationState state = _modelAnims[i][anim];
//
//                    if (state == null)
//                        continue;
//
//                    if (state.clip == _nowClip)
//                    {
//                        return;
//                    }
//
//                    else
//                    {
//                        _nowClip = state.clip;
//                        state.speed = (float)speed;
//                        state.wrapMode = wrap;
////                        state.time = (float)startTime;
//                        state.normalizedTime = normalizedTime;
//                        _modelAnims[i].CrossFade(anim, 0.2f);
//                    }
//
//                }
//
//            }

            //animator 
            if (_modelAnimators != null && _modelAnimators.Length > 0 && (_nowClip == null || Animator.StringToHash(anim) != _nowStateNameHash))
            {

                for (int i = 0; i < _modelAnimators.Length; i++)
                {
                    Animator ModelAnim = _modelAnimators[i];
                    if (string.IsNullOrEmpty(anim))
                        continue;
                    if (HasAnimator(ModelAnim, anim))
                    {
                        ModelAnim.Play(anim, 0, normalizedTime);

                        AnimatorClipInfo[] info = ModelAnim.GetCurrentAnimatorClipInfo(0);

                        if (info == null || info.Length == 0)
                            continue;

                        _nowClip = info[0].clip;

                        _nowStateNameHash = ModelAnim.GetCurrentAnimatorStateInfo(0).shortNameHash;

                        ModelAnim.speed = speed;
                        info[0].clip.wrapMode = wrap;

                    }
                    else
                    {
                        Log.Warning("Animator State Missing : " + anim + " at GameObject : " + ModelAnim.gameObject.name);
                    }
                }

            }

        }
        public bool HasAnimator(Animator animator, string name)
        {
            bool isExists = false;
            if (animator != null)
            {
                isExists = animator.HasState(0, Animator.StringToHash(name));
            }
            return isExists;
        }
        public void SetAnimtationSpeed(float speed)
        {
            if (_modelAnimators != null && _modelAnimators.Length > 0)
            {
                for (int i = 0; i < _modelAnimators.Length; i++)
                {
                    Animator ModelAnim = _modelAnimators[i];
                    if (ModelAnim)
                    {
                        ModelAnim.speed = speed;
                    }
                }
            }
        }

        public void SetVisible(bool visble)
        {
            //Todo 可视化实现 (未实现)

            //test
            gameObject.SetActive(visble);
        }
    }
}
