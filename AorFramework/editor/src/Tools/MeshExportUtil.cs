using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AorBaseUtility;
using UnityEngine;
using UnityEditor;

namespace Framework.editor
{

    public class MeshExportUtil
    {
        private static string _checkFliePath(string pathDir, string name, string suffix, int p = 0)
        {
            string o = pathDir + "/" + name + (p > 0 ? "_" + p : "") + suffix;
            if (File.Exists(o))
            {
                p++;
                return _checkFliePath(pathDir, name, suffix, p);
            }
            return o;
        }

        private static Mesh _copyMeshFromMeshFilter(MeshFilter src, bool RecalculateNormals, bool RecalculateTangents)
        {

            if (src.sharedMesh == null) return null;

            Mesh sMesh = src.sharedMesh;

            Mesh nMesh = new Mesh();

            nMesh.name = sMesh.name;

            //vertices
            List<Vector3> vertices = new List<Vector3>();
            sMesh.GetVertices(vertices);
            nMesh.SetVertices(vertices);

            //triangles
            for (int i = 0; i < sMesh.subMeshCount; i++)
            {
                List<int> triangles = new List<int>();
                sMesh.GetTriangles(triangles, i);
                nMesh.SetTriangles(triangles, i, true);
            }

            //normals
            if (RecalculateNormals)
            {
                nMesh.RecalculateNormals();
            }
            else
            {
                List<Vector3> normals = new List<Vector3>();
                sMesh.GetNormals(normals);
                nMesh.SetNormals(normals);
            }

            //Tangents
            if (RecalculateTangents)
            {
                nMesh.RecalculateTangents();
            }
            else
            {
                List<Vector4> tangents = new List<Vector4>();
                sMesh.GetTangents(tangents);
                nMesh.SetTangents(tangents);
            }

            //uv
            List<Vector2> uv = new List<Vector2>();
            sMesh.GetUVs(0, uv);
            nMesh.SetUVs(0, uv);

            //uv2
            if (sMesh.uv2 != null && sMesh.uv2.Length > 0)
            {
                List<Vector2> uv2 = new List<Vector2>();
                sMesh.GetUVs(1, uv2);
                nMesh.SetUVs(1, uv2);
            }

            //uv3
            if (sMesh.uv3 != null && sMesh.uv3.Length > 0)
            {
                List<Vector2> uv3 = new List<Vector2>();
                sMesh.GetUVs(2, uv3);
                nMesh.SetUVs(2, uv3);
            }

            //uv4
            if (sMesh.uv4 != null && sMesh.uv4.Length > 0)
            {
                List<Vector2> uv4 = new List<Vector2>();
                sMesh.GetUVs(3, uv4);
                nMesh.SetUVs(3, uv4);
            }

            //colors
            if (sMesh.colors != null && sMesh.colors.Length > 0)
            {
                List<Color> colors = new List<Color>();
                sMesh.GetColors(colors);
                nMesh.SetColors(colors);
            }

            nMesh.RecalculateBounds();

            return nMesh;
        }

        private static Mesh _copyMeshFromMeshFilter(MeshFilter src, Matrix4x4 trasfMatrix, bool RecalculateNormals, bool RecalculateTangents)
        {

            if (src.sharedMesh == null) return null;

            Mesh sMesh = src.sharedMesh;

            Mesh nMesh = new Mesh();

            nMesh.name = sMesh.name;

            //vertices
            List<Vector3> vertices = new List<Vector3>();
            sMesh.GetVertices(vertices);
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = trasfMatrix * vertices[i];
                vertices[i] += src.transform.position;
            }
            nMesh.SetVertices(vertices);

            //triangles
            for (int i = 0; i < sMesh.subMeshCount; i++)
            {
                List<int> triangles = new List<int>();
                sMesh.GetTriangles(triangles, i);
                nMesh.SetTriangles(triangles, i, true);
            }

            //normals
            if (RecalculateNormals)
            {
                nMesh.RecalculateNormals();
            }
            else
            {
                List<Vector3> normals = new List<Vector3>();
                sMesh.GetNormals(normals);
                nMesh.SetNormals(normals);
            }

            //Tangents
            if (RecalculateTangents)
            {
                nMesh.RecalculateTangents();
            }
            else
            {
                List<Vector4> tangents = new List<Vector4>();
                sMesh.GetTangents(tangents);
                nMesh.SetTangents(tangents);
            }

            //uv
            List<Vector2> uv = new List<Vector2>();
            sMesh.GetUVs(0, uv);
            nMesh.SetUVs(0, uv);

