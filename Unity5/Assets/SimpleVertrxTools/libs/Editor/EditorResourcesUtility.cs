using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework.editor
{
    /// <summary>
    /// UnityEditorInternal.EditorResourcesUtility 类的 Wrap反射类
    /// </summary>
    public class EditorResourcesUtility
    {

        private static EditorResourcesUtility _instance;

        //---------------------------------------------------

        public Assembly Assembly;
        public Type ClassType;

        public Dictionary<string,PropertyInfo> PropertyDic = new Dictionary<string, PropertyInfo>();

        private static EditorResourcesUtility GetInstance()
        {
            if (_instance == null)
            {
                _instance = new EditorResourcesUtility();
                _instance.Assembly = Assembly.GetAssembly(typeof (UnityEditor.Editor));
                _instance.ClassType = _instance.Assembly.GetType("UnityEditorInternal.EditorResourcesUtility");

                PropertyInfo[] propertyInfos = _instance.ClassType.GetProperties();
                int i, len = propertyInfos.Length;
                for (i = 0; i < len; i++)
                {
                    PropertyInfo pinfo = propertyInfos[i];

                    if (!_instance.PropertyDic.ContainsKey(pinfo.Name))
                    {
                        _instance.PropertyDic.Add(pinfo.Name, pinfo);
                    }
                }

            }
            return _instance;
        }

        public static string lightSkinSourcePath
        {
            get
            {
                return (string) GetInstance().PropertyDic["lightSkinSourcePath"].GetValue(GetInstance().ClassType, null);
            }
        }

        public static string darkSkinSourcePath
        {
            get
            {
                return (string)GetInstance().PropertyDic["darkSkinSourcePath"].GetValue(GetInstance().ClassType, null);
            }
        }

        public static string fontsPath
        {
            get
            {
                return (string)GetInstance().PropertyDic["fontsPath"].GetValue(GetInstance().ClassType, null);
            }
        }

        public static string brushesPath
        {
            get
            {
                return (string)GetInstance().PropertyDic["brushesPath"].GetValue(GetInstance().ClassType, null);
            }
        }

        public static string iconsPath
        {
            get
            {
                return (string)GetInstance().PropertyDic["iconsPath"].GetValue(GetInstance().ClassType, null);
            }
        }

        public static string generatedIconsPath
        {
            get
            {
                return (string)GetInstance().PropertyDic["generatedIconsPath"].GetValue(GetInstance().ClassType, null);
            }
        }

        public static string folderIconName
        {
            get
            {
                return (string)GetInstance().PropertyDic["folderIconName"].GetValue(GetInstance().ClassType, null);
            }
        }

        public static string emptyFolderIconName
        {
            get
            {
                return (string)GetInstance().PropertyDic["emptyFolderIconName"].GetValue(GetInstance().ClassType, null);
            }
        }

    }
}
