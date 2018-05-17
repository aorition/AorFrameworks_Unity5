using UnityEngine;
using System.Collections;

namespace Framework.Graphic.FastShadowProjector
{
    [AddComponentMenu("Fast Shadow Projector/Shadow Receiver")]
    public class ShadowReceiver : MonoBehaviour
    {

        MeshFilter _meshFilter;
        Mesh _mesh;
        Mesh _meshCopy;
        MeshRenderer _meshRenderer;
        Terrain _terrain = null;
        public Material _terrainMaterial;

        bool _isTerrain = false;

        public int _id;

        void Awake()
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
                    Debug.Log("Could not find: FSP_TerrainFirstPass shader!");
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

        void OnEnable()
        {
            AddReceiver();
        }

        void OnDisable()
        {
            RemoveReceiver();
        }

        void OnBecameVisible()
        {
            AddReceiver();
        }

        void OnBecameInvisible()
        {
            RemoveReceiver();
        }

        void OnDestroy()
        {
           //
        }

        void AddReceiver()
        {
            if (_meshFilter != null || _terrain != null)
            {
                if (IsTerrain())
                {
                    _terrain.enabled = true;
                }
                GlobalProjectorManager.Request(() =>
                {
                    GlobalProjectorManager.Instance.AddReceiver(this);
                });
            }
        }

        void RemoveReceiver()
        {
            if (GlobalProjectorManager.IsInit())
            {
                if (_meshFilter != null || _terrain != null)
                {
                    if (IsTerrain())
                    {
                        _terrain.enabled = false;
                    }
                    GlobalProjectorManager.Instance.RemoveReceiver(this);
                }
            }
        }
    }
}

