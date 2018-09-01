using System;
using System.Collections.Generic;
using Framework.Extends;
using UnityEngine;

namespace Framework.Graphic.CustomLight
{
    [ExecuteInEditMode]
    public class CustomLightSupports : MonoBehaviour
    {

        public static GameObject Main;

        public bool UseFixedUpdate = false;

        public bool isEdtorMode = false;

        private void FixedUpdate()
        {
            if (UseFixedUpdate) Processing();
        }

        private void Update()
        {
            if (!UseFixedUpdate) Processing();
        }

        private void Processing()
        {
            if (Main != gameObject)
            {
                gameObject.Dispose();
                return;
            }
            if (CustomLightManager.isExist) CustomLightManager.Instance.Update();
        }

    }

}
