using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
//using YoukiaUnity.App;
using YoukiaUnity.Graphics;
using YoukiaUnity.Graphics.FastShadowProjector;

namespace YoukiaUnity.Scene
{
    /// <summary>
    /// 场景天空设置
    /// </summary>
    public class EnvironmentSetting : GraphicsLauncher
    {

        [Serializable]
        public struct LightmapTexs
        {
            public LightmapTexs(Texture2D dir, Texture2D light)
            {
                this.Dir = dir;
                this.Light = light;
            }

            public Texture2D Dir;
            public Texture2D Light;
        }

        private Light _sunLight;
        public Light SunLight
        {
            get
            {
                if (GraphicsManager.GetInstance() != null && GraphicsManager.GetInstance().SunLight != null)
                {
                    return GraphicsManager.GetInstance().SunLight.light;
                }
                return _sunLight;
            }
            set { _sunLight = value; }
        }

        public LightmapTexs[] lightmapTexs;
        public LightProbes LightProbes;
        public string LightmapsMode;

        public Color AmbientColor;
        public Vector3 LightDir = new Vector3(45, 45, 0);
        public Color LightColor = Color.white;
        public float LightIntensity = 1;


        public Material RenderMaterial;

        /// <summary>
        /// 体积雾密度
        /// </summary>
        public float VolumeFogDestiy = 10;

        /// <summary>
        /// 体积雾偏移
        /// </summary>
        public float VolumeFogOffset = 10;


        public bool FogEnable = true;

        public Color FogColor = Color.gray;

        /// <summary>
        /// 雾效模式
        /// </summary>
        public FogMode FogMode = FogMode.Linear;

        /// <summary>
        /// 雾效密度
        /// </summary>
        public float FogDensity = 0.01f;

        /// <summary>
        /// 雾效开始值
        /// </summary>
        public float FogStart = 100;
        /// <summary>
        /// 雾效结束值
        /// </summary>
        public float FogEnd = 500;
        

        public float ZClipFar = 130;
        public float ShaodwIntensity = 0.5f;
        private float _ShaodwIntensity = 0f;
        //  public Transform Directionallight;
        [HideInInspector]
        public bool FixedProjection = false;

        public Material skyMat;
 
        private float _VolumeFogDestiy = 1;
        private float _VolumeFogOffset = 1;

        protected override void Launcher()
        {
            base.Launcher();
 
            StartAfterGraphicMgr(this, () =>
            {

                getRenderMaterial();
                gameObject.layer = LayerMask.NameToLayer("sky");
                
                EnvironmentUpdate(true);


            });
        }

        protected void OnEnable()
        {
            if (lightmapTexs != null)
            {
                int i, len = lightmapTexs.Length;
                LightmapData[] datas = new LightmapData[lightmapTexs.Length];
                for (i = 0; i < len; i++)
                {
                    LightmapData ld = new LightmapData();
                    ld.lightmapDir = lightmapTexs[i].Dir;
                    ld.lightmapColor = lightmapTexs[i].Light;
                    datas[i] = ld;
                }
                LightmapSettings.lightmaps = datas;
            }
            if (LightProbes != null)
            {
                LightmapSettings.lightProbes = LightProbes;
            }
            if (!string.IsNullOrEmpty(LightmapsMode))
            {
                //LightmapSettings.lightmapsMode = (LightmapsMode)Enum.Parse(typeof(LightmapsMode), LightmapsMode);
                // 通过上面的方法手机转换不出来,先改成下面的吧
                switch (LightmapsMode)
                {
                    case "Single":
                    case "NonDirectional":
                        LightmapSettings.lightmapsMode = (LightmapsMode)Enum.Parse(typeof(LightmapsMode), "NonDirectional");
                        break;
                    case "Dual":
                    case "CombinedDirectional":
                        LightmapSettings.lightmapsMode = (LightmapsMode)Enum.Parse(typeof(LightmapsMode), "CombinedDirectional");
                        break;
                    case "Directional":
                    case "SeparateDirectional":
                        LightmapSettings.lightmapsMode = (LightmapsMode)Enum.Parse(typeof(LightmapsMode), "SeparateDirectional");
                        break;
                    default :
                        break;
                }
            }
        }

        public void SetLightValues(Color color, float intensity, Vector3 dir)
        {
            LightDir = dir;
            LightColor = color;
            LightIntensity = intensity;
        }

        /// <summary>
        /// 设置天空的叠加颜色
        /// </summary>
        /// <param name="color">颜色</param>
        public void SetSkyColor(Color color)
        {
            skyMat.SetColor("_Color", color);
        }
        /// <summary>
        /// 设置天空的曝光程度
        /// </summary>
        /// <param name="expo">曝光度</param>
        public void SetSkyExposure(float expo)
        {
            skyMat.SetFloat("_Lighting", expo);
        }

        void getRenderMaterial()
        {
            if (GraphicsManager.GetInstance() != null && GraphicsManager.GetInstance().DrawCard != null)
                RenderMaterial = GraphicsManager.GetInstance().RenderMaterial;

        }

        public virtual void EnvironmentUpdate(bool force)
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = AmbientColor;

            RenderSettings.fog = FogEnable;
            RenderSettings.fogColor = FogColor;
            RenderSettings.fogMode = FogMode;
            RenderSettings.fogDensity = FogDensity;
            RenderSettings.fogStartDistance = FogStart;
            RenderSettings.fogEndDistance = FogEnd;
            
            GraphicsManager.GetInstance().SunLight.light.intensity = LightIntensity;
            GraphicsManager.GetInstance().SunLight.light.color = LightColor;
            GraphicsManager.GetInstance().SunLight.transform.eulerAngles = LightDir;

            GlobalProjectorManager fspMgr = GraphicsManager.GetInstance().ShadowMgr;
            fspMgr.GlobalProjectionDir = GraphicsManager.GetInstance().SunLight.transform.eulerAngles;

            if (RenderMaterial == null)
            {
                getRenderMaterial();
            }


            if (_VolumeFogDestiy != VolumeFogDestiy || _VolumeFogOffset != VolumeFogOffset || force)
            {
                _VolumeFogDestiy = VolumeFogDestiy;
                _VolumeFogOffset = VolumeFogOffset;
                GraphicsManager.GetInstance().VolumeFogDestiy = _VolumeFogDestiy;
                GraphicsManager.GetInstance().VolumeFogOffset = _VolumeFogOffset;

            }


            if (fspMgr.ZClipFar != ZClipFar || force)
            {
                fspMgr.ZClipFar = ZClipFar;
            }
            if (fspMgr.FixedProjection != FixedProjection || force)
            {
                fspMgr.FixedProjection = FixedProjection;
            }

            if (_ShaodwIntensity != ShaodwIntensity || force)
            {
                _ShaodwIntensity = ShaodwIntensity;

                if (fspMgr.AlphaMode)
                    fspMgr.PreShadowMaterial.SetFloat("_Intensity", _ShaodwIntensity);
                else
                    fspMgr.PreShadowMaterial.SetFloat("_Intensity", 1 - _ShaodwIntensity);
            }
        }

    }
}

