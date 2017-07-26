using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph.Utility
{

    public class NodeGraphUtility
    {

        /// <summary>
        /// 根据GUID获取该对象的Type;
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public static Type GetScriptByGUID(string GUID)
        {
            UnityEngine.Object cso = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(GUID));
            if (cso != null)
            {
                MonoScript ms = cso as MonoScript;
                return ms.GetClass();
            }
            return null;
        }
        
        public static bool Draw_NG_Toggle(object data, string refFieldName, GUIContent labelContent, Action<bool> changedAction = null)
        {
            bool value = (bool)data.ref_GetField_Inst_Public(refFieldName);
            bool nValue = EditorGUILayout.Toggle(labelContent, value);
            if (nValue != value)
            {
                data.ref_SetField_Inst_Public(refFieldName, nValue);
                if (changedAction != null)
                {
                    changedAction(nValue);
                }
            }
            return nValue;
        }

        public static string Draw_NG_TextField(object data,string refFieldName,GUIContent labelContent,Action<string> changedAction = null)
        {
            string value = (string)data.ref_GetField_Inst_Public(refFieldName);
            string nValue = EditorGUILayout.TextField(labelContent, value);
            if (nValue != value)
            {
                data.ref_SetField_Inst_Public(refFieldName, nValue);
                if (changedAction != null)
                {
                    changedAction(nValue);
                }
            }
            return nValue;
        }

        public static int Draw_NG_IntField(object data, string refFieldName, GUIContent labelContent, Action<int> changedAction = null)
        {
            int value = (int)data.ref_GetField_Inst_Public(refFieldName);
            int nValue = EditorGUILayout.IntField(labelContent, value);
            if (nValue != value)
            {
                data.ref_SetField_Inst_Public(refFieldName, nValue);
                if (changedAction != null)
                {
                    changedAction(nValue);
                }
            }
            return nValue;
        }

        public static float Draw_NG_FloatField(object data, string refFieldName, GUIContent labelContent, Action<float> changedAction = null)
        {
            float value = (float)data.ref_GetField_Inst_Public(refFieldName);
            float nValue = EditorGUILayout.FloatField(labelContent, value);
            if (nValue != value)
            {
                data.ref_SetField_Inst_Public(refFieldName, nValue);
                if (changedAction != null)
                {
                    changedAction(nValue);
                }
            }
            return nValue;
        }

        public static Vector2 Draw_NG_Vector2Field(object data, string refFieldName, GUIContent labelContent, Action<Vector2> changedAction = null)
        {
            Vector2 value = (Vector2)data.ref_GetField_Inst_Public(refFieldName);
            Vector2 nValue = EditorGUILayout.Vector2Field(labelContent, value);
            if (nValue != value)
            {
                data.ref_SetField_Inst_Public(refFieldName, nValue);
                if (changedAction != null)
                {
                    changedAction(nValue);
                }
            }
            return nValue;
        }

        public static Vector3 Draw_NG_Vector3Field(object data, string refFieldName, GUIContent labelContent, Action<Vector3> changedAction = null)
        {
            Vector3 value = (Vector3)data.ref_GetField_Inst_Public(refFieldName);
            Vector3 nValue = EditorGUILayout.Vector3Field(labelContent, value);
            if (nValue != value)
            {
                data.ref_SetField_Inst_Public(refFieldName, nValue);
                if (changedAction != null)
                {
                    changedAction(nValue);
                }
            }
            return nValue;
        }

        public static Vector4 Draw_NG_Vector4Field(object data, string refFieldName, GUIContent labelContent, Action<Vector4> changedAction = null)
        {
            Vector4 value = (Vector4)data.ref_GetField_Inst_Public(refFieldName);
            Vector4 nValue = EditorGUILayout.Vector4Field(labelContent, value);
            if (nValue != value)
            {
                data.ref_SetField_Inst_Public(refFieldName, nValue);
                if (changedAction != null)
                {
                    changedAction(nValue);
                }
            }
            return nValue;
        }
        
        public static Enum Draw_NG_EnumPopup<T>(object data, string refFieldName, GUIContent labelContent, Action<Enum> changedAction = null) where T : struct 
        {

            if (!typeof (T).IsEnum)
            {
                throw new Exception("Error T Type, T must be Enum.");
            }

            Enum value = (Enum)Enum.Parse(typeof(T), (string)data.ref_GetField_Inst_Public(refFieldName));
            Enum nValue = EditorGUILayout.EnumPopup(labelContent, value);
            if (nValue != value)
            {
                data.ref_SetField_Inst_Public(refFieldName, nValue.ToString());
                if (changedAction != null)
                {
                    changedAction(nValue);
                }
            }
            return nValue;
        }

        //Popup
        public static int Draw_NG_Popup(object data, string refFieldName, string[] popupList, Action<int> changedAction = null)
        {

            int value = (int)data.ref_GetField_Inst_Public(refFieldName);
            int nValue = EditorGUILayout.Popup(value, popupList);
            if (nValue != value)
            {
                data.ref_SetField_Inst_Public(refFieldName, nValue);
                if (changedAction != null)
                {
                    changedAction(nValue);
                }
            }
            return nValue;
        }

        public static int Draw_NG_Popup(object data, string refFieldName, GUIContent labelContent, string[] popupList, Action<int> changedAction = null)
        {

            int value = (int)data.ref_GetField_Inst_Public(refFieldName);
            int nValue = EditorGUILayout.Popup(labelContent.text, value, popupList);
            if (nValue != value)
            {
                data.ref_SetField_Inst_Public(refFieldName, nValue);
                if (changedAction != null)
                {
                    changedAction(nValue);
                }
            }
            return nValue;
        }


    }
}
