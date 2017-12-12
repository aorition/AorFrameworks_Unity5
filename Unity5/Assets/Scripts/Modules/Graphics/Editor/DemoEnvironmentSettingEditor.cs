using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DemoEnvironmentSetting))]
public class DemoEnvironmentSettingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            if (GUILayout.Button("更新配置"))
            {
                (target as DemoEnvironmentSetting).EnvironmentUpdate(true);
            }
        }
        else
        {
            Shader.SetGlobalFloat("_HdrIntensity", 0);
        }

    }
}
