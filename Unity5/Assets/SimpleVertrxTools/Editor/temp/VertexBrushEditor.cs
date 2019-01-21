//using System.Collections;
//using System.Collections.Generic;
//using Framework.editor;
//using UnityEditor;
//using UnityEngine;

//public enum VertexEditType
//{
//    Brush = 0,
//    Mouse, 
//}

//public class VertexBrushData
//{
//    public VertexEditType editType = VertexEditType.Mouse;
//    public bool isVertexSelected = false;
//    public List<int> curVertexs = new List<int>();
//    public MeshFilter meshFilter;
//    public MeshFilter originMeshFilter;
//    public Color selectedVertexColor = Color.red;
//    public float selectedVertexSize = 0.02f;
//    public bool isSelectExcludeBack = true;
//    public bool vertexToolSettingsToggle = true;
//    public bool vertexToolInfoToggle = true;

//    public float _brushRadius;
//    public Color _brushColor = Color.white;
//    //public float _opacity = 1;
//    public float colorRatio = 1;
//}


//public class VertexInfoTool
//{
//    private VertexBrushEditor holder;
//    private Mesh mesh;
//    private VertexBrushData _toolData;
//    public bool isMouseDown;
//    public Vector3 hitP;

//    public VertexInfoTool(VertexBrushData toolData, VertexBrushEditor holder)
//    {
//        this.holder = holder;
//        _toolData = toolData;
//        mesh = _toolData.meshFilter.sharedMesh;
//        if (mesh.colors.Length < mesh.vertices.Length)
//        {
//            var colors = new Color[mesh.vertices.Length];
//            for (int i = 0; i < colors.Length; i++)
//            {
//                colors[i] = Color.white;
//            }
//            mesh.colors = colors;
//        }
//    }


//    public void OnSceneGUI()
//    {
//        if (isMouseDown)
//            PaintVertices(hitP);
//    }

//    private void PaintVertices(Vector3 brushPosition)
//    {
//        Vector3 localPenPoint = _toolData.meshFilter.transform.InverseTransformPoint(brushPosition);

//        Color[] colors = mesh.colors;

//        float dist;

//        for (int i = 0; i < mesh.vertices.Length; i++)
//        {
//            Vector3 worldPos = _toolData.meshFilter.transform.TransformPoint(mesh.vertices[i]);
//            //Debug.LogWarning(Vector3.Distance(worldPos, brushPosition) + "pppp===pp  " + _toolData._brushRadius);
//            if ((dist = Vector3.Distance(worldPos, brushPosition)) < _toolData._brushRadius)
//            {
//                colors[i] = Color.black;;
//                //Debug.LogWarning(" ApplyBrush " + i + " --- " + colors[i]);
//            }
//        }

//        mesh.colors = colors;
//    }

//    public void OnGUI()
//    {
//        _toolData.isSelectExcludeBack = EditorGUILayout.Toggle("is Exclude Back Vertex", _toolData.isSelectExcludeBack);
//        _toolData.selectedVertexColor = EditorGUILayout.ColorField("Selected Vertex Color", _toolData.selectedVertexColor);
//        _toolData.selectedVertexSize = EditorGUILayout.FloatField("Selected Vertex Size", _toolData.selectedVertexSize);
//        GUILayout.Space(10);
//        _toolData.vertexToolInfoToggle = EditorGUILayout.Foldout(_toolData.vertexToolInfoToggle, "Vertex Information");
//        if (_toolData.vertexToolInfoToggle)
//        {
//            EditorGUI.indentLevel++;
//            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
//            SelectedInfoGUI();
//            EditorGUILayout.EndVertical();
//            EditorGUI.indentLevel--;
//        }
//    }

//    private void SelectedInfoGUI()
//    {
//        EditorGUI.indentLevel++;
//        string s;
//        if (_toolData.curVertexs.Count < 1)
//            s = "none";
//        else if (_toolData.curVertexs.Count == 1)
//            s = _toolData.curVertexs[0].ToString();
//        else
//            s = _toolData.curVertexs[0].ToString() + ", " + _toolData.curVertexs[1].ToString() + ", ...";
//        EditorGUILayout.LabelField(string.Format("Indexs: {0}", s));

