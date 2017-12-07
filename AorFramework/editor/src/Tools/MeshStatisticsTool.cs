using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshStatisticsTool : EditorWindow
{
    private static bool _LockTarget;
    private static GameObject _TargetMeshRoot;

    [MenuItem("Youkia/辅助工具/Mesh数据统计工具")]
    public static MeshStatisticsTool Init()
    {
        MeshStatisticsTool w = EditorWindow.GetWindow<MeshStatisticsTool>();
        return w;
    }

    protected void OnGUI()
    {

        if (EditorApplication.isCompiling)
        {
            GUILayout.Label("Compiling code ... Please waiting.");
            Repaint();
            return;
        }

        if (!_TargetMeshRoot)
        {
            _LockTarget = false;

            _TargetMeshRoot = Selection.activeGameObject;
        }

        if (!_TargetMeshRoot)
        {
            GUILayout.Label("请在Hierarchy中选择统计目标对象");
            Repaint();
            return;
        }

        bool l_n = EditorGUILayout.Toggle("锁定对象", _LockTarget);
        if (l_n != _LockTarget)
        {
            _LockTarget = l_n;
        }

        long vertexTotal = 0;
        long triangleTotal = 0;

        List<MeshFilter> mfList = _TargetMeshRoot.transform.FindAllComponentsInChildrenIncSelf<MeshFilter>();
        int MeshFilterNum = 0;
        long MF_vertexTotal = 0;
        long MF_triangleTotal = 0;

        if (mfList != null)
        {
            MeshFilterNum = mfList.Count;
            foreach (MeshFilter meshFilter in mfList)
            {
                if (meshFilter.sharedMesh)
                {
                    vertexTotal += meshFilter.sharedMesh.vertexCount;
                    MF_vertexTotal += meshFilter.sharedMesh.vertexCount;
                    triangleTotal += meshFilter.sharedMesh.triangles.Length;
                    MF_triangleTotal += meshFilter.sharedMesh.triangles.Length;
                }

            }
        }

        List<SkinnedMeshRenderer> smrList = _TargetMeshRoot.transform.FindAllComponentsInChildrenIncSelf<SkinnedMeshRenderer>();
        int SkinnedMeshRendererNum = 0;
        long SMR_vertexTotal = 0;
        long SMR_triangleTotal = 0;
        if (smrList != null)
        {
            SkinnedMeshRendererNum = smrList.Count;
            foreach (SkinnedMeshRenderer smrRenderer in smrList)
            {
                if (smrRenderer.sharedMesh)
                {
                    vertexTotal += smrRenderer.sharedMesh.vertexCount;
                    SMR_vertexTotal += smrRenderer.sharedMesh.vertexCount;
                    triangleTotal += smrRenderer.sharedMesh.triangles.Length;
                    SMR_triangleTotal += smrRenderer.sharedMesh.triangles.Length;
                }

            }
        }

        List<SpriteRenderer> srList = _TargetMeshRoot.transform.FindAllComponentsInChildrenIncSelf<SpriteRenderer>();
        int SpriteRendererNum = 0;
        long SR_vertexTotal = 0;
        long SR_triangleTotal = 0;
        if (srList != null)
        {
            SpriteRendererNum = srList.Count;
            foreach (SpriteRenderer srRenderer in srList)
            {
                if (srRenderer.sprite)
                {
                    vertexTotal += srRenderer.sprite.vertices.Length;
                    SR_vertexTotal += srRenderer.sprite.vertices.Length;
                    triangleTotal += srRenderer.sprite.triangles.Length;
                    SR_triangleTotal += srRenderer.sprite.triangles.Length;
                }

            }
        }

        GUILayout.Label("Vertex Total : " + vertexTotal.ToString("N0"));
        GUILayout.Label("Triangle Total : " + triangleTotal.ToString("N0"));

        GUILayout.Space(10);

        GUILayout.Label("MeshFilter : " + MeshFilterNum.ToString("N0"));
        GUILayout.Label("MeshFilter Vertex Total : " + MF_vertexTotal.ToString("N0"));
        GUILayout.Label("MeshFilter Triangle Total : " + MF_triangleTotal.ToString("N0"));

        GUILayout.Space(5);

        GUILayout.Label("SkinnedMeshRenderer : " + SkinnedMeshRendererNum.ToString("N0"));
        GUILayout.Label("SkinnedMeshRenderer Vertex Total : " + SMR_vertexTotal.ToString("N0"));
        GUILayout.Label("SkinnedMeshRenderer Triangle Total : " + SMR_triangleTotal.ToString("N0"));

        GUILayout.Space(5);

        GUILayout.Label("SpriteRenderer : " + SpriteRendererNum.ToString("N0"));
        GUILayout.Label("SpriteRenderer Vertex Total : " + SR_vertexTotal.ToString("N0"));
        GUILayout.Label("SpriteRenderer Triangle Total : " + SR_triangleTotal.ToString("N0"));

        if (!_LockTarget)
        {
            _TargetMeshRoot = null;
        }

        Repaint();
    }

}
