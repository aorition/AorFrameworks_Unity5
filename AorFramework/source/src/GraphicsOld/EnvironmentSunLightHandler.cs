using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using YoukiaUnity.Scene;

namespace YoukiaUnity.Graphics
{
    /// <summary>
    /// 此脚本辅助GraphicsManager 设置SunLight属性
    /// </summary>
    public class EnvironmentSunLightHandler : MonoBehaviour , IEditorOnlyScript
    {

        public EnvironmentSetting environmentSetting;

        public Light light;

        private void Awake()
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

//        private void Start()
//        {
//            if (Application.isPlaying && GraphicsManager.GetInstance() != null)
//            {
//                if (Application.isPlaying)
//                {
//                    Destroy(this);
//                }
//                else
//                {
//                    DestroyImmediate(this);
//                }
//            }
//        }

    }
}
