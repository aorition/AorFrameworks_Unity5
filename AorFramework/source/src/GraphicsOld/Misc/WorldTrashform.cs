using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WorldTrashform : MonoBehaviour
{
    public Vector3 Position;
    public Vector3 RotationEuler;
    public Quaternion Rotation;
    public Vector3 Scale;

    protected void Update()
    {
        Position = transform.position;
        RotationEuler = transform.rotation.eulerAngles;
        Rotation = transform.rotation;
        Scale = transform.lossyScale;
    }
}
