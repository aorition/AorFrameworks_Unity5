using System.Collections.Generic;
using System.Reflection;
using Framework.Extends;
using UnityEngine;
using UnityEditor;
using System;

namespace Framework.Editor.Utility
{

    public class ScriptableObjectAssetCreator : UnityEditor.EditorWindow
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

        private static GUIStyle _itemLabelStyle;
        protected static GUIStyle itemLabelStyle
        {
            get
            {
                if (_itemLabelStyle == null)
                {
                    _itemLabelStyle = GUI.skin.GetStyle("button").Clone();
                    _itemLabelStyle.fontSize = 12;
                    _itemLabelStyle.fontStyle = FontStyle.Bold;
                    _itemLabelStyle.alignment = TextAnchor.MiddleLeft;
                    _itemLabelStyle.padding = new RectOffset(20, 20, 0, 0);
                    _itemLabelStyle.normal.textColor = Color.white;
                }
                return _itemLabelStyle;
            }
        }

        //--------

        private static ScriptableObjectAssetCreator _instance;

        [MenuItem("Assets/ScriptableObject数据文件生成器")]
        public static ScriptableObjectAssetCreator init()
        {
            _instance = EditorWindow.GetWindow<ScriptableObjectAssetCreator>();
            return _instance;
        }

        private static bool IsBaseClass(Type type, Type baseType)
        {
            if (type.IsInterface)
                return false;
            if (type == baseType)
                return true;
            while (type != typeof(object))
            {
                if (type == typeof(UnityEditor.Editor) || type == typeof(UnityEditor.EditorWindow)) return false;
                if (type == baseType)
                    return true;
                type = type.BaseType;
            }
            return false;
        }

//        private List<Type> _sbOTypeList = new List<Type>();

        private readonly List<string> _asmNameList = new List<string>();
        private readonly List<bool> _asmEnableList = new List<bool>();
        private readonly List<List<Type>> _sbOTypeLists = new List<List<Type>>();

