using System.Collections;
using System.Collections.Generic;
using AorBaseUtility;
using Framework.Extends;
using UnityEngine;
using UnityEditor;

namespace Framework.Editor.Utility
{
    /// <summary>
    /// 位图通道分离/合并工具
    /// </summary>
    public class TexChannelToolWindow : UnityEditor.EditorWindow
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

        public static void MergeTextureColorsUsingChannel(Texture2D target, TChannelInfo info, ref Color[] colors)
        {

            bool changeRW = false;
            TextureImporterCompression ogCpn = TextureImporterCompression.Uncompressed;
            bool changeCompression = false;
            string path = AssetDatabase.GetAssetPath(target);
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

            Color[] srColors = target.GetPixels();

            for (int i = 0; i < colors.Length; i++)
            {
                Color oColor = colors[i];

                //如果仅仅合并出A通道,需要特殊处理(只取目标图像的R通道值)
                if (!info.R && !info.G && !info.B && info.A)
                {
                    float alpha = srColors[i].r;
                    colors[i] = new Color(oColor.r, oColor.g, oColor.b, alpha);
                }
                else
                {
                    //常规逻辑
                    colors[i] = new Color(
                        info.R ? oColor.r + srColors[i].r : oColor.r,
                        info.G ? oColor.g + srColors[i].g : oColor.g,
                        info.B ? oColor.b + srColors[i].b : oColor.b,
                        info.A ? oColor.a + srColors[i].a : oColor.a
                        );
                }
            }

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

        }

        /// <summary>
        /// 从源图中分离某(多个)通道
        /// </summary>
        /// <param name="src">源图</param>
        /// <param name="tcInfo">通道分离信息</param>
        /// <returns>返回分离通道后的新图</returns>
        public static Texture2D SliceChannel(Texture2D src, TChannelInfo tcInfo, out TextureImporter importer)
        {
            bool changeRW = false;
            TextureImporterCompression ogCpn = TextureImporterCompression.Uncompressed;
            bool changeCompression = false;
            string path = AssetDatabase.GetAssetPath(src);
            importer = (TextureImporter)TextureImporter.GetAtPath(path);
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

            //            Texture2D tex = new Texture2D(src.width, src.height, src.format, src.mipmapCount > 0);
            Texture2D tex = new Texture2D(src.width, src.height, TextureFormat.RGBA32, src.mipmapCount > 0);
            Color[] srcColors = src.GetPixels();
            Color[] nColors = new Color[srcColors.Length];

            for (int i = 0; i < srcColors.Length; i++)
            {

                //如果仅仅分离出A通道,需要特殊处理
                if (!tcInfo.R && !tcInfo.G && !tcInfo.B && tcInfo.A)
                {
                    nColors[i] = new Color(srcColors[i].a, srcColors[i].a, srcColors[i].a, 1);
                }
                else
                {
                    //常规逻辑
                    nColors[i] = new Color(
                            tcInfo.R ? srcColors[i].r : 0,
                            tcInfo.G ? srcColors[i].g : 0,
                            tcInfo.B ? srcColors[i].b : 0,
                            tcInfo.A ? srcColors[i].a : 1
                            );
                }


            }

            tex.SetPixels(nColors);
            tex.Apply();

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

            return tex;
        }

        public static void SaveTextureToPNG(Texture2D tex, TextureImporter srcImporter, string path)
        {

            byte[] bytes = tex.EncodeToPNG();
            AorIO.SaveBytesToFile(path, bytes);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
            if (importer)
            {
                importer.textureType = srcImporter.textureType;
                importer.textureShape = srcImporter.textureShape;
                importer.sRGBTexture = srcImporter.sRGBTexture;
                importer.alphaSource = srcImporter.alphaSource;
                importer.alphaIsTransparency = srcImporter.alphaIsTransparency;

                importer.npotScale = srcImporter.npotScale;
                importer.isReadable = srcImporter.isReadable;

                importer.mipmapEnabled = srcImporter.mipmapEnabled;
                importer.borderMipmap = srcImporter.borderMipmap;
                importer.mipmapFilter = srcImporter.mipmapFilter;
                importer.fadeout = srcImporter.fadeout;
                importer.mipmapFadeDistanceStart = srcImporter.mipmapFadeDistanceStart;
                importer.mipmapFadeDistanceEnd = srcImporter.mipmapFadeDistanceEnd;

                importer.wrapMode = srcImporter.wrapMode;
                importer.filterMode = srcImporter.filterMode;
                importer.anisoLevel = srcImporter.anisoLevel;

                importer.textureCompression = srcImporter.textureCompression;
                importer.allowAlphaSplitting = srcImporter.allowAlphaSplitting;
                importer.compressionQuality = srcImporter.compressionQuality;
                importer.crunchedCompression = srcImporter.crunchedCompression;
                importer.maxTextureSize = srcImporter.maxTextureSize;
                //etc ...

                importer.SetPlatformTextureSettings(srcImporter.GetDefaultPlatformTextureSettings());

                importer.SaveAndReimport();
            }

        }

