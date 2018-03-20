using UnityEngine;
using System.Collections;

public class EdgeRenderer : MonoBehaviour {
    public Material mat;

    // Use this for initialization
    void Start () {
	    mat = new Material(Shader.Find("Custom/EdgeDraw"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, mat);
    }
}
