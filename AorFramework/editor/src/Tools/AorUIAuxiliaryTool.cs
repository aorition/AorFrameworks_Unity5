using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using YoukiaCore.MiniJson;
using YoukiaUnity.GUI.AorUI;
using YoukiaUnity.Resource;

public class AorUIAuxiliaryTool : AorEditorWindow
{
    [MenuItem("Youkia/AorUI辅助工具")]
    public static void openWindow()
    {
        EditorWindow.GetWindow<AorUIAuxiliaryTool>("辅助工具");
    }

    /// <summary>
    /// 转换为等相对放定位
    /// </summary>
    private void TransRT2AorRT()
    {

        GameObject ausGO = GameObject.Find(AorUIManager.PrefabName);
        if (ausGO == null)
        {
            EditorUtility.DisplayDialog("消息", "Hierarchy里没有找到" + AorUIManager.PrefabName + ",\n操作失败.", "关闭");
            return;
        }
        AorUIManager aum = ausGO.GetComponent<AorUIManager>();
        if (aum == null)
        {
            EditorUtility.DisplayDialog("消息", AorUIManager.PrefabName + "没有挂载AorUIManager,\n操作失败.", "关闭");
            return;
        }

        if (aum.ScaleMode != ScaleModeType.noScale)
        {
            EditorUtility.DisplayDialog("消息", "请将AorUIManager.ScaleMode设置为No Scale,\n然后重试", "关闭");
            return;
        }

        UnityEngine.Object[] selectedList = Selection.objects;
        List<GameObject> gos = new List<GameObject>();

        int i, length = selectedList.Length;
        for (i = 0; i < length; i++)
        {
            if (selectedList[i] is GameObject)
            {
                GameObject ad = selectedList[i] as GameObject;
                gos.Add(ad);
            }
        }

        length = gos.Count;
        for (i = 0; i < length; i++)
        {

            RectTransform _rt = gos[i].GetComponent<RectTransform>();
            if (_rt == null) continue;
            Rect _rect = _rt.rect;
            RectTransform _parentRT = gos[i].transform.parent.GetComponent<RectTransform>();
            if (_rt != null)
            {
                /*
                Text _tx = _rt.GetComponent<Text>();
                if (_tx == null) {
                */
                //常规方式
                Vector2 parentBase = new Vector2(_parentRT.rect.width * _parentRT.pivot.x, _parentRT.rect.height * _parentRT.pivot.y);

                float xMin = (parentBase.x + _rt.localPosition.x - _rect.width * _rt.pivot.x) / _parentRT.rect.width;
                float yMin = (parentBase.y + _rt.localPosition.y - _rect.height * _rt.pivot.y) / _parentRT.rect.height;
                float xMax = (parentBase.x + _rt.localPosition.x + _rect.width * (1 - _rt.pivot.x)) / _parentRT.rect.width;
                float yMax = (parentBase.y + _rt.localPosition.y + _rect.height * (1 - _rt.pivot.y)) / _parentRT.rect.height;
                //  Debug.UiLog("ResizeHandler -> autoSetFixScale :: [ " + xMin.ToString() + " , " + yMin.ToString() + " ],[ " + xMax.ToString() + " , " + yMax.ToString() + " ]");

                _rt.anchorMin = new Vector2(xMin, yMin);
                _rt.anchorMax = new Vector2(xMax, yMax);
                _rt.sizeDelta = Vector2.zero;
                _rt.anchoredPosition = Vector2.zero;

                Debug.Log("||RectTransformHepler > 已转换[ " + _rt.gameObject.name + " ]为等相对放定位.");

            }

        }
    }

