namespace com.emadkhezri.vertextool
{
    using UnityEditor;
    using UnityEngine;
    public class VertexInfoPanel
    {
        VertexToolData _toolData;

        public VertexInfoPanel(VertexToolData data)
        {
            _toolData = data;
        }

        public void OnGUI()
        {
            _toolData.isEditVertex = EditorGUILayout.ToggleLeft("Is Eidit Vertex", _toolData.isEditVertex, EditorStyles.boldLabel);
            GUILayout.Space(10);
            _toolData.vertexToolInfoToggle = EditorGUILayout.Foldout(_toolData.vertexToolInfoToggle, "Vertex Information");
            if (_toolData.vertexToolInfoToggle)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(string.Format("Selected Object Name: {0}", _toolData.selectedObjectName));
                //EditorGUILayout.LabelField(string.Format("Vertex Count: {0}", _toolData.vertexCount));
                //EditorGUILayout.LabelField(string.Format("Sub Mesh Count: {0}", _toolData.subMeshCount));
                //EditorGUILayout.LabelField(string.Format("Index Format: {0}", _toolData.IndexFormat));
                //EditorGUILayout.BeginToggleGroup("Selected Vertex Info", true);
                SelectedInfoGUI();
                //EditorGUILayout.EndToggleGroup();
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }
        }

        private void SelectedInfoGUI()
        {
            EditorGUI.indentLevel++;
            string s;
            if (_toolData.curVertexs.Count < 1)
                s = "none";
            else if (_toolData.curVertexs.Count == 1)
                s = _toolData.curVertexs[0].ToString();
            else
                s = _toolData.curVertexs[0].ToString() + ", " + _toolData.curVertexs[1].ToString() + ", ...";
            EditorGUILayout.LabelField(string.Format("Indexs: {0}", s));
            //_toolData.VertexData.Position=EditorGUILayout.Vector3Field("Position", _toolData.VertexData.Position);
            //_toolData.VertexData.Normal=EditorGUILayout.Vector3Field("Normal", _toolData.VertexData.Normal);
            //_toolData.VertexData.Tangent = EditorGUILayout.Vector3Field("Tangent", _toolData.VertexData.Tangent);
            //_toolData.VertexData.Color = EditorGUILayout.ColorField("Color", _toolData.VertexData.Color);
            //_toolData.VertexData.UV = EditorGUILayout.Vector2Field("UV1", _toolData.VertexData.UV);
            //_toolData.VertexData.UV2=EditorGUILayout.Vector2Field("UV2", _toolData.VertexData.UV2);
            //_toolData.VertexData.UV3=EditorGUILayout.Vector2Field("UV3", _toolData.VertexData.UV3);
            //_toolData.VertexData.UV4=EditorGUILayout.Vector2Field("UV4", _toolData.VertexData.UV4);
      

            if (_toolData.curVertexs.Count > 0)
            {
                Mesh curMesh = _toolData.meshFilter.sharedMesh;
                var frist = _toolData.curVertexs[0];
                
                var oldColor = _toolData.curColors[frist];

                _toolData.preColors[frist] = EditorGUILayout.ColorField("Color", _toolData.preColors[frist]);

                if (oldColor != _toolData.preColors[frist])
                {
                    foreach (var index in _toolData.curVertexs)
                    {
                        _toolData.preColors[index] = _toolData.preColors[frist];
                    }
                    var temp = _toolData.preColors;
                    _toolData.preColors = _toolData.curColors;
                    _toolData.curColors = temp;
                    curMesh.colors = _toolData.curColors;

                    foreach (var index in _toolData.curVertexs)
                    {
                        _toolData.preColors[index] = _toolData.preColors[frist];
                    }
                }        
            }
            if (_toolData.isEditVertex)
            {
                if (GUILayout.Button("save the mesh"))
                {
                    //string path = AssetDatabase.GetAssetPath(_toolData.meshFilter.sharedMesh);
                    //EditorUtility.FocusProjectWindow();
                    //Mesh t = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
                    //Selection.activeObject = t;
                    if (_toolData.meshFilter == null)
                    {
                        EditorUtility.DisplayDialog("提示", "当前没有编辑的mesh", "OK");
                        return;
                    }
                    MeshFilter mf = _toolData.meshFilter;
                    string path = AssetDatabase.GetAssetPath(_toolData.originMeshFilter.sharedMesh);
                    EditorUtility.FocusProjectWindow();    
                    var saveDir = path.Substring(0, path.LastIndexOf('/'));
                    Debug.LogWarning(" ==== " + saveDir + " " + path);
                    AssetDatabase.CreateAsset(mf.sharedMesh, saveDir + "/" + _toolData.originMeshFilter.gameObject.name + "_modified.asset");
                    AssetDatabase.SaveAssets();
                    Mesh t = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
                    Selection.activeObject = t;
                }
            }
            EditorGUI.indentLevel--;
        }
    }

}