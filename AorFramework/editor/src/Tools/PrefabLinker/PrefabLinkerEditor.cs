using System.Collections.Generic;
using AorBaseUtility;
using Framework;
using UnityEngine;
using UnityEditor;
using Framework.editor;

[CustomEditor(typeof(PrefabLinker))]
public class PrefabLinkerEditor : Editor
{

    private static UnityEngine.Object LoadAssetByGUID(string guid)
    {
        return AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
    }

    private static void SaveLinkedToSource(PrefabLinker linker)
    {
        string srcPath = AssetDatabase.GUIDToAssetPath(linker.SourceGUID);
        if (!string.IsNullOrEmpty(srcPath))
        {
            UnityEngine.Object srcAsset = AssetDatabase.LoadAssetAtPath<GameObject>(srcPath);
            if (srcAsset)
            {
                linker.IsSource = true;
                PrefabUtility.ReplacePrefab(linker.gameObject, srcAsset, ReplacePrefabOptions.ReplaceNameBased);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                linker.IsSource = false;
            }

        }
    }

    private static void UpdateFormSource(PrefabLinker linker)
    {

        string srcPath = AssetDatabase.GUIDToAssetPath(linker.SourceGUID);
        if (!string.IsNullOrEmpty(srcPath))
        {

            UnityEngine.Object srcAsset = AssetDatabase.LoadAssetAtPath<GameObject>(srcPath);
            if (srcAsset)
            {

                GameObject go = GameObject.Instantiate<GameObject>((GameObject)srcAsset);
                if (go)
                {

                    go.name = srcAsset.name;
                    ReplaceLinked(linker.gameObject, go);

                }

            }

        }

    }

    /// <summary>
    /// 替换Linked对象
    /// </summary>
    /// <param name="o">被替换对象</param>
    /// <param name="n">新对象</param>
    /// <returns>返回一个GameObject用于编辑器刷新</returns>
    private static GameObject ReplaceLinked(GameObject o, GameObject n)
    {

        RectTransform ort = o.GetComponent<RectTransform>();
        bool isRt = false;

        Vector3 pivot_cache = Vector3.zero;
        Vector3 anchorMin_cache = Vector3.zero;
        Vector3 anchorMax_cache = Vector3.zero;
        Vector3 anchoredPosition3D_cache = Vector3.zero;
        Vector2 sizeDelta_cache = Vector2.zero;
        if (ort)
        {
            isRt = true;
            pivot_cache = ort.pivot;
            anchorMin_cache = ort.anchorMin;
            anchorMax_cache = ort.anchorMax;
            anchoredPosition3D_cache = ort.anchoredPosition3D;
            sizeDelta_cache = ort.sizeDelta;
        }

        int siblingIndex_cache = o.transform.GetSiblingIndex();
        bool hasParent = (o.transform.parent != null);
        Transform parent_cache = o.transform.parent;

        string name_cache = o.name;

        Vector3 T_cache = o.transform.localPosition;
        Vector3 R_cache = o.transform.localEulerAngles;
        Vector3 S_cache = o.transform.localScale;

        string tag_cache = o.gameObject.tag;
        int layer_cache = o.gameObject.layer;
        bool static_cache = o.gameObject.isStatic;

        PrefabLinker pl = n.GetComponent<PrefabLinker>();
        pl.IsSource = false;

        if (Application.isPlaying)
        {
            GameObject.Destroy(o);
        }
        else
        {
            GameObject.DestroyImmediate(o);
        }

        //        EditorPlusMethods.NextEditorApplicationUpdateDo(() =>
        //        {
        //            if (Application.isPlaying)
        //            {
        //                GameObject.Destroy(o);
        //            }
        //            else
        //            {
        //                GameObject.DestroyImmediate(o);
        //            }
        //        });

        n.name = name_cache;

        if (hasParent) n.transform.SetParent(parent_cache, false);
        n.transform.SetSiblingIndex(siblingIndex_cache);

        n.isStatic = static_cache;
        n.layer = layer_cache;
        n.tag = tag_cache;

        n.transform.localScale = S_cache;
        n.transform.localEulerAngles = R_cache;
        n.transform.localPosition = T_cache;

        if (isRt)
        {
            RectTransform rt = n.GetComponent<RectTransform>();
            rt.pivot = pivot_cache;
            rt.anchorMin = anchorMin_cache;
            rt.anchorMax = anchorMax_cache;
            rt.anchoredPosition3D = anchoredPosition3D_cache;
            rt.sizeDelta = sizeDelta_cache;
        }

        return hasParent ? parent_cache.gameObject : n;
    }

