using System;
using System.Collections.Generic;
using AorFramework.AssetsNodeGraph;
using AorFramework.NodeGraph;
using UnityEditor;
using UnityEngine;

public class PrefabProcessDemo : UnityEngine.Object, IGameObjectProcess
{

    public string TestKey;

    [CustomScriptDescribe("通过所有包含Renderer的预制体")]
    public bool PrefabProcess(GameObject prefab, ref List<string> ResultInfoList)
    {
        List<Renderer> renderers = prefab.FindComponentListInChildren<Renderer>();
        if (renderers != null && renderers.Count > 0)
        {
            int i, len = renderers.Count;
            for (i = 0; i < len; i++)
            {
                string assetPath = AssetDatabase.GetAssetPath(renderers[i].gameObject);
                string info = assetPath + "/" + renderers[i].gameObject.getHierarchyPath();
                ResultInfoList.Add(info);
            }
            return true;
        }
        return false;
    }

    public void Reset()
    {
        //
    }

    public string ResultInfoDescribe()
    {
        return "Renderer节点列表";
    }
}
