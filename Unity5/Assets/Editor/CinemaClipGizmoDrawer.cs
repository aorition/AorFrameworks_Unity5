using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using YoukiaUnity.CinemaSystem;

/// <summary>
/// 测试用代码
/// </summary>
public class CinemaClipGizmoDrawer
{
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    //    [DrawGizmo(GizmoType.SelectedOrChild)]
    static void DrawCinemaGizmo(Transform transform, GizmoType gizmoType)
    {
        Handles.Label(transform.position, transform.gameObject.name);
    }
}
