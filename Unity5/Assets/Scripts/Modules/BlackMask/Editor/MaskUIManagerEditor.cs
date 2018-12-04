using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MaskUIManager))]
public class MaskUIManagerEditor : UnityEditor.Editor
{
    private MaskUIManager _target;
    private void Awake()
    {
        _target = target as MaskUIManager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {

            GUILayout.Space(12);

            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);

                GUILayout.Label("Debug Tool : ");

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("EnableMask")) {
                        _target.EnableMask();
                    }
                    if (GUILayout.Button("DisableMask"))
                    {
                        _target.DisableMask();
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

            }
            GUILayout.EndVertical();

        }

    }

}