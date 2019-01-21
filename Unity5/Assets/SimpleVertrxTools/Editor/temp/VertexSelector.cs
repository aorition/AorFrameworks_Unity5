//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//public class VertexSelector
//{
//    private VertexBrushData curData;
//    private Mesh curMesh;
//    private MeshFilter curMeshFilter;

//    public VertexSelector(VertexBrushData data)
//    {
//        curData = data;
//        curMeshFilter = curData.meshFilter;
//        curMesh = curMeshFilter ? curMeshFilter.sharedMesh : null;
//    }

//    public bool ProcessSelectedAdditive(RaycastHit hit)
//    {
//        var index = GetVertexByClick(hit);
//        if (curData.curVertexs.Contains(index))
//        {
//            //curData.curVertexs.Remove(index);
//            curData.curVertexs.RemoveAll(id => curMesh.vertices[id] == curMesh.vertices[index]);
//        }
//        else
//        {
//            //curData.curVertexs.Add(index);
//            for (int i = 0; i < curMesh.vertices.Length; i++)
//            {
//                if (curMesh.vertices[i] == curMesh.vertices[index])
//                    curData.curVertexs.Add(i);
//            }
//        }

//        if (curData.curVertexs.Count > 0)
//            return true;
//        else
//            return false;
//    }

//    public void ProcessSelected(RaycastHit hit)
//    {
//        var index = GetVertexByClick(hit);
//        if (curData.curVertexs.Count  == 1 && curData.curVertexs[0] == index)
//        {
//            return;
//        }
//        curData.curVertexs.Clear();

//        for (int i = 0; i < curMesh.vertices.Length; i++)
//        {
//            if (curMesh.vertices[i] == curMesh.vertices[index])
//                curData.curVertexs.Add(i);
//        }

//        //Debug.LogWarning("ProcessSelected " + curData.curVertexs.Count);
//    }

//    private int findSelectedVertexIndex(RaycastHit hit)
//    {
//        var hitP = hit.point - curMeshFilter.transform.position;//curMeshFilter.transform.InverseTransformPoint(hit.point);

//        //Debug.LogWarning(hit.point);
//        float minDistance = float.MaxValue;
//        int minIndex = 0;
//        for (int i = 0; i < curMesh.vertices.Length; i++)
//        {
//            float distance = Vector3.Distance(curMesh.vertices[i], hitP);
//            if (distance < minDistance)
//            {
//                minDistance = distance;
//                minIndex = i;
//            }
//        }
//        return minIndex;
//    }

//    public int GetVertexByClick(RaycastHit hit)
//    {
  
//        //if (hit.triangleIndex < 0)
//            return findSelectedVertexIndex(hit);
//        //int i1 = curMesh.triangles[hit.triangleIndex * 3];
//        //int i2 = curMesh.triangles[hit.triangleIndex * 3 + 1];
//        //int i3 = curMesh.triangles[hit.triangleIndex * 3 + 2];
//        //var hitP = curMeshFilter.transform.InverseTransformPoint(hit.point);
//        //float distance1 = Vector3.Distance(curMesh.vertices[i1], hitP);
//        //float distance2 = Vector3.Distance(curMesh.vertices[i2], hitP);
//        //float distance3 = Vector3.Distance(curMesh.vertices[i3], hitP);
//        ////Debug.LogWarning(i1 + " " + i2 + " " + i3 + " " + hit.triangleIndex + " " + distance1 + " " + distance2 + " " + distance3);
//        //if (distance1 < distance2 && distance1 < distance3)
//        //    return i1;
//        //if (distance2 < distance1 && distance2 < distance3)
//        //    return i2;
//        //if (distance3 < distance1 && distance3 < distance2)
//        //    return i3;
//        //return i1;
//    }

//    public bool ProcessSelectedMulti(SceneView sceneView, Vector3 start, Vector3 end)
//    {
//        Vector3 p1 = Vector3.zero; 
//        Vector3 p2 = Vector3.zero;

//        if (start.x > end.x)
//        {//这些判断是用来确保p1的xy坐标小于p2的xy坐标，因为画的框不见得就是左下到右上这个方向的
//            p1.x = end.x;
//            p2.x = start.x;
//        }
//        else {
//            p1.x = start.x;
//            p2.x = end.x;
//        }

//        if (start.y > end.y)
//        {
//            p1.y = end.y;
//            p2.y = start.y;
//        }

//        else {
//            p1.y = start.y;
//            p2.y = end.y;
//        }

//        curData.curVertexs.Clear();
//        for (int i = 0; i < curMesh.vertices.Length; i++)
//        {//把可选择的对象保存在characters数组里
//            var disPosition = curMeshFilter.transform.TransformPoint(curMesh.vertices[i]);
//             Vector3 location = sceneView.camera.WorldToScreenPoint(disPosition);//把对象的position转换成屏幕坐标
//            //Debug.LogWarning(location + " " + p1 + " " + p2);
//            if (location.x < p1.x || location.x > p2.x || location.y < p1.y || location.y > p2.y
//            || location.z < Camera.main.nearClipPlane || location.z > Camera.main.farClipPlane)//z方向就用摄像机的设定值，看不见的也不需要选择了
//            {
//                //disselecting(obj);//上面的条件是筛选 不在选择范围内的对象，然后进行取消选择操作，比如把物体放到default层，就不显示轮廓线了
//            }
//            else
//            {  //selecting(obj);//否则就进行选中操作，比如把物体放到画轮廓线的层去
//                if (curData.isSelectExcludeBack)
//                {
//                    if (Vector3.Dot(curMesh.normals[i], sceneView.camera.transform.forward) < 0)
//                    {
//                        curData.curVertexs.Add(i);
//                    }
//                }
//                else
//                    curData.curVertexs.Add(i);
//            }

//        }
     
//        if (curData.curVertexs.Count > 0)
//        {
//            return true;
//        }
                
//        return false;
//    }
//}