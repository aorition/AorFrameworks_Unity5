namespace Framework.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    public enum SelectVertexType
    {
        Brush = 0,
        Mouse,
    }

    public class VBrushEditor : BrushEditorBase
    {
        protected SelectVertexType editType = SelectVertexType.Mouse;
        protected MeshFilter originMF;
        protected MeshFilter curMF;
        public MeshFilter curEditMF
        {
            get
            {
                if (curMF == null)
                    return originMF;
                return curMF;
            }
            set
            {
                curMF = value;
            }

        }
        
        Vector3 startP;
        Vector3 endP;
        bool isDrag = false;
        protected Vector3 curBrushPos;

        private List<int> selectVertexs = new List<int>();
        protected bool isSelectExcludeBack;
        private Color selectedVertexColor = Color.red;
        private float selectedVertexSize = 0.02f;
        private bool vertexToolSettingsToggle = false;

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


        protected override void OnPrePainting()//暂时这样
        {
            base.OnPrePainting();

            if (editType != SelectVertexType.Brush)
            {
                PaintingEnable = false;
            }
            else
            {
                PaintingEnable = true;
            }
        }

        protected virtual bool FilterPainting(Event e, RaycastHit hit)
        {
            if ((e.type != EventType.MouseDown && e.type != EventType.MouseDrag) || e.alt)
                return false;

            if (curBrushPos == hit.point)
                return false;
            return true;
        }

        protected override void OnPainting(Event e, RaycastHit raycastHit)
        {
            if (FilterPainting(e, raycastHit))
            {
                selectVertexs.Clear();
                curBrushPos = raycastHit.point;
                var mesh = curEditMF.sharedMesh;
                float dist;

                for (int i = 0; i < mesh.vertices.Length; i++)
                {
                    Vector3 worldPos = curEditMF.transform.TransformPoint(mesh.vertices[i]);
                    var sqLength = Vector3.Dot(curBrushPos - worldPos, curBrushPos - worldPos) -
                                   Mathf.Pow(Vector3.Dot(curBrushPos - worldPos, raycastHit.normal), 2);
                    if (sqLength < BrushPrev.orthographicSize * BrushPrev.orthographicSize)
                    {
                        if (isSelectExcludeBack)
                        {
                            var sceneView = SceneView.currentDrawingSceneView;
                            if (Vector3.Dot(curEditMF.sharedMesh.normals[i], sceneView.camera.transform.forward) < 0)
                            {
                                selectVertexs.Add(i);
                            }
                        }
                        else
                            selectVertexs.Add(i);  
                    }
                }

                PaintVertices(selectVertexs);
            }
        }

        protected virtual void PaintVertices(List<int> vertexs)
        {
            
        }

        public List<int> GetCurSelects()
        {
            return selectVertexs;
        }

        protected void ExecuteEvent(Event e)
        {
            //默认实现 鼠标左键按下(拖动) 和 鼠标弹起
            if (e.type == EventType.mouseDown)
            {
                if (e.button == 0)
                {
                    startP = e.mousePosition;
                    if (e.alt)
                        return;

                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    RaycastHit hit;
                    bool isHit = Physics.Raycast(ray, out hit, 1000);
  
                    if (hit.transform == ((MonoBehaviour)target).transform)
                    {
                        //Debug.LogWarning("triangle index " + hit.triangleIndex);
                        if (e.control)
                        {
                            ProcessSelectedAdditive(hit);
                        }
                        else
                        {
                            ProcessSelected(hit);
                        }
                        PaintVertices(selectVertexs);
                        Repaint();
                    }
                }
            }
            else if (e.type == EventType.mouseDrag)
            {
                //mouseDrag
                if (e.button == 0)
                {
                    if (e.alt)
                        return;

                    endP = e.mousePosition;
                    isDrag = true;

                    if (startP != endP)
                    {
                        var sceneView = SceneView.currentDrawingSceneView;
                        ProcessSelectedMulti(sceneView, ConvertPos(sceneView, startP), ConvertPos(sceneView, endP));
                        PaintVertices(selectVertexs);
                        Repaint();
                    }
                }
            }
            else if (e.type == EventType.mouseUp || Event.current.type == EventType.Ignore)
            {
                isDrag = false;
                Repaint();
            }
        }

        protected override void OnSceneGUI()
        {
            if (curEditMF == null)
                return;

            base.OnSceneGUI();

            if (editType == SelectVertexType.Mouse)
            {
                Event e = Event.current;
                HandleUtility.AddDefaultControl(0);
                ExecuteEvent(e);

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
            }

            if (editType == SelectVertexType.Mouse)
            {
                if (IsVertexSelected())
                {
                    var mesh = curEditMF.sharedMesh;
                    foreach (var index in selectVertexs)
                    {
                        //var disPosition = mesh.vertices[index] + curEditMF.transform.position;
                        var disPosition = curEditMF.transform.TransformPoint(mesh.vertices[index]);
                        Handles.color = selectedVertexColor;
                        Handles.SphereHandleCap(0, disPosition, Quaternion.identity, selectedVertexSize,
                            Event.current.type);
                    }
                }
            }

            SceneView.RepaintAll();
        }



        private bool IsVertexSelected()
        {
            return selectVertexs.Count > 0;
        }

        public override void OnInspectorGUI()
        {
            editType = (SelectVertexType)EditorGUILayout.EnumPopup("Edit Type:", editType);
            isSelectExcludeBack = EditorGUILayout.Toggle("is Exclude Back Vertex", isSelectExcludeBack);
            if (editType == SelectVertexType.Brush)
                BrushSize = (int)EditorGUILayout.Slider("BrushSize", BrushSize, 0f, 20f);
            else
            {
                vertexToolSettingsToggle = EditorGUILayout.Foldout(vertexToolSettingsToggle, "tool setting");
                if (vertexToolSettingsToggle)
                {
                    EditorGUI.indentLevel++;
                    selectedVertexColor = EditorGUILayout.ColorField("Selected Vertex Color", selectedVertexColor);
                    selectedVertexSize = EditorGUILayout.FloatField("Selected Vertex Size", selectedVertexSize);
                    EditorGUI.indentLevel--;
                }
            }
        }

        public void ProcessSelectedAdditive(RaycastHit hit)
        {
            var index = GetVertexByClick(hit);
            if (selectVertexs.Contains(index))
            {
                //curData.curVertexs.Remove(index);
                selectVertexs.RemoveAll(id => curEditMF.sharedMesh.vertices[id] == curEditMF.sharedMesh.vertices[index]);
            }
            else
            {
                //curData.curVertexs.Add(index);
                for (int i = 0; i < curEditMF.sharedMesh.vertices.Length; i++)
                {
                    if (curEditMF.sharedMesh.vertices[i] == curEditMF.sharedMesh.vertices[index])
                        selectVertexs.Add(i);
                }
            }
        }

        public void ProcessSelected(RaycastHit hit)
        {
            var index = GetVertexByClick(hit);
            if (selectVertexs.Count == 1 && selectVertexs[0] == index)
            {
                return;
            }
            selectVertexs.Clear();

            for (int i = 0; i < curEditMF.sharedMesh.vertices.Length; i++)
            {
                if (curEditMF.sharedMesh.vertices[i] == curEditMF.sharedMesh.vertices[index])
                    selectVertexs.Add(i);
            }

            //Debug.LogWarning("ProcessSelected " + curData.curVertexs.Count);
        }

        private int findSelectedVertexIndex(RaycastHit hit)
        {
            var hitP = curEditMF.transform.InverseTransformPoint(hit.point);// hit.point - curEditMF.transform.position;//

            //Debug.LogWarning(hit.point);
            float minDistance = float.MaxValue;
            int minIndex = 0;
            for (int i = 0; i < curEditMF.sharedMesh.vertices.Length; i++)
            {
                float distance = Vector3.Distance(curEditMF.sharedMesh.vertices[i], hitP);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minIndex = i;
                }
            }
            return minIndex;
        }

        public int GetVertexByClick(RaycastHit hit)
        {

            //if (hit.triangleIndex < 0)
            return findSelectedVertexIndex(hit);
            //int i1 = curMesh.triangles[hit.triangleIndex * 3];
            //int i2 = curMesh.triangles[hit.triangleIndex * 3 + 1];
            //int i3 = curMesh.triangles[hit.triangleIndex * 3 + 2];
            //var hitP = curMeshFilter.transform.InverseTransformPoint(hit.point);
            //float distance1 = Vector3.Distance(curMesh.vertices[i1], hitP);
            //float distance2 = Vector3.Distance(curMesh.vertices[i2], hitP);
            //float distance3 = Vector3.Distance(curMesh.vertices[i3], hitP);
            ////Debug.LogWarning(i1 + " " + i2 + " " + i3 + " " + hit.triangleIndex + " " + distance1 + " " + distance2 + " " + distance3);
            //if (distance1 < distance2 && distance1 < distance3)
            //    return i1;
            //if (distance2 < distance1 && distance2 < distance3)
            //    return i2;
            //if (distance3 < distance1 && distance3 < distance2)
            //    return i3;
            //return i1;
        }

        public void ProcessSelectedMulti(SceneView sceneView, Vector3 start, Vector3 end)
        {
            Vector3 p1 = Vector3.zero;
            Vector3 p2 = Vector3.zero;

            if (start.x > end.x)
            {//这些判断是用来确保p1的xy坐标小于p2的xy坐标，因为画的框不见得就是左下到右上这个方向的
                p1.x = end.x;
                p2.x = start.x;
            }
            else {
                p1.x = start.x;
                p2.x = end.x;
            }

            if (start.y > end.y)
            {
                p1.y = end.y;
                p2.y = start.y;
            }

            else {
                p1.y = start.y;
                p2.y = end.y;
            }

            selectVertexs.Clear();
            for (int i = 0; i < curEditMF.sharedMesh.vertices.Length; i++)
            {//把可选择的对象保存在characters数组里
                var disPosition = curEditMF.transform.TransformPoint(curEditMF.sharedMesh.vertices[i]);
                Vector3 location = sceneView.camera.WorldToScreenPoint(disPosition);//把对象的position转换成屏幕坐标
                                                                                    //Debug.LogWarning(location + " " + p1 + " " + p2);
                if (location.x < p1.x || location.x > p2.x || location.y < p1.y || location.y > p2.y
                || location.z < sceneView.camera.nearClipPlane || location.z > sceneView.camera.farClipPlane)//z方向就用摄像机的设定值，看不见的也不需要选择了
                {
                    //disselecting(obj);//上面的条件是筛选 不在选择范围内的对象，然后进行取消选择操作，比如把物体放到default层，就不显示轮廓线了
                }
                else
                {  //selecting(obj);//否则就进行选中操作，比如把物体放到画轮廓线的层去
                    if (isSelectExcludeBack)
                    {
                        if (Vector3.Dot(curEditMF.sharedMesh.normals[i], sceneView.camera.transform.forward) < 0)
                        {
                            selectVertexs.Add(i);
                        }
                    }
                    else
                        selectVertexs.Add(i);
                }

            }
        }
    }


}