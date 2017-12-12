using System;
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
        public Color AmbientColor;
        public Vector3 LightDir = new Vector3(45, 45, 0);
        public Color LightColor = Color.white;
        public float LightIntensity = 1;


        public Material RenderMaterial;


        /// <summary>
        /// 大气雾距离
        /// </summary>
        public float FogDestance = 20;

        /// <summary>
        /// 大气雾密度
        /// </summary>
        public float FogDestiy = 60;




        /// <summary>
        /// 体积雾密度
        /// </summary>
        public float VolumeFogDestiy = 10;

        /// <summary>
        /// 体积雾偏移
        /// </summary>
        public float VolumeFogOffset = 10;


        public float ZClipFar = 130;
        public float ShaodwIntensity = 0.5f;
        private float _ShaodwIntensity = 0f;
        //  public Transform Directionallight;
        [HideInInspector]
        public bool FixedProjection = false;



        private Material skyMat;
        private float _FogDestance = 1;
        private float _FogDestiy = 1;
        private float _VolumeFogDestiy = 1;
        private float _VolumeFogOffset = 1;



        void OnEnable()
        {


            StartAfterGraphicMgr(this, () =>
            {

                getRenderMaterial();
                gameObject.layer = LayerMask.NameToLayer("sky");



                EnvironmentUpdate(true);


            });




        }


        protected override void Launcher()
        {
            base.Launcher();

            skyMat = GetComponent<Renderer>().material;


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

            GraphicsManager.GetInstance().SunLight.light.intensity = LightIntensity;
            GraphicsManager.GetInstance().SunLight.light.color = LightColor;
            GraphicsManager.GetInstance().SunLight.transform.eulerAngles = LightDir;
            GlobalProjectorManager fspMgr = GraphicsManager.GetInstance().ShadowMgr;
            fspMgr.GlobalProjectionDir = GraphicsManager.GetInstance().SunLight.transform.eulerAngles;




            if (RenderMaterial == null)
            {
                getRenderMaterial();
            }


            if (_FogDestance != FogDestance || _FogDestiy != FogDestiy || force)
            {
                _FogDestance = FogDestance;
                _FogDestiy = FogDestiy;
                GraphicsManager.GetInstance().FogDestance = _FogDestance;
                GraphicsManager.GetInstance().FogDestiy = _FogDestiy;

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

