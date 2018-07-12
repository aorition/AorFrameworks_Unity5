using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using AorBaseUtility.MiniJSON;
using UnityEngine;

namespace Framework.JSON
{

    /// <summary>
    /// JSON解析器,基于MIniJSON的再封装.运行时支持JSON字符串到对象的反序列化.
    /// </summary>
    public static class JSONParser
    {


        private static readonly Regex JsonHeadTipReg = new Regex("//////>.*\n");

        /// <summary>
        /// 判断文本中是否包含JsonHeadTag
        /// </summary>
        public static bool CheckJsonHeadTagDefind(string src)
        {
            return JsonHeadTipReg.Match(src).Success;
        }

        /// <summary>
        /// 分离Json正文和JsonHeadTag
        /// </summary>
        /// <param name="src">带有JsonHeadTag的JSON</param>
        /// <param name="headTagContent">获取JsonHeadTag的容器</param>
        /// <returns>JSON正文</returns>
        public static string SplitJsonHeadTag(string src, ref string headTagContent)
        {
            Match m = JsonHeadTipReg.Match(src);
            if (m.Success)
            {
                headTagContent = m.Value.Replace("//////>", "").Replace("\n", "");
                string o = src.Replace(m.Value, "");
                return o;
            }
            else
            {
                return src;
            }
        }
        /// <summary>
        /// 分离Json正文和JsonHeadTag
        /// </summary>
        /// <param name="src">带有JsonHeadTag的JSON</param>
        /// <returns>JSON正文</returns>
        public static string SplitJsonHeadTag(string src)
        {
            Match m = JsonHeadTipReg.Match(src);
            if (m.Success)
            {
                string o = src.Replace(m.Value, "");
                return o;
            }
            else
            {
                return src;
            }
        }

        //=====---------------------------------------------------------

        public static Dictionary<string, object> DecodeToDic(string json)
        {
            return Json.DecodeToDic(json);
        }

        //================================== Json to object ======

        public static object ParseValue(Type type, object value)
        {
            if (value == null) return null;

            if (type.IsValueType)
            {
                //值类型
                if (type.IsEnum)
                {
                    //枚举
                    return string.IsNullOrEmpty(value.ToString()) ? Enum.GetNames(type).GetValue(0) : Enum.Parse(type, value.ToString());
                }
                else if (type.IsPrimitive)
                {
                    return _parseBaseData(type, value.ToString());
                }
                else
                {
                    return _parseStructOrClass(type, value);
                }

            }
            else
            {
                //引用类型
                if (type.IsArray)
                {
                    //数组
                    IList list = (IList)value;
                    Type subType = type.GetElementType();
                    Array ins = Array.CreateInstance(subType, list.Count);
                    int i, len = list.Count;
                    for (i = 0; i < len; i++)
                    {
                        ins.SetValue(ParseValue(subType, list[i]), i);
                    }
                    return ins;
                }
                else if (type.IsGenericType)
                {

                    if (type.GetInterface("IList") != null)
                    {
                        //List
                        IList list = (IList)value;
                        Type subType = type.GetElementType();
                        Type ttl = type.GetGenericArguments()[0];
                        Type tl = typeof(List<>).MakeGenericType(ttl);
                        IList ins = (IList)Activator.CreateInstance(tl);
                        int i, len = list.Count;
                        for (i = 0; i < len; i++)
                        {
                            ins.Add(ParseValue(ttl, list[i]));
                        }
                        return ins;
                    }
                    else if (type.GetInterface("IDictionary") != null)
                    {
                        //dictionary
                        if (value is IList)
                        {
                            IList list = (IList)value;

                            Type keyType = type.GetGenericArguments()[0];
                            Type valueType = type.GetGenericArguments()[1];
                            Type dicType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                            IDictionary ins = (IDictionary)Activator.CreateInstance(dicType);

                            int i, len = list.Count;
                            for (i = 0; i < len; i++)
                            {
                                Dictionary<string, object> sub = (Dictionary<string, object>)list[i];

                                if (sub != null && sub.Count > 0)
                                {
                                    foreach (string k in sub.Keys)
                                    {
                                        ins.Add(ParseValue(keyType, k), ParseValue(valueType, sub[k]));
                                    }
                                }
                            }
                            return ins;
                        }
                        else
                        {

                            IDictionary dic = (IDictionary)value;

                            Type keyType = type.GetGenericArguments()[0];
                            Type valueType = type.GetGenericArguments()[1];
                            Type dicType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                            IDictionary ins = (IDictionary)Activator.CreateInstance(dicType);

                            foreach (object key in dic.Keys)
                            {
                                ins.Add(ParseValue(keyType, key), ParseValue(valueType, dic[key]));
                            }
                            return ins;
                        }
                    }
                }else if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    //??? 未验证 条件是否正确
                    return _parseStructOrClass(type, value);
                }
                else
                {
                    //String 是引用
                    return _parseBaseData(type, value.ToString());
                }

            }
            return null;
        }

