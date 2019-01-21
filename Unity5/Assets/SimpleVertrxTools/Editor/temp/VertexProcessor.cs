namespace com.emadkhezri.vertextool
{
    using UnityEngine;
    using UnityEditor;

    public class VertexProcessor
    {
        VertexToolData _data;

        public VertexProcessor(VertexToolData data)
        {
            _data = data;
        }

        public void Clean()
        {
            _data.vertexCount = 0;
            _data.subMeshCount = 0;
            _data.curVertexs.Clear();
            //_data.VertexData.Position = Vector3.zero;
            //_data.VertexData.Normal = Vector3.zero;
            //_data.VertexData.Tangent = Vector3.zero;
            //_data.VertexData.Color = Color.clear;
            //_data.VertexData.UV = Vector3.zero;
            //_data.VertexData.UV2 = Vector3.zero;
            //_data.VertexData.UV3 = Vector3.zero;
            //_data.VertexData.UV4 = Vector3.zero;
            EditorWindow.GetWindow<VertexToolWindow>().Repaint();
        }

        public bool ProcessSelectedAdditive(RaycastHit hit)
        {
             Mesh mesh = _data.meshFilter.sharedMesh;
            _data.vertexCount = mesh.vertices.Length;
            _data.subMeshCount = mesh.subMeshCount;
            //_data.IndexFormat = mesh.indexFormat;
            var index = GetVertexByClick(hit);//findSelectedVertexIndex(mesh.vertices, hitPoint - _data.meshFilter.transform.position);
            if (_data.curVertexs.Contains(index))
            {
                _data.curVertexs.Remove(index);
            }
            else
            {
                _data.curVertexs.Add(index);
            }
            initColors();
            EditorWindow.GetWindow<VertexToolWindow>().Repaint();
            if (_data.curVertexs.Count > 0)
                return true;
            else
                return false;
        }

        private void initColors()
        {
            if (_data.meshFilter != null)
            {
                Mesh mesh = _data.meshFilter.sharedMesh;
                if (mesh.colors.Length < mesh.vertices.Length)
                {
                    _data.preColors = new Color[mesh.vertices.Length];
                    _data.curColors = new Color[mesh.vertices.Length];
                    for (int i = 0; i < _data.curColors.Length; i++)
                    {
                        _data.preColors[i] = Color.white;
                        _data.curColors[i] = Color.white;
                    }
                    mesh.colors = _data.preColors;
                }
                else
                {
                    _data.preColors = new Color[mesh.vertices.Length];
                    _data.curColors = new Color[mesh.vertices.Length];
                    for (int i = 0; i < _data.curColors.Length; i++)
                    {
                        _data.preColors[i] = mesh.colors[i];
                        _data.curColors[i] = mesh.colors[i];
                    }
                }
            }
         }

        public void ProcessSelected(RaycastHit hit)
        {
            Mesh mesh = _data.meshFilter.sharedMesh;
            _data.vertexCount = mesh.vertices.Length;
            _data.subMeshCount = mesh.subMeshCount;
            //_data.IndexFormat = mesh.indexFormat;
            var index = GetVertexByClick(hit);//findSelectedVertexIndex(mesh.vertices, hitPoint - _data.meshFilter.transform.position);
            if (_data.curVertexs.Count  == 1 && _data.curVertexs[0] == index)
            {
                return;
            }
            _data.curVertexs.Clear();
            _data.curVertexs.Add(index);
            initColors();
            
            //_data.VertexData.Position = mesh.vertices[_data.VertexData.Index];
            //_data.VertexData.Normal = mesh.normals[_data.VertexData.Index];
            //_data.VertexData.Tangent = mesh.tangents[_data.VertexData.Index];
            //if (_data.VertexData.Index < mesh.colors.Length)
            //    _data.VertexData.Color = mesh.colors[_data.VertexData.Index];

            //if (_data.VertexData.Index < mesh.uv.Length)
            //    _data.VertexData.UV = mesh.uv[_data.VertexData.Index];

            //if (_data.VertexData.Index < mesh.uv2.Length)
            //    _data.VertexData.UV2 = mesh.uv2[_data.VertexData.Index];

            //if (_data.VertexData.Index < mesh.uv3.Length)
            //    _data.VertexData.UV3 = mesh.uv3[_data.VertexData.Index];

            //if (_data.VertexData.Index < mesh.uv4.Length)
            //    _data.VertexData.UV4 = mesh.uv4[_data.VertexData.Index];

            EditorWindow.GetWindow<VertexToolWindow>().Repaint();
        }

        private int findSelectedVertexIndex(Vector3[] vertices, Vector3 point)
        {
            float minDistance = float.MaxValue;
            int minIndex = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                float distance = Vector3.Distance(vertices[i], point);
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
            Mesh mesh = _data.meshFilter.sharedMesh;
            //Debug.LogWarning(hit.triangleIndex);
            int i1 = mesh.triangles[hit.triangleIndex * 3];
            int i2 = mesh.triangles[hit.triangleIndex * 3 + 1];
            int i3 = mesh.triangles[hit.triangleIndex * 3 + 2];
            var hitP = hit.point - _data.meshFilter.transform.position;
            float distance1 = Vector3.Distance(mesh.vertices[i1], hitP);
            float distance2 = Vector3.Distance(mesh.vertices[i2], hitP);
            float distance3 = Vector3.Distance(mesh.vertices[i3], hitP);
            //Debug.LogWarning(i1 + " " + i2 + " " + i3 + " " + hit.triangleIndex + " " + distance1 + " " + distance2 + " " + distance3);
            if (distance1 < distance2 && distance1 < distance3)
                return i1;
            if (distance2 < distance1 && distance2 < distance3)
                return i2;
            if (distance3 < distance1 && distance3 < distance2)
                return i3;
            return i1;
        }

        public bool ProcessSelectedMulti(SceneView sceneView, Vector3 start, Vector3 end)
        {
            Mesh mesh = _data.meshFilter.sharedMesh;
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

            _data.curVertexs.Clear();
            for (int i = 0; i < mesh.vertices.Length; i++)
            {//把可选择的对象保存在characters数组里
                var disPosition = mesh.vertices[i] + _data.meshFilter.transform.position;
                Vector3 location = sceneView.camera.WorldToScreenPoint(disPosition);//把对象的position转换成屏幕坐标
                //Debug.LogWarning(location + " " + p1 + " " + p2);
                if (location.x < p1.x || location.x > p2.x || location.y < p1.y || location.y > p2.y
                || location.z < Camera.main.nearClipPlane || location.z > Camera.main.farClipPlane)//z方向就用摄像机的设定值，看不见的也不需要选择了
                {
                    //disselecting(obj);//上面的条件是筛选 不在选择范围内的对象，然后进行取消选择操作，比如把物体放到default层，就不显示轮廓线了
                }
                else
                {  //selecting(obj);//否则就进行选中操作，比如把物体放到画轮廓线的层去
                    if (_data.isSelectExcludeBack)
                    {
                        if (Vector3.Dot(mesh.normals[i], sceneView.camera.transform.forward) < 0)
                        {
                            _data.curVertexs.Add(i);
                        }
                    }
                    else
                        _data.curVertexs.Add(i);
                }

            }
            initColors();
     
            if (_data.curVertexs.Count > 0)
            {
                EditorWindow.GetWindow<VertexToolWindow>().Repaint();
                return true;
            }
                
            return false;
        }
    }

}