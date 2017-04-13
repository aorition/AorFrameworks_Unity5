using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using YoukiaCore;
using YoukiaUnity;
using YoukiaUnity.CinemaSystem;


[CustomEditor(typeof(CinemaCharacter))]
public class CinemaCharacterEditor : Editor
{

    private void SaveConfigData(Dictionary<string, int> animationDic)
    {

        List<AnimationClipConfig> configs = new List<AnimationClipConfig>();

        foreach (KeyValuePair<string, int> pair in animationDic)
        {
            Dictionary<string, object> updateDic = new Dictionary<string, object>();
            updateDic.Add("AnimationName", pair.Key);
            updateDic.Add("id", pair.Value);
            updateDic.Add("describe", "AnimationName");
            AnimationClipConfig src = new AnimationClipConfig();
            src = ConfigSaveUtility.updateConfigValues(src, updateDic);
            configs.Add(src);

        }

        ConfigSaveUtility.saveAllConfigToConfigSys(configs.ToArray(), true);
        AssetDatabase.Refresh();
    }

    private static Dictionary<string, int> GetConfigData()
    {
        Dictionary<string, int> dic = new Dictionary<string, int>();
        AnimationClipConfig[] srcs = ConfigSaveUtility.getAllCongfigInst<AnimationClipConfig>();

        if (srcs != null)
        {
            for (int i = 0; i < srcs.Length; i++)
            {

                dic.Add(srcs[i].AnimationName, (int)srcs[i].id);
            }
        }
        else
        {
            Debug.LogError("config不能加载");
        }

        if (dic.Count > 0)
        {
            return dic;
        }
        else
        {
            return null;
        }
    }

    private bool animationClipEditMode = false;

    protected static Color KEYABLECOLOR = new Color(1f, 0.8f, 0f, 1f);
    
    private CinemaCharacter _target;

    //private bool animationClipEditMode = false;
    private string newItem;

    protected void Awake()
    {
        _target = target as CinemaCharacter;


        //检查 公共配置表是否使用Config
        if (CinemaRTBActorBehavior.UseAnimationClipConfig)
        {
            CinemaRTBActorBehavior.AnimationClipConfigDic = GetConfigData();
        }

        if (EditorPlusMethods.AddUsedTag("CinemaCharacterEditorOnCurveWasModified") == 1)
        {
            AnimationUtility.onCurveWasModified += onCurveWasModified;
        }
    }

    private void OnEnable()
    {
//        Animator[] animators = (target as Animator).GetComponentsInParent<Animator>();
//
//        //animators会包括自己
//        if (animators.Length == 1 && animators[0] == target)
//            EditorSimulateManager.Instance.AddSimulateObj(target as Animator);

    }

