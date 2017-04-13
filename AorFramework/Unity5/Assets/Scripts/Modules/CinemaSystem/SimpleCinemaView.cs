using System;
using UnityEngine;
using YoukiaUnity.View;

namespace YoukiaUnity.CinemaSystem
{
    public class SimpleCinemaView : ObjectView
    {
        public override string[] Animtations
        {
            get { throw new NotImplementedException(); }
        }

        public override string CurrentAnimtation
        {
            get { return _nowClip == null ? "" : _nowClip.name; }
        }

        //Animation
        private Animation[] _modelAnims;
        private AnimationClip _nowClip;

        //Animator
        private Animator[] _modelAnimators;
        private int _nowStateNameHash;

        public override void PlayAnimtation(string anim, WrapMode wrap, float speed, float startTime)
        {
            if (_modelAnims != null && _modelAnims.Length > 0)
            {

                for (int i = 0; i < _modelAnims.Length; i++)
                {

                    AnimationState state = _modelAnims[i][anim];

                    if (state == null)
                        continue;

                    if (state.clip == _nowClip)
                    {
                        return;
                    }

                    else
                    {
                        _nowClip = state.clip;
                        state.speed = (float)speed;
                        state.wrapMode = wrap;
                        state.time = (float)startTime;
                        _modelAnims[i].CrossFade(anim, 0.2f);
                    }

                }

            }

            //animator 
            if (_modelAnimators != null && _modelAnimators.Length > 0 && (_nowClip == null || Animator.StringToHash(anim) != _nowStateNameHash))
            {

                for (int i = 0; i < _modelAnimators.Length; i++)
                {
                    Animator ModelAnim = _modelAnimators[i];
                    ModelAnim.Play(anim, 0, startTime);

                    AnimatorClipInfo[] info = ModelAnim.GetCurrentAnimatorClipInfo(0);

                    if (info == null || info.Length == 0)
                        continue;

                    _nowClip = info[0].clip;

                    _nowStateNameHash = ModelAnim.GetCurrentAnimatorStateInfo(0).shortNameHash;

                    ModelAnim.speed = speed;
                    info[0].clip.wrapMode = wrap;

                }



            }

        }



        public static SimpleCinemaView Create(GameObject obj)
        {
//            GameObject obj = Instantiate(prefab) as GameObject;

            GameObject baseObj = new GameObject(obj.name);

            SimpleCinemaView view = baseObj.AddComponent<SimpleCinemaView>();
            obj.transform.SetParent(baseObj.transform, false);
            view._modelAnims = obj.GetComponentsInChildren<Animation>();
            view._modelAnimators = obj.GetComponentsInChildren<Animator>();
            return view;
        }


    }
}
