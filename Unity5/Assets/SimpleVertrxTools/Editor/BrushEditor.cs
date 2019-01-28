using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{

    public class BrushEditor : BrushEditorBase
    {
        /// <summary>
        /// 笔刷绘制处理方法, 笔刷在探测可绘制时调用此函数
        /// </summary>
        protected sealed override void OnPainting(Event e, RaycastHit raycastHit)
        {
            //默认实现 鼠标左键按下(拖动) 和 鼠标弹起
            if (e.type == EventType.mouseDown)
            {
                if (e.button == 0)
                {
                    //mouseDown
                    OnPaintingMouseDown(raycastHit, e.control || e.command, e.alt, e.shift);
                }
                else if (e.button == 1)
                {
                    //mouseDown Right
                    OnPaintingMouseDownRight(raycastHit, e.control || e.command, e.alt, e.shift);
                }else if (e.button == 2)
                {
                    OnPaintingMouseDownMiddle(raycastHit, e.control || e.command, e.alt, e.shift);
                }
            }
            else if (e.type == EventType.mouseDrag)
            {
                //mouseDrag
                if (e.button == 0)
                {
                    //mouseDown
                    OnPaintingMouseDrag(raycastHit, e.control || e.command, e.alt, e.shift);
                }
                 else if (e.button == 1)
                {
                    //mouseDown Right
                    OnPaintingMouseDownRight(raycastHit, e.control || e.command, e.alt, e.shift);
                }
                else if (e.button == 2)
                {
                    OnPaintingMouseDownMiddle(raycastHit, e.control || e.command, e.alt, e.shift);
                }
            }
            else if (e.type == EventType.mouseUp && e.button == 0)
            {
                //mouseUp
                if (e.button == 0)
                {
                    //MouseUp
                    OnPaintingMouseUp(raycastHit, e.control || e.command, e.alt, e.shift);
                }
                else if (e.button == 1)
                {
                    //MouseUp Right
                    OnPaintingMouseRightUp(raycastHit, e.control || e.command, e.alt, e.shift);
                }else if (e.button == 2)
                {
                    OnPaintingMouseRightMiddle(raycastHit, e.control || e.command, e.alt, e.shift);
                }
            }
        }

        #region Event Drag

        protected virtual void OnPaintingMouseDrag(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
        }

        protected virtual void OnPaintingMouseDragRight(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
        }

        protected virtual void OnPaintingMouseDragMiddle(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
        }

        #endregion

        #region Event Down

        protected virtual void OnPaintingMouseDown(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
        }

        protected virtual void OnPaintingMouseDownRight(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
        }

        protected virtual void OnPaintingMouseDownMiddle(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
        }

        #endregion

        #region Event mouseUp

        protected virtual void OnPaintingMouseUp(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
            //mouseUp
        }

        protected virtual void OnPaintingMouseRightUp(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
            //mouseUp
        }

        protected virtual void OnPaintingMouseRightMiddle(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
            //mouseUp
        }

        #endregion


    }

}


