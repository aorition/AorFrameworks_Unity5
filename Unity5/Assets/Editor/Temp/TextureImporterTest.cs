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

    [MenuItem("Test/TestDo2")]
    public static void TestDo2()
    {

        UnityEngine.Object obj = Selection.activeObject;
        if (obj == null) return;

        string path = AssetDatabase.GetAssetPath(obj);

        AssetImporter ai = AssetImporter.GetAtPath(path);
        ai.SetAssetBundleNameAndVariant("test3",null);

    }

    //打包场景
    public static void TestDo3()
    {
        // 需要打包的场景名字
        string[] levels = { "Assets/2.unity" };
        // 注意这里【区别】通常我们打包，第2个参数都是指定文件夹目录，在此方法中，此参数表示具体【打包后文件的名字】
        // 记得指定目标平台，不同平台的打包文件是不可以通用的。最后的BuildOptions要选择流格式
        BuildPipeline.BuildPlayer(levels, Application.dataPath + "/Scene.unity3d", BuildTarget.Android, BuildOptions.BuildAdditionalStreamedScenes);
        // 刷新，可以直接在Unity工程中看见打包后的文件
        AssetDatabase.Refresh();
    }
}
