using System.Runtime.Remoting.Lifetime;
using UnityEngine;
using System.Collections;
//using YoukiaUnity.App;


namespace YoukiaUnity.Graphics
{
    /// <summary>
    /// 灯光信息
    /// </summary>
    [ExecuteInEditMode]
    public class LightInfo : MonoBehaviour {
        /// <summary>
        /// 是否使用强度曲线
        /// </summary>
        public bool UseIntensityCurve = false;
        /// <summary>
        /// 强度曲线
        /// </summary>
        public AnimationCurve IntensityCurve;
        private float LifeTime;


        private Light _light;

        public Light light
        {
            get
            {
                if (!_light)
                {
                    _light = GetComponent<Light>();
                }
                return _light;
            }
        }

        void OnEnable()
        {
//            base.OnEnable();

            _light = GetComponent<Light>();

            LightManager.Instance.AddLight(this);
            LifeTime = 0;
        }

        void OnDisable()
        {
//            base.OnDisable();

            if (LightManager.isExist)
                LightManager.Instance.RemoveLight(this);
        }

 
        private void Update()
        {
            if (Application.isPlaying && UseIntensityCurve)
            {
                LifeTime += Time.deltaTime;
                light.intensity = IntensityCurve.Evaluate(LifeTime);

                if (LifeTime > IntensityCurve.length && (IntensityCurve.postWrapMode == WrapMode.Clamp || IntensityCurve.postWrapMode == WrapMode.Default || IntensityCurve.postWrapMode == WrapMode.Once))
                {
                    if (light != null)
                        light.intensity = 0;
                }
            }



        }

    }

}