            //uv2
            if (sMesh.uv2 != null && sMesh.uv2.Length > 0)
            {
                List<Vector2> uv2 = new List<Vector2>();
                sMesh.GetUVs(1, uv2);
                nMesh.SetUVs(1, uv2);
            }

            //uv3
            if (sMesh.uv3 != null && sMesh.uv3.Length > 0)
            {
                List<Vector2> uv3 = new List<Vector2>();
                sMesh.GetUVs(2, uv3);
                nMesh.SetUVs(2, uv3);
            }

            //uv4
            if (sMesh.uv4 != null && sMesh.uv4.Length > 0)
            {
                List<Vector2> uv4 = new List<Vector2>();
                sMesh.GetUVs(3, uv4);
                nMesh.SetUVs(3, uv4);
            }

            //colors
            if (sMesh.colors != null && sMesh.colors.Length > 0)
            {
                List<Color> colors = new List<Color>();
                sMesh.GetColors(colors);
                nMesh.SetColors(colors);
            }

            nMesh.RecalculateBounds();

            return nMesh;
        }

        private static string _meshToObjFileStr(MeshFilter mf, Vector3 scale)
        {
            Mesh mesh = mf.sharedMesh;
//            Material[] sharedMaterials = mf.GetComponent<Renderer>().sharedMaterials;

            Material material = Application.isPlaying
                ? mf.GetComponent<Renderer>().material
                : mf.GetComponent<Renderer>().sharedMaterial;

            Vector2 textureOffset = material.GetTextureOffset("_MainTex");
            Vector2 textureScale = material.GetTextureScale("_MainTex");

            StringBuilder stringBuilder = new StringBuilder().Append("mtllib design.mtl")
                .Append("\n")
                .Append("g ")
                .Append(mf.name)
                .Append("\n");
            
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vector = vertices[i];
                stringBuilder.Append(string.Format("v {0} {1} {2}\n", vector.x * scale.x, vector.y * scale.y, vector.z * scale.z));
            }

            stringBuilder.Append("\n");

            Dictionary<int, int> dictionary = new Dictionary<int, int>();

            if (mesh.subMeshCount > 1)
            {
                int[] triangles = mesh.GetTriangles(1);

                for (int j = 0; j < triangles.Length; j += 3)
                {
                    if (!dictionary.ContainsKey(triangles[j]))
                    {
                        dictionary.Add(triangles[j], 1);
                    }

                    if (!dictionary.ContainsKey(triangles[j + 1]))
                    {
                        dictionary.Add(triangles[j + 1], 1);
                    }

                    if (!dictionary.ContainsKey(triangles[j + 2]))
                    {
                        dictionary.Add(triangles[j + 2], 1);
                    }
                }
            }

            for (int num = 0; num != mesh.uv.Length; num++)
            {
                Vector2 vector2 = Vector2.Scale(mesh.uv[num], textureScale) + textureOffset;

                if (dictionary.ContainsKey(num))
                {
                    stringBuilder.Append(string.Format("vt {0} {1}\n", mesh.uv[num].x, mesh.uv[num].y));
                }
                else
                {
                    stringBuilder.Append(string.Format("vt {0} {1}\n", vector2.x, vector2.y));
                }
            }

            for (int k = 0; k < mesh.subMeshCount; k++)
            {
                stringBuilder.Append("\n");

                if (k == 0)
                {
                    stringBuilder.Append("usemtl ").Append("Material_design").Append("\n");
                }

                if (k == 1)
                {
                    stringBuilder.Append("usemtl ").Append("Material_logo").Append("\n");
                }

                int[] triangles2 = mesh.GetTriangles(k);

                for (int l = 0; l < triangles2.Length; l += 3)
                {
                    stringBuilder.Append(string.Format("f {0}/{0} {1}/{1} {2}/{2}\n", triangles2[l] + 1, triangles2[l + 2] + 1, triangles2[l + 1] + 1));
                }
            }

