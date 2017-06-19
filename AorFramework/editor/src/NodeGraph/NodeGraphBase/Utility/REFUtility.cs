using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph.Utility
{ 
    public class REFUtility
    {
        
        public static void DrawFieldByFieldInfo(FieldInfo fieldInfo, object t)
        {
            GUILayout.BeginHorizontal();

            Type fType = fieldInfo.FieldType;
            switch (fType.Name)
            {
                case "Boolean":
                    BoolField(fieldInfo, t);
                    break;
                case "String":
                    StringField(fieldInfo, t);
                    break;
                case "Int32":
                    Int32Field(fieldInfo, t);
                    break;
                case "Int64":
                    Int64Field(fieldInfo, t);
                    break;
                case "Single":
                    SingleField(fieldInfo, t);
                    break;
                case "Vector2":
                    Vector2Field(fieldInfo, t);
                    break;
                case "Vector3":
                    Vector3Field(fieldInfo, t);
                    break;
                case "Vector4":
                    Vector4Field(fieldInfo, t);
                    break;
                default:
                    EditorGUILayout.LabelField("NonsupportField>" + fieldInfo.Name + "<" + fieldInfo.Name + ">" );
                    break;
            }
            GUILayout.EndHorizontal();
        }


        private static void BoolField(FieldInfo fieldInfo, object t)
        {
            bool o = (bool)fieldInfo.GetValue(t);
            bool n = EditorGUILayout.Toggle(fieldInfo.Name, o);
            if (!n.Equals(o))
            {
                fieldInfo.SetValue(t, n);
            }
        }

        #region 子方法

        private static void StringField(FieldInfo fieldInfo, object t)
        {
            string o = (string) fieldInfo.GetValue(t);
            string n = EditorGUILayout.TextField(fieldInfo.Name, o);
            if (n != o)
            {
                fieldInfo.SetValue(t, n);
            }
        }

        private static void Int32Field(FieldInfo fieldInfo, object t)
        {
            int o = (int) fieldInfo.GetValue(t);
            int n = EditorGUILayout.IntField(fieldInfo.Name, o);
            if (!n.Equals(o))
            {
                fieldInfo.SetValue(t, n);
            }
        }

        private static void Int64Field(FieldInfo fieldInfo, object t)
        {
            long o = (long) fieldInfo.GetValue(t);
            long n = EditorGUILayout.LongField(fieldInfo.Name, o);
            if (!n.Equals(o))
            {
                fieldInfo.SetValue(t, n);
            }
        }

        private static void SingleField(FieldInfo fieldInfo, object t)
        {
            float o = (float) fieldInfo.GetValue(t);
            float n = EditorGUILayout.FloatField(fieldInfo.Name, o);
            if (!n.Equals(o))
            {
                fieldInfo.SetValue(t, n);
            }
        }

        private static void DoubleField(FieldInfo fieldInfo, object t)
        {
            double o = (double) fieldInfo.GetValue(t);
            double n = EditorGUILayout.DoubleField(fieldInfo.Name, o);
            if (!n.Equals(o))
            {
                fieldInfo.SetValue(t, n);
            }
        }

        private static void Vector2Field(FieldInfo fieldInfo, object t)
        {
            Vector2 o = (Vector2)fieldInfo.GetValue(t);
            Vector2 n = EditorGUILayout.Vector2Field(fieldInfo.Name, o);
            if (!n.Equals(o))
            {
                fieldInfo.SetValue(t, n);
            }
        }

        private static void Vector3Field(FieldInfo fieldInfo, object t)
        {
            Vector3 o = (Vector3)fieldInfo.GetValue(t);
            Vector3 n = EditorGUILayout.Vector3Field(fieldInfo.Name, o);
            if (!n.Equals(o))
            {
                fieldInfo.SetValue(t, n);
            }
        }

        private static void Vector4Field(FieldInfo fieldInfo, object t)
        {
            Vector4 o = (Vector4)fieldInfo.GetValue(t);
            Vector4 n = EditorGUILayout.Vector4Field(fieldInfo.Name, o);
            if (!n.Equals(o))
            {
                fieldInfo.SetValue(t, n);
            }
        }


        #endregion


    }
}
