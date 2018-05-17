using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic.FastShadowProjector
{
    /// <summary>
    /// GlobalProjectorManagerLuncher GlobalProjectorManager启动器
    /// 
    /// 使用GlobalProjectorManager多种方式之一 : 将此脚本挂在任意一个GameObject上即可实现GlobalProjectorManager的启动(带配置).
    /// 
    /// </summary>
    public class GlobalProjectorManagerLuncher : MonoBehaviour
    {

        [SerializeField]
        private Transform ParentTransformPovit;

        [SerializeField]
        private FSPConfigAsset _SettingAsset;

        [SerializeField]
        private Camera _viewCamera;

        private void Awake()
        {
            GlobalProjectorManager.CreateInstance(ParentTransformPovit);
            onManagerBeforeInitialization();

            if (_viewCamera)
            {
                GlobalProjectorManager.Instance.Setup(_viewCamera, _SettingAsset);
            }
            else
            {
                GlobalProjectorManager.Instance.Setup(_SettingAsset);
            }
            onManagerAfterInitialization();
            GameObject.Destroy(this);
        }

        /// <summary>
        /// Manager初始化之前调用此方法.
        /// </summary>
        protected virtual void onManagerBeforeInitialization()
        {
            //
        }

        /// <summary>
        ///  Manager初始化之后调用此方法.
        /// </summary>
        protected virtual void onManagerAfterInitialization()
        {
            //
        }

    }
}