        private static string[] _filterTypeKeys = new string[] { "Boo.", "Mono." , "System.", "Unity.", "UnityScript", "UnityEditor", "UnityEngine" };
        private static bool _filterNames(string name)
        {
            int i, len = _filterTypeKeys.Length;
            for (i = 0; i < len; i++)
            {
                if (name.StartsWith(_filterTypeKeys[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private void Awake()
        {
            //Assembly asm = Assembly.Load("Assembly-CSharp");
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in asms)
            {

                string asmName = asm.GetName().Name;
                string asmFullName = asm.FullName;
                if (_filterNames(asmFullName))
                {
                    _asmNameList.Add(asmName);
                    _asmEnableList.Add(asmName == "Assembly-CSharp");
                    _sbOTypeLists.Add(new List<Type>());
                    int index = _sbOTypeLists.Count - 1;
                    foreach (Type tt in asm.GetTypes())
                    {

                        if (IsBaseClass(tt, typeof(ScriptableObject)))
                        {
                            _sbOTypeLists[index].Add(tt);
                           // _sbOTypeList.Add(tt);
                        }

                    }
                }

            }

        }

        private Vector2 _scrollPos = new Vector2();
        private void OnGUI()
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            {
                GUILayout.Space(15);
                _draw_toolTitle_UI();
                GUILayout.Space(15);
                _draw_AsmSelect_UI();
                GUILayout.Space(15);
                _draw_savePath_UI();
                GUILayout.Space(15);
                _draw_Main_UI();
                GUILayout.Space(15);
            }
            GUILayout.EndScrollView();

        }

        private void _draw_toolTitle_UI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("      ScriptableObject资源创建器      ", titleStyle);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
            }
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
        
        private int asmToggleUIWidthDefine = 400;
        private Vector2 _draw_AsmSelect_scroll_pos = Vector2.zero;
        private void _draw_AsmSelect_UI() {
            GUILayout.BeginVertical("box");
            {

                GUILayout.Space(5);
                _draw_subTitle_UI("------ 程序集选择 ------");
                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("全部选择")) {
                        for (int i = 0; i < _asmEnableList.Count; i++)
                        {
                            _asmEnableList[i] = true;
                        }
                    }
                    if (GUILayout.Button("反向选择"))
                    {
                        for (int i = 0; i < _asmEnableList.Count; i++)
                        {
                            _asmEnableList[i] = !_asmEnableList[i];
                        }
                    }
                    if (GUILayout.Button("取消选择"))
                    {
                        for (int i = 0; i < _asmEnableList.Count; i++)
                        {
                            _asmEnableList[i] = false;
                        }
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                _draw_AsmSelect_scroll_pos = GUILayout.BeginScrollView(_draw_AsmSelect_scroll_pos, GUILayout.Height(320));
                {
                    GUILayout.Space(5);

                    int uNum = Screen.width / asmToggleUIWidthDefine;
                    uNum = uNum < 1 ? 1 : uNum;
                    for (int i = 0; i < _asmNameList.Count; i += uNum)
                    {

                        if (i > 0) GUILayout.Space(5);
                        GUILayout.BeginHorizontal();
                        {
                            for (int u = 0; u < uNum; u++)
                            {
                                int idx = i + u;
                                if (idx < _asmNameList.Count)
                                {
                                    _asmEnableList[idx] = EditorGUILayout.ToggleLeft(_asmNameList[idx], _asmEnableList[idx],GUILayout.Width(asmToggleUIWidthDefine));
                                }
                            }

                        }
                        GUILayout.EndHorizontal();

                    }

                    GUILayout.Space(5);
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();

            _typeList.Clear();
            for (int i = 0; i < _sbOTypeLists.Count; i++)
            {
                if (_asmEnableList[i])
                {
                    foreach (var item in _sbOTypeLists[i])
                    {
                        _typeList.Add(item);
                    }
                }
            }

        }

        private readonly List<Type> _typeList = new List<Type>();
        private Vector2 _draw_Main_UI_pos = Vector2.zero;
        private void _draw_Main_UI()
        {
            GUILayout.BeginVertical("box");
            {

                GUILayout.Space(5);
                _draw_subTitle_UI("------ 操作列表 ------");
                GUILayout.Space(5);

                _draw_Main_UI_pos = GUILayout.BeginScrollView(_draw_Main_UI_pos, GUILayout.Height(600));
                {
                    GUILayout.Space(5);

                    for (int i = 0; i < _typeList.Count; i++)
                    {
                        if (i > 0)
                        {
                            GUILayout.Space(5);
                        }

                        _draw_MainItem_UI(_typeList[i]);

                    }
                    GUILayout.Space(5);
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }

        private void _draw_MainItem_UI(Type type)
        {

            bool valid = true;
            string name = "uname" + type.Name;
            string savePath = string.Empty;
            if (!string.IsNullOrEmpty(_savePath))
            {
                savePath = _savePath + "/" + name + ".asset"; ;
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(savePath, type);
                if (obj)
                {
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }

            if (valid)
            {
                GUI.color = Color.yellow;
                if (GUILayout.Button(type.Name, itemLabelStyle, GUILayout.Height(32)))
                {
                    if (EditorUtility.DisplayDialog("提示", "确定在 " + _savePath + " 路径下创建 " + name + " 资源文件 ?", "确定", "取消"))
                    {
                        _createScriptableObject(savePath, type);
                    }
                }
                GUI.color = Color.white;
            }
            else
            {
                GUI.color = Color.gray;
                if (GUILayout.Button(type.Name, itemLabelStyle, GUILayout.Height(32)))
                {
                    if (string.IsNullOrEmpty(savePath))
                    {
                        EditorUtility.DisplayDialog("提示", "请先设置资源文件保存路径!", "确定");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("提示", "确定在 " + _savePath + " 路径下已经存在 " + name + " 资源文件,请先删除后再进行创建!", "确定");
                    }
                }
                GUI.color = Color.white;
            }
        }

        private void _createScriptableObject(string path, Type type)
        {

            ScriptableObject scriptable = ScriptableObject.CreateInstance(type);
            if (scriptable)
            {
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.CreateAsset(scriptable, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

        }

    }

}