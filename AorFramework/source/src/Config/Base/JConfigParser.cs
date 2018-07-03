using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AorBaseUtility.Config;
using AorBaseUtility.MiniJSON;
using UnityEngine;

namespace Framework.core
{
    
    public class JConfigParmsAttribute : Attribute{}

    /// <summary>
    /// JSON / Config 互转
    /// </summary>
    public class JConfigParser
    {

        #region Obsolete
        //----------------------------------------------------------------

        [Obsolete("Using ToConfig(Type type, Dictionary<string, object> jsonDic)")]
        public static Config _toConfig(Type type, Dictionary<string, object> jsonDic)
        {
            return ToConfig(type, jsonDic);
        }

        [Obsolete("Using WirteJsonHeadTag(string headTagContent)")]
        public static string wirteJsonHeadTag(string headTagContent)
        {
            return WirteJsonHeadTag(headTagContent);
        }
        [Obsolete("Using SplitJsonHeadTag(string src, ref string headTagContent)")]
        public static string splitJsonHeadTag(string src, ref string headTagContent)
        {
            return SplitJsonHeadTag(src, ref headTagContent);
        }
        [Obsolete("Using SplitJsonHeadTag(string src)")]
        public static string splitJsonHeadTag(string src)
        {
            return SplitJsonHeadTag(src);
        }
        [Obsolete("Using ParseValue(Type type, object value)")]
        public static object _parseVaule(Type type, object value)
        {
            return ParseValue(type, value);
        }
        //----------------------------------------------------------------
        #endregion

        private static readonly Regex JsonHeadTipReg = new Regex("//////>.*\n");

        public static string WirteJsonHeadTag(string headTagContent)
        {
            return "//////>" + headTagContent + "\n";
        }

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

        public static T ToConfig<T>(string src) where T : Config
        {
            return ToConfig(src, typeof (T)) as T;
        }

        public static Config ToConfig(string src, Type type)
        {
            string headTagContent = null;
            string jsonStr = splitJsonHeadTag(src, ref headTagContent);

            Dictionary<string, object> jsonDic = Json.DecodeToDic(jsonStr);

            if (jsonDic != null && jsonDic.Count > 0)
            {
                return ToConfig(type, jsonDic);
            }

            return null;
        }

        public static Config ToConfig(Type type, Dictionary<string, object> jsonDic)
        {

            if (jsonDic != null && jsonDic.Count > 0)
            {
                Config inst = type.Assembly.CreateInstance(type.FullName) as Config;
                
                foreach (string key in jsonDic.Keys)
                {

  //                  FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance| BindingFlags.NonPublic| BindingFlags.Public| BindingFlags.GetField);

                    FieldInfo fieldInfo = type.GetField(key, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField);
                    if (fieldInfo != null)
                    {

                        Type fieldType = fieldInfo.FieldType;
                        fieldInfo.SetValue(inst, ParseValue(fieldType, jsonDic[key]));
                    }
                }

                return inst;

            }

            return null;
        }
        
        [Obsolete("using JSONParser.ParseValue")]
        public static object ParseValue(Type type, object value)
        {
            if (value == null) return null;

