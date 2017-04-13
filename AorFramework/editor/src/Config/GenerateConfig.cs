using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AorBaseUtility;

public class GenerateConfig : EditorWindow {
    //用于寻找配置表保存的数据
    Vector2 scrollPos;
    private static Dictionary<Assembly, List<Type>> dataDic = new Dictionary<Assembly, List<Type>>();
    private List<string> _cfgFileList;
    private string keyword = "";

    //特殊配置项类关联表（不同派生类共同关联一个基类，且配置项id互不重复）
    private static Dictionary<Type, Dictionary<Type, List<string>>> RelationalDataDic = 
        new Dictionary<Type, Dictionary<Type, List<string>>>();
    private static List<string> RelationalTypeNameList = new List<string>();

    [MenuItem("FrameworkTools/配置表管理/配置表编辑器")]
    static void AddWindow()
    {
        //创建窗口
        GetWindow<GenerateConfig>("已找到的Config");        
    }

    private static bool isInit;
    void Init()
    {
        //检查配置表所在目录是否存在
        if (!Directory.Exists(Application.dataPath + "/Resources/Config"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/Config");
        }

        List<string> pathList = GetPluginsDllFilePath();

        Assembly asm = Assembly.Load("Assembly-CSharp");
        if (!dataDic.ContainsKey(asm))
            dataDic.Add(asm, GetAssemblyConfigClassTypes(asm));

        foreach (string s in pathList)
        {
            asm = Assembly.LoadFile(s);
            if (!dataDic.ContainsKey(asm))
                dataDic.Add(asm, GetAssemblyConfigClassTypes(asm));
        }
        RelationalTypeNameList.Add("BattleSkillConfig");
        RelationalTypeNameList.Add("BattleEffectConfig");
        RelationalTypeNameList.Add("BulletConfig");
        InitRelationalDataDic();
        isInit = true;
    }
    void Awake()
    {
        if (!isInit)
        {
            Init();
        }
    }
    void OnFocus()
    {
        _cfgFileList = GetAllConfigFile();
    }

    //初始化特殊配置类关系数据表
    static void InitRelationalDataDic()
    {
        foreach (string s in RelationalTypeNameList)
        {
            Type type = GetType(s);
            if (type != null)
                AddRelationalData(type);
        }
    }

    static void AddRelationalData(Type targeType)
    {
        if (targeType == null) return;
        Dictionary<Type, List<string>> tempDic = new Dictionary<Type, List<string>>();
        foreach (KeyValuePair<Assembly, List<Type>> keyValuePair in dataDic)
        {
            foreach (Type types in keyValuePair.Value)
            {
                if (IsBaseClass(types, targeType) && !types.IsAbstract)
                {
                    List<List<string>> cfgInfo = ConfigEditorUI.GetCfgFileInfo(types);
                    if (cfgInfo != null)
                    {
                        string[] idList = new string[cfgInfo.Count - 3];
                        for (int i = 0; i < idList.Length; i++)
                        {
                            idList[i] = cfgInfo[i + 3][0];
                        }
                        tempDic.Add(types, idList.ToList());
                    }
                }
            }
        }
        RelationalDataDic.Add(targeType,tempDic);
    }

    internal static bool IsRelationalType(string typeName)
    {
        return RelationalTypeNameList.Contains(typeName);
    }
    internal static Type GetRelationalType(string BaseTypeName, string id)
    {
        if (!IsRelationalType(BaseTypeName))
            return null;
        foreach (KeyValuePair<Type, List<string>> kvp in RelationalDataDic[GetType(BaseTypeName)])
        {
            if (kvp.Value.Contains(id))
            {
                return kvp.Key;
            }
        }
        return null;
    }
    internal static Type GetRelationalBaseType(Type derivedType)
    {
        foreach (KeyValuePair<Type, Dictionary<Type, List<string>>> keyValuePair in RelationalDataDic)
        {
            if (keyValuePair.Value.ContainsKey(derivedType))
            {
                return keyValuePair.Key;
            }
        }
        return null;
    }
    internal static Type[] GetRelationalTypeArray(string BaseTypeName)
    {
        if (!IsRelationalType(BaseTypeName))
            return null;
        return RelationalDataDic[GetType(BaseTypeName)].Keys.ToArray();
    }

    internal static void UpdateRelationalDataDic()
    {
        RelationalDataDic.Clear();
        InitRelationalDataDic();
    }

    List<Type> GetAssemblyConfigClassTypes(Assembly asm)
    {
        List<Type> data = new List<Type>();
        foreach (Type tt in asm.GetTypes())
        {
            if (IsBaseClass(tt,typeof(Config)))
                data.Add(tt);
        }
        return data;
    }
    List<string> GetPluginsDllFilePath()
    {
        List<string> pathList = new List<string>();
        foreach (string s in Directory.GetFiles(Application.dataPath + "/Plugins"))
        {
            if (s.EndsWith(".dll"))
            {
                pathList.Add(s);
            }
        }
        return pathList;
    }

    List<string> GetAllConfigFile()
    {
        List<string> cfgList = new List<string>();
        string[] fileArr = Directory.GetFiles(Application.dataPath + "/Resources/Config", "*.txt", SearchOption.AllDirectories);

        for (int i = 0; i < fileArr.Length; i++)
            if (fileArr[i].EndsWith(".txt"))
                cfgList.Add(fileArr[i].Substring(fileArr[i].LastIndexOf('\\') + 1));

        return cfgList;        
    }

    //绘制窗口时调用
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.white;        
        GUILayout.Label("查找 : ", EditorStyles.largeLabel);
        keyword = EditorGUILayout.TextField("", keyword, "LargeTextField", GUILayout.Width(keyword.Length * 8 + 100));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.largeLabel);
        List<Assembly> asmList = new List<Assembly>(dataDic.Keys);

