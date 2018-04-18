using System;
using System.Collections.Generic;
using AorBaseUtility;
using Framework.NodeGraph.Utility;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    public class ProcessPredefinedActionController : NodeController
    {

        private string _customScriptGUIDCache;

        private Type _customComponentType;

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
                case 3:

                    bool action3UseCustomScript = (bool) nodeGUI.data.ref_GetField_Inst_Public("Action3UseCustomScript");

                    if (action3UseCustomScript)
                    {

                        string CustomScriptGUID = (string)nodeGUI.data.ref_GetField_Inst_Public("Action3CustomScriptGUID");
                        if (CustomScriptGUID != _customScriptGUIDCache || _customComponentType == null)
                        {
                            _customScriptGUIDCache = CustomScriptGUID;
                            //加载自定义
                            _customComponentType = NodeGraphUtility.GetScriptByGUID(CustomScriptGUID);
                        }
                        if (_customComponentType != null)
                        {
                            return _findComponent(prefab, prefab.transform, _customComponentType, ref ResultInfoList);
                        }
                        return false;
                    }
                    else
                    {
                        int action3ComponentID = (int)nodeGUI.data.ref_GetField_Inst_Public("Action3ComponentID");
                        Type componenType = _getStandComponentType(action3ComponentID);
                        if (componenType != null)
                        {
                            return _findComponent(prefab, prefab.transform, componenType, ref ResultInfoList);
                        }
                        return false;
                    }
                default:
                    return true;
            }
        }

        private Type _getStandComponentType(int ComponentId)
        {
            string ComponentStr = ProcessPredefinedActionGUIController.Action3ComponentIDs[ComponentId];

            switch (ComponentStr)
            {
                case "Renderer":
                    return typeof (Renderer);
                case "MeshRenderer":
                    return typeof (MeshRenderer);
                case "SkinnedMeshRenderer":
                    return typeof (SkinnedMeshRenderer);
                case "MeshFilter":
                    return typeof (MeshFilter);
                case "Collider":
                    return typeof(Collider);
                case "Collider2D":
                    return typeof(Collider2D);
                case "ParticleSystem":
                    return typeof(ParticleSystem);
                case "Light":
                    return typeof(Light);
                case "Camera":
                    return typeof(Camera);
                case "AudioSource":
                    return typeof(AudioSource);
                default:
                    return null;
            }

        }

        private bool _findComponent(GameObject prefab, Transform t, Type type, ref List<string> ResultInfoList)
        {
            bool r = false;
            Component[] components = t.GetComponents(type);
            if (components != null && components.Length > 0)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i])
                    {
                        string p = AssetDatabase.GetAssetPath(prefab);
                        ResultInfoList.Add("Find Component in " + p + "[" + t.getHierarchyPath() + "]");
                        if (!r) r = true;
                    }
                }
            }

            if (t.childCount > 0)
            {
                for (int i = 0; i < t.childCount; i++)
                {
                    Transform sub = t.GetChild(i);
                    bool nr = _findComponent(prefab, sub, type, ref ResultInfoList);
                    if (!r && nr) r = true;
                }
            }

            return r;
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
