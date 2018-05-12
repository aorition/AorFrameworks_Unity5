using UnityEngine;

namespace Framework.Graphic
{

    /// <summary>
    /// GraphicsManagerLuncher GraphicsManager启动器
    /// 
    /// 使用GraphicsManager多种方式之一 : 将此脚本挂在任意一个GameObject上即可实现GraphicsManager的启动(带配置).
    /// 
    /// </summary>
    public class GraphicsManagerLuncher : MonoBehaviour
    {
        [SerializeField]
        private bool DonDestroyOnLoad = true;

        [SerializeField]
        private Transform ParentTransformPovit;

        [SerializeField]
        private GraphicsSettingAsset _GraphicsManagerSetting;

        [SerializeField]
        private RectTransform _GMEUIRoot;

        private void Awake()
        {
            GraphicsManager.GetInstance(ParentTransformPovit, DonDestroyOnLoad);
            onManagerBeforeInitialization();

            if (_GMEUIRoot)
            {
                GraphicsManager.instance.Setup(_GraphicsManagerSetting, _GMEUIRoot);
            }
            else
            {
                GraphicsManager.instance.Setup(_GraphicsManagerSetting);
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