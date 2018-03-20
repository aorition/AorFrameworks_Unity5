using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AorBaseUtility;
using UnityEngine;
using UnityEditor;

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

    private static Mesh _copyMeshFromMeshFilter(MeshFilter src)
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

        //normals
        if (sMesh.normals != null && sMesh.normals.Length > 0)
        {
            List<Vector3> normals = new List<Vector3>();
            sMesh.GetNormals(normals);
            sMesh.SetNormals(normals);
        }

        //tangents
        if (sMesh.tangents != null && sMesh.tangents.Length > 0)
        {
            List<Vector4> tangents = new List<Vector4>();
            sMesh.GetTangents(tangents);
            sMesh.SetTangents(tangents);
        }

        return nMesh;
    }

    private static Mesh _copyMeshFromMeshFilter(MeshFilter src, Matrix4x4 trasfMatrix)
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
        }
        nMesh.SetVertices(vertices);

        //triangles
        for (int i = 0; i < sMesh.subMeshCount; i++)
        {
            List<int> triangles = new List<int>();
            sMesh.GetTriangles(triangles, i);
            nMesh.SetTriangles(triangles, i, true);
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
        nMesh.RecalculateNormals();
        nMesh.RecalculateTangents();

        return nMesh;
    }
    
    private static Mesh _maxMeshToUnityMesh(MeshFilter src)
    {
        if (src.sharedMesh == null) return null;

        Matrix4x4 trasfMatrix = new Matrix4x4();
        trasfMatrix.m00 = -1.00000024f;
        trasfMatrix.m01 = 8.742279e-08f;
        trasfMatrix.m02 = 0f;
        trasfMatrix.m03 = 0f;
        trasfMatrix.m10 = 0f;
        trasfMatrix.m11 = -1.1920929e-07f;
        trasfMatrix.m12 = 1.00000012f;
        trasfMatrix.m13 = 0f;
        trasfMatrix.m20 = 8.742279e-08f;
        trasfMatrix.m21 = 1.00000012f;
        trasfMatrix.m22 = -1.1920929e-07f;
        trasfMatrix.m23 = 0f;
        trasfMatrix.m30 = 0f;
        trasfMatrix.m31 = 0f;
        trasfMatrix.m32 = 0f;
        trasfMatrix.m33 = 1f;

        Mesh sMesh = src.sharedMesh;

        Mesh nMesh = new Mesh();

        nMesh.name = sMesh.name;

        int i, len;

        //vertices
        List<Vector3> vertices = new List<Vector3>();
        sMesh.GetVertices(vertices);

        len = vertices.Count;
        for (i = 0; i < len; i++)
        {
            vertices[i] = trasfMatrix * vertices[i];
        }
        nMesh.SetVertices(vertices);

        //triangles
        len = sMesh.subMeshCount;
        for (i = 0; i < len; i++)
        {
            List<int> triangles = new List<int>();
            sMesh.GetTriangles(triangles, i);
            nMesh.SetTriangles(triangles, i, true);
        }

        //uv
        List<Vector2> uv = new List<Vector2>();
        sMesh.GetUVs(0, uv);
        len = uv.Count;
        for (i = 0; i < len; i++)
        {
            uv[i] = new Vector2(uv[i].x, 1 - uv[i].y);
        }
        nMesh.SetUVs(0, uv);

        //uv2
        if (sMesh.uv2 != null && sMesh.uv2.Length > 0)
        {
            List<Vector2> uv2 = new List<Vector2>();
            sMesh.GetUVs(1, uv2);
            len = uv2.Count;
            for (i = 0; i < len; i++)
            {
                uv[i] = new Vector2(uv2[i].x, 1 - uv2[i].y);
            }
            nMesh.SetUVs(0, uv);
            nMesh.SetUVs(1, uv2);
        }

        //uv3
        if (sMesh.uv3 != null && sMesh.uv3.Length > 0)
        {
            List<Vector2> uv3 = new List<Vector2>();
            sMesh.GetUVs(2, uv3);
            len = uv3.Count;
            for (i = 0; i < len; i++)
            {
                uv[i] = new Vector2(uv3[i].x, 1 - uv3[i].y);
            }
            nMesh.SetUVs(2, uv3);
        }

        //uv4
        if (sMesh.uv4 != null && sMesh.uv4.Length > 0)
        {
            List<Vector2> uv4 = new List<Vector2>();
            sMesh.GetUVs(3, uv4);
            len = uv4.Count;
            for (i = 0; i < len; i++)
            {
                uv[i] = new Vector2(uv4[i].x, 1 - uv4[i].y);
            }
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
        nMesh.RecalculateNormals();
        nMesh.RecalculateTangents();

        return nMesh;
    }

    private static string _meshToObjFileStr(MeshFilter mf, Vector3 scale)
    {
        Mesh mesh = mf.sharedMesh;
        Material[] sharedMaterials = mf.GetComponent<Renderer>().sharedMaterials;
        Vector2 textureOffset = mf.GetComponent<Renderer>().material.GetTextureOffset("_MainTex");
        Vector2 textureScale = mf.GetComponent<Renderer>().material.GetTextureScale("_MainTex");

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

    public static void ExportMeshToAsset(MeshFilter mf)
    {

        Mesh newMesh = _copyMeshFromMeshFilter(mf);

        if (newMesh == null) return;

        string savePath = EditorUtility.SaveFolderPanel("输出路径", Application.dataPath, "");
        if (string.IsNullOrEmpty(savePath)) return;

        string p = _checkFliePath(savePath, newMesh.name, ".asset");
        p = p.Replace(Application.dataPath, "Assets");

        try
        {
            AssetDatabase.CreateAsset(newMesh, p);
            AssetDatabase.SaveAssets();
        }
        catch (Exception)
        {
            //
        }
        AssetDatabase.Refresh();
    }

    public static void ExportMeshWithWorldTranToAsset(MeshFilter mf)
    {

        Mesh newMesh = _copyMeshFromMeshFilter(mf, mf.transform.localToWorldMatrix);

        if (newMesh == null) return;

        string savePath = EditorUtility.SaveFolderPanel("输出路径", Application.dataPath, "");
        if (string.IsNullOrEmpty(savePath)) return;

        string p = _checkFliePath(savePath, newMesh.name, ".asset");
        p = p.Replace(Application.dataPath, "Assets");

        try
        {
            AssetDatabase.CreateAsset(newMesh, p);
            AssetDatabase.SaveAssets();
        }
        catch (Exception)
        {
            //
        }
        AssetDatabase.Refresh();
    }

    public static void ExportMaxMeshToUnityAsset(MeshFilter mf)
    {

        Mesh newMesh = _maxMeshToUnityMesh(mf);

        if (newMesh == null) return;

        string savePath = EditorUtility.SaveFolderPanel("输出路径", Application.dataPath, "");
        if (string.IsNullOrEmpty(savePath)) return;

        string p = _checkFliePath(savePath, newMesh.name, ".asset");
        p = p.Replace(Application.dataPath, "Assets");

        try
        {
            AssetDatabase.CreateAsset(newMesh, p);
            AssetDatabase.SaveAssets();
        }
        catch (Exception)
        {
            //
        }
        AssetDatabase.Refresh();
    }

    public static void ExprotMaxMeshToUnityPrefab(MeshFilter mf)
    {

        //Selection.activeGameObject = null;

        Mesh newMesh = _maxMeshToUnityMesh(mf);

        if (newMesh == null) return;

        string savePath = EditorUtility.SaveFolderPanel("保存预制体路径", Application.dataPath, "");
        if (string.IsNullOrEmpty(savePath)) return;


        if (!Directory.Exists(savePath + "/" + mf.gameObject.name))
        {
            string guid = AssetDatabase.CreateFolder(savePath.Replace(Application.dataPath, "Assets"),
                mf.gameObject.name);
            savePath = AssetDatabase.GUIDToAssetPath(guid);
        }
        else
        {
            savePath = savePath + "/" + mf.gameObject.name;
        }

        string msavePath = savePath + "/mesh";
        if (!Directory.Exists(msavePath))
        {
            string guid = AssetDatabase.CreateFolder(savePath.Replace(Application.dataPath, "Assets"), "mesh");
            msavePath = AssetDatabase.GUIDToAssetPath(guid);
        }

        string p = _checkFliePath(msavePath, newMesh.name, ".asset");
        p = p.Replace(Application.dataPath, "Assets");

        string pp = _checkFliePath(savePath, mf.gameObject.name, ".prefab");
        pp = pp.Replace(Application.dataPath, "Assets");

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

            GameObject asset = PrefabUtility.CreatePrefab(pp, mf.gameObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorPlusMethods.NextEditorApplicationUpdateDo(() =>
            {
                mf.gameObject.Dispose();
                Selection.activeGameObject = asset;
            });

            /*
            Vector3 ls = mf.transform.localScale;
            Vector3 la = mf.transform.localEulerAngles;
            Vector3 lp = mf.transform.localPosition;
            Transform pt = mf.transform.parent;
            int si = mf.transform.GetSiblingIndex();

            GameObject nga = AssetDatabase.LoadAssetAtPath<GameObject>(pp);
            //            GameObject ngai = GameObject.Instantiate(nga);
            GameObject ngai = (GameObject)PrefabUtility.InstantiatePrefab(nga);
            ngai.name = nga.name;

            ngai.transform.localScale = ls;
            ngai.transform.localEulerAngles = la;
            ngai.transform.localPosition = lp;
            if (pt) ngai.transform.SetParent(pt);
            ngai.transform.SetSiblingIndex(si);

           // Selection.activeGameObject = ngai;

            mf.gameObject.Dispose();*/
        }
        catch (Exception)
        {
            //
        }

        AssetDatabase.Refresh();

    }

    public static void ExprotMeshToOBJFile(MeshFilter mf)
    {

        Mesh newMesh = _copyMeshFromMeshFilter(mf);
        if (newMesh == null) return;

        string savePath = EditorUtility.SaveFolderPanel("输出路径", Application.dataPath, "");
        if (string.IsNullOrEmpty(savePath)) return;

        string fileStr = _meshToObjFileStr(mf, Vector3.one);
        if (string.IsNullOrEmpty(fileStr)) return;

        string p = _checkFliePath(savePath, newMesh.name, ".obj");

        AorIO.SaveStringToFile(p, fileStr);
        AssetDatabase.Refresh();
    }

    public static void ExprotMeshWithWorldTranToOBJFile(MeshFilter mf)
    {

        Mesh newMesh = _copyMeshFromMeshFilter(mf, mf.transform.localToWorldMatrix);
        if (newMesh == null) return;

        string savePath = EditorUtility.SaveFolderPanel("输出路径", Application.dataPath, "");
        if (string.IsNullOrEmpty(savePath)) return;

        string fileStr = _meshToObjFileStr(mf, Vector3.one);
        if (string.IsNullOrEmpty(fileStr)) return;

        string p = _checkFliePath(savePath, newMesh.name, ".obj");

        AorIO.SaveStringToFile(p, fileStr);
        AssetDatabase.Refresh();
    }
}

[CustomEditor(typeof(MeshFilter),true)]
public class MeshFilterPlus : Editor
{

    private bool Advanced = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        GUILayout.BeginVertical("box");

        GUILayout.Label("Mesh导出工具");

        GameObject prefab = (target as MeshFilter).gameObject;
        PrefabType pt = PrefabUtility.GetPrefabType(prefab);

        if (pt == PrefabType.ModelPrefabInstance || pt == PrefabType.DisconnectedModelPrefabInstance)
        {

            GUILayout.BeginHorizontal();

            if (
                GUILayout.Button(new GUIContent("Mesh_max导出为Asset_unity,并保存预制体",
                    "将Max导出的FBX Mesh的坐标系转换成Unity坐标系,并保存为预制体")))
            {
                MeshExportUtil.ExprotMaxMeshToUnityPrefab(target as MeshFilter);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(5);

        }

        Advanced = GUILayout.Toggle(Advanced, "高级Mesh导出选项");
        if (Advanced)
        {

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Mesh_max导出为Asset_unity", "将Max导出的FBX Mesh的坐标系转换成Unity坐标系,并保存为Asset文件")))
            {
                MeshExportUtil.ExportMaxMeshToUnityAsset(target as MeshFilter);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Mesh导出为asset"))
            {
                MeshExportUtil.ExportMeshToAsset(target as MeshFilter);
            }

            if (GUILayout.Button("重置Mesh坐标变化并导出为asset"))
            {
                MeshExportUtil.ExportMeshWithWorldTranToAsset(target as MeshFilter);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Mesh导出为.obj"))
            {
                MeshExportUtil.ExprotMeshToOBJFile(target as MeshFilter);
            }

            if (GUILayout.Button("重置Mesh坐标变化并导出为.obj"))
            {
                MeshExportUtil.ExprotMeshWithWorldTranToOBJFile(target as MeshFilter);
            }

            GUILayout.EndHorizontal();

        }

        GUILayout.EndVertical();
    }

}
