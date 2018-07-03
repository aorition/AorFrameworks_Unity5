using System.Collections;
using System.Collections.Generic;
using AorBaseUtility;
using Framework.Editor;
using Framework.Graphic.Utility;
using UnityEngine;
using UnityEditor;

namespace Framework.UI.editor
{
    [CustomEditor(typeof(GraphicsSettingAsset))]
    public class GraphicsSettingAssetEditor : UnityEditor.Editor
    {

        [MenuItem("Assets/GraphicsManagerAssets/GraphicsSettingAsset")]
        public static void CreateAsset()
        {
            EditorPlusMethods.CreateAssetFile<GraphicsSettingAsset>("unameGraphicsSettingAsset");
        }

        private enum editorProjection
        {
            Perspective = 0,
            Orthographic = 1,
        }

        private enum editorCameraClearFlags
        {
            Skybox = 1,
            SolidColor = 2,
            DepthOnly = 3,
            DontClear = 4,
        }

        private GraphicsSettingAsset _target;

        private void Awake()
        {
            _target = target as GraphicsSettingAsset;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();
            _readGSISerializeData();
            _draw_baseParam();
            GUILayout.Space(8);
            _draw_mainCameraMParam();
            GUILayout.Space(8);
            _draw_subCameraMParam();
            _saveGSISerializeData();
            serializedObject.ApplyModifiedProperties();

            #region *** 按钮-> <立即写入修改数据到文件> :: 建议所有.Asset文件的Editor都配备此段代码
            EditorPlusMethods.Draw_AssetFileApplyImmediateUI(target);
            #endregion
        }

        private void _draw_baseParam()
        {
            GUILayout.BeginVertical("box");

            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            GUILayout.Label("基础设置");
            GUILayout.EndVertical();

            GUILayout.Space(8);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseFixedUpdate"), new GUIContent("Use Fixed Update", "使用FixedUpdate刷新"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowVCamParaCover"), new GUIContent("Allow VCam ParaCover", "允许VisualCamera覆盖主相机参数"));

            GUILayout.Space(5);

