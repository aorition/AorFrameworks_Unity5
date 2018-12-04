using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GPUTextureUtils : MonoBehaviour
{
    private struct GPUTexTask
    {
        public RenderTexture Target;
        public Material Mat;

        public GPUTexTask(RenderTexture target, Material mat)
        {
            Target = target;
            Mat = mat;
        }
    }

    private static void Init()
    {
        _rootObject = new GameObject("GPUTextureUtils");
        _rootObject.transform.position = new Vector3(0, 0, -10);
        _instance = _rootObject.AddComponent<GPUTextureUtils>();
        _instance._camera = _rootObject.AddComponent<Camera>();
        _instance._camera.orthographic = true;
        _instance._camera.orthographicSize = 0.5f;
        _instance._camera.cullingMask = LayerMask.GetMask("Nothing");
        _instance._camera.clearFlags = CameraClearFlags.Nothing;
        _instance._camera.targetTexture = new RenderTexture(1, 1, 0);
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        _instance._mesh = quad.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(quad);
    }

    private static GameObject _rootObject;
    private static GPUTextureUtils _instance;
    public static GPUTextureUtils Instacne {
        get {
            if (!_instance)
            {
                GameObject go = GameObject.Find("GPUTextureUtils");
                if (go)
                {
                    DestroyImmediate(go);
                }
                Init();
            }
            return _instance;
        }
    }

    public static void Process(RenderTexture target, Material mat)
    {
        Instacne._tasks.Add(new GPUTexTask(target, mat));
    }

    public static void SetMaterialWithTarget(RenderTexture target, Texture2D mainTex)
    {
        GPUTexTask task;
        int iCount = _instance._tasks.Count;
        for (int i = 0; i < iCount; i++)
        {
            task = Instacne._tasks[i];
            if (task.Target == target)
            {
                task.Mat.SetTexture("_MainTex", mainTex);
            }
        }
    }

    //------------------------------------------------------------ 

    private List<GPUTexTask> _tasks = new List<GPUTexTask>();
    private Camera _camera;
    private Mesh _mesh;

    private void OnPostRender()
    {
        for (int i = 0; i < _tasks.Count; i++)
        {
            GPUTexTask task = _tasks[i];

            //            GL.Clear(false, true, Color.black);
            Graphics.SetRenderTarget(task.Target.colorBuffer, task.Target.depthBuffer);
            task.Mat.SetPass(0);
            Graphics.DrawMeshNow(_mesh, Vector3.zero, Quaternion.identity);
        }
        _tasks.Clear();
    }
}