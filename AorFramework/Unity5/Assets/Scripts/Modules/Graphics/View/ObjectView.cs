using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using AorBaseUtility;
using AorFramework.module;
using UnityEngine;
using YoukiaUnity.Graphics.FastShadowProjector;

//using YoukiaUnity.Resource;

namespace YoukiaUnity.View
{
    /// <summary>
    /// unity的物体控制器基类
    /// </summary>
    public abstract class ObjectView : MonoBehaviour, IConfigurable<RoleViewConfig>
    {
        //        protected static GameObject _effectRoot;

        private IViewController _activeController;
        private List<IViewController> _controllers = new List<IViewController>();
        private bool _isVisible = true;
        //        private AudioManager _audio;

        private double _lastTime;
        //        private bool _isIgnoreTimeScale;
        protected List<KeyValuePair<GameObject, int>> _layerCache = new List<KeyValuePair<GameObject, int>>();
        private RoleViewConfig _config;

        private PivotPointData _pivotData;

        protected virtual PivotPointData PivotData
        {
            get
            {
                if (_pivotData == null)
                {
                    _pivotData = GetComponentInChildren<PivotPointData>();
                }
                return _pivotData;
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
        }

        public IViewController ActiveController
        {
            get { return _activeController; }
        }

        public T GetController<T>() where T : class, IViewController
        {
            IViewController c = null;
            for (int i = 0; i < _controllers.Count; i++)
            {
                if (_controllers[i] is T)
                {
                    c = _controllers[i];
                }
            }

            return c as T;
        }

        protected virtual void Awake()
        {
            //            if (_effectRoot == null)
            //            {
            //                _effectRoot = new GameObject("EffectManager");
            //                DontDestroyOnLoad(_effectRoot);
            //            }
        }

        public void SetVisible(bool b)
        {
            if (b != _isVisible)
            {
                _isVisible = b;
                Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].enabled = b;
                }
                ModelShadowProjector[] projectors = gameObject.GetComponentsInChildren<ModelShadowProjector>();
                for (int i = 0; i < projectors.Length; i++)
                {
                    projectors[i].enabled = b;
                }
            }
        }

        public virtual void Fade(bool isFadeIn, float time)
        {
        }

        public void SetLayer(string layer)
        {
            SetLayer(LayerMask.NameToLayer(layer));
        }

        public virtual void SetLayer(int layer)
        {
            _SetLayer(gameObject, layer, _layerCache.Count == 0);
        }

        private void _SetLayer(GameObject o, int layer, bool record)
        {
            if (record)
            {
                _layerCache.Add(new KeyValuePair<GameObject, int>(o, o.layer));
            }
            o.layer = layer;
            for (int i = 0; i < o.transform.childCount; i++)
            {
                _SetLayer(o.transform.GetChild(i).gameObject, layer, record);
            }
        }

        public virtual void ResetLayer()
        {
            foreach (KeyValuePair<GameObject, int> kv in _layerCache)
            {
                if (kv.Key.gameObject != null)
                {
                    kv.Key.layer = kv.Value;
                }
            }
            _layerCache.Clear();
        }


        public void PlayAnimtation(string anim, WrapMode wrap = WrapMode.Clamp, float speed = 1)
        {
            PlayAnimtation(anim, wrap, speed, 0);
        }

        public void SetControllerEnable<T>()
        {
            IViewController c = null;

            for (int i = 0; i < _controllers.Count; i++)
            {
                if (_controllers[i] is T)
                {
                    c = _controllers[i];
                    break;
                }
            }

            if (c != null && c != _activeController)
            {
                SetControllerDisable();
                _activeController = c;
                _activeController.View = this;
                _activeController.OnEnable();
            }
        }

        public void SetControllerDisable()
        {
            if (_activeController != null)
            {
                _activeController.OnDisable();
                _activeController = null;
            }
        }

