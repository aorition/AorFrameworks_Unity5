using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Framework.JSON;
using UnityEngine;

namespace Framework.Editor
{

    /// <summary>
    /// JSON序列化,编辑器支持将对象序列化成JSON字符串
    /// </summary>
    public static class JSONEncoder
    {

        /// <summary>
        /// 再JSON字符串首部插入JsonHeadTag标识信息
        /// </summary>
        /// <param name="headTagContent"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string InsertJsonHeadTag(string headTagContent, string json)
        {
            return "//////>" + headTagContent + "\n" + json;
        }

        //================================== object to Json ==============================

        public static string ToJSON(object obj, string JsonHeadTag)
        {
            if (string.IsNullOrEmpty(JsonHeadTag))
            {
                return ToJSON(obj);
            }
            return InsertJsonHeadTag(JsonHeadTag, ToJSON(obj));
        }
        public static string ToJSON(object obj, bool useDefaultJsonHeadTag)
        {
            if (useDefaultJsonHeadTag)
            {
                Type type = obj.GetType();
                string FullClassName = type.Assembly.ManifestModule.Name + "::" +
                                       (string.IsNullOrEmpty(type.Namespace) ? " " : type.Namespace) + "::" + type.Name;
                return InsertJsonHeadTag(FullClassName, ToJSON(obj));
            }
            else
            {
                return ToJSON(obj);
            }
        }
        public static string ToJSON(object obj)
        {
            if (obj == null) return "null";

            Type type = obj.GetType();
            StringBuilder _result = new StringBuilder();

            if (type.IsValueType)
            {
                //值类型
                if (type.IsEnum)
                {
                    //枚举
                    _result.Append("\"" + obj.ToString() + "\"");
                }
                else if (type.IsPrimitive)
                {
                    //
                    //                    _result.Append(JsonUtility.ToJson(obj));
                    _result.Append(_encodeBaseObjToJson(obj));
                }
                else
                {
                    //Struct
                    _result.Append(_encodeStructOrClass(type, obj));
                }

            }
            else
            {
                //引用类型
                if (type.IsArray)
                {
                    //数组
                    _result.Append(_encodeArrayToJSON(obj));
                }
                else if (type.IsGenericType)
                {

                    if (type.GetInterface("IList") != null)
                    {
                        //List
                        _result.Append(_encodeListToJSON(obj));
                    }
                    else if (type.GetInterface("IDictionary") != null)
                    {
                        //dictionary
                        _result.Append(_encodeDicToJSON(obj));
                    }
                }else if (obj is string)
                {
                    //String 是引用
//                    _result.Append(JsonUtility.ToJson(obj));
                    _result.Append(_encodeBaseObjToJson(obj));
                }
                else if (obj is UnityEngine.Object)
                {
                    _result.Append(_encodeStructOrClass(type, obj));
                }

            }
            return _result.ToString();
        }

        private static string _encodeArrayToJSON(object arrayObj)
        {

            if (arrayObj == null) return "null";

            StringBuilder _result = new StringBuilder("[");

            Array array = (Array)arrayObj;

            int i, len = array.Length;
            for (i = 0; i < len; i++)
            {
                if (i > 0)
                {
                    _result.Append(",");
                }
                object obj = array.GetValue(i);
                _result.Append(ToJSON(obj));
            }

            _result.Append("]");

            return _result.ToString();
        }

        private static string _encodeListToJSON(object listObj)
        {
            if (listObj == null) return "null";

            StringBuilder _result = new StringBuilder("[");

            IList iList = (IList)listObj;

            int idx = 0;
            foreach (object o in iList)
            {
                if (idx > 0)
                {
                    _result.Append(",");
                }

                _result.Append(ToJSON(o));
                idx++;
            }

            _result.Append("]");

            return _result.ToString();
        }

        private static string _encodeDicToJSON(object dicObj)
        {
            if (dicObj == null) return "null";

            StringBuilder _result = new StringBuilder("[");

            IDictionary iDic = (IDictionary)dicObj;

            int idx = 0;
            foreach (object key in iDic.Keys)
            {
                if (idx > 0)
                {
                    _result.Append(",");
                }

                _result.Append("{");

                //key
                _result.Append(ToJSON(key));

                _result.Append(":");

                //value
                object value = iDic[key];
                _result.Append(ToJSON(value));

                _result.Append("}");
                idx++;
            }

            _result.Append("]");

            return _result.ToString();
        }

        private static string _encodeStructOrClass(Type type, object obj)
        {
            if (obj == null) return "null";
            StringBuilder _result = new StringBuilder();

            if (obj is JScriptableObject || !(obj is UnityEngine.Object))
            {
                _result.Append("{");
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
                    _result.Append(ToJSON(value));

                }
                _result.Append("}");
            }
            else// if (obj is UnityEngine.Object)
            {

                string subJson = JScriptableObjectDataWraper.EncodeJSON(obj as UnityEngine.Object);
                _result.Append(string.IsNullOrEmpty(subJson) ? "null" : subJson);
            }
            return _result.ToString();
        }

        private static string _encodeBaseObjToJson(object obj)
        {
            if (obj == null) return "null";
            if (obj is string || obj is bool)
            {
                return "\"" + obj.ToString() + "\"";
            }
            else
            {
                return obj.ToString();
            }
        }

        private static FieldInfo[] _getUsefulFieldInfos(Type type)
        {
            FieldInfo[] pFieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);
            FieldInfo[] nFieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
            List<FieldInfo> list = new List<FieldInfo>();
            list.AddRange(pFieldInfos);
            for (int i = 0; i < nFieldInfos.Length; i++)
            {
                Attribute[] a = Attribute.GetCustomAttributes(nFieldInfos[i]);
                if (a.Length > 0)
                {
                    int u, ulen = a.Length;
                    for (u = 0; u < ulen; u++)
                    {
                        if (a[u] is SerializeField)
                        {
                            list.Add(nFieldInfos[i]);
                            break;
                        }
                    }
                }
            }
            return list.ToArray();
        }

    }
}
