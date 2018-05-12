using UnityEngine;
using System.Collections;

namespace Framework.Graphic.FastShadowProjector
{
    //[AddComponentMenu("Fast Shadow Projector/Shadow Receiver")]
    public class ShadowReceiver : MonoBehaviour
    {

        MeshFilter _meshFilter;
        Mesh _mesh;
        Mesh _meshCopy;
        MeshRenderer _meshRenderer;
        Terrain _terrain;
        public Material _terrainMaterial;

        bool _isTerrain;
        private bool isInited;
        public int _id;

        public void Awake()
        {

            FastShadowProjectorManager.Request(() =>
            {
                _meshFilter = GetComponent<MeshFilter>();
                _meshRenderer = GetComponent<MeshRenderer>();
                _terrain = GetComponent<Terrain>();

                if (_terrain != null)
                {
                    _isTerrain = true;

                    _terrainMaterial = new Material(Shader.Find("FastShadowProjector/FSP_TerrainFirstPass"));

                    if (_terrainMaterial == null)
                    {
                        Log.Warning("Could not find: FSP_TerrainFirstPass shader!");
                    }
                    else
                    {
                        _terrain.materialTemplate = _terrainMaterial;
                    }
                }

                if (_terrain == null && null != _meshRenderer && _meshFilter != null)
                {
                    _mesh = _meshFilter.sharedMesh;
                }

                _meshCopy = null;
                isInited = true;
                AddReceiver();
            });

        }

        public Mesh GetMesh()
        {
            if (_meshCopy != null)
            {
                return _meshCopy;
            }
            else
            {
                return _mesh;
            }
        }

        public bool IsTerrain()
        {
            if (!Application.isPlaying)
            {
                if (GetComponent<Terrain>() != null)
                {
                    _isTerrain = true;
                    return true;
                }
            }
            return _isTerrain;
        }

        public Terrain GetTerrain()
        {
            return _terrain;
        }

        void OnDisable()
        {
            if (!checkInit())
                return;

            RemoveReceiver();
        }

        private bool checkInit()
        {
            return isInited;

        }

        void OnEnable()
        {
            if (!checkInit())
                return;

            AddReceiver();
        }

        void OnBecameVisible()
        {
            if (!checkInit())
                return;


            AddReceiver();
        }

        void OnBecameInvisible()
        {
            if (!checkInit())
                return;

            RemoveReceiver();
        }

        void OnDestroy()
        {
            if (!checkInit())
                return;

            RemoveReceiver();
        }

        void AddReceiver()
        {
            if (_meshFilter != null || _terrain != null)
            {
                if (IsTerrain())
                {
                    _terrain.enabled = true;
                }

                if(FastShadowProjectorManager.IsInit())
                    FastShadowProjectorManager.Instance.AddReceiver(this);
            }
        }

        void RemoveReceiver()
        {
            if (_meshFilter != null || _terrain != null)
            {
                if (IsTerrain())
                {
                    _terrain.enabled = false;
                }

                if (FastShadowProjectorManager.IsInit())
                    FastShadowProjectorManager.Instance.RemoveReceiver(this);
            }

        }
    }

}



