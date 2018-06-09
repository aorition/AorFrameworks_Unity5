using System;
using System.Collections.Generic;
using AorBaseUtility;
using UnityEngine;

namespace Framework.Utility
{
    /// <summary>
    /// 封装一些关于相机常用的方法
    /// </summary>
    public class CameraMathUtility
    {

        /// <summary>
        /// 根据相机距离获取边框Size数据(half)
        /// </summary>
        public static Vector2 GetCornerHalfSize(Camera cam, float distance)
        {
            return GetCornerHalfSize(cam.fieldOfView, cam.aspect, distance);
        }

        /// <summary>
        /// 根据相机距离获取边框Size数据(half)
        /// </summary>
        public static Vector2 GetCornerHalfSize(float Vfov, float aspect, float distance)
        {
            float halfFOV = (Vfov * 0.5f) * Mathf.Deg2Rad;

            float height = distance * Mathf.Tan(halfFOV);
            float width = height * aspect;

            return new Vector2(width, height);
        }

        /// <summary>
        /// 根据相机距离获取边框点位数据
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3[] GetVisualPlane(Camera cam, float distance)
        {
            Vector3[] corners = new Vector3[4];
            
            if (cam.orthographic)
            {

                float size = cam.orthographicSize;

                // UpperLeft
                corners[0] = cam.transform.position - cam.transform.right * size * cam.aspect;
                corners[0] += cam.transform.up * size;
                corners[0] += cam.transform.forward * distance;

                // UpperRight
                corners[1] = cam.transform.position + cam.transform.right * size * cam.aspect;
                corners[1] += cam.transform.up * size;
                corners[1] += cam.transform.forward * distance;

                // LowerLeft
                corners[2] = cam.transform.position - cam.transform.right * size * cam.aspect;
                corners[2] -= cam.transform.up * size;
                corners[2] += cam.transform.forward * distance;

                // LowerRight
                corners[3] = cam.transform.position + cam.transform.right * size * cam.aspect;
                corners[3] -= cam.transform.up * size;
                corners[3] += cam.transform.forward * distance;

            }
            else
            {

                Vector2 fsize = GetCornerHalfSize(cam, distance);

                // UpperLeft
                corners[0] = cam.transform.position - (cam.transform.right * fsize.x);
                corners[0] += cam.transform.up * fsize.y;
                corners[0] += cam.transform.forward * distance;

                // UpperRight
                corners[1] = cam.transform.position + (cam.transform.right * fsize.x);
                corners[1] += cam.transform.up * fsize.y;
                corners[1] += cam.transform.forward * distance;

                // LowerLeft
                corners[2] = cam.transform.position - (cam.transform.right * fsize.x);
                corners[2] -= cam.transform.up * fsize.y;
                corners[2] += cam.transform.forward * distance;

                // LowerRight
                corners[3] = cam.transform.position + (cam.transform.right * fsize.x);
                corners[3] -= cam.transform.up * fsize.y;
                corners[3] += cam.transform.forward * distance;
            }
            return corners;
        }

        /// <summary>
        /// H Fov值转V Fov值.(Unity相机默认使用V Fov 值, 而3Ds Max则默认使用H fov值)
        /// </summary>
        public static float HFov2VFov(Camera _target, float value)
        {
            return 180 / Mathf.PI * (2 * Mathf.Atan(Mathf.Tan(Mathf.PI / 180 * value / 2) * _target.aspect));
        }

        /// <summary>
        /// 计算H Fov 
        /// </summary>
        /// <param name="w">视平面半宽度</param>
        /// <param name="distance"></param>
        public float CalculateHFOV(float w, float distance)
        {
            return (Mathf.Atan(w / distance)) * Mathf.Rad2Deg * 2f;
        }

        /// <summary>
        /// 计算V Fov 
        /// </summary>
        /// <param name="w">视平面半宽度</param>
        /// <param name="distance"></param>
        public float CalculateVFOV(float w, float h, float distance)
        {
            return (Mathf.Atan(h / distance)) * Mathf.Rad2Deg * 2f;
        }

        /// <summary>
        /// 为相机设置投影矩阵
        /// </summary>
        public static void SetFrustum(Camera cam, float left, float right, float bottom, float top, float near, float far, bool isOrtho)
        {
            Matrix4x4 projMat = CreateProjectionMatrix(left, right, bottom, top, near, far, isOrtho);
            cam.projectionMatrix = projMat;
        }

        /// <summary>
        /// 创建投影矩阵
        /// </summary>
        public static void SetFrustum(Camera cam, float VFov, float aspect, float near, float far, bool isOrtho)
        {
            Matrix4x4 projMat = CreateProjectionMatrix(VFov, aspect, near, far, isOrtho);
            cam.projectionMatrix = projMat;
        }

        /// <summary>
        /// 创建投影矩阵
        /// </summary>
        public static Matrix4x4 CreateProjectionMatrix(float VFov, float aspect, float near, float far,  bool isOrtho)
        {
            float tangent = Mathf.Tan(VFov/2*Mathf.Deg2Rad);
            float height = near*tangent;
            float width = height*aspect;
            return CreateProjectionMatrix(-width, width, -height, height, near, far, isOrtho);
        }
        /// <summary>
        /// 创建投影矩阵
        /// </summary>
        public static Matrix4x4 CreateProjectionMatrix(float left, float right, float bottom, float top, float near, float far, bool isOrtho)
        {
            Matrix4x4 projMat = new Matrix4x4();
            if (isOrtho)
            {
                float dx = 1.0f / (right - left);
                float dy = 1.0f / (top - bottom);
                float dz = 1.0f / (near - far);
                
                projMat.m00 = 2 * dx;
                projMat.m11 = 2 * dy;
                projMat.m22 = 2 * dz;
                projMat.m03 = -dx * (left + right);
                projMat.m13 = -dy * (bottom + top);
                projMat.m23 = dz * (near + far);
                projMat.m33 = 1.0f;
            }
            else
            {
                float dx = 1.0f / (right - left);
                float dy = 1.0f / (top - bottom);
                float dz = 1.0f / (near - far);

                projMat.m00 = 2 * near * dx;
                projMat.m11 = 2 * near * dy;

                projMat.m02 = dx * (right + left);
                projMat.m12 = dy * (bottom + top);
                projMat.m22 = dz * (far + near);

                projMat.m32 = -1;
                projMat.m23 = 2 * dz * near * far;
            }

            return projMat;
        }

        /// <summary>
        /// 插值 Fov 
        /// </summary>
        /// <returns></returns>
        public static float InterpolateFOV(float fovA, float fovB, float dA, float dB, float t)
        {
            // We interpolate shot height
            float hA = dA * 2f * Mathf.Tan(fovA * Mathf.Deg2Rad / 2f);
            float hB = dB * 2f * Mathf.Tan(fovB * Mathf.Deg2Rad / 2f);
            float h = Mathf.Lerp(hA, hB, t);
            float fov = 179f;
            float d = Mathf.Lerp(dA, dB, t);
            if (d > FloatExtends.Epsilon)
                fov = 2f * Mathf.Atan(h / (2 * d)) * Mathf.Rad2Deg;
            return Mathf.Clamp(fov, Mathf.Min(fovA, fovB), Mathf.Max(fovA, fovB));
        }


    }
}
