using UnityEngine;
using UnityEditor;
using System.Collections;
using Framework.editor;

[CustomEditor(typeof(SpriteAsset))]
public class UGUISpriteAssetEditor : Editor {

    SpriteAsset spriteAsset;

    public void OnEnable() {
        spriteAsset = (SpriteAsset)target;
    }
    private Vector2 ve2ScorllView;
    public override void OnInspectorGUI() {
        ve2ScorllView = GUILayout.BeginScrollView(ve2ScorllView);
        GUILayout.Label("UGUI Sprite Asset");
        if (spriteAsset.listSpriteInfo == null)
            return;
        for (int i = 0; i < spriteAsset.listSpriteInfo.Count; i++) {
            GUILayout.Label("\n");
//            EditorGUILayout.ObjectField("", spriteAsset.listSpriteInfo[i].sprite, typeof(Sprite));
            EditorGUILayout.ObjectField("", spriteAsset.listSpriteInfo[i].sprite, typeof (Sprite), false);
            EditorGUILayout.IntField("ID:", spriteAsset.listSpriteInfo[i].ID);
            EditorGUILayout.LabelField("name:", spriteAsset.listSpriteInfo[i].name);
            EditorGUILayout.Vector2Field("povit:", spriteAsset.listSpriteInfo[i].pivot);
            EditorGUILayout.RectField("rect:", spriteAsset.listSpriteInfo[i].rect);
            GUILayout.Label("\n");
        }
        GUILayout.EndScrollView();

        #region *** 按钮-> <立即写入修改数据到文件> :: 建议所有.Asset文件的Editor都配备此段代码
        EditorPlusMethods.Draw_AssetFileApplyImmediateUI(target);
        #endregion

    }

}