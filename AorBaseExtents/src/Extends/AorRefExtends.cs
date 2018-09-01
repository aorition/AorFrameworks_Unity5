using System;
using System.Collections.Generic;
using System.Reflection;

namespace AorBaseUtility.Extends
{

    /// <summary>
    /// 常用反射Extends;
    /// </summary>
    public static class AorRefExtends
    {

        //缓存
        private static Dictionary<Type, Dictionary<string, FieldInfo>> __FieldsCache = new Dictionary<Type, Dictionary<string, FieldInfo>>();
        private static Dictionary<Type, Dictionary<string, MethodInfo>> __MethodInfoCache = new Dictionary<Type, Dictionary<string, MethodInfo>>();

        // -- Field get
        #region Field Get

        private static object getField(object obj, string fieldName, BindingFlags inst, BindingFlags area)
        {
            Type t = obj.GetType();
            FieldInfo fieldInfo = null;
            bool hasT = __FieldsCache.ContainsKey(t);
            object data = null;

            //有T就有Dic
            if (hasT)
            {
                if (__FieldsCache[t].ContainsKey(fieldName))
                {


                    fieldInfo = __FieldsCache[t][fieldName];

                }
                else
                {
                    //不包含字段则dic补字段
                    fieldInfo = t.GetField(fieldName, inst | area | BindingFlags.GetField);
                    __FieldsCache[t].Add(fieldName, fieldInfo);
                }

            }
            else
            {

                //无T就补带空Dic的T
                fieldInfo = t.GetField(fieldName, inst | area | BindingFlags.GetField);
                Dictionary<string, FieldInfo> dic = new Dictionary<string, FieldInfo>();
                dic.Add(fieldName, fieldInfo);
                __FieldsCache.Add(t, dic);
            }

            if (fieldInfo != null)
                data = fieldInfo.GetValue(obj);

            return data;

        }

        //---

        public static object GetNonPublicField(this object obj, string fieldName)
        {
            return getField(obj, fieldName, BindingFlags.Instance, BindingFlags.NonPublic);
        }

        public static object GetPublicField(this object obj, string fieldName)
        {
            return getField(obj, fieldName, BindingFlags.Instance, BindingFlags.Public);
        }

        //未验证
        public static object GetNonPublicStaticField(this object obj, string fieldName)
        {
            return getField(obj, fieldName, BindingFlags.Static, BindingFlags.NonPublic);
        }

        //未验证
        public static object GetPublicStaticField(this object obj, string fieldName)
        {
            return getField(obj, fieldName, BindingFlags.Static, BindingFlags.Public);
        }

        #endregion

        // -- Field set
        #region Field Set

