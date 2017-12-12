using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using YoukiaUnity.Misc;

namespace YoukiaUnity
{
    public class TimeScaleIgnore : MonoBehaviour
    {
        private double _lastTime;
        private ParticleSystem[] _particles;
        private Animation[] _animations;
        private Animator[] _animators;

        protected void Start()
        {
            OnEnable();
        }

        protected void OnEnable()
        {
            _particles = GetComponentsInChildren<ParticleSystem>();
            _animations = GetComponentsInChildren<Animation>();
            _animators = GetComponentsInChildren<Animator>();

            for (int i = 0; i < _animators.Length; i++)
            {
                _animators[i].updateMode = AnimatorUpdateMode.UnscaledTime;
            }

            _lastTime = Time.realtimeSinceStartup;
        }

        protected void OnDisable()
        {
            _particles = GetComponentsInChildren<ParticleSystem>();
            _animations = GetComponentsInChildren<Animation>();
            _animators = GetComponentsInChildren<Animator>();

            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i].Play();
            }

            for (int i = 0; i < _animators.Length; i++)
            {
                _animators[i].updateMode = AnimatorUpdateMode.Normal;
            }

            _particles = null;
            _animations = null;
            _animators = null;
        }

        protected void OnDestroy()
        {
            OnDisable();
        }


        protected virtual void Update()
        {
            float tpf = Time.realtimeSinceStartup - (float)_lastTime;
            float absTpf = tpf - Time.deltaTime;

//            _particles = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i].Simulate(tpf, false, false);
            }

//            _animations = GetComponentsInChildren<Animation>();
            for (int i = 0; i < _animations.Length; i++)
            {
                AnimationState currentState = _animations[i].GetCurrentState();
                if (currentState != null)
                {
                    currentState.time += absTpf * currentState.speed;
                    _animations[i].Sample();
                }
            }

//            _animators = GetComponentsInChildren<Animator>();
            for (int i = 0; i < _animators.Length; i++)
            {
                _animators[i].updateMode = AnimatorUpdateMode.UnscaledTime;
            }


            _lastTime = Time.realtimeSinceStartup;
        }
    }
}
