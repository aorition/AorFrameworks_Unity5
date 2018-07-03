using System.Collections;
using System.Collections.Generic;
using AorBaseUtility;
using Framework.Editor;
using UnityEngine;
using UnityEditor;

/// <summary>
/// RenderTexture 转 Texture2D 测试脚本
/// </summary>
public class RT2TexTestCode  {

    [MenuItem("Test/RT2TexTestCode")]
    public static void RT2TexFunc()
    {
        if (Selection.objects != null && Selection.objects.Length > 0)
        {
            int i, len = Selection.objects.Length;
            List<RenderTexture> rtList = new List<RenderTexture>();
            for (i = 0; i < len; i++)
            {
                if (Selection.objects[i] is RenderTexture)
                {
                    rtList.Add((RenderTexture)Selection.objects[i]);
                }
            }

            if (rtList.Count == 0) return;

            len = rtList.Count;
            for (i = 0; i < len; i++)
            {
                RenderTexture rt = rtList[i];
                string path = AssetDatabase.GetAssetPath(rt);
                if (!string.IsNullOrEmpty(path))
                {
                    EditorAssetInfo info = new EditorAssetInfo(path);
                    Texture2D t2d = core(rt);

                    byte[] bytes = t2d.EncodeToPNG();
                    AorIO.SaveBytesToFile(info.dirPath + "/" + info.name + ".png", bytes);

                }
                else
                {
                    //Error
                }
                AssetDatabase.Refresh();
            }

        }
    }

    private static Texture2D core(RenderTexture rt)
    {
        int width = rt.width;
        int height = rt.height;
        Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
        RenderTexture backup = RenderTexture.active;
        RenderTexture.active = rt;
        texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = backup;
        return texture2D;
    }



}
