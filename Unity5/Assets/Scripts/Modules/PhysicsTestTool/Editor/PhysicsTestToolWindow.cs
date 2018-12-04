using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Utility.Editor
{
    public class PhysicsTestToolWindow : UnityEditor.EditorWindow
    {

        private static PhysicsTestToolWindow _instance;
        [MenuItem("FrameworkTools/辅助工具/PhysicsTestToolWindow")]
        public static PhysicsTestToolWindow init()
        {
            _instance = UnityEditor.EditorWindow.GetWindow<PhysicsTestToolWindow>();
            return _instance;
        }

        private void OnGUI()
        {

            #region 编译中
            if (EditorApplication.isCompiling)
            {
                ShowNotification(new GUIContent("Compiling Please Wait..."));
                Repaint();
                return;
            }
            RemoveNotification();
            #endregion

            if (!Application.isPlaying)
            {
                GUILayout.Label("此工具需要工作于运行时状态.");
                return;
            }

            GameObject select = Selection.activeGameObject;
            if (select)
            {
                Rigidbody rigidbody = select.GetComponent<Rigidbody>();
                if (rigidbody)
                {

                    RigidbodyDebugHandler rdh = select.GetComponent<RigidbodyDebugHandler>();
                    if (!rdh)
                    {
                        rdh = select.gameObject.AddComponent<RigidbodyDebugHandler>();
                    }
                    GUILayout.Label("Target : " + select.name);

                    float nf = EditorGUILayout.FloatField("Force : ", rdh.force);
                    if (!nf.Equals(rdh.force))
                    {
                        rdh.force = nf;
                    }

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("AddForce"))
                    {
                        rigidbody.AddForce(rdh.velocity);
                    }
                    if (GUILayout.Button("AddRelativeForce"))
                    {
                        rigidbody.AddRelativeForce(rdh.velocity);
                    }
                    if (GUILayout.Button("AddRelativeTorque"))
                    {
                        rigidbody.AddRelativeTorque(rdh.velocity);
                    }

                    GUILayout.EndHorizontal();
                    //
                    //                //
                    //                SpringJoint springJoint = select.GetComponent<SpringJoint>();
                    //                if (springJoint)
                    //                {
                    //
                    //                    GUILayout.Space(18);
                    //
                    //                    GUILayout.BeginHorizontal();
                    //                    if (GUILayout.Button("AddForce"))
                    //                    {
                    //
                    //                    }
                    //                    GUILayout.EndHorizontal();
                    //
                    //                }

                }
            }
            Repaint();
        }

    }

}