    private void TransRT2AorRT_H()
    {

        GameObject ausGO = GameObject.Find(AorUIManager.PrefabName);
        if (ausGO == null)
        {
            EditorUtility.DisplayDialog("消息", "Hierarchy里没有找到" + AorUIManager.PrefabName + ",\n操作失败.", "关闭");
            return;
        }
        AorUIManager aum = ausGO.GetComponent<AorUIManager>();
        if (aum == null)
        {
            EditorUtility.DisplayDialog("消息", AorUIManager.PrefabName + "没有挂载AorUIManager,\n操作失败.", "关闭");
            return;
        }

        if (aum.ScaleMode != ScaleModeType.noScale)
        {
            EditorUtility.DisplayDialog("消息", "请将AorUIManager.ScaleMode设置为No Scale,\n然后重试", "关闭");
            return;
        }

        UnityEngine.Object[] selectedList = Selection.objects;
        List<GameObject> gos = new List<GameObject>();

        int i, length = selectedList.Length;
        for (i = 0; i < length; i++)
        {
            if (selectedList[i] is GameObject)
            {
                GameObject ad = selectedList[i] as GameObject;
                gos.Add(ad);
            }
        }

        length = gos.Count;
        for (i = 0; i < length; i++)
        {

            RectTransform _rt = gos[i].GetComponent<RectTransform>();
            if (_rt == null) continue;
            Rect _rect = _rt.rect;
            RectTransform _parentRT = gos[i].transform.parent.GetComponent<RectTransform>();
            if (_rt != null)
            {

                Vector2 parentBase = new Vector2(_parentRT.rect.width * _parentRT.pivot.x, _parentRT.rect.height * _parentRT.pivot.y);

                Vector2 baseSD = new Vector2(_rt.sizeDelta.x, _rt.sizeDelta.y);
                Vector2 baseAP = new Vector2(_rt.anchoredPosition.x, _rt.anchoredPosition.y);

                float xMin = (parentBase.x + _rt.localPosition.x - _rect.width * _rt.pivot.x) / _parentRT.rect.width;
                float yMin = _rt.anchorMin.y;
                float xMax = (parentBase.x + _rt.localPosition.x + _rect.width * (1 - _rt.pivot.x)) / _parentRT.rect.width;
                float yMax = _rt.anchorMax.y;
                //  Debug.UiLog("ResizeHandler -> autoSetFixScale :: [ " + xMin.ToString() + " , " + yMin.ToString() + " ],[ " + xMax.ToString() + " , " + yMax.ToString() + " ]");

                _rt.anchorMin = new Vector2(xMin, yMin);
                _rt.anchorMax = new Vector2(xMax, yMax);
                _rt.sizeDelta = new Vector2(0f, baseSD.y);
                _rt.anchoredPosition = new Vector2(0f, baseAP.y);

                Debug.Log("||RectTransformHepler > 已转换[ " + _rt.gameObject.name + " ]为等相对放定位(横向).");
            }
        }
    }

    private void TransRT2AorRT_V()
    {

        GameObject ausGO = GameObject.Find(AorUIManager.PrefabName);
        if (ausGO == null)
        {
            EditorUtility.DisplayDialog("消息", "Hierarchy里没有找到" + AorUIManager.PrefabName + ",\n操作失败.", "关闭");
            return;
        }
        AorUIManager aum = ausGO.GetComponent<AorUIManager>();
        if (aum == null)
        {
            EditorUtility.DisplayDialog("消息", AorUIManager.PrefabName + "没有挂载AorUIManager,\n操作失败.", "关闭");
            return;
        }

        if (aum.ScaleMode != ScaleModeType.noScale)
        {
            EditorUtility.DisplayDialog("消息", "请将AorUIManager.ScaleMode设置为No Scale,\n然后重试", "关闭");
            return;
        }

        UnityEngine.Object[] selectedList = Selection.objects;
        List<GameObject> gos = new List<GameObject>();

        int i, length = selectedList.Length;
        for (i = 0; i < length; i++)
        {
            if (selectedList[i] is GameObject)
            {
                GameObject ad = selectedList[i] as GameObject;
                gos.Add(ad);
            }
        }

        length = gos.Count;
        for (i = 0; i < length; i++)
        {

            RectTransform _rt = gos[i].GetComponent<RectTransform>();
            if (_rt == null) continue;
            Rect _rect = _rt.rect;
            RectTransform _parentRT = gos[i].transform.parent.GetComponent<RectTransform>();
            if (_rt != null)
            {

                Vector2 parentBase = new Vector2(_parentRT.rect.width * _parentRT.pivot.x, _parentRT.rect.height * _parentRT.pivot.y);

                Vector2 baseSD = new Vector2(_rt.sizeDelta.x, _rt.sizeDelta.y);
                Vector2 baseAP = new Vector2(_rt.anchoredPosition.x, _rt.anchoredPosition.y);

                float xMin = _rt.anchorMin.x;
                float yMin = (parentBase.y + _rt.localPosition.y - _rect.height * _rt.pivot.y) / _parentRT.rect.height;
                float xMax = _rt.anchorMax.x;
                float yMax = (parentBase.y + _rt.localPosition.y + _rect.height * (1 - _rt.pivot.y)) / _parentRT.rect.height;
                //  Debug.UiLog("ResizeHandler -> autoSetFixScale :: [ " + xMin.ToString() + " , " + yMin.ToString() + " ],[ " + xMax.ToString() + " , " + yMax.ToString() + " ]");

                _rt.anchorMin = new Vector2(xMin, yMin);
                _rt.anchorMax = new Vector2(xMax, yMax);
                _rt.sizeDelta = new Vector2(baseSD.x, 0f);
                _rt.anchoredPosition = new Vector2(baseAP.x, 0f);

                Debug.Log("||RectTransformHepler > 已转换[ " + _rt.gameObject.name + " ]为等相对放定位(纵向).");

            }

        }
    }

