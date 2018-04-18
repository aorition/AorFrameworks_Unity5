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
    /// 
    /// Author: Aorition
    /// 
    /// 
    /// 
    /// 须知:
    /// 
    ///     1.定义Shader描述格式:
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
    ///      ** PS(Properties端枚举定义的字段名须和)
    /// 
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Material))]
    public class MaterialInspectorBase : MaterialEditor
    {

        private static GUIStyle _descLabelStyle;
        protected static GUIStyle DescLabelStyle
        {
            get
            {
                if (_descLabelStyle == null)
                {
                    _descLabelStyle = GUI.skin.GetStyle("Label").Clone();
                    _descLabelStyle.fontSize = 16;
                    _descLabelStyle.wordWrap = true;
                }
                return _descLabelStyle;
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
            _shadercacheInit(targetMat.shader);
        }

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
            //获取Shader文本数据
            _shaderPath = AssetDatabase.GetAssetPath(_shaderCahce);

            //过滤内置Shader和无法找到其路径的shader
            if (string.IsNullOrEmpty(_shaderPath) || _shaderPath == "Resources/unity_builtin_extra") return;

            ShaderPropNameDefineDic.Clear();

            string path = Application.dataPath.Replace("Assets", "") + _shaderPath;
            _shaderCahceCode = AorIO.ReadStringFormFile(path);

            //统一结束符 (\r\n -> \n)
            _shaderCahceCode = _shaderCahceCode.Replace("\r\n", "\n");

            //获取描述
            Match InfoMatch = ShaderInfoRge.Match(_shaderCahceCode);
            if (InfoMatch.Success)
            {
                string inner = InfoMatch.Groups[1].Value;
                _shaderDescReadonly = inner.StartsWith("//<Readonly>");
                _shderDescription = _shaderDescReadonly ? inner.Substring(12) : inner.Substring(2);

                //防止获取的描述中包含\n 或者 \r\n
                _shderDescription = _shderDescription.Replace("\n", "");

                _shderDescriptionCache = _shderDescription;
            }

            //移除空格
            _shaderCahceCode = _shaderCahceCode.Replace(" ", "");

            //动态 RenderState 抓取 -----------

            //Zwirte
            Match zwirteMatch = zwirteEnumRegex.Match(_shaderCahceCode);
            if (zwirteMatch.Success)
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
            Match ztestMatch = ztestEnumRegex.Match(_shaderCahceCode);
            if (ztestMatch.Success)
            {
                _isDefine_Dynamic_ZTest = ztestMatch.Groups[2].Value.ToLower().Contains("ztest")
                                          && ztestMatch.Groups[3].Value.ToLower() == "float";
                ShaderPropNameDefineDic.Add("ZTest", ztestMatch.Groups[1].Value);
            }

            //Cull
            Match cullMatch = cullEnumRegex.Match(_shaderCahceCode);
            if (cullMatch.Success)
            {
                _isDefine_Dynamic_ZTest = cullMatch.Groups[2].Value.ToLower().Contains("cull")
                                          && cullMatch.Groups[3].Value.ToLower() == "float";
                ShaderPropNameDefineDic.Add("Cull", cullMatch.Groups[1].Value);
            }

            //Blend
            Match blendMatch = blendEnumRegex.Match(_shaderCahceCode);
            bool blendMatchLoop = blendMatch.Success;
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
                blendMatchLoop = blendMatch.Success;
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

            if(_isDynamicShader) OnDynamicShaderInit();
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

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //刷新
            if (_shaderCahce != targetMat.shader)
            {
                _targetMat = null;
                OnEnable();
            }

            GUILayout.Space(10);

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
                }
            }
            EditorGUILayout.EndToggleGroup();
            GUILayout.EndVertical();

            GUILayout.Space(10);

            //描述 (暂时不打算加入新建描述的功能,如果shader确实需要添加描述,请手动为shader添加描述识别字段)
            if (!string.IsNullOrEmpty(_shderDescriptionCache))
            {
                GUILayout.BeginVertical("box");
                GUILayout.Space(8);
                GUILayout.Label("Shader Description : ");
                GUILayout.Space(5);

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
                        if (GUILayout.Button("SaveChanged", GUILayout.Width(Screen.width*0.3f)))
                        {
                            //防止新输入描述中包含\n 或者 \r\n
                            _shderDescriptionCache = _shderDescriptionCache.Replace("\r\n", "");
                            _shderDescriptionCache = _shderDescriptionCache.Replace("\n", "");
                            //重写Shader文件
                           _ReBuildShaderCode(() =>
                           {
                               //重写成功后
                               _shderDescription = _shderDescriptionCache;
                           });
                        }
                    }
                    GUILayout.EndHorizontal();
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
                GUILayout.Label("DynamicShader Tools : ");
                GUILayout.Space(5);

                if (_isDefine_Dynamic_Blend)
                {
                    drawTag();
                    GUILayout.Space(10);
                }

                OnDrawExtendsInspectorGUI();

                GUILayout.Space(5);
                GUI.color = Color.white;
                GUILayout.EndVertical();
            }

        }

        protected void _ReBuildShaderCode(Action sucessDo)
        {
            Match infoMatch = ShaderInfoRge.Match(_shaderCahceCode);
            if (infoMatch.Success)
            {
                string rp = infoMatch.Value;
                rp = rp.Replace(infoMatch.Groups[1].Value, _shaderDescReadonly ? ("//<Readonly>" + _shderDescriptionCache) : ("//" + _shderDescriptionCache) + "\n");
                _shaderCahceCode = _shaderCahceCode.Replace(infoMatch.Value, rp);

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
        }

        public virtual void OnDrawExtendsInspectorGUI()
        {
            //
        }

        //Todo 这个UI有点丑.. 有空给改改
        private void drawTag()
        {

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("混合模式选单:");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("普通", GUILayout.Width(120)))
            {
                _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.One);
                _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.Zero);

                _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.Zero);

            }

            if (GUILayout.Button("Alpha混合", GUILayout.Width(120)))
            {
                _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.SrcAlpha);
                _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.OneMinusSrcAlpha);

                _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.OneMinusSrcAlpha);

            }
            if (GUILayout.Button("叠加", GUILayout.Width(120)))
            {
                _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.SrcAlpha);
                _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.One);

                _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.OneMinusSrcAlpha);

            }
            if (GUILayout.Button("背景加深", GUILayout.Width(120)))
            {
                _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.Zero);
                _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.DstColor);

                //alpha为覆盖模式
                _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.OneMinusSrcAlpha);

            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("叠加(忽略Alpha)", GUILayout.Width(120)))
            {
                _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.One);
                _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.One);

                _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.One);
            }

            if (GUILayout.Button("柔和叠加(忽略Alpha)", GUILayout.Width(120)))
            {
                _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.SrcColor);
                _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.One);

                _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.OneMinusSrcAlpha);


            }
            if (GUILayout.Button("前景加深", GUILayout.Width(120)))
            {
                _targetMat.SetInt(ShaderPropNameDefineDic["SrcBlend"], (int)BlendMode.Zero);
                _targetMat.SetInt(ShaderPropNameDefineDic["DstBlend"], (int)BlendMode.SrcColor);

                _targetMat.SetInt(ShaderPropNameDefineDic["SrcAlphaBlend"], (int)BlendMode.One);
                _targetMat.SetInt(ShaderPropNameDefineDic["DstAlphaBlend"], (int)BlendMode.OneMinusSrcAlpha);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            //        EditorUtility.SetDirty(target as Material);

        }

    }

}


