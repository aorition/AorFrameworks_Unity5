using System;
using System.Collections.Generic;
using System.Reflection;

namespace AorBaseUtility.Config
{

    public class ConfigInstCreater
    {

        /// <summary>
        /// 创建一个Config的实例，初始数据由initParms填入；
        /// </summary>
        public static T CreateConfigInstance<T>(Dictionary<string, object> initParms) where T : Config
        {

            Type type = typeof(T);

            T inst = Activator.CreateInstance<T>();

            foreach (KeyValuePair<string, object> keyValuePair in initParms)
            {
                try
                {
                    FieldInfo fieldInfo = type.GetField(keyValuePair.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);
                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(inst, keyValuePair.Value);
                    }
                    fieldInfo = type.GetField(keyValuePair.Key, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(inst, keyValuePair.Value);
                    }
                }
                catch (Exception ex)
                {
//                    Debug.LogError(ex.Message);
                    continue;
                }
            }

            return inst;
        }

    }

}