//        if (_toolData.curVertexs.Count > 0)
//        {
//            Mesh curMesh = mesh;
//            var frist = _toolData.curVertexs[0];
//            var colors = mesh.colors;
//            var oldColor = colors[frist];

//            colors[frist] = EditorGUILayout.ColorField("Color", colors[frist]);

//            if (oldColor != colors[frist])
//            {
//                foreach (var index in _toolData.curVertexs)
//                {
//                    colors[index] = colors[frist];
//                }
//                curMesh.colors = colors;
//            }
//        }

//        EditorGUI.indentLevel--;
//    }

//    public void OnBrushGUI()
//    {
//        holder.BrushSize = (int)EditorGUILayout.Slider("Size", holder.BrushSize, 0f, 10f);
//    }

//}

////[CustomEditor(typeof(VertextBrush))]
//public class VertexBrushEditor : BrushEditor
//{
//    private VertexBrushData _data = new VertexBrushData();
//    private VertexSelector curSelector;
//    private VertexInfoTool curInfoTool;
//    Vector3 startP;
//    Vector3 endP;
//    bool isDrag = false;

//    public virtual void OnEnable()
//    {
//        var vs = target as VertextBrush;
//        _data.meshFilter = vs.copyMeshFilter;
//        _data.originMeshFilter = vs.originMeshFilter;
//        curSelector = new VertexSelector(_data);
//        curInfoTool = new VertexInfoTool(_data, this);
//    }

//    private void EditEnd()
//    {
//        var vs = target as VertextBrush;

//        DestroyImmediate(vs);
//    }

//    public Vector3 ConvertPos(SceneView sceneView, Vector2 pos)
//    {
//        float mult = 1;
//#if UNITY_5_4_OR_NEWER
//        mult = EditorGUIUtility.pixelsPerPoint;
//#endif

//        // 转换成摄像机可接受的屏幕坐标，左下角是（0，0，0）右上角是（camera.pixelWidth，camera.pixelHeight，0）
//        pos.y = sceneView.camera.pixelHeight - pos.y * mult;
//        pos.x *= mult;

//        return pos;
//    }

//    protected override void OnPaintingMouseDown(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
//    {
//        base.OnPaintingMouseDown(raycastHit, ctrl, alt, shift);
//        curInfoTool.isMouseDown = true;
//        curInfoTool.hitP = raycastHit.point;
//    }

//    protected override void OnPaintingMouseDrag(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
//    {
//        base.OnPaintingMouseDrag(raycastHit, ctrl, alt, shift);
//        curInfoTool.hitP = raycastHit.point;
//    }

//    protected override void OnPaintingMouseUp(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
//    {
//        base.OnPaintingMouseUp(raycastHit, ctrl, alt, shift);
//        curInfoTool.isMouseDown = false;
//    }

//    protected override void OnPrePainting()//暂时这样
//    {
//        base.OnPrePainting();

//        if (_data.editType != VertexEditType.Brush)
//        {
//            PaintingEnable = false;
//        }
//        else
//        {
//            PaintingEnable = true;
//        }
//    }

//    protected void ExecuteEvent(Event e)
//    {
//        //默认实现 鼠标左键按下(拖动) 和 鼠标弹起
//        if (e.type == EventType.mouseDown)
//        {
//            if (e.button == 0)
//            {
//                startP = e.mousePosition;
//                if (e.alt)
//                    return;
//                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
//                RaycastHit hit;
//                bool isHit = Physics.Raycast(ray, out hit, 1000);
//                if (hit.transform == ((VertextBrush) target).transform)
//                {
//                    //Debug.LogWarning("triangle index " + hit.triangleIndex);
//                    if (e.control)
//                    {
//                        _data.isVertexSelected = curSelector.ProcessSelectedAdditive(hit);
//                    }
//                    else
//                    {
//                        _data.isVertexSelected = true;
//                        curSelector.ProcessSelected(hit);
//                        Repaint();
//                    }
//                }
//            }   
//        }
//        else if (e.type == EventType.mouseDrag)
//        {
//            //mouseDrag
//            if (e.button == 0)
//            {
//                if (e.alt)
//                    return;