        for (int i = 0; i < asmList.Count; i++)
        {
            GUI.color = Color.white;

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("程序集名： " + asmList[i].GetName().Name, "WhiteLargeLabel");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            for (int j = 0; j < dataDic[asmList[i]].Count; j++)
            {               
                if (!dataDic[asmList[i]][j].IsAbstract && dataDic[asmList[i]][j].Name.ToLower().Contains(keyword))
                {
                    string path = Application.dataPath + "/Resources/Config/" + dataDic[asmList[i]][j].Name + ".txt";
                    GUI.color = Color.white;
                    EditorGUILayout.BeginHorizontal();
                    if (_cfgFileList.Contains(dataDic[asmList[i]][j].Name + ".txt"))
                    {
                        GUI.color = Color.cyan;
                        if (GUILayout.Button("修改", "LargeButton", GUILayout.Width(70)))
                        {
                            ConfigEditorUI ceui = CreateInstance<ConfigEditorUI>();
                            ceui.title = dataDic[asmList[i]][j].Name;
                            ceui.typePool = dataDic[asmList[i]][j];
                            ceui.Init();
                            ceui.Show();
                        }
                    }
                    else
                    {
                        GUI.color = Color.yellow;
                        if (GUILayout.Button("创建", "LargeButton", GUILayout.Width(70)))
                        {
                            ProduceConfig(path, dataDic[asmList[i]][j]);
                            UpdateRelationalDataDic();
                            OnFocus();
                        }
                    }                    
                    GUILayout.Space(20);
                    GUI.color = Color.yellow;
                    GUILayout.Label("配置类名： " + dataDic[asmList[i]][j], "LargeLabel");                    
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }            
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 生成空配置表文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="type"></param>
    void ProduceConfig(string path, Type type)
    {
        List<FieldInfo[]> fisList = new List<FieldInfo[]>();
        List<FieldInfo> fiList = new List<FieldInfo>();

        while (type != typeof(object))
        {
            fisList.Add(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));
            type = type.BaseType;
        }

        fisList.Reverse();
        foreach (FieldInfo[] fieldInfose in fisList)
        {
            foreach (FieldInfo fieldInfo in fieldInfose)
            {
                if (fieldInfo.IsInitOnly)
                    fiList.Add(fieldInfo);
            }
        }

        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

        StringBuilder strData = new StringBuilder();

        for (int i = 0; i < fiList.Count; i++)
        {
            strData.Append(RuntimeTypeChangeToTypeName(fiList[i].FieldType));
            strData.Append(i != fiList.Count - 1 ? "\t" : "\r\n");     
        }

        for (int i = 0; i < fiList.Count; i++)
        {
            Attribute[] a = Attribute.GetCustomAttributes(fiList[i]);
            bool isHasCommment = false;
            for (int j = 0; j < a.Length; j++)
            {
                if (a[j].GetType() == typeof(ConfigCommentAttribute))
                {
                    strData.Append(a[j].GetType().GetField("comment").GetValue(a[j]));
                    isHasCommment = true;
                    break;
                }
            }
            if (!isHasCommment)
            {
                strData.Append("注释" + (i + 1));
            }
            strData.Append(i != fiList.Count - 1 ? "\t" : "\r\n");
        }

        for (int i = 0; i < fiList.Count; i++)
        {
            strData.Append(fiList[i].Name);
            strData.Append(i != fiList.Count - 1 ? "\t" : "\r\n");
        }        

        sw.Write(strData.ToString());
//        Log.Debug("已生成空配置表" + type.Name + "!");
        sw.Close();
        fs.Close();

        AssetDatabase.Refresh();
    }

    static bool IsBaseClass(Type type, Type baseType)
    {
        if (type.IsInterface)
            return false;
        if (type == baseType)
            return false;
        while (type != typeof(object))
        {
            if (type == baseType)
                return true;
            type = type.BaseType;
        }
        return false;
    }

    //字段类型名转为配置表字段类型名
    public static string RuntimeTypeChangeToTypeName(Type t)
    {
        if (!t.IsArray)
            return !t.IsGenericType ? BaseTypeNameChange(t, "") : BaseTypeNameChange(t.GetGenericArguments()[0], "List");
        if (t.Name.Contains("[][]"))
            return BaseTypeNameChange(t.GetElementType().GetElementType(), "[][]");
        if (t.Name.Contains("[]"))
            return BaseTypeNameChange(t.GetElementType(), "[]");
        return null;
    }

    //基本数据类型别名转换为C#名
    private static string BaseTypeNameChange(Type t, string strsuff)
    {
        string s = null;
        switch (t.Name)
        {
            case "String": s = "string"; break;
            case "Int32": s = "int"; break;
            case "UInt32": s = "uint"; break;
            case "UInt64": s = "ulong"; break;
            case "Byte": s = "byte"; break;
            case "SByte": s = "sbyte"; break;
            case "Int16": s = "short"; break;
            case "Char": s = "char"; break;
            case "UInt16": s = "ushort"; break;
            case "Single": s = "float"; break;
            case "Boolean": s = "bool"; break;
            case "Double": s = "double"; break;
            case "Int64": s = "long"; break;
        }
        if (s != null) return s + strsuff;
        if (t.IsEnum) return "enum" + strsuff;
        if (IsBaseClass(t, typeof(Config))) return t.Name + strsuff;
        return null;
    }

    internal static Type GetType(string typeName)
    {
        if (dataDic == null) return null;
        List<List<Type>> llList = new List<List<Type>>(dataDic.Values);

        for (int i = 0; i < llList.Count; i++)        
            for (int j = 0; j < llList[i].Count; j++)            
                if (llList[i][j].Name == typeName)                
                    return llList[i][j];         
        
        return null;
    }
}