    private void horizontalArrange()
    {
        UnityEngine.Object[] selectedList = Selection.objects;
        int i, length = selectedList.Length;
        List<RectTransform> rtList = new List<RectTransform>();
        float xPosMin = 0;
        float xPosMax = 0;
        for (i = 0; i < length; i++)
        {
            if (selectedList[i] is GameObject)
            {
                RectTransform rt = (selectedList[i] as GameObject).GetComponent<RectTransform>();
                if (rt != null)
                {
                    if (rt.anchoredPosition.x < xPosMin)
                    {
                        xPosMin = rt.anchoredPosition.x;
                    }
                    if (rt.anchoredPosition.x > xPosMax)
                    {
                        xPosMax = rt.anchoredPosition.x;
                    }
                    rtList.Add(rt);
                }
            }
        }
        rtList.Sort((p1, p2) =>
        {
            return p1.transform.GetSiblingIndex().CompareTo(p2.transform.GetSiblingIndex());
        });
        length = rtList.Count;
        for (i = 0; i < length; i++)
        {
            float nx = (xPosMax - xPosMin) / (length - 1) * i + xPosMin;
            rtList[i].anchoredPosition = new Vector2(nx, rtList[i].anchoredPosition.y);
        }
    }
    private void VerticalArrange()
    {
        UnityEngine.Object[] selectedList = Selection.objects;
        int i, length = selectedList.Length;
        List<RectTransform> rtList = new List<RectTransform>();
        float yPosMin = 0;
        float yPosMax = 0;
        for (i = 0; i < length; i++)
        {
            if (selectedList[i] is GameObject)
            {
                RectTransform rt = (selectedList[i] as GameObject).GetComponent<RectTransform>();
                if (rt != null)
                {
                    if (rt.anchoredPosition.y < yPosMin)
                    {
                        yPosMin = rt.anchoredPosition.y;
                    }
                    if (rt.anchoredPosition.y > yPosMax)
                    {
                        yPosMax = rt.anchoredPosition.y;
                    }
                    rtList.Add(rt);
                }
            }
        }
        rtList.Sort((p1, p2) =>
        {
            return p2.transform.GetSiblingIndex().CompareTo(p1.transform.GetSiblingIndex());
        });
        length = rtList.Count;
        for (i = 0; i < length; i++)
        {
            float ny = (yPosMax - yPosMin) / (length - 1) * i + yPosMin;
            rtList[i].anchoredPosition = new Vector2(rtList[i].anchoredPosition.x, ny);
        }
    }
    // -------------------------------------------------图集导入工具方法------------------------------------

