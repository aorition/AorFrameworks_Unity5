using UnityEngine;
using System.Collections.Generic;
using YoukiaUnity.Misc;

//using YoukiaUnity.App;

namespace YoukiaUnity.Graphics.FastShadowProjector
{
    //暂时不使用阴影的AlphaCulling,以此公用Material,否则预阴影材质需要和每一个使用透明裁剪的绘制体相对应
    public class ModelShadowProjector : GraphicsLauncher, IProjector
    {



        protected List<GameObject> shadowCasters;
        protected List<GameObject> shadowCastersParent;
        protected List<Renderer> shadowCasterRenderers;
        private bool isInited = false;
        public bool InCamera;

        public void ResetLayer()
        {
            for (int i = 0; i < shadowCasters.Count; i++)
            {
                shadowCasters[i].layer = LayerMask.NameToLayer(GlobalProjectorManager.GlobalProjectorLayer);
            }
        }


        //        public override void OnScriptCoverFinish()
        protected override void Launcher()
        {
            base.Launcher();
            shadowCasters = new List<GameObject>();
            shadowCastersParent = new List<GameObject>();
            shadowCasterRenderers = new List<Renderer>();
            initModelShadowProjector(gameObject, true);
            isInited = true;
            AddProjector();
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

                //过滤face
                RoleFaceHandler rfd = obj.GetComponent<RoleFaceHandler>();
                if (rfd) return;

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
                // Debug.Log(GraphicsManager.GetInstance().ShadowMgr.PreShadowMaterial);
                modelProjectorRenderer.sharedMaterial = GraphicsManager.GetInstance().ShadowMgr.PreShadowMaterial;

                shadowCasters.Add(modelProjector);
                shadowCastersParent.Add(renderer.gameObject);
                shadowCasterRenderers.Add(modelProjectorRenderer);
            }


        }

        private bool checkInit()
        {
            if (GraphicsManager.GetInstance() == null || GraphicsManager.GetInstance().ShadowMgr == null || !isInited)
            {
                return false;
            }
            return true;
        }

        void AddProjector()
        {
            if (GraphicsManager.GetInstance() == null)
                return;

            GraphicsManager.GetInstance().ShadowMgr.AddProjector(this);
            for (int i = 0; i < shadowCasters.Count; i++)
            {
                if (shadowCastersParent[i].activeInHierarchy)
                    shadowCasters[i].SetActive(true);
            }

        }

        void OnEnable()
        {

            StartAfterGraphicMgr(this, () =>
            {
                if (!checkInit())
                    return;

                AddProjector();

            });


        }

        private void OnDisable()
        {

            if (!checkInit())
                return;
            GraphicsManager.GetInstance().ShadowMgr.RemoveProjector(this);
            for (int i = 0; i < shadowCasters.Count; i++)
            {
                if (shadowCasters[i] != null)
                    shadowCasters[i].SetActive(false);
            }


        }

        void OnDestroy()
        {

            for (int i = 0; i < shadowCasters.Count; i++)
            {
                if (shadowCasters[i] != null)
                    Destroy(shadowCasters[i]);
            }

            shadowCastersParent = null;
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
                GraphicsManager.GetInstance().ShadowMgr.GlobalProjectionDir = _GlobalProjectionDir;
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
                GraphicsManager.GetInstance().ShadowMgr.GlobalShadowResolution = _GlobalShadowResolution;

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
            bool first = true;
            for (int i = 0; i < shadowCasterRenderers.Count; i++)
            {
                if (shadowCasterRenderers[i].gameObject.activeInHierarchy)
                {
                    if (first)
                    {
                        bounds = shadowCasterRenderers[i].bounds;
                        first = false;
                    }
                    else
                    {
                        bounds.Encapsulate(shadowCasterRenderers[i].bounds);
                    }
                }
            }
            return bounds;
        }

        public void SetVisible(bool visible)
        {


            for (int i = 0; i < shadowCasterRenderers.Count; i++)
            {
                shadowCasterRenderers[i].enabled = visible;
            }

        }

        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        private Vector3 v3;
        private Vector2 v2;
        private Vector3[] _points = new Vector3[8];

        void Update()
        {
            if (GraphicsManager.GetInstance() != null && GraphicsManager.GetInstance().CurrentSubCamera != null)
            {
                Camera cam = GraphicsManager.GetInstance().ShadowMgr.viewCamera;
                v3 = cam.WorldToScreenPoint(transform.position);
                v2.x = v3.x;
                v2.y = v3.y;

                Bounds aabb = GetBounds();
                Vector3 size = aabb.extents;
                _points[0] = aabb.center + size;
                size = new Vector3(-aabb.extents.x, aabb.extents.y, aabb.extents.z);
                _points[1] = aabb.center + size;
                size = new Vector3(aabb.extents.x, -aabb.extents.y, aabb.extents.z);
                _points[2] = aabb.center + size;
                size = new Vector3(aabb.extents.x, aabb.extents.y, -aabb.extents.z);
                _points[3] = aabb.center + size;
                size = new Vector3(-aabb.extents.x, -aabb.extents.y, aabb.extents.z);
                _points[4] = aabb.center + size;
                size = new Vector3(aabb.extents.x, -aabb.extents.y, -aabb.extents.z);
                _points[5] = aabb.center + size;
                size = new Vector3(-aabb.extents.x, aabb.extents.y, -aabb.extents.z);
                _points[6] = aabb.center + size;
                size = new Vector3(-aabb.extents.x, -aabb.extents.y, -aabb.extents.z);
                _points[7] = aabb.center + size;


                InCamera = false;
                for (int i = 0; i < _points.Length; i++)
                {
                    if (cam.CheckCollision(_points[i]))
                    {
                        InCamera = true;
                        break;
                    }
                }

            }
        }

        public void OnPreRenderShadowProjector(Camera camera)
        {


        }
    }
}



