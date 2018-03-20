using System;
using System.Collections.Generic;
using System.Linq;

namespace AorBaseUtility.Math
{
    public class Math
    {

        /// <summary>
        /// 线性插值
        /// </summary>
        public static double Linear_Interpolate(double a, double b, double x)
        {
            return a*(1 - x) + b*x;
        }

        public static double Linear_Interpolate(float a, float b, float x)
        {
            return a*(1 - x) + b*x;
        }

        /// <summary>
        /// 余弦插值
        /// </summary>
        public static double Cosine_Interpolate(double a, double b, double x)
        {
            double ft = x*3.1415927d;
            double f = (1d - System.Math.Cos(ft))*0.5d;
            return a*(1d - f) + b*f;
        }

        public static float Cosine_Interpolate(float a, float b, float x)
        {
            float ft = x*3.1415927f;
            float f = (1f - (float) System.Math.Cos(ft))*0.5f;
            return a*(1f - f) + b*f;
        }

        /// <summary>
        /// 立方插值
        /// (插值结果介于v1和v2之间)
        /// </summary>
        public static double Cubic_Interpolate(double v0, double v1, double v2, double v3, double x)
        {
            double P = (v3 - v2) - (v0 - v1);
            double Q = (v0 - v1) - P;
            double R = v2 - v0;
            return System.Math.Pow(P*x, 3) + System.Math.Pow(Q*x, 2) + R*x + v1;
        }

        public static float Cubic_Interpolate(float v0, float v1, float v2, float v3, float x)
        {
            float P = (v3 - v2) - (v0 - v1);
            float Q = (v0 - v1) - P;
            float R = v2 - v0;
            return (float) System.Math.Pow(P*x, 3) + (float) System.Math.Pow(Q*x, 2) + R*x + v1;
        }
        /*
        public static double Noise(int x)
        {
            x = (x << 13) ^ x;
            return (1.0 - ((x*(x*x*15731 + 789221) + 1376312589) & 0x7fffffff)/1073741824.0);
        }*/
    }
}