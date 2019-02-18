﻿#pragma warning disable
using System;
using System.Collections.Generic;
using Framework.Editor;
using Framework.Editor.Utility;
using Framework.Extends;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Utility.Editor
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

        private MeshFilter _target;

        private void Awake()
        {
            _target = target as MeshFilter;
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

        private bool _draw_normalLines = false;
        private bool _draw_tangentLines = false;

        /*
        private float _singleSideValue = 0; 

        private bool _showDubbleSide = false;

        private bool _showTriangleLine = false;
        private Color _triangleLineColor = Color.blue;

        private bool _showVertexNormal = false;
        private float _drawVertexNormalLineLength = 0.05f;
        private Color _vertexNormalColor = Color.blue;

        private bool _showTriganleNormal = false;
        private float _drawTriganleNormalLineLength = 0.05f;
        private Color _triganleNormalColor = Color.blue;


        private bool _showVertexTangentLine = false;
        private float _drawVertexTangentLineLength = 0.05f;
        private Color _tangentVertexColor = Color.yellow;

        private bool _showTriganleTangentLine = false;
        private float _drawTriganleTangentLineLength = 0.05f;
        private Color _tangentTriganleColor = Color.yellow;
        */

        private void OnSceneGUI()
        {
            if (!_target || !_checker) return;
            //normal
            if (_draw_normalLines)
            {
                Mesh mesh = Application.isPlaying ? _target.mesh : _target.sharedMesh;
                if (!mesh || mesh.triangles.Length == 0 || mesh.normals.Length != mesh.vertices.Length) return;

                Matrix4x4 wm = _target.transform.localToWorldMatrix;
                Vector3 p = _target.transform.position;
                Quaternion d = _target.transform.rotation;
                Vector3 s;
                Vector3 t;

                Vector3[] vertex = mesh.vertices;
                Vector3[] normals = mesh.normals;
                int[] triangles = mesh.triangles;
                for (int i = 0; i < triangles.Length; i+=3)
                {
                    Vector3 v0 = vertex[triangles[i]];
                    Vector3 v1 = vertex[triangles[i + 1]];
                    Vector3 v2 = vertex[triangles[i + 2]];

                    Vector3 wv0 = wm * v0; wv0 += p;
                    Vector3 wv1 = wm * v1; wv1 += p;
                    Vector3 wv2 = wm * v2; wv2 += p;

                    Vector3 n0 = normals[triangles[i]];
                    Vector3 n1 = normals[triangles[i + 1]];
                    Vector3 n2 = normals[triangles[i + 2]];

                    Vector3 wn0 = wm * n0;
                    Vector3 wn1 = wm * n1;
                    Vector3 wn2 = wm * n2;

                    Vector3 wcn = Vector3.Normalize((wn0 + wn1 + wn2) / 3);

                    if (!_checker.ShowDubbleSide && !_cullingCheck(wcn, _checker.SingleSideValue))
                    {
                        //back
                        //do nothing
                    }
                    else {

                        //front
                        if (_checker.ShowTriangleLine)
                        {
                            Handles.color = _checker.TriangleLineColor;

                            Handles.DrawLine(wv0, wv1);
                            Handles.DrawLine(wv1, wv2);
                            Handles.DrawLine(wv2, wv0);
                        }

                        if (_checker.ShowTriganleNormal)
                        {
                            s = (wv0 + wv1 + wv2) / 3;
                            t = s + wcn * _checker.DrawTriganleNormalLineLength;
                            Handles.color = _checker.TriganleNormalColor;
                            Handles.DrawLine(s, t);
                        }

                        if (_checker.ShowVertexNormal)
                        {
                            Handles.color = _checker.VertexNormalColor;

                            Vector3 t0 = wv0 + wn0 * _checker.DrawVertexNormalLineLength;
                            Handles.DrawLine(wv0, t0);

                            Vector3 t1 = wv1 + wn1 * _checker.DrawVertexNormalLineLength;
                            Handles.DrawLine(wv1, t1);

                            Vector3 t2 = wv2 + wn2 * _checker.DrawVertexNormalLineLength;
                            Handles.DrawLine(wv2, t2);
                        }

                        
                    }

                }

            }

            //tangent
            if (_draw_tangentLines)
            {

                Mesh mesh = Application.isPlaying ? _target.mesh : _target.sharedMesh;
                if (!mesh || mesh.triangles.Length == 0 || mesh.normals.Length != mesh.vertices.Length) return;

                Matrix4x4 wm = _target.transform.localToWorldMatrix;
                Vector3 p = _target.transform.position;
                Quaternion d = _target.transform.rotation;
                Vector3 s;
                Vector3 t;

                Vector3[] vertex = mesh.vertices;
                Vector3[] normals = mesh.normals;
                Vector4[] tangents = mesh.tangents;
                int[] triangles = mesh.triangles;
                for (int i = 0; i < triangles.Length; i += 3)
                {

                    Vector3 v0 = vertex[triangles[i]];
                    Vector3 v1 = vertex[triangles[i + 1]];
                    Vector3 v2 = vertex[triangles[i + 2]];

                    Vector3 wv0 = wm * v0; wv0 += p;
                    Vector3 wv1 = wm * v1; wv1 += p;
                    Vector3 wv2 = wm * v2; wv2 += p;

                    Vector3 n0 = normals[triangles[i]];
                    Vector3 n1 = normals[triangles[i + 1]];
                    Vector3 n2 = normals[triangles[i + 2]];

                    Vector3 wn0 = wm * n0;
                    Vector3 wn1 = wm * n1;
                    Vector3 wn2 = wm * n2;
                    Vector3 wcn = Vector3.Normalize((wn0 + wn1 + wn2) / 3);

                    Vector4 t0 = tangents[triangles[i]];
                    Vector4 t1 = tangents[triangles[i + 1]];
                    Vector4 t2 = tangents[triangles[i + 2]];

                    Vector3 wt0 = wm * t0;
                    Vector3 wt1 = wm * t1;
                    Vector3 wt2 = wm * t2;
                    Vector3 wct = Vector3.Normalize((wt0 + wt1 + wt2) / 3);

                    if (!_checker.ShowDubbleSide && !_cullingCheck(wcn, _checker.SingleSideValue))
                    {
                        //back
                        //do nothing
                    }
                    else
                    {

                        //front
                        if (_checker.ShowTriangleLine)
                        {
                            Handles.color = _checker.TriangleLineColor;

                            Handles.DrawLine(wv0, wv1);
                            Handles.DrawLine(wv1, wv2);
                            Handles.DrawLine(wv2, wv0);
                        }

                        if (_checker.ShowTriganleTangentLine)
                        {
                            s = (wv0 + wv1 + wv2) / 3;
                            t = s + wct * _checker.DrawTriganleTangentLineLength;
                            Handles.color = _checker.TangentTriganleColor;
                            Handles.DrawLine(s, t);
                        }

                        if (_checker.ShowVertexTangentLine)
                        {
                            Handles.color = _checker.TangentVertexColor;

                            Vector3 tt0 = wv0 + wt0 * _checker.DrawVertexTangentLineLength;
                            Handles.DrawLine(wv0, tt0);

                            Vector3 tt1 = wv1 + wt1 * _checker.DrawVertexTangentLineLength;
                            Handles.DrawLine(wv1, tt1);

                            Vector3 tt2 = wv2 + wt2 * _checker.DrawVertexTangentLineLength;
                            Handles.DrawLine(wv2, tt2);
                        }
                        
                    }

                }
            }
        }
        
        private static bool _cullingCheck(Vector3 wNormal, float sideOffset = 0) {

            float r = Vector3.Dot(SceneView.currentDrawingSceneView.camera.transform.forward, wNormal);
            return r < sideOffset;
        }

        private static string[] _viewTypeLabel = { "linkTextrue", "VertexColor", "Normal", "WorldNormal", "Tangent", "UV", "Lightmap" };

        //---------------- Mesh Check

        #region Mesh Check

        private bool _checkActive;
        private MeshChecker _checker;

        private void __Draw_meshCheckUI()
        {
            GUILayout.Space(10);

            GUILayout.BeginVertical("box");
            {

                GUILayout.Space(5);

                _checker = (target as MeshFilter).gameObject.GetComponent<MeshChecker>();
                _checkActive = _checker;

                _checkActive = EditorGUILayout.ToggleLeft(new GUIContent("Mesh检查工具"), _checkActive, Title0Style);
                GUILayout.Space(5);

                if (!_checkActive)
                {

                    if (_checker)
                    {

                        _checker.enabled = false;
                        EditorPlusMethods.NextEditorApplicationUpdateDo(() =>
                        {
                            try
                            {
                                MeshChecker del = _checker;
                                _checker = null;
                                if (Application.isPlaying)
                                {
                                    Destroy(del);
                                }
                                else
                                {
                                    DestroyImmediate(del);
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

                if (!_checker)
                {

                    //检查依赖shader是否存在
                    ShaderDefine_ModelChecker ShaderDefine = new ShaderDefine_ModelChecker();
                    Shader shader = Shader.Find(ShaderDefine.ShaderLabel);
                    if (!shader) FrameworkBaseShaderCreater.BuildingShaderFile(ShaderDefine, false);

                    //checker init
                    _checker = (target as MeshFilter).gameObject.AddComponent<MeshChecker>();
                    _checker.hideFlags = HideFlags.HideInInspector | HideFlags.DontSave | HideFlags.DontSaveInEditor;
                }

                if (!_checker)
                {
                    GUILayout.EndVertical();
                    return;
                }

                //Todo 


                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("检查类型:");
                    _checker.ViewType = EditorGUILayout.Popup(_checker.ViewType, _viewTypeLabel);
                }
                GUILayout.EndHorizontal();

                //link Textrue
                if (_checker.ViewType == 0)
                {

                    GUILayout.Space(10);

                    GUILayout.BeginVertical("box");
                    {
                        _checker.mainTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("并联贴图"), _checker.mainTexture, typeof(Texture2D), true);
                        _checker.SetViewType(_checker.ViewType, _checker.mainTexture);
                    }
                    GUILayout.EndVertical();
                }
                else
                {
                    //                checker.mainTexture = null;
                    _checker.SetViewType(_checker.ViewType);
                }

                //Normal && World Normal
                if (_checker.ViewType == 2 || _checker.ViewType == 3)
                {
                    _draw_normalLines = true;

                    GUILayout.Space(10);

                    GUILayout.BeginVertical("box");
                    {
                        GUILayout.Space(5);

                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.Space(5);
                            _checker.ShowDubbleSide = EditorGUILayout.ToggleLeft("Show Dubble Side", _checker.ShowDubbleSide);
                            if (!_checker.ShowDubbleSide)
                                _checker.SingleSideValue = EditorGUILayout.Slider("SidedOffset", _checker.SingleSideValue, -1f, 1f);
                            else
                                _checker.SingleSideValue = 0;
                            GUILayout.Space(5);
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(5);

                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.Space(5);
                            _checker.ShowVertexNormal = EditorGUILayout.ToggleLeft("Show Vertex Normal", _checker.ShowVertexNormal);
                            GUILayout.Space(5);
                            _checker.DrawVertexNormalLineLength = EditorGUILayout.FloatField("Length", _checker.DrawVertexNormalLineLength);
                            _checker.VertexNormalColor = EditorGUILayout.ColorField("Color", _checker.VertexNormalColor);
                            GUILayout.Space(5);
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(5);

                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.Space(5);
                            _checker.ShowTriganleNormal = EditorGUILayout.ToggleLeft("Show Triganle Normal", _checker.ShowTriganleNormal);
                            GUILayout.Space(5);
                            _checker.DrawTriganleNormalLineLength = EditorGUILayout.FloatField("Length", _checker.DrawTriganleNormalLineLength);
                            _checker.TriganleNormalColor = EditorGUILayout.ColorField("Color", _checker.TriganleNormalColor);
                            GUILayout.Space(5);
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(5);

                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.Space(5);
                            _checker.ShowTriangleLine = EditorGUILayout.ToggleLeft("Show Wireframe", _checker.ShowTriangleLine);
                            GUILayout.Space(5);
                            _checker.TriangleLineColor = EditorGUILayout.ColorField("Color", _checker.TriangleLineColor);
                            GUILayout.Space(5);
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(5);
                    }
                    GUILayout.EndVertical();
                }
                else if (_checker.ViewType == 4) //Tangent
                {
                    _draw_tangentLines = true;

                    GUILayout.Space(10);

                    GUILayout.BeginVertical("box");
                    {

                        GUILayout.Space(5);

                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.Space(5);
                            _checker.ShowDubbleSide = EditorGUILayout.ToggleLeft("Show Dubble Side", _checker.ShowDubbleSide);
                            if (!_checker.ShowDubbleSide)
                                _checker.SingleSideValue = EditorGUILayout.Slider("SidedOffset", _checker.SingleSideValue, -1f, 1f);
                            else
                                _checker.SingleSideValue = 0;
                            GUILayout.Space(5);
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(5);
                        
                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.Space(5);
                            _checker.ShowVertexTangentLine = EditorGUILayout.ToggleLeft("Show Vertex Tangent", _checker.ShowVertexTangentLine);
                            GUILayout.Space(5);
                            _checker.DrawVertexTangentLineLength = EditorGUILayout.FloatField("Length", _checker.DrawVertexTangentLineLength);
                            _checker.TangentVertexColor = EditorGUILayout.ColorField("Color", _checker.TangentVertexColor);
                            GUILayout.Space(5);
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(5);

                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.Space(5);
                            _checker.ShowTriganleTangentLine = EditorGUILayout.ToggleLeft("Show Triganle Tangent", _checker.ShowTriganleTangentLine);
                            GUILayout.Space(5);
                            _checker.DrawTriganleTangentLineLength = EditorGUILayout.FloatField("Length", _checker.DrawTriganleTangentLineLength);
                            _checker.TangentTriganleColor = EditorGUILayout.ColorField("Color", _checker.TangentTriganleColor);
                            GUILayout.Space(5);
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(5);

                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.Space(5);
                            _checker.ShowTriangleLine = EditorGUILayout.ToggleLeft("Show Wireframe", _checker.ShowTriangleLine);
                            GUILayout.Space(5);
                            _checker.TriangleLineColor = EditorGUILayout.ColorField("Color", _checker.TriangleLineColor);
                            GUILayout.Space(5);
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(5);
                    }
                    GUILayout.EndVertical();
                }
                else
                {
                    //other
                    _draw_tangentLines = false;
                    _draw_normalLines = false;
                }

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