    //-------------------------------------------------

    private static bool _CheckHasLinkedLoop(Transform tran, PrefabLinker source)
    {
        PrefabLinker lk = tran.GetComponent<PrefabLinker>();
        if (lk && !lk.IsSource && lk.SourceGUID == source.SourceGUID)
        {
            return true;
        }
        if (tran.childCount > 0)
        {
            for (int i = 0; i < tran.childCount; i++)
            {
                Transform sub = tran.GetChild(i);
                if (_CheckHasLinkedLoop(sub, source)) return true;
            }
        }
        return false;
    }

    private static bool _ReplaceLinkedLoop(Transform tran, PrefabLinker source)
    {
        PrefabLinker lk = tran.GetComponent<PrefabLinker>();
        if (lk && !lk.IsSource && lk.SourceGUID == source.SourceGUID)
        {

            Transform p = tran.parent;
            ReplaceLinked(tran.gameObject, GameObject.Instantiate(source.gameObject));

            return true;
        }

        if (tran.childCount > 0)
        {
            for (int i = 0; i < tran.childCount; i++)
            {
                Transform sub = tran.GetChild(i);
                if (_ReplaceLinkedLoop(sub, source)) return true;
            }
        }
        return false;
    }

    private static void UpdateAllLinked(PrefabLinker linker)
    {

        //处理Hierarchy
        int i, len;
        PrefabLinker[] HierarchyPLs = GameObject.FindObjectsOfType<PrefabLinker>();

        if (HierarchyPLs != null && HierarchyPLs.Length > 0)
        {
            len = HierarchyPLs.Length;
            for (i = 0; i < len; i++)
            {
                if (HierarchyPLs[i].SourceGUID == linker.SourceGUID)
                {
                    EditorUtility.SetDirty(ReplaceLinked(HierarchyPLs[i].gameObject, GameObject.Instantiate(linker.gameObject)));
                }
            }
        }

        //获取所有Prefabs
        List<EditorAssetInfo> allEAInfos = EditorAssetInfo.FindEditorAssetInfoInPath(Application.dataPath, "*.prefab");
        bool isDirty = false;
        List<GameObject> hasLikers = new List<GameObject>();
        len = allEAInfos.Count;
        for (i = 0; i < len; i++)
        {
            EditorUtility.DisplayProgressBar("运行中", "正在收集资源,请耐心等待.." + i + " / " + len, (float)i / len);
            GameObject tmpLoadAsset = AssetDatabase.LoadAssetAtPath<GameObject>(allEAInfos[i].path);
            if (tmpLoadAsset)
            {
                if (_CheckHasLinkedLoop(tmpLoadAsset.transform, linker))
                {
                    hasLikers.Add(tmpLoadAsset);
                }
            }
        }

        len = hasLikers.Count;
        for (i = 0; i < len; i++)
        {
            EditorUtility.DisplayProgressBar("运行中", "正在更新资源,请耐心等待.." + i + " / " + len, (float)i / len);
            GameObject ins = GameObject.Instantiate(hasLikers[i]);
            if (ins)
            {
                if (_ReplaceLinkedLoop(ins.transform, linker))
                {
                    PrefabUtility.ReplacePrefab(ins, hasLikers[i], ReplacePrefabOptions.ReplaceNameBased);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            if (Application.isPlaying)
            {
                GameObject.Destroy(ins);
            }
            else
            {
                GameObject.DestroyImmediate(ins);
            }

        }

        EditorUtility.ClearProgressBar();

    }

    //-------------------------------------------------

    private PrefabLinker _target;

    private void Awake()
    {
        _target = target as PrefabLinker;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        bool isSource = (bool)_target.ref_GetField_Inst_Public("IsSource");
        PrefabType pt = PrefabUtility.GetPrefabType(_target);
        if (isSource)
        {
            switch (pt)
            {
                case PrefabType.Prefab:
                case PrefabType.ModelPrefab:
                    string p = AssetDatabase.GetAssetPath(_target);
                    string guid = AssetDatabase.AssetPathToGUID(p);
                    bool nIsSource = EditorGUILayout.Toggle(new GUIContent("IsSource"), isSource);
                    if (nIsSource != isSource)
                    {
                        _target.ref_SetField_Inst_Public("IsSource", nIsSource);
                    }
                    _target.ref_SetField_Inst_Public("SourceGUID", guid);
                    Draw_SourceUI();
                    break;
                default:
                    PrefabUtility.DisconnectPrefabInstance(_target);
                    _target.ref_SetField_Inst_Public("IsSource", false);
                    Draw_InstanceUI();
                    break;
            }

        }
        else
        {
            switch (pt)
            {
                case PrefabType.Prefab:
                case PrefabType.ModelPrefab:
                    bool nIsSource = EditorGUILayout.Toggle(new GUIContent("IsSource"), isSource);
                    if (nIsSource != isSource)
                    {
                        _target.ref_SetField_Inst_Public("IsSource", nIsSource);
                    }
                    Draw_InstanceUI(false);
                    break;
                default:
                    if (linksSource)
                    {
                        PrefabUtility.DisconnectPrefabInstance(_target);
                        _target.ref_SetField_Inst_Public("IsSource", false);
                        Draw_InstanceUI();
                    }
                    else
                    {
                        Draw_warring();
                    }
                    break;
            }

        }

    }

    private UnityEngine.Object _linksSource;
    protected UnityEngine.Object linksSource
    {
        get
        {
            if (!_linksSource)
            {
                string srcPath = AssetDatabase.GUIDToAssetPath((string)_target.ref_GetField_Inst_Public("SourceGUID"));
                if (!string.IsNullOrEmpty(srcPath))
                {
                    UnityEngine.Object srcAsset = AssetDatabase.LoadAssetAtPath<GameObject>(srcPath);
                    if (srcAsset) _linksSource = srcAsset;
                }
            }
            return _linksSource;
        }
    }

    private void Draw_warring()
    {

        GUI.backgroundColor = Color.red;
        GUILayout.BeginVertical("box");
        GUILayout.Label(new GUIContent("无效Perfab,请重建Link源"), EditorStyles.boldLabel);
        GUILayout.EndVertical();
        GUI.backgroundColor = Color.white;
    }

    private void Draw_SourceUI()
    {
        GUILayout.Label("=== Prefab Link Source ===");
        GUILayout.Label(_target.SourceGUID);
        GUILayout.Label("==========================");
        GUI.color = Color.yellow;
        if (GUILayout.Button("Update All Linkes", GUILayout.Height(40)))
        {
            if (EditorUtility.DisplayDialog("提示", "确定更新所有连接对象?", "确定", "取消"))
            {
                PrefabLinkerEditor.UpdateAllLinked(_target);
            }
        }
    }

    private void Draw_InstanceUI(bool showMore = true)
    {
        GUILayout.BeginHorizontal();
        GUI.color = Color.yellow;
        if (GUILayout.Button("Select Source", GUILayout.Height(40)))
        {
            UnityEngine.Object asset = LoadAssetByGUID(_target.SourceGUID);
            if (asset)
            {
                Selection.activeObject = asset;
            }
        }
        if (showMore)
        {
            GUI.color = Color.red;
            if (GUILayout.Button("SaveToSource", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog("提示", "保存变更到Link源?", "确定", "取消"))
                {
                    PrefabLinkerEditor.SaveLinkedToSource(_target);
                }
            }
            GUI.color = Color.green;
            if (GUILayout.Button("Update Form Source", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog("提示", "从源更新Link?", "确定", "取消"))
                {
                    PrefabLinkerEditor.UpdateFormSource(_target);
                }
            }
            GUI.color = Color.white;
        }
        GUILayout.EndHorizontal();
    }

}
