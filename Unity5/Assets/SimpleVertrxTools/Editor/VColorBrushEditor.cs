using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace Framework.Editor
{
    [CustomEditor(typeof(VertextBrush))]
    [RequireComponent(typeof(MeshFilter))]
    public class VColorBrushEditor : VBrushEditor
    {
        private Color modifyColor = Color.white;
        public Color[] backUpColors;
        private Material backUpMaterial;
        public bool isModify = false;
        public Mesh curHoldOriginMesh;
        public MeshRenderer curHoldOriginMR;
        public Mesh curHoldSharedMesh;
        //public virtual void OnEnable()
        //{
        //    var vs = target as VertextBrush;
        //    originMF = vs.originMeshFilter;
        //    curMF = vs.copyMeshFilter;
        //    Undo.undoRedoPerformed += DiaplayUndoRedo;
        //}

        public virtual void OnEnable()
        {
            var vs = target as VertextBrush;
            var curOriginMF = GetMeshFilter(vs.transform);
            bool isNotMeshSame = curOriginMF.sharedMesh != vs.curHoldOriginMesh;
            //Debug.LogWarning(" curOriginMF.sharedMesh != curHoldOriginMesh " + curOriginMF.sharedMesh + "   " + (vs.curHoldOriginMesh == null) );
            originMF = curOriginMF;
            
            if (curMF == null || isNotMeshSame)
            {
                var mf = curOriginMF;
                if (mf != null)
                {
                    string path = AssetDatabase.GetAssetPath(mf.sharedMesh);
                    Debug.LogWarning(path);
                    if (path.Contains(".FBX"))
                    {
                        var trans = vs.transform.Find(name + "_MeshCopy");
                        Debug.LogWarning("mesh trans " + trans);
                        if (trans != null)
                        {
                            DestroyImmediate(trans.gameObject);
                        }
                        var copyObj = new GameObject(name + "_MeshCopy");
                        //target.tag = "meshCopy";
                        copyObj.transform.SetParent(vs.transform);
                        copyObj.transform.localPosition = Vector3.zero;
                        copyObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        copyObj.transform.localScale = Vector3.one;
                        copyObj.hideFlags = HideFlags.HideInHierarchy;
                        var meshFilter = copyObj.AddComponent<MeshFilter>();
                        var meshRenderer = copyObj.AddComponent<MeshRenderer>();
                        var render = GetMeshRenderer(vs.transform);
                        //meshRenderer.sharedMaterial = render.sharedMaterial;
                        //Material t =(Material)AssetDatabase.LoadAssetAtPath("Assets/SimpleVertrxTools/UnityVC/Standard.mat", typeof(Material));
                        Material t = new Material(Shader.Find("Hidden/ShowVertexColor"));
                        if (render.sharedMaterial != null)
                        {
                            t.mainTexture = render.sharedMaterial.mainTexture;
                            t.mainTextureOffset = render.sharedMaterial.mainTextureOffset;
                            t.mainTextureScale = render.sharedMaterial.mainTextureScale;
                        }
                        meshRenderer.sharedMaterial = t;
                        //Resources.UnloadAsset(t);
                        //meshRenderer.material.
                        render.enabled = false;
                        curHoldOriginMR = render;

                        Mesh mesh = new Mesh();
                        mesh.name = copyObj.transform.name + "_Mesh";
                        mesh.vertices = (Vector3[])mf.sharedMesh.vertices.Clone();
                        mesh.triangles = (int[])mf.sharedMesh.triangles.Clone();
                        mesh.normals = (Vector3[])mf.sharedMesh.normals.Clone();
                        mesh.uv = (Vector2[])mf.sharedMesh.uv.Clone();
                        mesh.uv2 = (Vector2[])mf.sharedMesh.uv2.Clone();
                        mesh.colors = (Color[])mf.sharedMesh.colors.Clone();
                        meshFilter.sharedMesh = mesh;
                        mesh.RecalculateNormals();

                        curMF = meshFilter;
         
                        if (curMF.sharedMesh.colors.Length < curMF.sharedMesh.vertices.Length)
                        {
                            var colors = new Color[curMF.sharedMesh.vertices.Length];
                            for (int i = 0; i < colors.Length; i++)
                            {
                                colors[i] = Color.white;
                            }
                            curMF.sharedMesh.colors = colors;
                        }
                    }
                    else
                    {
                        var render = GetMeshRenderer(vs.transform);
                        //backUpMaterial = render.sharedMaterial;
                        //Material t = (Material)AssetDatabase.LoadAssetAtPath("Assets/SimpleVertrxTools/UnityVC/Standard.mat", typeof(Material));
                        Material t = new Material(Shader.Find("Hidden/ShowVertexColor"));
                        if (render.sharedMaterial != null)
                        {
                            t.mainTexture = render.sharedMaterial.mainTexture;
                            t.mainTextureOffset = render.sharedMaterial.mainTextureOffset;
                            t.mainTextureScale = render.sharedMaterial.mainTextureScale;
                        }
                        if (isNotMeshSame || vs.backUpColors == null)
                        {
                            backUpColors = originMF.sharedMesh.colors;
                            vs.backUpColors = backUpColors;
                        }
                        else
                        {
                            backUpColors = vs.backUpColors;
                        }

                        if (render.sharedMaterial != t && vs.curHoldOriginMesh == null)
                        {
                            vs.backUpMaterial = render.sharedMaterial;
                            render.sharedMaterial = t;
                        }
                        backUpMaterial = vs.backUpMaterial;
                        curHoldOriginMR = render;
                        //Resources.UnloadAsset(t);

                        if (originMF.sharedMesh.colors.Length < originMF.sharedMesh.vertices.Length)
                        {
                            var colors = new Color[originMF.sharedMesh.vertices.Length];
                            for (int i = 0; i < colors.Length; i++)
                            {
                                colors[i] = Color.white;
                            }
                            originMF.sharedMesh.colors = colors;
                        }
                    }

                    curHoldSharedMesh = curEditMF.sharedMesh;

                }
                else
                {
                    curMF = null;
                    curHoldSharedMesh = null;
                    curHoldOriginMR = null;
                }


                vs.curHoldOriginMesh = originMF.sharedMesh;
                curHoldOriginMesh = originMF.sharedMesh;
            }
            Undo.undoRedoPerformed += DiaplayUndoRedo;
        }

        public void OnDisable()
        {
            var vs = target as VertextBrush;
            Undo.undoRedoPerformed -= DiaplayUndoRedo;
            Debug.LogWarning("this is very + " + vs + "+ " + vs.curHoldOriginMesh);
            if (vs != null && vs.curHoldOriginMesh != null)
                return;

            if (curHoldSharedMesh != null)
            {
                string path = AssetDatabase.GetAssetPath(curHoldOriginMesh);
                if (path.Contains(".FBX"))
                {
                    if (!isModify)
                    {
                        curHoldSharedMesh.colors = backUpColors;
                    }

                    if (curMF)
                    {
                        DestroyImmediate(curMF.gameObject);
                    }
                    if (curHoldOriginMR != null)
                    {
                        curHoldOriginMR.enabled = true;
                    }
                }
                else
                {
                    if (curHoldOriginMR != null)
                    {
                        curHoldOriginMR.sharedMaterial = backUpMaterial;
                    }
                    if (!isModify)
                    {
                        curHoldSharedMesh.colors = backUpColors;
                    }
                }
            }
        }

        private void DiaplayUndoRedo()
        {
            //Debug.LogWarning("this is undo redo ");
            Mesh mesh = curEditMF.sharedMesh;
            var colors = mesh.colors;
            mesh.colors = colors;
            SceneView.RepaintAll();
        }

        private void EditEnd()
        {
            var vs = target as VertextBrush;

            DestroyImmediate(vs);
        }
        
        protected override void PaintVertices(List<int> vertex)
        {
            if (editType == SelectVertexType.Brush)
            {
                Mesh mesh = curEditMF.sharedMesh;
                var colors = mesh.colors;

                bool isModify = false;
                for (int i = 0; i < vertex.Count; i++)
                {
                    var index = vertex[i];
                    if (colors[index] != modifyColor)
                    {
                        colors[index] = modifyColor;
                        isModify = true;
                    }
                }

                if (isModify)
                {
                    Undo.RegisterCompleteObjectUndo(curEditMF.sharedMesh, "change color");
                    mesh.colors = colors;         
                }
            }
        }

        private void OnEditBrushGUI()
        {
            EditorGUILayout.LabelField("Color Information");
            modifyColor = EditorGUILayout.ColorField("Color", modifyColor);
        }

        private void OnEditMouseGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Color Information");
     
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            string s;
            var vertexs = GetCurSelects();
            if (vertexs.Count < 1)
                s = "none";
            else if (vertexs.Count == 1)
                s = vertexs[0].ToString();
            else
                s = vertexs[0].ToString() + ", " + vertexs[1].ToString() + ", ...";
            EditorGUILayout.LabelField(string.Format("Indexs: {0}", s));

            if (vertexs.Count > 0)
            {
                Mesh mesh = curEditMF.sharedMesh;
                var frist = vertexs[0];
                var colors = mesh.colors;
                var oldColor = colors[frist];

                colors[frist] = EditorGUILayout.ColorField("Color", colors[frist]);

                if (oldColor != colors[frist])
                {
                    foreach (var index in vertexs)
                    {
                        colors[index] = colors[frist];
                    }
                    Undo.RegisterCompleteObjectUndo(curEditMF.sharedMesh, "change color");
                    mesh.colors = colors;            
                }
            }
            EditorGUILayout.EndVertical();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (editType == SelectVertexType.Brush)
                OnEditBrushGUI();
            else
                OnEditMouseGUI();


            var old = GUI.color;
            GUI.color = Color.yellow;
            if (GUILayout.Button("Save The Mesh", "LargeButton"))
            {
                //string path = AssetDatabase.GetAssetPath(_toolData.meshFilter.sharedMesh);
                //EditorUtility.FocusProjectWindow();
                //Mesh t = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
                //Selection.activeObject = t;

                (target as VertextBrush).curHoldOriginMesh = null;

                if (curEditMF == null)
                {
                    EditorUtility.DisplayDialog("提示", "当前没有编辑的mesh", "OK");
                    return;
                }

                if (curEditMF == originMF)
                {
                    var vs = target as VertextBrush;
                    isModify = true;
                    return;
                }

                string saveDir = "";
                string scenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
                if (UnityEngine.SceneManagement.SceneManager.sceneCount == 1 && !string.IsNullOrEmpty(scenePath))
                {
                    saveDir = scenePath.Substring(0, scenePath.LastIndexOf('/'));
                }
                else
                {
                    string seldir = EditorUtility.OpenFolderPanel("选择保存目录", "", "");
                    if (string.IsNullOrEmpty(seldir))
                    {
                        return;
                    }
                    saveDir = seldir;
                }

                string savePath = saveDir + "/ModifyColorMeshs";
                if (!Directory.Exists(savePath))
                {
                    string guid = AssetDatabase.CreateFolder(saveDir, "ModifyColorMeshs");
                    savePath = AssetDatabase.GUIDToAssetPath(guid);
                }

                if (AssetDatabase.Contains(curEditMF.sharedMesh))
                {
                    var vs = target as VertextBrush;
                    isModify = true;
                    backUpColors = (Color[])curEditMF.sharedMesh.colors.Clone();
                    vs.backUpColors = backUpColors;
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    isModify = true;
                    var vs = target as VertextBrush;
                    backUpColors = (Color[])curEditMF.sharedMesh.colors.Clone();
                    vs.backUpColors = backUpColors;
                    MeshFilter mf = curEditMF;
                    string path = AssetDatabase.GetAssetPath(originMF.sharedMesh);
                    //EditorUtility.FocusProjectWindow();
                    //var saveDir = path.Substring(0, path.LastIndexOf('/'));
                    //Debug.LogWarning(" ==== " + savePath + " " + path);
                    AssetDatabase.CreateAsset(mf.sharedMesh, savePath + "/" + originMF.gameObject.name + ".asset");
                    AssetDatabase.SaveAssets();                 
                    //Mesh t = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
                    //Selection.activeObject = t;
                }

            }

            //GUI.color = Color.red;
            //if (GUILayout.Button("Edit End", "LargeButton"))
            //{
            //    //EditEnd();
            //}

            GUI.color = old;
        }
        
        private MeshFilter GetMeshFilter(Transform transform)
        {
            MeshFilter meshFilter = transform.GetComponentInChildren<MeshFilter>();
            while (meshFilter == null && transform.parent != null)
            {
                meshFilter = GetMeshFilter(transform.parent);
            }
            return meshFilter;
        }

        private MeshRenderer GetMeshRenderer(Transform transform)
        {
            MeshRenderer meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
            while (meshRenderer == null && transform.parent != null)
            {
                meshRenderer = GetMeshRenderer(transform.parent);
            }
            return meshRenderer;
        }
    }


}