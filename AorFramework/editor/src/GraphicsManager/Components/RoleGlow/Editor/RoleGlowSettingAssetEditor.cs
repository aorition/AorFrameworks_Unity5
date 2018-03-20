using System.Collections;
using System.Collections.Generic;
using AorFramework.editor;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoleGlowSettingAsset))]
public class RoleGlowSettingAssetEditor : Editor {

    [MenuItem("Assets/GraphicsManagerAssets/RoleGlowSettingAsset")]
    public static void CreateAsset()
    {

        RoleGlowSettingAsset info = ScriptableObject.CreateInstance<RoleGlowSettingAsset>();
        string dir;
        string name = "unameRoleGlowSettingAsset.asset";
        if (Selection.objects == null || Selection.objects.Length == 0)
        {
            dir = "Assets";
        }
        else
        {
            
            string dataPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
            if (Selection.objects[0] is DefaultAsset)
            {
                dir = dataPath;
            }
            else
            {
                EditorAssetInfo ainfo = new EditorAssetInfo(dataPath);
                dir = ainfo.dirPath;
            }
        }
        string path = dir + "/" + name;
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        AssetDatabase.CreateAsset(info, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
