using System;
using System.Collections.Generic;
using AorBaseUtility.Extends;
using Framework.Extends;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    public class ProcessMaterialActionController : NodeController
    {

        private string _customScriptGUIDCache;

        private Type _customComponentType;

        public bool PredefinedAction(int actionId, Material  mat,NodeData nd, ref List<string> ResultInfoList)
        {
          
            switch (actionId)
            {
                case 1:
                    if ( (bool)nd.ref_GetField_Inst_Public("_ModifyZTest"))
                    {
                        mat.SetFloat("_ZTest",  (int)Enum.Parse(typeof(UnityEngine.Rendering.CompareFunction), (string)nd.ref_GetField_Inst_Public("_ZTest")));
                       
                    }
                  
                    if ((bool)nd.ref_GetField_Inst_Public("_ModifyZWrite"))
                    {
                        mat.SetFloat("_ZWrite", (bool )nd.ref_GetField_Inst_Public("_ZWrite")?1:0);

                    }

                  

                    if ((bool)nd.ref_GetField_Inst_Public("_ModifyCull"))
                    {
                        mat.SetFloat("_Cull", (int)Enum.Parse(typeof(UnityEngine.Rendering.CullMode ), (string)nd.ref_GetField_Inst_Public("_Cull")));

                    }
                  
                    if ((bool)nd.ref_GetField_Inst_Public("_ModifySrcBlend"))
                    {
                        mat.SetFloat("_SrcBlend", (int)Enum.Parse(typeof(UnityEngine.Rendering.BlendMode ), (string)nd.ref_GetField_Inst_Public("_SrcBlend")));

                    }
                  
                    if ((bool)nd.ref_GetField_Inst_Public("_ModifyDstBlend"))
                    {
                        mat.SetFloat("_DstBlend", (int)Enum.Parse(typeof(UnityEngine.Rendering.BlendMode), (string)nd.ref_GetField_Inst_Public("_DstBlend")));

                    }
             
                    if ((bool)nd.ref_GetField_Inst_Public("_ModifySrcAlphaBlend"))
                    {
                        mat.SetFloat("_SrcAlphaBlend", (int)Enum.Parse(typeof(UnityEngine.Rendering.BlendMode), (string)nd.ref_GetField_Inst_Public("_SrcAlphaBlend")));

                    }
                   
                    if ((bool)nd.ref_GetField_Inst_Public("_ModifyDstAlphaBlend"))
                    {
                        mat.SetFloat("_DstAlphaBlend", (int)Enum.Parse(typeof(UnityEngine.Rendering.BlendMode), (string)nd.ref_GetField_Inst_Public("_DstAlphaBlend")));

                    }
                    EditorUtility.SetDirty(mat);
                    return false;

                /*_findMissingComponents(prefab, prefab.transform, ref ResultInfoList);*/
                //case 2:
                //    if (!prefab.activeSelf)
                //    {
                //        string p = AssetDatabase.GetAssetPath(prefab);
                //        ResultInfoList.Add("Find The Prefab " + p + " ActiveFalse.");
                //        return true;
                //    }
                //    return false;
                //case 3:

                //    bool action3UseCustomScript = (bool) nodeGUI.data.ref_GetField_Inst_Public("Action3UseCustomScript");

                //    if (action3UseCustomScript)
                //    {

                //        string CustomScriptGUID = (string)nodeGUI.data.ref_GetField_Inst_Public("Action3CustomScriptGUID");
                //        if (CustomScriptGUID != _customScriptGUIDCache || _customComponentType == null)
                //        {
                //            _customScriptGUIDCache = CustomScriptGUID;
                //            //加载自定义
                //            _customComponentType = NodeGraphUtility.GetScriptByGUID(CustomScriptGUID);
                //        }
                //        if (_customComponentType != null)
                //        {
                //            return _findComponent(prefab, prefab.transform, _customComponentType, ref ResultInfoList);
                //        }
                //        return false;
                //    }
                //    else
                //    {
                //        int action3ComponentID = (int)nodeGUI.data.ref_GetField_Inst_Public("Action3ComponentID");
                //        Type componenType = _getStandComponentType(action3ComponentID);
                //        if (componenType != null)
                //        {
                //            return _findComponent(prefab, prefab.transform, componenType, ref ResultInfoList);
                //        }
                //        return false;
                //    }


                case 2:
                   
                    if (mat.shader .name =="Hidden/InternalErrorShader"|| mat.shader == null)
                    {
                        ResultInfoList.Add(AssetDatabase.GetAssetPath(mat));
                        //Debug.Log(AssetDatabase.GetAssetPath(mat));
                    }



                    return false;
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
                        ResultInfoList.Add("Find Component in " + p + "[" + t.GetHierarchyPath() + "]");
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
                        ResultInfoList.Add("Find Missing in " + p + "[" + t.GetHierarchyPath() + "]");
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
