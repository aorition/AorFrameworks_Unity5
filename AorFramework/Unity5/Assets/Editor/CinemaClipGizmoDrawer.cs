using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using YoukiaUnity.CinemaSystem;

public class CinemaClipGizmoDrawer
{
    [DrawGizmo(GizmoType.SelectedOrChild | GizmoType.NotSelected)]
    //    [DrawGizmo(GizmoType.SelectedOrChild)]
    static void DrawCinemaGizmo(Transform transform, GizmoType gizmoType)
    {
        Handles.Label(transform.position, transform.gameObject.name);
    }
}
