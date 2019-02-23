#pragma warning disable
using UnityEngine;
using UnityEditor;
using Framework.Editor;

namespace Framework.Graphic.CustomLight.Editor
{

    [CustomEditor(typeof(CustomLightInfo))]
    public class CustomLightInfoEditor : UnityEditor.Editor
    {
        private CustomLightInfo _target;
        private void Awake()
        {
            _target = target as CustomLightInfo;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(12);

            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);

                GUILayout.Label("CustomLightInfo handle Tool");

                GUILayout.Space(5);

                if (GUILayout.Button("转换为系统灯光"))
                {
                    EditorPlusMethods.NextEditorApplicationUpdateDo(()=> {
                        CustomLightInfo.Convert2UnityLight(_target);
                    });
                }

                GUILayout.Space(5);
            }
            GUILayout.EndVertical();

        }
    }

}