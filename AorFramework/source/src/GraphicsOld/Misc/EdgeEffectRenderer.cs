using UnityEngine;
using System.Collections;
using YoukiaUnity.Graphics;

public class EdgeEffectRenderer : MonoBehaviour
{

    private static Camera _createEdgeCamera(Camera srcCamera)
    {

        GameObject edgeCameraObj = new GameObject("EdgeCamera");
        edgeCameraObj.transform.SetParent(srcCamera.transform, false);

        Camera disCamera = edgeCameraObj.AddComponent<Camera>();
        disCamera.CopyFrom(srcCamera); //确保不同分辨率下渲染正确
               
        return disCamera;
    }

    public Material mat;
    public Material reverseMat;

    public RenderTexture LinkedRT;
    public RenderTexture TempRT;

    private Camera _linkedCamera;

    void Awake()
    {
        mat = new Material(Shader.Find("Custom/EdgeDrawWithBlend"));
        reverseMat = new Material(Shader.Find("Hidden/PostEffect/reverse"));
    }

    void Start()
    {
        if (LinkedRT == null)
        {
            LinkedRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
            LinkedRT.hideFlags = HideFlags.HideAndDontSave;
        }
        if (TempRT == null)
        {
            TempRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
            TempRT.hideFlags = HideFlags.HideAndDontSave;
        }

        _linkedCamera = _createEdgeCamera(GetComponent<Camera>());

        _linkedCamera.backgroundColor = new Color(0, 0, 0, 0);
        _linkedCamera.clearFlags = CameraClearFlags.SolidColor;
        _linkedCamera.targetTexture = LinkedRT;

    }

    void OnDestroy()
    {
        if (_linkedCamera)
        {
            GameObject.Destroy(_linkedCamera.gameObject);
            _linkedCamera = null;
        }
        if (LinkedRT != null)
        {
            LinkedRT.DiscardContents();
            LinkedRT = null;
        }
        if (TempRT != null)
        {
            TempRT.DiscardContents();
            TempRT = null;
        }

    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {

#if UNITY_5_6_OR_NEWER

        //5.6 解决了编辑器上混合颠倒的问题
        UnityEngine.Graphics.Blit(LinkedRT, TempRT);
#else
            //默认后期模式需要区分平台,不然上下颠倒
            if (Application.isEditor && Application.platform==RuntimePlatform.WindowsEditor)
            {
            
                UnityEngine.Graphics.Blit(LinkedRT, TempRT, reverseMat);
            }
            else
            {
                UnityEngine.Graphics.Blit(LinkedRT, TempRT);
            }
#endif

        mat.SetTexture("_SecTex", TempRT);

        Graphics.Blit(src, dest, mat);

    }
}