    /// <summary>
    /// 根据json数据创建SpriteData
    /// </summary>
    private void importSpriteData(bool keepOldData)
    {

        if (Selection.objects.Length < 2)
        {
            EditorUtility.DisplayDialog("提示", "导入Sprite数据操作需要您选中两个对象(一张图片,一个文本),请选择后重试", "关闭");
            return;
        }

        UnityEngine.Object[] texList = Selection.objects;
        if (texList[0] is TextAsset)
        {
            TextAsset temp = texList[0] as TextAsset;
            texList[0] = texList[1];
            texList[1] = temp;
        }

        if (texList == null || texList.Length <= 1)
            return;

        string path = AssetDatabase.GetAssetPath(texList[0]);
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        textureImporter.textureType = TextureImporterType.Advanced;
        textureImporter.generateCubemap = TextureImporterGenerateCubemap.None;
        textureImporter.isReadable = false;
        textureImporter.mipmapEnabled = false;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;


        //保存轴心点
        SpriteMetaData[] spriteMetaData = textureImporter.spritesheet;

        if (textureImporter == null)
            return;

        //选json数据
        TextAsset jsonText = texList[1] as TextAsset;

        //SpriteMetaData

        Dictionary<string, object> dic = Json.Decode(jsonText.text) as Dictionary<string, object>;
        Dictionary<string, object> frames = dic["frames"] as Dictionary<string, object>;
        List<SpriteMetaData> spriteMetaDataList = new List<SpriteMetaData>();

        Dictionary<string, object> meta = dic["meta"] as Dictionary<string, object>;
        Dictionary<string, object> metaSize = meta["size"] as Dictionary<string, object>;

        //float texW = Utils.StrToFloat (metaSize["w"].ToString ());
        //float texW = float.Parse(metaSize["w"].ToString());
        //float texH = Utils.StrToFloat (metaSize["h"].ToString ());
        float texH = float.Parse(metaSize["h"].ToString());

        foreach (KeyValuePair<string, object> item in frames)
        {

            Dictionary<string, object> frame = (item.Value as Dictionary<string, object>)["frame"] as Dictionary<string, object>;


            Dictionary<string, object> sourceSize = (item.Value as Dictionary<string, object>)["sourceSize"] as Dictionary<string, object>;

            Dictionary<string, object> spriteSourceSize = (item.Value as Dictionary<string, object>)["spriteSourceSize"] as Dictionary<string, object>;

            bool trimmed = (bool)((item.Value as Dictionary<string, object>)["trimmed"]);

            //float w = (Utils.StrToFloat (frame["w"].ToString ()));
            float w = float.Parse(frame["w"].ToString());
            //float h = (Utils.StrToFloat (frame["h"].ToString ()));
            float h = float.Parse(frame["h"].ToString());
            //float x = (Utils.StrToFloat (frame["x"].ToString ()));
            float x = float.Parse(frame["x"].ToString());
            //float y = (texH - Utils.StrToFloat (frame["y"].ToString ()) - h);
            float y = texH - float.Parse(frame["y"].ToString()) - h;

            if (item.Key == "daitynpc_work_a_01.png")
            {
                Debug.Log("");
            }
            Vector2 center = new Vector2(0.5f, 0.5f);
            if (trimmed)
            {
                float orgW = float.Parse(sourceSize["w"].ToString());
                float orgH = float.Parse(sourceSize["h"].ToString());
                float spriteX = float.Parse(spriteSourceSize["x"].ToString());
                float spriteY = float.Parse(spriteSourceSize["y"].ToString());
                float spriteW = float.Parse(spriteSourceSize["w"].ToString());
                float spriteH = float.Parse(spriteSourceSize["h"].ToString());

                center.x = (orgW * 0.5f - spriteX) / w;
                center.y = (spriteH - (orgH * 0.5f - spriteY)) / h;
            }

            SpriteMetaData za = new SpriteMetaData();

            za.name = item.Key;
            za.name = za.name.Split('.')[0];
            za.rect = new Rect(x, y, w, h);
            za.alignment = 9;
            za.pivot = center;

            if (keepOldData)
                PviotFix(ref za, spriteMetaData);

            spriteMetaDataList.Add(za);

        }

        textureImporter.spritesheet = spriteMetaDataList.ToArray();
        EditorUtility.DisplayDialog("消息", "恭喜你成功了!!!", "关闭");

    }


    //分离图片的RGB和Alpha通道
    private void separateImage(Texture2D tex)
    {
        string path = AssetDatabase.GetAssetPath(tex);
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        textureImporter.textureType = TextureImporterType.Advanced;
        textureImporter.generateCubemap = TextureImporterGenerateCubemap.None;
        textureImporter.isReadable = true;
        textureImporter.mipmapEnabled = false;
        textureImporter.textureFormat = TextureImporterFormat.RGBA32;
        AssetDatabase.SaveAssets();


        path = path.Substring(6, path.Length - 6);
        path = Application.dataPath + path;
        string[] str = path.Split('.');
        if (str == null || str.Length > 2)
        {
            Debug.Log("文件名不合规范!");
            return;
        }

        string RGBpath = str[0] + "_RGB." + str[1];
        string Alphapath = str[0] + "_Alpha." + str[1];


        Texture2D rgbTex = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
        Texture2D AlphaTex = new Texture2D(tex.width, tex.height);

        //数据产生
        Color[] datas = tex.GetPixels(0, 0, tex.width, tex.height);
        Color[] RGBdatas = new Color[datas.Length];
        Color[] Alphadatas = new Color[datas.Length];


        //数据处理
        for (int i = 0; i < datas.Length; i++)
        {
            RGBdatas[i] = datas[i];
            RGBdatas[i].a = 1;
        }
        rgbTex.SetPixels(RGBdatas);
        rgbTex.Apply();

        for (int i = 0; i < datas.Length; i++)
        {
            Alphadatas[i] = Color.Lerp(Color.black, Color.white, datas[i].a);
            Alphadatas[i].a = 1;
        }

        AlphaTex.SetPixels(Alphadatas);
        AlphaTex.Apply();





        savePng(rgbTex, ResourceCommon.GetFolder(RGBpath), ResourceCommon.GetFileName(RGBpath, true));

        savePng(AlphaTex, ResourceCommon.GetFolder(Alphapath), ResourceCommon.GetFileName(Alphapath, true));


    }


