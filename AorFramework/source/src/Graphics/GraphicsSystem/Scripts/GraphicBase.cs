using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic
{
    public class GraphicBase : MonoBehaviour
    {

        protected virtual void Awake()
        {
            GraphicsManager.Request(Init);
        }

        protected bool m_isInit;
        public bool isInit {
            get { return m_isInit; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void Init()
        {
            m_isInit = true;
        }

    }
}
