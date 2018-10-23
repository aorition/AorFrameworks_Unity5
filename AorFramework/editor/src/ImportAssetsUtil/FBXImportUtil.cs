using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{

    /// <summary>
    /// 强制关闭 FBX导入时自动创建材质
    /// 
    /// Todo 是否需要一个全局控制来使其有效或者失效?
    /// 
    /// </summary>
    public class FBXImportUtil : AssetPostprocessor
    {

        //模型导入之前调用  
        public void OnPreprocessModel()
        {
            //Debug.Log("OnPreprocessModel=" + this.assetPath);
            //        ModelImporter mi = (ModelImporter)ModelImporter.GetAtPath(assetPath);
            ModelImporter mi = (ModelImporter)this.assetImporter;
            if (mi)
            {
                mi.importMaterials = false;
            }
        }
        //模型导入之后调用  
        public void OnPostprocessModel(GameObject go)
        {
            //Debug.Log("OnPostprocessModel=" + go.name);
        }

        //纹理导入之前调用，针对入到的纹理进行设置  
        //    public void OnPreprocessTexture()
        //    {
        //        Debug.Log("OnPreProcessTexture=" + this.assetPath);
        //        TextureImporter impor = this.assetImporter as TextureImporter;
        //        impor.textureFormat = TextureImporterFormat.ARGB32;
        //        impor.maxTextureSize = 512;
        //        impor.textureType = TextureImporterType.Advanced;
        //        impor.mipmapEnabled = false;
        //
        //    }
        //    public void OnPostprocessTexture(Texture2D tex)
        //    {
        //        Debug.Log("OnPostProcessTexture=" + this.assetPath);
        //    }
        //    public void OnPostprocessAudio(AudioClip clip)
        //    {
        //
        //    }
        //    public void OnPreprocessAudio()
        //    {
        //        AudioImporter audio = this.assetImporter as AudioImporter;
        //        audio.format = AudioImporterFormat.Compressed;
        //    }
        //所有的资源的导入，删除，移动，都会调用此方法，注意，这个方法是static的  
        //    public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        //    {
        //        Debug.Log("OnPostprocessAllAssets");
        //        foreach (string str in importedAsset)
        //        {
        //            Debug.Log("importedAsset = " + str);
        //        }
        //        foreach (string str in deletedAssets)
        //        {
        //            Debug.Log("deletedAssets = " + str);
        //        }
        //        foreach (string str in movedAssets)
        //        {
        //            Debug.Log("movedAssets = " + str);
        //        }
        //        foreach (string str in movedFromAssetPaths)
        //        {
        //            Debug.Log("movedFromAssetPaths = " + str);
        //        }
        //    }

    }

}