        //-------------------------------------------------------------

        private static TexChannelToolWindow _instance;

        [MenuItem("FrameworkTools/位图工具/通道分离(合并)工具")]
        public static TexChannelToolWindow init()
        {

            _instance = UnityEditor.EditorWindow.GetWindow<TexChannelToolWindow>();
            _instance.minSize = new Vector2(495, 612);

            return _instance;
        }

        public struct TChannelInfo
        {

            public TChannelInfo(bool r, bool g, bool b, bool a)
            {
                this.R = r;
                this.G = g;
                this.B = b;
                this.A = a;
            }

            public bool R;
            public bool G;
            public bool B;
            public bool A;

            public string ToShortString()
            {
                return "_" + (R ? "R" : "") + (G ? "G" : "") + (B ? "B" : "") + (A ? "A" : "");
            }

        }

        private Vector2 _scrollPos = new Vector2();
        private void OnGUI()
        {

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            GUILayout.Space(15);

            _draw_toolTitle_UI();

            GUILayout.Space(15);

            _draw_savePath_UI();
            GUILayout.Space(15);
            _draw_SliceChannel_UI();
            GUILayout.Space(15);
            _draw_MergeTex_UI();
            GUILayout.Space(15);
            GUILayout.EndScrollView();

            //EditorPlusMethods.Draw_DebugWindowSizeUI();
        }

        //--------------------------------------

