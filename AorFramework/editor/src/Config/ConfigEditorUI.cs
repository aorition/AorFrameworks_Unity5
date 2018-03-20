using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AorBaseUtility;
using AorBaseUtility.Config;
using AorFramework.core;
using UnityEditor;
using UnityEngine;

public class ConfigEditorUI : EditorWindow
{
    internal Type typePool;
    internal List<List<string>> currentCfgFileInfo;

    private string keyword = "";
    private string targetId = "";
    private Vector2 scrollPos_top;
    private Vector2 scrollPos_main;

    //每个字段的基础类型数组
    private Type[] _fieldBaseTypes;
    //编辑字段的状态缓存数组
    private List<List<FieldState>> _fieldStatesCache;
    //编辑界面缓存对象，一次只能有一个存在
    private FieldState _editorCache;

    private class FieldState
    {
        public bool isEditor;
        public bool isNeedShowUtility;
        public int[] editorFlag;
    }
    //--------分页变量---------------
    private int currentPage = 1;
    private int pageSize = 10;
    private int maxPage = 1;
    private bool[] isShow;
    private bool[] isShowByKeys;
    internal void Init()
    {
        if (typePool == null)
        {
            Close();
            return;
        }
        currentCfgFileInfo = GetCfgFileInfo(typePool);
        TypeInit();
        StateInit();
    }

    internal void TypeInit()
    {
        _fieldBaseTypes = new Type[currentCfgFileInfo[2].Count];
        for (int i = 0; i < currentCfgFileInfo[2].Count; i++)
        {
            FieldInfo fi = typePool.GetField(currentCfgFileInfo[2][i]);
            Type t;
            if (fi == null) continue;
            t = fi.FieldType;
            if (t.Name.Contains("[][]"))
            {
                t = t.GetElementType().GetElementType();
            }
            if (t.Name.Contains("[]"))
            {
                t = t.GetElementType();
            }
            if (t.IsGenericType)
            {
                t = t.GetGenericArguments()[0];
            }
            _fieldBaseTypes[i] = t;
        }
    }
    internal void StateInit()
    {
        _fieldStatesCache = new List<List<FieldState>>();
        for (int i = 3; i < currentCfgFileInfo.Count; i++)
        {
            List<FieldState> tempList = new List<FieldState>();
            for (int j = 0; j < currentCfgFileInfo[i].Count; j++)
            {
                FieldState fs = new FieldState();
                fs.isEditor = false;
                fs.isNeedShowUtility = false;
                if (currentCfgFileInfo[0][j].Contains("[][]"))
                {
                    string[] str = currentCfgFileInfo[i][j].Split('|');
//                    int[] ls = new int[str.Length + 1];
//                    ls[0] = str.Length;
//                    for (int k = 0; k < str.Length; k++)
//                    {
//                        ls[k + 1] = str[k].Split('+').Length;
//                    }
//                    fs.editorFlag = ls;
                    fs.editorFlag = new[] {str.Length, str[0].Split('+').Length};
                }
                else if (currentCfgFileInfo[0][j].Contains("[]") || currentCfgFileInfo[0][j].EndsWith("List"))
                {
                    string[] str = currentCfgFileInfo[i][j].Split('+');
                    fs.editorFlag = new[] { str.Length };
                }
                else
                {
                    fs.editorFlag = new int[0];
                }
                tempList.Add(fs);
            }
            _fieldStatesCache.Add(tempList);
        }
        Screening(keyword);
        Paging(pageSize);
    }

    void OnGUI()
    {
        if (typePool == null)
            return;
        DrawListUI();
    }

    internal static List<List<string>> GetCfgFileInfo(Type t)
    {
        string path = ConfigSaveUtility.GetConfigFilePath(t.Name, ConfigSaveUtility.defaultConfigFilesPath);
        if (path == null)
        {
            return null;
        }
        TextAsset configAsset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;

        string[] str = configAsset.text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        List<List<string>> ss = new List<List<string>>();
        for (int i = 0; i < str.Length; i++)
        {
            ss.Add(str[i].Split('\t').ToList());
        }
        return ss;
    }
    void DrawListUI()
    {
        if (currentCfgFileInfo == null)
            return;

        string path = ConfigSaveUtility.GetConfigFilePath(typePool.Name, ConfigSaveUtility.defaultConfigFilesPath);
        if (path == null)
        {
            return;
        }
        EditorGUILayout.BeginVertical();
        TitleUI(path);
        TableUI();
        GUILayout.FlexibleSpace();
        PageUI();
        EditorGUILayout.EndVertical();
    }

