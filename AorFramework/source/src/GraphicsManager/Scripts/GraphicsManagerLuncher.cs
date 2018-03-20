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
            onGraphicsManagerInstanced();

            if (_GMEUIRoot)
            {
                GraphicsManager.instance.setup(_GraphicsManagerSetting, _GMEUIRoot);
            }
            else
            {
                GraphicsManager.instance.setup(_GraphicsManagerSetting);
            }

            onGraphicsManagerSetuped();
            GameObject.Destroy(this);
        }

        /// <summary>
        /// GraphicsManager在GetInstance并Setup之前调用此方法.
        /// </summary>
        protected virtual void onGraphicsManagerInstanced()
        {
            //请继承此类,并复写此函数对GraphicsManager.GetInstence并在执行Setup之前进行其他初始化内容
        }

        /// <summary>
        /// GraphicsManager在GetInstance并Setup之后调用此方法.
        /// </summary>
        protected virtual void onGraphicsManagerSetuped()
        {
            //请继承此类,并复写此函数对GraphicsManager.Setup后进行其他初始化内容
        }

    }

}