using System;
using System.Collections.Generic;
using Assets.AorTile3D.Scripts.runtimeEditor;
using UnityEngine;

namespace AorFramework.AorTile3D.runtime
{

    public class AorTile3DUtils
    {

        #region Int3方法集

        public static int[] Vector3ToInt3(Vector3 src)
        {
            return new[] {(int) src.x, (int) src.y, (int) src.z};
        }

        public static Vector3 Int3ToVector3(int[] src)
        {
            return new Vector3(src[0], src[1], src[2]);
        }

        public static void Int3Set(Vector3 src, int[] target)
        {
            target[0] = (int) src.x;
            target[1] = (int) src.y;
            target[2] = (int) src.z;
        }

        public static void Int3Set(int[] src, int[] target)
        {
            target[0] = src[0];
            target[1] = src[1];
            target[2] = src[2];
        }

        public static bool Int3Equals(int[] a, int[] b)
        {
            return a[0] == b[0] && a[1] == b[1] && a[2] == b[2];
        }

        #endregion
        
        public static Color GetDefineBGColor()
        {
            return new Color(
                    AorTile3dEditorUIDefines.UIBGColor[0],
                    AorTile3dEditorUIDefines.UIBGColor[1],
                    AorTile3dEditorUIDefines.UIBGColor[2],
                    AorTile3dEditorUIDefines.UIBGColor[3]
                   );
        }

    }
}
