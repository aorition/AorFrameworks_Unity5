using System;
using System.Collections.Generic;
using Framework.Extends;
using UnityEngine;

namespace Framework.NodeGraph
{

    //TODO 可能有一下东西要移动到NodeGraphBaseSetting中去。

    /// <summary>
    /// NodeGraph 样式定义
    /// </summary>
    public class NodeGraphDefind
    {

        private static GUIStyle _ToggleARALStyle;
        public static GUIStyle GetToggleARStyle()
        {
            if (_ToggleARALStyle == null)
            {
                _ToggleARALStyle = GUI.skin.toggle.Clone();
                _ToggleARALStyle.alignment = TextAnchor.MiddleRight;;
            }
            return _ToggleARALStyle;
        }

        private static GUIStyle _ToolItemFoldoutStyle;
        public static GUIStyle GetToolItemFoldoutStyle()
        {
            if (_ToolItemFoldoutStyle == null)
            {
                _ToolItemFoldoutStyle = GUI.skin.GetStyle("IN Foldout").Clone();
                _ToolItemFoldoutStyle.fontSize = 10;
                _ToolItemFoldoutStyle.alignment = TextAnchor.MiddleLeft;
                _ToolItemFoldoutStyle.wordWrap = true;
                _ToolItemFoldoutStyle.normal.textColor = Color.white;
            }
            return _ToolItemFoldoutStyle;
        }

        private static GUIStyle _toolBarBtnStyle;
        private static GUIStyle _toolBarBtnStyle_hidden;
        public static GUIStyle GetToolBarBtnStyle(bool isShow)
        {
            if (isShow)
            {
                if (_toolBarBtnStyle == null)
                {
                    _toolBarBtnStyle = GUI.skin.GetStyle("TL tab right").Clone();
                }
                return _toolBarBtnStyle;
                
            }
            else
            {
                if (_toolBarBtnStyle_hidden == null)
                {
                    _toolBarBtnStyle_hidden = GUI.skin.GetStyle("Label").Clone();
                }
                return _toolBarBtnStyle_hidden;
            }
        }

        private static GUIStyle _inspectorBtnStyle;
        private static GUIStyle _inspectorBtnStyle_hidden;
        public static GUIStyle GetInspectorBtnStyle(bool isShow)
        {
            if (isShow)
            {
                if (_inspectorBtnStyle == null)
                {
                    _inspectorBtnStyle = GUI.skin.GetStyle("TL tab left").Clone();
                }
                return _inspectorBtnStyle;
            }
            else
            {
                if (_inspectorBtnStyle_hidden == null)
                {
                    _inspectorBtnStyle_hidden = GUI.skin.GetStyle("RightLabel").Clone();
                }
                return _inspectorBtnStyle_hidden;
                
            }
        }

        private static GUIStyle _ConnectPointLabelStyle_input;
        private static GUIStyle _ConnectPointLabelStyle_output;
        /// <summary>
        /// 
        /// </summary>
        public static GUIStyle GetConnectPointLabelStyle(bool isOutput)
        {
            if (isOutput)
            {
                if (_ConnectPointLabelStyle_output == null)
                {
                    //                    _ConnectPointLabelStyle_output = GUI.skin.box.Clone
                    _ConnectPointLabelStyle_output = new GUIStyle();
                    _ConnectPointLabelStyle_output.fontSize = 10;
                    _ConnectPointLabelStyle_output.alignment = TextAnchor.MiddleRight;
                    _ConnectPointLabelStyle_output.wordWrap = true;
                    _ConnectPointLabelStyle_output.normal.textColor = Color.white;
                }
                return _ConnectPointLabelStyle_output;
            }
            else
            {
                if (_ConnectPointLabelStyle_input == null)
                {
//                    _ConnectPointLabelStyle_input = GUI.skin.box.Clone();
                    _ConnectPointLabelStyle_input = new GUIStyle();
                    _ConnectPointLabelStyle_input.fontSize = 10;
                    _ConnectPointLabelStyle_input.alignment = TextAnchor.MiddleLeft;
                    _ConnectPointLabelStyle_input.wordWrap = true;
                    _ConnectPointLabelStyle_input.normal.textColor = Color.white;
                }
                return _ConnectPointLabelStyle_input;
            }
        }

        private static GUIStyle _NodeGUIBaseMainStyle;
        private static GUIStyle _NodeGUIBaseMainStyle_Active;
        /// <summary>
        /// 获取MainNodeGUI的基础GUIStyle
        /// </summary>
        public static GUIStyle GetNodeGUIBaseMainStyle(bool isActive = false)
        {
            if (isActive)
            {
                if (_NodeGUIBaseMainStyle_Active == null)
                {
                    _NodeGUIBaseMainStyle_Active = GUI.skin.GetStyle("flow node 6 on").Clone();
                    _NodeGUIBaseMainStyle_Active.fontSize = 14;
                }
                return _NodeGUIBaseMainStyle_Active;
            }
            else
            {
                if (_NodeGUIBaseMainStyle == null)
                {
                    _NodeGUIBaseMainStyle = GUI.skin.GetStyle("flow node 6").Clone();
                    _NodeGUIBaseMainStyle.fontSize = 14;
                }
                return _NodeGUIBaseMainStyle;
            }
        }

