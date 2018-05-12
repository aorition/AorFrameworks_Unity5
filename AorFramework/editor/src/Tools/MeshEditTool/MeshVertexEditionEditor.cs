using System;
using System.Collections.Generic;
using Framework.Tools;
using UnityEditor;
using UnityEngine;

namespace Framework.editor
{
    [CustomEditor(typeof(MeshVertexEdition))]
    public class MeshVertexEditionEditor : UnityEditor.Editor
    {

        private static float CapSizeDefine = 0.025f;

        protected void OnSceneGUI()
        {
            MeshVertexEdition edition = target as MeshVertexEdition;
            SkinnedMeshRenderer skinned = edition.GetComponent<SkinnedMeshRenderer>();
            MeshFilter meshFilter = edition.GetComponent<MeshFilter>();
            if (!meshFilter && skinned) return;

            Mesh mesh;
            if (skinned)
            {
                mesh = skinned.sharedMesh;
            }
            else
            {
                mesh = meshFilter.sharedMesh;
            }

            List<Vector3> vertexs = new List<Vector3>();
            mesh.GetVertices(vertexs);

            List<Vector3> normals = new List<Vector3>();
            mesh.GetNormals(normals);

            Vector3 ScamForward = SceneView.lastActiveSceneView.camera.transform.forward;
            Matrix4x4 l2wMatrix = meshFilter.transform.localToWorldMatrix;
            for (int i = 0; i < vertexs.Count; i++)
            {
                Vector3 n3w = l2wMatrix*normals[i];
                Vector3 v3w = l2wMatrix*vertexs[i];

                v3w += meshFilter.transform.position;

                if (Vector3.Dot(ScamForward,n3w) < 0)
                {
                    float capSize = HandleUtility.GetHandleSize(v3w) * CapSizeDefine;
                    Handles.Button(v3w, Quaternion.FromToRotation(Vector3.left, n3w),
                        capSize, capSize, Handles.DotHandleCap);
                }
            }


        }

    }
}
