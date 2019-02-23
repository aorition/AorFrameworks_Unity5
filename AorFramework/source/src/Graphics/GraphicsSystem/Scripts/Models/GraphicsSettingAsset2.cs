using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic
{

    /// <summary>
    /// ScriptableObject子类目前有个bug(Unity5.x): 在需要热更的情况下,自定义struct结构不能够反序列化成功,导致数据丢失
    /// 
    /// 目前此版本去掉了struct嵌套结构,改为数据平坦存储.
    /// 
    /// </summary>
    public class GraphicsSettingAsset : ScriptableObject
    {

        public bool UseFixedUpdate = false;
        public bool AllowVCamParaCover = true;

        /// <summary>
        /// 主相机描述
        /// </summary>
        private GCamGDesInfo _mainDesInfo;
        public GCamGDesInfo MainCamDesInfo
        {
            get
            {
                if (!_mainDesInfo.init)
                {
                    _mainDesInfo = _getCamGDesInfo(0);
                }
                return _mainDesInfo;
            }
            set { _mainDesInfo = value; }
        }

        /// <summary>
        /// 子相机描述
        /// </summary>
        private List<GCamGDesInfo> _subCamGDesInfos;
        public List<GCamGDesInfo> SubCamGDesInfos
        {
            get
            {
                if (_subCamGDesInfos == null)
                {
                    _subCamGDesInfos = new List<GCamGDesInfo>();
                    for (int i = 1; i < NameList.Count; i++)
                    {
                        _subCamGDesInfos.Add(_getCamGDesInfo(i));
                    }
                }
                return _subCamGDesInfos;
            }
        }

        private GCamGDesInfo _getCamGDesInfo(int i)
        {
            if (i >= 0 && i < NameList.Count)
            {
                GCLensSetting ls = HasLensSettingList[i] ? new GCLensSetting(ClearFlagsList[i], BackgroundColorList[i]
                    , isOrthographicCameraList[i], OrthographicSizeList[i], FieldOfViewList[i]
                    , NearClipPlaneList[i], FarClipPlaneList[i], RenderingPathList[i], UseOcclusionCullingList[i]
                    , AllowHDRList[i], AllowMSAAList[i]
                    ) : new GCLensSetting();

                GCamGDesInfo info = new GCamGDesInfo(NameList[i], TypeList[i], CullingMaskList[i], DepthList[i], ls);
                return info;
            }
            return new GCamGDesInfo();
        }

        //---------------------------------------------------------- 

        /// ScriptableObject子类目前有个bug(Unity5.x): 在需要热更的情况下,自定义struct结构不能够反序列化成功,导致数据丢失
        /// 
        /// 目前此版本去掉了struct嵌套结构,改为数据平坦存储.
        /// 
        /// ps : 主相机描述 一定是在List的Index 0位, 其他的为子相机描述

        //-----------  GCamGDesInfo
        public List<string> NameList = new List<string>();
        public List<SubGCamType> TypeList = new List<SubGCamType>();
        public List<int> CullingMaskList = new List<int>();
        public List<float> DepthList = new List<float>();

        //----------  GCLensSetting
        public List<bool> HasLensSettingList = new List<bool>();

        public List<CameraClearFlags> ClearFlagsList = new List<CameraClearFlags>();
        public List<Color> BackgroundColorList = new List<Color>();
        public List<bool> isOrthographicCameraList = new List<bool>();
        public List<float> OrthographicSizeList = new List<float>();
        public List<float> FieldOfViewList = new List<float>();
        public List<float> NearClipPlaneList = new List<float>();
        public List<float> FarClipPlaneList = new List<float>();
        public List<RenderingPath> RenderingPathList = new List<RenderingPath>();
        public List<bool> UseOcclusionCullingList = new List<bool>();
        public List<bool> AllowHDRList = new List<bool>();
        public List<bool> AllowMSAAList = new List<bool>();


    }

    [Serializable]
    public enum SubGCamType
    {
        Normal,
        MainCamera,
        RenderTextureCombine,
        FinalOutput
    }


    //public class GCamGDesInfo : ScriptableObject
    //[Serializable]
    public struct GCamGDesInfo
    {

        public static GCamGDesInfo Default()
        {
            return new GCamGDesInfo("Uname", SubGCamType.Normal, 0, 0);
        }

        public static GCamGDesInfo Main()
        {
            return new GCamGDesInfo("MainCamera", SubGCamType.MainCamera, -1, 0, GCLensSetting.Default());
        }

        public GCamGDesInfo(string name, SubGCamType type, int cullingMask, int depth)
        {
            this.init = true;
            this.name = name;
            this.type = type;
            this.cullingMask = cullingMask;
            this.depth = depth;
            this.lensSetting = new GCLensSetting();
        }

        public GCamGDesInfo(string name, SubGCamType type, int cullingMask, float depth, GCLensSetting lens)
        {
            this.init = true;
            this.name = name;
            this.type = type;
            this.cullingMask = cullingMask;
            this.depth = depth;
            this.lensSetting = lens;
        }

        public bool init;
        public string name;
        public SubGCamType type;
        //    public int cullingMask;
        public int cullingMask;
        public float depth;
        public GCLensSetting lensSetting;
    }

    //[Serializable]
    public struct GCLensSetting
    {

        public static GCLensSetting Default()
        {
            return new GCLensSetting(CameraClearFlags.Skybox, Color.blue, false, 5, 60f, 0.3f, 1000f, RenderingPath.UsePlayerSettings, true, true, true);
        }

        public GCLensSetting(CameraClearFlags ClearFlags, Color BackgroundColor,
            bool isOrthographicCamera, float OrthographicSize, float FieldOfView,
            float NearClipPlane, float FarClipPlane,
            RenderingPath RenderingPath,
            bool UseOcclusionCulling, bool AllowHDR, bool AllowMSAA
            )
        {
            this.init = true;
            this.ClearFlags = ClearFlags;
            this.BackgroundColor = BackgroundColor;
            this.isOrthographicCamera = isOrthographicCamera;
            this.OrthographicSize = OrthographicSize;
            this.FieldOfView = FieldOfView;
            this.NearClipPlane = NearClipPlane;
            this.FarClipPlane = FarClipPlane;
            this.RenderingPath = RenderingPath;
            this.UseOcclusionCulling = UseOcclusionCulling;
            this.AllowHDR = AllowMSAA;
            this.AllowMSAA = AllowMSAA;
        }

        public bool init;
        public CameraClearFlags ClearFlags;
        public Color BackgroundColor;
        public bool isOrthographicCamera;
        public float OrthographicSize;
        public float FieldOfView;
        public float NearClipPlane;
        public float FarClipPlane;
        public RenderingPath RenderingPath;
        public bool UseOcclusionCulling;
        public bool AllowHDR;
        public bool AllowMSAA;
    }
}