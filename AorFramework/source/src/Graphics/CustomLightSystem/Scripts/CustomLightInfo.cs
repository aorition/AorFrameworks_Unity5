#pragma warning disable
using UnityEngine;

namespace Framework.Graphic.CustomLight
{
    /// <summary>
    /// 自定义灯光信息
    /// </summary>
    [ExecuteInEditMode]
    public class CustomLightInfo : MonoBehaviour
    {

        public static void Convert2UnityLight(CustomLightInfo customLightInfo) {

            customLightInfo.enabled = false;

            Light light = customLightInfo.gameObject.GetComponent<Light>();
            if(!light) light = customLightInfo.gameObject.AddComponent<Light>();

            light.type = customLightInfo.lightType;
            light.color = customLightInfo.lightColor;
            light.intensity = customLightInfo.intensity;
            light.range = customLightInfo.range;

            if (Application.isPlaying)
                GameObject.Destroy(customLightInfo);
            else
                GameObject.DestroyImmediate(customLightInfo);

        }

        public LightType lightType = LightType.Directional;

        public Color lightColor = Color.white;

        public float intensity = 1.0f;

        public float range = 1.0f;

        private void OnEnable()
        {
            Light unitylight = GetComponent<Light>();
            if (unitylight && unitylight.enabled)
            {
                unitylight.enabled = false;
                lightType = unitylight.type;
                lightColor = unitylight.color;
                intensity = unitylight.intensity;
                range = unitylight.range;
                if (Application.isPlaying)
                    GameObject.Destroy(unitylight);
                else
                    GameObject.DestroyImmediate(unitylight);
            }

            CustomLightManager.Instance.AddLight(this);
        }

        private void OnDisable()
        {
            if (CustomLightManager.isExist)
                CustomLightManager.Instance.RemoveLight(this);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Vector3 s = transform.position;
            Vector3 t = s + transform.forward * range;
            Gizmos.DrawWireSphere(s, 0.1f);
            Gizmos.DrawLine(s, t);
            if (lightType == LightType.Point)
                Gizmos.DrawWireSphere(s, range);
        }
        
    }

}