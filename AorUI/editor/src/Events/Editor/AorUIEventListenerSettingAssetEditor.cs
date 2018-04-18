using System;
using System.Collections.Generic;
using Framework.editor;
using UnityEditor;
using UnityEngine;

namespace Framework.UI.Editor
{

    [CustomEditor(typeof(AorUIEventListenerSettingAsset))]
    public class AorUIEventListenerSettingAssetEditor : UnityEditor.Editor
    {

        [MenuItem("Assets/UIManagerAssets/UIEventListenerSettingAsset")]
        public static void CreateAsset()
        {
            EditorPlusMethods.CreateAssetFile<AorUIEventListenerSettingAsset>("unameUIEventListenerSettingAsset");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            #region *** 按钮-> <立即写入修改数据到文件> :: 建议所有.Asset文件的Editor都配备此段代码
            EditorPlusMethods.Draw_AssetFileApplyImmediateUI(target);
            #endregion

        }
    }
}
