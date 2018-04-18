using AorBaseUtility;
using Framework;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabLinker))]
public class PrefabLinkerEditor : Editor
{ 

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
                    replaceLinked(linker.gameObject, go);

                }

            }

        }

    }

    private static void replaceLinked(GameObject o, GameObject n)
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
        Transform parent_cache = o.transform.parent;

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

        n.transform.SetParent(parent_cache, false);
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

    }

    private PrefabLinker _target;

    private void Awake()
    {
        _target = target as PrefabLinker;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        bool isSource = (bool)_target.ref_GetField_Inst_Public("IsSource");
        bool nIsSource = EditorGUILayout.Toggle(new GUIContent("IsSource"), isSource);
        if (nIsSource != isSource)
        {
            _target.ref_SetField_Inst_Public("IsSource", nIsSource);
        }

        if (nIsSource)
        {

            PrefabType pt = PrefabUtility.GetPrefabType(_target);
            switch (pt)
            {
                case PrefabType.Prefab:
                case PrefabType.ModelPrefab:
                    string p = AssetDatabase.GetAssetPath(_target);
                    string guid = AssetDatabase.AssetPathToGUID(p);

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

            if (!string.IsNullOrEmpty((string) _target.ref_GetField_Inst_Public("SourceGUID")))
            {
                Draw_InstanceUI();
            }
            
        }

    }
    

    private void Draw_SourceUI()
    {
        GUILayout.Label("=== Prefab Link Source ===");
        GUILayout.Label(_target.SourceGUID);
        GUILayout.Label("==========================");
    }

    private void Draw_InstanceUI()
    {
        GUILayout.BeginHorizontal();
        GUI.color = Color.yellow;
        if (GUILayout.Button("Select Source"))
        {
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(_target.SourceGUID));
            if (asset)
            {
                Selection.activeObject = asset;
            }
        }

        GUI.color = Color.green;
        if (GUILayout.Button("Update Form Source"))
        {
            PrefabLinkerEditor.UpdateFormSource(_target);
        }
        GUI.color = Color.white;

        GUILayout.EndHorizontal();
    }

}
