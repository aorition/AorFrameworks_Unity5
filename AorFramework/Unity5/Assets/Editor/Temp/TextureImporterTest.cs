using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TextureImporterTest {

    [MenuItem("Test/TestDo")]
    public static void TestDo()
    {

        UnityEngine.Object obj = Selection.activeObject;
        if (obj == null) return;

        string path = AssetDatabase.GetAssetPath(obj);


        TextureImporter textureImporter = (TextureImporter) TextureImporter.GetAtPath(path);
        if (textureImporter)
        {

            TextureImporterPlatformSettings defaultPlatformSettings = textureImporter.GetDefaultPlatformTextureSettings();
            if (defaultPlatformSettings != null)
            {
                Debug.Log("******************* default");
            }

            TextureImporterPlatformSettings platformSettings = textureImporter.GetPlatformTextureSettings(BuildTarget.Android.ToString());
            if (platformSettings != null)
            {
                Debug.Log("******************* Android");
            }

            TextureImporterPlatformSettings platformSettings2 = textureImporter.GetPlatformTextureSettings(BuildTarget.PSP2.ToString());
            if (platformSettings2 != null)
            {
                Debug.Log("******************* PSP2");
            }

        }

    }

}