        private void _draw_toolTitle_UI()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("      通道分离(合并)工具      ", titleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("提示: 本工具仅支持PNG/JPG位图文件.");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        //=============================================================================

        private string _savePath;
        private void _draw_savePath_UI()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Space(5);
            _draw_subTitle_UI("------ 设置保存路径 ------");
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            _savePath = EditorGUILayout.TextField(_savePath);
            if (GUILayout.Button("UseSelection", GUILayout.Width(120)))
            {
                if (Selection.activeObject)
                {
                    string tp = AssetDatabase.GetAssetPath(Selection.activeObject);
                    if (!string.IsNullOrEmpty(tp))
                    {

                        EditorAssetInfo info = new EditorAssetInfo(tp);
                        _savePath = info.dirPath;

                    }
                    else
                    {
                        _savePath = "";
                    }
                }
                else
                {
                    _savePath = "";
                }
            }
            if (GUILayout.Button("Set", GUILayout.Width(50)))
            {
                _savePath = EditorUtility.SaveFolderPanel("设置保存路径", "", "");
                _savePath = _savePath.Replace(Application.dataPath, "Assets");
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        //=============================================================================

        private Texture2D _srcTexture;
        private readonly List<TChannelInfo> _sliceList = new List<TChannelInfo>();
        private int _sid = 0;
        private static string[] _tagLabels = {"快捷分离","高级分离"};

        private void _draw_SliceChannel_UI()
        {

            GUILayout.BeginVertical("box");
            GUILayout.Space(5);
            _draw_subTitle_UI("------ 分离通道 ------");
            GUILayout.Space(5);

            _sid = GUILayout.Toolbar(_sid, _tagLabels);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("源图");
            _srcTexture = (Texture2D)EditorGUILayout.ObjectField(_srcTexture, typeof(Texture2D), false);
            if (GUILayout.Button("SetFormSelection", GUILayout.Width(120)))
            {
                if (Selection.objects.Length > 0)
                {
                    if (Selection.objects[0] is Texture2D)
                    {
                        _srcTexture = (Texture2D) Selection.objects[0];
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            switch (_sid)
            {
                case 1:

                    GUILayout.BeginVertical("box");
                    GUILayout.Space(5);
                    GUILayout.Label("设置");
                    GUILayout.Space(5);

                    if (_sliceList.Count == 0)
                    {
                        _draw_noDataTip_UI();
                    }

                    for (var i = 0; i < _sliceList.Count; i++)
                    {

                        if (i > 0)
                        {
                            GUILayout.Space(2);
                        }

                        GUILayout.BeginHorizontal();

                        GUILayout.Label("No." + (i + 1));

                        GUILayout.FlexibleSpace();

                        bool r = _sliceList[i].R;
                        bool g = _sliceList[i].G;
                        bool b = _sliceList[i].B;
                        bool a = _sliceList[i].A;

                        bool dirty = __draw_TChannelInfo_UI(ref r, ref g, ref b, ref a);
                        if (dirty)
                        {
                            _sliceList[i] = new TChannelInfo(r, g, b, a);
                        }

                        if (GUILayout.Button("-", GUILayout.Width(50)))
                        {

                            _sliceList.RemoveAt(i);
                            Repaint();
                            return;
                        }

                        GUILayout.EndHorizontal();
                    }

                    GUILayout.BeginHorizontal();

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("+", GUILayout.Width(200), GUILayout.Height(22)))
                    {
                        _sliceList.Add(new TChannelInfo());
                        Repaint();
                        return;
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                    GUILayout.Space(5);

                    //--------------------

                    if (GUILayout.Button("Start", GUILayout.Height(28)))
                    {

                        if (!_srcTexture)
                        {
                            //Error :: 源图为空
                            EditorUtility.DisplayDialog("提示", "未设置源图片,请设置源图片.", "确定");
                            return;
                        }

                        if (!_vaildInputData(_sliceList)) return;

                        for (var i = 0; i < _sliceList.Count; i++)
                        {


                            EditorUtility.DisplayProgressBar("处理中..", "正在生成分离文件..." + i + " / " + _sliceList.Count, (float)i / _sliceList.Count);

                            TChannelInfo info = _sliceList[i];
                            TextureImporter importer;
                            Texture2D nTex = SliceChannel(_srcTexture, info, out importer);
                            if (nTex)
                            {
                                string nfName = _srcTexture.name + info.ToShortString() + ".png";
                                string path = _savePath + "/" + nfName;

                                //save;
                                SaveTextureToPNG(nTex, importer, path);
                            }
                            else
                            {
                                //Error 分离失败
                            }

                        }

                        EditorUtility.ClearProgressBar();

                    }

                    break;
                default: //0

                    GUILayout.BeginVertical("box");
                    GUILayout.Space(5);
                    GUILayout.Label("信息:快速模式将目标对象拆分为RBG和Alpha两张图片.");
                    GUILayout.Space(5);
                    GUILayout.EndVertical();

                    if (GUILayout.Button("Start", GUILayout.Height(28)))
                    {

                        if (!_srcTexture)
                        {
                            //Error :: 源图为空
                            EditorUtility.DisplayDialog("提示", "未设置源图片,请设置源图片.", "确定");
                            return;
                        }

                        //生成快速Info
                        List<TChannelInfo> _tmpInfos = new List<TChannelInfo>();
                        _tmpInfos.Add(new TChannelInfo(true, true, true, false));
                        _tmpInfos.Add(new TChannelInfo(false, false, false, true));

                        for (var i = 0; i < _tmpInfos.Count; i++)
                        {

                            EditorUtility.DisplayProgressBar("处理中..", "正在生成分离文件..." + i + " / " + _tmpInfos.Count, (float)i / _tmpInfos.Count);

                            TChannelInfo info = _tmpInfos[i];
                            TextureImporter importer;
                            Texture2D nTex = SliceChannel(_srcTexture, info, out importer);
                            if (nTex)
                            {
                                string nfName = _srcTexture.name + info.ToShortString() + ".png";
                                string path = _savePath + "/" + nfName;

                                //save;
                                SaveTextureToPNG(nTex, importer, path);
                            }
                            else
                            {
                                //Error 分离失败
                            }

                        }

                        _tmpInfos.Clear();
                        EditorUtility.ClearProgressBar();

                    }

                    break;
            }

            GUILayout.Space(5);

            GUILayout.EndVertical();
        }

        //=============================================================================

        private readonly List<TChannelInfo> _mergeInfoList = new List<TChannelInfo>();
        private readonly List<Texture2D> _srcMergeList = new List<Texture2D>();

        private void _draw_MergeTex_UI()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Space(5);
            _draw_subTitle_UI("------ 合并通道 ------");
            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            GUILayout.Space(5);
            GUILayout.Label("设置");
            GUILayout.Space(5);

            if (_mergeInfoList.Count == 0)
            {
                _draw_noDataTip_UI();
            }

            for (int i = 0; i < _mergeInfoList.Count; i++)
            {
                if (i > 0)
                {
                    GUILayout.Space(2);
                }

                GUILayout.BeginHorizontal();

                GUILayout.Label("No." + (i + 1));

                GUILayout.FlexibleSpace();

                _srcMergeList[i] = (Texture2D)EditorGUILayout.ObjectField(_srcMergeList[i], typeof(Texture2D), false);
                if (GUILayout.Button(new GUIContent("set","Set Form Selection"), GUILayout.Width(28)))
                {
                    if (Selection.objects.Length > 0)
                    {
                        if (Selection.objects[0] is Texture2D)
                        {
                            _srcMergeList[i] = (Texture2D)Selection.objects[0];
                        }
                    }
                }

                bool r = _mergeInfoList[i].R;
                bool g = _mergeInfoList[i].G;
                bool b = _mergeInfoList[i].B;
                bool a = _mergeInfoList[i].A;

                bool dirty = __draw_TChannelInfo_UI(ref r, ref g, ref b, ref a);
                if (dirty)
                {
                    _mergeInfoList[i] = new TChannelInfo(r, g, b, a);
                }

                if (GUILayout.Button("-", GUILayout.Width(50)))
                {
                    _mergeInfoList.RemoveAt(i);
                    _srcMergeList.RemoveAt(i);
                    Repaint();
                    return;
                }

                GUILayout.EndHorizontal();

            }

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("+", GUILayout.Width(200), GUILayout.Height(22)))
            {
                _mergeInfoList.Add(new TChannelInfo());
                _srcMergeList.Add(null);
                Repaint();
                return;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Space(5);

            if (GUILayout.Button("Start", GUILayout.Height(28)))
            {
                if (!_vaildInputData(_mergeInfoList)) return;

                //检查 _srcMergeList里面的图是否一样大
                bool isSameSize = true;
                int w = 0, h = 0;
                bool mipmap = false;
                string cacheTexName = "uname";
                TextureImporter cacheImporter = null;

                for (int v = 0; v < _srcMergeList.Count; v++)
                {
                    if (v > 0)
                    {
                        if (w != _srcMergeList[v].width || h != _srcMergeList[v].height)
                        {
                            isSameSize = false;
                            break;
                        }
                    }
                    else
                    {
                        w = _srcMergeList[v].width;
                        h = _srcMergeList[v].height;
                        mipmap = _srcMergeList[v].mipmapCount > 0;
                        string path = AssetDatabase.GetAssetPath(_srcMergeList[v]);
                        cacheImporter = (TextureImporter)TextureImporter.GetAtPath(path);
                        cacheTexName = _srcMergeList[v].name;
                    }
                }

                if (isSameSize)
                {
                    Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, mipmap);
                    Color[] nColors = new Color[w * h];
                    for (var i = 0; i < _mergeInfoList.Count; i++)
                    {

                        EditorUtility.DisplayProgressBar("处理中..", "正在合并通道..." + i + " / " + _mergeInfoList.Count, (float)i / _mergeInfoList.Count);

                        if (_srcMergeList[i])
                        {
                            MergeTextureColorsUsingChannel(_srcMergeList[i], _mergeInfoList[i], ref nColors);
                        }
                    }

                    EditorUtility.ClearProgressBar();

                    tex.SetPixels(nColors);
                    tex.Apply();

                    string path = _savePath + "/" + cacheTexName + "_merged.png";

                    SaveTextureToPNG(tex, cacheImporter, path);

                }
                else
                {
                    //Error :: 输入的图不一样大
                }

            }

            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        //-----------------------------------------------------------------

        private bool _vaildInputData(IList list)
        {
            if (string.IsNullOrEmpty(_savePath))
            {
                //Error :: 保存路径为空
                EditorUtility.DisplayDialog("提示", "保存路径未设置,请设置保存路径.", "确定");
                return false;
            }

            if (list.Count == 0)
            {
                EditorUtility.DisplayDialog("提示", "通道数据未设置,请添加通道设置.", "确定");
                //Error :: 导出图设置为空
                return false;
            }

            return true;
        }

        private bool __draw_TChannelInfo_UI(ref bool r, ref bool g, ref bool b, ref bool a)
        {
            bool dirty = false;
            bool nr = EditorGUILayout.ToggleLeft("R", r, GUILayout.Width(50));
            if (nr != r)
            {
                r = nr;
                dirty = true;
            }
            bool ng = EditorGUILayout.ToggleLeft("G", g, GUILayout.Width(50));
            if (ng != g)
            {
                g = ng;
                dirty = true;
            }
            bool nb = EditorGUILayout.ToggleLeft("B", b, GUILayout.Width(50));
            if (nb != b)
            {
                b = nb;
                dirty = true;
            }
            bool na = EditorGUILayout.ToggleLeft("A", a, GUILayout.Width(50));
            if (na != a)
            {
                a = na;
                dirty = true;
            }
            return dirty;
        }

        private void _draw_noDataTip_UI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(32);
            GUILayout.Label("<-- 暂无数据, 请单击 \"+\" 键添加数据. -->");
            GUILayout.EndHorizontal();
        }

        private void _draw_subTitle_UI(string label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(label, sTitleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

    }
}


