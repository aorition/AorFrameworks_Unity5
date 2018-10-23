using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Utility.Editor
{

    /// <summary>
    /// Todo 该组件功能存在bug .. 需要进一步使用验证.
    /// </summary>

    [CustomEditor(typeof(MeshGroupChecker))]
    public class MeshGroupCheckerEditor : UnityEditor.Editor
    {

        private static string[] viewTypeLabel = { "待选", "VertexColor", "Normal", "WorldNormal", "Tangent", "UV", "Lightmap" };

        private bool isInit = false;
        private MeshGroupChecker _target;

        private Dictionary<int, Material[]> _oMaterialDic = new Dictionary<int, Material[]>();
        private List<Renderer> _renderers = new List<Renderer>();

        private Material _checkMaterial;

        private void OnEnable()
        {
            _target = target as MeshGroupChecker;

            if (!_target.enabled) return;

            _checkMaterial = new Material(Shader.Find("FrameworkTools/ModelChecker"));

            //init
            Renderer[] renderers = _target.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
                {
                    int hash = renderer.GetHashCode();
                    if (!_oMaterialDic.ContainsKey(hash))
                    {
                        //新建
                        Material[] oMt = Application.isPlaying ? renderer.materials : renderer.sharedMaterials;
                        int i, len = oMt.Length;
                        Material[] nMt = new Material[len];
                        for (i = 0; i < len; i++)
                        {
                            nMt[i] = _checkMaterial;
                        }
                        if (Application.isPlaying)
                        {
                            renderer.materials = nMt;
                        }
                        else
                        {
                            renderer.sharedMaterials = nMt;
                        }

                        _renderers.Add(renderer);
                        _oMaterialDic.Add(hash, oMt);
                    }
                }
            }

            isInit = true;

        }

        private void OnDisable()
        {

            if (!isInit) return;

            foreach (Renderer renderer in _renderers)
            {

                int hash = renderer.GetHashCode();
                if (_oMaterialDic.ContainsKey(hash))
                {
                    //新建
                    if (Application.isPlaying)
                    {
                        renderer.materials = _oMaterialDic[hash];
                    }
                    else
                    {
                        renderer.sharedMaterials = _oMaterialDic[hash];
                    }
                }
            }

            _renderers.Clear();
            _oMaterialDic.Clear();
            isInit = false;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);

            GUILayout.Label("MeshGroup检查工具(编辑器工具)");
            GUILayout.Space(2);
            GUILayout.Label("检查此节点以下所有子节点的Mesh情况 ...");

            GUILayout.Space(5);

            if (isInit)
            {
                _target.checkType = EditorGUILayout.Popup("Check Type", _target.checkType, viewTypeLabel);
                _checkMaterial.SetFloat("_checkType", _target.checkType);
            }

            GUILayout.EndVertical();

        }
    }

}


