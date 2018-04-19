using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Tools
{

    [ExecuteInEditMode]
    public class MeshChecker : MonoBehaviour, IEditorOnlyScript
    {


        private Shader _ckShader;

        public int ViewType = 0;

        public Texture2D mainTexture;
        public Material[] orginalMaterials;
        public Material[] checkMaterials;
        public Material cMaterial;

        private Renderer _renderer;

        private void OnDisable()
        {
            if (_renderer && cMaterial)
            {
                if (Application.isPlaying)
                {
                    _renderer.materials = orginalMaterials;
                }
                else
                {
                    _renderer.sharedMaterials = orginalMaterials;
                }

                _renderer = null;
                cMaterial = null;
                orginalMaterials = null;
                checkMaterials = null;
            }
        }

        private void Update()
        {


            if (!cMaterial)
            {

                if (!_ckShader) _ckShader = Shader.Find("FrameworkTools/ModelChecker");

                if (!_ckShader) return;

                Renderer renderer = GetComponent<Renderer>();
                if (renderer && (renderer is MeshRenderer || renderer is SkinnedMeshRenderer))
                {
                    cMaterial = new Material(_ckShader);

                    orginalMaterials = Application.isPlaying ? renderer.materials : renderer.sharedMaterials;
                    int i, len = orginalMaterials.Length;
                    checkMaterials = new Material[len];
                    for (i = 0; i < len; i++)
                    {
                        checkMaterials[i] = cMaterial;
                    }

                    if (Application.isPlaying)
                    {
                        renderer.materials = checkMaterials;
                    }
                    else
                    {
                        renderer.sharedMaterials = checkMaterials;
                    }

                    _renderer = renderer;
                }

            }
        }

        public void SetViewType(int viewType)
        {
            if (cMaterial)
                cMaterial.SetFloat("_checkType", viewType);
        }

        public void SetViewType(int viewType, Texture2D t2d)
        {
            if (cMaterial)
            {
                cMaterial.SetFloat("_checkType", viewType);
                cMaterial.SetTexture("_MainTex", t2d);
            }   
        }

    }
}


