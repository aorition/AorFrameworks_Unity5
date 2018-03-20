using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using YoukiaCore;
//using YoukiaUnity.App;
//using YoukiaUnity.Audio;
using YoukiaUnity.CinemaSystem;
using YoukiaUnity.Graphics;
using YoukiaUnity.Graphics.FastShadowProjector;
using YoukiaUnity.Misc;
using Object = UnityEngine.Object;

//using YoukiaUnity.Resource;


namespace YoukiaUnity.View
{
    /// <summary>
    /// unity的物体控制器基类
    /// </summary>
    public abstract class ObjectView : MonoBehaviour, IConfigurable<Config>
    {
        //        protected static GameObject _effectRoot;
        private IViewController _activeController;
        private List<IViewController> _controllers = new List<IViewController>();
        protected bool _isVisible = true;
        //        private AudioManager _audio;

        private double _lastTime;
        //        private bool _isIgnoreTimeScale;
        protected List<KeyValuePair<GameObject, int>> _layerCache = new List<KeyValuePair<GameObject, int>>();
        private Config _config;

        private PivotPointData _pivotData;

        private static EffectDescript _defaultEffectDescript = new EffectDescript();

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
            set { _pivotData = value; }
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

        private List<GameObject> _disabledObjects = new List<GameObject>(); 
        private List<Renderer> _disabledRenderers = new List<Renderer>(); 

        public virtual void SetVisible(bool b)
        {
            if (!gameObject) return;
            if (b != _isVisible)
            {
                _isVisible = b;

                if (!b)
                {
                    List<Renderer> renderers = gameObject.FindAllComponentsInChildrenIncSelf<Renderer>();
                    for (int i = 0; i < renderers.Count; i++)
                    {
                        if (renderers[i].gameObject.tag == "ItemSubEffect")
                        {
                            renderers[i].gameObject.SetActive(false);
                            _disabledObjects.Add(renderers[i].gameObject);
                            continue;
                        }

                        if (!(renderers[i] is ParticleSystemRenderer))
                        {
                            renderers[i].enabled = false;
                            _disabledRenderers.Add(renderers[i]);
                        }
                    }
                }
                else
                {
                    foreach (GameObject go in _disabledObjects)
                    {
                        go.SetActive(true);
                    }
                    _disabledObjects.Clear();
                    foreach (Renderer r in _disabledRenderers)
                    {
                        r.enabled = true;
                    }
                    _disabledRenderers.Clear();
                }

                List<ModelShadowProjector> projectors = gameObject.FindAllComponentsInChildrenIncSelf<ModelShadowProjector>();
                for (int i = 0; i < projectors.Count; i++)
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
//            _controllers.RemoveAll(t.IsInstanceOfType);

            _controllers.RemoveAll(obj=>
            {
                if (t.IsInstanceOfType(obj))
                {
                    if (obj == _activeController)
                    {
                        _activeController.OnDisable();
                        _activeController.View = null;
                        _activeController = null;
                    }
                    return true;
                }

                return false;

//                return t.IsInstanceOfType(obj);
            });
        }


        public void RemoveControllers()
        {
            _controllers.Clear();
        }

        protected virtual void Update()
        {
//            if (_activeController != null)
//            {
//                _activeController.Update();
//            }
            //if (transform.localScale.x !=1 && this is ModelView)
            //{
            //    Log.error("ScaleX" + transform.localScale.x);
            //}
        }

        protected virtual void FixedUpdate()
        {
            if (_activeController != null)
            {
                _activeController.Update();
            }
            //if (transform.localScale.x !=1 && this is ModelView)
            //{
            //    Log.error("ScaleX" + transform.localScale.x);
            //}
        }

        public abstract string[] Animtations { get; }

        public abstract string CurrentAnimtation { get; }

        public abstract float CurrentAnimtationLength { get; }

        public abstract void PlayAnimtation(string anim, WrapMode wrap, float speed, float startTime);

        public abstract void SetAnimtationSpeed(float speed);

        //public virtual GameObject PlayClip(string clipPath)
        //{
        //    return new GameObject(clipPath);
        //}

        //注意:在disable操作后会直接释放该特效 
        //注意2： ObjectView.IsVisible == false 时，PlayEffect无效
        public virtual void PlayEffect(string effect, CallBack<GameObject> call = null, bool autoDestory = true, bool useCacheEffect = true)
        {
            if (effect.IsNullOrEmpty()) return;

            EffectManager.Instance.CreateEffect(effect, autoDestory, useCacheEffect, (assGameObject) =>
             {
                 if (!this)
                 {
                     return;
                 }

                 if (assGameObject != null)
                {
                    EffectDescript evb = assGameObject.GetComponent<EffectDescript>();
                    if (evb == null)
                    {
                        evb = _defaultEffectDescript;
                    }
                    else
                    {
                        evb.EffectRootGameObject = this.gameObject;
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
                        case EffectPivotType.Screen:
                            if (GraphicsManager.GetInstance() != null && GraphicsManager.GetInstance().EffectCamera)
                            {
                                 assGameObject.transform.SetParent(GraphicsManager.GetInstance().EffectCamera.transform, false);
                                 assGameObject.transform.localEulerAngles = Vector3.zero;
                                 assGameObject.transform.localPosition = new Vector3(0, 0, 1);
                            }
                            break;
                        default:
                            assGameObject.transform.position = transform.position;
                            assGameObject.transform.rotation = transform.rotation;
                            break;
                    }
                    assGameObject.SetActive(true);

                    Utils.SetAnimatorCullingMode(assGameObject);

                    if (call != null)
                    {
                        call(assGameObject);
                    }
                    return;
                }

                assGameObject = new GameObject();
//                assGameObject.AddComponent<DummyEffectHandle>().Init();
                if (call != null)
                {
                    call(assGameObject);
                }

                if (!IsVisible)
                {
                    assGameObject.SetVisible(false);
                }
            });
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

        public Config Config { get { return _config; } }

        public void SetConfig(Config config)
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