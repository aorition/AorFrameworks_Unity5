using System;
using System.Collections.Generic;
using AorBaseUtility.Extends;
using Framework.Editor;
using Framework.Extends;
using Framework.Graphic.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Editor
{

    [CustomEditor(typeof(UIManagerSettingAsset))]
    public class UIManagerSettingAssetEditor : UnityEditor.Editor
    {

        private static GUIStyle _titleStyle;
        private static GUIStyle TitleStyle
        {
            get
            {
                if (_titleStyle == null)
                {
                    _titleStyle = GUI.skin.GetStyle("Label").Clone();
                    _titleStyle.fontSize = 14;
                    _titleStyle.normal.textColor = Color.yellow;
                }
                return _titleStyle;
            }
        }

        private static string[] TargetDisplayLabels = {"Display 1", "Display 2", "Display 3", "Display 4", "Display 5", "Display 6", "Display 7", "Display 8", };
        private static int[] TargetDisplayValues = {0,1,2,3,4,5,6,7};
        private static GUIContent[] ProjectionLabels = {new GUIContent("Perspective"), new GUIContent("Orthographic")};

        [MenuItem("Assets/UIManagerAssets/UIManagerSettingAsset")]
        public static void CreateAsset()
        {
            EditorPlusMethods.CreateAssetFile<UIManagerSettingAsset>("unameUIManagerSettingAsset");
        }

        private UIManagerSettingAsset _target;

        private void OnEnable()
        {
            _target = target as UIManagerSettingAsset;
        }

        private Vector2 _scpos = Vector2.zero;

        public override void OnInspectorGUI()
        {

            bool isInfoDirty = false;
            bool isLayersDirty = false;

            List<UICanvasInfo> InfoList = (List<UICanvasInfo>)_target.GetNonPublicField("m_InfoList");
            List<UILayerGroup> LayerList = (List<UILayerGroup>) _target.GetNonPublicField("m_LayerList");

            if (InfoList == null || InfoList.Count == 0)
            {
                //init
                InfoList = new List<UICanvasInfo>();
                InfoList.Add(UICanvasInfo.Default());

                if (LayerList == null)
                {
                    LayerList = new List<UILayerGroup>();
                }

                UILayerGroup uiLayerGroup = new UILayerGroup(true);
                uiLayerGroup.Add(UILayer.Default());
                LayerList.Add(uiLayerGroup);

                _target.SetNonPublicField("_isInit", true);

                isInfoDirty = true;
                isLayersDirty = true;
            }

            List<UICanvasInfo> nInfoList = new List<UICanvasInfo>();
            List<UILayerGroup> nLayerList = new List<UILayerGroup>();

            _scpos = GUILayout.BeginScrollView(_scpos);
            for (int i = 0; i < InfoList.Count; i++)
            {
                if (i > 0)
                {
                    GUILayout.Space(5);
                }

                bool infoDirty = false;
                bool layersDirty = false;
                Draw_UICanvasInfo_UI(i, InfoList[i],ref nInfoList, ref infoDirty, LayerList, ref nLayerList, ref layersDirty,(InfoList.Count > 1));
                if (infoDirty)
                {
                    isInfoDirty = true;
                }
                if (layersDirty)
                {
                    isLayersDirty = true;
                }
            }

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.color = Color.yellow;
            if (GUILayout.Button(new GUIContent("Canvas +", "Add New Canvas Setting"), GUILayout.Width(Mathf.Max(Screen.width * 0.5f,128)),
                GUILayout.Height(32)))
            {
                if (EditorUtility.DisplayDialog("提示", "正在创建新Canvas Setting ?", "确定", "取消"))
                {
                    nInfoList.Add(UICanvasInfo.Default("UnameCanvas"));

                    nLayerList.Add(new UILayerGroup(true));
                    nLayerList[nLayerList.Count - 1].Add(UILayer.Default());

                    isInfoDirty = true;
                    isLayersDirty = true;
                }
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();

            if (isInfoDirty)
            {
                _target.ref_SetField_Inst_NonPublic("m_InfoList", nInfoList);
            }

            if (isLayersDirty)
            {
                _target.ref_SetField_Inst_NonPublic("m_LayerList", nLayerList);
            }

            #region *** 按钮-> <立即写入修改数据到文件> :: 建议所有.Asset文件的Editor都配备此段代码
            EditorPlusMethods.Draw_AssetFileApplyImmediateUI(target);
            #endregion

        }

        private void Draw_UICanvasInfo_UI(int index, UICanvasInfo info, ref List<UICanvasInfo> nInfoList, ref bool isInfoDirty, List<UILayerGroup> LayerList, ref List<UILayerGroup> nLayerList, ref bool isLayersDirty, bool delButtom)
        {

            GUILayout.BeginVertical("box");

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);
            GUILayout.Label("Canvas Setting ::", TitleStyle);
            GUILayout.Space(8);

            string name = info.Name;
            string nName = EditorGUILayout.TextField("Name", name);
            if(nName != name) isInfoDirty = true;

            RenderMode RenderMode = info.RenderMode;
            RenderMode nRenderMode = (RenderMode) EditorGUILayout.EnumPopup("RenderMode", RenderMode);
            if (nRenderMode != RenderMode) isInfoDirty = true;

            bool PixelPerfect = info.PixelPerfect;
            bool nPixelPerfect = PixelPerfect;

            int SortOrder = info.SortOrder;
            int nSortOrder;

            int TargetDisplay = info.TargetDisplay;
            int nTargetDisplay = TargetDisplay;

            string SortingLayerName = info.SortingLayerName;
            string nSortingLayerName = SortingLayerName;

            CameraStructInfo CameraInfo = info.CameraInfo;
            if (!CameraInfo.isInit && (nRenderMode == RenderMode.ScreenSpaceCamera || nRenderMode == RenderMode.WorldSpace))
            {
                CameraInfo = CameraStructInfo.DefaultUI();
                isInfoDirty = true;
            }else if (CameraInfo.isInit && nRenderMode == RenderMode.ScreenSpaceOverlay)
            {
                CameraInfo = new CameraStructInfo();
                isInfoDirty = true;
            }

            CameraStructInfo nCameraInfo = CameraInfo;
            bool CameraInfoDirty = false;

            float PlaneDistance = info.PlaneDistance;
            float nPlaneDistance = PlaneDistance;

            switch (nRenderMode)
            {
                case RenderMode.ScreenSpaceCamera:
                    nPixelPerfect = EditorGUILayout.Toggle("PixelPerfect", PixelPerfect);
                    if (nPixelPerfect != PixelPerfect) isInfoDirty = true;

                    nPlaneDistance = EditorGUILayout.FloatField("Plane Distance", PlaneDistance);
                    if (!nPlaneDistance.Equals(PlaneDistance)) isInfoDirty = true;

                    nCameraInfo = Draw_CameraStructInfo_UI(CameraInfo, "Render Camera", ref CameraInfoDirty);
                    if (CameraInfoDirty) isInfoDirty = true;

                    nSortOrder = EditorGUILayout.IntField("Order in Layer", SortOrder);
                    if (nSortOrder != SortOrder) isInfoDirty = true;

                    break;
                case RenderMode.WorldSpace:

                    nCameraInfo = Draw_CameraStructInfo_UI(CameraInfo, "Event Camera", ref CameraInfoDirty);
                    if (CameraInfoDirty) isInfoDirty = true;

                    SortingLayer[] layers = SortingLayer.layers;
                    GUIContent[] SortingLayerNames = new GUIContent[layers.Length + 1];
                    SortingLayerNames[0] = new GUIContent("Default");
                    for (int i = 0; i < layers.Length; i++)
                    {
                        SortingLayerNames[i+1] = new GUIContent(layers[i].name);
                    }
                    int stId = 0;
                    for (int i = 0; i < SortingLayerNames.Length; i++)
                    {
                        if (SortingLayerName == SortingLayerNames[i].text)
                        {
                            stId = i;
                            break;
                        }
                    }
                    nSortingLayerName = SortingLayerNames[EditorGUILayout.Popup(new GUIContent("Sorting Layer"), stId, SortingLayerNames)].text;
                    if(nSortingLayerName != SortingLayerName) isInfoDirty = true;

                    nSortOrder = EditorGUILayout.IntField("Order in Layer", SortOrder);
                    if (nSortOrder != SortOrder) isInfoDirty = true;

                    break;
                default: //Overlay
                    nPixelPerfect = EditorGUILayout.Toggle("PixelPerfect", PixelPerfect);
                    if (nPixelPerfect != PixelPerfect) isInfoDirty = true;

                    nSortOrder = EditorGUILayout.IntField("SortOrder", SortOrder);
                    if (nSortOrder != SortOrder) isInfoDirty = true;

                    nTargetDisplay = EditorGUILayout.IntPopup(TargetDisplay, TargetDisplayLabels, TargetDisplayValues);
                    if (nTargetDisplay != TargetDisplay) isInfoDirty = true;

                    break;
            }

            AdditionalCanvasShaderChannels AdditionalShaderChannels = info.AdditionalShaderChannels;
            AdditionalCanvasShaderChannels nAdditionalShaderChannels = (AdditionalCanvasShaderChannels)EditorGUILayout.EnumPopup("AdditionalShaderChannels", AdditionalShaderChannels);
            if (nAdditionalShaderChannels != AdditionalShaderChannels) isInfoDirty = true;

            GUILayout.EndVertical();

            //=========================================================================

            GUILayout.Space(8);

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);
            GUILayout.Label("Canvas Scaler Setting ::", TitleStyle);
            GUILayout.Space(8);

            CanvasScaler.ScaleMode ScaleMode = info.ScaleMode;
            CanvasScaler.ScaleMode nScaleMode = ScaleMode;
            if (nRenderMode == RenderMode.WorldSpace)
            {
                nScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            }
            else
            {
                nScaleMode = (CanvasScaler.ScaleMode) EditorGUILayout.EnumPopup("UI Scale Mode", ScaleMode);
            }
            if(nScaleMode != ScaleMode) isInfoDirty = true;

            float ScaleFactor = info.ScaleFactor;
            float nScaleFactor = ScaleFactor;

            Vector2 ReferenceResolution = info.ReferenceResolution;
            Vector2 nReferenceResolution = ReferenceResolution;

            CanvasScaler.ScreenMatchMode ScreenMatchMode = info.ScreenMatchMode;
            CanvasScaler.ScreenMatchMode nScreenMatchMode = ScreenMatchMode;

            float MatchWidthOrHeight = info.MatchWidthOrHeight;
            float nMatchWidthOrHeight = MatchWidthOrHeight;

            CanvasScaler.Unit PhysicalUnit = info.PhysicalUnit;
            CanvasScaler.Unit nPhysicalUnit = PhysicalUnit;

            float FallbackScreenDPI = info.FallbackScreenDPI;
            float nFallbackScreenDPI = FallbackScreenDPI;

            float DefaultSpriteDPI = info.DefaultSpriteDPI;
            float nDefaultSpriteDPI = DefaultSpriteDPI;

            switch (nScaleMode)
            {
                case CanvasScaler.ScaleMode.ScaleWithScreenSize:
                    nReferenceResolution = EditorGUILayout.Vector2Field("Reference Resolution", ReferenceResolution);
                    if(nReferenceResolution != ReferenceResolution) isInfoDirty = true;

                    nScreenMatchMode = (CanvasScaler.ScreenMatchMode)EditorGUILayout.EnumPopup("Screen Match Mode", ScreenMatchMode);
                    if(nScreenMatchMode != ScreenMatchMode) isInfoDirty = true;

                    if (nScreenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
                    {
                        nMatchWidthOrHeight = EditorGUILayout.Slider(new GUIContent("Match"), MatchWidthOrHeight, 0, 1);
                        if(!nMatchWidthOrHeight.Equals(MatchWidthOrHeight)) isInfoDirty = true;
                    }

                    break;
                case CanvasScaler.ScaleMode.ConstantPhysicalSize:

                    nPhysicalUnit = (CanvasScaler.Unit)EditorGUILayout.EnumPopup(new GUIContent("Physical Unit"), PhysicalUnit);
                    if(nPhysicalUnit != PhysicalUnit) isInfoDirty = true;

                    nFallbackScreenDPI = EditorGUILayout.FloatField("Fallback Screen DPI", FallbackScreenDPI);
                    if(!nFallbackScreenDPI.Equals(FallbackScreenDPI)) isInfoDirty = true;

                    nDefaultSpriteDPI = EditorGUILayout.FloatField("Default Sprite DPI", DefaultSpriteDPI);
                    if (!nDefaultSpriteDPI.Equals(DefaultSpriteDPI)) isInfoDirty = true;

                    break;
                default: //ConstantPixelSize
                    nScaleFactor = EditorGUILayout.FloatField("Scale Factor", ScaleFactor);
                    if (nScaleFactor != ScaleFactor) isInfoDirty = true;
                    break;
            }

            float ReferencePixelsPerUnit = info.ReferencePixelsPerUnit;
            float nReferencePixelsPerUnit = EditorGUILayout.FloatField("Reference Pixels Per Unit", ReferencePixelsPerUnit);
            if(!nReferencePixelsPerUnit.Equals(ReferencePixelsPerUnit)) isInfoDirty = true;
            
            GUILayout.EndVertical();

            //========================================

            GUILayout.Space(8);

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);
            GUILayout.Label("Graphic Raycaster Setting ::", TitleStyle);
            GUILayout.Space(8);

            bool IgnoreReversedGraphic = info.IgnoreReversedGraphic;
            bool nIgnoreReversedGraphic = EditorGUILayout.Toggle("Ignore Reversed Graphic", IgnoreReversedGraphic);
            if(nIgnoreReversedGraphic != IgnoreReversedGraphic) isInfoDirty = true;

            GraphicRaycaster.BlockingObjects BlockingObjects = info.BlockingObjects;
            GraphicRaycaster.BlockingObjects nBlockingObjects = (GraphicRaycaster.BlockingObjects)EditorGUILayout.EnumPopup("Blocking Objects", BlockingObjects);
            if (nBlockingObjects != BlockingObjects) isInfoDirty = true;

            int BlockingMask = info.BlockingMask;
            int nBlockingMask = EditorGUILayout.MaskField(new GUIContent("Blocking Mask"), BlockingMask, GraphicsCamUtility.GetMaskDisplayOption());
            if (nBlockingMask != BlockingMask) isInfoDirty = true;

            GUILayout.EndVertical();

            GUILayout.Space(8);

            GUILayout.BeginVertical("box");

            GUILayout.Space(5);
            GUILayout.Label("Layers Setting ::", TitleStyle);
            GUILayout.Space(8);

            UILayerGroup oLayers = LayerList[index];
            bool islayerDirty = false;
            UILayerGroup nLayers = Draw_UILayers_UI(oLayers, ref islayerDirty);
            if (islayerDirty) isLayersDirty = true;

            GUILayout.EndVertical();

            bool isDel = false;
            //delbuttom
            if (delButtom)
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.color = Color.red;
                if (GUILayout.Button(new GUIContent("Canvas -", "RemoveTheCanvasSetting"), GUILayout.Width(Mathf.Max(Screen.width * 0.25f, 64)),
                    GUILayout.Height(32)))
                {
                    if (EditorUtility.DisplayDialog("提示", "你确定要删除此条CanvasSetting?", "确定", "取消"))
                    {
                        isDel = true;
                        isInfoDirty = true;
                    }
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            if (!isDel)
            {
                nInfoList.Add(new UICanvasInfo(nName, nRenderMode, nPixelPerfect, nSortingLayerName, nSortOrder, nTargetDisplay,
                    nAdditionalShaderChannels, nCameraInfo, nPlaneDistance, nScaleMode, nScaleFactor, nReferencePixelsPerUnit,
                    nReferenceResolution, nScreenMatchMode, nMatchWidthOrHeight, nPhysicalUnit, nFallbackScreenDPI,
                    nDefaultSpriteDPI, nIgnoreReversedGraphic, nBlockingObjects, nBlockingMask));

                nLayerList.Add(nLayers);
            }

        }

        private CameraStructInfo Draw_CameraStructInfo_UI(CameraStructInfo CameraInfo, string cameraLabel, ref bool isDirty)
        {

            GUILayout.Space(8);

            GUILayout.BeginVertical("box");

            GUILayout.Space(8);
            GUILayout.Label(cameraLabel + " :", TitleStyle);
            GUILayout.Space(8);

            CameraClearFlags ClearFlags = CameraInfo.ClearFlags;
            CameraClearFlags nClearFlags = (CameraClearFlags) EditorGUILayout.EnumPopup("Clear Flags", ClearFlags);
            if (nClearFlags != ClearFlags) isDirty = true;

            Color Background = CameraInfo.Background;
            Color nBackground = Background;
            if (nClearFlags == CameraClearFlags.Skybox || nClearFlags == CameraClearFlags.SolidColor)
            {
                nBackground = EditorGUILayout.ColorField("Background", Background);
                if(nBackground != Background) isDirty = true;
            }

            int CullingMask = CameraInfo.CullingMask;
            int nCullingMask = EditorGUILayout.MaskField(new GUIContent("CullingMask"), CullingMask, GraphicsCamUtility.GetMaskDisplayOption());
            if(nCullingMask != CullingMask) isDirty = true;

            bool Orthographic = CameraInfo.Orthographic;
            bool nOrthographic = Orthographic;
            int projection = Orthographic ? 1 : 0;
            int nProjection = EditorGUILayout.Popup(new GUIContent("Projection"), projection, ProjectionLabels);
            if (nProjection != projection)
            {
                nOrthographic = nProjection == 1;
                isDirty = true;
            }

            float OrthographicSize = CameraInfo.OrthographicSize;
            float nOrthographicSize = OrthographicSize;

            float FieldOfView = CameraInfo.FieldOfView;
            float nFieldOfView = FieldOfView;

            if (nOrthographic)
            {
                nOrthographicSize = EditorGUILayout.FloatField("Size", OrthographicSize);
                if(!nOrthographicSize.Equals(OrthographicSize)) isDirty = true;
            }
            else
            {
                nFieldOfView = EditorGUILayout.Slider("Field of View", FieldOfView, 1, 179);
                if (!nFieldOfView.Equals(FieldOfView)) isDirty = true;
            }

            float NearClipPlane = CameraInfo.NearClipPlane;
            float nNearClipPlane = EditorGUILayout.FloatField("Near Clip Plane", NearClipPlane);
            if(!nNearClipPlane.Equals(NearClipPlane)) isDirty = true;

            float FarClipPlane = CameraInfo.FarClipPlane;
            float nFarClipPlane = EditorGUILayout.FloatField("Far Clip Plane", FarClipPlane);
            if (!nFarClipPlane.Equals(FarClipPlane)) isDirty = true;

            Rect ViewportRect = CameraInfo.ViewportRect;
            Rect nViewportRect = EditorGUILayout.RectField("Viewport Rect", ViewportRect);
            if (!nViewportRect.Equals(ViewportRect)) isDirty = true;

            float Depth = CameraInfo.Depth;
            float nDepth = EditorGUILayout.FloatField("Depth", Depth);
            if (!nDepth.Equals(Depth)) isDirty = true;

            RenderingPath RenderingPath = CameraInfo.RenderingPath;
            RenderingPath nRenderingPath = (RenderingPath) EditorGUILayout.EnumPopup("Rendering Path", RenderingPath);
            if (!nRenderingPath.Equals(RenderingPath)) isDirty = true;

            RenderTexture TargetTexture = CameraInfo.TargetTexture;
            RenderTexture nTargetTexture = (RenderTexture)EditorGUILayout.ObjectField("Target Texture", TargetTexture,typeof(UnityEngine.Object));
            if(nTargetTexture != null && !nTargetTexture.Equals(TargetTexture)) isDirty = true;

            bool OcclusionCulling = CameraInfo.OcclusionCulling;
            bool nOcclusionCulling = EditorGUILayout.Toggle("Occlusion Culling", OcclusionCulling);
            if (!nOcclusionCulling.Equals(OcclusionCulling)) isDirty = true;

            bool AllowHDR = CameraInfo.AllowHDR;
            bool nAllowHDR = EditorGUILayout.Toggle("Allow HDR", AllowHDR);
            if (!nAllowHDR.Equals(AllowHDR)) isDirty = true;

            bool AllowMSAA = CameraInfo.AllowMSAA;
            bool nAllowMSAA = EditorGUILayout.Toggle("Allow MSAA", AllowMSAA);
            if (!nAllowMSAA.Equals(AllowMSAA)) isDirty = true;

            GUILayout.EndVertical();

            GUILayout.Space(8);

            return new CameraStructInfo(nClearFlags, nBackground, nCullingMask, nOrthographic, nOrthographicSize,
                nFieldOfView, nNearClipPlane, nFarClipPlane, nViewportRect, nDepth, nRenderingPath, nTargetTexture,
                nOcclusionCulling, nAllowHDR, nAllowMSAA);

        }

        private UILayerGroup Draw_UILayers_UI(UILayerGroup list, ref bool isDirty)
        {

            UILayerGroup nlist = new UILayerGroup(true);
            for (int i = 0; i < list.Count; i++)
            {

                if (i > 0)
                {
                    GUILayout.Space(5);
                }

                bool isDel = false;

                UILayer o = list[i];

                GUILayout.BeginVertical("box");

                string name = o.Name;
                string nName = EditorGUILayout.TextField("LayerName", name);
                if (nName != name) isDirty = true;

                UILayerPovitState state = o.PovitState;
                UILayerPovitState ntState = (UILayerPovitState) EditorGUILayout.EnumPopup("Povit State", state);
                if (ntState != state) isDirty = true;

                int siblingIndex = o.SiblingIndex;
                int nSiblingIndex = EditorGUILayout.IntField("SiblingIndex", siblingIndex);
                if (nSiblingIndex != siblingIndex) isDirty = true;

                /////////////////////////

                bool IgnoreReversedGraphics = o.IgnoreReversedGraphics;
                bool nIgnoreReversedGraphics = IgnoreReversedGraphics;

                GraphicRaycaster.BlockingObjects BlockingObjects = o.BlockingObjects;
                GraphicRaycaster.BlockingObjects nBlockingObjects = BlockingObjects;

                int BlockingMask = o.BlockingMask;
                int nBlockingMask = BlockingMask;

                /////////////////////////////////

                bool useInnerCanvas = o.UseInnerCanvas;
                bool nUseInnerCanvas = EditorGUILayout.Toggle("Use InnerCanvas", useInnerCanvas);
                if (nUseInnerCanvas != useInnerCanvas) isDirty = true;

                bool overrideSorting = o.OverrideSorting;
                bool nOverrideSorting = overrideSorting;

                int sortOrder = o.SortOrder;
                int nSortOrder = sortOrder;

                if (nUseInnerCanvas)
                {
                    nOverrideSorting = EditorGUILayout.Toggle("Override Sorting", overrideSorting);
                    if (nOverrideSorting != overrideSorting) isDirty = true;

                    nSortOrder = EditorGUILayout.IntField("Sort Order", sortOrder);
                    if (nSortOrder != sortOrder) isDirty = true;

                    //-----------------

                    GUILayout.Space(5);

                    nIgnoreReversedGraphics = EditorGUILayout.Toggle("Ignore Reversed Graphics", IgnoreReversedGraphics);
                    if (nIgnoreReversedGraphics != IgnoreReversedGraphics) isDirty = true;

                    nBlockingObjects = (GraphicRaycaster.BlockingObjects)EditorGUILayout.EnumPopup("Blocking Objects", BlockingObjects);
                    if (nBlockingObjects != BlockingObjects) isDirty = true;

                    nBlockingMask = EditorGUILayout.MaskField(new GUIContent("Blocking Mask"), BlockingMask, GraphicsCamUtility.GetMaskDisplayOption());
                    if (nBlockingMask != BlockingMask) isDirty = true;

                    GUILayout.Space(8);
                }

                bool useCanvasGroup = o.UseCanvasGroup;
                bool nUseCanvasGroup = EditorGUILayout.Toggle("Use CanvasGroup", useCanvasGroup);
                if (nUseCanvasGroup != useCanvasGroup) isDirty = true;

                float Alpha = o.Alpha;
                float nAlpha = Alpha;

                bool Interactable = o.Interactable;
                bool nInteractable = Interactable;

                bool BlocksRaycasts = o.BlocksRaycasts;
                bool nBlocksRaycasts = BlocksRaycasts;

                bool IgnoreParentGroups = o.IgnoreParentGroups;
                bool nIgnoreParentGroups = IgnoreParentGroups;

                if (nUseCanvasGroup)
                {
                    nAlpha = EditorGUILayout.Slider(new GUIContent("Alpha"), Alpha, 0, 1);
                    if(!nAlpha.Equals(Alpha)) isDirty = true;

                    nInteractable = EditorGUILayout.Toggle("Interactable", Interactable);
                    if (nInteractable != Interactable) isDirty = true;

                    nBlocksRaycasts = EditorGUILayout.Toggle("Blocks Raycasts", BlocksRaycasts);
                    if (nBlocksRaycasts != BlocksRaycasts) isDirty = true;

                    nIgnoreParentGroups = EditorGUILayout.Toggle("Ignore Parent Groups", IgnoreParentGroups);
                    if (nIgnoreParentGroups != IgnoreParentGroups) isDirty = true;
                }

                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.color = Color.red;
                if (GUILayout.Button(new GUIContent("Layer -", "Remove The Layer Setting"), GUILayout.Width(Mathf.Max(Screen.width * 0.125f, 32)),
                    GUILayout.Height(32)))
                {
                    if (EditorUtility.DisplayDialog("提示", "你确定要删除此条LayerSetting?", "确定", "取消"))
                    {
                        isDel = true;
                        isDirty = true;
                    }
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                if (!isDel)
                {
                    nlist.Add(new UILayer(
                                            nName, ntState, nSiblingIndex
                                            , nUseInnerCanvas, nOverrideSorting, nSortOrder
                                            , nIgnoreReversedGraphics,nBlockingObjects,nBlockingMask
                                            ,nUseCanvasGroup,nAlpha,nInteractable,nBlocksRaycasts,nIgnoreParentGroups
                        ));
                }
            }

            GUILayout.Space(8);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.color = Color.yellow;
            if (GUILayout.Button(new GUIContent("Layer +", "Add New Layer Setting"), GUILayout.Width(Mathf.Max(Screen.width * 0.25f, 64)),
                GUILayout.Height(32)))
            {
                if (EditorUtility.DisplayDialog("提示", "正在创建新Layer Setting ?", "确定", "取消"))
                {
                    int ix = nlist.Count;
                    //nlist.Add(new UILayer("unameLayer", ix));
                    nlist.Add(UILayer.Normal("unameLayer", ix));
                    isDirty = true;
                }
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            nlist.Sort((a, b) =>
            {
                if (a.SiblingIndex > b.SiblingIndex)
                {
                    return 1;
                }
                else if (a.SiblingIndex < b.SiblingIndex)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            });

            return nlist;
        }
    }

}
