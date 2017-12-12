using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using YoukiaUnity.CinemaSystem;
using YoukiaUnity.View;


namespace YoukiaUnity.View
{
    public class DummyEffectHandle : MonoBehaviour
    {
        private bool _isInit;
        private EffectDescript _effectHandler;

        public bool IsInit
        {
            get { return _isInit; }
        }

        public bool IsDispose;

        public void Init(EffectDescript effectHandler )
        {
            _effectHandler = effectHandler;
            _effectHandler.gameObject.SetActive(true);
            _isInit = true;

        }

        public void Init()
        {
            _isInit = true;
        }

        protected void Update()
        {
            if (IsInit && (transform.childCount == 0 || IsDispose))
            {
                Dispose();
            }
        }

        protected void Dispose()
        {
            if (_effectHandler != null && transform.childCount != 0)
            {
                EffectUtil.EffectDispose(_effectHandler);
            }
            Destroy(gameObject);
        }
    }
}
