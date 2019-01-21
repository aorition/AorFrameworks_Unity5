namespace com.emadkhezri.vertextool
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using System.Collections.Generic;

    public class VertexToolData
    {
        public static string NO_OBJECT_SELECTED = "No Object Selected";
        public string selectedObjectName = NO_OBJECT_SELECTED;
        public bool isVertexSelected = false;
        public int vertexCount = 0;
        public int subMeshCount=0;
        //public IndexFormat IndexFormat;
        //public VertexData VertexData=new VertexData();
        public Color[] preColors = new Color[0];
        public Color[] curColors = new Color[0];
        public List<int> curVertexs = new List<int>();
        public TriangleData TriangleData = new TriangleData();
        public MeshFilter meshFilter;
        public MeshFilter originMeshFilter;
        public bool vertexToolSettingsToggle = false;
        public bool vertexToolInfoToggle = true;
        public Color normalArrowColor = new Color(0.7f, 0.4f, 0.2f, 0.3f);
        public Color solidDiskColor = new Color(0.7f, 0.4f, 0.2f, 0.3f);
        public Color selectedVertexColor = Color.red;
        public float normalArrowSize = 0.5f;
        public float solidDiskSize = 0.2f;
        public float selectedVertexSize = 0.01f;
        public bool isSelectExcludeBack = true;
        public bool isEditVertex = false;
    }

    //public class VertexData
    //{
    //    public int Index;
    //    //    public Vector3 Position;
    //    //    public Vector3 Normal;
    //    //    public Vector3 Tangent;
    //    public Color Color;
    //    //    public Vector2 UV;
    //    //    public Vector2 UV2;
    //    //    public Vector2 UV3;
    //    //    public Vector2 UV4;
    //}

    public class TriangleData
    {
        public int ID = -100;
        public int Vertex1;
        public int Vertex2;
        public int Vertex3;
    }
}