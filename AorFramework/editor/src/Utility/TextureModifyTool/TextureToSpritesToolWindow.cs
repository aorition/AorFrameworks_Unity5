using System;
using System.Collections.Generic;
using System.IO;
using AorBaseUtility;
using AorBaseUtility.Extends;
using AorBaseUtility.MiniJSON;
using Framework.Extends;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor.Utility
{
    public class TextureToSpritesToolWindow : UnityEditor.EditorWindow
    {

        private static GUIStyle _titleStyle;
        protected static GUIStyle titleStyle
        {
            get
            {
                if (_titleStyle == null)
                {
                    _titleStyle = EditorStyles.largeLabel.Clone();
                    _titleStyle.fontSize = 16;
                }
                return _titleStyle;
            }
        }

        private static GUIStyle _sTitleStyle;
        protected static GUIStyle sTitleStyle
        {
            get
            {
                if (_sTitleStyle == null)
                {
                    _sTitleStyle = EditorStyles.largeLabel.Clone();
                    _sTitleStyle.fontSize = 14;
                    _sTitleStyle.fontStyle = FontStyle.Bold;
                }
                return _sTitleStyle;
            }
        }

        //--------------------------------------------------------------

        private static TextureToSpritesToolWindow _instance;

        [MenuItem("FrameworkTools/位图工具/Sprites图集数据导入工具")]
        public static TextureToSpritesToolWindow init()
        {

            _instance = UnityEditor.EditorWindow.GetWindow<TextureToSpritesToolWindow>();
            _instance.minSize = new Vector2(497,280);

            return _instance;
        }

        private Vector2 _scrollPos = new Vector2();
        private void OnGUI()
        {

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            GUILayout.Space(15);

            _draw_toolTitle_UI();

            GUILayout.Space(15);

            _draw_importSprites_UI();

            GUILayout.Space(5);

            GUILayout.EndScrollView();

           // EditorPlusMethods.Draw_DebugWindowSizeUI();
        }

        //--------------------------------------

        private void _draw_toolTitle_UI()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("      Sprites图集数据导入工具      ", titleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("提示: 支持TexturePackerGUI(v3.0.+)DataFormat为Unity3D的Json描述文件");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        private Texture2D _targetTex;
        private TextAsset _targetJson;
        private bool _keepOldPovitData = true;

        private void _draw_importSprites_UI()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Space(5);
            _draw_subTitle_UI("------ Sprites图集数据导入 ------");
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.7f));

            _targetTex = (Texture2D)EditorGUILayout.ObjectField(_targetTex, typeof (Texture2D), false);
            _targetJson = (TextAsset)EditorGUILayout.ObjectField(_targetJson, typeof(TextAsset), false);

            GUILayout.EndVertical();

            if (GUILayout.Button("SetFromSelection",GUILayout.Height(28)))
            {
                if (Selection.objects.Length < 2)
                {
                    EditorUtility.DisplayDialog("提示", "您需要选中两个对象(一张图片,一个文本),请选择后重试", "关闭");
                    return;
                }

                foreach (UnityEngine.Object o in Selection.objects)
                {

                    if(o == null) continue;
                    if (o is Texture2D)
                    {
                        _targetTex = (Texture2D) o;
                    }else if (o is TextAsset)
                    {
                        _targetJson = (TextAsset)o;
                    }
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            _keepOldPovitData = EditorGUILayout.ToggleLeft("是否保留已有Povit数据?", _keepOldPovitData);

            GUILayout.Space(5);

            if (_targetTex && _targetJson)
            {
                if (GUILayout.Button("Start", GUILayout.Height(36)))
                {
                    ImportSpriteData(_targetTex, _targetJson.text, _keepOldPovitData);
                }
            }
            else
            {
                GUI.color = Color.gray;
                if (GUILayout.Button("Start", GUILayout.Height(36)))
                {
                    //do nothing;
                }
                GUI.color = Color.white;
            }


            GUILayout.Space(5);

            GUILayout.EndVertical();
        }

        private void _draw_subTitle_UI(string label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(label, sTitleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        //------------------------------------

        private static string SavePng(Texture2D inputTex, string dir, string pngName)
        {
            byte[] bytes = inputTex.EncodeToPNG();
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string path = dir + "/" + pngName + ".png";
            AorIO.SaveBytesToFile(path, bytes);
            return path;
        }

        //分离图片的RGB和Alpha通道
        public static void SeparateTexture2D(Texture2D tex)
        {
            string path = AssetDatabase.GetAssetPath(tex);

            EditorAssetInfo assetInfo = new EditorAssetInfo(path);
            
            bool changeRW = false;
            TextureImporterCompression ogCpn = TextureImporterCompression.Uncompressed;
            bool changeCompression = false;
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
            if (importer)
            {
                if (!importer.isReadable)
                {
                    changeRW = true;
                    importer.isReadable = true;
                }

                if (!importer.textureCompression.Equals(TextureImporterCompression.Uncompressed))
                {
                    changeCompression = true;
                    ogCpn = importer.textureCompression;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                }

                if (changeRW || changeCompression)
                {
                    importer.SaveAndReimport();
                }
            }

            Texture2D rgbTex = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
            Texture2D AlphaTex = new Texture2D(tex.width, tex.height, TextureFormat.Alpha8, false);

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

            SavePng(rgbTex, assetInfo.dirPath, assetInfo.name + "_rgb");
            SavePng(AlphaTex, assetInfo.dirPath, assetInfo.name + "_alpha");

            if (changeRW || changeCompression)
            {
                if (changeRW)
                {
                    importer.isReadable = false;
                }
                if (changeCompression)
                {
                    importer.textureCompression = ogCpn;
                }
                importer.SaveAndReimport();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
        }

        /// <summary>
        /// 根据json数据创建SpriteData
        /// </summary>
        private static void ImportSpriteData(Texture2D tex, string jsonData, bool keepOldData)
        {

            string path = AssetDatabase.GetAssetPath(tex);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (!textureImporter || string.IsNullOrEmpty(jsonData))
                return;

            //保存轴心点
            SpriteMetaData[] spriteMetaData = textureImporter.spritesheet;

            //SpriteMetaData
            Dictionary<string, object> dic = Json.DecodeToDic(jsonData);
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

            //-------------------

            textureImporter.textureType = TextureImporterType.Sprite;
            //textureImporter.generateCubemap = TextureImporterGenerateCubemap.AutoCubemap;
            textureImporter.isReadable = false;
            textureImporter.mipmapEnabled = false;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;

            textureImporter.spritesheet = spriteMetaDataList.ToArray();

            //m_SpriteMeshType 这个是个私用属性, 居然在TextureImporter反编译里都找不到...别问我是这么找到这个字段名的..md.
            textureImporter.SetNonPublicField("m_SpriteMeshType", SpriteMeshType.FullRect);

            textureImporter.SaveAndReimport();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("消息", "Sprite数据导入完成!", "关闭");

        }
        
        private static Vector2 PviotFix(ref SpriteMetaData za, SpriteMetaData[] spriteMetaDataForPviot)
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

    }
}
