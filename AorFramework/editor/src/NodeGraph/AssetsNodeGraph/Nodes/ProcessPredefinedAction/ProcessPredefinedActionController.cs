using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class ProcessPredefinedActionController : NodeController
    {

        public bool PredefinedAction(int actionId, GameObject prefab, ref List<string> ResultInfoList)
        {
            switch (actionId)
            {
                case 1:
                    return _findMissingComponents(prefab, prefab.transform, ref ResultInfoList);
                case 2:
                    if (!prefab.activeSelf)
                    {
                        string p = AssetDatabase.GetAssetPath(prefab);
                        ResultInfoList.Add("Find The Prefab " + p + " ActiveFalse.");
                        return true;
                    }
                    return false;
                default:
                    return true;
            }
        }

        private bool _findMissingComponents(GameObject prefab, Transform t, ref List<string> ResultInfoList)
        {
            bool r = false;
            Component[] components = t.GetComponents(typeof (Component));
            if (components != null && components.Length > 0)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (!components[i])
                    {
                        string p = AssetDatabase.GetAssetPath(prefab);
                        ResultInfoList.Add("Find Missing in " + p + "[" + t.getHierarchyPath() + "]");
                        if (!r) r = true;
                    }
                }
            }

            if (t.childCount > 0)
            {
                for (int i = 0; i < t.childCount; i++)
                {
                    Transform sub = t.GetChild(i);
                    bool nr = _findMissingComponents(prefab, sub, ref ResultInfoList);
                    if (!r && nr) r = true;
                }
            }

            return r;
        }


    }
}
