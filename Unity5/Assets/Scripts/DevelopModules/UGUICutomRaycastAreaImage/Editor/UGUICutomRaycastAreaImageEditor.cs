using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(UGUICutomRaycastAreaImage), true)]
public class UGUICutomRaycastAreaImageEditor : ImageEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowImageGraphic"));

        //RaycastCollider2DList
        EditorGUILayout.PropertyField(serializedObject.FindProperty("SubArea2DList"), true);

        serializedObject.ApplyModifiedProperties();
    }
}