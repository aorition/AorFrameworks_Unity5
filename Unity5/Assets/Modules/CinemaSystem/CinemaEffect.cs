using System;
using System.Collections.Generic;
using UnityEngine;

namespace YoukiaUnity.CinemaSystem
{
    /// <summary>
    /// 电影系统特效类
    /// 提过特效挂载点
    /// </summary>
    public class CinemaEffect : MonoBehaviour, ICinemaEffectGizmo
    {

        [NonSerialized]
        public List<Animator> LoadedAnimators = new List<Animator>();
//        [NonSerialized]
//        public List<Animation> LoadedAnimations = new List<Animation>();
        [NonSerialized]
        public List<ParticleSystem> LoadedParticles = new List<ParticleSystem>();

        [NonSerialized]
        public GameObject LoadedGameObject;

        /// <summary>
        /// 关联特效预制体路径
        /// </summary>
        [SerializeField]//[HideInInspector]
        private string _EffectLoadPath;
        public string EffectLoadPath
        {
            get { return _EffectLoadPath; }
        }

        /// <summary>
        /// 角色缩放值
        /// </summary>
        public float EffectScale = 1f;

        /// <summary>
        /// 动画播放速度倍率
        /// </summary>
        public float Speed = 1f;

        public void Reset()
        {
            LoadedAnimators.Clear();
//            LoadedAnimations.Clear();
            LoadedParticles.Clear();
            LoadedGameObject = null;
        }

        public void LinkSubAnims()
        {
            LoadedAnimators.Clear();
//            LoadedAnimations.Clear();
            LoadedParticles.Clear();

            if (LoadedGameObject)
            {
                _LinkSubAnimsLoop(LoadedGameObject.transform);
            }

        }

        private void _LinkSubAnimsLoop(Transform t)
        {
            Animator an = t.GetComponent<Animator>();
            if(an) LoadedAnimators.Add(an);

//            Animation an2 = t.GetComponent<Animation>();
//            if (an) LoadedAnimations.Add(an2);

            ParticleSystem ps = t.GetComponent<ParticleSystem>();
            if (an) LoadedParticles.Add(ps);

            if (t.childCount > 0)
            {
                int i, len = t.childCount;
                for (i = 0; i < len; i++)
                {
                    Transform subT = t.GetChild(i);
                    _LinkSubAnimsLoop(subT);
                }
            }

        }

        private int i = 0; 
        private int len = 0;
        private void Update()
        {

            if (LoadedGameObject)
            {
                LoadedGameObject.transform.localScale = new Vector3(EffectScale, EffectScale, EffectScale);
            }

            if (LoadedAnimators != null && LoadedAnimators.Count > 0)
            {

                len = LoadedAnimators.Count;
                for (i = 0; i < len; i++)
                {
                    if(LoadedAnimators[i]) LoadedAnimators[i].speed = Speed;
                }
            }

//            if (LoadedAnimations != null && LoadedAnimations.Count > 0)
//            {
//
//                len = LoadedAnimations.Count;
//                for (i = 0; i < len; i++)
//                {
//                    
////                    if (LoadedAnimations[i]) LoadedAnimations[i].GetCurrentState().speed = Speed;
//                    if (LoadedAnimations[i]) LoadedAnimations[i].
//                }
//            }

            if (LoadedParticles != null && LoadedParticles.Count > 0)
            {

                len = LoadedParticles.Count;
                for (i = 0; i < len; i++)
                {
                    if (LoadedParticles[i])
                    {
                        var main = LoadedParticles[i].main;
                        main.simulationSpeed = Speed;
                    } 
                }
            }

        }

    }
}