    private void TitleUI(string path)
    {
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.white;
        GUILayout.Label("配置表名：", EditorStyles.largeLabel, GUILayout.Width(80));
        GUI.color = Color.cyan;
        GUILayout.Label(typePool.Name, EditorStyles.largeLabel, GUILayout.Width(500));
        GUI.color = Color.white;
        GUI.SetNextControlName("EditorButton");
        if (GUILayout.Button("返回", "LargeButton", GUILayout.Width(100)))
        {
            Close();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        //---------------------------------------------------------------------------------------------
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.white;
        GUILayout.Label("配置表地址：", EditorStyles.largeLabel, GUILayout.Width(80));
        GUI.color = Color.cyan;
        GUILayout.Label(path.Replace("\\", "/"), EditorStyles.largeLabel, GUILayout.Width(500));
        GUI.color = Color.white;
        if (GUILayout.Button("保存修改", "LargeButton", GUILayout.Width(100)))
        {
            if (EditorUtility.DisplayDialog("保存" + typePool.Name + "配置", "确认保存修改？这会覆盖原有的文件数据", "确认", "取消"))
            {
                string filePath = ConfigSaveUtility.GetConfigFilePath(typePool.Name, ConfigSaveUtility.defaultConfigFilesPath);
                if (filePath == null)
                {
                    EditorUtility.DisplayDialog("保存失败", "未能找到相关文件路径 \nFileName :: " + typePool.Name, "返回");
                    return;
                }
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < currentCfgFileInfo.Count; i++)
                {
                    for (int j = 0; j < currentCfgFileInfo[i].Count; j++)
                    {
                        sb.Append(j != currentCfgFileInfo[i].Count - 1
                            ? currentCfgFileInfo[i][j] + "\t"
                            : currentCfgFileInfo[i][j] + "\r\n");
                    }
                }
                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                AssetDatabase.Refresh();
                GenerateConfig.UpdateRelationalDataDic();
                if (Application.isPlaying)
                {
                    Type derivedType = GenerateConfig.GetRelationalBaseType(typePool);
                    if (derivedType != null)
                    {
                        ConfigManager.Instance.RefreshStrInfo(derivedType, typePool, sb.ToString());
                    }
                    else
                    {
                        ConfigManager.Instance.RefreshStrInfo(typePool, sb.ToString());
                    }
                }                
                Debug.Log("修改" + typePool.Name + "成功！");
                Close();
                return;
            }
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(20);
        //---------------------------------------------------------------------------------------------
        EditorGUILayout.BeginHorizontal();
        //创建按钮 
        GUI.color = Color.white;
        if (GUILayout.Button("创建新的配置", "LargeButton", GUILayout.Width(150)))
        {
            string[] idarrs = new string[currentCfgFileInfo.Count - 3];
            for (int i = 0; i < idarrs.Length; i++)
            {
                idarrs[i] = currentCfgFileInfo[i + 3][0];
            }
            CreateField.ceui = this;
            CreateField.idArr = idarrs;
            GetWindow<CreateField>("输入新的配置id");
        }
        GUILayout.Space(10);
        GUILayout.Label("复制目标配置数据id : ", EditorStyles.largeLabel);
        targetId = EditorGUILayout.TextField("", targetId, "LargeTextField", GUILayout.Width(targetId.Length * 8 + 100));
        if (GUILayout.Button("复制并创建", "LargeButton"))
        {
            int target = 3;
            if (targetId == "")
            {
                EditorUtility.DisplayDialog("创建失败", "id号不能为空！", "返回");
                return;
            }
            long n;
            if (!long.TryParse(targetId, out n))
            {
                EditorUtility.DisplayDialog("创建失败", "配置项id :: " + targetId + "格式错误！", "返回");
                return;
            }
            bool isExist = false;
            for (int i = 3; i < currentCfgFileInfo.Count; i++)
            {
                if (currentCfgFileInfo[i][0] == targetId)
                {
                    isExist = true;
                    target = i;
                    break;
                }
            }
            if (!isExist)
            {
                EditorUtility.DisplayDialog("创建失败", "配置项id :: " + targetId + "在配置表中不存在，无法复制数据！", "返回");
                return;
            }

            string[] idarrs = new string[currentCfgFileInfo.Count - 3];
            for (int i = 0; i < idarrs.Length; i++)
            {
                idarrs[i] = currentCfgFileInfo[i + 3][0];
            }
            CreateField.ceui = this;
            CreateField.idArr = idarrs;
            CreateField.targetId = target.ToString();
            GetWindow<CreateField>("输入新的配置id");
        }
        //清理字段
        GUILayout.Space(10);
        if (GUILayout.Button("清理不符合规范的字段", "LargeButton", GUILayout.Width(180)))
        {
            List<string> sList = new List<string>();

            for (int i = 0; i < currentCfgFileInfo[2].Count; i++)
                if (typePool.GetField(currentCfgFileInfo[2][i]) == null ||
                    GenerateConfig.RuntimeTypeChangeToTypeName(typePool.GetField(currentCfgFileInfo[2][i]).FieldType) != currentCfgFileInfo[0][i])
                    sList.Add(currentCfgFileInfo[2][i]);

            if (sList.Count == 0)
            {
                EditorUtility.DisplayDialog("清理完成", "所有字段都在该配置类中且符合规范，无需清理", "返回");
                return;
            }
            string clearTip = "";
            for (int i = 0; i < sList.Count; i++)
                clearTip += i != sList.Count - 1 ? sList[i] + "，" : sList[i];

            if (EditorUtility.DisplayDialog("清理字段",
                "字段" + clearTip + "在配置类" + typePool.Name + "中不存在或者字段类型与字段名不匹配，确认清理吗？这会清除该字段所有的配置数据",
                "确认", "取消"))
            {
                for (int i = 0; i < sList.Count; i++)
                {
                    for (int j = 0; j < currentCfgFileInfo[2].Count; j++)
                    {
                        if (currentCfgFileInfo[2][j] == sList[i])
                        {
                            for (int k = 0; k < currentCfgFileInfo.Count; k++)
                            {
                                currentCfgFileInfo[k].RemoveAt(j);
                            }
                        }
                    }
                }
                TypeInit();
                return;
            }
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(20);

        //查找
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("查找 : ", EditorStyles.largeLabel);
        keyword = EditorGUILayout.TextField("", keyword, "LargeTextField", GUILayout.Width(keyword.Length * 8 + 100));
        Screening(keyword);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }
    void TableUI()
    {
        scrollPos_top = EditorGUILayout.BeginScrollView(scrollPos_top, EditorStyles.largeLabel);

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();

        //字段处理按钮
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(50);
        if (GUILayout.Button("添加", GUILayout.Width(40)))
        {
            InsertField(0);
            return;
        }
        EditorGUILayout.EndHorizontal();

        //字段类型
        GUI.color = Color.cyan;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(50);
        GUILayout.Label(new GUIContent(currentCfgFileInfo[0][0], currentCfgFileInfo[0][0]), "WhiteLargeLabel", GUILayout.Width(70));
        EditorGUILayout.EndHorizontal();

        //描述
        GUI.color = Color.green;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(50);
        GUILayout.Label(new GUIContent(currentCfgFileInfo[1][0], currentCfgFileInfo[1][0]), "WhiteLargeLabel", GUILayout.Width(70));
        EditorGUILayout.EndHorizontal();

        //字段名
        GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(50);
        GUILayout.Label(new GUIContent(currentCfgFileInfo[2][0], currentCfgFileInfo[2][0]), "WhiteLargeLabel", GUILayout.Width(70));
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(13);

        for (int i = 3; i < currentCfgFileInfo.Count; i++)
        {
            if (isShow[i - 3])
            {
                GUI.color = Color.white;
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("删除", GUILayout.Width(45)))
                {
                    if (EditorUtility.DisplayDialog("确认删除", "确认删除配置" + currentCfgFileInfo[i][0] + "? 注意这会清除该行所有配置数据。",
                        "确认", "取消"))
                    {
                        currentCfgFileInfo.RemoveAt(i);
                        StateInit();
                        return;
                    }
                }
                GUI.color = Color.yellow;
                GUILayout.Button(currentCfgFileInfo[i][0], "WhiteLargeLabel");
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(9);
            }
        }

        EditorGUILayout.EndVertical();

        for (int i = 1; i < currentCfgFileInfo[0].Count; i++)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUI.color = Color.white;
            GUILayout.Space(8);
            if (GUILayout.Button("添加", GUILayout.Width(40)))
            {
                InsertField(i);
                return;
            }
            if (GUILayout.Button("删除", GUILayout.Width(40)))
            {
                if (EditorUtility.DisplayDialog("删除" + currentCfgFileInfo[2][i] + "字段", "确认删除？这会清除该字段所有的配置数据", "确认", "取消"))
                {
                    for (int j = 0; j < currentCfgFileInfo.Count; j++)
                    {
                        currentCfgFileInfo[j].RemoveAt(i);
                    }
                    StateInit();
                    TypeInit();
                    return;
                }
            }
            GUILayout.Space(8);
            EditorGUILayout.EndHorizontal();
            //字段类型
            GUI.color = Color.cyan;
            GUILayout.Label(new GUIContent(currentCfgFileInfo[0][i], currentCfgFileInfo[0][i]), "WhiteLargeLabel", GUILayout.Width(100));
            //描述
            GUI.color = Color.green;
            GUILayout.Label(new GUIContent(currentCfgFileInfo[1][i], currentCfgFileInfo[1][i]), "WhiteLargeLabel", GUILayout.Width(100));
            //字段名
            GUI.color = Color.white;
            GUILayout.Label(new GUIContent(currentCfgFileInfo[2][i], currentCfgFileInfo[2][i]), "WhiteLargeLabel", GUILayout.Width(100));
            GUILayout.Space(10);

            for (int j = 3; j < currentCfgFileInfo.Count; j++)
            {
                if (isShow[j - 3])
                {
                    DrawField(j, i);
                }
            }
            EditorGUILayout.EndVertical();
        }
        GUILayout.FlexibleSpace();
        GUILayout.Space(200);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(100);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }
    void PageUI()
    {
        //分页界面
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.white;
        GUILayout.Label("  分页数量：", EditorStyles.largeLabel);
        if (GUILayout.Button("1"))
        {
            Paging(1);
        }
        if (GUILayout.Button("5"))
        {
            Paging(5);
        }
        if (GUILayout.Button("10"))
        {
            Paging(10);
        }
        if (GUILayout.Button("20"))
        {
            Paging(20);
        }
        if (GUILayout.Button("不限"))
        {
            Paging(0);
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("上一页"))
        {
            Paging(pageSize, --currentPage);
        }
        if (GUILayout.Button("下一页"))
        {
            Paging(pageSize, ++currentPage);
        }
        GUILayout.Label("(", EditorStyles.largeLabel);

        string s = currentPage.ToString();
        s = GUILayout.TextField(s, "LargeTextField", GUILayout.Width(s.Length * 8 + 20));

        int n;
        if (int.TryParse(s, out n))
        {
            Paging(pageSize, int.Parse(s));
        }
        else
            Paging(pageSize);

        GUILayout.Label("/" + maxPage + ")", EditorStyles.largeLabel);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(20);
    }

