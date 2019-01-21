namespace com.emadkhezri.vertextool
{
    using UnityEditor;
    using UnityEngine;

    using System;
    using System.Linq;
    using System.Reflection;

    [InitializeOnLoad]
    public class RXLookingGlass
    {
        public static Type type_HandleUtility;
        protected static MethodInfo meth_IntersectRayMesh;

        static RXLookingGlass()
        {
            var editorTypes = typeof(Editor).Assembly.GetTypes();

            type_HandleUtility = editorTypes.FirstOrDefault(t => t.Name == "HandleUtility");
            meth_IntersectRayMesh = type_HandleUtility.GetMethod("IntersectRayMesh", (BindingFlags.Static | BindingFlags.NonPublic));
        }

        public static bool IntersectRayMesh(Ray ray, MeshFilter meshFilter, out RaycastHit hit)
        {
            return IntersectRayMesh(ray, meshFilter.mesh, meshFilter.transform.localToWorldMatrix, out hit);
        }

        static object[] parameters = new object[4];
        public static bool IntersectRayMesh(Ray ray, Mesh mesh, Matrix4x4 matrix, out RaycastHit hit)
        {
            parameters[0] = ray;
            parameters[1] = mesh;
            parameters[2] = matrix;
            parameters[3] = null;
            bool result = (bool)meth_IntersectRayMesh.Invoke(null, parameters);
            hit = (RaycastHit)parameters[3];
            return result;
        }
    }

    public class VertexToolSceneManager
    {
        VertexProcessor _vertexProcessor;
        VertexToolData _data;
        Vector3 startP;
        Vector3 endP;
        bool isMouseDown = false;
        GameObject curSelectObject = null;
        GameObject curEditMeshObj = null;

        public VertexToolSceneManager(VertexToolData data)
        {
            _data = data;
            _vertexProcessor = new VertexProcessor(_data);
        }

        private void ChangeHandleTool()
        {
            //if (Tools.current == Tool.View)
            //{
            //    _currentHandleTool = HandleTool.None;
            //}
            //else if (Tools.current == Tool.Move)
            //{
            //    _currentHandleTool = HandleTool.Move;
            //}
            //else if (Tools.current == Tool.Rotate)
            //{
            //    _currentHandleTool = HandleTool.Rotate;
            //}
            //else if (Tools.current == Tool.Scale)
            //{
            //    _currentHandleTool = HandleTool.Scale;
            //}
            //Tools.current = Tool.None;

            //if (_secondaryHandle)
            //{
            //    _currentHandleTool = HandleTool.None;
            //}
        }

        public Vector3 ConvertPos(SceneView sceneView, Vector2 pos)
        {
            float mult = 1;
#if UNITY_5_4_OR_NEWER
            mult = EditorGUIUtility.pixelsPerPoint;
#endif

            // 转换成摄像机可接受的屏幕坐标，左下角是（0，0，0）右上角是（camera.pixelWidth，camera.pixelHeight，0）
            pos.y = sceneView.camera.pixelHeight - pos.y * mult;
            pos.x *= mult;

            return pos;
        }

        Vector3 hitPoint;
        int _currentHoverTriangle;
        bool isDrag = false;
        public void OnSceneGUI(SceneView sceneView)
        {
            //if (Event.current.type != EventType.MouseMove && Event.current.type != EventType.Layout)
            //    Debug.LogWarning("EventType   " + Event.current.type + (int)Event.current.type + isMouseDown + Event.current.button);
            if (!_data.isEditVertex)
                return;
            if (Selection.activeGameObject != curSelectObject && Selection.activeGameObject != null)
            {
                curSelectObject = Selection.activeGameObject;
                var mf = GetMeshFilter(curSelectObject.transform);
                Debug.LogWarning("mesh mf " + mf);
                if (mf != null)
                {
                    _data.originMeshFilter = mf;
                    var trans = curSelectObject.transform.Find(curSelectObject.name + "_MeshCopy");
                    Debug.LogWarning("mesh trans " + trans);
                    if (trans == null)
                    {
                        var target = new GameObject(curSelectObject.name + "_MeshCopy");
                        //target.tag = "meshCopy";
                        target.transform.SetParent(curSelectObject.transform);
                        target.transform.localPosition = Vector3.zero;
                        target.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        target.transform.localScale = Vector3.one;
                        //target.hideFlags = HideFlags.HideInHierarchy;
                        var meshFilter = target.AddComponent<MeshFilter>();
                        var meshRenderer = target.AddComponent<MeshRenderer>();
                        var render = GetMeshRenderer(curSelectObject.transform);
                        //meshRenderer.sharedMaterial = render.sharedMaterial;
                        Material t = (Material)AssetDatabase.LoadAssetAtPath("Assets/SimpleVertrxTools/UnityVC/Standard.mat", typeof(Material));   
                        meshRenderer.material = t;
                        Resources.UnloadAsset(t);
                        //meshRenderer.material.
                        render.enabled = false;
                        curEditMeshObj = target;

                        Mesh mesh = new Mesh();
                        mesh.name = target.transform.name + "_Mesh";
                        mesh.vertices = (Vector3[])mf.sharedMesh.vertices.Clone();
                        mesh.triangles = (int[])mf.sharedMesh.triangles.Clone();
                        mesh.normals = (Vector3[])mf.sharedMesh.normals.Clone();
                        mesh.uv = (Vector2[])mf.sharedMesh.uv.Clone();
                        mesh.uv2 = (Vector2[])mf.sharedMesh.uv2.Clone();
                        mesh.colors = (Color[])mf.sharedMesh.colors.Clone();
                        meshFilter.sharedMesh = mesh;
                        mesh.RecalculateNormals();
                    }
                    else
                        curEditMeshObj = trans.gameObject;
                }
                else
                {
                    curSelectObject = null;
                    _data.originMeshFilter = null;
                }
            }

            if (curSelectObject == null)
                return;
            ChangeHandleTool();
            if (Event.current.type == EventType.MouseDown)
            {
                startP = Event.current.mousePosition;
            }
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (((Event.current.type == EventType.MouseDown && Event.current.button == 0) || Event.current.type == EventType.MouseMove) && Event.current.alt == false)
            {
                ClearHoverTarget();

                Vector2 mousePosition = Event.current.mousePosition;

                // Retina 屏幕需要拉伸值
                float mult = 1;
#if UNITY_5_4_OR_NEWER
                mult = EditorGUIUtility.pixelsPerPoint;
#endif

                // 转换成摄像机可接受的屏幕坐标，左下角是（0，0，0）右上角是（camera.pixelWidth，camera.pixelHeight，0）
                mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y * mult;
                mousePosition.x *= mult;

                // 近平面往里一些，才能看得到摄像机里的位置
                Vector3 fakePoint = mousePosition;
                fakePoint.z = 20;
                Vector3 point = sceneView.camera.ScreenToWorldPoint(fakePoint);

                Ray ray = sceneView.camera.ScreenPointToRay(mousePosition);
                RaycastHit hit = default(RaycastHit);
                // MeshFilter[] componentsInChildren = GameObject.FindObjectsOfType<MeshFilter>();
                float num = float.PositiveInfinity;
                // foreach (MeshFilter meshFilter in componentsInChildren)
                // {
                _data.meshFilter = GetMeshFilter(curEditMeshObj.transform);
                if (_data.meshFilter != null)
                {
                    var mesh1 = _data.meshFilter.sharedMesh;
                    //Debug.LogWarning(mesh1.vertices.Length);
                    Mesh sharedMesh = _data.meshFilter.sharedMesh;

                    if (sharedMesh
                        && RXLookingGlass.IntersectRayMesh(ray, sharedMesh, _data.meshFilter.transform.localToWorldMatrix, out hit))
                    // && hit.distance < num)
                    {
                        // point = hit.point;
                        // num = hit.distance;
                        //Debug.LogWarning("hit.triangleIndex" + hit.triangleIndex);
                    }
                }
                // }
                //Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                //RaycastHit hit;
                //bool isHit = Physics.Raycast(ray, out hit, 1000);
                if (_data.meshFilter != null)
                {
                    //_data.meshFilter = GetMeshFilter(hit.transform);
                    _data.selectedObjectName = _data.originMeshFilter.name;
                    if (_data.meshFilter != null)
                    {
                        if (Event.current.type == EventType.MouseDown)
                        {
                            if (Event.current.control)
                            {
                                _data.isVertexSelected = _vertexProcessor.ProcessSelectedAdditive(hit);
                                if (_data.isVertexSelected)
                                    return;
                                _data.isVertexSelected = false;
                                _vertexProcessor.Clean();
                            }
                            else
                            {
                                _data.isVertexSelected = true;
                                _vertexProcessor.ProcessSelected(hit);
                                hitPoint = hit.point;
                                //Debug.LogWarning("hit._currentHoverTriangle" + _currentHoverTriangle);               
                                return;
                            }
                        }
                        else
                        {
                            //_currentHoverTriangle = hit.triangleIndex;//不显示附近顶点
                        }
                    }
                    else
                    {
                        _data.isVertexSelected = false;
                        _vertexProcessor.Clean();
                    }
                }
                else
                {
                    _data.isVertexSelected = false;
                    _data.selectedObjectName = VertexToolData.NO_OBJECT_SELECTED;
                    _data.meshFilter = null;
                    _vertexProcessor.Clean();
                }
                
            }
            
            if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragExited) && Event.current.button == 0 && Event.current.control)
            {
                //Debug.LogWarning("EventType.MouseDrag == ===" + startP + " " + endP);
                _data.isVertexSelected = false;
                if (Event.current.type == EventType.DragExited)
                    isDrag = false;
                else
                    isDrag = true;
                //_vertexProcessor.Clean();
                endP =  Event.current.mousePosition;
                if (startP != endP)
                {
                    bool isSelect = _vertexProcessor.ProcessSelectedMulti(sceneView, ConvertPos(sceneView, startP), ConvertPos(sceneView, endP));
                    if (isSelect)
                    {
                        _data.isVertexSelected = true;
                    }
                    else
                    {
                        _data.isVertexSelected = false;
                        _vertexProcessor.Clean();
                    }
                }
       
            }
            if (Event.current.type == EventType.DragExited || !Event.current.control)
                isDrag = false;
            if (isDrag)
            {
                Handles.BeginGUI();
                Vector3 p1 = startP;
                Vector3 p2 = endP;
                if (p1.x > p2.x)
                {
                    var temp = p1.x;
                    p1.x = p2.x;
                    p2.x = temp;
                }
                if (p1.y > p2.y)
                {
                    var temp = p1.y;
                    p1.y = p2.y;
                    p2.y = temp;
                }
                var width = p2.x - p1.x;
                var height = p2.y - p1.y;
            
                EditorGUI.DrawRect(new Rect(p1.x, p1.y, width, height), new Color(0.5f, 0.5f, 0.85f, 0.3f));
                Handles.EndGUI();
            }
            //Debug.LogWarning(" data  == " + _data.isVertexSelected);
            if (_currentHoverTriangle >= 0 && _data.meshFilter != null)
            {
                var mesh = _data.meshFilter.sharedMesh;
                for (int i = 0; i < 3; i++)
                {
                    var disPosition1 = mesh.vertices[mesh.triangles[_currentHoverTriangle * 3 + i]] + _data.meshFilter.transform.position;
                    Handles.Label(disPosition1, "Vertex" + mesh.triangles[_currentHoverTriangle * 3 + i] + " - " + (_currentHoverTriangle * 3 + i));
                    Handles.color = Color.cyan;
                    Handles.SphereHandleCap(0, disPosition1, Quaternion.identity, _data.selectedVertexSize, Event.current.type);
                }
            }

            if (_data.isVertexSelected)
            {
                var mesh = _data.meshFilter.sharedMesh;
                foreach (var index in _data.curVertexs)
                {
                    var disPosition = mesh.vertices[index] + _data.meshFilter.transform.position;
                    //Handles.Label(disPosition, "Selected Vertex");
                    //Handles.color = _data.normalArrowColor;
                    //Handles.ArrowHandleCap(0, disPosition, Quaternion.LookRotation(_data.VertexData.Normal), _data.normalArrowSize, Event.current.type);
                    //Handles.color = _data.solidDiskColor;
                    //Handles.DrawSolidDisc(disPosition, _data.VertexData.Normal, _data.solidDiskSize);
                    Handles.color = _data.selectedVertexColor;
                    Handles.SphereHandleCap(0, disPosition, Quaternion.identity, _data.selectedVertexSize, Event.current.type);
                }       

                //Handles.color = Color.yellow;
 
                //Handles.SphereHandleCap(0, hitPoint, Quaternion.identity, _data.selectedVertexSize, Event.current.type);
            }
            SceneView.RepaintAll();
        }

        private void ClearHoverTarget()
        {
            if (_currentHoverTriangle != -100)
                _currentHoverTriangle = -100;
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