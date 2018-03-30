using System;
using System.Collections.Generic;
using System.Reflection;
using AorBaseUtility;
using Framework.Graphic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using YoukiaUnity.CinemaSystem;

[CustomEditor(typeof(CinemaClip))]
public class CinemaClipEditor : UnityEditor.Editor
{

    private ICinemaBridge _bridge;
    private CinemaClip _target;
    private Animator _animator;

    private void Awake()
    {
        _target = this.target as CinemaClip;
        _animator = _target.gameObject.GetComponent<Animator>();
    }

    public override void OnInspectorGUI()
    {
        
        if (_target == null) return;

        if (Application.isPlaying)
        {
            base.OnInspectorGUI();
            return;
        }

        _linkAllChildElements();

        _bridge = _target.gameObject.GetInterface<ICinemaBridge>();
        if (_bridge == null && string.IsNullOrEmpty(_target.BridgeAutoLoadKey))
        {

            #region Vertical

            GUILayout.BeginVertical("helpbox");
            GUI.color = Color.red;

            EditorGUILayout.HelpBox("注意！CinemaClip缺少CinemaBridge组件，或将导致角色在运行时行为丢失或异常！！", MessageType.Warning, true);

            GUI.color = Color.white;
            GUILayout.EndVertical();

            #endregion

        }

        if (_bridge == null && !string.IsNullOrEmpty(_target.BridgeAutoLoadKey))
        {

            #region Horizontal

            GUILayout.BeginHorizontal("box");

            #region Vertical

            GUILayout.BeginVertical();

            int li = _target.BridgeAutoLoadKey.LastIndexOf('.');
            string lb = (li != -1 ? _target.BridgeAutoLoadKey.Substring(li + 1) : _target.BridgeAutoLoadKey);

            GUILayout.Label("linkBridge : " + lb);

            if (!string.IsNullOrEmpty(_target.BridgeAutoLoadValueString))
            {
                GUILayout.Label("Values   : " + _target.BridgeAutoLoadValueString);
            }

            GUILayout.EndVertical();

            #endregion
            
            if (_bridge == null)
            {
                GUILayout.FlexibleSpace();
                GUI.color = Color.yellow;
                if (GUILayout.Button("addBridge", GUILayout.Width(80)))
                {
                    _target.ref_InvokeMethod_Inst_NonPublic("getBridge", null);
                    _bridge = _target.gameObject.GetInterface<ICinemaBridge>();
                }
                GUI.color = Color.white;
            }


            GUILayout.EndHorizontal();

            #endregion

        }
        
        //编辑器模式下（非编辑器运行时）强制对象保持世界坐标原点
        if (_target.KeepRootPosToOrigin)
        {
            _target.transform.SetParent(null);
            _target.transform.localScale = Vector3.one;
            _target.transform.localRotation = Quaternion.identity;
            _target.transform.localPosition = Vector3.zero;
        }

        if (_animator != null && Application.isPlaying == false)
        {
            _animator.enabled = false;
        }

        serializedObject.Update();

        if (_bridge != null)
        {

            serializedObject.FindProperty("BridgeAutoLoadKey").stringValue = _bridge.GetType().FullName;

            //创建序列化数据
            serializedObject.FindProperty("BridgeAutoLoadValueString").stringValue = _bridge.ExportValuesString();

        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("HideCurrentSceneOnLowRenderQuality"), new GUIContent("在低配模式下隐藏场景", "在低质量渲染模式下会自动隐藏当前场景"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("KeepRootPosToOrigin"), new GUIContent("保持原点位置", "是否保持在世界坐标原点上"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoPlay"), new GUIContent("自动开始", "创建后自动开始播放该剪辑"));

        List<CinemaCharacter> ActorList = (List<CinemaCharacter>)_target.ref_GetField_Inst_Public("ActorList");
        List<CinemaEffect> EffectList = (List<CinemaEffect>)_target.ref_GetField_Inst_Public("EffectList");
        if ((ActorList != null && ActorList.Count > 0) || (EffectList != null && EffectList.Count > 0))
        {
            draw_createEditorActorOrEffects();
        }

        serializedObject.ApplyModifiedProperties();

        Transform root = _target.transform;

        GUI.color = Color.yellow;
        if (GUILayout.Button("添加角色"))
        {

            GameObject obj = new GameObject("Actor_" + Time.realtimeSinceStartup.GetHashCode());
            obj.AddComponent<CinemaCharacter>();

            obj.transform.parent = root;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localEulerAngles = Vector3.zero;
        }

        if (GUILayout.Button("添加特效"))
        {

            GameObject obj = new GameObject("Effect_" + Time.realtimeSinceStartup.GetHashCode());
            obj.AddComponent<CinemaEffect>();

            obj.transform.parent = root;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localEulerAngles = Vector3.zero;

        }

        if (GUILayout.Button("添加物件"))
        {

            GameObject obj = new GameObject("Object_" + Time.realtimeSinceStartup.GetHashCode());
            obj.AddComponent<CinemaObject>();

            obj.transform.parent = root;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localEulerAngles = Vector3.zero;

        }

        if (GUILayout.Button("添加动态挂点"))
        {

            GameObject obj = new GameObject("SubMountPoint_" + Time.realtimeSinceStartup.GetHashCode());
            obj.AddComponent<CinemaSubMountPoint>();

            obj.transform.parent = root;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localEulerAngles = Vector3.zero;

        }

        if (GUILayout.Button("添加摄像机"))
        {

            GameObject obj = new GameObject("Camera_" + Time.realtimeSinceStartup.GetHashCode());
            
            Camera cam = obj.AddComponent<Camera>();
//            SubCameraInfo caminfo = obj.AddComponent<SubCameraInfo>();
//            caminfo.Level = 100;
            VisualCamera vcam = obj.AddComponent<VisualCamera>();
            vcam.Level = 100;
            
            cam.fieldOfView = 45;
            cam.nearClipPlane = 1f;
            cam.clearFlags = CameraClearFlags.Depth;
            obj.transform.parent = root;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localEulerAngles = Vector3.zero;

        }
        GUI.color = Color.white;

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
            
            if (strs != null && strs.Length > 0)
            {
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

        }
        
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

                CinemaEventsWindow.init(bridge.GetType());
            }

            GUI.color = Color.white;
        }

        List<CinemaLongtao> lts = _target.transform.FindComponentListInChildren<CinemaLongtao>();
        if ((lts != null && lts.Count > 0))
        {

            #region Vertical

            GUILayout.BeginVertical("box");

            GUI.color = Color.yellow;
            if (GUILayout.Button("一键清理龙套"))
            {
                int i, len = lts.Count;
                for (i = 0; i < len; i++)
                {
                    DestroyImmediate(lts[i].gameObject);
                }
            }
            GUI.color = Color.white;

            GUILayout.EndVertical();

            #endregion
            
        }

       Repaint();
    }

    private void draw_createEditorActorOrEffects()
    {
        GUI.color = Color.green;
        if (GUILayout.Button(new GUIContent("创建龙套角色/特效用于动作编辑", "运行时绑定的不能自定创建")))
        {
            if (EditorUtility.DisplayDialog("提示", "将创建一系列关联角色/特效用于动作编辑，此角色仅仅用于编辑中方便观察角色动作，请在动作编辑完成后手动删除该角色/特效节点再保持预制体。", "明白了", "不明白"))
            {

                List<CinemaCharacter> ActorList = (List<CinemaCharacter>)_target.ref_GetField_Inst_Public("ActorList");
                if (ActorList != null && ActorList.Count > 0)
                {

                    foreach (CinemaCharacter character in ActorList)
                    {
                        if (character != null)
                        {
                            GameObject actorPrefab = null;
                            string path = "Assets/Resources/" + character.ActorLoadPath + ".prefab";
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

                                pre = new GameObject(ActorName);
                                pre.layer = LayerMask.NameToLayer("Role");
                                pre.transform.SetParent(character.transform, false);

                                GameObject loadGO = GameObject.Instantiate(actorPrefab);
                                loadGO.name = ActorName;
                                loadGO.layer = LayerMask.NameToLayer("Role");
                                loadGO.transform.SetParent(pre.transform);
                                loadGO.transform.localRotation = Quaternion.identity;
                                loadGO.transform.localScale = Vector3.one;
                                loadGO.transform.localPosition = Vector3.zero;

//                                Selection.activeGameObject = pre;
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

                List<CinemaEffect> EffectList = (List<CinemaEffect>)_target.ref_GetField_Inst_Public("EffectList");
                if (EffectList != null && EffectList.Count > 0)
                {

                    foreach (CinemaEffect effect in EffectList)
                    {
                        if (effect != null)
                        {
                            GameObject actorPrefab = null;
                            string path = "Assets/Resources/" + effect.EffectLoadPath + ".prefab";
                            if (!string.IsNullOrEmpty(effect.EffectLoadPath))
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
                            int i, len = effect.transform.childCount;
                            for (i = 0; i < len; i++)
                            {
                                GameObject ck = effect.transform.GetChild(i).gameObject;
                                if (ck.name == ActorName)
                                {
                                    EditorUtility.DisplayDialog("提示", "关联特效已存在!", "确定");
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
//                                pre.layer = LayerMask.NameToLayer("Role");
                                pre.transform.SetParent(effect.transform, false);
                                pre.transform.localScale = Vector3.one;
                                pre.transform.localRotation = Quaternion.identity;
                                pre.transform.localPosition = Vector3.zero;

//                                Selection.activeGameObject = pre;
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

    private void _linkAllChildElements()
    {
        _target.LinkActor(null);
        _target.LinkEffects(null);
        _target.LinkObjects(null);
        _target.LinkSubMountPoints(null);
//        _target.LinkCamera();
        _target.LinkAnim();
    }

}
