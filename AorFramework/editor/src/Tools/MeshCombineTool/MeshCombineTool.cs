﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MeshCombineTool
{
    [MenuItem("FrameworkTools/Mesh合并工具/MeshCombineInScene")]
    public static void MeshCombineInScene()
    {
        GameObject root = Selection.activeGameObject;

        if (root == null)
        {
            EditorUtility.DisplayDialog("提示", "MeshCombine需要你指定合并对象的Root节点", "OK");
            return;
        }

        MeshCombineInScene(root.transform);
    }

    private static string _getSubPathInRoot(Transform root, Transform sub, string loopStr = "")
    {
        if (sub.parent != null && sub.parent != root)
        {
            return _getSubPathInRoot(root, sub.parent, (string.IsNullOrEmpty(loopStr) ? sub.name : sub.name + "/" + loopStr));
        }
        else
        {
            return sub.name + "/" + loopStr;
        }
    }

    public static GameObject MeshCombineInScene(Transform srcRoot)
    {

        List<string> unActiveList = new List<string>();
        List<string> unMeshRenderEnableList = new List<string>();
        List<string> pldList = new List<string>();

        MeshConbinePreDoing(srcRoot, srcRoot, unActiveList, unMeshRenderEnableList, pldList);

        GameObject combined = GameObject.Instantiate(srcRoot.gameObject);
        combined.name = srcRoot.name + "_combined";
        combined.transform.SetParent(srcRoot.parent, false);

        removeTransWidthEditorOnlyTag(combined.transform);

        PrefabLightmapData[] prefabLightmapDatas = combined.FindComponentsInChildren<PrefabLightmapData>();
        if (prefabLightmapDatas.Length > 0)
        {
            int i, len = prefabLightmapDatas.Length;
            for (i = 0; i < len; i++)
            {
                PrefabLightmapData prefabLightmapData = prefabLightmapDatas[i];
                if (prefabLightmapData)
                {
                    prefabLightmapData.LoadLightmap();
                }
            }
        }

        StaticBatchingUtility.Combine(combined);

        //恢复本体(Active/MeshRenderEnable设置以及移除PrefabLightmapData)
        restoreMeshCombineObjOriginal(srcRoot,unActiveList, unMeshRenderEnableList, pldList);
        //恢复复制体的Active/MeshRenderEnable设置
        restoreMeshCombineObjOriginal(combined.transform,unActiveList, unMeshRenderEnableList, null);

        srcRoot.gameObject.SetActive(false);

        unMeshRenderEnableList.Clear();
        unActiveList.Clear();
        pldList.Clear();

        return combined;
    }

    private static void removeTransWidthEditorOnlyTag(Transform tran)
    {
        if (tran.tag == "EditorOnly")
        {
            GameObject.DestroyImmediate(tran.gameObject);
        }
        else
        {
            if (tran.childCount > 0)
            {
                int i, len = tran.childCount;
                for (i = 0; i < len; i++)
                {
                    Transform sub = tran.GetChild(i);
                    removeTransWidthEditorOnlyTag(sub);
                }
            }
        }
    }

    private static void MeshConbinePreDoing(Transform root, Transform sub, List<string> unActiveList, List<string> unMeshRenderEnableList, List<string> pldList)
    {

        if (!sub.gameObject.activeInHierarchy)
        {
            sub.gameObject.SetActive(true);
            unActiveList.Add(_getSubPathInRoot(root, sub));
        }

        MeshRenderer mr = sub.GetComponent<MeshRenderer>();
        if (mr)
        {

            if (!mr.enabled)
            {
                mr.enabled = true;
                unMeshRenderEnableList.Add(_getSubPathInRoot(root, sub));
            }

            
            PrefabLightmapData PLMdata = mr.gameObject.GetComponent<PrefabLightmapData>();
            if (!PLMdata)
            {
                PLMdata = mr.gameObject.AddComponent<PrefabLightmapData>();
            }

            PLMdata.SaveLightmap();

            pldList.Add(_getSubPathInRoot(root, mr.transform));

        }

        if (sub.childCount > 0)
        {
            int i, len = sub.childCount;
            for (i = 0; i < len; i++)
            {
                Transform subT = sub.GetChild(i);
                MeshConbinePreDoing(root, subT, unActiveList, unMeshRenderEnableList, pldList);
            }
        }

    }

    private static void restoreMeshCombineObjOriginal(Transform target, List<string> unActiveList, List<string> unMeshRenderEnableList, List<string> pldList)
    {
        int i, len;
        if (unMeshRenderEnableList != null && unMeshRenderEnableList.Count > 0)
        {
            len = unMeshRenderEnableList.Count;
            for (i = 0; i < len; i++)
            {
                Transform t = target.FindChild(unMeshRenderEnableList[i]);
                if (t)
                {
                    MeshRenderer mr = t.GetComponent<MeshRenderer>();
                    if (mr)
                    {
                        mr.enabled = false;
                    }
                }
            }
        }
        if (unActiveList != null && unActiveList.Count > 0)
        {
            len = unActiveList.Count;
            for (i = 0; i < len; i++)
            {
                Transform t = target.FindChild(unActiveList[i]);
                if (t)
                {
                    t.gameObject.SetActive(false);
                }
            }
        }

        if (pldList != null && pldList.Count > 0)
        {
            len = pldList.Count;
            for (i = 0; i < len; i++)
            {
                Transform t = target.FindChild(pldList[i]);
                if (t)
                {
                    PrefabLightmapData pld = t.GetComponent<PrefabLightmapData>();
                    if (pld)
                    {
                        GameObject.DestroyImmediate(pld);
                    }
                }
            }
        }
    }

    //-------------------------------------------------------------------

    [MenuItem("FrameworkTools/Mesh合并工具/MeshCombineToAsset")]
    public static void MeshComineToAsset()
    {
        GameObject root = Selection.activeGameObject;

        if (root == null)
        {
            EditorUtility.DisplayDialog("提示", "MeshCombine需要你指定合并对象的Root节点", "OK");
            return;
        }

        string saveDir = "";
        string scenePath = SceneManager.GetActiveScene().path;
        if (SceneManager.sceneCount == 1 && !string.IsNullOrEmpty(scenePath))
        {
            saveDir = scenePath.Substring(0, scenePath.LastIndexOf('/'));
        }
        else
        {
            string seldir = EditorUtility.OpenFolderPanel("选择保存目录", "", "");
            if (string.IsNullOrEmpty(seldir))
            {
                return;
            }
            saveDir = seldir;
        }

        MeshComineToAsset(root.transform, saveDir);

    }

    public static void MeshComineToAsset(Transform tran, string saveDir)
    {

        GameObject combined = MeshCombineInScene(tran);

        List<MeshFilter> mfs = combined.FindComponentListInChildren<MeshFilter>();

        if (mfs.Count > 0)
        {
            string savePath = saveDir + "/CombineMeshs";
            if (!Directory.Exists(savePath))
            {
                string guid = AssetDatabase.CreateFolder(saveDir, "CombineMeshs");
                savePath = AssetDatabase.GUIDToAssetPath(guid);
            }

            List<string> meshNames = new List<string>();
            List<Mesh> hashMeshes = new List<Mesh>();

            int i, len = mfs.Count;
            for (i = 0; i < len; i++)
            {

                MeshFilter mf = mfs[i];
                Mesh mesh = mf.sharedMesh;
                if (mesh != null)
                {
                    string path = AssetDatabase.GetAssetPath(mesh);
                    if (string.IsNullOrEmpty(path) && !hashMeshes.Contains(mesh))
                    {
                        hashMeshes.Add(mesh);
                        meshNames.Add(mf.name);
                    }
                }
            }

            len = hashMeshes.Count;
            for (i = 0; i < len; i++)
            {
                Mesh mesh = hashMeshes[i];
                AssetDatabase.CreateAsset(mesh, savePath + "/" + meshNames[i] + "_CombineMesh_" + i + ".asset");
                AssetDatabase.SaveAssets();
            }
        }

    }


    //--------------------------------------------------------------

    [MenuItem("FrameworkTools/Mesh合并工具/MeshMergeInScene")]
    public static void MeshMerge()
    {
        GameObject root = Selection.activeGameObject;

        if (root == null)
        {
            EditorUtility.DisplayDialog("提示", "MeshCombine需要你指定合并对象的Root节点", "OK");
            return;
        }

        MeshMerge(root.transform);
    }
    
    private struct MeshKey
    {
        public MeshKey(Material m, int i)
        {
            this.material = m;
            this.lightMapId = i;
        }

        public Material material;
        public int lightMapId;
    }

    public static GameObject MeshMerge(Transform root)
    {
        
        List<MeshFilter> MeshFilterList = root.FindComponentListInChildren<MeshFilter>();

        if (MeshFilterList != null && MeshFilterList.Count > 0)
        {
            
            GameObject rootGameObject = new GameObject(root.name + "_MeshMeraged");
            rootGameObject.transform.localScale = root.localScale;
            rootGameObject.transform.localRotation = root.localRotation;
            rootGameObject.transform.localPosition = root.localPosition;
            rootGameObject.transform.SetParent(root.parent, false);

            Dictionary<MeshKey, List<string>> NameGroup = new Dictionary<MeshKey, List<string>>();
            Dictionary<MeshKey, List<MeshFilter>> MerageGroup = new Dictionary<MeshKey, List<MeshFilter>>();

            int i, len = MeshFilterList.Count;
            for (i = 0; i < len; i++)
            {
                MeshRenderer mr = MeshFilterList[i].gameObject.GetComponent<MeshRenderer>();
                if (mr)
                {
                    MeshKey mk = new MeshKey(mr.sharedMaterial, mr.lightmapIndex);

                    if (MerageGroup.ContainsKey(mk))
                    {
                        MerageGroup[mk].Add(MeshFilterList[i]);
                        NameGroup[mk].Add(MeshFilterList[i].gameObject.name);
                    }
                    else
                    {
                        List<MeshFilter> list = new List<MeshFilter>();
                        list.Add(MeshFilterList[i]);
                        MerageGroup.Add(mk, list);
                        
                        List<string> nList = new List<string>();
                        nList.Add(MeshFilterList[i].gameObject.name);
                        NameGroup.Add(mk, nList);
                    }
                }
            }

            if (MerageGroup.Count > 0)
            {
                foreach (MeshKey mk in MerageGroup.Keys)
                {
                    GameObject sub = MeshMerge(MerageGroup[mk], NameGroup[mk], mk.material);
                    if (sub)
                    {
                        sub.transform.SetParent(rootGameObject.transform);
                    }
                }
            }

            MerageGroup.Clear();
            NameGroup.Clear();
            MeshFilterList.Clear();


            if (root.gameObject.activeInHierarchy)
            {
                root.gameObject.SetActive(false);
            }

            return rootGameObject;

        }

        return null;
    }

    public static GameObject MeshMerge(List<MeshFilter> meshes, List<string> names, Material useMaterial)
    {
        if (meshes == null || meshes.Count == 0) return null;

        string name = names[0] + "_merge_" + names.Count;

        CombineInstance[] combineInstances = new CombineInstance[meshes.Count];

        bool hasLightmap = false;
        bool ckLM = false;

        int i, len = meshes.Count;
        for (i = 0; i < len; i++)
        {
            Mesh sMesh = _copyMeshFromMeshFilter(meshes[i], ref ckLM);
            combineInstances[i].mesh = sMesh;
            combineInstances[i].transform = meshes[i].transform.localToWorldMatrix;
            if (ckLM)
            {
                hasLightmap = true;
            }
        }

        Mesh newMesh = new Mesh();
        newMesh.name = name;
        newMesh.CombineMeshes(combineInstances, true);

        GameObject meshGameObject = new GameObject(name);

        MeshFilter mf = meshGameObject.AddComponent<MeshFilter>();
        mf.sharedMesh = newMesh;

        MeshRenderer srcMr = meshes[0].gameObject.GetComponent<MeshRenderer>();
        MeshRenderer mr = meshGameObject.AddComponent<MeshRenderer>();
        mr.sharedMaterial = useMaterial;

        if (hasLightmap)
        {
            mr.lightmapIndex = srcMr.lightmapIndex;
            mr.lightmapScaleOffset = new Vector4(1, 1, 0, 0);

            PrefabLightmapData prefabLightmapData = meshGameObject.AddComponent<PrefabLightmapData>();
            prefabLightmapData.SaveLightmap(mr.lightmapIndex, mr.lightmapScaleOffset);
        }

        return meshGameObject;
    }

    private static Mesh _copyMeshFromMeshFilter(MeshFilter src,ref bool hasLightmap)
    {

        MeshRenderer mr = src.gameObject.GetComponent<MeshRenderer>();

        Mesh newMesh = new Mesh();
        EditorUtility.CopySerialized(src.sharedMesh, newMesh);

        if (mr && mr.lightmapIndex != -1)
        {
            Vector2[] srcUV = (src.sharedMesh.uv2.Length > 0 ? src.sharedMesh.uv2 : src.sharedMesh.uv);
            Vector2[] lmUV = new Vector2[srcUV.Length];
            int i, len = lmUV.Length;
            for (i = 0; i < len; i++)
            {
                lmUV[i] =
                    new Vector2(srcUV[i].x*mr.lightmapScaleOffset.x,
                        srcUV[i].y*mr.lightmapScaleOffset.y) +
                    new Vector2(mr.lightmapScaleOffset.z, mr.lightmapScaleOffset.w);
            }
            newMesh.uv2 = lmUV;
            hasLightmap = true;
        }
        else
        {
            hasLightmap = false;
        }

        return newMesh;
    }

    //-------------------------------------------------------------------

    [MenuItem("FrameworkTools/Mesh合并工具/MeshMergeToAsset")]
    public static void MeshMergeToAsset()
    {
        GameObject root = Selection.activeGameObject;

        if (root == null)
        {
            EditorUtility.DisplayDialog("提示", "MeshCombine需要你指定合并对象的Root节点", "OK");
            return;
        }

        string saveDir = "";
        string scenePath = SceneManager.GetActiveScene().path;
        if (SceneManager.sceneCount == 1 && !string.IsNullOrEmpty(scenePath))
        {
            saveDir = scenePath.Substring(0, scenePath.LastIndexOf('/'));
        }
        else
        {
            string seldir = EditorUtility.OpenFolderPanel("选择保存目录", "", "");
            if (string.IsNullOrEmpty(seldir))
            {
                return;
            }
            saveDir = seldir;
        }

        MeshMergeToAsset(root.transform, saveDir);

    }

    public static void MeshMergeToAsset(Transform tran, string saveDir)
    {
        string savePath = saveDir + "/MerageMeshs";

        if (!Directory.Exists(savePath))
        {
            string guid = AssetDatabase.CreateFolder(saveDir, "MerageMeshs");
            savePath = AssetDatabase.GUIDToAssetPath(guid);
        }

        GameObject meragedGameObject = MeshMerge(tran);
        if (meragedGameObject)
        {

            List<MeshFilter> mfList = meragedGameObject.FindComponentListInChildren<MeshFilter>();

            if (mfList != null && mfList.Count > 0)
            {
                int i, len = mfList.Count;

                for (i = 0; i < len; i++)
                {
                    MeshFilter mf = mfList[i];
                    AssetDatabase.CreateAsset(mf.sharedMesh, savePath + "/" + mf.gameObject.name + "_meraged.asset");
                    AssetDatabase.SaveAssets();
                }
            }
        }

    }
}
