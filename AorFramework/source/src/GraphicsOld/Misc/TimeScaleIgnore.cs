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
        private ICustomTimeScale[] _customTimeScales;

        protected void Start()
        {
//            OnEnable();
        }

        protected void OnEnable()
        {
            _particles = GetComponentsInChildren<ParticleSystem>(true);
            _animations = GetComponentsInChildren<Animation>(true);
            _animators = GetComponentsInChildren<Animator>(true);
            _customTimeScales = GetComponentsInChildren<ICustomTimeScale>(true);
            

            for (int i = 0; i < _animators.Length; i++)
            {
                _animators[i].updateMode = AnimatorUpdateMode.UnscaledTime;
            }

            for (int i = 0; i < _customTimeScales.Length; i++)
            {
                _customTimeScales[i].Pause = true;
            }

            _lastTime = Time.realtimeSinceStartup;
        }

        protected void OnDisable()
        {
//            _particles = GetComponentsInChildren<ParticleSystem>();
//            _animations = GetComponentsInChildren<Animation>();
//            _animators = GetComponentsInChildren<Animator>();

            if (_particles != null)
            {
                for (int i = 0; i < _particles.Length; i++)
                {
                    _particles[i].Play();
                }
            }

            if (_animators != null)
            {
                for (int i = 0; i < _animators.Length; i++)
                {
                    _animators[i].updateMode = AnimatorUpdateMode.Normal;
                }
            }

            if (_customTimeScales != null)
            {
                for (int i = 0; i < _customTimeScales.Length; i++)
                {
                    _customTimeScales[i].Pause = false;
                }
            }

            _particles = null;
            _animations = null;
            _animators = null;
            _customTimeScales = null;
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
                if (_particles[i].gameObject.activeInHierarchy)
                {
                    _particles[i].Simulate(tpf, false, false);
                }
            }

//            _animations = GetComponentsInChildren<Animation>();
            for (int i = 0; i < _animations.Length; i++)
            {
                if (_animations[i].gameObject.activeInHierarchy)
                {
                    AnimationState currentState = _animations[i].GetCurrentState();
                    if (currentState != null)
                    {
                        currentState.time += absTpf*currentState.speed;
                        _animations[i].Sample();
                    }
                }
            }

//            _animators = GetComponentsInChildren<Animator>();
            for (int i = 0; i < _animators.Length; i++)
            {
                if (_animators[i].gameObject.activeInHierarchy)
                {
                    _animators[i].updateMode = AnimatorUpdateMode.UnscaledTime;
                }
            }

            for (int i = 0; i < _customTimeScales.Length; i++)
            {
                if (((MonoBehaviour) _customTimeScales[i]).gameObject.activeInHierarchy)
                {
                    _customTimeScales[i].Update(tpf);
                }
            }


            _lastTime = Time.realtimeSinceStartup;
        }
    }
}