//                _data.isVertexSelected = false;
//                endP = e.mousePosition;
//                isDrag = true;

//                if (startP != endP)
//                {
//                    var sceneView = SceneView.currentDrawingSceneView;
//                    bool isSelect = curSelector.ProcessSelectedMulti(sceneView, ConvertPos(sceneView, startP), ConvertPos(sceneView, endP));
//                    if (isSelect)
//                    {
//                        _data.isVertexSelected = true;
//                    }
//                    else
//                    {
//                        _data.isVertexSelected = false;
//                    }
//                    Repaint();
//                }
//            }  
//        }
//        else if (e.type == EventType.mouseUp || Event.current.type == EventType.Ignore)
//        {
//            isDrag = false;
//            Repaint();
//        }
//    }

//    protected override void OnSceneGUI()
//    {
//        base.OnSceneGUI();

//        if (_data.editType == VertexEditType.Mouse)
//        {
//            Event e = Event.current;
//            HandleUtility.AddDefaultControl(0);
//            ExecuteEvent(e);

//            if (isDrag)
//            {
//                Handles.BeginGUI();
//                Vector3 p1 = startP;
//                Vector3 p2 = endP;
//                if (p1.x > p2.x)
//                {
//                    var temp = p1.x;
//                    p1.x = p2.x;
//                    p2.x = temp;
//                }
//                if (p1.y > p2.y)
//                {
//                    var temp = p1.y;
//                    p1.y = p2.y;
//                    p2.y = temp;
//                }
//                var width = p2.x - p1.x;
//                var height = p2.y - p1.y;

//                EditorGUI.DrawRect(new Rect(p1.x, p1.y, width, height), new Color(0.5f, 0.5f, 0.85f, 0.3f));
//                Handles.EndGUI();
//            }

//            if (_data.isVertexSelected)
//            {
//                var mesh = _data.meshFilter.sharedMesh;
//                foreach (var index in _data.curVertexs)
//                {
//                    var disPosition = mesh.vertices[index] + _data.meshFilter.transform.position;
//                    Handles.color = _data.selectedVertexColor;
//                    Handles.SphereHandleCap(0, disPosition, Quaternion.identity, _data.selectedVertexSize,
//                        Event.current.type);
//                }
//            }
//        }
//        else
//        {
//            curInfoTool.OnSceneGUI();
//        }
//        SceneView.RepaintAll();
//    }

//    public override void OnInspectorGUI()
//    {

//        _data.editType = (VertexEditType)EditorGUILayout.EnumPopup("Edit Type:", _data.editType);

//        if (_data.editType != VertexEditType.Brush)
//            curInfoTool.OnGUI();
//        else
//            curInfoTool.OnBrushGUI();
    

//        var old = GUI.color;
//        GUI.color = Color.yellow;
//        if (GUILayout.Button("Save The Mesh", "LargeButton"))
//        {
//            //string path = AssetDatabase.GetAssetPath(_toolData.meshFilter.sharedMesh);
//            //EditorUtility.FocusProjectWindow();
//            //Mesh t = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
//            //Selection.activeObject = t;
//            if (_data.meshFilter == null)
//            {
//                EditorUtility.DisplayDialog("提示", "当前没有编辑的mesh", "OK");
//                return;
//            }
//            MeshFilter mf = _data.meshFilter;
//            string path = AssetDatabase.GetAssetPath(_data.originMeshFilter.sharedMesh);
//            EditorUtility.FocusProjectWindow();
//            var saveDir = path.Substring(0, path.LastIndexOf('/'));
//            Debug.LogWarning(" ==== " + saveDir + " " + path);
//            AssetDatabase.CreateAsset(mf.sharedMesh, saveDir + "/" + _data.originMeshFilter.gameObject.name + "_modified.asset");
//            AssetDatabase.SaveAssets();
//            Mesh t = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
//            Selection.activeObject = t;
//        }
        
//        GUI.color = Color.red;
//        if (GUILayout.Button("Edit End", "LargeButton"))
//        {
//            //EditEnd();
//        }

//        GUI.color = old;  
//    }
//}

