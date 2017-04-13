using System;
using System.Collections.Generic;
using AorBaseUtility;
using UnityEngine;

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
            return new Vector3((float)src.x, (float)src.y, (float)src.z);
        }
        public static Vector3 ToVector3U(this YKVector2f src, float y = 0)
        {
            return new Vector3(src.x, y, src.y);
        }

        public static Vector3 ToVector3U(this YKVector2d src, double y = 0)
        {
            return new Vector3((float)src.x, (float)y, (float)src.y);
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
            return new Vector2((float)src.x, (float)src.y);
        }

        public static Vector2 ToVector2(this Vector3 src)
        {
            return new Vector2(src.x, src.y);
        }

        public static Vector3 ToVector3(this Vector2 src, double y = 0)
        {
            return new Vector3(src.x, (float)y, src.y);
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
            while (!dst.localPosition.EquelEpsilon(src.localPosition, 0.01f) || !dst.localEulerAngles.EquelEpsilon(src.localEulerAngles, 0.01f))
            {
                src.localPosition = Vector3.Lerp(src.localPosition, dst.localPosition, Time.deltaTime * speed);

                src.localRotation = Quaternion.Slerp(src.localRotation, dst.localRotation, Time.deltaTime * speed);

                src.localScale = Vector3.Lerp(src.localScale, dst.localScale, Time.deltaTime * speed);

                yield return 1;
            }

            src.localPosition = dst.localPosition;
            src.localEulerAngles = dst.eulerAngles;
            if (callBack != null) callBack();
        }
    }
}
