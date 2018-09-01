using UnityEngine;

namespace Framework.Graphic
{

    public class PrefabLightmapData : MonoBehaviour
    {

        public bool AutoLoadOnEnable = true;

        [SerializeField] protected Vector4 _lightmapScaleOffset;
        public Vector4 lightmapScaleOffset
        {
            get { return _lightmapScaleOffset; }
        }

        [SerializeField] protected int _lightmapIndex;
        public int lightmapIndex
        {
            get { return _lightmapIndex; }
        }

        protected virtual void OnEnable()
        {
            if (AutoLoadOnEnable) LoadLightmap();
        }

        public virtual void SaveLightmap()
        {
            Renderer rd = GetComponent<Renderer>();
            if (rd)
            {
                SaveLightmap(rd.lightmapIndex, rd.lightmapScaleOffset);
            }
        }

        public virtual void SaveLightmap(int index, Vector4 scaleOffset)
        {
            _lightmapIndex = index;
            _lightmapScaleOffset = scaleOffset;
        }

        public virtual void LoadLightmap()
        {
            Renderer rd = GetComponent<Renderer>();
            if (rd)
            {
                rd.lightmapIndex = _lightmapIndex;
                rd.lightmapScaleOffset = _lightmapScaleOffset;
            }
            
        }
    }

}