            if (type.IsArray)
            {
                //数组
                IList list = (IList) value;
                Type subType = type.GetElementType();
                Array ins = Array.CreateInstance(subType, list.Count);
                int i, len = list.Count;
                for (i = 0; i < len; i++)
                {
                    ins.SetValue(ParseValue(subType, list[i]), i);
                }
                return ins;
            }
            else if (type.IsEnum)
            {
                //枚举
                return string.IsNullOrEmpty(value.ToString()) ? Enum.GetNames(type).GetValue(0) : Enum.Parse(type, value.ToString());
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
                            Dictionary<string, object> sub = (Dictionary<string, object>) list[i];

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

                        IDictionary dic = (IDictionary) value;

                        Type keyType = type.GetGenericArguments()[0];
                        Type valueType = type.GetGenericArguments()[1];
                        Type dicType = typeof (Dictionary<,>).MakeGenericType(keyType, valueType);
                        IDictionary ins = (IDictionary) Activator.CreateInstance(dicType);

                        foreach (object key in dic.Keys)
                        {
                            ins.Add(ParseValue(keyType, key), ParseValue(valueType, dic[key]));
                        }
                        return ins;
                    }
                }

            }
            else if (type.IsAssignableFrom(typeof (Config)))
            {
                //其他Config类
                Dictionary<string, object> dic = (Dictionary<string, object>) value;
                return ToConfig(type, dic);
            }
            else
            {
                return _parseBaseData(type, value.ToString());
            }
            return null;
        }
        [Obsolete("using JSONParser._parseBaseData")]
        private static object _parseBaseData(Type type, string data)
        {
            if (data.Equals(""))
                return type.Name.Equals("String") ? "" : type.Assembly.CreateInstance(type.FullName);

            if (type == typeof (Vector2))
            {
                string sd = data.Substring(1, data.Length - 2);
                string[] v = sd.Split(',');
                return new Vector2(float.Parse(v[0]), float.Parse(v[1]));
            }

            if (type == typeof(Vector3))
            {
                string sd = data.Substring(1, data.Length - 2);
                string[] v = sd.Split(',');
                return new Vector3(float.Parse(v[0]), float.Parse(v[1]), float.Parse(v[2]));
            }

            if (type == typeof(Vector4))
            {
                string sd = data.Substring(1, data.Length - 2);
                string[] v = sd.Split(',');
                return new Vector4(float.Parse(v[0]), float.Parse(v[1]), float.Parse(v[2]), float.Parse(v[3]));
            }

            if (type == typeof(Rect))
            {
                string sd = data.Substring(1, data.Length - 2);
                string[] v = sd.Split(',');
                return new Rect(float.Parse(v[0]), float.Parse(v[1]), float.Parse(v[2]), float.Parse(v[3]));
            }

            try
            {
                switch (type.Name)
                {
                    case "String":
                        return data;
                    case "Byte":
                        return byte.Parse(data);
                    case "SByte":
                        return sbyte.Parse(data);
                    case "Int16":
                        {
                            return short.Parse(data);
                        }
                    case "UInt16":
                        {
                            return ushort.Parse(data);
                        }
                    case "Int32":
                        {
                            return int.Parse(data);
                        }
                    case "UInt32":
                        {
                            return uint.Parse(data);
                        }
                    case "Int64":
                        {
                            return long.Parse(data);
                        }
                    case "UInt64":
                        {
                            return ulong.Parse(data);
                        }
                    case "Char":
                        return char.Parse(data);
                    case "Boolean":
                        return data.ToLower() == "true" || data == "1";
                    case "Single":
                        {
                            return float.Parse(data);
                        }
                    case "Double":
                        {
                            return double.Parse(data);
                        }
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }

        //---------------------

        public static string ToJSON(Config config)
        {

            StringBuilder _string = new StringBuilder();
            Type type = config.GetType();
            string FullClassName = type.Assembly.ManifestModule.Name + "::" + (string.IsNullOrEmpty(type.Namespace) ? " " : type.Namespace) + "::" + type.Name;

            //写入Json注释头
            _string.Append(WirteJsonHeadTag(FullClassName));

            _string.Append(_ClassToJSON(config, type));

            return _string.ToString();
        }

        #region ToJSON 实现

        private static string _ClassToJSON(object obj, Type type)
        {
            if (obj == null) return "null";

            StringBuilder _result = new StringBuilder("{");

            FieldInfo[] pFieldInfos = _getUsefulFieldInfos(type);

            int i, len = pFieldInfos.Length;
            for (i = 0; i < len; i++)
            {

                if (i > 0)
                {
                    _result.Append(",");
                }

                FieldInfo pFieldInfo = pFieldInfos[i];
                string key = "\"" + pFieldInfo.Name + "\"";
                Type FType = pFieldInfo.FieldType;
                object value = pFieldInfo.GetValue(obj);

                _result.Append(key + ":");

                //value
                _result.Append(_ObjToJSON(value, FType));

            }

            _result.Append("}");

            return _result.ToString();
        }

        public static string _ObjToJSON(object obj, Type type)
        {

            if (obj == null) return "null";

            StringBuilder _result = new StringBuilder();

            if (type.IsArray)
            {
                //数组
                _result.Append(_ArrayToJSON(obj));
            }
            else if (type.IsEnum)
            {
                //枚举
                _result.Append("\"" + obj.ToString() + "\"");
            }
            else if (type.IsGenericType)
            {
                //List
                if (type.GetInterface("IList") != null)
                {
                    _result.Append(_ListToJSON(obj));

                }
                else if (type.GetInterface("IDictionary") != null)
                {
                    //dictionary
                    _result.Append(_DicToJSON(obj));
                }

            }
            else
            {
                //值类型
                if (obj is string || obj is bool)
                {
                    _result.Append("\"" + obj.ToString() + "\"");
                }
                else if (obj is Config)
                {
                    //其他Config类
                    _result.Append(_ClassToJSON(obj, obj.GetType()));
                }else if (obj is Vector2)
                {
                    Vector2 v2 = (Vector2) obj;
                    _result.Append("\"<" + v2.x + "," + v2.y + ">\"");
                }else if (obj is Vector3)
                {
                    Vector3 v3 = (Vector3)obj;
                    _result.Append("\"<" + v3.x + "," + v3.y + "," + v3.z + ">\"");
                }
                else if (obj is Vector4)
                {
                    Vector4 v4 = (Vector4)obj;
                    _result.Append("\"<" + v4.x + "," + v4.y + "," + v4.z + "," + v4.w + ">\"");
                }
                else if (obj is Rect)
                {
                    Rect r = (Rect)obj;
                    _result.Append("\"<" + r.x + "," + r.y + "," + r.width + "," + r.height + ">\"");
                }
                else
                {
                    _result.Append(obj.ToString());
                }
            }

            return _result.ToString();
        }

        private static string _ArrayToJSON(object arrayObj)
        {

            if (arrayObj == null) return "null";

            StringBuilder _result = new StringBuilder("[");

            Array array = (Array) arrayObj;

            int i, len = array.Length;
            for (i = 0; i < len; i++)
            {

                if (i > 0)
                {
                    _result.Append(",");
                }

                object obj = array.GetValue(i);

                _result.Append(_ObjToJSON(obj, obj.GetType()));

            }

            _result.Append("]");

            return _result.ToString();
        }

        private static string _ListToJSON(object listObj)
        {
            if (listObj == null) return "null";

            StringBuilder _result = new StringBuilder("[");

            IList iList = (IList) listObj;

            int idx = 0;
            foreach (object o in iList)
            {
                if (idx > 0)
                {
                    _result.Append(",");
                }

                _result.Append(_ObjToJSON(o, o.GetType()));

                idx ++;
            }

            _result.Append("]");

            return _result.ToString();
        }

        private static string _DicToJSON(object dicObj)
        {
            if (dicObj == null) return "null";

            StringBuilder _result = new StringBuilder("[");

            IDictionary iDic = (IDictionary) dicObj;

            int idx = 0;
            foreach (object key in iDic.Keys)
            {
                if (idx > 0)
                {
                    _result.Append(",");
                }

                _result.Append("{");

                //key
                _result.Append(_ObjToJSON(key, key.GetType()));

                _result.Append(":");

                //value
                object value = iDic[key];
                _result.Append(_ObjToJSON(value, value.GetType()));

                _result.Append("}");
                idx++;
            }

            _result.Append("]");

            return _result.ToString();
        }

        #endregion

        private static FieldInfo[] _getUsefulFieldInfos(Type type)
        {
            FieldInfo[] pFieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);
            FieldInfo[] nFieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

            List<FieldInfo> list = new List<FieldInfo>();
            list.AddRange(pFieldInfos);
            
            if (nFieldInfos != null && nFieldInfos.Length > 0)
            {
                for (int i = 0; i < nFieldInfos.Length; i++)
                {
                    Attribute[] a = Attribute.GetCustomAttributes(nFieldInfos[i]);
                    if (a.Length > 0)
                    {
                        int u, ulen = a.Length;
                        for (u = 0; u < ulen; u++)
                        {
                            if (a[u].GetType() == typeof (JConfigParmsAttribute))
                            {
                                list.Add(nFieldInfos[i]);
                                break;
                            }
                        }
                    }
                }
            }

            if (list.Count > 0)
            {
                return list.ToArray();
            }

            return null;
        }

    }
}
