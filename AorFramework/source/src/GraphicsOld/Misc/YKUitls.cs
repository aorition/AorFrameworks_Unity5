using System;
using System.Collections.Generic;
using UnityEngine;
using YoukiaCore;
using YoukiaUnity.Graphics;
using YoukiaUnity.Graphics.FastShadowProjector;

namespace YoukiaUnity.Misc
{
    /// <summary>
    /// YoukiaUnity的工具类
    /// </summary>
    public static class YKUitls
    {
        /// <summary>
        /// Vector3的注入方法,Vector3f转换为Vector3
        /// </summary>
        /// <param name="src">Vector3f源</param>
        /// <returns>一个新的Vector3变量</returns>
        public static Vector3 ToVector3U(this YKVector3f src)
        {
            return new Vector3(src.x, src.y, src.z);
        }

        public static Vector3 ToVector3U(this YKVector3d src)
        {
            return new Vector3((float) src.x, (float) src.y, (float) src.z);
        }

        public static Vector3 ToVector3U(this YKVector2f src, float y = 0)
        {
            return new Vector3(src.x, y, src.y);
        }

        public static Vector3 ToVector3U(this YKVector2d src, double y = 0)
        {
            return new Vector3((float) src.x, (float) y, (float) src.y);
        }

        public static YKVector2f ToVector2f(this Vector3 src)
        {
            return new YKVector2f(src.x, src.z);
        }

        public static YKVector2d ToVector2d(this Vector3 src)
        {
            return new YKVector2d(src.x, src.z);
        }

        public static YKVector3f ToVector3f(this Vector3 src)
        {
            return new YKVector3f(src.x, src.y, src.z);
        }

        public static YKVector3d ToVector3d(this Vector3 src)
        {
            return new YKVector3d(src.x, src.y, src.z);
        }

        public static Vector2 ToVector2U(this YKVector2f src)
        {
            return new Vector2(src.x, src.y);
        }

        public static Vector2 ToVector2U(this YKVector2d src)
        {
            return new Vector2((float) src.x, (float) src.y);
        }

        public static Vector2 ToVector2(this Vector3 src)
        {
            return new Vector2(src.x, src.z);
        }

        public static Vector3 ToVector3(this Vector2 src, double y = 0)
        {
            return new Vector3(src.x, (float) y, src.y);
        }

        public static Matrix4x4 GetViewMatrix(this Transform t)
        {
            Vector3 direction;
            Vector3 up;
            Vector3 left;
            Vector3 location = t.position;

            Matrix4x4 m = Matrix4x4.identity;

            up = t.up;
            direction = t.forward;
            left = Vector3.Cross(up, direction);

            m.m00 = left.x;
            m.m01 = left.y;
            m.m02 = left.z;
            m.m10 = up.x;
            m.m11 = up.y;
            m.m12 = up.z;
            m.m20 = -direction.x;
            m.m21 = -direction.y;
            m.m22 = -direction.z;

            m.m03 = -(m.m00 * location.x + m.m01 * location.y + m.m02 * location.z);
            m.m13 = -(m.m10 * location.x + m.m11 * location.y + m.m12 * location.z);
            m.m23 = -(m.m20 * location.x + m.m21 * location.y + m.m22 * location.z);

            return m;
        }

        /// <summary>
        /// Vector3的注入方法，可控误差比较
        /// </summary>
        /// <param name="src">Vector3源</param>
        /// <param name="dst">Vector3源</param>
        /// <param name="epsilon">公差范围</param>
        /// <returns>是否在误差内</returns>
        public static bool EquelEpsilon(this Vector3 src, Vector3 dst, float epsilon = float.Epsilon)
        {
            return FloatEquel(src.x, dst.x, epsilon) && FloatEquel(src.y, dst.y, epsilon) && FloatEquel(src.z, dst.z, epsilon);
        }

        /// <summary>
        /// 可控的误差比较
        /// </summary>
        /// <param name="f1">浮点源1</param>
        /// <param name="f2">浮点源2</param>
        /// <param name="epsilon">公差范围</param>
        /// <returns>是否在误差内</returns>
        public static bool FloatEquel(float f1, float f2, float epsilon = float.Epsilon)
        {
            return Mathf.Abs(f1 - f2) < epsilon;
        }

        public static AnimationState GetCurrentState(this Animation anim)
        {
            //TODO:注意,多通道播放时(比如使用CrossFade)可能同时有多个动画在不同通道上播放
            if (anim == null)
            {
                return null;
            }
            foreach (AnimationState state in anim)
            {
                if (anim.IsPlaying(state.name))
                {
                    return state;
                }
            }
            return null;
        }

