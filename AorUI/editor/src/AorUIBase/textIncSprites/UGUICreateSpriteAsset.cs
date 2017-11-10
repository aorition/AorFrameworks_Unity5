using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public static class UGUICreateSpriteAsset {
    [MenuItem("Assets/Create/UGUI Sprite Asset", false, 10)]
    static void main() {
        Object target = Selection.activeObject;
        if (target == null || target.GetType() != typeof(Texture2D))
            return;

        Texture2D sourceTex = target as Texture2D;
        //整体路径
        string filePathWithName = AssetDatabase.GetAssetPath(sourceTex);
        //带后缀的文件名
        string fileNameWithExtension = Path.GetFileName(filePathWithName);
        //不带后缀的文件名
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePathWithName);
        //不带文件名的路径
        string filePath = filePathWithName.Replace(fileNameWithExtension, "");

        SpriteAsset spriteAsset = AssetDatabase.LoadAssetAtPath(filePath + fileNameWithoutExtension + ".asset", typeof(SpriteAsset)) as SpriteAsset;
        bool isNewAsset = spriteAsset == null ? true : false;
        if (isNewAsset) {
            spriteAsset = ScriptableObject.CreateInstance<SpriteAsset>();
            spriteAsset.texSource = sourceTex;
            spriteAsset.listSpriteInfo = GetSpritesInfo(sourceTex);
            AssetDatabase.CreateAsset(spriteAsset, filePath + fileNameWithoutExtension + ".asset");
        }
    }

    public static List<SpriteAssetInfo> GetSpritesInfo(Texture2D tex) {
        List<SpriteAssetInfo> m_sprites = new List<SpriteAssetInfo>();

        string filePath = UnityEditor.AssetDatabase.GetAssetPath(tex);

        Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(filePath);

        for (int i = 0; i < objects.Length; i++) {
            if (objects[i].GetType() == typeof(Sprite)) {
                SpriteAssetInfo temp = new SpriteAssetInfo();
                Sprite sprite = objects[i] as Sprite;
                temp.ID = i;
                temp.name = sprite.name;
                temp.pivot = new Vector2(.5f,.5f);//sprite.pivot;
                temp.rect = sprite.rect;
                temp.sprite = sprite;
                m_sprites.Add(temp);
            }
        }
        return m_sprites;
    }

}