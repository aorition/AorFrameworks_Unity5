using System;
using System.Collections.Generic;
using System.Reflection;
using AorFramework.module;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using YoukiaUnity.CinemaSystem;
using AorBaseUtility;

[CustomEditor(typeof(CinemaClip))]
public class CinemaClipEditor : UnityEditor.Editor
{

    // private Animator _animator;
    //private float time;

    private ICinemaBridge _bridge;
    private CinemaClip _target;

    //    private Transform _root;
    private Animator _animator;

    private void OnEnable()
    {
        //  time = 0;
        _target = this.target as CinemaClip;
        _animator = _target.gameObject.GetComponent<Animator>();
        //        _root = _target.transform.FindChild("root");
    }

    public override void OnInspectorGUI()
    {

        if (_target == null) return;

        _bridge = _target.gameObject.GetInterface<ICinemaBridge>();
        if (_bridge == null && string.IsNullOrEmpty(_target.BridgeAutoLoadKey))
        {
            GUILayout.BeginVertical("helpbox");
            GUI.color = Color.red;

            EditorGUILayout.HelpBox("注意！CinemaClip缺少CinemaBridge组件，或将导致角色在运行时行为丢失或异常！！", MessageType.Warning, true);

            GUI.color = Color.white;
            GUILayout.EndVertical();
        }

        if (_bridge == null && !string.IsNullOrEmpty(_target.BridgeAutoLoadKey))
        {
            AorGUILayout.Horizontal("box", () =>
            {
                int li = _target.BridgeAutoLoadKey.LastIndexOf('.');
                string lb = (li != -1 ? _target.BridgeAutoLoadKey.Substring(li + 1) : _target.BridgeAutoLoadKey);

                GUILayout.Label("linkBridge : " + lb);

                if(_bridge == null)
                {
                    GUILayout.FlexibleSpace();
                    GUI.color = Color.yellow;
                    if (GUILayout.Button("addBridge",GUILayout.Width(80)))
                    {
                        _target.ref_InvokeMethod_Inst_NonPublic("getBridge", null);
                        _bridge = _target.gameObject.GetInterface<ICinemaBridge>();
                    }
                    GUI.color = Color.white;
                }
            });
        }

        if (Application.isPlaying)
            return;

        //编辑器模式下（非编辑器运行时）强制对象保持世界坐标原点
        if (_target.KeepRootPosToOrigin)
        {
            _target.transform.SetParent(null);
            _target.transform.localScale = Vector3.one;
            _target.transform.localRotation = Quaternion.identity;
            _target.transform.localPosition = Vector3.zero;
        }

        //time += Time.deltaTime;
        //  base.OnInspectorGUI();
        //SerializedObject m_Object = new SerializedObject(target);
        SerializedObject m_Object = serializedObject;

        if (_animator != null && Application.isPlaying == false)
        {
            _animator.enabled = false;
        }

        //EditorGUILayout.HelpBox("不要对此物体的位移缩放和旋转K帧，保持不嵌套并且在世界坐标原点", MessageType.Warning);


        m_Object.Update();

        if (_bridge != null)
        {
            m_Object.FindProperty("BridgeAutoLoadKey").stringValue = _bridge.GetType().FullName;
        }

        EditorGUILayout.PropertyField(m_Object.FindProperty("KeepRootPosToOrigin"), new GUIContent("保持原点位置", "是否保持在世界坐标原点上"));

        //m_Object.FindProperty("AutoPlay").boolValue = 
        EditorGUILayout.PropertyField(m_Object.FindProperty("AutoPlay"), new GUIContent("自动开始", "创建后自动开始播放该剪辑"));


        //   EditorGUILayout.PropertyField(m_Object.FindProperty("PauseBattle"), new GUIContent("暂停战斗", "播放时停止战斗进程"));
        EditorGUILayout.PropertyField(m_Object.FindProperty("ActorList"), new GUIContent("角色列表", "此电影剪辑中的出场的角色"), true);

        //  EditorGUILayout.PropertyField(m_Object.FindProperty("test33"), new GUIContent("test444", "此电影剪辑中的出场的角色"), true);
        //  CutOff.PauseBattle = EditorGUILayout.Toggle(new GUIContent("暂停战斗", "播放时停止战斗进程"), CutOff.PauseBattle);

        List<CinemaCharacter> ActorList = (List<CinemaCharacter>)_target.ref_GetField_Inst_Public("ActorList");
        if (ActorList != null && ActorList.Count > 0)
        {
            draw_createEditorActors();
        }

        //        if (_root == null)
        //        {
        //            if (GUILayout.Button(new GUIContent("添加root节点", "此节点允许在世界坐标中定制位置")))
        //            {
        //                _root = new GameObject("root").transform;
        //                _root.SetParent(_target.transform, false);
        //            }
        //        }

        //        Transform root;
        //        if (_root != null)
        //        {
        //            root = _root;
        //        }
        //        else
        //        {
        //            root = _target.transform;
        //        }

        Transform root = _target.transform;

        GUI.color = Color.yellow;
        if (GUILayout.Button("添加角色"))
        {

            GameObject obj = new GameObject("Actor_" + Time.realtimeSinceStartup.GetHashCode());
            BoxCollider collider = obj.AddComponent<BoxCollider>();
            collider.center = new Vector3(0, 0.5f, 0);
            //CinemaCharacter actor = 
            obj.AddComponent<CinemaCharacter>();

            obj.transform.parent = root;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localEulerAngles = Vector3.zero;
        }
        GUI.color = Color.white;

        EditorGUILayout.PropertyField(m_Object.FindProperty("CameraList"), new GUIContent("机位列表", "此电影剪辑中的camera"), true);

        GUI.color = Color.yellow;
        if (GUILayout.Button("添加摄像机"))
        {

            GameObject obj = new GameObject("Camera_" + Time.realtimeSinceStartup.GetHashCode());


            Camera cam = obj.AddComponent<Camera>();
            SubCameraInfo caminfo = obj.AddComponent<SubCameraInfo>();
            caminfo.Level = 100;

            cam.fieldOfView = 45;
            // cam.enabled = false;
            cam.nearClipPlane = 1f;
            cam.clearFlags = CameraClearFlags.Depth;
            obj.transform.parent = root;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localEulerAngles = Vector3.zero;

        }
        GUI.color = Color.white;

        EditorGUILayout.PropertyField(m_Object.FindProperty("AnimList"), new GUIContent("动画列表", "此电影剪辑中的Animator"), true);

        string[] strs = null;
        if (_target.CameraList != null)
        {

            strs = new string[_target.CameraList.Count];

            int activeIndex = -1;
            for (int i = 0; i < _target.CameraList.Count; i++)
            {
                if (_target.CameraList[i] == null)
                    continue;

                strs[i] = _target.CameraList[i].name;
                if (_target.CameraList[i].activeInHierarchy)
                {
                    activeIndex = i;
                }
            }

            int newIndex = EditorGUILayout.Popup("当前摄像机:", activeIndex, strs);
            if (newIndex != activeIndex)
            {

                for (int i = 0; i < _target.CameraList.Count; i++)
                {


                    SerializedObject gameobj = new SerializedObject(_target.CameraList[i]);

                    gameobj.Update();

                    if (i == newIndex)
                    {
                        gameobj.FindProperty("m_IsActive").boolValue = true;
                    }
                    else
                    {

                        gameobj.FindProperty("m_IsActive").boolValue = false;
                    }

                    gameobj.ApplyModifiedProperties();

                }


            }




        }

        _target.LinkActor(null);
        _target.LinkCamera();
        _target.LinkAnim();


        if (_animator && _animator.runtimeAnimatorController != null)
        {
            GUI.color = Color.cyan;
            if (GUILayout.Button("事件编辑器"))
            {

                EditorWindow w = EditorPlusMethods.GetPlusDefindWindow(EditorPlusMethods.PlusDefindWindow.AnimationWindow);

                BaseCinemaBridge bridge = _target.gameObject.GetComponent<BaseCinemaBridge>();

                if (!bridge)
                {
                    EditorUtility.DisplayDialog("提示", "没有找到CineamBridge脚本,开启失败.", "OK");
                    return;
                }

                AnimatorController controller = _animator.runtimeAnimatorController as AnimatorController;
                if (controller)
                {
                    AnimatorStateMachine sm = controller.layers[0].stateMachine;
                    if (sm)
                    {
                        AnimatorState state = sm.defaultState;
                        if (state)
                        {
                            AnimationClip clip = state.motion as AnimationClip;

                            if (w == null)
                            {
                                CinemaEventsWindow.init(clip, bridge.GetType());
                            }
                            else
                            {
                                CinemaEventsWindow.init(clip, bridge.GetType(), w);
                            }
                        }
                    }
                }

            }
            GUI.color = Color.white;
        }


        /*
        UnityEditorInternal.AnimatorController ac = _animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
        UnityEditorInternal.StateMachine sm = ac.GetLayer(0).stateMachine;
        UnityEditorInternal.State state = sm.GetState(0);
        clip.Clip = state.GetMotion() as AnimationClip;
        */

        GUILayout.BeginVertical("box");
        //        /**
        //         * 暂时禁用 
        //        */
        CinemaLongtao[] lts = _target.GetComponentsInChildren<CinemaLongtao>();
        if ((lts != null && lts.Length > 0))
        //        if ((lts != null && lts.Length > 0))
        {
            GUI.color = Color.cyan;
            if (GUILayout.Button("一键清理龙套"))
            {
                int i, len = lts.Length;
                for (i = 0; i < len; i++)
                {
                    GameObject.DestroyImmediate(lts[i].gameObject);
                }
            }
            GUI.color = Color.white;
        }


        GUILayout.EndVertical();

        m_Object.ApplyModifiedProperties();

        this.Repaint();

    }

