using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class PrefabLightmapData : MonoBehaviour
{
    [SerializeField]
    private int _lightmapIndex;
    public int lightmapIndex
    {
        get { return _lightmapIndex; }
    }

    [SerializeField]
    private Vector4 _lightmapOffsetScale;
    public Vector4 lightmapOffsetScale
    {
        get { return _lightmapOffsetScale; }
    }

    public bool AutoLoadOnEnable = true;

    public int LightmapIndex
    {
        get { return _lightmapIndex; }
        set { _lightmapIndex = value; }
     
    }


    void OnEnable()
    {
        if (AutoLoadOnEnable)
        {
            LoadLightmap();
        }
        //       Debug.Log("--------------------------------------------");
//        var renderers = GetComponentsInChildren<MeshRenderer>();
//        foreach (var item in renderers)
//        {
//            Debug.Log(item.lightmapIndex);
//        }

        //        
        //      var lightmaps = LightmapSettings.lightmaps;  
        //      var combinedLightmaps = new LightmapData[lightmaps.Length + m_Lightmaps.Count];  
        //        
        //      lightmaps.CopyTo(combinedLightmaps, 0);  
        //      for (int i = 0; i < m_Lightmaps.Count; i++)  
        //      {  
        //          combinedLightmaps[i+lightmaps.Length] = new LightmapData();  
        //          combinedLightmaps[i+lightmaps.Length].lightmapFar = m_Lightmaps[i];  
        //      }  
        //        
        //      ApplyRendererInfo(m_RendererInfo, lightmaps.Length);  
        //      LightmapSettings.lightmaps = combinedLightmaps;  
    }

    public void SaveLightmap()
    {
        Renderer _renderer = GetComponent<Renderer>();

        if (_renderer)
        {
            SaveLightmap(_renderer.lightmapIndex, _renderer.lightmapScaleOffset);
        }
    }

    public void SaveLightmap(int index, Vector4 offestScale)
    {
        _lightmapOffsetScale = offestScale;
        _lightmapIndex = index;
    }

    public void LoadLightmap()
    {
        Renderer _renderer = GetComponent<Renderer>();

        if (_renderer)
        {
            _renderer.lightmapIndex = _lightmapIndex;
            _renderer.lightmapScaleOffset = _lightmapOffsetScale;
      
        }

            
    }
}