            GUILayout.EndVertical();
        }

        //-----------  GCamGDesInfo
        private List<string> NameList;
        private List<SubGCamType> TypeList;
        private List<int> CullingMaskList;
        private List<float> DepthList;

        //----------  GCLensSetting
        private List<bool> HasLensSettingList;

        private List<CameraClearFlags> ClearFlagsList;
        private List<Color> BackgroundColorList;
        private List<bool> isOrthographicCameraList;
        private List<float> OrthographicSizeList;
        private List<float> FieldOfViewList;
        private List<float> NearClipPlaneList;
        private List<float> FarClipPlaneList;
        private List<RenderingPath> RenderingPathList;
        private List<bool> UseOcclusionCullingList;
        private List<bool> AllowHDRList;
        private List<bool> AllowMSAAList;

        private void _readGSISerializeData()
        {
            NameList = (List<string>)_target.GetPublicField("NameList");
            TypeList = (List<SubGCamType>)_target.GetPublicField("TypeList");
            CullingMaskList = (List<int>)_target.GetPublicField("CullingMaskList");
            DepthList = (List<float>)_target.GetPublicField("DepthList");
            HasLensSettingList = (List<bool>)_target.GetPublicField("HasLensSettingList");
            ClearFlagsList = (List<CameraClearFlags>)_target.GetPublicField("ClearFlagsList");
            BackgroundColorList = (List<Color>)_target.GetPublicField("BackgroundColorList");
            isOrthographicCameraList = (List<bool>)_target.GetPublicField("isOrthographicCameraList");
            OrthographicSizeList = (List<float>)_target.GetPublicField("OrthographicSizeList");
            FieldOfViewList = (List<float>)_target.GetPublicField("FieldOfViewList");
            NearClipPlaneList = (List<float>)_target.GetPublicField("NearClipPlaneList");
            FarClipPlaneList = (List<float>)_target.GetPublicField("FarClipPlaneList");
            RenderingPathList = (List<RenderingPath>)_target.GetPublicField("RenderingPathList");
            UseOcclusionCullingList = (List<bool>)_target.GetPublicField("UseOcclusionCullingList");
            AllowHDRList = (List<bool>)_target.GetPublicField("AllowHDRList");
            AllowMSAAList = (List<bool>)_target.GetPublicField("AllowMSAAList");
        }

        private bool _saveDirty = false;
        private void _saveGSISerializeData()
        {
            if (!_saveDirty) return;
            _target.SetPublicField("NameList", NameList);
            _target.SetPublicField("TypeList", TypeList);
            _target.SetPublicField("CullingMaskList", CullingMaskList);
            _target.SetPublicField("DepthList", DepthList);
            _target.SetPublicField("HasLensSettingList", HasLensSettingList);
            _target.SetPublicField("ClearFlagsList", ClearFlagsList);
            _target.SetPublicField("BackgroundColorList", BackgroundColorList);
            _target.SetPublicField("isOrthographicCameraList", isOrthographicCameraList);
            _target.SetPublicField("OrthographicSizeList", OrthographicSizeList);
            _target.SetPublicField("FieldOfViewList", FieldOfViewList);
            _target.SetPublicField("NearClipPlaneList", NearClipPlaneList);
            _target.SetPublicField("FarClipPlaneList", FarClipPlaneList);
            _target.SetPublicField("RenderingPathList", RenderingPathList);
            _target.SetPublicField("UseOcclusionCullingList", UseOcclusionCullingList);
            _target.SetPublicField("AllowHDRList", AllowHDRList);
            _target.SetPublicField("AllowMSAAList", AllowMSAAList);
            _saveDirty = false;
        }
        private void _addDesInfoToList(GCamGDesInfo desInfo)
        {
            NameList.Add(desInfo.name);
            TypeList.Add(desInfo.type);
            CullingMaskList.Add(desInfo.cullingMask);
            DepthList.Add(desInfo.depth);
            HasLensSettingList.Add(desInfo.lensSetting.init);
            ClearFlagsList.Add(desInfo.lensSetting.ClearFlags);
            BackgroundColorList.Add(desInfo.lensSetting.BackgroundColor);
            isOrthographicCameraList.Add(desInfo.lensSetting.isOrthographicCamera);
            OrthographicSizeList.Add(desInfo.lensSetting.OrthographicSize);
            FieldOfViewList.Add(desInfo.lensSetting.FieldOfView);
            NearClipPlaneList.Add(desInfo.lensSetting.NearClipPlane);
            FarClipPlaneList.Add(desInfo.lensSetting.FarClipPlane);
            RenderingPathList.Add(desInfo.lensSetting.RenderingPath);
            UseOcclusionCullingList.Add(desInfo.lensSetting.UseOcclusionCulling);
            AllowHDRList.Add(desInfo.lensSetting.AllowHDR);
            AllowMSAAList.Add(desInfo.lensSetting.AllowMSAA);
            _saveDirty = true;
        }

        private void _removeDesInfoFromList(int i)
        {
            if (i >= 0 && i < NameList.Count)
            {
                NameList.RemoveAt(i);
                TypeList.RemoveAt(i);
                CullingMaskList.RemoveAt(i);
                DepthList.RemoveAt(i);
                HasLensSettingList.RemoveAt(i);
                ClearFlagsList.RemoveAt(i);
                BackgroundColorList.RemoveAt(i);
                isOrthographicCameraList.RemoveAt(i);
                OrthographicSizeList.RemoveAt(i);
                FieldOfViewList.RemoveAt(i);
                NearClipPlaneList.RemoveAt(i);
                FarClipPlaneList.RemoveAt(i);
                RenderingPathList.RemoveAt(i);
                UseOcclusionCullingList.RemoveAt(i);
                AllowHDRList.RemoveAt(i);
                AllowMSAAList.RemoveAt(i);
                _saveDirty = true;
            }
        }

        private void __draw_camPramInList(int i)
        {
            //            GCamGDesInfo mainDesInfo = (GCamGDesInfo)_target.ref_GetField_Inst_Public("MainCamDesInfo");
            //            if (!mainDesInfo.init)
            //            {
            //                mainDesInfo = new GCamGDesInfo("MainCamera", SubGCamType.MainCamera, -1, 0);
            //            }

            if (NameList.Count == 0)
            {
                _addDesInfoToList(new GCamGDesInfo("MainCamera", SubGCamType.MainCamera, -1, 0, GCLensSetting.Default()));
            }

            //-----------------------------------------------------------------------------

            //name 
            //string name = (string)mainDesInfo.ref_GetField_Inst_Public("name");
            string nName = EditorGUILayout.TextField(new GUIContent("name"), NameList[i]);
            if (!nName.Equals(name))
            {
                //            mainDesInfo.ref_SetField_Inst_Public("name", nName);
                NameList[i] = nName;
                _saveDirty = true;
            }
            //type
            if (i == 0)
            {
                if (!TypeList[i].Equals(SubGCamType.MainCamera))
                {
                    TypeList[i] = SubGCamType.MainCamera;
                    EditorGUILayout.EnumPopup(new GUIContent("type"), TypeList[i]);
                    _saveDirty = true;
                }
            }
            else
            {
                SubGCamType camType = TypeList[i];
                SubGCamType nCamType = (SubGCamType)EditorGUILayout.EnumPopup(new GUIContent("type"), camType);
                if (!nCamType.Equals(camType))
                {
                    TypeList[i] = nCamType;
                    _saveDirty = true;
                }
            }

            //CullingMask
            int CullingMask = CullingMaskList[i];
            int nCullingMask = EditorGUILayout.MaskField(new GUIContent("cullingMask"), CullingMask, GraphicsCamUtility.GetMaskDisplayOption());

            if (!nCullingMask.Equals(CullingMask))
            {
                CullingMaskList[i] = nCullingMask;
                _saveDirty = true;
            }
            //depth
            float nDepth = EditorGUILayout.FloatField(new GUIContent("depth"), DepthList[i]);
            if (!nDepth.Equals(DepthList[i]))
            {
                DepthList[i] = nDepth;
                _saveDirty = true;
            }

            //-----------------------------------------------------------------------------

            //            GCLensSetting mainLensSetting = (GCLensSetting)mainDesInfo.ref_GetField_Inst_Public("lensSetting");
            //            if (!mainLensSetting.init)
            //            {
            //                mainLensSetting = GCLensSetting.Default();
            //            }

            if (HasLensSettingList[i])
            {

                //ClearFlags
                CameraClearFlags orgCF = ClearFlagsList[i];
                editorCameraClearFlags ClearFlags = (editorCameraClearFlags) (int) orgCF;
                editorCameraClearFlags nClearFlags =
                    (editorCameraClearFlags) EditorGUILayout.EnumPopup("ClearFlags", ClearFlags);
                if (nClearFlags != ClearFlags)
                {
                    CameraClearFlags nCF = (CameraClearFlags) (int) nClearFlags;
                    //            mainLensSetting.ref_SetField_Inst_Public("ClearFlags", nCF);
                    ClearFlagsList[i] = nCF;
                    _saveDirty = true;
                }
                if (nClearFlags == editorCameraClearFlags.SolidColor || nClearFlags == editorCameraClearFlags.Skybox)
                {
                    //            Color bgcolor = (Color) mainLensSetting.ref_GetField_Inst_Public("BackgroundColor");
                    Color nBgcolor = EditorGUILayout.ColorField("Background Color", BackgroundColorList[i]);
                    if (!nBgcolor.Equals(BackgroundColorList[i]))
                    {
                        //                mainLensSetting.ref_SetField_Inst_Public("BackgroundColor", nBgcolor);
                        BackgroundColorList[i] = nBgcolor;
                        _saveDirty = true;
                    }
                }
                //isOrthographicCamera
                //        bool isOCamera = (bool)mainLensSetting.ref_GetField_Inst_Public("isOrthographicCamera");
                bool isOCamera = isOrthographicCameraList[i];
                editorProjection eprojn = (editorProjection) (isOCamera ? 1 : 0);
                editorProjection nEprojn = (editorProjection) EditorGUILayout.EnumPopup("Projection", eprojn);
                if (nEprojn != eprojn)
                {
                    //            mainLensSetting.ref_SetField_Inst_Public("isOrthographicCamera", (nEprojn == editorProjection.Orthographic));
                    isOrthographicCameraList[i] = (nEprojn == editorProjection.Orthographic);
                    _saveDirty = true;
                }
                if (nEprojn == editorProjection.Orthographic)
                {
                    //OrthographicSize
                    //            float OrthographicSize = (float)mainLensSetting.ref_GetField_Inst_Public("OrthographicSize");
                    float OrthographicSize = OrthographicSizeList[i];
                    float nOrthographicSize = EditorGUILayout.FloatField(new GUIContent("Orthographic Size"),
                        OrthographicSize);
                    if (!nOrthographicSize.Equals(OrthographicSize))
                    {
                        //                mainLensSetting.ref_SetField_Inst_Public("OrthographicSize", nOrthographicSize);
                        OrthographicSizeList[i] = nOrthographicSize;
                        _saveDirty = true;
                    }
                }
                else
                {
                    //Perspective
                    //            float fov = (float)mainLensSetting.ref_GetField_Inst_Public("FieldOfView");
                    float fov = FieldOfViewList[i];
                    float nFov = EditorGUILayout.Slider("Field Of View", fov, 1f, 179f);
                    if (!nFov.Equals(fov))
                    {
                        //                mainLensSetting.ref_SetField_Inst_Public("FieldOfView", nFov);
                        FieldOfViewList[i] = nFov;
                        _saveDirty = true;
                    }
                }

                //near
                //        float NearClipPlane = (float)mainLensSetting.ref_GetField_Inst_Public("NearClipPlane");
                float NearClipPlane = NearClipPlaneList[i];
                float nNearClipPlane = EditorGUILayout.FloatField(new GUIContent("Near Clip Plane"), NearClipPlane);
                if (!nNearClipPlane.Equals(NearClipPlane))
                {
                    //            mainLensSetting.ref_SetField_Inst_Public("NearClipPlane", nNearClipPlane);
                    NearClipPlaneList[i] = nNearClipPlane;
                    _saveDirty = true;
                }

                //fear
                //        float FarClipPlane = (float)mainLensSetting.ref_GetField_Inst_Public("FarClipPlane");
                float FarClipPlane = FarClipPlaneList[i];
                float nFarClipPlane = EditorGUILayout.FloatField(new GUIContent("Far Clip Plane"), FarClipPlane);
                if (!nFarClipPlane.Equals(FarClipPlane))
                {
                    //            mainLensSetting.ref_SetField_Inst_Public("FarClipPlane", nFarClipPlane);
                    FarClipPlaneList[i] = nFarClipPlane;
                    _saveDirty = true;
                }

                RenderingPath RenderingPath = RenderingPathList[i];
                RenderingPath nRenderingPath =
                    (RenderingPath) EditorGUILayout.EnumPopup("Rendering Path", RenderingPath);
                if (!nRenderingPath.Equals(RenderingPath))
                {
                    RenderingPathList[i] = nRenderingPath;
                    _saveDirty = true;
                }

                //UseOcclusionCulling
                //        bool UseOcclusionCulling = (bool)mainLensSetting.ref_GetField_Inst_Public("UseOcclusionCulling");
                bool UseOcclusionCulling = UseOcclusionCullingList[i];
                bool nUseOcclusionCulling = EditorGUILayout.Toggle(new GUIContent("Use Occlusion Culling"),
                    UseOcclusionCulling);
                if (!nUseOcclusionCulling.Equals(UseOcclusionCulling))
                {
                    //            mainLensSetting.ref_SetField_Inst_Public("UseOcclusionCulling", nUseOcclusionCulling);
                    UseOcclusionCullingList[i] = nUseOcclusionCulling;
                    _saveDirty = true;
                }
                //AllowHDR
                //        bool AllowHDR = (bool)mainLensSetting.ref_GetField_Inst_Public("AllowHDR");
                bool AllowHDR = AllowHDRList[i];
                bool nAllowHDR = EditorGUILayout.Toggle(new GUIContent("Allow HDR"), AllowHDR);
                if (!nAllowHDR.Equals(AllowHDR))
                {
                    //            mainLensSetting.ref_SetField_Inst_Public("AllowHDR", nAllowHDR);
                    AllowHDRList[i] = nAllowHDR;
                    _saveDirty = true;
                }
                //AllowMSAA
                //        bool AllowMSAA = (bool)mainLensSetting.ref_GetField_Inst_Public("AllowMSAA");
                bool AllowMSAA = AllowMSAAList[i];
                bool nAllowMSAA = EditorGUILayout.Toggle(new GUIContent("Allow MSAA"), AllowMSAA);
                if (!nAllowMSAA.Equals(AllowMSAA))
                {
                    //            mainLensSetting.ref_SetField_Inst_Public("AllowMSAA", nAllowMSAA);
                    AllowMSAAList[i] = nAllowMSAA;
                    _saveDirty = true;
                }
            }

            GUILayout.Space(8);
        }

        private void _draw_mainCameraMParam()
        {

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            GUILayout.Label("主相机设置");
            GUILayout.EndVertical();

            GUILayout.Space(8);

            __draw_camPramInList(0);

            //        EditorGUILayout.PropertyField(serializedObject.FindProperty("MainCamDesInfo"), new GUIContent("Main Camera 描述"),true);

            GUILayout.Space(5);

            //        mainDesInfo.ref_SetField_Inst_Public("lensSetting", mainLensSetting);
//            mainDesInfo.lensSetting = mainLensSetting;
//            _target.ref_SetField_Inst_Public("MainCamDesInfo", mainDesInfo);

            _draw_cameraParamCoppyer();

            GUILayout.EndVertical();
        }

        private float _define_CoppyerBtnHeight = 30;

        private void _draw_cameraParamCoppyer()
        {
            GUILayout.BeginHorizontal("box");

            if (GUILayout.Button(new GUIContent("CopyParmFormSelectCamera", "复制选中相机参数"), GUILayout.Height(_define_CoppyerBtnHeight)))
            {
                Camera cam = _getSelectionCamera();
                if (cam)
                {

                    //=--------------
                    CullingMaskList[0] = cam.cullingMask;
                    DepthList[0] = cam.depth;
                    HasLensSettingList[0] = true;
                    ClearFlagsList[0] = cam.clearFlags;
                    BackgroundColorList[0] = cam.backgroundColor;
                    isOrthographicCameraList[0] = cam.orthographic;
                    OrthographicSizeList[0] = cam.orthographicSize;
                    FieldOfViewList[0] = cam.fieldOfView;
                    NearClipPlaneList[0] = cam.nearClipPlane;
                    FarClipPlaneList[0] = cam.farClipPlane;
                    RenderingPathList[0] = cam.renderingPath;
                    UseOcclusionCullingList[0] = cam.useOcclusionCulling;
                    AllowHDRList[0] = cam.allowHDR;
                    AllowMSAAList[0] = cam.allowMSAA;
                    _saveDirty = true;
                }
            }

            if (GUILayout.Button(new GUIContent("ApplyParmToSelectCamera", "应用参数到选中相机"), GUILayout.Height(_define_CoppyerBtnHeight)))
            {
                Camera cam = _getSelectionCamera();
                if (cam)
                {
                    //cullingMask
                    cam.cullingMask = CullingMaskList[0];
                    cam.depth = DepthList[0];
                    cam.clearFlags = ClearFlagsList[0];
                    cam.backgroundColor = BackgroundColorList[0];
                    cam.orthographic = isOrthographicCameraList[0];
                    cam.orthographicSize = OrthographicSizeList[0];
                    cam.fieldOfView = FieldOfViewList[0];
                    cam.nearClipPlane = NearClipPlaneList[0];
                    cam.farClipPlane = FarClipPlaneList[0];
                    cam.renderingPath = RenderingPathList[0];
                    cam.useOcclusionCulling = UseOcclusionCullingList[0];
                    cam.allowHDR = AllowHDRList[0];
                    cam.allowMSAA = AllowMSAAList[0];
                }
            }

            GUILayout.EndHorizontal();
        }

        private Camera _getSelectionCamera()
        {
            if (Selection.activeGameObject)
            {
                Camera cam = Selection.activeGameObject.GetComponent<Camera>();
                if (cam)
                {
                    return cam;
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "选择的对象不是相机.", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "没有选择对象.", "OK");
            }
            return null;
        }

        private List<GCamGDesInfo> _delSubDesInfos = new List<GCamGDesInfo>();
        private void _draw_subCameraMParam()
        {
            GUILayout.BeginVertical("box");

            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            GUILayout.Label("附加相机描述");
            GUILayout.EndVertical();

            GUILayout.Space(8);


            if (NameList.Count > 1)
            {
                for (int i = 1; i < NameList.Count; i++)
                {
                    _draw_subCamDesInfoItem(i);
                }


      
            }
            if (GUILayout.Button(new GUIContent("Add New Item", "添加新的子相机描述")))
            {
                GCamGDesInfo info = new GCamGDesInfo();
                info.name = "UnameSubCameraInfo";
                _addDesInfoToList(info);
            }
            //            _delSubDesInfos.Clear();
            //            List<GCamGDesInfo> subDesInfos = (List<GCamGDesInfo>)_target.ref_GetField_Inst_Public("SubCamGDesInfos");
            //            if (subDesInfos != null && subDesInfos.Count > 0)
            //            {
            //                for (int i = 0; i < subDesInfos.Count; i++)
            //                {
            //                    if (i > 0)
            //                    {
            //                        GUILayout.Space(2);
            //                    }
            //                    subDesInfos[i] = _draw_subCamDesInfoItem(subDesInfos[i]);
            //                }
            //            }
            //
            //            if (GUILayout.Button(new GUIContent("Add New Item", "添加新的子相机描述")))
            //            {
            //                GCamGDesInfo info = new GCamGDesInfo();
            //                info.name = "UnameSubCameraInfo";
            //                if (subDesInfos == null) subDesInfos = new List<GCamGDesInfo>();
            //                subDesInfos.Add(info);
            //            }
            //
            //            //EditorGUILayout.PropertyField(serializedObject.FindProperty("SubCamGDesInfos"), true);
            //
            //            for (int i = 0; i < _delSubDesInfos.Count; i++)
            //            {
            //                subDesInfos.Remove(_delSubDesInfos[i]);
            //            }
            //
            //            _target.ref_SetField_Inst_Public("SubCamGDesInfos", subDesInfos);

            GUILayout.Space(5);

            GUILayout.EndVertical();



        }

        private GCamGDesInfo _draw_subCamDesInfoItem(GCamGDesInfo info)
        {

            GUILayout.BeginHorizontal("box");

            GUILayout.BeginVertical();

            if (!info.init) info.init = true;

            //name 
            //        string name = (string)info.ref_GetField_Inst_Public("name");
            string name = info.name;
            string nName = EditorGUILayout.TextField(new GUIContent("Name"), name);
            if (!nName.Equals(name))
            {
                //            info.ref_SetField_Inst_Public("name", nName);
                info.name = nName;
            }
            //type
            //        SubGCamType type = (SubGCamType)info.ref_GetField_Inst_Public("type");
            SubGCamType type = info.type;
            SubGCamType nType = (SubGCamType)EditorGUILayout.EnumPopup(new GUIContent("Type"), type);
            if (!nType.Equals(type))
            {
                //            info.ref_SetField_Inst_Public("type", nType);
                info.type = nType;
            }
            //cullingMask
            int cullingMask = info.cullingMask;
            int nCullingMask = EditorGUILayout.MaskField(new GUIContent("CullingMask"), cullingMask, GraphicsCamUtility.GetMaskDisplayOption());

            if (!nCullingMask.Equals(cullingMask))
            {
                info.cullingMask = nCullingMask;
            }
            //        string cullingMask = info.cullingMask;
            //        if (cullingMask == null)
            //        {
            //            cullingMask = "";
            //            info.cullingMask = "";
            ////            info.ref_SetField_Inst_Public("cullingMask", cullingMask);
            //        }
            //        string nCullingMask = EditorGUILayout.TextField(new GUIContent("cullingMask"), cullingMask);
            //        if (!nCullingMask.Equals(cullingMask))
            //        {
            ////            info.ref_SetField_Inst_Public("cullingMask", nCullingMask);
            //            info.cullingMask = nCullingMask;
            //        }

            //depth
            //        int depth = (int)info.ref_GetField_Inst_Public("depth");
            float depth = info.depth;
            float nDepth = EditorGUILayout.FloatField(new GUIContent("depth"), depth);
            if (!nDepth.Equals(depth))
            {
                //            info.ref_SetField_Inst_Public("depth", nDepth);
                info.depth = nDepth;
            }

            GUILayout.EndVertical();
            //

            if (GUILayout.Button(new GUIContent("-"), GUILayout.Height(72), GUILayout.Width(50)))
            {
                _delSubDesInfos.Add(info);
            }

            GUILayout.EndHorizontal();


            return info;
        }

        private void _draw_subCamDesInfoItem(int i)
        {

            GUILayout.BeginHorizontal("box");

            GUILayout.BeginVertical();

            __draw_camPramInList(i);

            GUILayout.EndVertical();
            //

            if (GUILayout.Button(new GUIContent("-"), GUILayout.Height(72), GUILayout.Width(50)))
            {
                //_delSubDesInfos.Add(info);
                _removeDesInfoFromList(i);
            }

            GUILayout.EndHorizontal();


//            return info;
        }

    }
}


