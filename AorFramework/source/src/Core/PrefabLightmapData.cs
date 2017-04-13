using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class PrefabLightmapData : MonoBehaviour
{
    [SerializeField]
    private int _lightmapIndex;
    [SerializeField]
    private Vector4 _lightmapOffsetScale;

    public bool AutoLoadOnAwake = true;

    void Awake()
    {
        if (AutoLoadOnAwake)
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

    public void SaveLightmap(int index, Vector4 offestScale)
    {
        _lightmapOffsetScale = offestScale;
        _lightmapIndex = index;
    }

    public void SaveLightmap()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr)
        {

//            MeshFilter mf = GetComponent<MeshFilter>();
//            if (mf)
//            {
//               
//            }

            _lightmapOffsetScale = mr.lightmapScaleOffset;
            _lightmapIndex = mr.lightmapIndex;

            //                Texture2D lightmap = LightmapSettings.lightmaps[r.lightmapIndex].lightmapFar;  
            //  
            //                info.lightmapIndex = m_Lightmaps.IndexOf(lightmap);  
            //                if (info.lightmapIndex == -1) {  
            //                    info.lightmapIndex = m_Lightmaps.Count;  
            //                    m_Lightmaps.Add(lightmap);  
            //                }  

        }
    }

    public void LoadLightmap()
    {
        Renderer _renderer = GetComponent<Renderer>();

        if (_renderer)
        {
            if(_renderer.isPartOfStaticBatch)
            {

                MeshFilter mf = GetComponent<MeshFilter>();
                if (mf)
                {
                    Debug.Log(mf.sharedMesh);
                    List<Vector2> uvs = new List<Vector2>();
                    //mf.sharedMesh.GetUVs(3, uvs);
                    //mf.sharedMesh.SetUVs(0, uvs);
                    //推测，合并后，uv1、uv2不正确了。需要重新计算
//                    mf.sharedMesh.SetUVs(1, uvs);
//                    mf.sharedMesh.SetUVs(2, uvs);
                }

                _renderer.lightmapIndex = _lightmapIndex;
                _renderer.lightmapScaleOffset = _lightmapOffsetScale;
            }
            else
            {

                MeshFilter mf = GetComponent<MeshFilter>();
                if (mf)
                {
                    Debug.Log(mf.sharedMesh);
                    
                }

                _renderer.lightmapIndex = _lightmapIndex;
                _renderer.lightmapScaleOffset = _lightmapOffsetScale;
            }
        }

            
    }
}