using UnityEngine;

[ExecuteInEditMode, DisallowMultipleComponent]
public class VertextBrush : MonoBehaviour
{
    [System.NonSerialized]
    public Mesh curHoldOriginMesh;
    [System.NonSerialized]
    public Color[] backUpColors;
    [System.NonSerialized]
    public Material backUpMaterial;

    private void OnDestroy()
    {
        curHoldOriginMesh = null;
    }
    //[System.NonSerialized] public Material material;
    //[System.NonSerialized] public Color[] backUpColors;
    //private Material backUpMaterial;
    //[System.NonSerialized] public bool isModify = false;

    //[System.NonSerialized]
    //public MeshFilter copyMeshFilter;

    //[System.NonSerialized]
    //public MeshFilter originMeshFilter;

    //private void OnDestroy()
    //{
    //    string path = AssetDatabase.GetAssetPath(originMeshFilter.sharedMesh);
    //    if (path.Contains(".FBX"))
    //    {
    //        if (copyMeshFilter)
    //        {
    //            if (!isModify)
    //            {
    //                copyMeshFilter.sharedMesh.colors = backUpColors;
    //            }
    //            DestroyImmediate(copyMeshFilter.gameObject);
    //        }
    //        var render = GetMeshRenderer(transform);
    //        render.enabled = true;
    //    }
    //    else
    //    {
    //        var render = GetMeshRenderer(transform);
    //        render.sharedMaterial = backUpMaterial;
    //        if (!isModify)
    //        {
    //            var originMF = GetMeshFilter(transform);
    //            originMF.sharedMesh.colors = backUpColors;
    //        }
    //    }
    //}

    //private void Awake()
    //{
    //    var mf = GetMeshFilter(transform);

    //    if (mf != null)
    //    {
    //        string path = AssetDatabase.GetAssetPath(mf.sharedMesh);
    //        originMeshFilter = mf;
    //        Debug.LogWarning(path);
    //        if (path.Contains(".FBX"))
    //        { 
    //            var trans = transform.Find(name + "_MeshCopy");
    //            Debug.LogWarning("mesh trans " + trans);
    //            if (trans == null)
    //            {
    //                var copyObj = new GameObject(name + "_MeshCopy");
    //                //target.tag = "meshCopy";
    //                copyObj.transform.SetParent(transform);
    //                copyObj.transform.localPosition = Vector3.zero;
    //                copyObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
    //                copyObj.transform.localScale = Vector3.one;
    //                copyObj.hideFlags = HideFlags.HideInHierarchy;
    //                var meshFilter = copyObj.AddComponent<MeshFilter>();
    //                var meshRenderer = copyObj.AddComponent<MeshRenderer>();
    //                var render = GetMeshRenderer(transform);
    //                //meshRenderer.sharedMaterial = render.sharedMaterial;
    //                Material t =
    //                    (Material)
    //                        AssetDatabase.LoadAssetAtPath("Assets/SimpleVertrxTools/UnityVC/Standard.mat",
    //                            typeof (Material));
    //                meshRenderer.material = t;
    //                Resources.UnloadAsset(t);
    //                //meshRenderer.material.
    //                render.enabled = false;

    //                Mesh mesh = new Mesh();
    //                mesh.name = copyObj.transform.name + "_Mesh";
    //                mesh.vertices = (Vector3[]) mf.sharedMesh.vertices.Clone();
    //                mesh.triangles = (int[]) mf.sharedMesh.triangles.Clone();
    //                mesh.normals = (Vector3[]) mf.sharedMesh.normals.Clone();
    //                mesh.uv = (Vector2[]) mf.sharedMesh.uv.Clone();
    //                mesh.uv2 = (Vector2[]) mf.sharedMesh.uv2.Clone();
    //                mesh.colors = (Color[]) mf.sharedMesh.colors.Clone();
    //                meshFilter.sharedMesh = mesh;
    //                mesh.RecalculateNormals();

    //                copyMeshFilter = meshFilter;
    //            }
    //            else
    //                copyMeshFilter = GetMeshFilter(trans);
    //            if (copyMeshFilter.sharedMesh.colors.Length < copyMeshFilter.sharedMesh.vertices.Length)
    //            {
    //                var colors = new Color[copyMeshFilter.sharedMesh.vertices.Length];
    //                for (int i = 0; i < colors.Length; i++)
    //                {
    //                    colors[i] = Color.white;
    //                }
    //                copyMeshFilter.sharedMesh.colors = colors;
    //            }
    //        }
    //        else
    //        {
    //            var render = GetMeshRenderer(transform);
    //            var originMF = GetMeshFilter(transform);
    //            backUpMaterial = render.sharedMaterial;
    //            Material t = (Material)AssetDatabase.LoadAssetAtPath("Assets/SimpleVertrxTools/UnityVC/Standard.mat", typeof(Material));
    //            backUpMaterial = render.sharedMaterial;
    //            backUpColors = originMF.sharedMesh.colors;
    //            if (render.sharedMaterial != t)
    //            {
    //                render.sharedMaterial = t;
    //            }
    //            Resources.UnloadAsset(t);

    //            if (originMF.sharedMesh.colors.Length < originMF.sharedMesh.vertices.Length)
    //            {
    //                var colors = new Color[originMF.sharedMesh.vertices.Length];
    //                for (int i = 0; i < colors.Length; i++)
    //                {
    //                    colors[i] = Color.white;
    //                }
    //                originMF.sharedMesh.colors = colors;
    //            }
    //        }

    //    }
    //    else
    //    {
    //        originMeshFilter = null;
    //        copyMeshFilter = null;
    //    }
    //}

    //private MeshFilter GetMeshFilter(Transform transform)
    //{
    //    MeshFilter meshFilter = transform.GetComponentInChildren<MeshFilter>();
    //    while (meshFilter == null && transform.parent != null)
    //    {
    //        meshFilter = GetMeshFilter(transform.parent);
    //    }
    //    return meshFilter;
    //}
    //private MeshRenderer GetMeshRenderer(Transform transform)
    //{
    //    MeshRenderer meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
    //    while (meshRenderer == null && transform.parent != null)
    //    {
    //        meshRenderer = GetMeshRenderer(transform.parent);
    //    }
    //    return meshRenderer;
    //}
}
