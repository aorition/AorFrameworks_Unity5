using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestCode
{

    [MenuItem("AAA/Do")]
    private static void Save()
    {

        //Mesh _mesh = new Mesh();
        /*
        _mesh.name = "DefaultTile";
        _mesh.vertices = new[]
        {
            new Vector3(-0.5f, 0, -0.5f),
            new Vector3(-0.5f, 0, 0.5f),
            new Vector3(0.5f, 0, 0.5f),
            new Vector3(0.5f, 0, -0.5f)
        };
        _mesh.uv = new[]
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0)
        };
        _mesh.normals = new[]
        {
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.up
        };
        _mesh.triangles = new[]
        {
            0, 1, 3, 1, 2, 3
        };*/

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Mesh _mesh = go.GetComponent<MeshFilter>().mesh;

        _mesh.name = "DefualtCube";
        _mesh.uv2 = null;

        string seldir = EditorUtility.OpenFolderPanel("选择保存目录", "", "");
        if (string.IsNullOrEmpty(seldir))
        {
            return;
        }

        string savePath = seldir + "/" + _mesh.name + ".asset";

        savePath = savePath.Replace(Application.dataPath + "/", "Assets/");

        Debug.Log("preDo **** " + Application.dataPath + "/");
        Debug.Log("Save **** " + savePath);

        AssetDatabase.CreateAsset(_mesh, savePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

}
