﻿using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 常用反射Extends;
/// </summary>
public static class AorRefExtends
{

    // -- Field get
    #region Field Get

    public static object ref_GetField_Inst_NonPublic(this object obj, string fieldName)
    {
        Type t = obj.GetType();
        FieldInfo fieldInfo = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
        if (fieldInfo != null)
        {
            return fieldInfo.GetValue(obj);
        }
        else
        {
            return null;
        }
    }

    public static object ref_GetField_Inst_Public(this object obj, string fieldName)
    {
        Type t = obj.GetType();
        FieldInfo fieldInfo = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);
        if (fieldInfo != null)
        {
            return fieldInfo.GetValue(obj);
        }
        else
        {
            return null;
        }
    }

    //未验证
    public static object ref_GetField_Static_NonPublic(this object obj, string fieldName)
    {
        Type t = obj.GetType();
        FieldInfo fieldInfo = t.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);
        if (fieldInfo != null)
        {
            return fieldInfo.GetValue(null);
        }
        else
        {
            return null;
        }
    }

    //未验证
    public static object ref_GetField_Static_Public(this object obj, string fieldName)
    {
        Type t = obj.GetType();
        FieldInfo fieldInfo = t.GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField);
        if (fieldInfo != null)
        {
            return fieldInfo.GetValue(null);
        }
        else
        {
            return null;
        }
    }

    #endregion

    // -- Field set
    #region Field Set
    public static bool ref_SetField_Inst_NonPublic(this object obj, string fieldName, object value)
    {
        Type t = obj.GetType();
        FieldInfo fieldInfo = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
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

    public static bool ref_SetField_Inst_Public(this object obj, string fieldName, object value)
    {
        Type t = obj.GetType();
        FieldInfo fieldInfo = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);
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

    //未验证
    public static bool ref_SetField_Static_NonPublic(this object obj, string fieldName, object value)
    {
        Type t = obj.GetType();
        FieldInfo fieldInfo = t.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);
        if (fieldInfo != null)
        {
            fieldInfo.SetValue(null, value);
            return true;
        }
        else
        {
            return false;
        }
    }

    //未验证
    public static bool ref_SetField_Static_Public(this object obj, string fieldName, object value)
    {
        Type t = obj.GetType();
        FieldInfo fieldInfo = t.GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField);
        if (fieldInfo != null)
        {
            fieldInfo.SetValue(null, value);
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    // -- InvokeMethod
    #region InvokeMethod

    public static object ref_InvokeMethod_Inst_NonPublic(this object obj, string MethodName, object[] parameters)
    {
        Type t = obj.GetType();
        MethodInfo methodInfo = t.GetMethod(MethodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
        if (methodInfo != null)
        {
            return methodInfo.Invoke(obj, parameters);
        }
        else
        {
            return null;
        }
    }

    public static object ref_InvokeMethod_Inst_Public(this object obj, string MethodName, object[] parameters)
    {
        Type t = obj.GetType();
        MethodInfo methodInfo = t.GetMethod(MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);
        if (methodInfo != null)
        {
            return methodInfo.Invoke(obj, parameters);
        }
        else
        {
            return null;
        }
    }

    public static object ref_InvokeMethod_Static_NonPublic(this object obj, string MethodName, object[] parameters)
    {
        Type t = obj.GetType();
        MethodInfo methodInfo = t.GetMethod(MethodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
        if (methodInfo != null)
        {
            return methodInfo.Invoke(null, parameters);
        }
        else
        {
            return null;
        }
    }

    public static object ref_InvokeMethod_Static_Public(this object obj, string MethodName, object[] parameters)
    {
        Type t = obj.GetType();
        MethodInfo methodInfo = t.GetMethod(MethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod);
        if (methodInfo != null)
        {
            return methodInfo.Invoke(null, parameters);
        }
        else
        {
            return null;
        }
    }

    #endregion
}
