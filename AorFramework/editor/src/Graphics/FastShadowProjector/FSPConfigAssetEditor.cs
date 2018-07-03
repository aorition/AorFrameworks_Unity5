using System.Collections;
using System.Collections.Generic;
using Framework.Editor;
using UnityEditor;

namespace Framework.Graphic.FastShadowProjector.Editor
{
    public class FSPConfigAssetEditor : UnityEditor.Editor
    {

        [MenuItem("Assets/FastShadowProjector/ConfigAsset")]
        public static void CreateAsset()
        {
            EditorPlusMethods.CreateAssetFile<FSPConfigAsset>("unameFastShadowProjectorSettingAsset");
        }

    }

}


