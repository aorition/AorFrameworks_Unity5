using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Extends
{

    public static class VectorExtends
    {

        public static string ToNString(this Vector2 v)
        {
            return "Vector2[" + v.x + "," + v.y + "]";
        }

        public static string ToNString(this Vector3 v)
        {
            return "Vector3[" + v.x + "," + v.y + "," + v.z + "]";
        }

        public static string ToNString(this Vector4 v)
        {
            return "Vector4[" + v.x + "," + v.y + "," + v.z + "," + v.w + "]";
        }

        public static string ToShortString(this Vector2 v, string delimiter = ",")
        {
            return v.x + delimiter + v.y;
        }

        public static string ToShortString(this Vector3 v, string delimiter = ",")
        {
            return v.x + delimiter + v.y + delimiter + v.z;
        }

        public static string ToShortString(this Vector4 v, string delimiter = ",")
        {
            return v.x + delimiter + v.y + delimiter + v.z + delimiter + v.w;
        }

    }

}


