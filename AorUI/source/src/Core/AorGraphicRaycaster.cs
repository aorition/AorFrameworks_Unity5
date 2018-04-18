using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    /// <summary>
    /// 解决 BlockingMask 在运行时不能设值 ....
    /// </summary>
    public class AorGraphicRaycaster : GraphicRaycaster
    {
        public LayerMask BlockingMask
        {
            get { return m_BlockingMask; }
            set { m_BlockingMask = value; }
        }
    }
}
