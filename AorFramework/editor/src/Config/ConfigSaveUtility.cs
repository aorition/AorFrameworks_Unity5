using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AorBaseUtility;
using UnityEngine;
using UnityEditor;
using AorFramework.core;

public class ConfigSaveUtility
{

    /// <summary>
    /// 定义配置数据文件的默认格式
    /// </summary>
    public static string defaultConfigAssetExt = ".txt";
    /// <summary>
    /// 定义配置数据文件的默认存放地址
    /// </summary>
    public static string defaultConfigFilesPath = "Assets/Resources/Config";

    /// <summary>
    /// 更新实例
    /// #由于配置类字段都为ReadOnly,所有更新字段的值必须使用此方法.
    /// #将需要更新的<字段名,值>以字典形式传入,实现数据更新
    /// </summary>
    /// <typeparam name="T">配置数据类</typeparam>
    /// <param name="cfgInst">配置数据实例</param>
    /// <param name="updateValues">需要更新的字段/值的字典集合</param>
    /// <returns>更新后的配置数据实例</returns>
    public static T updateConfigValues<T>(T cfgInst, Dictionary<string, object> updateValues)
    {
        Type t = cfgInst.GetType();
        foreach (KeyValuePair<string, object> kv in updateValues)
        {
            string fieldName = kv.Key;
            object value = kv.Value;
            Type vt = value.GetType();
            FieldInfo fi = t.GetField(fieldName);
            Type vft = fi.FieldType;
            fi.SetValue(cfgInst, value);
        }
        return cfgInst;
    }

    /// <summary>
    /// 从对应的CSV文件中获取所有配置表实例
    /// </summary>
    /// <typeparam name="T">配置数据类</typeparam>
    /// <returns>配置数据实例集合</returns>
    public static T[] getAllCongfigInst<T>() where T : TConfig
    {
        InitConfigManager<T>();

        T[] o = ConfigManager.Instance.GetConfigDic<T>();

        if (o != null)
        {
            //
        }

        ConfigManager.Instance.Destroy();
        return o;
    }


    /// <summary>
    /// 从对应的CSV文件中获取配置数据实例
    /// </summary>
    /// <typeparam name="T">配置数据类</typeparam>
    /// <param name="id">配置数据id</param>
    /// <returns>配置数据实例</returns>
    public static T getConfigInstance<T>(long id) where T : TConfig
    {

        string className = typeof(T).Name;
        //string configFilePath = defaultConfigFilesPath + className + defaultConfigAssetExt;
        string configFilePath = GetConfigFilePath<T>(defaultConfigFilesPath);
        InitConfigManager<T>();

        T ins = ConfigManager.Instance.GetConfigFromDic<T>(id);

        if (ins == null)
        {
            // Debug.LogWarning("ConfigSaveUtility.getConfigInstance Warning :: 无法加载在 " + configFilePath + " < " + id + " > 加载" + className + ", 生成新Config数据对象!");
            return null;
        }

        ConfigManager.Instance.Destroy();
        return ins;
    }

    /// <summary>
    /// 将实例数据集合到对应的CSV中.
    /// </summary>
    /// <typeparam name="T">配置数据类</typeparam>
    /// <param name="cfgInstList">配置数据实例集合</param>
    /// <param name="OverrideMode">是否覆盖所有既有数据,默认为false:只更新id相同的数据,添加新id的数据,但不会删除既有数据.为true则清除所有旧数据,并存储传入数据集合</param>
    /// <returns></returns>
    public static bool saveAllConfigToConfigSys<T>(T[] cfgInstList, bool OverrideMode = false) where T : TConfig
    {
        string className = typeof(T).Name;
        InitConfigManager<T>();

        if (cfgInstList != null && cfgInstList.Length > 0)
        {

            if (OverrideMode)
            {
                ConfigManager.Instance.ClearConfigDic<T>();
                foreach (T val in cfgInstList)
                {
                    ConfigManager.Instance.addConfigToDic(val);
                }
            }
            else
            {

                foreach (T val in cfgInstList)
                {
                    ConfigManager.Instance.removeConfigFromDic<T>(val.id);
                    ConfigManager.Instance.addConfigToDic(val);
                }
            }
            string expStr = ConfigManager.Instance.ExportStrInfo<T>();

            ConfigManager.Instance.Destroy();

            return SaveStrToFile(className, expStr);
        }

        return false;
    }

    /// <summary>
    /// 将实例数据保存到对应的CSV中.
    /// </summary>
    /// <typeparam name="T">配置数据类</typeparam>
    /// <param name="cfgInstace">配置数据实例</param>
    /// <returns>是否保存成功</returns>
    public static bool saveConfigToConfigSys<T>(T cfgInstace) where T : TConfig
    {

        string className = typeof(T).Name;
        InitConfigManager<T>();

        ConfigManager.Instance.removeConfigFromDic<T>(cfgInstace.id);
        ConfigManager.Instance.addConfigToDic(cfgInstace);

        string expStr = ConfigManager.Instance.ExportStrInfo<T>();

        ConfigManager.Instance.Destroy();

        return SaveStrToFile(className, expStr);
    }
    private static bool SaveStrToFile(string fileName, string fileStr)
    {
        // string projectPath = Application.dataPath + defaultConfigFilesPath.Replace("Assets", "") + fileName + defaultConfigAssetExt;
        string projectPath = GetConfigFilePath(fileName, defaultConfigFilesPath);
        if (string.IsNullOrEmpty(projectPath))
        {
            projectPath = Application.dataPath + defaultConfigFilesPath.Replace("Assets", "") + "/" + fileName + defaultConfigAssetExt;
        }
        return AorIO.SaveStringToFile(projectPath, fileStr);
    }

