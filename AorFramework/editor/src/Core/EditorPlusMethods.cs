using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 提供Editor下常用功能静态方法
/// </summary>
public class EditorPlusMethods
{
    /// <summary>
    /// 编辑器在下一次Update时调用
    /// </summary>
    public static void NextEditorApplicationUpdateDo(Action doSomething)
    {
        EditorApplication.CallbackFunction UDDoOnce = null;
        UDDoOnce = () =>
        {
            EditorApplication.update -= UDDoOnce;
            //
            if (doSomething != null) doSomething();
        };
        EditorApplication.update += UDDoOnce;
    }

    // ------------------------------------------ UsedTags 

    private static Dictionary<string, int> UsedTags; 
    /// <summary>
    /// 计数Tag机制
    /// 
    /// 添加一个计数
    /// 
    /// </summary>
    public static int AddUsedTag(string tag)
    {
        if (UsedTags == null)
        {
            UsedTags = new Dictionary<string, int>();
            UsedTags.Add(tag, 1);
            return UsedTags[tag];
        }

        if (UsedTags.ContainsKey(tag))
        {
            UsedTags[tag] ++; 
            return UsedTags[tag];
        }
        else
        {
            UsedTags.Add(tag, 1);
            return UsedTags[tag];
        }
    }

    public static int SubUsedTag(string tag)
    {
        if (UsedTags == null)
        {
            return -1;
        }
        if (UsedTags.ContainsKey(tag))
        {
            UsedTags[tag]--;
            return UsedTags[tag];
        }
        else
        {
            return -1;
        }
    }

    public static int UsedTagCount(string tag)
    {
        if (UsedTags == null)
        {
            return 0;
        }
        if (UsedTags.ContainsKey(tag))
        {
            return UsedTags[tag];
        }
        else
        {
            return 0;
        }
    }

    // ------------------------------------------ UsedTags End
    
    public enum PlusDefindWindow
    {
        AnimationWindow,
    }

    private static string _getPlusDefindWindowFullName(PlusDefindWindow defind)
    {
        switch (defind)
        {
            case PlusDefindWindow.AnimationWindow:
                return "UnityEditor.AnimationWindow";
        }
        return null;
    }

    public static EditorWindow GetPlusDefindWindow(PlusDefindWindow defind)
    {
        Assembly assembly = Assembly.GetAssembly(typeof(EditorWindow));
        string fullName = _getPlusDefindWindowFullName(defind);
        if (string.IsNullOrEmpty(fullName)) return null;
        Type t = assembly.GetType(fullName);
        if (t == null) return null;
        EditorWindow aw = EditorWindow.GetWindow(t);
        if (aw == null) return null;
        return aw;
    }

}