            return stringBuilder.ToString();
        }

        public static void ExportMeshWithWorldTran(MeshFilter mf, string savePath, bool createPrfab = true, string rename = null, bool RecalculateNormals = false, bool RecalculateTangents = false)
        {

            //Selection.activeGameObject = null;

            Mesh newMesh = _copyMeshFromMeshFilter(mf, mf.transform.localToWorldMatrix, RecalculateNormals, RecalculateTangents);

            if (newMesh == null) return;

            if (!string.IsNullOrEmpty(rename)) newMesh.name = rename;

            //            string savePath = EditorUtility.SaveFolderPanel("保存预制体路径", Application.dataPath, "");
            if (string.IsNullOrEmpty(savePath)) return;

            if (!Directory.Exists(savePath + "/" + mf.gameObject.name))
            {
                string guid = AssetDatabase.CreateFolder(savePath.Replace(Application.dataPath, "Assets"),
                    mf.gameObject.name);
                savePath = AssetDatabase.GUIDToAssetPath(guid);
            }
            else
            {
                savePath = savePath + (createPrfab ? "/" + mf.gameObject.name : "");
            }

            string msavePath = savePath + (createPrfab ? "/mesh" : "");
            if (!Directory.Exists(msavePath))
            {
                string guid = createPrfab ? AssetDatabase.CreateFolder(savePath.Replace(Application.dataPath, "Assets"), "mesh")
                    : AssetDatabase.CreateFolder(savePath.Replace(Application.dataPath, "Assets"), mf.gameObject.name);
                msavePath = AssetDatabase.GUIDToAssetPath(guid);
            }

            string p = _checkFliePath(msavePath, newMesh.name, ".asset");
            p = p.Replace(Application.dataPath, "Assets");

            string pp = string.Empty;
            if (createPrfab)
            {
                pp = _checkFliePath(savePath, mf.gameObject.name, ".prefab");
                pp = pp.Replace(Application.dataPath, "Assets");
            }

            try
            {
                AssetDatabase.CreateAsset(newMesh, p);
                AssetDatabase.SaveAssets();

                AssetDatabase.Refresh();

                mf.sharedMesh = newMesh;
                EditorUtility.SetDirty(mf.gameObject);
                AssetDatabase.Refresh();

                mf.transform.localScale = Vector3.one;
                mf.transform.localEulerAngles = Vector3.zero;
                mf.transform.localPosition = Vector3.zero;

                EditorUtility.SetDirty(mf.gameObject);
                AssetDatabase.Refresh();

                if (createPrfab)
                {
                    GameObject asset = PrefabUtility.CreatePrefab(pp, mf.gameObject);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    EditorPlusMethods.NextEditorApplicationUpdateDo(() =>
                    {
                        mf.gameObject.Dispose();
                        Selection.activeGameObject = asset;
                    });
                }
            }
            catch (Exception)
            {
                //
            }

            AssetDatabase.Refresh();
        }

        public static void ExprotMeshWithWorldTranToOBJ(MeshFilter mf, string savePath, string rename = null, bool RecalculateNormals = false, bool RecalculateTangents = false)
        {

            Mesh newMesh = _copyMeshFromMeshFilter(mf, RecalculateNormals, RecalculateTangents);
            if (newMesh == null) return;

            if (!string.IsNullOrEmpty(rename)) newMesh.name = rename;

            if (string.IsNullOrEmpty(savePath)) return;

            string fileStr = _meshToObjFileStr(mf, Vector3.one);
            if (string.IsNullOrEmpty(fileStr)) return;

            string p = _checkFliePath(savePath, newMesh.name, ".obj");

            AorIO.SaveStringToFile(p, fileStr);
            AssetDatabase.Refresh();
        }

    }

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

        private bool Active = false;

        private bool JOBExport = false;
        private bool RenameMesh = false;
        private string ReMeshName;
        private bool UseFBXPath = true;
        private bool CreatePrefab = true;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            GameObject prefab = (target as MeshFilter).gameObject;
            PrefabType pt = PrefabUtility.GetPrefabType(prefab);

            if (pt == PrefabType.ModelPrefabInstance)
            {

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
                UseFBXPath = EditorGUILayout.Toggle(new GUIContent("Use FBX File Path", "使用FBX文件路径导出"), UseFBXPath);
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

                string buttonLabel = JOBExport ? "导出Mesh为OBJ文件 (beta)" : CreatePrefab ? "重置Mesh坐标,导出Mesh为Asset并保存预制体" : "重置Mesh坐标,导出Mesh为Asset";
                string buttonTip = JOBExport ? "Mesh为OBJ文件 (beta)" : CreatePrefab ? "重置Mesh坐标为当前世界坐标,导出Mesh为Asset并保存预制体" : "重置Mesh坐标为当前世界坐标,导出Mesh为Asset";

                if (GUILayout.Button(new GUIContent(buttonLabel, buttonTip), GUILayout.Height(32)))
                {
                    string savePath;
                    if (UseFBXPath)
                    {
                        UnityEngine.Object asset = PrefabUtility.GetPrefabParent(prefab);
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
                        MeshExportUtil.ExprotMeshWithWorldTranToOBJ(target as MeshFilter, savePath, ReMeshName);
                    }
                    else
                    {
                        MeshExportUtil.ExportMeshWithWorldTran(target as MeshFilter, savePath, CreatePrefab, ReMeshName);
                    }

                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                GUILayout.EndVertical();

            }
        }

    }

}