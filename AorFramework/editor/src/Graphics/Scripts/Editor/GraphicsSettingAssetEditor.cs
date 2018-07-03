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
            _draw_baseParam();
            GUILayout.Space(8);
            _draw_mainCameraMParam();
            GUILayout.Space(8);
            _draw_subCameraMParam();
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

        private void _draw_mainCameraMParam()
        {

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            GUILayout.Label("主相机设置");
            GUILayout.EndVertical();

            GUILayout.Space(8);

            GCamGDesInfo mainDesInfo = (GCamGDesInfo)_target.GetPublicField("MainCamDesInfo");
            if (!mainDesInfo.init)
            {
                mainDesInfo = new GCamGDesInfo("MainCamera", SubGCamType.MainCamera, -1, 0);
            }

            //-----------------------------------------------------------------------------

            //name 
            //string name = (string)mainDesInfo.ref_GetField_Inst_Public("name");
            string nName = EditorGUILayout.TextField(new GUIContent("name"), mainDesInfo.name);
            if (!nName.Equals(name))
            {
                //            mainDesInfo.ref_SetField_Inst_Public("name", nName);
                mainDesInfo.name = nName;
            }
            //type
            if (!mainDesInfo.type.Equals(SubGCamType.MainCamera))
            {
                mainDesInfo.type = SubGCamType.MainCamera;
                EditorGUILayout.EnumPopup(new GUIContent("type"), mainDesInfo.type);
            }

            //CullingMask
            int CullingMask = mainDesInfo.cullingMask;
            int nCullingMask = EditorGUILayout.MaskField(new GUIContent("cullingMask"), CullingMask, GraphicsCamUtility.GetMaskDisplayOption());

            if (!nCullingMask.Equals(CullingMask))
            {
                mainDesInfo.cullingMask = nCullingMask;
            }
            //depth
            float nDepth = EditorGUILayout.FloatField(new GUIContent("depth"), mainDesInfo.depth);
            if (!nDepth.Equals(mainDesInfo.depth))
            {
                mainDesInfo.depth = nDepth;
            }

            //-----------------------------------------------------------------------------

            GCLensSetting mainLensSetting = (GCLensSetting)mainDesInfo.ref_GetField_Inst_Public("lensSetting");
            if (!mainLensSetting.init)
            {
                mainLensSetting = GCLensSetting.Default();
            }

            //ClearFlags
            CameraClearFlags orgCF = mainLensSetting.ClearFlags;
            editorCameraClearFlags ClearFlags = (editorCameraClearFlags)(int)orgCF;
            editorCameraClearFlags nClearFlags = (editorCameraClearFlags)EditorGUILayout.EnumPopup("ClearFlags", ClearFlags);
            if (nClearFlags != ClearFlags)
            {
                CameraClearFlags nCF = (CameraClearFlags)(int)nClearFlags;
                //            mainLensSetting.ref_SetField_Inst_Public("ClearFlags", nCF);
                mainLensSetting.ClearFlags = nCF;
            }
            if (nClearFlags == editorCameraClearFlags.SolidColor || nClearFlags == editorCameraClearFlags.Skybox)
            {
                //            Color bgcolor = (Color) mainLensSetting.ref_GetField_Inst_Public("BackgroundColor");
                Color nBgcolor = EditorGUILayout.ColorField("Background Color", mainLensSetting.BackgroundColor);
                if (!nBgcolor.Equals(mainLensSetting.BackgroundColor))
                {
                    //                mainLensSetting.ref_SetField_Inst_Public("BackgroundColor", nBgcolor);
                    mainLensSetting.BackgroundColor = nBgcolor;
                }
            }
            //isOrthographicCamera
            //        bool isOCamera = (bool)mainLensSetting.ref_GetField_Inst_Public("isOrthographicCamera");
            bool isOCamera = mainLensSetting.isOrthographicCamera;
            editorProjection eprojn = (editorProjection)(isOCamera ? 1 : 0);
            editorProjection nEprojn = (editorProjection)EditorGUILayout.EnumPopup("Projection", eprojn);
            if (nEprojn != eprojn)
            {
                //            mainLensSetting.ref_SetField_Inst_Public("isOrthographicCamera", (nEprojn == editorProjection.Orthographic));
                mainLensSetting.isOrthographicCamera = (nEprojn == editorProjection.Orthographic);
            }
            if (nEprojn == editorProjection.Orthographic)
            {
                //OrthographicSize
                //            float OrthographicSize = (float)mainLensSetting.ref_GetField_Inst_Public("OrthographicSize");
                float OrthographicSize = mainLensSetting.OrthographicSize;
                float nOrthographicSize = EditorGUILayout.FloatField(new GUIContent("Orthographic Size"), OrthographicSize);
                if (!nOrthographicSize.Equals(OrthographicSize))
                {
                    //                mainLensSetting.ref_SetField_Inst_Public("OrthographicSize", nOrthographicSize);
                    mainLensSetting.OrthographicSize = nOrthographicSize;
                }
            }
            else
            {
                //Perspective
                //            float fov = (float)mainLensSetting.ref_GetField_Inst_Public("FieldOfView");
                float fov = mainLensSetting.FieldOfView;
                float nFov = EditorGUILayout.Slider("Field Of View", fov, 1f, 179f);
                if (!nFov.Equals(fov))
                {
                    //                mainLensSetting.ref_SetField_Inst_Public("FieldOfView", nFov);
                    mainLensSetting.FieldOfView = nFov;
                }
            }

            //near
            //        float NearClipPlane = (float)mainLensSetting.ref_GetField_Inst_Public("NearClipPlane");
            float NearClipPlane = mainLensSetting.NearClipPlane;
            float nNearClipPlane = EditorGUILayout.FloatField(new GUIContent("Near Clip Plane"), NearClipPlane);
            if (!nNearClipPlane.Equals(NearClipPlane))
            {
                //            mainLensSetting.ref_SetField_Inst_Public("NearClipPlane", nNearClipPlane);
                mainLensSetting.NearClipPlane = nNearClipPlane;
            }

            //fear
            //        float FarClipPlane = (float)mainLensSetting.ref_GetField_Inst_Public("FarClipPlane");
            float FarClipPlane = mainLensSetting.FarClipPlane;
            float nFarClipPlane = EditorGUILayout.FloatField(new GUIContent("Far Clip Plane"), FarClipPlane);
            if (!nFarClipPlane.Equals(FarClipPlane))
            {
                //            mainLensSetting.ref_SetField_Inst_Public("FarClipPlane", nFarClipPlane);
                mainLensSetting.FarClipPlane = nFarClipPlane;
            }

            RenderingPath RenderingPath = mainLensSetting.RenderingPath;
            RenderingPath nRenderingPath = (RenderingPath)EditorGUILayout.EnumPopup("Rendering Path", RenderingPath);
            if (!nRenderingPath.Equals(RenderingPath))
            {
                mainLensSetting.RenderingPath = nRenderingPath;
            }

            //UseOcclusionCulling
            //        bool UseOcclusionCulling = (bool)mainLensSetting.ref_GetField_Inst_Public("UseOcclusionCulling");
            bool UseOcclusionCulling = mainLensSetting.UseOcclusionCulling;
            bool nUseOcclusionCulling = EditorGUILayout.Toggle(new GUIContent("Use Occlusion Culling"), UseOcclusionCulling);
            if (!nUseOcclusionCulling.Equals(UseOcclusionCulling))
            {
                //            mainLensSetting.ref_SetField_Inst_Public("UseOcclusionCulling", nUseOcclusionCulling);
                mainLensSetting.UseOcclusionCulling = nUseOcclusionCulling;
            }
            //AllowHDR
            //        bool AllowHDR = (bool)mainLensSetting.ref_GetField_Inst_Public("AllowHDR");
            bool AllowHDR = mainLensSetting.AllowHDR;
            bool nAllowHDR = EditorGUILayout.Toggle(new GUIContent("Allow HDR"), AllowHDR);
            if (!nAllowHDR.Equals(AllowHDR))
            {
                //            mainLensSetting.ref_SetField_Inst_Public("AllowHDR", nAllowHDR);
                mainLensSetting.AllowHDR = nAllowHDR;
            }
            //AllowMSAA
            //        bool AllowMSAA = (bool)mainLensSetting.ref_GetField_Inst_Public("AllowMSAA");
            bool AllowMSAA = mainLensSetting.AllowMSAA;
            bool nAllowMSAA = EditorGUILayout.Toggle(new GUIContent("Allow MSAA"), AllowMSAA);
            if (!nAllowMSAA.Equals(AllowMSAA))
            {
                //            mainLensSetting.ref_SetField_Inst_Public("AllowMSAA", nAllowMSAA);
                mainLensSetting.AllowMSAA = nAllowMSAA;
            }
            GUILayout.Space(8);

            //        EditorGUILayout.PropertyField(serializedObject.FindProperty("MainCamDesInfo"), new GUIContent("Main Camera 描述"),true);

            GUILayout.Space(5);

            //        mainDesInfo.ref_SetField_Inst_Public("lensSetting", mainLensSetting);
            mainDesInfo.lensSetting = mainLensSetting;
            _target.ref_SetField_Inst_Public("MainCamDesInfo", mainDesInfo);

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

                    GCLensSetting cgLensSetting = new GCLensSetting(cam.clearFlags, cam.backgroundColor,
                        cam.orthographic, cam.orthographicSize, cam.fieldOfView,
                        cam.nearClipPlane, cam.farClipPlane,
                        cam.renderingPath,
                        cam.useOcclusionCulling, cam.allowHDR, cam.allowMSAA
                        );

                    GCamGDesInfo mainDesInfo = (GCamGDesInfo)_target.ref_GetField_Inst_Public("MainCamDesInfo");
                    mainDesInfo.lensSetting = cgLensSetting;
                    mainDesInfo.depth = cam.depth;

                    mainDesInfo.cullingMask = cam.cullingMask;

                    _target.ref_SetField_Inst_Public("MainCamDesInfo", mainDesInfo);
                }
            }

            if (GUILayout.Button(new GUIContent("ApplyParmToSelectCamera", "应用参数到选中相机"), GUILayout.Height(_define_CoppyerBtnHeight)))
            {
                Camera cam = _getSelectionCamera();
                if (cam)
                {


                    GCamGDesInfo mainDesInfo = (GCamGDesInfo)_target.ref_GetField_Inst_Public("MainCamDesInfo");

                    //cullingMask
                    cam.cullingMask = mainDesInfo.cullingMask;
                    cam.depth = mainDesInfo.depth;
                    cam.clearFlags = mainDesInfo.lensSetting.ClearFlags;
                    cam.backgroundColor = mainDesInfo.lensSetting.BackgroundColor;
                    cam.orthographic = mainDesInfo.lensSetting.isOrthographicCamera;
                    cam.orthographicSize = mainDesInfo.lensSetting.OrthographicSize;
                    cam.fieldOfView = mainDesInfo.lensSetting.FieldOfView;
                    cam.nearClipPlane = mainDesInfo.lensSetting.NearClipPlane;
                    cam.farClipPlane = mainDesInfo.lensSetting.FarClipPlane;
                    cam.renderingPath = mainDesInfo.lensSetting.RenderingPath;
                    cam.useOcclusionCulling = mainDesInfo.lensSetting.UseOcclusionCulling;
                    cam.allowHDR = mainDesInfo.lensSetting.AllowHDR;
                    cam.allowMSAA = mainDesInfo.lensSetting.AllowMSAA;
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

            _delSubDesInfos.Clear();
            List<GCamGDesInfo> subDesInfos = (List<GCamGDesInfo>)_target.ref_GetField_Inst_Public("SubCamGDesInfos");
            if (subDesInfos != null && subDesInfos.Count > 0)
            {
                for (int i = 0; i < subDesInfos.Count; i++)
                {
                    if (i > 0)
                    {
                        GUILayout.Space(2);
                    }
                    subDesInfos[i] = _draw_subCamDesInfoItem(subDesInfos[i]);
                }
            }

            if (GUILayout.Button(new GUIContent("Add New Item", "添加新的子相机描述")))
            {
                GCamGDesInfo info = new GCamGDesInfo();
                info.name = "UnameSubCameraInfo";
                if (subDesInfos == null) subDesInfos = new List<GCamGDesInfo>();
                subDesInfos.Add(info);
            }

            //EditorGUILayout.PropertyField(serializedObject.FindProperty("SubCamGDesInfos"), true);

            for (int i = 0; i < _delSubDesInfos.Count; i++)
            {
                subDesInfos.Remove(_delSubDesInfos[i]);
            }

            _target.ref_SetField_Inst_Public("SubCamGDesInfos", subDesInfos);

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

    }
}


