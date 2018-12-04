using UnityEngine;
using AorBaseUtility.Extends;
using UnityEditor;

namespace Framework.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(JScriptableObject), true)]
    public class JScriptableObjectEditor : UnityEditor.Editor
    {

        protected JScriptableObject _target;
        protected JScriptableObject[] _targets;
        protected virtual void Awake()
        {
            _target = this.target as JScriptableObject;

            int i = 0;
            int len = this.targets.Length;
            _targets = new JScriptableObject[len];
            for (i = 0; i < len; i++)
            {
                _targets[i] = this.targets[i] as JScriptableObject;
            }

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //--

            if (_target == null || _targets == null) return;

            GUILayout.Space(12);
            Draw_AssetFileApplyImmediateUI();
            GUILayout.Space(8);
            /*
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("ClearInnerJsonData"))
                    {
                        for (int i = 0; i < _targets.Length; i++)
                        {
                            _targets[i].ClearInnerJsonData();
                        }
                    }
                    if (GUILayout.Button("ClearSerializeData"))
                    {
                        for (int i = 0; i < _targets.Length; i++)
                        {
                            _targets[i].ClearSerializeData();
                        }
                    }
                }
                GUILayout.EndHorizontal();
                if (_targets.Length == 1)
                {
                    GUILayout.Space(8);
                    string json = (string)_target.GetNonPublicField("_innerJsonValue");
                    if (!string.IsNullOrEmpty(json))
                    {
                        GUILayout.TextArea(json);
                        GUILayout.Space(5);
                        if (GUILayout.Button("BuildDataFormJson")) {
                            _target.InvokeNonPublicMethod("OnEnable", null);
                        }
                    }
                }
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
            */

            GUILayout.Space(8);

            Draw_JScriptableObject_editor_UI();

        }

        protected void Draw_AssetFileApplyImmediateUI() {
            EditorPlusMethods.Draw_AssetFileApplyImmediateUI(target);
        }

        protected bool _JSEUIEnable = false;
        protected void Draw_JScriptableObject_editor_UI() {

            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(8);

                GUILayout.Label("JScriptableObject Info : ");

                GUILayout.Space(8);
                if (_targets.Length == 1)
                {

                    GUILayout.BeginVertical("box");
                    {

                        GUILayout.Space(5);

                        JScriptableObjectStatus status = (JScriptableObjectStatus)_target.GetNonPublicField("_status");
                        bool useJsonPrase = !status.isInit;
                        GUILayout.Label("Status : ");
                        if (useJsonPrase)
                        {
                            GUI.color = Color.yellow;
                            GUILayout.Label("仅使用JSON数据 (Just use JSONData only.)");
                            GUI.color = Color.white;
                        }
                        else
                        {
                            GUI.color = Color.green;
                            GUILayout.Label("优先使用Serialize数据 (Use SerializeData first.)");
                            GUI.color = Color.white;
                        }

                        GUILayout.Space(5);

                    }
                    GUILayout.EndVertical();

                    GUILayout.Space(8);

                    GUILayout.BeginVertical("box");
                    {
                        GUILayout.Space(5);

                        GUILayout.Label("JSON 数据内容: ");

                        GUILayout.Space(5);

                        string json = (string)_target.GetNonPublicField("_innerJsonValue");
                        if (!string.IsNullOrEmpty(json))
                        {
                            GUILayout.TextArea(json);

                            GUILayout.Space(5);

                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("复制到剪贴板", GUILayout.Height(26)))
                            {
                                //复制到剪贴板
                                TextEditor textEd = new TextEditor();
                                textEd.text = json;
                                textEd.OnFocus();
                                textEd.Copy();
                                EditorUtility.DisplayDialog("Done", "内容已经复制到剪贴板上了.", "OK");
                            }
                            GUI.color = Color.red;
                            if (GUILayout.Button("使用JSON数据恢复Serialize数据", GUILayout.Height(26)))
                            {
                                if (EditorUtility.DisplayDialog("提示", "确定使用JSON数据恢复Serialize数据 ? ", "确定", "取消"))
                                {
                                    Undo.RecordObject(this, "使用JSON数据恢复Serialize数据");
                                    _target.InvokeNonPublicMethod("OnEnable", null);
                                }
                            }
                            GUI.color = Color.white;
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.Label("<No JSON Data>");
                        }

                        GUILayout.Space(5);
                    }

                    GUILayout.EndVertical();
                }

                GUILayout.Space(8);

                GUILayout.BeginHorizontal();
                {
                    GUI.color = Color.yellow;
                    if (GUILayout.Button("创建JSON数据",GUILayout.Height(26)))
                    {

                        if (EditorUtility.DisplayDialog("提示", "确定创建JSON数据 ? ", "确定", "取消"))
                        {
                            Undo.RecordObjects(_targets, "创建JSON数据");
                            for (int i = 0; i < _targets.Length; i++)
                            {
                                JScriptableObject jso = _targets[i] as JScriptableObject;
                                string json = JSONEncoder.ToJSON(jso);
                                target.SetNonPublicField("_innerJsonValue", json);
                            }
                        }

                    }
                    GUI.color = Color.red;
                    if (GUILayout.Button("清除JSON数据", GUILayout.Height(26)))
                    {
                        if (EditorUtility.DisplayDialog("提示", "确定清除JSON数据 ? ", "确定", "取消"))
                        {
                            Undo.RecordObjects(_targets, "清除JSON数据");
                            for (int i = 0; i < _targets.Length; i++)
                            {
                                _targets[i].ClearInnerJsonData();
                            }
                        }
                    }
                    GUI.color = Color.white;
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(8);
                //
                GUILayout.BeginHorizontal();
                {
                    GUI.color = Color.red;
                    if (GUILayout.Button("清除Serialize数据", GUILayout.Height(26)))
                    {

                        if (EditorUtility.DisplayDialog("提示", "确定清除Serialize数据 ? ", "确定", "取消"))
                        {
                            Undo.RecordObjects(_targets, "清除Serialize数据");
                            for (int i = 0; i < _targets.Length; i++)
                            {
                                _targets[i].ClearSerializeData();
                            }
                        }
                    }
                    GUI.color = Color.white;
                }
                GUILayout.EndHorizontal();
                //
                GUILayout.Space(8);
            }
            GUILayout.EndVertical();
        }

    }

}