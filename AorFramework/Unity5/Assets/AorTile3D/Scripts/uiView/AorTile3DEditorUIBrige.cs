using System;
using System.Collections.Generic;
using AorFramework.AorTile3D.runtimeEditor;
using UnityEngine;

namespace Assets.AorTile3D.Scripts.runtimeEditor
{
    public class AorTile3DEditorUIBrige : MonoBehaviour
    {

        /// <summary>
        /// Btn_隐藏Nmap
        /// </summary>
        public void HideTheNMap()
        {
            AorTile3DRuntimeEditor.Instance.SetNCameraActive(false);
        }

    }
}
