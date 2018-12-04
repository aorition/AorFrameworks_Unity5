using UnityEngine;
using System.Collections;
using UnityEditor;
using Framework.Editor;
using System.IO;
using Object = UnityEngine.Object;


public class ArtistFont
{
    public static void BatchCreateArtistFont()
    {
        if (Selection.objects == null || Selection.objects.Length == 0)
        {
            EditorUtility.DisplayDialog("警告", "未选中!!!", "关闭");
            return;
        }
        Object obj = Selection.objects[0];
        string assetPath = AssetDatabase.GetAssetPath(obj);

        //string fntFileName = ResourceCommon.GetFileName(assetPath, false);
        EditorAssetInfo fntInfo = new EditorAssetInfo(assetPath);


        //if (!fntFileName.EndsWith(".fnt"))
        if (fntInfo.suffix != ".fnt")
        {
            EditorUtility.DisplayDialog("警告", "请选择后缀为.fnt的配置文件!!!", "关闭");
            return;
        }




        string fontPath = fntInfo.dirPath + "/" + fntInfo.name + "_font.fontsettings";
        Font CustomFont = AssetDatabase.LoadAssetAtPath(fontPath, typeof(Font)) as Font;
        if (CustomFont == null)
        {
            CustomFont = new Font();

          //  AssetDatabase.SaveAssets();
           // AssetDatabase.ImportAsset(fontPath);
        }

        TextAsset BMFontText = null;
        {
            BMFontText = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
        }
        BMFont mbFont = new BMFont();
        BMFontReader.Load(mbFont, BMFontText.name, BMFontText.bytes);  // 借用NGUI封装的读取类
        CharacterInfo[] characterInfo = new CharacterInfo[mbFont.glyphs.Count];
        for (int i = 0; i < mbFont.glyphs.Count; i++)
        {
            BMGlyph bmInfo = mbFont.glyphs[i];
            CharacterInfo info = new CharacterInfo();
            info.index = bmInfo.index;
#if UNITY_4
            info.uv.x = (float)bmInfo.x / (float)mbFont.texWidth;
			info.uv.y = 1 - (float)bmInfo.y / (float)mbFont.texHeight;
			info.uv.width = (float)bmInfo.width / (float)mbFont.texWidth;
			info.uv.height = -1f * (float)bmInfo.height / (float)mbFont.texHeight;
			info.vert.x = (float)bmInfo.offsetX;
			info.vert.y = (float)bmInfo.offsetY;
			info.vert.width = (float)bmInfo.width;
			info.vert.height = (float)bmInfo.height;
			info.width = (float)bmInfo.advance;
#elif UNITY_5
            float uvx = 1f * bmInfo.x / mbFont.texWidth;
            float uvy = 1 - (1f * bmInfo.y / mbFont.texHeight);
            float uvw = 1f * bmInfo.width / mbFont.texWidth;
            float uvh = -1f * bmInfo.height / mbFont.texHeight;

            info.uvBottomLeft = new Vector2(uvx, uvy);
            info.uvBottomRight = new Vector2(uvx + uvw, uvy);
            info.uvTopLeft = new Vector2(uvx, uvy + uvh);
            info.uvTopRight = new Vector2(uvx + uvw, uvy + uvh);

            info.minX = bmInfo.offsetX;
            info.minY = - bmInfo.offsetY;
            // info.minY = 0;

            info.glyphWidth = bmInfo.width;
            info.glyphHeight = -bmInfo.height;
            info.advance = bmInfo.advance;
#endif
            characterInfo[i] = info;
        }
        CustomFont.characterInfo = characterInfo;

        string filePath = Application.dataPath + fntInfo.dirPath;

        string[] _pathArray = null;
        _pathArray = AssetDatabase.FindAssets("t:Texture", new string[] {
            //ResourceCommon.GetFolder(assetPath, true)
            fntInfo.dirPath
        });
        if (_pathArray == null || _pathArray.Length == 0)
        {
            EditorUtility.DisplayDialog("警告", "找不到图集!!!", "关闭");
            return;
        }
        string textureFilename = AssetDatabase.GUIDToAssetPath(_pathArray[0]);
        string AlphaMatpath = fntInfo.dirPath + fntInfo.name + "_mat.mat";
        string matPath = AlphaMatpath.Substring(AlphaMatpath.IndexOf("Assets"));

        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat == null)
        {
            mat = new Material(Shader.Find("UI/Default"));
            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.ImportAsset(matPath);
        }
        Texture tex = AssetDatabase.LoadAssetAtPath(textureFilename, typeof(Texture)) as Texture;
        mat.SetTexture("_MainTex", tex);

        CustomFont.material = mat;

        AssetDatabase.CreateAsset(CustomFont, fontPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("消息", "制作完成!!", "确定");
    }
}
