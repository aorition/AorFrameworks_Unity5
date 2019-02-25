using System.Collections;
using System.Collections.Generic;
using Framework.Editor;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoleShadowSettingAsset))]
public class RoleShadowSettingAssetEditor : Editor {

    [MenuItem("Assets/GraphicsManagerAssets/RoleShadowSettingAsset")]
    public static void CreateAsset()
    {

        RoleShadowSettingAsset info = ScriptableObject.CreateInstance<RoleShadowSettingAsset>();
        string dir;
        string name = "unameRoleShadowSettingAsset.asset";
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

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        #region *** 按钮-> <立即写入修改数据到文件> :: 建议所有.Asset文件的Editor都配备此段代码

        GUILayout.Space(13);
        GUI.color = Color.yellow;
        if (GUILayout.Button(new GUIContent("Save Data To Asset Immediate", "立即将修改的数据保存到文件"), GUILayout.Height(26)))
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
        GUI.color = Color.white;

        #endregion
    }
}