    private void draw_createEditorActors()
    {
        GUI.color = Color.green;
        if (GUILayout.Button(new GUIContent("创建龙套角色们用于动作编辑", "运行时绑定的不能自定创建")))
        {
            if (EditorUtility.DisplayDialog("提示", "将创建一系列关联角色用于动作编辑，此角色仅仅用于编辑中方便观察角色动作，请在动作编辑完成后手动删除该角色节点再保持预制体。", "明白了", "不明白"))
            {

                List<CinemaCharacter> ActorList = (List<CinemaCharacter>)_target.ref_GetField_Inst_Public("ActorList");
                if (ActorList != null && ActorList.Count > 0)
                {

                    foreach (CinemaCharacter character in ActorList)
                    {
                        if (character != null)
                        {
                            GameObject actorPrefab = null;
                            string path =  "Assets/Resources/" + character.ActorLoadPath + ".prefab";
                            if (!string.IsNullOrEmpty(character.ActorLoadPath))
                            {

                                actorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                                if (!actorPrefab)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }

                            string ActorName = actorPrefab.name;
                            //检查是否存在
                            bool isEx = false;
                            GameObject pre = null;
                            int i, len = character.transform.childCount;
                            for (i = 0; i < len; i++)
                            {
                                GameObject ck = character.transform.GetChild(i).gameObject;
                                if (ck.name == ActorName)
                                {
                                    EditorUtility.DisplayDialog("提示", "关联角色已存在!", "确定");
                                    Selection.activeGameObject = ck;
                                    pre = ck;
                                    isEx = true;
                                    break;
                                }
                            }

                            if (!isEx)
                            {
                                pre = GameObject.Instantiate(actorPrefab);
                                pre.name = ActorName;
                                pre.layer = LayerMask.NameToLayer("character");
                                pre.transform.SetParent(character.transform, false);
                                pre.transform.localScale = Vector3.one;
                                pre.transform.localRotation = Quaternion.identity;
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

                    }

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
