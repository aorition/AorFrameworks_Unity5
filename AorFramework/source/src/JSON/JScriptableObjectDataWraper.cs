using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.JSON
{
    /// <summary>
    /// JSON导出包装类, JsonUtility不支持将单个UnityEngine.Object转成JSON,所以只能包装单个UnityEngine.Object包装JScriptableObjectDataWraper再进行转换.
    /// </summary>
    public class JScriptableObjectDataWraper : ScriptableObject
    {

        public static JScriptableObjectDataWraper CreateInstance()
        {
            return ScriptableObject.CreateInstance<JScriptableObjectDataWraper>();
        }

        public static string EncodeJSON(UnityEngine.Object obj)
        {
            if (!obj) return string.Empty;
            JScriptableObjectDataWraper wraper = CreateInstance();
            wraper.Value = obj;
            string json = JsonUtility.ToJson(wraper);
            wraper.Dispose();
            return json;
        }

        public static T CreateObjectFromJSON<T>(string json)
            where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(json)) return null;
            JScriptableObjectDataWraper warper = JScriptableObjectDataWraper.CreateInstance();
            JsonUtility.FromJsonOverwrite(json, warper);
            T obj = warper.Value as T;
            warper.Dispose();
            return obj;
        }

        public static UnityEngine.Object CreateObjectFromJSON(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            JScriptableObjectDataWraper warper = JScriptableObjectDataWraper.CreateInstance();
            JsonUtility.FromJsonOverwrite(json, warper);
            UnityEngine.Object obj = warper.Value;
            warper.Dispose();
            return obj;
        }

        //-------------------------------------------------------------------------

        public UnityEngine.Object Value;

        public void Dispose()
        {
            GameObject.DestroyImmediate(this);
        }
    }
}
