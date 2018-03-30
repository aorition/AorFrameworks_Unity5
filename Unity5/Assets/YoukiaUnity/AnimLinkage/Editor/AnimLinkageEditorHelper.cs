using System;
using System.CodeDom;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Animator))]
public class AnimLinkageEditorHelper : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginVertical("box");
        //        /**
        //         * 暂时禁用 
        AnimLinkage alg = ((Component)target).GetComponent<AnimLinkage>();
        if (alg == null)
        {
            GUI.color = Color.green;
            if (GUILayout.Button("创建动画联动解算组件（编辑器功能）"))
            {
                ((Component)target).gameObject.AddComponent<AnimLinkage>();
            }
            GUI.color = Color.white;
        }
        //        */
        if (alg != null)
        {
            GUI.color = Color.cyan;
            if (GUILayout.Button("清理联动解算组件（编辑器功能）"))
            {
                if (alg != null)
                {
                    AnimLinkageEditorSimulate.Instance.RemoveSimulateObj(alg);

                    //直接使用GameObject.DestroyImmediate(alg, false)移除组件，编辑器会报 “... has been destroyed but you are still trying to access it.” 错误，无奈只能讲删除指令放在EditorApplication.update中执行。
//                    UDDoOnce = () =>
//                    {
//                        EditorApplication.update -= UDDoOnce;
//                        GameObject.DestroyImmediate(alg, false);
//                    };
//                    EditorApplication.update += UDDoOnce;

                    EditorPlusMethods.NextEditorApplicationUpdateDo(() =>
                    {
                        GameObject.DestroyImmediate(alg, false);
                    });

                }
            }
            GUI.color = Color.white;
        }


        GUILayout.EndVertical();

    }

//    private EditorApplication.CallbackFunction UDDoOnce;

}
