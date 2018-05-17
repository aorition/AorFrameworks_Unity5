using UnityEngine;
using System.Collections.Generic;

namespace Framework.Graphic.FastShadowProjector
{
    //暂时不使用阴影的AlphaCulling,以此公用Material,否则预阴影材质需要和每一个使用透明裁剪的绘制体相对应
    public class ModelShadowProjector : MonoBehaviour, IProjector
    {
        protected Material preShadowMaterial;
        protected List<GameObject> shadowCasters;
        protected List<Renderer> shadowCasterRenderers;

        void Awake()
        {
            preShadowMaterial = new Material(Shader.Find("Fast Shadow Projector/PreModel"));
            shadowCasters = new List<GameObject>();
            shadowCasterRenderers = new List<Renderer>();

            initModelShadowProjector(gameObject, true);
        }

        private void initModelShadowProjector(GameObject obj, bool original = false)
        {
            List<GameObject> stack = new List<GameObject>(obj.transform.childCount);
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                GameObject child = obj.transform.GetChild(i).gameObject;
                if (child.GetComponent<ModelShadowProjector>() == null && child.GetComponent<ShadowProjector>() == null)
                {
                    stack.Add(child);
                }
            }
            for (int i = 0; i < stack.Count; i++)
            {
                initModelShadowProjector(stack[i]);
            }

            if (original)
            {
                return;
            }

            Renderer renderer = obj.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                renderer = obj.GetComponent<SkinnedMeshRenderer>();
            }
            if (renderer != null)
            {
                GameObject modelProjector = GameObject.Instantiate(obj) as GameObject;
                modelProjector.name = obj.name + "_shadow";
                modelProjector.transform.parent = obj.transform.parent;
                modelProjector.transform.localPosition = obj.transform.localPosition;
                modelProjector.transform.localRotation = obj.transform.localRotation;
                modelProjector.transform.localScale = obj.transform.localScale;
                modelProjector.layer = LayerMask.NameToLayer(GlobalProjectorManager.GlobalProjectorLayer);

                Renderer modelProjectorRenderer = modelProjector.GetComponent<MeshRenderer>();
                if (modelProjectorRenderer == null)
                {
                    modelProjectorRenderer = modelProjector.GetComponent<SkinnedMeshRenderer>();
                }
                modelProjectorRenderer.enabled = false;

                modelProjectorRenderer.material = preShadowMaterial;

                shadowCasters.Add(modelProjector);
                shadowCasterRenderers.Add(modelProjectorRenderer);
            }
        }

        void OnEnable()
        {
            GlobalProjectorManager.Request(() =>
            {
                GlobalProjectorManager.Instance.AddProjector(this);
            });
            for (int i = 0; i < shadowCasters.Count; i++)
            {
                shadowCasters[i].SetActive(true);
            }
        }

        void OnDisable()
        {
            if (GlobalProjectorManager.IsInit())
            {
                GlobalProjectorManager.Instance.RemoveProjector(this);
            }
            for (int i = 0; i < shadowCasters.Count; i++)
            {
                shadowCasters[i].SetActive(false);
            }
        }

        void OnDestroy()
        {
            preShadowMaterial = null;
            shadowCasters = null;
            shadowCasterRenderers = null;
    }

        public bool IsLight
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        protected Vector3 _GlobalProjectionDir = new Vector3(0.0f, -1.0f, 0.0f);

        public Vector3 GlobalProjectionDir
        {
            set
            {
                _GlobalProjectionDir = value;
                if (GlobalProjectorManager.IsInit())
                {
                    GlobalProjectorManager.Instance.GlobalProjectionDir = _GlobalProjectionDir;
                }
            }
            get
            {
                return _GlobalProjectionDir;
            }
        }

        public int GlobalShadowResolution
        {
            set
            {
                _GlobalShadowResolution = value;

                if (GlobalProjectorManager.IsInit())
                {
                    GlobalProjectorManager.Instance.GlobalShadowResolution = _GlobalShadowResolution;
                }
            }
            get
            {
                return _GlobalShadowResolution;
            }
        }

        protected int _GlobalShadowResolution = 1;

        public GlobalProjectorManager.ShadowType Type
        {
            get { return GlobalProjectorManager.ShadowType.ModelProjection; }
        }

        public Bounds GetBounds()
        {
            Bounds bounds = new Bounds();
            for (int i = 0; i < shadowCasterRenderers.Count; i++)
            {
                if (i == 0)
                {
                    bounds = shadowCasterRenderers[i].bounds;
                }
                else
                {
                    bounds.Encapsulate(shadowCasterRenderers[i].bounds);
                }
            }
            return bounds;
        }

        public void SetVisible(bool visible)
        {
            if (!visible)
            {
                int j = 0;
            }

            for (int i = 0; i < shadowCasterRenderers.Count; i++)
            {
                shadowCasterRenderers[i].enabled = visible;
            }
        }


        public void OnPreRenderShadowProjector(Camera camera)
        {
        }
    }
}


