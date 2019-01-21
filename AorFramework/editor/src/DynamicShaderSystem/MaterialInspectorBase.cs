using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AorBaseUtility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Framework.Editor
{

    /// <summary>
    /// 
    /// 材质球(Shader)增强Inspector
    /// Author: Aorition
    /// 
    /// 扩展Shader须知:
    /// 
    ///     1.定义Shader描述格式:
    /// 
    ///                             //@@@DynamicShaderInfoStart
    ///                             //<Readonly> 此处定义你的shader描述 (单行; <Readonly>标签可选, 表示此描述不可更改)
    ///                             //@@@DynamicShaderInfoEnd
    /// 
    ///     2.指定动态可修改标签枚举范例 (Properties端):
    /// 
    ///     		                [Enum(UnityEngine.Rendering.CullMode)] _cull("Cull Mode", Float) = 2
    ///                             [Space(12)]
    ///                             [Enum(Off, 0, On, 1)] _zWrite("ZWrite", Float) = 0
    ///	                            [Enum(UnityEngine.Rendering.CompareFunction)] _zTest("ZTest", Float) = 0
    ///	                            [Space(12)]
    ///                             [Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 5
    ///	                            [Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 10
    ///	                            [Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
    ///	                            [Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
    /// 
    ///     3.指定动态可修改标签范例 (pass/SubShader端):
    /// 
    ///                             Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]
    ///                             ZWrite[_zWrite]
    ///                             ZTest[_zTest]
    ///                             Cull[_cull]
    /// 
    /// </summary>
    
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Material))]
    public class MaterialInspectorBase : MaterialEditor
    {

        public static GUIStyle CloneGUIStyle(GUIStyle obj)
        {
            GUIStyle s = new GUIStyle(obj);
            s.name = obj.name + "_Clone";
            return s;
        }

        private static GUIStyle _descLabelStyle;
        protected static GUIStyle DescLabelStyle
        {
            get
            {
                if (_descLabelStyle == null)
                {
                    _descLabelStyle = MaterialInspectorBase.CloneGUIStyle(GUI.skin.GetStyle("Label"));
                    _descLabelStyle.fontSize = 16;
                    _descLabelStyle.wordWrap = true;
                }
                return _descLabelStyle;
            }
        }

        private static GUIStyle _t0Style;
        protected static GUIStyle T0Style
        {
            get
            {
                if (_t0Style == null)
                {
                    _t0Style = CloneGUIStyle(GUI.skin.GetStyle("Label"));
                    _t0Style.fontSize = 14;
                    _t0Style.fontStyle = FontStyle.Bold;
                    _t0Style.wordWrap = true;
                }
                return _t0Style;
            }
        }

        private static GUIStyle _t1Style;
        protected static GUIStyle T1Style
        {
            get
            {
                if (_t1Style == null)
                {
                    _t1Style = CloneGUIStyle(GUI.skin.GetStyle("Label"));
                    _t1Style.fontSize = 12;
                    _t1Style.fontStyle = FontStyle.Bold;
                    _t1Style.wordWrap = true;
                }
                return _t1Style;
            }
        }

        private static Regex ShaderInfoRge = new Regex("//@@@DynamicShaderInfoStart\n(.*)\n//@@@DynamicShaderInfoEnd\n");
        private static Regex zwirteEnumRegex = new Regex("\\[Enum\\(Off,0,On,1\\)\\]([a-zA-Z0-9_]*)\\(\"([a-zA-Z0-9_]*)\",([a-zA-Z]+)\\)");
        private static Regex ztestEnumRegex = new Regex("\\[Enum\\(UnityEngine.Rendering.CompareFunction\\)\\]([a-zA-Z0-9_]*)\\(\"([a-zA-Z0-9_]*)\",([a-zA-Z]+)\\)");
        private static Regex cullEnumRegex = new Regex("\\[Enum\\(UnityEngine.Rendering.CullMode\\)\\]([a-zA-Z0-9_]*)\\(\"([a-zA-Z0-9_]*)\",([a-zA-Z]+)\\)");
        private static Regex blendEnumRegex = new Regex("\\[Enum\\(UnityEngine.Rendering.BlendMode\\)\\]([a-zA-Z0-9_]*)\\(\"([a-zA-Z0-9_]*)\",([a-zA-Z]+)\\)");
        private static Color DefineGUIColor = new Color(0.5f, 0.8f, 1f);

        /// <summary>
        /// 动态Render State Shader的基本可变内容的字段名称记录
        /// </summary>
        protected Dictionary<string, string> _shaderPropNameDefineDic = new Dictionary<string, string>();
        protected virtual Dictionary<string, string> ShaderPropNameDefineDic
        {
            get
            {
                return _shaderPropNameDefineDic;
            }
        }

        protected string _shaderPath;
        protected Material _targetMat;
        protected Shader _shaderCahce;
        protected string _shaderCahceCode;
        protected bool _shaderDescReadonly;
        protected string _shderDescription;
        protected string _shderDescriptionCache;
        public Material targetMat
        {
            get
            {
                if (_targetMat == null)
                    _targetMat = target as Material;

                return _targetMat;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (targetMat && targetMat.shader)
            {
                _shadercacheInit(targetMat.shader);
            }
        }

        protected bool _isCannotWirteDes;
        protected bool _isDynamicShader;

        protected bool _isDefine_Dynamic_Blend;
        protected bool _isDefine_Dynamic_Zwirte;
        protected bool _isDefine_Dynamic_ZTest;
        protected bool _isDefine_Dynamic_Cull;

        private string DynamicZWirtePropErrorInfo;
        private string DynamicSrcBlendPropErrorInfo;
        private string DynamicDstBlendPropErrorInfo;

        private void _shadercacheInit(Shader shader)
        {
            if (!shader) return;
            _shaderCahce = shader;

            //重置所有bool标识以及标识变量
            _isCannotWirteDes = false;
            _isDynamicShader = false;
            _isDefine_Dynamic_Blend = false;
            _isDefine_Dynamic_Zwirte = false;
            _isDefine_Dynamic_ZTest = false;
            _isDefine_Dynamic_Cull = false;

            _shaderDescReadonly = false;
            _isNewDescription = false;

            _isShowKeywords = false;
            _shaderCahceCode = string.Empty;
            _shderDescription = string.Empty;
            _shderDescriptionCache = string.Empty;

            //获取Shader文本数据
            _shaderPath = AssetDatabase.GetAssetPath(_shaderCahce);

            //过滤内置Shader和无法找到其路径的shader
            if (string.IsNullOrEmpty(_shaderPath)
                || _shaderPath == "Resources/unity_builtin_extra"
                || _shaderPath == "Library/unity default resources"
                )
            {
                //标识该Shader不能被注入Description;
                _isCannotWirteDes = true;
                return;
            }

            ShaderPropNameDefineDic.Clear();

            string path = Application.dataPath.Replace("Assets", "") + _shaderPath;
            _shaderCahceCode = AorIO.ReadStringFormFile(path);

            //材质的Shader为Missing状态时拿不到Shader的文本
            if (string.IsNullOrEmpty(_shaderCahceCode))
                return;

            //统一结束符 (\r\n -> \n)
            _shaderCahceCode = _shaderCahceCode.Replace("\r\n", "\n");

            //获取描述
            Match InfoMatch = ShaderInfoRge.Match(_shaderCahceCode);
            if (InfoMatch.Success)
            {
                string inner = InfoMatch.Groups[1].Value;
                _shaderDescReadonly = inner.ToLower().StartsWith("//<readonly>");
                _shderDescription = _shaderDescReadonly ? inner.Substring(12) : inner.Substring(2);

                //防止获取的描述中包含\n 或者 \r\n
                _shderDescription = _shderDescription.Replace("\n", "");

                _shderDescriptionCache = _shderDescription;
            }

            //建立副本并移除空格, 用于以下数据抓取
            string shaderCode = _shaderCahceCode.Replace(" ", "");

            //动态 RenderState 抓取 -----------

            //Zwirte
            Match zwirteMatch = zwirteEnumRegex.Match(shaderCode);
            bool zwirteSuccess = !regexCodeHasBeenCommentedOut(zwirteMatch, shaderCode);
            if (zwirteSuccess)
            {
                if (zwirteMatch.Groups[1].Value == "_ZWirte")
                {
                    DynamicZWirtePropErrorInfo = "警告:: Shader Properties中定义的_ZWirte字段与Unity默认字段重复,此问题可能导致参数记录的混乱(记录不了,默认值不对,记录错误值)等错误!";
                }

                _isDefine_Dynamic_Zwirte = zwirteMatch.Groups[2].Value.ToLower().Contains("zwirte")
                                          && zwirteMatch.Groups[3].Value.ToLower() == "float";
                ShaderPropNameDefineDic.Add("ZWirte", zwirteMatch.Groups[1].Value);
            }

            //ZTest
            Match ztestMatch = ztestEnumRegex.Match(shaderCode);
            bool ztestSuccess = !regexCodeHasBeenCommentedOut(ztestMatch, shaderCode);
            if (ztestSuccess)
            {
                _isDefine_Dynamic_ZTest = ztestMatch.Groups[2].Value.ToLower().Contains("ztest")
                                          && ztestMatch.Groups[3].Value.ToLower() == "float";
                ShaderPropNameDefineDic.Add("ZTest", ztestMatch.Groups[1].Value);
            }

            //Cull
            Match cullMatch = cullEnumRegex.Match(shaderCode);
            bool cullSuccess = !regexCodeHasBeenCommentedOut(cullMatch, shaderCode);
            if (cullSuccess)
            {
                _isDefine_Dynamic_ZTest = cullMatch.Groups[2].Value.ToLower().Contains("cull")
                                          && cullMatch.Groups[3].Value.ToLower() == "float";
                ShaderPropNameDefineDic.Add("Cull", cullMatch.Groups[1].Value);
            }

            //Blend
            Match blendMatch = blendEnumRegex.Match(shaderCode);
            bool blendMatchLoop = !regexCodeHasBeenCommentedOut(blendMatch, shaderCode);
            while (blendMatchLoop)
            {
                if (blendMatch.Groups[3].Value.ToLower() == "float")
                {
                    if (blendMatch.Groups[2].Value.ToLower().Contains("srcblend"))
                    {
                        //SrcBlend
                        if (blendMatch.Groups[1].Value == "_SrcBlend")
                        {
                            DynamicSrcBlendPropErrorInfo = "警告:: Shader Properties中定义的_SrcBlend字段与Unity默认字段重复,此问题可能导致参数记录的混乱(记录不了,默认值不对,记录错误值)等错误!";
                        }
                        ShaderPropNameDefineDic.Add("SrcBlend", blendMatch.Groups[1].Value);
                    }
                    else if (blendMatch.Groups[2].Value.ToLower().Contains("dstblend"))
                    {
                        //DstBlend
                        if (blendMatch.Groups[1].Value == "_DstBlend")
                        {
                            DynamicDstBlendPropErrorInfo = "警告:: Shader Properties中定义的_DstBlend字段与Unity默认字段重复,此问题可能导致参数记录的混乱(记录不了,默认值不对,记录错误值)等错误!";
                        }
                        ShaderPropNameDefineDic.Add("DstBlend", blendMatch.Groups[1].Value);
                    }
                    else if (blendMatch.Groups[2].Value.ToLower().Contains("srcalphablend"))
                    {
                        //SrcAlphaBlend
                        ShaderPropNameDefineDic.Add("SrcAlphaBlend", blendMatch.Groups[1].Value);
                    }
                    else if (blendMatch.Groups[2].Value.ToLower().Contains("dstalphablend"))
                    {
                        //DstAlphaBlend
                        ShaderPropNameDefineDic.Add("DstAlphaBlend", blendMatch.Groups[1].Value);
                    }
                }

                blendMatch = blendMatch.NextMatch();
                blendMatchLoop = !regexCodeHasBeenCommentedOut(blendMatch, shaderCode);

            }
            //判断Shader是否启用Dynamic_Blend
            _isDefine_Dynamic_Blend = ShaderPropNameDefineDic.ContainsKey("SrcBlend")
                          && ShaderPropNameDefineDic.ContainsKey("DstBlend")
                          && ShaderPropNameDefineDic.ContainsKey("SrcAlphaBlend")
                          && ShaderPropNameDefineDic.ContainsKey("DstAlphaBlend");

            _isDynamicShader = _isDefine_Dynamic_Blend
                               || _isDefine_Dynamic_Zwirte
                               || _isDefine_Dynamic_ZTest
                               || _isDefine_Dynamic_Cull;

            if (_isDynamicShader) OnDynamicShaderInit();
        }

        /// <summary>
        /// 检查匹配的字符串是否已经被注释掉了。
        /// 注意： 如果匹配不成功则直接返回true
        /// </summary>
        protected virtual bool regexCodeHasBeenCommentedOut(Match match, string matchingStr)
        {

            if (match == null || !match.Success || string.IsNullOrEmpty(matchingStr)) return true;

            int i = match.Index;
            string k;
            while (true)
            {
                i--;
                k = matchingStr.Substring(i, 1);
                if (k == "\n" || k == "\r" || k == "\r\n" || i <= 0)
                {
                    break;
                }
            }
            k = matchingStr.Substring(i, match.Index - i);
            return k.Contains("//");
        }

        protected virtual void OnDynamicShaderInit()
        {
            //
        }

        protected bool hasKeyWord(string key)
        {
            string[] keyWords = targetMat.shaderKeywords;
            foreach (string each in keyWords)
            {

                if (each.Contains(key))
                    return true;
            }

            return false;
        }

        private bool _isShowKeywords;
        private bool _isNewDescription;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //刷新
            if (_shaderCahce != targetMat.shader)
            {
                _targetMat = null;
                OnEnable();
            }

            if (!_targetMat || !_targetMat.shader) return;

            GUILayout.Space(10);

            // ShaderKeywords 功能

            GUILayout.BeginVertical("box");
            _isShowKeywords = EditorGUILayout.BeginToggleGroup("Show ShaderKeywords", _isShowKeywords);
            if (_isShowKeywords)
            {
                if (targetMat.shaderKeywords.Length == 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(Screen.width * 0.3f);
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Space(Screen.width * 0.1f);
                    GUILayout.Label("( none )");
                    GUILayout.EndHorizontal();
                    GUILayout.EndHorizontal();
                }
                else
                {
                    string[] keyWords = targetMat.shaderKeywords;
                    foreach (string each in keyWords)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(Screen.width * 0.3f);
                        GUILayout.BeginHorizontal("box");
                        GUILayout.Space(Screen.width * 0.1f);
                        GUILayout.Label(each);
                        GUILayout.EndHorizontal();
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(Screen.width * 0.3f);
                    GUI.backgroundColor = Color.red;
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Space(Screen.width * 0.1f);
                    if (GUILayout.Button(new GUIContent("Clear ShaderKeyWords", "清除Shader关键字")))
                    {
                        if (EditorUtility.DisplayDialog("提示", "你确定要清除现有关键字?", "确定", "取消"))
                        {
                            _targetMat.shaderKeywords = new string[0];
                        }
                    }
                    GUILayout.Space(Screen.width * 0.1f);
                    GUILayout.EndHorizontal();
                    GUI.backgroundColor = Color.white;
                    GUILayout.EndHorizontal();

                }
            }
            EditorGUILayout.EndToggleGroup();
            GUILayout.EndVertical();

            GUILayout.Space(10);

            // Shader Description : 
            if (!_isCannotWirteDes)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Space(8);
                GUILayout.Label("Shader Description : ", T0Style);
                GUILayout.Space(5);

                if (string.IsNullOrEmpty(_shderDescriptionCache))
                {
                    if (_isNewDescription)
                    {
                        _shderDescriptionCache = EditorGUILayout.TextArea(_shderDescriptionCache);
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("New Description", GUILayout.Width(Screen.width * 0.3f)))
                        {
                            _isNewDescription = true;
                        }
                        GUILayout.EndHorizontal();
                    }

                }
                else
                {

                    if (_shaderDescReadonly)
                    {
                        GUILayout.Label(_shderDescriptionCache, DescLabelStyle);
                    }
                    else
                    {

                        _shderDescriptionCache = EditorGUILayout.TextArea(_shderDescriptionCache, DescLabelStyle);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (_shderDescriptionCache != _shderDescription)
                        {
                            if (GUILayout.Button("SaveChanged", GUILayout.Width(Screen.width * 0.3f)))
                            {
                                _isNewDescription = false;
                                //防止新输入描述中包含\n 或者 \r\n
                                _shderDescriptionCache = _shderDescriptionCache.Replace("\r\n", "");
                                _shderDescriptionCache = _shderDescriptionCache.Replace("\n", "");

                                //移除首尾空白
                                _shderDescriptionCache = _shderDescriptionCache.Trim();

                                //检测是否含有"只读"标签
                                bool setReadOnly = false;
                                if (_shderDescriptionCache.StartsWith("<只读>"))
                                {
                                    _shderDescriptionCache = _shderDescriptionCache.Substring(4);
                                    setReadOnly = true;
                                }
                                else if(_shderDescriptionCache.ToLower().StartsWith("<readonly>"))
                                {
                                    _shderDescriptionCache = _shderDescriptionCache.Substring(10);
                                    setReadOnly = true;
                                }

                                //确认描述只读
                                if (setReadOnly)
                                {
                                    if (!EditorUtility.DisplayDialog("提示", "确定设置描述为只读描述?!(该操作无法撤销)", "确认", "取消"))
                                    {
                                        setReadOnly = false;
                                    }
                                }

                                //重写Shader文件
                                _writingDescription(setReadOnly, () =>
                                {
                                    _ReBuildShaderCode(() =>
                                    {
                                        //重写成功后
                                        _shderDescription = _shderDescriptionCache;
                                        if (setReadOnly)
                                        {
                                            _shaderDescReadonly = true;
                                        }
                                    });
                                });
                            }
                        }
                        GUILayout.EndHorizontal();
                    }

                }

                GUILayout.EndVertical();

                GUILayout.Space(10);
            }

            //不是基础的变形shader不给选项
            if (targetMat.shader && _isDynamicShader)
            {
                GUILayout.BeginVertical("box");
                GUI.color = DefineGUIColor;

                GUILayout.Space(8);
                GUILayout.Label("DynamicShader Tools : ", T0Style);
                GUILayout.Space(5);

                if (_isDefine_Dynamic_Blend)
                {
                    _draw_BlendTagUI();
                    GUILayout.Space(10);
                }

                OnDrawExtendsInspectorGUI();

                GUILayout.Space(5);
                GUI.color = Color.white;
                GUILayout.EndVertical();
            }

        }
        
        protected void _writingDescription(bool descReadonly, Action sucessDo)
        {
            Match infoMatch = ShaderInfoRge.Match(_shaderCahceCode);
            if (infoMatch.Success)
            {
                string rp = infoMatch.Value;
                rp = rp.Replace(infoMatch.Groups[1].Value,
                    descReadonly ? ("//<Readonly>" + _shderDescriptionCache) : ("//" + _shderDescriptionCache));
                _shaderCahceCode = _shaderCahceCode.Replace(infoMatch.Value, rp);
                sucessDo();
            }
            else
            {
                string des = string.Format("//@@@DynamicShaderInfoStart\n{0}\n//@@@DynamicShaderInfoEnd\n", descReadonly ? ("//<Readonly>" + _shderDescriptionCache) : ("//" + _shderDescriptionCache));
                _shaderCahceCode = des + _shaderCahceCode;
                sucessDo();
            }
        }

        protected void _ReBuildShaderCode(Action sucessDo)
        {
            string path = Application.dataPath.Replace("Assets", "") + _shaderPath;
            if (AorIO.SaveStringToFile(path, _shaderCahceCode))
            {
                EditorUtility.SetDirty(_shaderCahce);
                EditorUtility.SetDirty(_targetMat);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                sucessDo();
            }
        }

        public virtual void OnDrawExtendsInspectorGUI()
        {
            //draw something
        }

        private static GUIContent[] m_BlendTagUIGUIContents;
        protected static GUIContent[] BlendTagUIGUIContents
        {
            get
            {
                //index label                 Value
                //0     普通                  One Zero, One Zero
                //1     Alpha混合             SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                //2     叠加                  SrcAlpha One, One OneMinusSrcAlpha
                //3     背景加深              Zero DstColor, One OneMinusSrcAlpha
                //4     叠加(忽略Alpha)       One One, One One
                //5     柔和叠加(忽略Alpha)   SrcColor One, One OneMinusSrcAlpha
                //6     前景加深              Zero SrcColor, One OneMinusSrcAlpha
                //7     Spine专用             One OneMinusSrcAlpha, One OneMinusSrcAlpha
                if (m_BlendTagUIGUIContents == null)
                {
                    m_BlendTagUIGUIContents = new GUIContent[8];
                    m_BlendTagUIGUIContents[0] = new GUIContent("普通");
                    m_BlendTagUIGUIContents[1] = new GUIContent("Alpha混合");
                    m_BlendTagUIGUIContents[2] = new GUIContent("叠加");
                    m_BlendTagUIGUIContents[3] = new GUIContent("背景加深");
                    m_BlendTagUIGUIContents[4] = new GUIContent("叠加(忽略Alpha)");
                    m_BlendTagUIGUIContents[5] = new GUIContent("柔和叠加(忽略Alpha)");
                    m_BlendTagUIGUIContents[6] = new GUIContent("前景加深");
                    m_BlendTagUIGUIContents[7] = new GUIContent("Spine专用");
                }
                return m_BlendTagUIGUIContents;
            }
        }

        private int _getBlendValuesIndex()
        {
            BlendMode SrcBlend = (BlendMode) _targetMat.GetInt(ShaderPropNameDefineDic["SrcBlend"]);
            BlendMode DstBlend = (BlendMode) _targetMat.GetInt(ShaderPropNameDefineDic["DstBlend"]);
            BlendMode SrcAlphaBlend = (BlendMode) _targetMat.GetInt(ShaderPropNameDefineDic["SrcAlphaBlend"]);
            BlendMode DstAlphaBlend = (BlendMode) _targetMat.GetInt(ShaderPropNameDefineDic["DstAlphaBlend"]);

            if (SrcBlend == BlendMode.One && DstBlend == BlendMode.Zero 
                && SrcAlphaBlend == BlendMode.One && DstAlphaBlend == BlendMode.Zero)
            {
                //0     普通                  One Zero, One Zero
                return 0;
            }
            if (SrcBlend == BlendMode.SrcAlpha && DstBlend == BlendMode.OneMinusSrcAlpha
                     && SrcAlphaBlend == BlendMode.One && DstAlphaBlend == BlendMode.OneMinusSrcAlpha)
            {
                //1     Alpha混合             SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                return 1;
            }
            if (SrcBlend == BlendMode.SrcAlpha && DstBlend == BlendMode.One
                     && SrcAlphaBlend == BlendMode.One && DstAlphaBlend == BlendMode.OneMinusSrcAlpha)
            {
                //2     叠加                  SrcAlpha One, One OneMinusSrcAlpha
                return 2;
            }
            if (SrcBlend == BlendMode.Zero && DstBlend == BlendMode.DstColor
                     && SrcAlphaBlend == BlendMode.One && DstAlphaBlend == BlendMode.OneMinusSrcAlpha)
            {
                //3     背景加深              Zero DstColor, One OneMinusSrcAlpha
                return 3;
            }
            if (SrcBlend == BlendMode.One && DstBlend == BlendMode.One
                     && SrcAlphaBlend == BlendMode.One && DstAlphaBlend == BlendMode.One)
            {
                //4     叠加(忽略Alpha)       One One, One One
                return 4;
            }
            if (SrcBlend == BlendMode.SrcColor && DstBlend == BlendMode.One
                     && SrcAlphaBlend == BlendMode.One && DstAlphaBlend == BlendMode.OneMinusSrcAlpha)
            {
                //5     柔和叠加(忽略Alpha)   SrcColor One, One OneMinusSrcAlpha
                return 5;
            }
            if (SrcBlend == BlendMode.Zero && DstBlend == BlendMode.SrcColor
                     && SrcAlphaBlend == BlendMode.One && DstAlphaBlend == BlendMode.OneMinusSrcAlpha)
            {
                //6     前景加深              Zero SrcColor, One OneMinusSrcAlpha
                return 6;
            }
            if (SrcBlend == BlendMode.One && DstBlend == BlendMode.OneMinusSrcAlpha
                     && SrcAlphaBlend == BlendMode.One && DstAlphaBlend == BlendMode.OneMinusSrcAlpha)
            {
                //7     Spine专用             One OneMinusSrcAlpha, One OneMinusSrcAlpha
                return 7;
            }
            return -1;
        }

        private void _draw_BlendTagUI()
        {

            GUILayout.BeginVertical("box");
            GUILayout.Space(5);

            GUILayout.Label("混合模式:", T1Style);
            GUILayout.Space(5);

            //获取index
            int index = _getBlendValuesIndex();
            int xCount = Screen.width >= 480 ? 4 : (int) Screen.width/120;
            int hNum = Mathf.CeilToInt((float) BlendTagUIGUIContents.Length/xCount);
            int SGHeight = hNum * 30;
            index = GUILayout.SelectionGrid(index, BlendTagUIGUIContents, xCount,GUILayout.Height(SGHeight));

            GUILayout.Space(5);
            GUILayout.EndVertical();

            switch (index)
            {
                case 0:
                    //0     普通                  One Zero, One Zero
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.One);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.Zero);
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.Zero);
                    break;
                case 1:
                    //1     Alpha混合             SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.SrcAlpha);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.OneMinusSrcAlpha);
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.OneMinusSrcAlpha);
                    break;
                case 2:
                    //2     叠加                  SrcAlpha One, One OneMinusSrcAlpha
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.SrcAlpha);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.One);
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.OneMinusSrcAlpha);
                    break;
                case 3:
                    //3     背景加深              Zero DstColor, One OneMinusSrcAlpha
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.Zero);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.DstColor);
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.OneMinusSrcAlpha);
                    break;
                case 4:
                    //4     叠加(忽略Alpha)       One One, One One
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.One);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.One);
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.One);
                    break;
                case 5:
                    //5     柔和叠加(忽略Alpha)   SrcColor One, One OneMinusSrcAlpha
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.SrcColor);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.One);
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.OneMinusSrcAlpha);
                    break;
                case 6:
                    //6     前景加深              Zero SrcColor, One OneMinusSrcAlpha
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.Zero);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.SrcColor);
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.OneMinusSrcAlpha);
                    break;
                case 7:
                    //7     Spine专用             One OneMinusSrcAlpha, One OneMinusSrcAlpha
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.One);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.OneMinusSrcAlpha);
                    _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                    _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.OneMinusSrcAlpha);
                    break;
                default:
                    break;
            }

        }

    }

}