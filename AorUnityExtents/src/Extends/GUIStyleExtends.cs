using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

    public static class GUIStyleExtends
    {

        public static GUIStyle Clone(this GUIStyle obj)
        {
            GUIStyle s = new GUIStyle(obj);
            s.name = obj.name + "_Clone";
            return s;
        }

    }

}

