using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

namespace Framework.Editor
{
    [CustomPropertyDrawer(typeof(SetPropertyAttribute))]
    public class SetPropertyAttributeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Rely on the default inspector GUI  
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);

            // Update only when necessary  
            SetPropertyAttribute setProperty = attribute as SetPropertyAttribute;
            if (EditorGUI.EndChangeCheck())
            {
                setProperty.IsDirty = true;

            }
            else if (setProperty.IsDirty)
            {
                object obj = property.serializedObject.targetObject;
                Type type = obj.GetType();
                PropertyInfo pi = type.GetProperty(setProperty.Name);
                if (pi == null)
                {
                    Debug.LogError("Invalid property name: " + setProperty.Name + "\nCheck your [SetProperty] attribute");
                }
                else
                {
                    pi.SetValue(obj, fieldInfo.GetValue(obj), null);
                }
                setProperty.IsDirty = false;
            }
        }
    }
}