        /// <summary>
        /// 渐变物体Transform(局部空间)
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static IEnumerator<object> TransformFadeTo(Transform src, Transform dst, float speed = 1, Action callBack = null)
        {
            if (src == null || dst == null)
            {
                if (callBack != null)
                    callBack();
            }
            else
            {
                while ((dst != null && src != null) && (!dst.localPosition.EquelEpsilon(src.localPosition, 0.01f) || !dst.localEulerAngles.EquelEpsilon(src.localEulerAngles, 0.01f)))
                {
                    src.localPosition = Vector3.Lerp(src.localPosition, dst.localPosition, Time.deltaTime * speed);
                    src.localRotation = Quaternion.Slerp(src.localRotation, dst.localRotation, Time.deltaTime * speed);
                    src.localScale = Vector3.Lerp(src.localScale, dst.localScale, Time.deltaTime * speed);
                    yield return 1;
                }
                if (src != null && dst != null)
                {
                    src.localPosition = dst.localPosition;
                    src.localEulerAngles = dst.eulerAngles;
                }
            }
            if (callBack != null)
                callBack();
        }

        public static bool CheckCollision(this Camera cam, Vector3 point)
        {
            Vector3 vPos = (cam.projectionMatrix*cam.worldToCameraMatrix).MultiplyPoint(point);

            return Math.Max(Math.Abs(vPos.x), Math.Abs(vPos.y)) < 1 && vPos.z < 1 && vPos.z > 0;
        }

        public static float CheckCollisionDis(this Camera cam, Vector3 point)
        {
            Vector3 vPos = (cam.projectionMatrix*cam.worldToCameraMatrix).MultiplyPoint(point);

            return Math.Max(Math.Abs(vPos.x), Math.Abs(vPos.y));
        }

        public static Vector3 WorldToScreenAdapt(this Camera cam, Vector3 worldPos)
        {
            Vector3 r = (GraphicsManager.GetInstance().ProjMatAdapt*cam.worldToCameraMatrix).MultiplyPoint(worldPos);
            r.x = (r.x + 1)/2f*Screen.width;
            r.y = (r.y + 1)/2f*Screen.height;

            return r;
        }
        /// <summary>
        ///  获取当前相机视锥体的世界空间点位坐标
        /// </summary>
        /// <param name="cam">当前相机</param>
        /// <param name="zFar">可重写的远裁剪面的距离 默认使用cam.farClipPlane</param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Vector3[] GetFrustumPoints(Camera cam, float zFar, Vector3[] buffer = null)
        {
            
            if (cam == null)
                return buffer;
            Vector3[] result = buffer ?? new Vector3[8];
            uint i;
            Matrix4x4 viewProjMatrixInverse;
            float a, b, projZFar;
            viewProjMatrixInverse = cam.projectionMatrix * cam.worldToCameraMatrix;
            viewProjMatrixInverse = viewProjMatrixInverse.inverse;
            a = -(cam.farClipPlane + cam.nearClipPlane) / (cam.farClipPlane - cam.nearClipPlane);
            b = -(2 * cam.farClipPlane * cam.nearClipPlane) / (cam.farClipPlane - cam.nearClipPlane);
            projZFar = -(a * zFar - b) / zFar;
            result[0].Set(-1, -1, -1);
            result[1].Set(1, -1, -1);
            result[2].Set(1, 1, -1);
            result[3].Set(-1, 1, -1);
            result[4].Set(-1, -1, projZFar);
            result[5].Set(1, -1, projZFar);
            result[6].Set(1, 1, projZFar);
            result[7].Set(-1, 1, projZFar);
            for (i = 0; i < 8; i++)
            {
                result[i] = viewProjMatrixInverse.MultiplyPoint(result[i]);
            }

            return result;
        }

        public static void SetVisible(this GameObject gameObject, bool b)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (!(renderers[i] is ParticleSystemRenderer))
                {
                    renderers[i].enabled = b;
                }
            }
            ModelShadowProjector[] projectors = gameObject.GetComponentsInChildren<ModelShadowProjector>();
            for (int i = 0; i < projectors.Length; i++)
            {
                projectors[i].enabled = b;
            }
        }

        public static void SetUIVisible(this GameObject gameObject, bool b, bool includeChildren = true)
        {
                                    if (b != gameObject.activeInHierarchy)
                                    {
                                        if (b)
                                        {
                        //                gameObject.transform.localScale = Vector3.one;
                                            gameObject.SetActive(true);
                        
                                        }
                                        else
                                        {
                        //                gameObject.transform.localScale = Vector3.zero;
                                            gameObject.SetActive(false);
                                        }
                                    }
//            if (b && !gameObject.activeInHierarchy)
//            {
//                gameObject.SetActive(true);
//            }
//                UIWidget[] uiWidgets = includeChildren ? gameObject.GetComponentsInChildren<UIWidget>() : gameObject.GetComponents<UIWidget>();
//                if (uiWidgets != null)
//                {
//                    for (int i = 0; i < uiWidgets.Length; i++)
//                    {
//                        uiWidgets[i].enabled = b;
////                        uiWidgets[i].finalAlpha = 0;
//                    }
//                }
        }
    }
}