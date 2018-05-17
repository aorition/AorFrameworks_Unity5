using UnityEngine;

namespace Framework.Graphic
{
    /// <summary>
    /// 灯光信息
    /// </summary>
    [ExecuteInEditMode]
    public class CustomLightInfo : MonoBehaviour {

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

        private void OnEnable()
        {
            _light = GetComponent<Light>();
            CustomLightManager.Instance.AddLight(this);
        }

        private void OnDisable()
        {
            if (CustomLightManager.isExist)
                CustomLightManager.Instance.RemoveLight(this);
        }

    }

}