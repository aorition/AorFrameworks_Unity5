using System;
using System.Collections.Generic;

using UnityEngine;
using YoukiaCore;

namespace YoukiaUnity.Graphics
{
    public class RoleFaceHandler : MonoBehaviour
    {

        public Transform RoleBodyTransform;

        public static string FaceShaderNameDefined = "Custom/Light/Diffuse - Toon - Normal -Noline";

        private bool _HasChangeToFaceShader = false;

        private void OnEnable()
        {

            if (_HasChangeToFaceShader || !RoleBodyTransform) return;

            Renderer renderer = GetComponent<Renderer>();
            if (renderer)
            {
                if (renderer.material)
                {
                    if (renderer.material.shader.name != FaceShaderNameDefined)
                    {
                        Shader sd = Shader.Find(FaceShaderNameDefined);
                        if (sd != null)
                        {
                            renderer.material.shader = sd;

                            Renderer rootRenderer = RoleBodyTransform.GetComponent<Renderer>();
                            if (rootRenderer)
                            {

                                renderer.material.renderQueue = rootRenderer.material.renderQueue + 5;
        
                            }


                        }
                    }
                }
                else
                {
                    Log.error("RoleFaceHandler.OnEnable Error : renderer.material == null");
                }
            }
            else
            {
                Log.error("RoleFaceHandler.OnEnable Error : can not find Renderer in " + transform.getHierarchyPath());
            }
            _HasChangeToFaceShader = true;
        }

    }
}
