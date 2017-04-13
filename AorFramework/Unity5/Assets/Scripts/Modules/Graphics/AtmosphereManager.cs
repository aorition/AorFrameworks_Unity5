using UnityEngine;
using System.Collections;

namespace YoukiaUnity.Graphics
{

    /// <summary>
    /// 大气雾系统
    /// </summary>
    public class AtmosphereManager : MonoBehaviour
    {
        /// <summary>
        /// 雾距离
        /// </summary>
        public float FogDestance = 20;
        /// <summary>
        /// 雾密度
        /// </summary>
        public float FogDestiy = 15;


        /// <summary>
        /// 体积雾偏移
        /// </summary>
        public float VolumeFogOffset = 10;

        /// <summary>
        /// 体积雾密度
        /// </summary>
        public float VolumeFogDestiy = 10;



        /// <summary>
        /// 环境色
        /// </summary>
        public Color AmbientColor;


        private float _fogDestance;
        private float _fogDestiy;

        private float _VolumeFogOffset;
        private float _VolumeFogDestiy;


        private Color _AmbientColor;
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {


            _fogDestance = FogDestance;
            _fogDestiy = FogDestiy;
            _AmbientColor = AmbientColor;
            Shader.SetGlobalFloat("_fogDestance", -_fogDestance);
            Shader.SetGlobalFloat("_fogDestiy", 100 - _fogDestiy);
            Shader.SetGlobalVector("_AmbientColor", _AmbientColor);
        }



        void Update()
        {
            if (_fogDestance != FogDestance)
            {
                _fogDestance = FogDestance;
                Shader.SetGlobalFloat("_fogDestance", -_fogDestance);
            }

            if (_fogDestiy != FogDestiy)
            {
                _fogDestiy = FogDestiy;

                Shader.SetGlobalFloat("_fogDestiy", 100 - _fogDestiy);
            }

            if (_AmbientColor != RenderSettings.ambientLight)
            {

                _AmbientColor = RenderSettings.ambientLight;
                Shader.SetGlobalVector("_AmbientColor", _AmbientColor);
            }

            if (_VolumeFogOffset != VolumeFogOffset)
            {

                _VolumeFogOffset = VolumeFogOffset;

                Shader.SetGlobalFloat("_volumeFogOffset", VolumeFogOffset);
            }


            if (_VolumeFogDestiy != VolumeFogDestiy)
            {
                _VolumeFogDestiy = VolumeFogDestiy;
                Shader.SetGlobalFloat("_volumeFogDestiy", VolumeFogDestiy);
            }
        }

    }
}