    private static void InitConfigManager<T>() where T : TConfig
    {
        if (ConfigManager.Instance == null)
        {
            ConfigManager.Reset();
        }

        string className;
        string configFilePath;
        Type[] types = ConfigManager.Instance.GetConfigTypePriorityArray(typeof(T));
        if (types != null)
        {

            for (int i = 0; i < types.Length; i++)
            {

                Type t = types[i];

                //                className = t.Name;
                //                configFilePath = defaultConfigFilesPath + className + defaultConfigAssetExt;

                configFilePath = GetConfigFilePath(t.Name, defaultConfigFilesPath);
                if (configFilePath == null)
                {
                    Debug.LogError("ConnfigSaveUtility.InitConfigManager Error : 配置文件在该目录不存在 :: " + defaultConfigFilesPath);
                    continue;
                }

                TextAsset configAsset = AssetDatabase.LoadAssetAtPath(configFilePath, typeof(TextAsset)) as TextAsset;
                if (configAsset != null)
                {
                    ConfigManager.Instance.ImportStrInfo(t, configAsset.text);
                }
                else
                {
                    Debug.LogError("ConnfigSaveUtility.InitConfigManager Error : 未能读取到配置相关文本内容 :: " + configFilePath);
                }

            }
        }

        className = typeof(T).Name;
        //configFilePath = defaultConfigFilesPath + className + defaultConfigAssetExt;
        configFilePath = GetConfigFilePath<T>(defaultConfigFilesPath);

        TextAsset ConfigAsset = AssetDatabase.LoadAssetAtPath(configFilePath, typeof(TextAsset)) as TextAsset;
        if (ConfigAsset != null)
        {
            ConfigManager.Instance.ImportStrInfo<T>(ConfigAsset.text);
        }
        else
        {
            //Debug.LogError("ConnfigSaveUtility.InitConfigManager Error : 未能读取到配置相关文本内容 :: " + configFilePath);
        }

    }

    //------------------------------

    /// <summary>
    /// 查找Config配置文件，搜索范围是path目录及其子目录的下所有文件
    /// ##文件名和类名须一致
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns>返回找到的第一个文件的路径,文件不存在返回null</returns>
    public static string GetConfigFilePath<T>(string path) where T : TConfig
    {
        return GetConfigFilePath(typeof(T).Name, path);
    }

    /// <summary>
    /// 查找Config配置文件，搜索范围是path目录及其子目录的下所有文件
    /// </summary>
    /// <param name="cfgType"></param>
    /// <param name="path"></param>
    /// <returns>返回找到的第一个文件的路径,文件不存在返回null</returns>
    public static string GetConfigFilePath(string fileName, string path)
    {
        if (Directory.Exists(path))
        {
            string[] fileArr = Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories);

            for (int i = 0; i < fileArr.Length; i++)
            {
                if (fileArr[i].Substring(fileArr[i].LastIndexOf('\\') + 1) == fileName + ".txt")
                    return fileArr[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 获取config配置文件信息
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static List<List<string>> GetCfgFileInfo(Type t)
    {
        string path = GetConfigFilePath(t.Name, defaultConfigFilesPath);
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

    public static List<List<string>> GetCfgFileInfo<T>()
    {
        return GetCfgFileInfo(typeof(T));
    }

    public static void SaveCfgFileInfo(Type t, List<List<string>> cfgFileInfo)
    {
        string filePath = GetConfigFilePath(t.Name, defaultConfigFilesPath);
        if (filePath == null)
        {
            EditorUtility.DisplayDialog("保存失败", "未能找到相关文件路径 \nFileName :: " + t.Name, "返回");
            return;
        }
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < cfgFileInfo.Count; i++)
        {
            for (int j = 0; j < cfgFileInfo[i].Count; j++)
            {
                sb.Append(j != cfgFileInfo[i].Count - 1
                    ? cfgFileInfo[i][j] + "\t"
                    : cfgFileInfo[i][j] + "\r\n");
            }
        }
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }

    public static void SaveCfgFileInfo<T>(List<List<string>> cfgFileInfo)
    {
        SaveCfgFileInfo(typeof(T), cfgFileInfo);
    }

    public static string[] AddNewValue<T>(Dictionary<string,string> newValue ,List<string> fieldName) where T:Config
    {
        string[] Value = new string[fieldName.Count];
        for (int i = 0; i < fieldName.Count; i++)
        {
            string str;
            if (newValue.TryGetValue(fieldName[i], out str))
            {
                Value[i] = str;
            }
            else Value[i] = "";
        }
        return Value;
    }
}
