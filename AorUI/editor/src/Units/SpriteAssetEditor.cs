using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Framework.Editor;

namespace UnityEngine.UI.Editor
{

    [CustomEditor(typeof(SpriteAsset))]
    public class SpriteAssetEditor : UnityEditor.Editor
    {

        private static List<SpriteAssetInfo> GetSpritesInfo(Texture2D tex)
        {
            List<SpriteAssetInfo> m_sprites = new List<SpriteAssetInfo>();

            string filePath = UnityEditor.AssetDatabase.GetAssetPath(tex);

            Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(filePath);

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].GetType() == typeof(Sprite))
                {
                    SpriteAssetInfo temp = new SpriteAssetInfo();
                    Sprite sprite = objects[i] as Sprite;
                    temp.ID = i;
                    temp.name = sprite.name;
                    temp.pivot = new Vector2(.5f, .5f);//sprite.pivot;
                    temp.rect = sprite.rect;
                    temp.sprite = sprite;
                    m_sprites.Add(temp);
                }
            }
            return m_sprites;
        }

        [MenuItem("Assets/Create/UGUI Sprite Asset", false, 10)]
        private static void create()
        {
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
            if (isNewAsset)
            {
                spriteAsset = ScriptableObject.CreateInstance<SpriteAsset>();
                spriteAsset.texSource = sourceTex;
                spriteAsset.listSpriteInfo = GetSpritesInfo(sourceTex);
                AssetDatabase.CreateAsset(spriteAsset, filePath + fileNameWithoutExtension + ".asset");
            }
        }

        //--------------------------------------------------------

        SpriteAsset spriteAsset;

        public void OnEnable()
        {
            spriteAsset = (SpriteAsset)target;
        }
        private Vector2 ve2ScorllView;
        public override void OnInspectorGUI()
        {
            ve2ScorllView = GUILayout.BeginScrollView(ve2ScorllView);
            GUILayout.Label("UGUI Sprite Asset");
            if (spriteAsset.listSpriteInfo == null)
                return;
            for (int i = 0; i < spriteAsset.listSpriteInfo.Count; i++)
            {
                GUILayout.Label("\n");
                //            EditorGUILayout.ObjectField("", spriteAsset.listSpriteInfo[i].sprite, typeof(Sprite));
                EditorGUILayout.ObjectField("", spriteAsset.listSpriteInfo[i].sprite, typeof(Sprite), false);
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

}
