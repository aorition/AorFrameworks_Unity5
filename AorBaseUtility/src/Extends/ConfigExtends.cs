using System;
using System.Collections.Generic;
using System.Reflection;
using AorBaseUtility;

public static class ConfigExtends
{

    /// <summary>
    /// 更新实例
    /// #由于配置类字段都为ReadOnly,所有更新字段的值必须使用此方法.
    /// #将需要更新的<字段名,值>以字典形式传入,实现数据更新
    /// </summary>
    /// <typeparam name="T">配置数据类</typeparam>
    /// <param name="cfgInst">配置数据实例</param>
    /// <param name="updateValues">需要更新的字段/值的字典集合</param>
    /// <returns>更新后的配置数据实例</returns>
    public static T updateConfigValues<T>(this T cfgInst, Dictionary<string, object> updateValues) where T : Config
    {
        Type t = cfgInst.GetType();
        foreach (KeyValuePair<string, object> kv in updateValues)
        {
            string fieldName = kv.Key;
            object value = kv.Value;
            FieldInfo fi = t.GetField(fieldName);
            if (fi != null)
            {
                Type vt = value.GetType();
                Type vft = fi.FieldType;
                if (vt.FullName == vft.FullName)
                {
                    fi.SetValue(cfgInst, value);
                }
            }
        }
        return cfgInst;
    }

    /// <summary>
    /// 更新实例
    /// #由于配置类字段都为ReadOnly,所有更新字段的值必须使用此方法.
    /// </summary>
    /// <typeparam name="T">配置数据类</typeparam>
    /// <param name="cfgInst">配置数据实例</param>
    /// <param name="key">需要更新的字段</param>
    /// <param name="value">新值</param>
    /// <returns>更新后的配置数据实例</returns>
    public static T updateConfigValue<T>(this T cfgInst, string key, object value) where T : Config
    {
        Type t = cfgInst.GetType();
        FieldInfo fi = t.GetField(key);
        if (fi != null)
        {
            Type vft = fi.FieldType;
            Type vt = value.GetType();
            if (vt.FullName == vft.FullName)
            {
                fi.SetValue(cfgInst, value);
            }
        }
        return cfgInst;
    }

}
