using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic.FastShadowProjector
{
    public class FastShadowProjectorManagerLuncher : MonoBehaviour
    {

        [SerializeField]
        private bool DonDestroyOnLoad = true;

        [SerializeField]
        private Transform ParentTransformPovit;
        
        private void Awake()
        {
            FastShadowProjectorManager.CreateInstance(ParentTransformPovit, DonDestroyOnLoad);
            onManagerBeforeInitialization();
            FastShadowProjectorManager.Instance.Setup();
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