    void DrawField(int i, int j)
    {
        GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal("HelpBox");
        if (_fieldBaseTypes[j] == null)
        {
            GUI.color = Color.red;
            GUILayout.Label("该字段须清理！", "WhiteLargeLabel",
                GUILayout.Width(100));
            EditorGUILayout.EndVertical();
            return;
        }
        if (_fieldStatesCache[i - 3][j].isEditor)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUI.color = Color.yellow;

            if (currentCfgFileInfo[0][j].Contains("bool[]") || currentCfgFileInfo[0][j] == "boolList")
            {
                currentCfgFileInfo[i][j] = EditorBoolData(currentCfgFileInfo[i][j]);
            }
            else
            {
                currentCfgFileInfo[i][j] = EditorData(currentCfgFileInfo[i][j], _fieldBaseTypes[j], 
                    IsConfigAsset(typePool.GetField(currentCfgFileInfo[2][j])));
            }         
            if (currentCfgFileInfo[0][j].Contains("[]") || currentCfgFileInfo[0][j].Contains("List"))
            {
                GUI.color = Color.yellow;
                _fieldStatesCache[i - 3][j].isNeedShowUtility =
                    EditorGUILayout.Foldout(_fieldStatesCache[i - 3][j].isNeedShowUtility, "");
                GUI.color = Color.white;
                if (GUILayout.Button("确定"))
                {
                    _fieldStatesCache[i - 3][j].isEditor = false;
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                if (_fieldStatesCache[i - 3][j].isNeedShowUtility)
                {
                    GUI.color = Color.yellow;
                    currentCfgFileInfo[i][j] = DrawEditorField(i, j);
                }
            }
            else
            {
                GUI.color = Color.white;
                if (GUILayout.Button("确定"))
                {
                    _fieldStatesCache[i - 3][j].isEditor = false;
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        else
        {
            if (GUILayout.Button(new GUIContent(currentCfgFileInfo[i][j], currentCfgFileInfo[i][j]), "WhiteLargeLabel", GUILayout.Width(100)))
            {
                if (_editorCache != null)
                {
                    _editorCache.isEditor = false;
                }
                _editorCache = _fieldStatesCache[i - 3][j];
                _editorCache.isEditor = true;
                GUI.FocusControl("EditorButton");
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    //分页
    internal void Paging(int size, int page = 1)
    {
        if (size == 0)
        {
            isShow = new bool[currentCfgFileInfo.Count - 3];
            for (int i = 0; i < isShow.Length; i++)
            {
                isShow[i] = isShowByKeys[i];
            }
            currentPage = 1;
            pageSize = 0;
            maxPage = 1;
        }
        else
        {
            isShow = new bool[currentCfgFileInfo.Count - 3];
            for (int i = 0; i < isShow.Length; i++)
            {
                isShow[i] = false;
            }
            List<int> pageFlaglists = new List<int>();
            for (int i = 0; i < isShowByKeys.Length; i++)
            {
                if (isShowByKeys[i])
                    pageFlaglists.Add(i);
            }
            maxPage = pageFlaglists.Count % size == 0 ? pageFlaglists.Count / size : pageFlaglists.Count / size + 1;
            if (page < 1)
                page = 1;
            if (page > maxPage)
                page = maxPage;

            for (int i = 0; i < pageFlaglists.Count; i++)
            {
                isShow[pageFlaglists[i]] = i >= (page - 1) * size && i <= page * size - 1;
            }
            currentPage = page;
            pageSize = size;
        }
    }

    void Screening(string keys)
    {
        isShowByKeys = new bool[currentCfgFileInfo.Count - 3];
        for (int i = 0; i < isShowByKeys.Length; i++)
        {
            bool flag = false;
            for (int j = 0; j < currentCfgFileInfo[0].Count; j++)
            {
                if (currentCfgFileInfo[i + 3][j].ToLower().Contains(keys))
                {
                    flag = true;
                    break;
                }
            }
            isShowByKeys[i] = flag;
        }
    }
    string EditorData(string data, Type fieldType, bool isConfigAsset = false)
    {
        if(fieldType.Name != "Boolean")
        {
            data = EditorGUILayout.TextField("", data, "LargeTextField", GUILayout.Width(data.Length * 8 + 50));  
        }
        if (data.Contains("+") || data.Contains("|"))
            return data;
        GUILayout.Space(10);
        //如果是枚举类型就显示下拉选择按钮
        if (fieldType.IsEnum)
        {
            string[] es = Enum.GetNames(fieldType);
            int ind = 0;
            if (data != "")
            {
                for (int i = 0; i < es.Length; i++)
                {
                    if (data == es[i])
                    {
                        ind = i;
                        break;
                    }
                }
                ind = EditorGUILayout.Popup("", ind, es, "LargePopup", GUILayout.Width(es[ind].Length * 8 + 20));
                data = es[ind];
            }
        }
        //如果是Asset特性就显示获取路径按钮
        else if (isConfigAsset)
        {
            data = GetResoucesPath(data);
        }
        //如果是bool类型就显示开关按钮
        else if (fieldType.Name == "Boolean")
        {
            bool bdata = data == "1" || data.ToLower() == "true";
            bdata = EditorGUILayout.ToggleLeft("", bdata);
            if (data.ToLower() != "true" && data.ToLower() != "false")
                data = bdata ? "1" : "0";
            else
                data = bdata ? "TRUE" : "FALSE";
        }

        //如果是自定义类型就显示点击转移配置类型按钮
        else if (IsConfigType(fieldType.Name))
        {
            DrawGoToButton(fieldType.Name, data);
        }

        //如果是有特殊关联的自定义类型就显示点击转移配置类型按钮
        else if (GenerateConfig.IsRelationalType(fieldType.Name))
        {
            DrawGoToSpecialButton(fieldType.Name, data);
        }
        return data;
    }

    bool IsConfigAsset(FieldInfo fi)
    {
        if (fi.FieldType.Name != "String")
        {
            return false;
        }
        Attribute[] a = Attribute.GetCustomAttributes(fi);
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i].GetType() == typeof(ConfigAssetAttribute) || a[i].GetType() == typeof(ConfigPathAttribute))
            {
                return true;
            }
        }
        return false;
    }

    string EditorBoolData(string data)
    {
        data = EditorGUILayout.TextField("", data, "LargeTextField", GUILayout.Width(data.Length * 8 + 50));  
        if (data.Contains("+") || data.Contains("|"))
            return data;
        GUILayout.Space(10);
        if (data != "")
        {
            bool bdata = data == "1" || data.ToLower() == "true";
            bdata = EditorGUILayout.ToggleLeft("", bdata);
            if (data.ToLower() != "true" && data.ToLower() != "false")
                data = bdata ? "1" : "0";
            else
                data = bdata ? "TRUE" : "FALSE";
        }
        return data;
    }

    string GetResoucesPath(string path)
    {
        if (GUILayout.Button("获取资源路径"))
        {
            path = EditorUtility.OpenFilePanel("获取资源路径(## 注意：只有在Resources文件夹下的路径相关的配置参数适用，获取的路径会转成相对路径)", Application.dataPath + "/Resources", "");
            path = path.Replace(Application.dataPath + "/Resources/", "");
            int index = path.LastIndexOf(".");
            return index > 0 ? path.Substring(0, index) : path;
        }
        return path;
    }

    void DrawGoToSpecialButton(string typeName, string data)
    {
        long n;
        if (data != "" && long.TryParse(data, out n))
        {
            Type type = GenerateConfig.GetRelationalType(typeName, data);
            if (type != null)
            {
                string[][] temp = GetCfgFileInfo(type.Name);
                int index = 0;
                for (int i = 3; i < temp.Length; i++)
                {
                    if (temp[i][0] == data)
                    {
                        index = i;
                        break;
                    }
                }
                GUI.color = Color.cyan;
                if (GUILayout.Button(new GUIContent(" 跳转 ", "类型：" + type.Name)))
                {
                    ConfigEditorUI ceui = CreateInstance<ConfigEditorUI>();
                    ceui.title = type.Name;
                    ceui.typePool = type;
                    ceui.Init();
                    ceui.Paging(1, index - 2);
                    ceui.Show();
                }
            }
            else
            {
                GUI.color = Color.yellow;
                if (GUILayout.Button(" 创建 "))
                {
                    CreateSpecialField.baseTypeName = typeName;
                    CreateSpecialField.targetId = data;
                    GetWindow<CreateSpecialField>("创建派生类配置");
                }
            }
        }
        else if(data != "")
        {
            GUI.color = Color.red;
            GUILayout.Label("id格式错误", EditorStyles.largeLabel);
        }
    }
    void DrawGoToButton(string typeName, string data)
    {
        long n;
        if (data != "" && long.TryParse(data, out n))
        {
            string[][] temp = GetCfgFileInfo(typeName);
            Type tempt = GenerateConfig.GetType(typeName);
            if (temp == null || tempt == null)
            {
                return;
            }
            bool isCanUpdate = false;
            int index = 0;
            for (int i = 3; i < temp.Length; i++)
            {
                if (temp[i][0] == data)
                {
                    index = i;
                    isCanUpdate = true;
                    break;
                }
            }
            if (!isCanUpdate)
            {
                GUI.color = Color.yellow;
                if (GUILayout.Button(new GUIContent(" 创建 ", "类型：" + tempt.Name)))
                {
                    ConfigEditorUI ceui = CreateInstance<ConfigEditorUI>();
                    ceui.title = tempt.Name;
                    ceui.typePool = tempt;
                    ceui.Init();
                    string[] newStrs = new string[ceui.currentCfgFileInfo[0].Count];
                    newStrs[0] = data;
                    for (int i = 1; i < newStrs.Length; i++)
                    {
                        newStrs[i] = "";
                    }
                    ceui.currentCfgFileInfo.Add(newStrs.ToList());
                    ceui.StateInit();
                    ceui.Paging(1, currentCfgFileInfo.Count - 2);
                    ceui.Show();
                }
            }
            else
            {
                GUI.color = Color.cyan;
                if (GUILayout.Button(new GUIContent(" 跳转 ", "类型：" + tempt.Name)))
                {
                    ConfigEditorUI ceui = CreateInstance<ConfigEditorUI>();
                    ceui.title = tempt.Name;
                    ceui.typePool = tempt;
                    ceui.Init();
                    ceui.Paging(1, index - 2);
                    ceui.Show();
                }
            }
        }
        else
        {
            GUI.color = Color.red;
            GUILayout.Label("id格式错误", EditorStyles.largeLabel);
        }
    }
    bool IsConfigType(string typeName)
    {
        return ConfigSaveUtility.GetConfigFilePath(typeName, ConfigSaveUtility.defaultConfigFilesPath) == null ? false : true;
    }

    int StrToIndex(string s)
    {
        int n;
        if (int.TryParse(s, out n))
        {
            return int.Parse(s);
        }
        return 0;
    }
    string DrawEditorField(int x, int y)
    {
        Type baseType = _fieldBaseTypes[y];
        string data = currentCfgFileInfo[x][y];
        string type = currentCfgFileInfo[0][y];
        FieldState fs = _fieldStatesCache[x - 3][y];
        if (type.Contains("[][]"))
        {
            string[] ss = data.Split('|');
            string[][] str2 = new string[ss.Length][];
            for (int i = 0; i < ss.Length; i++)
            {
                str2[i] = ss[i].Split('+');
            }

            fs.editorFlag[0] = str2.Length;
            fs.editorFlag[1] = str2[0].Length;

            string height = str2.Length.ToString();
            string width = str2[0].Length.ToString();

            EditorGUILayout.BeginVertical();
            //------------------------------------------------------------------------------------
            EditorGUILayout.BeginHorizontal();
            GUI.color = Color.yellow;
            GUILayout.Label("数组高度 : ", EditorStyles.largeLabel);
            GUI.color = Color.cyan;
            height = EditorGUILayout.TextField("", height, "LargeTextField", GUILayout.Width(30));

            GUILayout.Space(10);
            if (GUILayout.Button("+"))
            {
                height = (StrToIndex(height) + 1).ToString();
            }
            if (GUILayout.Button("—"))
            {
                height = (StrToIndex(height) - 1).ToString();
            }

            GUILayout.Space(30);
            GUI.color = Color.yellow;
            GUILayout.Label("数组宽度 : ", EditorStyles.largeLabel);
            GUI.color = Color.cyan;
            width = EditorGUILayout.TextField("", width, "LargeTextField", GUILayout.Width(30));

            GUILayout.Space(10);
            if (GUILayout.Button("+"))
            {
                width = (StrToIndex(width) + 1).ToString();
            }
            if (GUILayout.Button("—"))
            {
                width = (StrToIndex(width) - 1).ToString();
            }
            GUILayout.Space(10);

            GUI.color = Color.white;
            GUILayout.Label("注意 ： 填入的数据不要带有\"+\"或者\"|\"字符  ", EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
            //------------------------------------------------------------------------------------

            StringBuilder newData = new StringBuilder();
            string[,] editStr = new string[StrToIndex(height), StrToIndex(width)];            
            for (int i = 0; i < StrToIndex(height); i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < StrToIndex(width); j++)
                {
                    if (i < str2.Length && j < str2[i].Length)
                        editStr[i, j] = str2[i][j];
                    else
                        editStr[i, j] = "";
                    
                    GUI.color = Color.white;
                    EditorGUILayout.BeginVertical();
                    editStr[i, j] = EditorData(editStr[i, j], baseType);
                    EditorGUILayout.EndVertical();
                    GUI.color = Color.white;

                    newData.Append(editStr[i, j]);

                    if (j < StrToIndex(width) - 1)
                    {
                        GUILayout.Label("+", EditorStyles.largeLabel);
                        newData.Append("+");
                    }
                }
                if (i < StrToIndex(height) - 1)
                {
                    GUILayout.Label("|", EditorStyles.largeLabel);
                    newData.Append("|");
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(20);
            EditorGUILayout.EndVertical();
            //------------------------------------------------------------------------------------
            return newData.ToString();
        }
        if (type.Contains("[]") || type.EndsWith("List"))
        {
            string[] str1 = data.Split('+');

            fs.editorFlag[0] = str1.Length;

            string height = str1.Length.ToString();

            EditorGUILayout.BeginVertical();
            //---------------------------------------------------------------------
            EditorGUILayout.BeginHorizontal();
            GUI.color = Color.yellow;
            GUILayout.Label("数组长度 : ", EditorStyles.largeLabel);
            GUI.color = Color.cyan;
            height = EditorGUILayout.TextField("", height, "LargeTextField", GUILayout.Width(30));

            GUILayout.Space(10);
            if (GUILayout.Button("+"))
            {
                height = (StrToIndex(height) + 1).ToString();
            }
            if (GUILayout.Button("—"))
            {
                if (StrToIndex(height) > 0)
                    height = (StrToIndex(height) - 1).ToString();
            }
            GUILayout.Space(10);

            GUI.color = Color.white;
            GUILayout.Label("注意 ： 填入的数据不要带有\"+\"字符  ", EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            //---------------------------------------------------------------------
            string[] editStr = new string[StrToIndex(height)];
            //---------------------------------------------------------------------
            for (int i = 0; i < StrToIndex(height); i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (i < str1.Length)
                {
                    editStr[i] = str1[i];
                }
                else
                    editStr[i] = "";

                GUI.color = Color.white;
                editStr[i] = EditorData(editStr[i], baseType);
                GUI.color = Color.white;

                if (i < StrToIndex(height) - 1)
                    GUILayout.Label("+", EditorStyles.largeLabel);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            //---------------------------------------------------------------------
            GUILayout.Space(20);
            EditorGUILayout.EndVertical();

            StringBuilder newData = new StringBuilder();
            for (int i = 0; i < StrToIndex(height); i++)
            {
                newData.Append(editStr[i]);
                if (i < StrToIndex(height) - 1)
                    newData.Append("+");
            }
            return newData.ToString();
        }
        return data;
    }
    static string[][] GetCfgFileInfo(string typeName)
    {
        string path = ConfigSaveUtility.GetConfigFilePath(typeName, ConfigSaveUtility.defaultConfigFilesPath);
        if (path == null)
        {
            EditorUtility.DisplayDialog("配置不存在！", "未能找到相关配置文件路径 \nFileName :: " + typeName, "返回");
            return null;
        }

        TextAsset configAsset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;

        string[] str = configAsset.text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        string[][] ss = new string[str.Length][];
        for (int i = 0; i < str.Length; i++)
        {
            ss[i] = str[i].Split('\t');
        }
        return ss;
    }

    void InsertField(int index)
    {
        FieldInfo[] fis = typePool.GetFields();
        List<string> sList = new List<string>();
        for (int i = 0; i < fis.Length; i++)
        {
            if (!currentCfgFileInfo[2].Contains(fis[i].Name))
            {
                sList.Add(fis[i].Name);
            }
        }
        if (sList.Count > 0)
        {
            ConfigFieldEditor.Index = index + 1;
            ConfigFieldEditor.ceui = this;
            ConfigFieldEditor.efs = sList.ToArray();
            GetWindow<ConfigFieldEditor>("添加新的字段");
        }
        else
        {
            EditorUtility.DisplayDialog("添加失败", "所有字段都在该配置表中，无需添加新的字段", "返回");
        }
    }
}
