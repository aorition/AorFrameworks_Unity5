using System;
using System.Collections.Generic;
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
//            cam.cullingMask = ParseLayerStrToCullingMask(desInfo.cullingMask);
            cam.cullingMask = desInfo.cullingMask;

            if (desInfo.lensSetting.init)
            {
                cam.clearFlags = desInfo.lensSetting.ClearFlags;
                cam.backgroundColor = desInfo.lensSetting.BackgroundColor;
                cam.orthographic = desInfo.lensSetting.isOrthographicCamera;
                cam.orthographicSize = desInfo.lensSetting.OrthographicSize;
                cam.fieldOfView = desInfo.lensSetting.FieldOfView;
                cam.nearClipPlane = desInfo.lensSetting.NearClipPlane;
                cam.farClipPlane = desInfo.lensSetting.FarClipPlane;
                cam.renderingPath = desInfo.lensSetting.RenderingPath;
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

                    GraphicsManager.Instance.AddSubCamera(rtCamera);

                    RenderTextureCombine rtc = RenderTextureCombine.Create(rtcGo,
                        RenderTextureCombine.PostEffectCombineType.Normal);

                    rtcam.rtCombine = rtc;

                    rtcGo.transform.SetParent(cam.transform, false);

                    break;
                case SubGCamType.FinalOutput:
                    cam.clearFlags = CameraClearFlags.Nothing;
                    cam.gameObject.AddComponent<FlareLayer>();
                    GraphicsManager.Instance.FLPostEffectController = cam.gameObject.AddComponent<FLPostEffectController>();
                    GraphicsManager.Instance.FLPostEffectController.enabled = false; //初始化默认是关闭状态以节省性能开销
                    break;
            }

            if (desInfo.type == SubGCamType.MainCamera && GraphicsManager.Instance.OnMainCameraInited != null)
            {
                GraphicsManager.Instance.OnMainCameraInited(cam, desInfo);
            }
            else  if (desInfo.type != SubGCamType.MainCamera && GraphicsManager.Instance.OnSubCameraInited != null)
            {
                GraphicsManager.Instance.OnSubCameraInited(cam, desInfo);
            }

        }

        public static string[] GetMaskDisplayOption()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < 32; i++)
            {
                string n = LayerMask.LayerToName(i);
                list.Add(n);
            }
            return list.ToArray();
        }

        public static string[] GetMaskDisplayOption(ref int[] indexs)
        {
            List<int> ids = new List<int>();
            List<string> list = new List<string>();
            for (int i = 0; i < 32; i++)
            {
                string n = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(n))
                {
                    ids.Add(i);
                    list.Add(n);
                }
            }
            indexs = ids.ToArray();
            return list.ToArray();
        }

        /// <summary>
        /// 将GraphicsCamGroupDescribeAsset记录的CullingMask字符串转成int的LayerMask
        /// </summary>
        /// <param name="layerStr"></param>
        [Obsolete("GCamGDesInfo.CullingMask 由原来的string变更为int,故废弃该方法")]
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