using AorFrameworks;
using Framework.Graphic;
using UnityEngine;

namespace Framework.Graphic.Utility
{
    public class GraphicsCamUtility
    {

//    public static void ApplayCamParamToCamera(Camera cam, GraphicsSettingAsset des)
//    {
//        cam.clearFlags = des.ClearFlags;
//        cam.backgroundColor = des.BackgroundColor;
//        cam.orthographic = des.isOrthographicCamera;
//        cam.orthographicSize = des.OrthographicSize;
//        cam.fieldOfView = des.FieldOfView;
//        cam.nearClipPlane = des.NearClipPlane;
//        cam.farClipPlane = des.FarClipPlane;
//        cam.useOcclusionCulling = des.UseOcclusionCulling;
//        cam.allowHDR = des.AllowHDR;
//        cam.allowMSAA = des.AllowMSAA;
//    }

        public static void ApplyDesDataToCameraFormDesInfo(Camera cam, GCamGDesInfo desInfo)
        {

            //此处不检查是否初始化,以免数据引出出错!!
            //if (!desInfo.init) return;

            cam.name = desInfo.name;
            cam.depth = desInfo.depth;
            cam.cullingMask = ParseLayerStrToCullingMask(desInfo.cullingMask);

            if (desInfo.lensSetting.init)
            {
                cam.clearFlags = desInfo.lensSetting.ClearFlags;
                cam.backgroundColor = desInfo.lensSetting.BackgroundColor;
                cam.orthographic = desInfo.lensSetting.isOrthographicCamera;
                cam.orthographicSize = desInfo.lensSetting.OrthographicSize;
                cam.fieldOfView = desInfo.lensSetting.FieldOfView;
                cam.nearClipPlane = desInfo.lensSetting.NearClipPlane;
                cam.farClipPlane = desInfo.lensSetting.FarClipPlane;
                cam.useOcclusionCulling = desInfo.lensSetting.UseOcclusionCulling;
                cam.allowHDR = desInfo.lensSetting.AllowHDR;
                cam.allowMSAA = desInfo.lensSetting.AllowMSAA;
            }

            switch (desInfo.type)
            {
                case SubGCamType.Normal:
                    cam.clearFlags = CameraClearFlags.Nothing;
                    break;
                case SubGCamType.RenderTextureCombine:
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.backgroundColor = new Color(0, 0, 0, 0);
                    RenderTextureCaptureCamera rtcam = cam.gameObject.AddComponent<RenderTextureCaptureCamera>();

                    GameObject rtcGo = new GameObject(cam.name + "RtCombine");
                    Camera rtCamera = rtcGo.AddComponent<Camera>();
                    rtCamera.clearFlags = CameraClearFlags.Nothing;
                    rtCamera.cullingMask = 0;
                    rtCamera.depth = cam.depth;

                    GraphicsManager.instance.AddSubCamera(rtCamera);

                    RenderTextureCombine rtc = RenderTextureCombine.Create(rtcGo,
                        RenderTextureCombine.PostEffectCombineType.Normal);

                    rtcam.rtCombine = rtc;

                    rtcGo.transform.SetParent(cam.transform, false);

                    break;
                case SubGCamType.FinalOutput:
                    cam.clearFlags = CameraClearFlags.Nothing;
                    break;
            }

            if (desInfo.type != SubGCamType.MainCamera && GraphicsManager.instance.OnSubCameraInited != null)
            {
                GraphicsManager.instance.OnSubCameraInited(cam, desInfo);
            }

        }

        /// <summary>
        /// 将GraphicsCamGroupDescribeAsset记录的CullingMask字符串转成int的LayerMask
        /// </summary>
        /// <param name="layerStr"></param>
        /// <returns></returns>
        public static int ParseLayerStrToCullingMask(string layerStr)
        {
            int m = 0;

            if (string.IsNullOrEmpty(layerStr) || layerStr.ToLower() == "nothing")
            {
                return m;
            }

            if (layerStr.ToLower() == "all" || layerStr.ToLower() == "everything")
            {
                return -1;
            }

            if (layerStr.Contains("|"))
            {
                string[] li = layerStr.Split('|');
                for (int i = 0; i < li.Length; i++)
                {
                    if (i == 0)
                    {
                        m = 1 << LayerMask.NameToLayer(li[i]);
                    }
                    else
                    {
                        m |= 1 << LayerMask.NameToLayer(li[i]);
                    }
                }
                return m;
            }
            else
            {
                m = 1 << LayerMask.NameToLayer(layerStr);
                return m;
            }
        }
    }
}