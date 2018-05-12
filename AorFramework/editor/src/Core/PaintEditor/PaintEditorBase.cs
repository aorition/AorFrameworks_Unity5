using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.editor
{

    public class PaintEditorBase : UnityEditor.Editor
    {

        #region 笔刷属性

        public bool BrushRelative
        {
            get { return EditPaintBrush.BrushRelative; }
            set { EditPaintBrush.BrushRelative = value; }
        }

        public EditPaintBrush.PaintHandle PaintPrev
        {
            get { return EditPaintBrush.PaintPrev; }
            set { EditPaintBrush.PaintPrev = value; }
        }

        public float MaskTexUVCoord
        {
            get { return EditPaintBrush.MaskTexUVCoord; }
            set { EditPaintBrush.MaskTexUVCoord = value; }
        }

        public Transform CurrentSelect
        {
            get { return EditPaintBrush.CurrentSelect; }
            set { EditPaintBrush.CurrentSelect = value; }
        }

        public int BrushSize
        {
            get { return EditPaintBrush.BrushSize; }
            set { EditPaintBrush.BrushSize = value; }
        }

        public int LayerMask
        {
            get { return EditPaintBrush.LayerMask; }
            set { EditPaintBrush.LayerMask = value; }
        }

        public Projector BrushPrev => EditPaintBrush.BrushPrev;


        public void BrushPrevDispose()
        {
            EditPaintBrush.BrushPrevDispose();
        }

        #endregion

        //---------------------------------------------------

        /// <summary>
        /// 销毁时亦销毁笔刷对象
        /// </summary>
        protected virtual void OnDestroy()
        {
            BrushPrevDispose();
        }

        /// <summary>
        /// 绘制开关
        /// </summary>
        public bool PaintingEnable = true;

        /// <summary>
        /// 在OnSceneGUI方法之前调用, 可以在此判定是否需要进行笔刷绘制
        /// </summary>
        protected virtual void OnPrePainting()
        {
            //
        }

        /// <summary>
        /// 笔刷绘制处理方法, 笔刷在探测可绘制时调用此函数
        /// </summary>
        protected virtual void OnPainting(Event e, RaycastHit raycastHit)
        {
            //复写此方法实现更复杂的事件逻辑处理 ...
        }

        /// <summary>
        /// 在笔刷状态为EditPaintBrush.PaintHandle.Classic时调用该方法,子类覆盖此方法用于实现brushPrev如何显示
        /// </summary>
        protected virtual void OnBrushPreview()
        {
            
        }

        protected  virtual void OnSceneGUI()
        {

            //处理依赖组件
            _checkComponentDependent();

            OnPrePainting();

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

            if (    (e.type == EventType.KeyDown && e.keyCode == KeyCode.KeypadPlus)
                ||  (e.type == EventType.KeyDown && e.keyCode == KeyCode.Equals && e.shift)
            )
            {
                BrushSize += 1;
            }
            else if (   (e.type == EventType.KeyDown && e.keyCode == KeyCode.KeypadMinus)
                 ||     (e.type == EventType.KeyDown && e.keyCode == KeyCode.Minus && e.shift)    
            ){
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

                    if (PaintPrev != EditPaintBrush.PaintHandle.Classic &&
                        PaintPrev != EditPaintBrush.PaintHandle.Hide_preview &&
                        PaintPrev != EditPaintBrush.PaintHandle.Follow_Normal_WireCircle)
                    {
                        Handles.color = new Color(1f, 1f, 0f, 0.05f);
                        Handles.DrawSolidDisc(raycastHit.point, raycastHit.normal,
                            BrushPrev.orthographicSize * 0.9f);
                    }
                    else if (PaintPrev != EditPaintBrush.PaintHandle.Classic &&
                             PaintPrev != EditPaintBrush.PaintHandle.Hide_preview &&
                             PaintPrev != EditPaintBrush.PaintHandle.Follow_Normal_Circle)
                    {
                        Handles.color = new Color(1f, 1f, 0f, 1f);
                        Handles.DrawWireDisc(raycastHit.point, raycastHit.normal,
                            BrushPrev.orthographicSize*0.9f);
                    }

                    if (PaintPrev == EditPaintBrush.PaintHandle.Hide_preview)
                    {
                        BrushPrev.enabled = false;
                    }
                    else
                    {
                        BrushPrev.enabled = true;
                        OnBrushPreview();
                    }

                    OnPainting(e, raycastHit);
                }
            }

        }

        protected Collider _collider;

        /// <summary>
        /// 检查依赖组件
        /// </summary>
        protected virtual void _checkComponentDependent()
        {
            if (!_collider)
            {
                _collider = (target as MonoBehaviour).GetComponent<Collider>();
                if (!_collider)
                {
                    _collider = (target as MonoBehaviour).gameObject.AddComponent<MeshCollider>();

                    SkinnedMeshRenderer skinned = CurrentSelect.GetComponent<SkinnedMeshRenderer>();
                    if (skinned) (_collider as MeshCollider).sharedMesh = skinned.sharedMesh;
                }
            }
        }

    }

}