    protected void OnDestroy()
    {
        if (_target == null)
        {
            if (EditorPlusMethods.SubUsedTag("CinemaCharacterEditorOnCurveWasModified") <= 0)
            {
                AnimationUtility.onCurveWasModified -= onCurveWasModified;
            }
        }
    }

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

//    private EditorApplication.CallbackFunction UDDoOnce;
    private bool _curveIsModify = false;
    private bool _curveModifyIgnore = false;
    private void onCurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType type)
    {

        if (_curveIsModify) return;

        if (_curveModifyIgnore)
        {
            _curveIsModify = true;

            EditorPlusMethods.NextEditorApplicationUpdateDo(() =>
            {
                _curveIsModify = false;
                _curveModifyIgnore = false;
            });
//            UDDoOnce = () =>
//            {
//                EditorApplication.update -= UDDoOnce;
//                //
//                _curveIsModify = false;
//                _curveModifyIgnore = false;
//            };
//            EditorApplication.update += UDDoOnce;
            return;
        }

        if (binding.type == typeof (CinemaCharacter) && binding.propertyName == "BehaviorID")
        {

            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
            if (curve != null)
            {

                _curveIsModify = true;

                AnimationCurve newCurve = new AnimationCurve();
                foreach (Keyframe keyframe in curve.keys)
                {
                    Keyframe k = new Keyframe(keyframe.time, keyframe.value, float.PositiveInfinity,
                        float.PositiveInfinity);
                    k.tangentMode = (int) KeyframeTangentMode.ConstantBoth;

                    newCurve.AddKey(k);
                }

                AnimationUtility.SetEditorCurve(clip, binding, newCurve);
                AssetDatabase.SaveAssets();

                _curveModifyIgnore = true;
                _curveIsModify = false;

            }

        }
    }

    private void BuildAnimBehaviorListSerialize_global()
    {
        if (CinemaRTBActorBehavior.UseAnimationClipConfig)
        {

            foreach (KeyValuePair<string, int> each in CinemaRTBActorBehavior.AnimationClipConfigDic)
            {
                _AnimBehaviorList.Add(each.Key);
            }
        }
        else
        {

            _AnimBehaviorList.AddRange(CinemaRTBActorBehavior.ExportAllRTBActorBehaviorNames());
        }

        //生成Target._BehaviorNameList序列化数据 ** 一定不能自动清除已经序列化的数据
        //_target.ref_SetField_Inst_NonPublic("_BehaviorNameList", null);

        useRuntimeActorBehaviorList = true;

    }

    private void BuildAnimBehaviorListSerialize_private(GameObject ActorPrefab, bool hasTargetArray, ref List<string> saveList)
    {
        Animator animator = ActorPrefab.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            AnimatorController animatorController = (AnimatorController)animator.runtimeAnimatorController;
            if (animatorController != null)
            {
                AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;
                if (stateMachine != null)
                {
                    int i, len = stateMachine.states.Length;

                    List<string> stageList = new List<string>();

                    for (i = 0; i < len; i++)
                    {
                        //获取state
                        AnimatorState state = stateMachine.states[i].state;
                        if (state != null && state.motion != null)
                        {
                            stageList.Add(state.name);
                        }
                    }

                    //移除废弃序列化数据 *  不自定移除已序列化的动作名
//                    len = saveList.Count;
//                    for (i = 0; i < len; i++)
//                    {
//                        if (!stageList.Contains(saveList[i]))
//                        {
//                            saveList[i] = null;
//                        }
//                    }

                    //添加新StateName
                    len = stageList.Count;
                    for (i = 0; i < len; i++)
                    {
                        int index;
                        string behaviorName = stageList[i];

                        if (hasTargetArray)
                        {
                            if (!saveList.Contains(behaviorName))
                            {
                                if (saveList.Contains(null))
                                {
                                    index = saveList.IndexOf(null);
                                    saveList[index] = behaviorName;
                                }
                                else
                                {
                                    saveList.Add(behaviorName);
                                }
                            }
                        }
                        else
                        {
                            saveList.Add(behaviorName);
                        }
                    }

                }
            }
        }

        //生成Target._BehaviorNameList序列化数据
        if (saveList.Count > 0)
        {
            string[] newList = saveList.ToArray();
            _target.ref_SetField_Inst_NonPublic("_BehaviorNameList", newList);
        }

        useRuntimeActorBehaviorList = false;

    }

    private List<string> _AnimBehaviorList;
    private void BuildingAnimBehaviorList()
    {

        _AnimBehaviorList = new List<string>();
        
        if (!string.IsNullOrEmpty(_target.ActorLoadPath) && !_UseActorIDPreBinding)
        {

            bool hasTargetArray = false;
            //尝试获取已序列化数据
            string[] targetArray = (string[])_target.ref_GetField_Inst_NonPublic("_BehaviorNameList");
            if (targetArray != null && targetArray.Length > 0)
            {
                _AnimBehaviorList.AddRange(targetArray);
                hasTargetArray = true;
            }

            BuildAnimBehaviorListSerialize_private(ActorPrefab, hasTargetArray, ref _AnimBehaviorList);

        }else if (_UseActorIDPreBinding)
        {
            //尝试获取已序列化数据
            string[] targetArray = (string[])_target.ref_GetField_Inst_NonPublic("_BehaviorNameList");
            if (targetArray != null && targetArray.Length > 0)
            {
                _AnimBehaviorList.AddRange(targetArray);
            }
            else
            {
                //序列化数据已经丢失，尝试补救
                string ActorPath = (string) _target.ref_GetField_Inst_NonPublic("_ActorLoadPath");
                if (!string.IsNullOrEmpty(ActorPath))
                {
                    GameObject assetsGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(ActorPath);
                    if (assetsGameObject)
                    {
                        BuildAnimBehaviorListSerialize_private(assetsGameObject, false, ref _AnimBehaviorList);
                    }
                    else
                    {
                        BuildAnimBehaviorListSerialize_global();
                    }
                }
                else
                {
                    BuildAnimBehaviorListSerialize_global();
                }

            }

        }
        else
        {
            BuildAnimBehaviorListSerialize_global();
        }

    }

    private int getAnimIDbyName(string behaviorName)
    {
        if (_AnimBehaviorList.Contains(behaviorName))
        {
            return _AnimBehaviorList.IndexOf(behaviorName);
        }
        return -1;
    }

    private string getAnimNameByID(int behaviorID)
    {
        if (behaviorID >= 0 && behaviorID < _AnimBehaviorList.Count)
        {
            return _AnimBehaviorList[behaviorID];
        }
        return null;
    }

    private GameObject ActorPrefab;

    private Vector2 _srollVector2;
    private bool useRuntimeActorBehaviorList = false;
    private bool _UseActorIDPreBinding = false;
    private bool _HideBinding = false;
    private string _ActorLoadPath;
    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            base.OnInspectorGUI();
            return;
        }

        SerializedObject m_Object = new SerializedObject(target);

        _UseActorIDPreBinding = m_Object.FindProperty("_UseActorIDPreBinding").boolValue;
        _ActorLoadPath = m_Object.FindProperty("_ActorLoadPath").stringValue;
        _HideBinding = m_Object.FindProperty("_HideBindingActorPrefab").boolValue;

        if (!ActorPrefab && !string.IsNullOrEmpty(_ActorLoadPath))
        {
            ActorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_getAssetPathFromActorLoadPath(_ActorLoadPath));
        }

        if (!_UseActorIDPreBinding)
        {
            GUILayout.BeginHorizontal();

            GameObject nc =
                (GameObject)
                    EditorGUILayout.ObjectField(new GUIContent("关联角色", "关联后运行时自动创建该人物实例"), ActorPrefab,
                        typeof (GameObject), false);

            if (nc != ActorPrefab)
            {
                ActorPrefab = nc;
                string newAssetPath = AssetDatabase.GetAssetPath(nc);
                _target.ref_SetField_Inst_NonPublic("_ActorLoadPath", _getActorLoadPath(newAssetPath));
                Repaint();
            }

            if (ActorPrefab)
            {
                if (GUILayout.Button(new GUIContent("-", "删除关联角色"), GUILayout.Width(50)))
                {
                    ActorPrefab = null;
                    _ActorLoadPath = null;
                    _target.ref_SetField_Inst_NonPublic("_ActorLoadPath", null);
                    Repaint();
                }
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            GUI.color = Color.yellow;
            //取消 预关联角色模式
            bool newchange = EditorGUILayout.Toggle(new GUIContent("已指定预关联角色"), _UseActorIDPreBinding);
            if (newchange != _UseActorIDPreBinding)
            {
                if (EditorUtility.DisplayDialog("警告", "取消预关联角色模式将导致之前记录的动画内容错误，你确定取消该模式？", "我确定", "不清楚"))
                {
                    _AnimBehaviorList = null;
                    _target.ref_SetField_Inst_NonPublic("_BehaviorNameList", null);
                    _UseActorIDPreBinding = false;
                    _target.ref_SetField_Inst_NonPublic("_UseActorIDPreBinding", false);
                    Repaint();
                }
            }
            GUI.color = Color.white;

            EditorGUILayout.HelpBox("你正在使用预关联运行时角色模式，如不清楚该模式的用途。请RTX上找谢超咨询.", MessageType.Warning);

            GameObject nc = (GameObject)EditorGUILayout.ObjectField("预关联角色", ActorPrefab, typeof(GameObject), false);
            if (nc != ActorPrefab)
            {
                EditorUtility.DisplayDialog("提示", "预关联运行时角色不允许替换，如有需要请取消此模式后替换角色。", "我知道啦");
            }

            //创建龙套角色按钮
//            draw_createEditorActor(ActorPrefab);
        }

        if (ActorPrefab)
        {

            if (useRuntimeActorBehaviorList)
            {
                BuildingAnimBehaviorList();
                Repaint();
            }

            //创建龙套角色按钮
            draw_createEditorActor(ActorPrefab);

            if (!_UseActorIDPreBinding)
            {
                //启用 预关联角色模式
                bool newchange = EditorGUILayout.Toggle(new GUIContent("预关联运行时角色模式"), _UseActorIDPreBinding);
                if (newchange != _UseActorIDPreBinding)
                {
                    if (EditorUtility.DisplayDialog("提示", "确定启用预关联运行时角色模式？", "我确定", "不清楚"))
                    {
                        _UseActorIDPreBinding = true;
                        _target.ref_SetField_Inst_NonPublic("_UseActorIDPreBinding", true);
                        Repaint();
                    }
                }

                if (!_HideBinding)
                {
                    bool newchange2 = EditorGUILayout.Toggle(new GUIContent("隐藏运行时角色"), _HideBinding);
                    if (newchange2 != _HideBinding)
                    {
                        _HideBinding = true;
                        _target.ref_SetField_Inst_NonPublic("_HideBindingActorPrefab", true);
                        Repaint();
                    }
                }
                else
                {
                    bool newchange2 = EditorGUILayout.Toggle(new GUIContent("隐藏运行时角色"), _HideBinding);
                    if (newchange2 != _HideBinding)
                    {
                        _HideBinding = false;
                        _target.ref_SetField_Inst_NonPublic("_HideBindingActorPrefab", false);
                        Repaint();
                    }
                }

            }
        }

        //初始化_AnimBehaviorList
        if (_AnimBehaviorList == null)
        {
            BuildingAnimBehaviorList();
            Repaint();
        }

        //选择人物动作部分
        //   CinemaCharacter.eActorBehavior be = (CinemaCharacter.eActorBehavior)EditorGUILayout.EnumPopup(new GUIContent("动作", "触发人物动作"), (CinemaCharacter.eActorBehavior)Mathf.Floor(beID));
        //   SerializedProperty p = m_Object.FindProperty("BehaviorID");
        int beID = (int)_target.BehaviorID;

        string[] animList = _AnimBehaviorList.ToArray();
        
        string animName = getAnimNameByID(beID);

        int index = 0;
        for (int i = 0; i < animList.Length; i++)
        {
            if (animList[i] == animName)
            {
                index = i;
                break;
            }

        }

        if (_UseActorIDPreBinding || !ActorPrefab || _HideBinding)
        {
            EditorGUILayout.PropertyField(m_Object.FindProperty("ActorID"), new GUIContent("运行时关联指令"));
        }

        GUI.color = KEYABLECOLOR;

        index = EditorGUILayout.Popup("人物动作", index, animList);

        int newBeID = getAnimIDbyName(animList[index]);

        if (!this.isFoucsAnimtionWindow())
        {
            if (newBeID != beID)
            {
                m_Object.FindProperty("BehaviorID").floatValue = (float)newBeID;

            }
        }

        if (CinemaRTBActorBehavior.UseAnimationClipConfig)
        {

            //添加动作部分
            if (GUILayout.Button("编辑动作列表"))
            {
                animationClipEditMode = !animationClipEditMode;

            }

            if (!Application.isEditor || Application.isPlaying)
            {
                animationClipEditMode = false;
            }

            if (animationClipEditMode && CinemaRTBActorBehavior.AnimationClipConfigDic != null)
            {

                EditorGUILayout.HelpBox("删除动作会导致已经做好的动画剪辑内的人物丢失动画行为!", MessageType.Warning);


                string delKey = "";
                _srollVector2 = GUILayout.BeginScrollView(_srollVector2);
                foreach (KeyValuePair<string, int> each in CinemaRTBActorBehavior.AnimationClipConfigDic)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("ID:" + (each.Value > 9 ? each.Value.ToString() : ("0" + each.Value)) +
                                               "  " + each.Key);

                    if (GUILayout.Button("删除"))
                    {

                        delKey = each.Key;

                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
                EditorGUILayout.BeginHorizontal();
                newItem = EditorGUILayout.TextField(newItem);
                if (GUILayout.Button("添加") && !string.IsNullOrEmpty(newItem))
                {

                    int idx = 0;
                    for (int i = 1; i <= 100; i++)
                    {
                        if (!CinemaRTBActorBehavior.AnimationClipConfigDic.ContainsValue(i))
                        {
                            idx = i;
                            break;
                        }
                    }


                    CinemaRTBActorBehavior.AnimationClipConfigDic.Add(newItem, idx);
                    newItem = "";
                }
                EditorGUILayout.EndHorizontal();
                
                if (!string.IsNullOrEmpty(delKey))
                    CinemaRTBActorBehavior.AnimationClipConfigDic.Remove(delKey);

                EditorGUILayout.BeginHorizontal();

                GUI.color = Color.yellow;
                if (GUILayout.Button("保存配置"))
                {
                    SaveConfigData(CinemaRTBActorBehavior.AnimationClipConfigDic);
                    BuildingAnimBehaviorList();
                    Repaint();
                }

                GUI.color = Color.green;
                if (GUILayout.Button("重读配置"))
                {
                    BuildingAnimBehaviorList();
                    Repaint();
                }

                EditorGUILayout.EndHorizontal();

            }

        }

        GUI.color = Color.white;

        EditorGUILayout.PropertyField(m_Object.FindProperty("FollowPrefab"), new GUIContent("反向跟随", "这个点位跟随关联的预制体"));

        EditorGUILayout.PropertyField(m_Object.FindProperty("UseModelShadowProjector"), new GUIContent("阴影投射器", "勾选此项可为角色生成影子"));

        m_Object.ApplyModifiedProperties();

    }

    private void draw_createEditorActor(GameObject ActorPrefab)
    {
        GUI.color = Color.green;
            if (GUILayout.Button(new GUIContent("创建角色用于动作编辑")))
            {
                if (EditorUtility.DisplayDialog("提示", "将创建一个关联角色用于动作编辑，此角色仅仅用于编辑中方便观察角色动作，请在动作编辑完成后手动删除该角色节点再保持预制体。", "明白了", "不明白"))
                {
                    string ActorName = ActorPrefab.name;
                    //检查是否存在
                    bool isEx = false;
                    GameObject pre = null;
                    int i, len = _target.transform.childCount;
                    for (i = 0; i < len; i++)
                    {
                        GameObject ck = _target.transform.GetChild(i).gameObject;
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
                        pre = GameObject.Instantiate(ActorPrefab);
                        pre.name = ActorName;
                        pre.layer = LayerMask.NameToLayer("character");
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
                    EditorUtility.DisplayDialog("信息","请在RTX上找 谢超 进行咨询。","我知道了");
                }
            }
            GUI.color = Color.white;
    }

}
