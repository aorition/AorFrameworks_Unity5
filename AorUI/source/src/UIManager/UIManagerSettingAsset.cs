using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public class UIManagerSettingAsset : JScriptableObject
    {

        public static UIManagerSettingAsset Default()
        {
            UIManagerSettingAsset def = ScriptableObject.CreateInstance<UIManagerSettingAsset>();
            def.InfoList.Add(UICanvasInfo.Default());
            def.LayerList.Add(new UILayerGroup());
            def.LayerList[0].Add(UILayer.Default());
            return def;
        }

        public static UIManagerSettingAsset Normal()
        {
            UIManagerSettingAsset def = ScriptableObject.CreateInstance<UIManagerSettingAsset>();
            def.InfoList.Add(UICanvasInfo.Normal("Default", new Vector2(1280, 720)));
            def.LayerList.Add(new UILayerGroup());
            def.LayerList[0].Add(UILayer.Default());
            return def;
        }

        [SerializeField]
        protected List<UICanvasInfo> m_InfoList = new List<UICanvasInfo>();
        public List<UICanvasInfo> InfoList
        {
            get { return m_InfoList; }
        }

        [SerializeField]
        protected List<UILayerGroup> m_LayerList = new List<UILayerGroup>();
        public List<UILayerGroup> LayerList
        {
            get { return m_LayerList;}
        }
/*
 * (默认保存JSON已经可以适用通用)
        public override void SetupFormJson(string jsonText)
        {
            //
            Dictionary<string, object> JDic = Json.DecodeToDic(jsonText);
            
            IList UICanvasInfo = JDic["m_InfoList"] as IList;
            foreach (object o in UICanvasInfo)
            {
                m_InfoList.Add((UICanvasInfo)JSONParser.ParseValue(typeof(UICanvasInfo), o));
            }

            IList UILayerGroup = JDic["m_LayerList"] as IList;
            foreach (object o in UILayerGroup)
            {
                m_LayerList.Add((UILayerGroup)JSONParser.ParseValue(typeof(UILayerGroup), o));
            }
        }
        */
        public override void ClearSerializeData()
        {
            base.ClearSerializeData();
            m_InfoList.Clear();
            m_LayerList.Clear();
        }
    }

    [Serializable]
    public struct UILayerGroup
    {

        public UILayer this[int i]
        {
            get { return m_UILayers[i]; }
        }

        public int Count
        {
            get
            {
                if (m_UILayers == null)
                {
                    return 0;
                }
                return m_UILayers.Count;
            }
        }


        public void Sort()
        {
            if (m_UILayers == null) return;
            m_UILayers.Sort();
        }
        public void Sort(IComparer<UILayer> comparer)
        {
            if (m_UILayers == null) return;
            m_UILayers.Sort(comparer);
        }
        public void Sort(int index, int count, IComparer<UILayer> comparer)
        {
            if (m_UILayers == null) return;
            m_UILayers.Sort(index, count, comparer);
        }
        public void Sort(Comparison<UILayer> comparison)
        {
            if (m_UILayers == null) return;
            m_UILayers.Sort(comparison);
        }

        public void Add(UILayer layer)
        {
            if (m_UILayers == null)
            {
                m_UILayers = new List<UILayer>();
            }
            m_UILayers.Add(layer);
        }

        public bool Remove(UILayer layer)
        {
            if (m_UILayers == null) return false;
            return m_UILayers.Remove(layer);
        }

        public int RemoveAll(Predicate<UILayer> match)
        {
            if (m_UILayers == null) return -1;
            return m_UILayers.RemoveAll(match);
        }

        public void RemoveAt(int index)
        {
            if (m_UILayers == null) return;
            m_UILayers.RemoveAt(index);
        }

        public UILayerGroup(bool setup)
        {
            this.m_UILayers = new List<UILayer>();
            this._isInit = true;
        }


        [SerializeField]
        private bool _isInit;
        public bool isInit
        {
            get { return _isInit; }
        }

        [SerializeField]
        private List<UILayer> m_UILayers;

    }

    [Serializable]
    public enum UILayerPovitState
    {
        Center,
        TopLeft,
        Top,
        TopRight,
        Left,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }

    [Serializable]
    public struct UILayer
    {
        public static UILayer Default()
        {
            return new UILayer( "DefaultLayer", UILayerPovitState.Center, 0
                                , false,false,0
                                ,true,GraphicRaycaster.BlockingObjects.None, -1
                                ,false,1,true,true,false
                );
        }

        public static UILayer Normal(string name, int siblingIndex)
        {
            return new UILayer( name, UILayerPovitState.Center
                                , siblingIndex
                                , false, false, 0
                                , true, GraphicRaycaster.BlockingObjects.None, -1
                                , false, 1, true, true, false
                );
        }

        public UILayer(string name, UILayerPovitState povitState, int siblingIndex
            , bool useInnerCanvas, bool overrideSorting, int sortOrder
            , bool ignoreReversedGraphics, GraphicRaycaster.BlockingObjects blockingObjects, int blockingMask
            , bool useCanvasGroup , float alpha, bool interactable, bool blocksRaycasts, bool ignoreParentGroups
        )
        {
            this.Name = name;
            this.PovitState = povitState;
            this.SiblingIndex = siblingIndex;
            this.UseInnerCanvas = useInnerCanvas;
            this.OverrideSorting = overrideSorting;
            this.SortOrder = sortOrder;

            this.IgnoreReversedGraphics = ignoreReversedGraphics;
            this.BlockingObjects = blockingObjects;
            this.BlockingMask = blockingMask;

            this.UseCanvasGroup = useCanvasGroup;
            this.Alpha = alpha;
            this.Interactable = interactable;
            this.BlocksRaycasts = blocksRaycasts;
            this.IgnoreParentGroups = ignoreParentGroups;
            
            this._isInit = true;
        }

        [SerializeField]
        private bool _isInit;
        public bool isInit
        {
            get { return _isInit; }
        }

        public string Name;
        public UILayerPovitState PovitState;
        public int SiblingIndex;
        public bool UseInnerCanvas;
        public bool OverrideSorting;
        public int SortOrder;
        //------- GraphicRaycaster
        public bool IgnoreReversedGraphics;
        public GraphicRaycaster.BlockingObjects BlockingObjects;
        public int BlockingMask;
        //------  CanvasGroup
        public bool UseCanvasGroup;
        public float Alpha;
        public bool Interactable;
        public bool BlocksRaycasts;
        public bool IgnoreParentGroups;
    }

    [Serializable]
    public struct UICanvasInfo
    {

        public static UICanvasInfo Default()
        {
            return new UICanvasInfo("Default", RenderMode.ScreenSpaceOverlay, false, "Default", 0
                , 0, AdditionalCanvasShaderChannels.None, new CameraStructInfo(), 50
                , CanvasScaler.ScaleMode.ConstantPixelSize, 1f, 100, new Vector2(800,600)
                , CanvasScaler.ScreenMatchMode.MatchWidthOrHeight, 0
                , CanvasScaler.Unit.Points, 96, 96, true, GraphicRaycaster.BlockingObjects.None, -1
                );
        }

        public static UICanvasInfo Default(string name, int sortOrder = 0)
        {
            return new UICanvasInfo("Default", RenderMode.ScreenSpaceOverlay, false, "Default", sortOrder
                , 0, AdditionalCanvasShaderChannels.None, new CameraStructInfo(), 50
                , CanvasScaler.ScaleMode.ConstantPixelSize, 1f, 100, new Vector2(800, 600)
                , CanvasScaler.ScreenMatchMode.MatchWidthOrHeight, 0
                , CanvasScaler.Unit.Points, 96, 96, true, GraphicRaycaster.BlockingObjects.None, -1
                );
        }

        public static UICanvasInfo Normal(string name, Vector2 referenceResolution,float matchWidthOrHeight = 1, int sortOrder = 0)
        {
            return new UICanvasInfo(name, RenderMode.ScreenSpaceOverlay, false, "Default", sortOrder
                , 0, AdditionalCanvasShaderChannels.None, new CameraStructInfo(), 50
                , CanvasScaler.ScaleMode.ScaleWithScreenSize, 1f, 100, referenceResolution
                , CanvasScaler.ScreenMatchMode.MatchWidthOrHeight, matchWidthOrHeight
                , CanvasScaler.Unit.Points, 96, 96, true, GraphicRaycaster.BlockingObjects.None, -1
                );
        }

        public UICanvasInfo(string name, RenderMode renderMode, bool pixelPerfect, string sortingLayerName, int sortOrder, int targetDisplay, AdditionalCanvasShaderChannels additionalShaderChannels, CameraStructInfo cameraInfo, float PlaneDistance
            , CanvasScaler.ScaleMode scaleMode, float scaleFactor, float referencePixelsPerUnit, Vector2 referenceResolution, CanvasScaler.ScreenMatchMode screenMatchMode, float matchWidthOrHeight, CanvasScaler.Unit physicalUnit, float fallbackScreenDpi, float defaultSpriteDpi
            , bool ignoreReversedGraphic, GraphicRaycaster.BlockingObjects blockingObjects,int blockingMask
            )
        {

            this.Name = name;
            this.RenderMode = renderMode;
            this.PixelPerfect = pixelPerfect;
            this.SortingLayerName = sortingLayerName;
            this.SortOrder = sortOrder;
            this.TargetDisplay = targetDisplay;
            this.AdditionalShaderChannels = additionalShaderChannels;
            this.CameraInfo = cameraInfo;
            this.PlaneDistance = PlaneDistance;
            this.ScaleMode = scaleMode;
            this.ScaleFactor = scaleFactor;
            this.ReferencePixelsPerUnit = referencePixelsPerUnit;
            this.ReferenceResolution = referenceResolution;
            this.ScreenMatchMode = screenMatchMode;
            this.MatchWidthOrHeight = matchWidthOrHeight;
            this.PhysicalUnit = physicalUnit;
            this.FallbackScreenDPI = fallbackScreenDpi;
            this.DefaultSpriteDPI = defaultSpriteDpi;
            this.IgnoreReversedGraphic = ignoreReversedGraphic;
            this.BlockingObjects = blockingObjects;
            this.BlockingMask = blockingMask;
            this._isInit = true;
        }

        [SerializeField]
        private bool _isInit;
        public bool isInit
        {
            get { return _isInit;}
        }

        public string Name;
        public RenderMode RenderMode;
        public bool PixelPerfect;
        public string SortingLayerName;
        public int SortOrder;
        public int TargetDisplay;
        public AdditionalCanvasShaderChannels AdditionalShaderChannels;
        public CameraStructInfo CameraInfo;
        public float PlaneDistance;
        //---------------
        public CanvasScaler.ScaleMode ScaleMode;

        public float ScaleFactor;
        public float ReferencePixelsPerUnit;
        //
        public Vector2 ReferenceResolution;
        public CanvasScaler.ScreenMatchMode ScreenMatchMode;
        public float MatchWidthOrHeight;
        //
        public CanvasScaler.Unit PhysicalUnit;
        public float FallbackScreenDPI;
        public float DefaultSpriteDPI;
        //---------------
        public bool IgnoreReversedGraphic;
        public GraphicRaycaster.BlockingObjects BlockingObjects;
        public int BlockingMask;
    }

}
