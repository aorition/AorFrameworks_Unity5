using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic
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
