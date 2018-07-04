﻿using System;
using System.Collections.Generic;
using AorBaseUtility;
using UnityEngine;

namespace Framework.NodeGraph
{

    public class HierarchyObjTransformData : NodeData
    {
        
        public HierarchyObjTransformData() {}

        public HierarchyObjTransformData(long id) : base(id) {}
        
        public readonly bool UseEditorSelection = false;

        public readonly int[] InstancesPath;

        public readonly string[] ResultInfo;

        public readonly string TransformType = "Relative";
        public readonly Vector3 Transform = Vector3.zero;
        public readonly Vector3 TransformOffset = Vector3.zero;
        public readonly Vector3 TransformRate = Vector3.one;

        public readonly float CircleDistance = 0f;
        public readonly float CircleDistanceRate = 1f;
        public readonly float CircleAngleStart = 0f;
        public readonly float CircleAngle = 0f;
        public readonly float CircleAngleRate = 1f;
        public readonly string HOCircleType = "Y";

        public readonly string RotationType = "Relative";
        public readonly Vector3 Rotation = Vector3.zero;
        public readonly Vector3 RotationOffset = Vector3.zero;
        public readonly Vector3 RotationRate = Vector3.one;

        public readonly string ScaleType = "Relative";
        public readonly Vector3 Scale = Vector3.one;
        public readonly Vector3 ScaleOffset = Vector3.zero;
        public readonly Vector3 ScaleRate = Vector3.one;
        
    }
}
