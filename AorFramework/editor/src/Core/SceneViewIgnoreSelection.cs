using UnityEngine;
using UnityEditor;


namespace Framework.Editor
{

    /// <summary>
    /// 框架预置功能: 使层级在 Ignore Raycast的对象在 SceneView 里面不能被选择（通过Hierarchy选择的不受限制）
    /// </summary>
    [InitializeOnLoad]
    public class SceneViewIgnoreSelection : AssetPostprocessor
    {

        static SceneViewIgnoreSelection()
        {
            __setIgnoreRaycastLayerLocked();
            Debug.Log("<SceneViewIgnoreSelection> __ set IgnoreRaycast Layer Locked done.");
        }

        public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            __setIgnoreRaycastLayerLocked();
        }

        private static void __setIgnoreRaycastLayerLocked() {
            Tools.lockedLayers |= 1 << LayerMask.NameToLayer("Ignore Raycast");
        }

    }
}