        private static GUIStyle _NodeGUIBaseStyle;
        private static GUIStyle _NodeGUIBaseStyle_Active;
        /// <summary>
        /// 获取NodeGUI的基础GUIStyle
        /// </summary>
        public static GUIStyle GetNodeGUIBaseStyle(bool isActive = false)
        {
            if (isActive)
            {
                if (_NodeGUIBaseStyle_Active == null)
                {
                    _NodeGUIBaseStyle_Active = GUI.skin.GetStyle("flow node 0 on").Clone();
                    _NodeGUIBaseStyle_Active.fontSize = 14;
                }
                return _NodeGUIBaseStyle_Active;
            }
            else
            {
                if (_NodeGUIBaseStyle == null)
                {
                    _NodeGUIBaseStyle = GUI.skin.GetStyle("flow node 0").Clone();
                    _NodeGUIBaseStyle.fontSize = 14;
                }
                return _NodeGUIBaseStyle;
            }
        }

        private static GUIStyle _NodeToolItemStyle;
        /// <summary>
        /// 返回 NodeToolItemStyle
        /// </summary>
        public static GUIStyle GetNodeToolItemStyle()
        {
            if (_NodeToolItemStyle == null)
            {
                _NodeToolItemStyle = GUI.skin.GetStyle("box").Clone();
                _NodeToolItemStyle.alignment = TextAnchor.MiddleCenter;
                _NodeToolItemStyle.fontSize = 12;
                _NodeToolItemStyle.wordWrap = true;
                _NodeToolItemStyle.normal.textColor = Color.white;
            }
            return _NodeToolItemStyle;
        }

        /// <summary>
        /// 根据 nodeData 的类型 返回 ConnectCenterTip的基础GUIStyle
        /// </summary>
        public static GUIStyle GetConnectCenterTipBaseStyle()
        {
            return GUI.skin.GetStyle("sv_label_0").Clone();
        }
        
        public static Vector2 WindowMinSize
        {
            get { return new Vector2(960,600);}
        }

        /// <summary>
        /// 定义自动行为每隔多少秒执行一次
        /// </summary>
        public const float NodeApp_Time_Interval = 3f; 

        public const float ConnectCenterTipMargin = 5f;
        public const float ConnectCenterTipLabelPreHeight = 26f;
        public const float ConnectCenterTipLabelPreWidth = 8f;

        private static Color m_ConnectionLineColor = Color.gray;
        public static Color ConnectionLineColor
        {
            get { return m_ConnectionLineColor; }
        }

        public const float ConnectionLineWidth = 4f;
        public const float ConnectionLineAddOffset = 20f;


        //--------------------- 菜单 定义---------------

        public const string MENU_ROOT = "Window";

        public const string MENU_MAIN_DIR = MENU_ROOT + "/NodeGraph";

        //--------------------- 菜单 定义----------- end

        //--------------------- 数据缓存  资源定义 -----

        /// <summary>
        /// *** 定义NodeGraph 数据缓存所在路径
        /// 
        /// (** 注意： NodeGraph使用 Application.dataPath.Replace("/Assets", "") 作为根路径)
        /// 
        /// </summary>
        public static string RESCACHE_ROOTPATH
        {
            get
            {
                return Application.dataPath.Replace("/Assets", "") + RESCACHE_ROOTDIR;
            }
        }
        /// <summary>
        /// 定义NodeGraph 数据缓存所在文件夹名
        /// (** 注意因路径连接需求，保留前面的一反斜杠)
        /// </summary>
        public const string RESCACHE_ROOTDIR = "/NodeGraphRESCaches";

        /// <summary>
        /// 定义NodeGraph Setting文件夹
        /// </summary>
        public const string RESCACHE_SETTINGS = "/settings";


        /// <summary>
        /// 定义NodeGraph 最后快照数据路径
        /// </summary>
        public const string RESCACHE_LASTSHOTCUT_DIR = "/lastShotcut";

        public const string RESCACHE_LASTSHOTCUT_NAME = "lastShotcutGraph";

        /// <summary>
        /// 定义NodeGraph Setting文件(不包含后缀名)
        /// </summary>
        public const string RESCACHE_SETTING_NAME = "NodeGraphSettings";

        /// <summary>
        /// 定义动态生成的脚本存放地址
        /// </summary>
        public const string RESCACHE_DYNAMICSCRIPTDIR = "/Editor/DynamicScripts";