        private static object _parseStructOrClass(Type type, object data)
        {

            //其他定义的Struct , data必须是Dictionary<string, object>
            if (data is IDictionary)
            {
                Dictionary<string, object> dic = (Dictionary<string, object>)data;
//                if (type.IsSubclassOf(typeof (UnityEngine.Object)))
                if(type.IsSubclassOf(typeof(UnityEngine.ScriptableObject)) || !type.IsSubclassOf(typeof(UnityEngine.Object)))
                {

                    
                    if (dic != null && dic.Count > 0)
                    {
                        List<FieldInfo> FieldInfoList = new List<FieldInfo>();
                        //找Type的所有public字段
                        FieldInfo[] FieldInfos =
                            type.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
                        if (FieldInfos.Length > 0)
                        {
                            foreach (FieldInfo info in FieldInfos)
                            {
                                FieldInfoList.Add(info);
                            }
                        }
                        //找Type的NonPublic并且带有SerializeField标记的字段
                        FieldInfo[] _fieldInfos =
                            type.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);
                        if (_fieldInfos.Length > 0)
                        {
                            foreach (FieldInfo info in _fieldInfos)
                            {
                                bool vaild = false;
                                object[] attributes = info.GetCustomAttributes(true);
                                for (int i = 0; i < attributes.Length; i++)
                                {
                                    if (attributes[i] is SerializeField)
                                    {
                                        vaild = true;
                                        break;
                                    }
                                }
                                if (vaild)
                                {
                                    FieldInfoList.Add(info);
                                }
                            }
                        }
                        if (FieldInfoList.Count > 0)
                        {
                            object ins = Activator.CreateInstance(type);
                            foreach (FieldInfo info in FieldInfoList)
                            {
                                string fName = info.Name;
                                object fValue = ParseValue(info.FieldType, dic[fName]);
                                info.SetValue(ins, fValue);
                            }
                            return ins;
                        }
                        else
                        {
                            //Error 没有找到可以序列化的字段
                        }

                    }
                    else
                    {
                        //Error 数据错误
                    }
                }else // if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    int instanceId = int.Parse((dic["Value"] as Dictionary<string, object>)["instanceID"].ToString());
                    string subJson = "{\"Value\":{ \"instanceID\":" + instanceId + "}";
                    return JScriptableObjectDataWraper.CreateObjectFromJSON(subJson);
                }
            }
            else
            {
                //Erorr 未知数据.
            }

            return null;
        }
        private static object _parseBaseData(Type type, string data)
        {
            if (data.Equals(""))
                return type.Name.Equals("String") ? "" : type.Assembly.CreateInstance(type.FullName);

            try
            {
                switch (type.Name)
                {
                    case "String": return data;
                    case "Byte": return byte.Parse(data);
                    case "SByte": return sbyte.Parse(data);
                    case "Int16": return short.Parse(data);
                    case "UInt16": return ushort.Parse(data);
                    case "Int32": return int.Parse(data);
                    case "UInt32": return uint.Parse(data);
                    case "Int64": return long.Parse(data);
                    case "UInt64": return ulong.Parse(data);
                    case "Char": return char.Parse(data);
                    case "Boolean": return data.ToLower() == "true" || data == "1";
                    case "Single": return float.Parse(data);
                    case "Double": return double.Parse(data);
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }

    }
}
