using System;
using System.Collections;
using UnityEngine;

namespace Framework.Graphic
{
    public class LightmapManager : MonoBehaviour
    {

        private static string _NameDefine = "LightmapManager";

        private static LightmapManager _findOrCreateLightmapManager(Transform parenTransform = null)
        {
            GameObject go = null;
            if (parenTransform)
            {
                Transform t = parenTransform.Find(_NameDefine);
                if (t) go = t.gameObject;
            }

            if (!go) go = GameObject.Find(_NameDefine);

            if (go)
            {

                if (parenTransform) go.transform.SetParent(parenTransform, false);
                LightmapManager gm = go.GetComponent<LightmapManager>();
                if (gm)
                {
                    return gm;
                }
                else
                {
                    return go.AddComponent<LightmapManager>();
                }
            }
            else
            {
                go = new GameObject(_NameDefine);
                if (parenTransform) go.transform.SetParent(parenTransform, false);
                //                if (Application.isPlaying && _dontDestroyOnLoad && !_parentTransform) GameObject.DontDestroyOnLoad(go);
                return go.AddComponent<LightmapManager>();
            }
        }

        private static LightmapManager _instance;

        public static LightmapManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public static LightmapManager CreateInstance(Transform parenTransform = null)
        {
            if (_instance == null)
            {
                _instance = _findOrCreateLightmapManager(parenTransform);
                _instance.parentTransform = parenTransform;
            }
            else if (parenTransform)
            {
                _instance.transform.SetParent(parenTransform, false);
                _instance.parentTransform = parenTransform;
            }
            return _instance;
        }

        public static void Request(Action LightmapManagerIniteDoSh)
        {
            CreateInstance().AddLightmapManagerInited(LightmapManagerIniteDoSh);
        }

        public static bool IsInit()
        {
            return _instance && _instance._isInit;
        }

        //----------

        protected Transform _parentTransform;
        public Transform parentTransform
        {
            set { _parentTransform = value; }
        }

        protected bool _isSetuped = false;
        protected bool _isInit = false;

        protected Action _AfterInitDo;
        public void AddLightmapManagerInited(Action doSh)
        {
            if (_isInit)
            {
                doSh();
            }
            else
            {
                _AfterInitDo += doSh;
            }
        }

        protected void Awake()
        {
            //单例限制
            if (_instance != null && _instance != this)
            {
                GameObject.Destroy(this);
            }
            else if (_instance == null)
            {
                _instance = this;
                gameObject.name = _NameDefine;
            }
        }

        protected void Start()
        {
            if (_isSetuped && !_isInit)
            {
                init();
            }
        }

        protected void OnDestroy()
        {

            _AfterInitDo = null;
            if (Instance != null && Instance == this)
            {
                _instance = null;
            }
        }

        protected void init()
        {

            if (_isInit) return;
            
            // Todo something

            _isInit = true;
            StartCoroutine(_afterInitRun());
        }

        IEnumerator _afterInitRun()
        {
            yield return new WaitForEndOfFrame();
            if (_AfterInitDo != null)
            {
                Action tmpDo = _AfterInitDo;
                tmpDo();
                _AfterInitDo = null;
            }
        }

        //=====================================================
        

    }
}
