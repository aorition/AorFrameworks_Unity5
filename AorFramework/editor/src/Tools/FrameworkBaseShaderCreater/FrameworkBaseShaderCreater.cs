using System;
using System.Collections.Generic;
using AorBaseUtility;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor.tools
{
    
    public class FrameworkBaseShaderCreater
    {

        public static void BuildingShaderFile(IShaderDefine shaderDefine, bool existWarning = true)
        {

            //检查Shader是否已经存在
            Shader shader = Shader.Find(shaderDefine.ShaderLabel);
            if (shader)
            {
                if(existWarning)
                    Log.Warning("* FrameworkBaseShaderCreater.BuildingShaderFile Warning :: " + shaderDefine.ShaderName + " < " + shaderDefine.ShaderLabel + " > has already exist.");
                return;
            }

            //文件夹创建验证
            string[] folders = shaderDefine.SavePath.Split('/');
            string savePath = string.Empty;
            int i, len = folders.Length;
            for (i = 0; i < len; i++)
            {
                if (i == 0 && folders[i].ToLower() == "assets")
                {
                    savePath = folders[i];
                    continue;
                }

                if (!AssetDatabase.IsValidFolder(savePath + "/" + folders[i]))
                {
                    AssetDatabase.CreateFolder(savePath, folders[i]);
                }

                if (i > 0)
                {
                    savePath += "/" + folders[i];
                }
            }

            string fullSavePath = Application.dataPath.Replace("Assets","") + savePath + "/" + shaderDefine.ShaderName + ".shader";
            if (AorIO.SaveStringToFile(fullSavePath, shaderDefine.ShdaerCode))
            {

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                AssetImporter importer = AssetImporter.GetAtPath(fullSavePath);
                if (importer)
                {
                    importer.SaveAndReimport();
                }

            }
            else
            {
                //Error for fail to wirte file
            }

        }
    }
}