        private static bool setField(object obj, string fieldName, object value, BindingFlags inst, BindingFlags area)
        {
            Type t = obj.GetType();
            FieldInfo fieldInfo = null;
            bool hasT = __FieldsCache.ContainsKey(t);
            object data = null;

            //有T就有Dic
            if (hasT)
            {
                if (__FieldsCache[t].ContainsKey(fieldName))
                {


                    fieldInfo = __FieldsCache[t][fieldName];

                }
                else
                {
                    //不包含字段则dic补字段
                    fieldInfo = t.GetField(fieldName, inst | area | BindingFlags.GetField);
                    __FieldsCache[t].Add(fieldName, fieldInfo);

                }

            }
            else
            {

                //无T就补带空Dic的T
                fieldInfo = t.GetField(fieldName, inst | area | BindingFlags.GetField);
                Dictionary<string, FieldInfo> dic = new Dictionary<string, FieldInfo>();
                dic.Add(fieldName, fieldInfo);
                __FieldsCache.Add(t, dic);
            }

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, value);
                return true;
            }
            else
            {
                return false;
            }

        }


        public static bool SetNonPublicField(this object obj, string fieldName, object value)
        {
            return setField(obj, fieldName, value, BindingFlags.Instance, BindingFlags.NonPublic);


        }

        public static bool SetPublicField(this object obj, string fieldName, object value)
        {
            return setField(obj, fieldName, value, BindingFlags.Instance, BindingFlags.Public);
        }

        //未验证
        public static bool SetNonPublicStaticField(this object obj, string fieldName, object value)
        {
            return setField(obj, fieldName, value, BindingFlags.Static, BindingFlags.NonPublic);
        }

        //未验证
        public static bool SetPublicStaticField(this object obj, string fieldName, object value)
        {
            return setField(obj, fieldName, value, BindingFlags.Static, BindingFlags.Public);
        }

        #endregion

        // -- InvokeMethod
        #region InvokeMethod

        private static object invokeMethod(object obj, string MethodName, object[] parameters, BindingFlags inst, BindingFlags area)
        {
            Type t = obj.GetType();
            MethodInfo methodInfo = null;
            bool hasT = __MethodInfoCache.ContainsKey(t);
            object data = null;

            //有T就有Dic
            if (hasT)
            {
                if (__MethodInfoCache[t].ContainsKey(MethodName))
                {


                    methodInfo = __MethodInfoCache[t][MethodName];

                }
                else
                {
                    //不包含字段则dic补字段
                    methodInfo = t.GetMethod(MethodName, inst | area | BindingFlags.InvokeMethod);
                    __MethodInfoCache[t].Add(MethodName, methodInfo);

                }

            }
            else
            {

                //无T就补带空Dic的T
                methodInfo = t.GetMethod(MethodName, inst | area | BindingFlags.InvokeMethod);
                Dictionary<string, MethodInfo> dic = new Dictionary<string, MethodInfo>();
                dic.Add(MethodName, methodInfo);
                __MethodInfoCache.Add(t, dic);
            }

            if (methodInfo != null)
            {

                return methodInfo.Invoke(obj, parameters);
            }
            else
            {
                return null;
            }

        }

        //---

        public static object InvokeNonPublicMethod(this object obj, string MethodName, object[] parameters)
        {
            return invokeMethod(obj, MethodName, parameters, BindingFlags.Instance, BindingFlags.NonPublic);
        }

        public static object InvokePublicMethod(this object obj, string MethodName, object[] parameters)
        {
            return invokeMethod(obj, MethodName, parameters, BindingFlags.Instance, BindingFlags.Public);
        }

        public static object InvokeNonPublicStaticMethod(this object obj, string MethodName, object[] parameters)
        {
            return invokeMethod(obj, MethodName, parameters, BindingFlags.Static, BindingFlags.NonPublic);
        }

        public static object InvokePublicStaticMethod(this object obj, string MethodName, object[] parameters)
        {
            return invokeMethod(obj, MethodName, parameters, BindingFlags.Static, BindingFlags.Public);
        }

        #endregion

        #region 废弃的方法

        [Obsolete("已由GetNonPublicField替代")]
        public static object ref_GetField_Inst_NonPublic(this object obj, string fieldName)
        {
            return GetNonPublicField(obj, fieldName);
        }
        [Obsolete("已由GetPublicField替代")]
        public static object ref_GetField_Inst_Public(this object obj, string fieldName)
        {
            return GetPublicField(obj, fieldName);
        }

        [Obsolete("已由GetNonPublicStaticField替代")]
        public static object ref_GetField_Static_NonPublic(this object obj, string fieldName)
        {
            return GetNonPublicStaticField(obj, fieldName);
        }

        [Obsolete("已由GetPublicStaticField替代")]
        public static object ref_GetField_Static_Public(this object obj, string fieldName)
        {
            return GetPublicStaticField(obj, fieldName);
        }

        [Obsolete("已由SetNonPublicField替代")]
        public static bool ref_SetField_Inst_NonPublic(this object obj, string fieldName, object value)
        {
            return SetNonPublicField(obj, fieldName, value);
        }

        [Obsolete("已由SetPublicField替代")]
        public static bool ref_SetField_Inst_Public(this object obj, string fieldName, object value)
        {
            return SetPublicField(obj, fieldName, value);
        }

        [Obsolete("已由SetNonPublicStaticField替代")]
        public static bool ref_SetField_Static_NonPublic(this object obj, string fieldName, object value)
        {
            return SetNonPublicStaticField(obj, fieldName, value);
        }

        [Obsolete("已由SetPublicStaticField替代")]
        public static bool ref_SetField_Static_Public(this object obj, string fieldName, object value)
        {
            return SetPublicStaticField(obj, fieldName, value);
        }

        [Obsolete("已由InvokeNonPublicMethod替代")]
        public static object ref_InvokeMethod_Inst_NonPublic(this object obj, string MethodName, object[] parameters)
        {
            return InvokeNonPublicMethod(obj, MethodName, parameters);
        }

        [Obsolete("已由InvokePublicMethod替代")]
        public static object ref_InvokeMethod_Inst_Public(this object obj, string MethodName, object[] parameters)
        {
            return InvokePublicMethod(obj, MethodName, parameters);
        }

        [Obsolete("已由InvokeNonPublicStaticMethod替代")]
        public static object ref_InvokeMethod_Static_NonPublic(this object obj, string MethodName, object[] parameters)
        {
            return InvokeNonPublicStaticMethod(obj, MethodName, parameters);
        }

        [Obsolete("已由InvokePublicStaticMethod替代")]
        public static object ref_InvokeMethod_Static_Public(this object obj, string MethodName, object[] parameters)
        {
            return InvokePublicStaticMethod(obj, MethodName, parameters);
        }

        #endregion

    }

}