        //--------------------- 数据缓存  资源定义 --end

        public const string RESOURCE_ROOT = "Assets/NodeGraphAssets";

        public const string RESOURCE_LAGJSON = RESOURCE_ROOT + "/Language/lag.json";

        //--------------------- NodeGraph 样式图片资源定义 -----

        /// <summary>
        /// *** 定义NodeGraph界面使用的图形资源所在基本路径
        /// </summary>
        public const string RESOURCE_BASEPATH = RESOURCE_ROOT + "/Bitmaps";

        public const string RESOURCE_REFRESH_ICON = RESOURCE_BASEPATH + "/AssetGraph_RefreshIcon.png";
        public const string RESOURCE_ARROW = RESOURCE_BASEPATH + "/AssetGraph_Arrow.png";
        public const string RESOURCE_CONNECTIONPOINT_ENABLE = RESOURCE_BASEPATH + "/AssetGraph_ConnectionPoint_EnableMark.png";
        public const string RESOURCE_CONNECTIONPOINT_INPUT = RESOURCE_BASEPATH + "/AssetGraph_ConnectionPoint_InputMark.png";
        public const string RESOURCE_CONNECTIONPOINT_OUTPUT = RESOURCE_BASEPATH + "/AssetGraph_ConnectionPoint_OutputMark.png";
        public const string RESOURCE_CONNECTIONPOINT_OUTPUT_CONNECTED = RESOURCE_BASEPATH + "/AssetGraph_ConnectionPoint_OutputMark_Connected.png";

        public const string RESOURCE_INPUT_BG = RESOURCE_BASEPATH + "/AssetGraph_InputBG.png";
        public const string RESOURCE_OUTPUT_BG = RESOURCE_BASEPATH + "/AssetGraph_OutputBG.png";

        public const string RESOURCE_SELECTION = RESOURCE_BASEPATH + "/AssetGraph_Selection.png";

        //--------------------- NodeGraph 样式图片资源定义 -- end


        //判定NodeGUI.OnClick的偏移阈值 
        public const float MouseClickThresholdX = 5f;
        public const float MouseClickThresholdY = 5f;

        //菜单栏高度
        public const float MenuLayoutHeight = 30f;
        //工具箱的宽度
        public const float ToolAreaWidth = 135f;
        //工具箱物品高度
        public const float ToolAreaItemHeight = 46f;
        public const float ToolAreaItemFoldoutHeight = 20f;
        //Inspector宽度
        public const float InspectorWidth = 360f;

        //NodeGUI 最小Size
        public const float NodeGUIMinSizeX = 100f;
        public const float NodeGUIMinSizeY = 120f;

        //NodeGraphCanvas 最小Szie
        public const float NodeGraphMinSizeX = 1200f;
        public const float NodeGraphMinSizeY = 1600f;

        //NodeGraphCanvas 动态Size延展尺寸
        public const float NodeGraphSizeExtX = 100f;
        public const float NodeGraphSizeExtY = 60f;

        //NodeGUI内容Margin
        public const float NodeGUIContentMarginX = 10f;
        public const float NodeGUIContentMarginY = 10f;

        public const float ModeGUIRefreshIconX = 16f;
        public const float ModeGUIRefreshIconY = 16f;

        //NodeGUI Resize图标size
        public const float NodeGUIResizeIconSizeX = 8f;
        public const float NodeGUIResizeIconSizeY = 8f;

        //NodeGraph框体基本颜色
        private static Color m_NodeGraphBaseColor = Color.white;
        public static Color NodeGraphBaseColor
        {
            get { return m_NodeGraphBaseColor; }
        }

        

        //MutiSelection 框体颜色
        private static Color m_MutiSelectionColor = Color.cyan;
        public static Color MutiSelectionColor {
            get { return m_MutiSelectionColor;}
        }

        public const float NODE_TITLE_HEIGHT = 30f;

        /// 输入点size(BG)
        public const float INPUT_POINT_WIDTH = 16f;
        public const float INPUT_POINT_HEIGHT = 26f;

        //输出点size(BG)
        public const float OUTPUT_POINT_WIDTH = 18f;
        public const float OUTPUT_POINT_HEIGHT = 32f;

        public const float CONNECTION_INPUT_POINT_MARK_SIZE = 20f;
        public const float CONNECTION_OUTPUT_POINT_MARK_SIZE = 16f;

        public const string NODE_INPUTPOINT_FIXED_LABEL = "FIXED_INPUTPOINT_ID";

        //连线箭头size
        public const float CONNECTION_ARROW_WIDTH = 12f;
        public const float CONNECTION_ARROW_HEIGHT = 15f;

        //连线最小长度
        public const float CONNECTION_CURVE_LENGTH = 10f;

    }
}