        public void AddControllers(params IViewController[] controllers)
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                RemoveController(controllers[i].GetType());
            }
            _controllers.AddRange(controllers);
        }

        public void RemoveController<T>()
        {
            _controllers.RemoveAll((c) => { return c is T; });
        }

        protected void RemoveController(Type t)
        {
            _controllers.RemoveAll(t.IsInstanceOfType);
        }

        public void RemoveControllers()
        {
            _controllers.Clear();
        }

        protected virtual void Update()
        {
            if (_activeController != null)
            {
                _activeController.Update();
            }

            //            if (_isIgnoreTimeScale)
            //            {
            //                ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
            //                double tpf = Time.realtimeSinceStartup - (double)_lastTime;
            //                double absTpf = tpf - Time.deltaTime;
            //                for (int i = 0; i < particles.Length; i++)
            //                {
            //                    particles[i].Simulate(tpf, false, false);
            //                }
            //
            //                Animation[] animations = GetComponentsInChildren<Animation>();
            //                for (int i = 0; i < animations.Length; i++)
            //                {
            //                    AnimationState _currentState = animations[i].GetCurrentState();
            //                    if (_currentState != null)
            //                    {
            //                        _currentState.time += absTpf * _currentState.speed;
            //                        animations[i].Sample();
            //                    }
            //                }
            //
            //
            //                _lastTime = Time.realtimeSinceStartup;
            //            }
        }

        public abstract string[] Animtations { get; }

        public abstract string CurrentAnimtation { get; }

        public abstract void PlayAnimtation(string anim, WrapMode wrap, float speed, float startTime);

        //        public virtual void PlaySound(string sound)
        //        {
        //            if (_audio == null)
        //            {
        //                _audio = YKApplication.Instance.GetManager<AudioManager>();
        //            }
        //            try
        //            {
        //                _audio.PlaySound(sound);
        //            }
        //            catch (Exception)
        //            {
        //                YoukiaCore.Log.Warning("ObjectView:" + name + " could not find Sound:" + sound + "!");
        //            }
        //        }


        public virtual GameObject PlayClip(string clipPath)
        {
            //            YKApplication.Instance.GetManager<ResourcesManager>().PoolMg.LoadObjectFromPool(clipPath, (obj) =>
            //            {
            //
            //
            //            });

            return new GameObject(clipPath);
        }

        public GameObject PlayEffect(string effect)
        {
            return PlayEffect(effect, true);
        }

        //注意:在disable操作后会直接释放该特效 
        public virtual GameObject PlayEffect(string effect, bool autoDestory)
        {
            GameObject assGameObject = EffectManager.Instance.CreateEffect(effect, autoDestory); //Resources.Load<GameObject>(effect);
            if (assGameObject != null)
            {


                EffectDescript evb = assGameObject.GetComponent<EffectDescript>();
                if (evb == null)
                {
                    evb = assGameObject.AddComponent<EffectDescript>();
                }
     
 
                Transform pivotT = GetPivot(evb.EffectPivot);
                switch (evb.effectPivotType)
                {
                    case EffectPivotType.Follow:
                        assGameObject.transform.SetParent(pivotT, false);
                        assGameObject.transform.position = pivotT.position;
                        assGameObject.transform.eulerAngles = transform.eulerAngles;
                        break;
                    case EffectPivotType.World:
                        assGameObject.transform.position = pivotT.position;
                        assGameObject.transform.eulerAngles = transform.eulerAngles;
                        break;
                    case EffectPivotType.WorldPos:
                        assGameObject.transform.position = pivotT.position;
                        break;
                    default:
                        assGameObject.transform.position = transform.position;
                        assGameObject.transform.rotation = transform.rotation;
                        break;

                }
 
                assGameObject.SetActive(true);

                return assGameObject;
            }
            assGameObject = new GameObject();
            assGameObject.AddComponent<DummyEffectHandle>().Init();
            return assGameObject;
        }

        public static void DisposeEffect(GameObject effect)
        {
            //            if (effect == null)
            //            {
            //                return;
            //            }
            //            DummyEffectHandle effectHandler = effect.transform.GetComponentInChildren<DummyEffectHandle>();
            //            if (effectHandler != null)
            //            {
            //                effectHandler.IsDispose = true;
            //            }
            EffectManager.Instance.DestroyEffect(effect);
        }

        public RoleViewConfig Config { get { return _config; } }
        public void SetConfig(RoleViewConfig config)
        {
            _config = config;
        }

        public Transform GetPivot(string pName)
        {
            if (PivotData == null)
                return transform;
            else
            {
                GameObject go = PivotData.GetPivot<GameObject>(pName);

                if (go == null)
                {
                    return transform;
                }
                else
                {
                    return go.transform;
                }
            }
        }
    }
}