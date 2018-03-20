using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic
{
    public class GraphicBase : MonoBehaviour
    {

        protected virtual void Awake()
        {
            GraphicsManager.RequestGraphicsManager(Luncher);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void Luncher()
        {

        }

    }
}
