using System;
using System.Collections.Generic;
using Framework.Editor.tools;
using Framework.Tools;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Editor
{
    
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MeshFilter), true)]
    public class MeshFilterPlus : UnityEditor.Editor
    {

        private static GUIStyle _title0Style;
        private static GUIStyle Title0Style
        {
            get
            {
                if (_title0Style == null)
                {
                    _title0Style = new GUIStyle();
                    _title0Style.name = "Title0Style";
                    _title0Style.fontSize = 16;
                    _title0Style.normal.textColor = Color.white;
                }
                return _title0Style;
            }
        }

        private static GUIStyle _title1Style;
        private static GUIStyle Title1Style
        {
            get
            {
                if (_title1Style == null)
                {
                    _title1Style = GUI.skin.GetStyle("Label").Clone();
                    _title0Style.name = "Title1Style";
                    _title1Style.fontSize = 14;
                }
                return _title1Style;
            }
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (targets != null && targets.Length == 1)
            {
                //Mesh检查工具
                __Draw_meshCheckUI();

                //Mesh导出工具
                __Draw_meshExportUI();
            }


        }

        private static string[] _viewTypeLabel = { "linkTextrue", "VertexColor", "Normal", "WorldNormal", "Tangent", "UV", "Lightmap" };

        //---------------- Mesh Check

        #region Mesh Check

        private bool _checkActive;

        private void __Draw_meshCheckUI()
        {
            GUILayout.Space(10);

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);

            MeshChecker checker = (target as MeshFilter).gameObject.GetComponent<MeshChecker>();
            _checkActive = checker;

            _checkActive = EditorGUILayout.ToggleLeft(new GUIContent("Mesh检查工具"), _checkActive, Title0Style);
            GUILayout.Space(5);

            if (!_checkActive)
            {

                if (checker)
                {

                    checker.enabled = false;
                    EditorPlusMethods.NextEditorApplicationUpdateDo(() =>
                    {
                        try
                        {
                            if (Application.isPlaying)
                            {
                                Destroy(checker);
                            }
                            else
                            {
                                DestroyImmediate(checker);
                            }
                        }
                        catch (Exception ex)
                        {
                            //do nothing
                        }
                    });
                }

                GUILayout.EndVertical();
                return;
            }


            if (!checker)
            {

                //检查依赖shader是否存在
                ShaderDefine_ModelChecker ShaderDefine = new ShaderDefine_ModelChecker();
                Shader shader = Shader.Find(ShaderDefine.ShaderLabel);
                if (!shader) FrameworkBaseShaderCreater.BuildingShaderFile(ShaderDefine, false);

                //checker init
                checker = (target as MeshFilter).gameObject.AddComponent<MeshChecker>();
                checker.hideFlags = HideFlags.HideInInspector | HideFlags.DontSave | HideFlags.DontSaveInEditor;
            }

            if (!checker)
            {
                GUILayout.EndVertical();
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("检查类型:");
            checker.ViewType = EditorGUILayout.Popup(checker.ViewType, _viewTypeLabel);
            GUILayout.EndHorizontal();

            if (checker.ViewType == 0)
            {
                checker.mainTexture =
                    (Texture2D)
                        EditorGUILayout.ObjectField(new GUIContent("并联贴图"), checker.mainTexture, typeof (Texture2D),
                            true);
                checker.SetViewType(checker.ViewType, checker.mainTexture);
            }
            else
            {
//                checker.mainTexture = null;
                checker.SetViewType(checker.ViewType);
            }

            GUILayout.EndVertical();
        }

        #endregion

        //---------------- Mesh Export

        #region Mesh Export

        private bool Active = false;

        private bool JOBExport = false;
        private bool RenameMesh = false;
        private string ReMeshName;

        private bool RecalculateNormals = false;
        private bool RecalculateTangents = false;

        private bool UseFBXPath = true;
        private bool CreatePrefab = true;

        private void __Draw_meshExportUI()
        {
            GameObject prefab = (target as MeshFilter).gameObject;

            PrefabType pt = PrefabUtility.GetPrefabType(prefab);
            bool isFBX = (pt == PrefabType.ModelPrefabInstance);
            bool isPrefab = (pt == PrefabType.PrefabInstance);

            GUILayout.Space(10);

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);
            //            GUILayout.Label("Mesh导出工具",Title0Style);
            Active = EditorGUILayout.ToggleLeft(new GUIContent("Mesh导出工具"), Active, Title0Style);
            GUILayout.Space(5);

            if (!Active)
            {
                GUILayout.EndVertical();
                return;
            }

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);
            GUILayout.Label("Mesh导出选项", Title1Style);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();

            JOBExport = EditorGUILayout.Toggle(new GUIContent("Export OBJ File (beta)", "导出Mesh为JOB文件(beta)"), JOBExport);

            RenameMesh = EditorGUILayout.Toggle(new GUIContent("Rename Mesh", "重命名Mesh"), RenameMesh);

            GUILayout.EndHorizontal();

            if (RenameMesh)
            {
                GUILayout.BeginHorizontal();
                ReMeshName = EditorGUILayout.TextField(new GUIContent("Mesh Name"), ReMeshName);
                GUILayout.EndHorizontal();
            }
            else
            {
                ReMeshName = "";
            }

            GUILayout.BeginHorizontal();
            RecalculateNormals = EditorGUILayout.Toggle(new GUIContent("Recalculate Normals", "重新计算Normal数据"),
                RecalculateNormals);
            RecalculateTangents = EditorGUILayout.Toggle(new GUIContent("Recalculate Tangents", "重新计算Tangent数据"),
                RecalculateTangents);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (isPrefab || isFBX)
            {
                UseFBXPath = EditorGUILayout.Toggle(new GUIContent("Use FBX File Path", "使用FBX文件路径导出"), UseFBXPath);
            }
            else
            {
                UseFBXPath = false;
            }

            if (!JOBExport)
            {
                CreatePrefab = EditorGUILayout.Toggle(new GUIContent("Create Prefab ?", "是否创建预制体"), CreatePrefab);
            }
            else
            {
                CreatePrefab = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();

            string buttonLabel = JOBExport
                ? "导出Mesh为OBJ文件 (beta)"
                : CreatePrefab ? "重置Mesh坐标,导出Mesh为Asset并保存预制体" : "重置Mesh坐标,导出Mesh为Asset";
            string buttonTip = JOBExport
                ? "Mesh为OBJ文件 (beta)"
                : CreatePrefab ? "重置Mesh坐标为当前世界坐标,导出Mesh为Asset并保存预制体" : "重置Mesh坐标为当前世界坐标,导出Mesh为Asset";

            if (GUILayout.Button(new GUIContent(buttonLabel, buttonTip), GUILayout.Height(32)))
            {
                string savePath;
                if (UseFBXPath)
                {
                    Object asset = PrefabUtility.GetPrefabParent(prefab);
                    string assetPath = AssetDatabase.GetAssetPath(asset);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        savePath = Application.dataPath.Replace("Assets", "") +
                                   assetPath.Substring(0, assetPath.LastIndexOf('/'));
                    }
                    else
                    {
                        savePath = EditorUtility.SaveFolderPanel("导出路径", Application.dataPath, "");
                    }
                }
                else
                {
                    savePath = EditorUtility.SaveFolderPanel("导出路径", Application.dataPath, "");
                }

                if (JOBExport)
                {
                    MeshExportUtil.ExprotMeshWithWorldTranToOBJ(target as MeshFilter, savePath, ReMeshName,
                        RecalculateNormals, RecalculateTangents);
                }
                else
                {
                    MeshExportUtil.ExportMeshWithWorldTran(target as MeshFilter, savePath, CreatePrefab, ReMeshName,
                        RecalculateNormals, RecalculateTangents);
                }

            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.EndVertical();


        }

        #endregion


    }
}
