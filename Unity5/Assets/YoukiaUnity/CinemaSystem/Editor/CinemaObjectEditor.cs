using System;
using System.Collections.Generic;
using AorBaseUtility;
using UnityEditor;
using UnityEngine;
using YoukiaUnity.CinemaSystem;

[CustomEditor(typeof(CinemaObject))]
public class CinemaObjectEditor : Editor
{

    protected static Color KEYABLECOLOR = new Color(1f, 0.8f, 0f, 1f);

    private string _getActorLoadPath(string path)
    {
        string np = path.Replace("Assets/Resources/", "");
        np = np.Substring(0, np.LastIndexOf('.'));
        return np;
    }

    private string _getAssetPathFromActorLoadPath(string path)
    {
        return "Assets/Resources/" + path + ".prefab";
    }

    private CinemaObject _target;

    private GameObject ObjectPrefab;
    private string _ObjectLoadPath;

    protected void Awake()
    {
        _target = target as CinemaObject;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            base.OnInspectorGUI();
            return;
        }


        serializedObject.Update();

        GUILayout.BeginVertical("box");

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ObjectScale"));

        GUILayout.EndVertical();

        _ObjectLoadPath = serializedObject.FindProperty("_ObjectLoadPath").stringValue;

        if (!ObjectPrefab && !string.IsNullOrEmpty(_ObjectLoadPath))
        {
            ObjectPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_getAssetPathFromActorLoadPath(_ObjectLoadPath));
        }

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical("box");
        GUI.color = Color.yellow;

        GUILayout.Label("提示:关联资源必须位于Assets/Resources/文件夹下");

        GUI.color = Color.white;
        GUILayout.EndVertical();

        GameObject nc = (GameObject)EditorGUILayout.ObjectField(new GUIContent("关联物件", "关联后运行时自动创建该特效实例"), ObjectPrefab, typeof(GameObject), false);
        if (nc != ObjectPrefab)
        {
            ObjectPrefab = nc;
            string newAssetPath = AssetDatabase.GetAssetPath(nc);
            _target.ref_SetField_Inst_NonPublic("_ObjectLoadPath", _getActorLoadPath(newAssetPath));
            Repaint();
        }

        if (ObjectPrefab)
        {
            if (GUILayout.Button(new GUIContent("-", "删除关联物件"), GUILayout.Width(50)))
            {
                ObjectPrefab = null;
                _ObjectLoadPath = null;
                _target.ref_SetField_Inst_NonPublic("_ObjectLoadPath", null);
                Repaint();
            }

            GUILayout.EndHorizontal();

            //创建龙套特效按钮
            draw_createEditorObject(ObjectPrefab);

        }
        else
        {
            GUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();

    }

    private void draw_createEditorObject(GameObject EffectPrefab)
    {
        GUI.color = Color.green;
        if (GUILayout.Button(new GUIContent("创建物件用于动作编辑")))
        {
            if (EditorUtility.DisplayDialog("提示", "将创建一个关联物件用于动作编辑，此特效仅仅用于编辑中方便观察角色动作，请在动作编辑完成后手动删除该特效节点再保存预制体。", "明白了", "不明白"))
            {
                string ActorName = EffectPrefab.name;
                //检查是否存在
                bool isEx = false;
                GameObject pre = null;
                int i, len = _target.transform.childCount;
                for (i = 0; i < len; i++)
                {
                    GameObject ck = _target.transform.GetChild(i).gameObject;
                    if (ck.name == ActorName)
                    {
                        EditorUtility.DisplayDialog("提示", "关联物件已存在!", "确定");
                        Selection.activeGameObject = ck;
                        pre = ck;
                        isEx = true;
                        break;
                    }
                }

                if (!isEx)
                {
                    pre = GameObject.Instantiate(EffectPrefab);
                    pre.name = ActorName;
                    //                        pre.layer = LayerMask.NameToLayer("Role");
                    pre.transform.SetParent(_target.transform);
                    pre.transform.localPosition = Vector3.zero;

                    Selection.activeGameObject = pre;
                }

                //检查 龙套
                CinemaLongtao longtao = pre.GetComponent<CinemaLongtao>();
                if (longtao == null)
                {
                    longtao = pre.AddComponent<CinemaLongtao>();
                }
            }
            else
            {
                EditorUtility.DisplayDialog("信息", "请在RTX上找 谢超 进行咨询。", "我知道了");
            }
        }
        GUI.color = Color.white;
    }

}
