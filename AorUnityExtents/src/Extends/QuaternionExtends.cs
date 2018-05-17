using System;
using UnityEngine;

namespace Framework
{

    public static class QuaternionExtends
    {

        /// <summary>Normalize a quaternion</summary>
        /// <param name="q"></param>
        /// <returns>The normalized quaternion.  Unit length is 1.</returns>
        public static Quaternion Normalized(this Quaternion q)
        {
            Vector4 v = new Vector4(q.x, q.y, q.z, q.w).normalized;
            return new Quaternion(v.x, v.y, v.z, v.w);
        }

    }

}