    void savePng(Texture2D inputTex, string contents, string pngName)
    {
        byte[] bytes = inputTex.EncodeToPNG();
        if (!Directory.Exists(contents))
            Directory.CreateDirectory(contents);
        FileStream file = File.Open(contents + "/" + pngName + ".png", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();

    }


    private Vector2 PviotFix(ref SpriteMetaData za, SpriteMetaData[] spriteMetaDataForPviot)
    {
        foreach (SpriteMetaData each2 in spriteMetaDataForPviot)
        {
            if (za.name == each2.name)
            {
                za.alignment = each2.alignment;
                za.pivot = each2.pivot;
                za.border = each2.border;
                za.rect = new Rect(za.rect.x, za.rect.y, each2.rect.width, each2.rect.height);
                break;
            }
        }
        return new Vector2(0.5f, 0.5f);
    }



    //-----------------------------------------------------end---------------------------------------------
    private Color col = Color.black;
    private bool keepOld = true;
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        EditorGUILayout.Separator();
        GUILayout.Label("转换RectTransform为等相对放定位");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("选定一个RectTransform对象,\n转换为相对定位(横向)!"))
        {
            TransRT2AorRT_H();
        }
        GUILayout.Space(1);
        if (GUILayout.Button("选定一个RectTransform对象,\n转换为相对定位(纵向)!"))
        {
            TransRT2AorRT_V();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("选定一个RectTransform对象,\n转换为相对定位!"))
        {
            TransRT2AorRT();
        }

        EditorGUILayout.Separator();
        GUILayout.Label("排列分布对象");
        if (GUILayout.Button("选定一组RectTransfrom,点击横向分布"))
        {
            horizontalArrange();
        }
        EditorGUILayout.Separator();
        if (GUILayout.Button("选定一组RectTransfrom,点击竖向分布"))
        {
            VerticalArrange();
        }

        EditorGUILayout.Separator();
        GUILayout.Label("场景相机查看器");
        if (GUILayout.Button("场景相机查看器", GUI.skin.GetStyle("LargeButton"), GUILayout.Height(30)))
        {
            EditorWindow.GetWindow<CameraBrowserWindow>("场景相机查看器", true);
        }

        EditorGUILayout.Separator();
        GUILayout.Label("场景物件辅助工具");
        if (GUILayout.Button("场景物件辅助工具", GUI.skin.GetStyle("LargeButton"), GUILayout.Height(30)))
        {
            EditorWindow.GetWindow<AorEditorSceneInfoTool>("场景物件辅助工具", true);
        }

        EditorGUILayout.Separator();
        GUILayout.Label("图集导入工具(for TexturePacker)");
        keepOld = GUILayout.Toggle(keepOld, "保持老数据");
        if (GUILayout.Button("选定一个TextAesset对象和一个Texture2D对象,\n生成SpriteMetaData!"))
        {
            importSpriteData(keepOld);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }



        if (GUILayout.Button("分离图集"))
        {
            object obj = Selection.activeObject;
            if (obj is Texture2D)
            {
                separateImage(obj as Texture2D);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

        }



        EditorGUILayout.Separator();
        GUILayout.Label("其他工具");
        if (GUILayout.Button("GUI样式查看器", GUI.skin.GetStyle("LargeButton"), GUILayout.Height(30)))
        {
            EditorWindow.GetWindow<EditorStyleView>("GUI样式查看器", true);
        }

        GUILayout.EndVertical();
    }
}