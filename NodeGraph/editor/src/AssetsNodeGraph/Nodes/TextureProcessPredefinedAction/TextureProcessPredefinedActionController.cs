using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    public class TextureProcessPredefinedActionController : NodeController
    {

        public bool PredefinedAction(int actionId, Texture2D t2d, ref List<string> ResultInfoList)
        {
            switch (actionId)
            {
                case 1:
                    return false;
                case 2:
                    return false;
                default:
                    return true;
            }
        }

    }
}
