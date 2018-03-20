using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace YoukiaUnity.Graphics
{
    public class BakedValuesRecorder : MonoBehaviour, IEditorOnlyScript
    {

        public void Awake()
        {
            if (Application.isPlaying)
            {
                GameObject.Destroy(this);
            }
        }

        public Color BakedAmbientColor;
        public Color BakedFogColor;

    }
}
