using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.editor
{
    public class PaintEditor : UnityEditor.Editor
    {

        #region 静态笔刷类

        public static class EditPainter
        {

            public enum PaintHandle
            {
                Classic,
                Follow_Normal_Circle,
                Follow_Normal_WireCircle,
                Hide_preview
            }

            public static PaintHandle PaintPrev = PaintHandle.Follow_Normal_Circle;
            public static float MaskTexUVCoord = 1f;
            public static Transform CurrentSelect;
            public static int BrushSize = 10;
            public static int LayerMask = -1;
            //----
            private static Projector _brushPrev;

            public static Projector BrushPrev
            {
                get
                {
                    if (!_brushPrev)
                    {
                        if (!CurrentSelect) return null;
                        var ProjectorB = GameObject.Find("AorTerrainProEditPainter");
                        if (!ProjectorB) ProjectorB = new GameObject("AorTerrainProEditPainter");
                        _brushPrev = ProjectorB.GetComponent(typeof (Projector)) as Projector;
                        if (!_brushPrev) _brushPrev = ProjectorB.AddComponent(typeof (Projector)) as Projector;
                        // ProjectorB.hideFlags = HideFlags.HideInHierarchy;

                        Vector2 MeshSize = Vector2.zero;
                        Terrain terrain = CurrentSelect.GetComponent<Terrain>();
                        MeshFilter SizeOfGeo = CurrentSelect.GetComponent<MeshFilter>();
                        if (terrain)
                        {
                            MeshSize = new Vector2(terrain.terrainData.heightmapWidth,
                                terrain.terrainData.heightmapHeight);
                        }
                        else if (SizeOfGeo)
                        {
                            MeshSize = new Vector2(SizeOfGeo.sharedMesh.bounds.size.x,
                                SizeOfGeo.sharedMesh.bounds.size.z);
                        }
                        _brushPrev.orthographic = true; //先设置类型，再修改参数，不然会覆盖
                        _brushPrev.nearClipPlane = -20f;
                        _brushPrev.farClipPlane = 20f;
                        _brushPrev.orthographicSize = (BrushSize*CurrentSelect.localScale.x)*(MeshSize.x/100);
                        _brushPrev.ignoreLayers = ~LayerMask;
                        _brushPrev.transform.Rotate(90, -90, 0);
                    }
                    return _brushPrev;
                }
            }

            public static void BrushPrevDispose()
            {
                if (_brushPrev)
                {
                    GameObject del = _brushPrev.gameObject;
                    _brushPrev = null;

                    del.name = del.name + "(dispose)";
                    del.SetActive(false);
                    EditorPlusMethods.NextEditorApplicationUpdateDo(() =>
                    {
                        if (Application.isPlaying)
                        {
                            Destroy(del);
                        }
                        else
                        {
                            DestroyImmediate(del);
                        }
                    });
                }
            }

        }

        #endregion

        //---------------------------------------------------

        #region 笔刷属性

        public EditPainter.PaintHandle PaintPrev
        {
            get { return EditPainter.PaintPrev; }
            set { EditPainter.PaintPrev = value; }
        }

        public float MaskTexUVCoord
        {
            get { return EditPainter.MaskTexUVCoord; }
            set { EditPainter.MaskTexUVCoord = value; }
        }

        public Transform CurrentSelect
        {
            get { return EditPainter.CurrentSelect; }
            set { EditPainter.CurrentSelect = value; }
        }

        public int BrushSize
        {
            get { return EditPainter.BrushSize; }
            set { EditPainter.BrushSize = value; }
        }

        public int LayerMask
        {
            get { return EditPainter.LayerMask; }
            set { EditPainter.LayerMask = value; }
        }

        public Projector BrushPrev => EditPainter.BrushPrev;


        public void BrushPrevDispose()
        {
            EditPainter.BrushPrevDispose();
        }

        #endregion

        //---------------------------------------------------

        /// <summary>
        /// 绘制开关
        /// </summary>
        public bool PaintingEnable = true;

        /// <summary>
        /// 笔刷绘制处理方法, 笔刷在探测可绘制时调用此函数
        /// </summary>
        protected virtual void Painting(Event e, RaycastHit raycastHit)
        {
            //复写此方法处理绘制数据
        }

        protected  virtual void OnSceneGUI()
        {

            if (!PaintingEnable)
            {
                BrushPrevDispose();
                return;
            }

            //归置当前选择
            CurrentSelect = Selection.activeTransform;

            Event e = Event.current;
            HandleUtility.AddDefaultControl(0);
            RaycastHit raycastHit = new RaycastHit();

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.KeypadPlus)
            {
                BrushSize += 1;
            }
            else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.KeypadMinus)
            {
                BrushSize -= 1;
            }

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, LayerMask))
            {
                CurrentSelect = raycastHit.transform;
                if (CurrentSelect)
                {
                    BrushPrev.transform.localEulerAngles = new Vector3(90, CurrentSelect.localEulerAngles.y, 0);
                    BrushPrev.transform.position = raycastHit.point;

                    if (PaintPrev != EditPainter.PaintHandle.Classic &&
                        PaintPrev != EditPainter.PaintHandle.Hide_preview &&
                        PaintPrev != EditPainter.PaintHandle.Follow_Normal_WireCircle)
                    {
                        Handles.color = new Color(1f, 1f, 0f, 0.05f);
                        Handles.DrawSolidDisc(raycastHit.point, raycastHit.normal,
                            BrushPrev.orthographicSize * 0.9f);
                    }
                    else if (PaintPrev != EditPainter.PaintHandle.Classic &&
                             PaintPrev != EditPainter.PaintHandle.Hide_preview &&
                             PaintPrev != EditPainter.PaintHandle.Follow_Normal_Circle)
                    {
                        Handles.color = new Color(1f, 1f, 0f, 1f);
                        Handles.DrawWireDisc(raycastHit.point, raycastHit.normal,
                            BrushPrev.orthographicSize * 0.9f);
                    }

                }

            }





        }



    }
}


