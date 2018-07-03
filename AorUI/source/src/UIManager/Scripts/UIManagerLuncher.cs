using UnityEngine;

namespace Framework.UI
{

    /// <summary>
    /// AorUIManagerLuncher AorUIManager启动器
    /// 
    /// 使用UIManagerLuncher多种方式之一 : 将此脚本挂在任意一个GameObject上即可实现AorUIManager的启动(带配置).
    /// 
    /// </summary>
    public class UIManagerLuncher : MonoBehaviour
    {
        [SerializeField]
        private bool DonDestroyOnLoad = true;

        [SerializeField]
        private Transform ParentTransformPovit;

        [SerializeField]
        private UIManagerSettingAsset _UIManagerSetting;

        private void Awake()
        {
//            GraphicsManager.GetInstance(ParentTransformPovit, DonDestroyOnLoad);
            UIManager manager = UIManager.CreateInstance(ParentTransformPovit);
            onUIManagerInstanced();

            manager.setup(_UIManagerSetting);

            if (DonDestroyOnLoad)
            {
                GameObject.DontDestroyOnLoad(manager.gameObject);
            }

            onUIManagerSetuped();
            GameObject.Destroy(this);
        }

        /// <summary>
        /// GraphicsManager在GetInstance并Setup之前调用此方法.
        /// </summary>
        protected virtual void onUIManagerInstanced()
        {
            //请继承此类,并复写此函数对AorUIManager.GetInstence并在执行Setup之前进行其他初始化内容
        }

        /// <summary>
        /// GraphicsManager在GetInstance并Setup之后调用此方法.
        /// </summary>
        protected virtual void onUIManagerSetuped()
        {
            //请继承此类,并复写此函数对AorUIManager.Setup后进行其他初始化内容
        }

    }

}