using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AorBaseUtility;
using AorBaseUtility.Extends;
using Framework.NodeGraph.Tool;
using NodeGraph.SupportLib;

namespace Framework.NodeGraph
{

    public enum NodeGraphConnectMode
    {
        LINK, //链表结构，允许交叉连线，节点只允许有一个输入和一个输出
        TREE, //树状结构，不允许交叉连线，节点可以有多个输入/输出

        //Todo 看是否需要把 网状结构分出来？！

    }

    public enum NodeGraphModifyState
    {
        Default,
        MutiSelect,
        ConnectionDraw,
        DragToolItem,
    }

    public class NodeGraphBase : EditorWindow
    {

        public static bool TimeInterval_Request_SAVESHOTCUTGRAPH = false;

        private static double NGB_Time_Start;
        private static double NGB_time_Delay;
        private static double NGB_time;
        private static void NodeGraphBaseUpdate()
        {
            if (m_Instance == null)
            {
                EditorApplication.update -= NodeGraphBaseUpdate;
                return;
            }

            NGB_time_Delay = EditorApplication.timeSinceStartup - NGB_Time_Start;
            NGB_Time_Start = EditorApplication.timeSinceStartup;
            NGB_time += NGB_time_Delay;

            if (NGB_time > NodeGraphDefind.NodeApp_Time_Interval)
            {
                 
                //Debug.Log("** Time Interval!!");
                                
                //每隔单位时间检查自动处理的事物
                if (TimeInterval_Request_SAVESHOTCUTGRAPH)
                {
                    m_Instance.SaveLastShotcutGraph();
                    TimeInterval_Request_SAVESHOTCUTGRAPH = false;
                }


                NGB_time = 0;
            }
        }

        protected static NodeGraphBase m_Instance;
        public static NodeGraphBase Instance
        {
            get { return m_Instance; }
            //            get
            //            {
            //                return EditorWindow.GetWindow<NodeGraphBase>("NodeGraph");
            //            }
        }

        //[MenuItem("Window/NodeGraphBase")]
        public static void init()
        {
            NodeGraphBase w = EditorWindow.GetWindow<NodeGraphBase>("NodeGraph");
            w.setup();
        }

        //---------------------------------------- NodeGraphBase settings -------------------

        protected NodeGraphLagDefind.NodeGraphLagTag _LAGTag = NodeGraphLagDefind.NodeGraphLagTag.CH;
        public NodeGraphLagDefind.NodeGraphLagTag LAGTag
        {
            get { return _LAGTag; }
        }

        //是否显示工具箱
        protected bool _isShowToolArea = true;

        public bool isShowToolArea
        {
            get { return _isShowToolArea; }
        }
        //是否显示Inspector
        protected bool _isShowInspector = true;

        protected float _ToolAreaWidth = NodeGraphDefind.ToolAreaWidth;
        protected float _InspectorWidth = NodeGraphDefind.InspectorWidth;

        protected Vector2 _windowMinSize = NodeGraphDefind.WindowMinSize;

        //---------------------------------------- NodeGraphBase settings ----------------end
        protected bool _isInit = false;

        protected NodeGraphConnectMode _connectMode;
        public NodeGraphConnectMode connectMode
        {
            get { return _connectMode; }
        }

        protected NodeGraphModifyState _state = NodeGraphModifyState.Default;
        public NodeGraphModifyState state
        {
            get { return _state; }
        }

        protected Rect _mutiSelectRect;

        /// <summary>
        /// 初始化窗体方法
        /// </summary>
        public virtual void setup()
        {
            setupNodeGraphbaseModeDefine();

            if (m_Instance == null)
            {
                NGB_Time_Start = EditorApplication.timeSinceStartup;
                EditorApplication.update += NodeGraphBaseUpdate;
            }

            m_Instance = this;

            //初始化 工具箱
            initToolAreaItems();

            //检查缓存文件夹
            checkAndCreateCacheFolder();

            _Settings = _tryGetNodeGraphBaseSettingsInCache();
            if (_Settings == null)
            {
                //创建默认的Setting并保存
                _Settings = _createDefaultSettings();
                _saveSettingFileToCache();
            }

            //初始化Setting相关内容
            initSettings();

            OnSetup();

            //尝试加载最后一次快照
            _tryOpenLastShotcutGraph();
            
            _isInit = true;
        }


        #region  缓存文件夹相关函数

        private string _cacheFolderPath;
        private void checkAndCreateCacheFolder()
        {

            _cacheFolderPath = NodeGraphDefind.RESCACHE_ROOTPATH;

            if (!Directory.Exists(_cacheFolderPath))
            {
                //创建缓存文件夹
                Directory.CreateDirectory(_cacheFolderPath);
            }
        }

        #endregion

        #region  Setting相关函数

        private NodeGraphBaseSetting _Settings;
        private string _cacheSettingsPath;

        /// <summary>
        /// 更新并保存Setting
        /// </summary>
        /// <param name="setting"></param>
        public virtual void UpdateSettings(NodeGraphBaseSetting setting)
        {
            _Settings = setting;
            _saveSettingFileToCache();
            initSettings();
        }

        //保存快照数据
        public virtual void SaveLastShotcutGraph()
        {
            if (_NodeGUIList == null || _NodeGUIList.Count == 0) return;

            string lsgDir = _cacheFolderPath + NodeGraphDefind.RESCACHE_LASTSHOTCUT_DIR;
            if (!Directory.Exists(lsgDir))
            {
                Directory.CreateDirectory(lsgDir);
            }

            string lsgPath = lsgDir +"/" + NodeGraphDefind.RESCACHE_LASTSHOTCUT_NAME + ".json";

            SaveGraphToFile(lsgPath);
        }

        //尝试读取快照数据
        private void _tryOpenLastShotcutGraph()
        {

            string lsgPath = _cacheFolderPath + NodeGraphDefind.RESCACHE_LASTSHOTCUT_DIR + "/" + NodeGraphDefind.RESCACHE_LASTSHOTCUT_NAME + ".json";
            
            if (!File.Exists(lsgPath)) return;

            OpenGraph(lsgPath);
        }

        protected virtual void initSettings()
        {
            //Todo 将settings文件的数据应用到程序中来。
        }




        private NodeGraphBaseSetting _tryGetNodeGraphBaseSettingsInCache()
        {
            string settingDir = _cacheFolderPath + NodeGraphDefind.RESCACHE_SETTINGS;
            if (!Directory.Exists(settingDir))
            {
                Directory.CreateDirectory(settingDir);
            }

            _cacheSettingsPath = settingDir + "/" + NodeGraphDefind.RESCACHE_SETTING_NAME + ".json";

            if (File.Exists(_cacheSettingsPath))
            {
                string settingSrc = AorIO.ReadStringFormFile(_cacheSettingsPath);
                NodeGraphBaseSetting setting = JConfigParser.ToConfig<NodeGraphBaseSetting>(settingSrc);
                if (setting != null)
                {
                    return setting;
                }
            }

            return null;
        }

        //创建默认的Setting
        private NodeGraphBaseSetting _createDefaultSettings()
        {
            return new NodeGraphBaseSetting();
        }

        //保存Setting
        private bool _saveSettingFileToCache()
        {
            if (_Settings == null) return false;

            string settingsStr = JConfigParser.ToJSON(_Settings);
            if (!string.IsNullOrEmpty(settingsStr))
            {
                return AorIO.SaveStringToFile(_cacheSettingsPath, settingsStr);
            }
            return false;
        }

        #endregion

        protected virtual void setupNodeGraphbaseModeDefine()
        {
            //set NodeGraphConnectMode
            _connectMode = NodeGraphConnectMode.TREE;
            //set NodeGraphModifyState
            _state = NodeGraphModifyState.Default;

        }

        protected virtual void OnSetup()
        {
            //
        }

        /// <summary>
        /// 设置界面相关
        /// </summary>
        public virtual void setSettingsInFile(Dictionary<string, string> keyValue)
        {
            if (keyValue == null) return;

            Type type = GetType();

            foreach (string key in keyValue.Keys)
            {
                FieldInfo fieldInfo = type.GetField(key, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField);
                if (fieldInfo != null)
                {
                    Type fieldType = fieldInfo.FieldType;

                    fieldInfo.SetValue(this, JConfigParser.ParseValue(fieldType, keyValue[key]));
                }
            }
        }

        //连线绘制模式 指定的开始点
        protected ConnectionPointGUI _startPointGUI;
        public ConnectionPointGUI GetDrawLineModeStartPoint()
        {
            return _startPointGUI;
        }

        /// <summary>
        /// 进入连线绘制模式
        /// </summary>
        public void EnterDrawLineMode(ConnectionPointGUI startPointGUI)
        {
            _startPointGUI = startPointGUI;
            _state = NodeGraphModifyState.ConnectionDraw;
        }

        /// <summary>
        /// 绘制_欢迎界面
        /// </summary>
        //
        protected virtual void _draw_welcome()
        {

            GUILayout.BeginVertical(GUILayout.Height(Screen.height - 50));

                GUILayout.FlexibleSpace();
                
                GUILayout.BeginHorizontal();

                    GUILayout.FlexibleSpace();
                    GUILayout.Label(NodeGraphLagDefind.Get("welcome"), "HeaderLabel");
                    GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();
                
                //test 新建工作流
                if (GUILayout.Button(NodeGraphLagDefind.Get("new") + " Graph"))
                {
                    NewNodeGraph();
                    Repaint();
                }

                if (GUILayout.Button(NodeGraphLagDefind.Get("open") + " Graph"))
                {
                    OpenGraph();
                    Repaint();
                }

                GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
            
            GUILayout.BeginHorizontal(GUILayout.Height(50));

                GUILayout.FlexibleSpace();
                GUILayout.Label("w:" + Screen.width + " , h:" + Screen.height);
                GUILayout.Space(10);

            GUILayout.EndHorizontal();

        }

        /// <summary>
        /// 验证INodeData.id是否是唯一的
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual bool verifyNodeID(long id)
        {
            if (_nodeGuiIdSet == null) return true;
            if (_nodeGuiIdSet.Contains(id))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取一个在本工作流中唯一的NodeID
        /// </summary>
        /// <returns></returns>
        public virtual int GetNewNodeID()
        {
            if (_nodeGuiIdSet == null) return int.MinValue;

            int i = int.MinValue;
            while (true)
            {
                if (!_nodeGuiIdSet.Contains(i))
                {
                    return i;
                }
                i++;
            }
        }

        /// <summary>
        /// 绘制_未初始化时界面的显示
        /// </summary>
        protected virtual void _draw_unInitScreen()
        {

            GUILayout.BeginVertical(GUILayout.Height(Screen.height - 50));

                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();

                    GUILayout.FlexibleSpace();
                    GUILayout.Label(NodeGraphLagDefind.Get("noInit"), "HeaderLabel");
                    GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.BeginHorizontal(GUILayout.Height(50));

                GUILayout.FlexibleSpace();
                GUILayout.Label("w:" + Screen.width + " , h:" + Screen.height);
                GUILayout.Space(10);

            GUILayout.EndHorizontal();

        }

        protected NodeGUI _MainNode;
        public NodeGUI MainNode
        {
            get { return _MainNode; }
        }

        public void SetMainNode(NodeGUI node)
        {
            _MainNode = node;
        }

        //插座点的集合
        protected List<ConnectionPointGUI> _connectionPointGUIList;

        //连线的集合
        protected List<ConnectionGUI> _ConnectionGUIList;
        //NodeGUI节点集合
        protected List<NodeGUI> _NodeGUIList;
        //激活的NodeGUI节点集合
        protected List<NodeGUI> _activeNodeGUIList;
        //NodeGUI的ID集合
        protected HashSet<long> _nodeGuiIdSet;

        /// <summary>
        /// 添加NodeGUI
        /// </summary>
        public virtual void AddNodeGUI(NodeGUI node)
        {
            if (_NodeGUIList == null)
            {
                NewNodeGraph();
            }

            if (verifyNodeID(node.data.id))
            {

                node.SetDirty();

                _NodeGUIList.Add(node);
                _nodeGuiIdSet.Add(node.data.id);

                //添加由此node产生的ConnectionPointGUI
                List<ConnectionPointGUI> newPl = node.GUIController.GetConnectionPointInfo(GetConnectionPointMode.AllPoints);
                if (newPl != null && newPl.Count > 0)
                {
                    int i, len = newPl.Count;
                    for (i = 0; i < len; i++)
                    {
                        ConnectionPointGUI p = newPl[i];
                        if (!_connectionPointGUIList.Contains(p))
                        {
                            _connectionPointGUIList.Add(p);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 移除NodeGUI
        /// </summary>
        public virtual bool RemoveNodeGUI(NodeGUI node)
        {
            if (_NodeGUIList == null)
            {
                return false;
            }
            if (_NodeGUIList.Contains(node))
            {

                if (_MainNode != null && _MainNode == node)
                {
                    _MainNode = null;
                }

                if (_NodeGUIList.Remove(node))
                {

                    //                    Undo.RecordObject(this, "REMOVE NODEGUI");

                    _nodeGuiIdSet.Remove(node.data.id);

                    List<ConnectionPointGUI> points = node.GUIController.GetConnectionPointInfo(GetConnectionPointMode.AllPoints);

                    //移除由此node产生的ConnectionGUI
                    if (_ConnectionGUIList != null && _ConnectionGUIList.Count > 0)
                    {
                        List<ConnectionGUI> delConnection = _ConnectionGUIList.Where(gui =>
                        {
                            return points.Contains(gui.InputPointGui) || points.Contains(gui.OutputPointGui);
                        }).ToList();

                        int c, len = delConnection.Count;
                        for (c = 0; c < len; c++)
                        {
                            RemoveConnection(delConnection[c]);
                        }

                        delConnection.Clear();
                    }

                    //移除由此node产生的ConnectionPointGUI
                    if (_connectionPointGUIList != null && _connectionPointGUIList.Count > 0)
                    {
                        List<ConnectionPointGUI> delPointGuis = _connectionPointGUIList.Where(gui =>
                        {
                            return points.Contains(gui);
                        }).ToList();

                        int i, len = delPointGuis.Count;
                        for (i = 0; i < len; i++)
                        {
                            if (_connectionPointGUIList.Contains(delPointGuis[i]))
                            {
                                _connectionPointGUIList.Remove(delPointGuis[i]);
                                delPointGuis[i].Dispose();
                            }
                        }

                        delPointGuis.Clear();

                    }

                }
            }
            return false;
        }

        /// <summary>
        /// 将node加入_activeNodeGUIList，并且将_isActive字段设置为true
        /// </summary>
        public virtual void AddActiveNodeGUI(NodeGUI node)
        {
            if (_activeNodeGUIList == null) return;
            //
            if (!_activeNodeGUIList.Contains(node))
            {
                node.ref_SetField_Inst_NonPublic("_isActive", true);
                _activeNodeGUIList.Add(node);
            }
        }

        /// <summary>
        /// 将node从_activeNodeGUIList中移除，并且将_isActive字段设置为false
        /// </summary>
        public virtual bool RemoveActiveNodeGUI(NodeGUI node)
        {
            if (_activeNodeGUIList != null && _activeNodeGUIList.Contains(node))
            {
                node.ref_SetField_Inst_NonPublic("_isActive", false);
                return _activeNodeGUIList.Remove(node);
            }
            return false;
        }

        /// <summary>
        /// 移除所有_activeNodeGUIList中的node，并且将_isActive字段设置为false
        /// </summary>
        public virtual void ClearActiveNodeGUI()
        {
            if (_activeNodeGUIList == null) return;
            int i, len = _activeNodeGUIList.Count;
            for (i = 0; i < len; i++)
            {
                NodeGUI node = _activeNodeGUIList[i];
                node.ref_SetField_Inst_NonPublic("_isActive", false);
            }
            _activeNodeGUIList.Clear();
        }

        /// <summary>
        /// 将不在_activeNodeGUIList的节点的_isActive字段设置为false;
        /// </summary>
        public virtual void ResetActiveNodeGUIWithoutList()
        {
            if (_activeNodeGUIList == null || _NodeGUIList == null) return;
            int i, len = _NodeGUIList.Count;
            for (i = 0; i < len; i++)
            {
                NodeGUI node = _NodeGUIList[i];
                if (!_activeNodeGUIList.Contains(node))
                {
                    node.ref_SetField_Inst_NonPublic("_isActive", false);
                }
            }
        }

        /// <summary>
        /// 检查交叉引用
        /// </summary>
        public virtual bool CheckCrossReferences(NodeGUI node, NodeGUI targetNode)
        {
            if (node == targetNode) return true;

            //向上查找
            bool result = _findCrossReferencesUpper(node, targetNode);

            //TODO 尚不确定是否需要向下查找交叉引用，且检查交叉引用的算法不准确有逻辑bug。暂时先屏蔽向下查找
            //if(result) return true;
            //向下查找
            //result = _findCrossReferencesDowner(node, targetNode);
            return result;
        }

        private bool _findCrossReferencesUpper(NodeGUI node, NodeGUI targetNode)
        {
            if (node == targetNode) return true;

            List<ConnectionPointGUI> list = node.GUIController.GetConnectionPointInfo(GetConnectionPointMode.InputPoints);
            if (list != null && list.Count > 0)
            {
                bool result = false;
                int i, len = list.Count;
                for (i = 0; i < len; i++)
                {
                    ConnectionPointGUI intputPoint = list[i];
                    List<ConnectionGUI> clist = GetContainsConnectionGUI(intputPoint);
                    if (clist != null)
                    {
                        int c, clen = clist.Count;
                        for (c = 0; c < clen; c++)
                        {
                            result = _findCrossReferencesUpper(clist[c].OutputPointGui.node, targetNode);
                            if (result)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool _findCrossReferencesDowner(NodeGUI node, NodeGUI targetNode)
        {
            if (node == targetNode) return true;

            List<ConnectionPointGUI> list = node.GUIController.GetConnectionPointInfo(GetConnectionPointMode.OutputPoints);
            if (list != null && list.Count > 0)
            {
                bool result = false;
                int i, len = list.Count;
                for (i = 0; i < len; i++)
                {
                    ConnectionPointGUI outputPoint = list[i];
                    List<ConnectionGUI> clist = GetContainsConnectionGUI(outputPoint);
                    if (clist != null)
                    {
                        int c, clen = clist.Count;
                        for (c = 0; c < clen; c++)
                        {
                            result = _findCrossReferencesDowner(clist[c].InputPointGui.node, targetNode);
                            if (result)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 检查连线是否合法
        /// </summary>
        /// <param name="outputPoint">输出点(开始点)</param>
        /// <param name="intputPoint">输入点(结束点)</param>
        /// <returns></returns>
        public virtual bool CheckDoConnection(ConnectionPointGUI outputPoint, ConnectionPointGUI inputPoint, bool allowCross = false)
        {
            //输入点不能是用过的
            if (inputPoint.isUsed) return false;

            //输入输出点必须是同一DataTpye
            if (outputPoint.DataTypeName != inputPoint.DataTypeName) return false;

            //输入和输出点不能是同一个节点
            if (outputPoint.node == inputPoint.node) return false;

            //交叉应用检查
            if (!allowCross)
            {
                if (CheckCrossReferences(outputPoint.node, inputPoint.node)) return false;
            }

            return true;
        }

        /// <summary>
        /// 建立连线
        /// </summary>
        /// <param name="outputPoint">输出点(开始点)</param>
        /// <param name="intputPoint">输入点(结束点)</param>
        /// <param name="allowCross">是否允许交叉连线，默认不允许</param>
        /// <returns></returns>
        public virtual bool CreateConnection(ConnectionPointGUI outputPoint, ConnectionPointGUI inputPoint,bool autoUpdate = true)
        {

            ConnectionPointGUI o, i;
            if (outputPoint.isInput)
            {
                i = outputPoint;
                o = inputPoint;
            }
            else
            {
                i = inputPoint;
                o = outputPoint;
            }

            //检查连线是否合法
            if (CheckDoConnection(o, i, _connectMode == NodeGraphConnectMode.LINK))
            {

                ConnectionGUI connection = new ConnectionGUI(o, i);
                if (_ConnectionGUIList != null)
                {
                    if (!_ConnectionGUIList.Contains(connection))
                    {

                        //                        Undo.RecordObject(this, "ADD CONNECTION");

                        //标识这两点已经被用过了
                        connection.SetPointUsed(true);
                        _ConnectionGUIList.Add(connection);

                        if (autoUpdate)
                        {
                            //更新子节点
                            inputPoint.node.controller.update();
                        }
                    }
                }

                return true;

            }
            return false;
        }

        /// <summary>
        /// 获取Node上ConnectionPoint List
        /// </summary>
        public virtual List<ConnectionPointGUI> GetConnectionPointInfo(NodeGUI node, GetConnectionPointMode GetMode)
        {
            return node.GUIController.GetConnectionPointInfo(GetMode);
        }

        /// <summary>
        /// 获取ConnectionPointGUI
        /// </summary>
        /// <param name="NodeID">NodeGUI的id（NodeData.id）</param>
        /// <param name="CPointID">ConnectionPointGUI的id(ConnectionPointGUI.id)</param>
        /// <param name="inout">表示是Input，还是Output</param>
        /// <returns></returns>
        public virtual ConnectionPointGUI GetConnectionPointGui(int NodeID, long CPointID, ConnectionPointInoutType inout)
        {
            if (_connectionPointGUIList == null) return null;
            int i, len = _connectionPointGUIList.Count;
            for (i = 0; i < len; i++)
            {
                if (_connectionPointGUIList[i].node.data.id == NodeID &&
                    _connectionPointGUIList[i].id == CPointID)
                {
                    if (inout == ConnectionPointInoutType.Output ||
                        (inout == ConnectionPointInoutType.Input || inout == ConnectionPointInoutType.MutiInput))
                    {
                        return _connectionPointGUIList[i];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取包含此连线点的连线集合
        /// </summary>
        public List<ConnectionGUI> GetContainsConnectionGUI(ConnectionPointGUI connectionPointGUI)
        {
            if (_ConnectionGUIList == null) return null;
            List<ConnectionGUI> list = new List<ConnectionGUI>();
            int i, len = _ConnectionGUIList.Count;
            if (connectionPointGUI.isOutput)
            {
                for (i = 0; i < len; i++)
                {
                    if (_ConnectionGUIList[i].OutputPointGui == connectionPointGUI)
                    {
                        list.Add(_ConnectionGUIList[i]);
                    }
                }
            }
            else
            {
                for (i = 0; i < len; i++)
                {
                    if (_ConnectionGUIList[i].InputPointGui == connectionPointGUI)
                    {
                        list.Add(_ConnectionGUIList[i]);
                    }
                }
            }

            if (list.Count > 0) return list;

            return null;
        }

        /// <summary>
        /// 删除连线
        /// </summary>
        public virtual void RemoveConnection(ConnectionGUI connection)
        {
            if (_ConnectionGUIList.Contains(connection))
            {

                //                Undo.RecordObject(this, "REMOVE CONNECTION");

                connection.SetPointUsed(false);
                _ConnectionGUIList.Remove(connection);

                //更新子节点
                connection.InputPointGui.node.controller.update();

                connection.Dispose();
            }
        }

        protected void OnGUI()
        {
            //设置界面最小尺寸
            if (minSize != _windowMinSize)
            {
                minSize = _windowMinSize;
            }

            GUI.color = NodeGraphDefind.NodeGraphBaseColor;

            #region 编译中
            if (EditorApplication.isCompiling)
            {
                ShowNotification(new GUIContent("Compiling Please Wait..."));
                Repaint();
                return;
            }
            RemoveNotification();
            #endregion

            if (!_isInit)
            {
                _draw_unInitScreen();
                setup();
                Repaint();
                return;
            }

            if (_NodeGUIList == null)
            {
                //这里有个莫名其妙的补救
                if (m_Instance == null)
                {
                    setup();
                }

                _draw_welcome();
                HandleStageEvents();

                return;
            }

            //主状态机检查
            FixedEventToState();

            //menu draws
            Draw_menu();

            //main draws
            if (_isShowToolArea && _isShowInspector)
            {
                _NodeGraphSize = new Rect(_ToolAreaWidth, NodeGraphDefind.MenuLayoutHeight, position.width - _InspectorWidth - _ToolAreaWidth, position.height - NodeGraphDefind.MenuLayoutHeight);
                _ToolAreaSize = new Rect(0, NodeGraphDefind.MenuLayoutHeight, _ToolAreaWidth, position.height - NodeGraphDefind.MenuLayoutHeight);
                //                _ToolAreaSize = new Rect(0, 0, _ToolAreaWidth, position.height);
                //                _InspectorAreaSize = new Rect(position.width - _InspectorWidth, NodeGraphDefind.MenuLayoutHeight, _InspectorWidth, position.height - NodeGraphDefind.MenuLayoutHeight);
                _InspectorAreaSize = new Rect(position.width - _InspectorWidth, 0, _InspectorWidth, position.height);

                Draw_ToolArea(_ToolAreaSize);
                Draw_GUINodeGraph(_NodeGraphSize);
                Draw_Inspector(_InspectorAreaSize);
            }
            else if (_isShowToolArea)
            {
                _NodeGraphSize = new Rect(_ToolAreaWidth, NodeGraphDefind.MenuLayoutHeight, position.width - _ToolAreaWidth, position.height - NodeGraphDefind.MenuLayoutHeight);
                _ToolAreaSize = new Rect(0, NodeGraphDefind.MenuLayoutHeight, _ToolAreaWidth, position.height - NodeGraphDefind.MenuLayoutHeight);
                //                _ToolAreaSize = new Rect(0, 0, _ToolAreaWidth, position.height);
                _InspectorAreaSize = new Rect(0, 0, 0, 0);

                Draw_ToolArea(_ToolAreaSize);
                Draw_GUINodeGraph(_NodeGraphSize);
            }
            else if (_isShowInspector)
            {
                _ToolAreaSize = new Rect(0, 0, 0, 0);
                //                _InspectorAreaSize = new Rect(position.width - _InspectorWidth, NodeGraphDefind.MenuLayoutHeight, _InspectorWidth, position.height - NodeGraphDefind.MenuLayoutHeight);
                _InspectorAreaSize = new Rect(position.width - _InspectorWidth, 0, _InspectorWidth, position.height);
                _NodeGraphSize = new Rect(0, NodeGraphDefind.MenuLayoutHeight, position.width - _InspectorWidth, position.height - NodeGraphDefind.MenuLayoutHeight);

                Draw_GUINodeGraph(_NodeGraphSize);
                Draw_Inspector(_InspectorAreaSize);
            }
            else
            {
                _InspectorAreaSize = new Rect(0, 0, 0, 0);
                _ToolAreaSize = new Rect(0, 0, 0, 0);
                _NodeGraphSize = new Rect(0, NodeGraphDefind.MenuLayoutHeight, position.width, position.height - NodeGraphDefind.MenuLayoutHeight);
                Draw_GUINodeGraph(_NodeGraphSize);
            }

            //
            HandleStageEvents();

            //Top draws
            if (_state == NodeGraphModifyState.MutiSelect)
            {
                GUI.color = NodeGraphDefind.MutiSelectionColor;
                GUI.Box(_mutiSelectRect, new GUIContent(), "U2D.createRect");
                GUI.color = NodeGraphDefind.NodeGraphBaseColor;
            }
            if (_state == NodeGraphModifyState.ConnectionDraw)
            {
                //连线绘制表现
                if (_startPointGUI != null)
                {
                    Rect sRect = _startPointGUI.GlobalPointRect;
                    Vector3 startV3 = new Vector3(
                        sRect.x + sRect.width * 0.5f - _NodeGraphCanvasScrollPos.x + (_isShowToolArea ? _ToolAreaWidth : 0),
                        sRect.y + sRect.height * 0.5f - _NodeGraphCanvasScrollPos.y,
                        0f);

                    Vector3 endV3 = new Vector3(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f);

                    ConnectionGUI.DrawConnction(startV3, endV3, Vector3.zero, Vector3.zero, false, false);
                }
            }

            Repaint();
        }

        /// <summary>
        /// 绘制_菜单栏
        /// </summary>
        protected virtual void Draw_menu()
        {

            
            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width), GUILayout.Height(NodeGraphDefind.MenuLayoutHeight));
//            AorGUILayout.Horizontal(() =>
//            {
                if (GUILayout.Button(NodeGraphLagDefind.Get("toolbar"), NodeGraphDefind.GetToolBarBtnStyle(_isShowToolArea), GUILayout.Height(28), GUILayout.Width(_ToolAreaWidth)))
                {
                    _isShowToolArea = !_isShowToolArea;
                }

                if (_NodeGUIList != null && _NodeGUIList.Count > 0)
                {

                    //GUILayout.Space(28);

                    //候选样式名： InnerShadowBg AnimationKeyframeBackground
                    GUILayout.BeginHorizontal("AnimationEventBackground", GUILayout.Height(NodeGraphDefind.MenuLayoutHeight), GUILayout.ExpandWidth(true));

                    //新建Graph
                    if (GUILayout.Button(NodeGraphLagDefind.Get("new") + " Graph", GUILayout.Height(28),
                        GUILayout.Width(120)))
                    {
                        if (string.IsNullOrEmpty(_saveGraphPath))
                        {
                            if (EditorUtility.DisplayDialog(NodeGraphLagDefind.Get("prompt"),
                                "Graph " + NodeGraphLagDefind.Get("nonSave") + "," +
                                NodeGraphLagDefind.Get("needToSave") + "?", NodeGraphLagDefind.Get("save"),
                                NodeGraphLagDefind.Get("cancel")))
                            {
                                SaveGraphToFile();
                            }
                        }
                        else
                        {
                            SaveGraphToFile();
                        }
                        NewNodeGraph();
                    }
                    //打开Graph
                    if (GUILayout.Button(NodeGraphLagDefind.Get("open") + " Graph", GUILayout.Height(28),
                        GUILayout.Width(120)))
                    {
                        if (string.IsNullOrEmpty(_saveGraphPath))
                        {
                            if (EditorUtility.DisplayDialog(NodeGraphLagDefind.Get("prompt"),
                                "Graph " + NodeGraphLagDefind.Get("nonSave") + "," +
                                NodeGraphLagDefind.Get("needToSave") + "?", NodeGraphLagDefind.Get("save"),
                                NodeGraphLagDefind.Get("cancel")))
                            {
                                SaveGraphToFile();
                            }
                        }
                        else
                        {
                            SaveGraphToFile();
                        }
                        OpenGraph();
                    }
                    //保存Graph
                    if (string.IsNullOrEmpty(_saveGraphPath))
                    {
                        if (GUILayout.Button(NodeGraphLagDefind.Get("save"), GUILayout.Height(28),
                            GUILayout.Width(120)))
                        {
                            SaveGraphToFile();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(NodeGraphLagDefind.Get("saveas"), GUILayout.Height(28),
                            GUILayout.Width(120)))
                        {
                            SaveGraphToFile(true);
                        }
                    }

                    GUILayout.FlexibleSpace();

                    if (_MainNode != null)
                    {
                        if (GUILayout.Button(NodeGraphLagDefind.Get("fastFollowRun"), GUILayout.Height(28),
                            GUILayout.Width(120)))
                        {
                            _MainNode.controller.update();
                        }
                    }

                    GUILayout.EndHorizontal();

                }
                else
                {
                    GUILayout.FlexibleSpace();
                }

                if (GUILayout.Button(NodeGraphLagDefind.Get("inspector"), NodeGraphDefind.GetInspectorBtnStyle(_isShowInspector), GUILayout.Height(28), GUILayout.Width(_InspectorWidth)))
                {
                    _isShowInspector = !_isShowInspector;
                }

                //if (_isShowInspector)
                //{
                //  GUILayout.Space(_InspectorWidth - 120);
                //}

//            }, GUILayout.Width(Screen.width), GUILayout.Height(NodeGraphDefind.MenuLayoutHeight));

            GUILayout.EndHorizontal();

        }

        /// <summary>
        /// 此方法由子类覆盖实现处理从NodeToolItem拿到的ToolItemLabelDefine数据到实际显示的数据
        /// </summary>
        /// <returns></returns>
        protected virtual string _GetToolItemLabel(string label)
        {
            return label;
        }

        /// <summary>
        /// ToolItem集合
        /// </summary>
        protected List<NodeGraphToolItemCollection> _ToolAreaItemCollection = new List<NodeGraphToolItemCollection>();

        protected void _addToolItemInCollection(NodeGraphToolItemData toolItemData, string tag = "de")
        {
            if (_ToolAreaItemCollection.Exists(e => e.TAG == tag))
            {
                NodeGraphToolItemCollection coll = _ToolAreaItemCollection.Find(e => e.TAG == tag);
                if (!coll.Contains(toolItemData))
                {
                    coll.Add(toolItemData);
                }
                else
                {
                    throw new Exception("NodeGraphToolItemCollection存在相同的NodeGraphToolItemData: " + toolItemData.ToString() + ".TAG = " + coll.TAG);
                }
            }
            else
            {
                NodeGraphToolItemCollection coll = new NodeGraphToolItemCollection(tag);
                coll.Add(toolItemData);
                _ToolAreaItemCollection.Add(coll);
            }
        }

        protected float _GetAllToolItemHeight()
        {
            float c = 0;
            int i, len = _ToolAreaItemCollection.Count;
            for (i = 0; i < len; i++)
            {
                c += _ToolAreaItemCollection[i].Count * NodeGraphDefind.ToolAreaItemHeight + NodeGraphDefind.ToolAreaItemFoldoutHeight;
            }
            return c;
        }

        protected NodeGraphToolItemCollection _GeToolItemListWithTag(string tag = "default")
        {
            if (_ToolAreaItemCollection.Exists(e => e.TAG == tag))
            {
                return _ToolAreaItemCollection.Find(e => e.TAG == tag);
            }
            else
            {
                throw new Exception("_GeToolItemListWithTag Error : 没有找到查询的tag :: " + tag);
            }
        }

        protected virtual void initToolAreaItems()
        {

            //找出所有INodeGUIController
            string Ns = GetType().Namespace;
            List<Type> GCTypes = new List<Type>();
            Assembly asm = Assembly.GetExecutingAssembly();
            Type[] allTypes = asm.GetTypes();
            int i, len = allTypes.Length;
            for (i = 0; i < len; i++)
            {
                Type t = allTypes[i];
                if (t != null
                    && t.Namespace == Ns
                    && typeof(INodeGUIController).IsAssignableFrom(t)
                    )
                {
                    GCTypes.Add(t);
                }
            }

            if (GCTypes.Count == 0) return;

            //在INodeGUIController集合中找NodeToolItemAttribute
            List<NodeToolItemAttribute> NTIlist = new List<NodeToolItemAttribute>();

            len = GCTypes.Count;
            for (i = 0; i < len; i++)
            {
                Type t = GCTypes[i];
                object[] attributes = t.GetCustomAttributes(typeof(NodeToolItemAttribute), true);
                if (attributes.Length > 0)
                {
                    //约定每个GUIController只能有一个NodeToolItemAttribute
                    NTIlist.Add(attributes[0] as NodeToolItemAttribute);
                    //
                }
            }

            //排序
            NTIlist.Sort((a, b) =>
            {
                if (a.sortId < b.sortId)
                {
                    return -1;
                }
                else if (a.sortId == b.sortId)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            });

            len = NTIlist.Count;
            for (i = 0; i < len; i++)
            {
                NodeToolItemAttribute ntItemAttribute = NTIlist[i];
                //
                Rect itemRect = new Rect(0, 0, _ToolAreaWidth, NodeGraphDefind.ToolAreaItemHeight);
                string data = ntItemAttribute.getFullReflectionParams();
                string label = _GetToolItemLabel(ntItemAttribute.labelDefine);
                string tag = ntItemAttribute.collectionTag;
                if (ntItemAttribute.defaultIntoShortcutMenu)
                {
                    _AddToolItemInShortcutContextMenu(label, data);
                }
                _addToolItemInCollection(new NodeGraphToolItemData(itemRect, label, data, ntItemAttribute.defaultIntoShortcutMenu), tag);
            }

            _ToolAreaScrollCanvas = new Rect(0, 0, NodeGraphDefind.ToolAreaWidth - 15, _GetAllToolItemHeight());
        }

        protected Rect _ToolAreaScrollCanvas;
        protected Vector2 _ToolAreaScrollPos = Vector2.zero;
        protected virtual void Draw_ToolArea(Rect taRect)
        {

            // GUI.BeginGroup(taRect);
            _ToolAreaScrollPos = GUI.BeginScrollView(taRect, _ToolAreaScrollPos, _ToolAreaScrollCanvas, false, true);
            //            _ToolAreaScrollPos = GUILayout.BeginScrollView(_ToolAreaScrollPos, false, true, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true), GUILayout.Width(taRect.width));

            float stHight = 0;
            int i, len;
            int u, ulen = _ToolAreaItemCollection.Count;
            for (u = 0; u < ulen; u++)
            {

                GUILayout.BeginVertical();
                NodeGraphToolItemCollection currentCollection = _ToolAreaItemCollection[u];

                Rect currentCollectionRect = new Rect(0, stHight, taRect.width, NodeGraphDefind.ToolAreaItemFoldoutHeight);
                currentCollection.isFoldout = EditorGUI.Foldout(currentCollectionRect, currentCollection.isFoldout, currentCollection.TAG, NodeGraphDefind.GetToolItemFoldoutStyle());
                stHight += currentCollectionRect.height;

                if (currentCollection.isFoldout)
                {
                    float innerItemHeight = 0;
                    len = currentCollection.Count;
                    for (i = 0; i < len; i++)
                    {
                        Rect r = new Rect(0, stHight + innerItemHeight, currentCollection[i].DefineWidth,
                            currentCollection[i].DefineHeight);
                        currentCollection[i].rect = r;
                        innerItemHeight += currentCollection[i].DefineHeight;
                        GUI.Box(new Rect(r.x, r.y, r.width - 15, r.height), currentCollection[i].label,
                            NodeGraphDefind.GetNodeToolItemStyle());
                    }
                    stHight += innerItemHeight;
                }
                else
                {
                    len = currentCollection.Count;
                    for (i = 0; i < len; i++)
                    {
                        currentCollection[i].rect = new Rect(0, 0, 0, 0);
                    }
                }
                GUILayout.EndVertical();
            }

            //            GUILayout.EndScrollView();
            GUI.EndScrollView();
            //  GUI.EndGroup();
        }

        protected int _activeItemNum = -1;
        protected Vector2 _InspectorScrollPos = Vector2.zero;
        //        protected Rect _InspectorCanvas;

        /// <summary>
        /// 绘制_Inspector区域
        /// </summary>
        /// <param name="inspRect"></param>
        protected virtual void Draw_Inspector(Rect inspRect)
        {
            GUI.BeginGroup(inspRect);

            if (_activeNodeGUIList != null && _activeNodeGUIList.Count > 0)
            {
                //当选择发生变化时重置_InspectorScrollPos
                if (_activeItemNum != _activeNodeGUIList.Count)
                {
                    _activeItemNum = _activeNodeGUIList.Count;
                    _InspectorScrollPos = Vector2.zero;
                }

                _InspectorScrollPos = GUILayout.BeginScrollView(_InspectorScrollPos, false, true, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true), GUILayout.Width(inspRect.width));

                int i, len = _activeNodeGUIList.Count;
                for (i = 0; i < len; i++)
                {
                    _activeNodeGUIList[i].GUIController.DrawNodeInspector(inspRect.width - 25);
                }

                GUILayout.EndScrollView();
            }

            GUI.EndGroup();
        }

        /// <summary>
        /// 修正可能发生的状态错误
        /// (主状态机不是Default状态且MouseDown不为true,则主状态机重置)
        /// </summary>
        protected virtual void FixedEventToState()
        {
            if (_state != NodeGraphModifyState.Default && !_isMouseDown) _state = NodeGraphModifyState.Default;
        }

        protected Vector2 _NodeGraphCanvasScrollPos = Vector2.zero;
        public Vector2 NodeGraphCanvasScrollPos
        {
            get { return _NodeGraphCanvasScrollPos; }
        }

        protected Rect _MinNodeGraphSize = new Rect(0, 0, NodeGraphDefind.NodeGraphMinSizeX, NodeGraphDefind.NodeGraphMinSizeY);

        protected Rect _NodeGraphSize;
        protected Rect _ToolAreaSize;
        protected Rect _InspectorAreaSize;

        protected bool _isMouseDown = false;

        /// <summary>
        /// 绘制_GUINodeGraph区域
        /// </summary>
        protected virtual void Draw_GUINodeGraph(Rect canvasRect)
        {

            int i, len;

            //Canvas移动处理
            if (Event.current.button == 0 && Event.current.isMouse && Event.current.type == EventType.MouseDrag && Event.current.alt)
                _NodeGraphCanvasScrollPos -= Event.current.delta;


            //计算所有NodeGUI的外框
            Rect canvas = new Rect();
            if (_NodeGUIList != null)
            {
                float maxX = float.MinValue, maxY = float.MinValue;
                len = _NodeGUIList.Count;
                for (i = 0; i < len; i++)
                {
                    if (maxX < _NodeGUIList[i].position.x + _NodeGUIList[i].size.x)
                    {
                        maxX = _NodeGUIList[i].position.x + _NodeGUIList[i].size.x;
                    }
                    if (maxY < _NodeGUIList[i].position.y + _NodeGUIList[i].size.y)
                    {
                        maxY = _NodeGUIList[i].position.y + _NodeGUIList[i].size.y;
                    }
                }
                canvas = new Rect(0, 0, Mathf.Max(maxX + NodeGraphDefind.NodeGraphSizeExtX, _MinNodeGraphSize.width), Mathf.Max(maxY + NodeGraphDefind.NodeGraphSizeExtY, _MinNodeGraphSize.height));
            }
            else
            {
                canvas = _MinNodeGraphSize;
            }

            //标记mouseDown事件，以防止GUI.DragWindow()强行暂用mouseDown事件，使后续判断无法成立
            //            _isMouseDown = (Event.current.type == EventType.MouseDown);

            //Begin windows and ScrollView for the nodes.
            _NodeGraphCanvasScrollPos = GUI.BeginScrollView(canvasRect, _NodeGraphCanvasScrollPos, canvas);

            //绘制节点
            if (_NodeGUIList != null)
            {
                BeginWindows();
                len = _NodeGUIList.Count;
                for (i = 0; i < len; i++)
                {
                    NodeGUI node = _NodeGUIList[i];
                    if (_MainNode != null && node == _MainNode)
                    {
                        node.DrawNode(true);
                    }
                    else
                    {
                        node.DrawNode();
                    }
                }
                EndWindows();
            }

            //绘制连线
            if (_ConnectionGUIList != null)
            {
                len = _ConnectionGUIList.Count;
                for (i = 0; i < len; i++)
                {
                    //绘制已有连线
                    _ConnectionGUIList[i].DrawConnection();
                }
            }

            GUI.EndScrollView();
        }

        //-------------------> Event handle

        protected bool _checkNodeGUIsIsDefault()
        {

            if (_NodeGUIList == null) return true;

            int i, len = _NodeGUIList.Count;
            for (i = 0; i < len; i++)
            {
                if (_NodeGUIList[i].state != NodeGUIModifyState.Default)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 框选逻辑
        /// </summary>
        protected void _checkMutiSelect(bool shift, bool ctrl)
        {
            if (_NodeGUIList == null) return;
            //

            //重建MutiSelectRect
            Rect rect = new Rect(
                                    (_mutiSelectRect.width < 0 ? _mutiSelectRect.x + _mutiSelectRect.width : _mutiSelectRect.x) + _NodeGraphCanvasScrollPos.x,
                                    (_mutiSelectRect.height < 0 ? _mutiSelectRect.y + _mutiSelectRect.height : _mutiSelectRect.y) + _NodeGraphCanvasScrollPos.y,
                                    Mathf.Abs(_mutiSelectRect.width),
                                    Mathf.Abs(_mutiSelectRect.height)
                                );

            int i, len;

            if (shift && ctrl)
            {
                //翻转选择模式
                len = _NodeGUIList.Count;
                for (i = 0; i < len; i++)
                {
                    NodeGUI node = _NodeGUIList[i];
                    if (rect.Overlaps(node.rect))
                    {
                        if (_activeNodeGUIList.Contains(node))
                        {
                            RemoveActiveNodeGUI(node);
                        }
                        else
                        {
                            AddActiveNodeGUI(node);
                        }
                    }
                }
                ResetActiveNodeGUIWithoutList();
            }
            else if (shift)
            {
                //+ 模式
                len = _NodeGUIList.Count;
                for (i = 0; i < len; i++)
                {
                    NodeGUI node = _NodeGUIList[i];
                    if (rect.Overlaps(node.rect))
                    {
                        AddActiveNodeGUI(node);
                    }
                }
                ResetActiveNodeGUIWithoutList();
            }
            else if (ctrl)
            {
                //- 模式
                len = _NodeGUIList.Count;
                for (i = 0; i < len; i++)
                {
                    NodeGUI node = _NodeGUIList[i];
                    if (rect.Overlaps(node.rect))
                    {
                        RemoveActiveNodeGUI(node);
                    }
                }
                ResetActiveNodeGUIWithoutList();
            }
            else
            {
                //new 模式
                ClearActiveNodeGUI();
                len = _NodeGUIList.Count;
                for (i = 0; i < len; i++)
                {
                    NodeGUI node = _NodeGUIList[i];
                    if (rect.Overlaps(node.rect))
                    {
                        AddActiveNodeGUI(node);
                    }
                }
            }
        }

        /// <summary>
        /// 事件处理
        /// </summary>
        protected void HandleStageEvents()
        {
            /*
                mouse drag event handling.
            */
            switch (Event.current.type)
            {
                // draw line while dragging.
                case EventType.MouseDrag:
                    {
                        if (_state == NodeGraphModifyState.MutiSelect)
                        {
                            _mutiSelectRect = new Rect(_mutiSelectRect.x, _mutiSelectRect.y,
                            _mutiSelectRect.width + Event.current.delta.x,
                            _mutiSelectRect.height + Event.current.delta.y);
                        }

                        //连线绘制
                        if (_state == NodeGraphModifyState.ConnectionDraw)
                        {
                            //Todo 这里提供连线绘制过程中所需要的数据（也许可以省略）
                        }

                        OnMouseDrag();
                        Event.current.Use();
                        break;
                    }
            }

            /*
                mouse up event handling.
                use rawType for detect for detectiong mouse-up which raises outside of window.
            */
            switch (Event.current.rawType)
            {
                case EventType.MouseUp:
                    {

                        //框选判定 (完毕)
                        if (_state == NodeGraphModifyState.MutiSelect)
                        {
                            _checkMutiSelect(Event.current.shift, Event.current.control);
                        }

                        //连线绘制 (完毕)
                        if (_state == NodeGraphModifyState.ConnectionDraw)
                        {
                            //-- 这里判断是否成功连线
                            if (_startPointGUI != null)
                            {
                                int i, len = _connectionPointGUIList.Count;
                                for (i = 0; i < len; i++)
                                {
                                    Vector3 mouseInputPos = new Vector3(Event.current.mousePosition.x + _NodeGraphCanvasScrollPos.x - (_isShowToolArea ? _ToolAreaWidth : 0),
                                                                        Event.current.mousePosition.y + _NodeGraphCanvasScrollPos.y,
                                                                        0f);

                                    if (_connectionPointGUIList[i].GlobalPointRect.Contains(mouseInputPos))
                                    {

                                        ConnectionPointGUI endPoint = _connectionPointGUIList[i];
                                        if (_startPointGUI.InOutType != endPoint.InOutType)
                                        {
                                            ConnectionPointGUI startPoint = _startPointGUI;
                                            //创建连接
                                            CreateConnection(startPoint, endPoint, false);
                                        }
                                    }
                                }
                                _startPointGUI = null;
                            }
                        }

                        //mouseUp后，state应为NodeGraphModifyState.Default
                        _isMouseDown = false;
                        _state = NodeGraphModifyState.Default;
                        OnMouseUpRaw();
                        break;
                    }
            }

            switch (Event.current.type)
            {
                case EventType.MouseDown:

                    DragAndDrop.PrepareStartDrag();

                    if (_state == NodeGraphModifyState.Default && Event.current.button == 0 && !Event.current.alt)
                    {

                        Vector2 msPos = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y - NodeGraphDefind.MenuLayoutHeight + _ToolAreaScrollPos.y);
                        int dragSeclet = -1;

                        string data = "";

                        int i, len;
                        int u, ulen = _ToolAreaItemCollection.Count;
                        for (u = 0; u < ulen; u++)
                        {
                            if (dragSeclet > -1)
                            {
                                break;
                            }
                            NodeGraphToolItemCollection _collection = _ToolAreaItemCollection[u];
                            len = _collection.Count;
                            for (i = 0; i < len; i++)
                            {
                                if (_collection[i].rect.Contains(msPos))
                                {
                                    dragSeclet = i;
                                    data = _collection[i].data;
                                    break;
                                }
                            }
                        }


                        if (dragSeclet > -1)
                        {
                            _state = NodeGraphModifyState.DragToolItem;

                            DragAndDrop.paths = new[] { data };
                            DragAndDrop.StartDrag("Dragging > " + data);
                        }
                        else
                        {
                            if (_checkNodeGUIsIsDefault() && _NodeGraphSize.Contains(Event.current.mousePosition))
                            {
                                _state = NodeGraphModifyState.MutiSelect;
                            }
                        }
                    }

                    //NodeGraphModifyState.MutiSelect状态异常时，重算多选框位置
                    if (_state == NodeGraphModifyState.MutiSelect)
                    {
                        _mutiSelectRect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1, 1);
                    }

                    _isMouseDown = true;

                    OnMouseDown();
                    break;
                // detect dragging script then change interface to "(+)" icon.
                case EventType.DragUpdated:
                    {
                        if (_state == NodeGraphModifyState.DragToolItem)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        }
                        OnDragUpdated();
                        break;
                    }
                case EventType.DragExited:
                    {
                        OnDragExited();
                        break;
                    }
                // script drop on editor.
                case EventType.DragPerform:
                    {

                        if (_state == NodeGraphModifyState.DragToolItem)
                        {
                            if (_NodeGraphSize.Contains(Event.current.mousePosition))
                            {
                                OnDragToolItem(DragAndDrop.paths[0]);
                            }
                            _state = NodeGraphModifyState.Default;
                        }
                        else
                        {
                            //默认拖入对象识别
                            string[] DDPaths = DragAndDrop.paths;
                            UnityEngine.Object[] DDRefs = DragAndDrop.objectReferences;

                            if (DDRefs != null && DDRefs.Length > 0 && DDPaths != null && DDPaths.Length > 0 && _NodeGraphSize.Contains(Event.current.mousePosition))
                            {
                                //Json拖入(只识别第一个对象)
                                if (DDRefs[0] is TextAsset)
                                {
                                    string suffix = DDPaths[0].Substring(DDPaths[0].LastIndexOf('.'));
                                    TextAsset textAsset = (TextAsset)DDRefs[0];

                                    if (suffix.ToLower() == ".json" && JConfigParser.CheckJsonHeadTagDefind(textAsset.text))
                                    {
                                        if (_NodeGUIList != null && _NodeGUIList.Count > 0)
                                        {
                                            if (string.IsNullOrEmpty(_saveGraphPath))
                                            {
                                                if (EditorUtility.DisplayDialog(NodeGraphLagDefind.Get("prompt"),
                                                    "Graph " + NodeGraphLagDefind.Get("nonSave") + "," + NodeGraphLagDefind.Get("needToSave") +
                                                    "?", NodeGraphLagDefind.Get("save"), NodeGraphLagDefind.Get("cancel")))
                                                {
                                                    SaveGraphToFile();
                                                }
                                            }
                                            else
                                            {
                                                SaveGraphToFile();
                                            }
                                        }

                                        _saveGraphPath = DDPaths[0];
                                        OpenGraphFromJSON(JConfigParser.splitJsonHeadTag(textAsset.text));
                                        Repaint();
                                    }

                                }

                            }
                        }
                        DragAndDrop.AcceptDrag();
                        OnDragPerform();
                        break;
                    }
                // show context menu
                case EventType.ContextClick:
                    {
                        //ContextClick后，state应为NodeGraphModifyState.Default
                        _state = NodeGraphModifyState.Default;
                        //Vector2 inputPos =  - new Vector2((_isShowToolArea ? _ToolAreaWidth : 0),NodeGraphDefind.MenuLayoutHeight);
                        if (_NodeGraphSize.Contains(Event.current.mousePosition))
                        {
                            OnContextMenu();
                        }
                        else if (_isShowToolArea && _ToolAreaSize.Contains(Event.current.mousePosition))
                        {
                            OnToolAreaContextMenu();
                        }
                        else if (_isShowInspector && _InspectorAreaSize.Contains(Event.current.mousePosition))
                        {
                            OnInspectorContextMenu();
                        }

                        if (_isShowToolArea)
                        {
                            int i, len;
                            int u, ulen = _ToolAreaItemCollection.Count;

                            for (u = 0; u < ulen; u++)
                            {
                                NodeGraphToolItemCollection conllection = _ToolAreaItemCollection[u];
                                len = conllection.Count;
                                for (i = 0; i < len; i++)
                                {
                                    if (conllection[i].rect.Contains(new Vector2(Event.current.mousePosition.x,
                                        Event.current.mousePosition.y - NodeGraphDefind.MenuLayoutHeight +
                                        _ToolAreaScrollPos.y
                                        )))
                                    {
                                        OnToolItemContextMenu(conllection[i].label, u, i);
                                    }
                                }
                            }
                        }

                        break;
                    }
                //Handling mouseUp at empty space. 
                case EventType.MouseUp:
                    {
                        DragAndDrop.PrepareStartDrag();
                        OnMouseUp();
                        break;
                    }
                case EventType.KeyDown:
                    {
                        OnKeyDown();
                        break;
                    }
                case EventType.keyUp:
                    OnKeyUp();
                    break;
                case EventType.ValidateCommand:
                    {
                        OnValidateCommand();
                        break;
                    }
                default:
                    break;
            }
        }

        protected virtual void OnMouseDown()
        {
            //Debug.Log("NodeGraphBase.OnMouseDown");
        }

        protected virtual void OnMouseDrag()
        {
            //Debug.Log("NodeGraphBase.OnMouseDrag " + Event.current.mousePosition);
        }

        protected virtual void OnMouseUpRaw()
        {
            //Debug.Log("NodeGraphBase.OnMouseUpRaw");
        }

        protected virtual void OnMouseUp()
        {
            //Debug.Log("NodeGraphBase.OnMouseUp");
        }


        protected virtual void OnDragUpdated()
        {
            //Debug.Log("NodeGraphBase.OnDragUpdated");
        }

        protected virtual void OnDragToolItem(string data)
        {
            //Debug.Log("NodeGraphBase.OnDragToolItem *** " + DragAndDrop.paths[0]);
            var inputPos = Event.current.mousePosition + _NodeGraphCanvasScrollPos;
            NodeGraphToolItemUtility.CallToolItemDefinedMethod(data, inputPos);
        }

        protected virtual void OnDragExited()
        {
            //Debug.Log("NodeGraphBase.OnDragExited");            
        }

        protected virtual void OnDragPerform()
        {
            //Debug.Log("NodeGraphBase.OnDragPerform");
        }

        protected virtual void OnInspectorContextMenu()
        {
        }

        protected virtual void OnToolAreaContextMenu()
        {
        }

        protected virtual void OnToolItemContextMenu(string label, int cIndex, int index)
        {
            var rightClickPos = Event.current.mousePosition + _NodeGraphCanvasScrollPos;
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent(string.Format(NodeGraphLagDefind.Get("addShortcutMenu"), label)), _ToolAreaItemCollection[cIndex][index].isDefaultIntoShortcutMenu, () =>
            {
                _ToolAreaItemCollection[cIndex][index].isDefaultIntoShortcutMenu = !_ToolAreaItemCollection[cIndex][index].isDefaultIntoShortcutMenu;
                if (_ToolAreaItemCollection[cIndex][index].isDefaultIntoShortcutMenu)
                {
                    _AddToolItemInShortcutContextMenu(label, _ToolAreaItemCollection[cIndex][index].data);
                }
                else
                {
                    _RemoveToolItemInShortcutContextMenu(label);
                }
                Repaint();
            });

            menu.ShowAsContext();
        }

        protected Dictionary<string, string> _MainShortcutContextMenuList = new Dictionary<string, string>();

        protected void _AddToolItemInShortcutContextMenu(string label, string data)
        {
            if (!_MainShortcutContextMenuList.ContainsKey(label))
            {
                _MainShortcutContextMenuList.Add(label, data);
            }
        }

        protected void _RemoveToolItemInShortcutContextMenu(string label)
        {
            if (_MainShortcutContextMenuList.ContainsKey(label))
            {
                _MainShortcutContextMenuList.Remove(label);
            }
        }

        protected virtual void OnContextMenu()
        {
            //Debug.Log("NodeGraphBase.OnContextMenu");

            var rightClickPos = Event.current.mousePosition + _NodeGraphCanvasScrollPos;
            var menu = new GenericMenu();

            //绘制主界面ShortcutMenu

            if (_MainShortcutContextMenuList.Keys.Count > 0)
            {

                foreach (string key in _MainShortcutContextMenuList.Keys)
                {

                    string data = _MainShortcutContextMenuList[key];
                    menu.AddItem(new GUIContent(key), false, () =>
                    {
                        //-- 执行ToolItem
                        NodeGraphToolItemUtility.CallToolItemDefinedMethod(data, rightClickPos);
                        Repaint();
                    });
                }
            }

            //            menu.AddItem(new GUIContent("AddNodeData"), false, () =>
            //            {
            //                //---创建 NodeGUI
            //                NodeGraphToolItemDefinder.Instance.AddNodeBase(rightClickPos);
            //                Repaint();
            //            });

            if (_activeNodeGUIList != null && _activeNodeGUIList.Count > 1)
            {
                menu.AddItem(new GUIContent("RemoveActiveNodes"), false, () =>
                {
                    List<NodeGUI> dels = _activeNodeGUIList.Clone();
                    int i, len = dels.Count;
                    for (i = 0; i < len; i++)
                    {
                        RemoveNodeGUI(dels[i]);
                    }
                });
            }

            //固定显示 settingPanel
            menu.AddItem(new GUIContent(NodeGraphLagDefind.Get("settingsPanel")), false, () =>
            {
                NodeGraphSettingWindow.init(_Settings);
            });

            menu.ShowAsContext();
        }

        protected virtual void OnKeyDown()
        {
            //            Debug.Log("NodeGraphBase.OnKeyDown : " + Event.current.keyCode);

        }

        protected virtual void OnKeyUp()
        {
            //默认快捷保存
            if (Event.current.keyCode == KeyCode.S && Event.current.control)
            {
                if (SaveGraphToFile())
                {
                    Debug.Log("NodeGraph.SaveGraphToFile Done !  At " + DateTime.Now.ToShortTimeString());
                }
            }
        }

        protected virtual void OnValidateCommand()
        {

            switch (Event.current.commandName)
            {

                case "Copy":

                    if (_activeNodeGUIList != null && _activeNodeGUIList.Count > 0)
                    {

                        //Undo.RecordObject(this, "COPY NODEGUI");

                        NodeGraphTool.CopyActiveNodes = new List<NodeGUI>();
                        NodeGraphTool.CopyActiveNodes.AddRange(_activeNodeGUIList.ToArray());

                    }
                    break;
                case "Paste":

                    if (NodeGraphTool.CopyActiveNodes != null && NodeGraphTool.CopyActiveNodes.Count > 0)
                    {

                        //Undo.RecordObject(this, "PASTE NODEGUI");

                        NodeGraphTool.PasteNodes(Event.current.mousePosition + _NodeGraphCanvasScrollPos);
                    }
                    break;
                case "Cut":

                    if (_activeNodeGUIList != null && _activeNodeGUIList.Count > 0)
                    {

                        //Undo.RecordObject(this, "CUT NODEGUI");

                        List<NodeGUI> dels = new List<NodeGUI>();
                        NodeGraphTool.CopyActiveNodes = new List<NodeGUI>();
                        for (int i = 0; i < _activeNodeGUIList.Count; i++)
                        {
                            NodeGUI n = NodeGraphTool.CloneNodeGui(_activeNodeGUIList[i]);
                            dels.Add(_activeNodeGUIList[i]);
                        }

                        ClearActiveNodeGUI();

                        for (int i = 0; i < dels.Count; i++)
                        {
                            RemoveNodeGUI(dels[i]);
                        }

                    }
                    break;
                case "SelectAll":

                    _activeNodeGUIList = new List<NodeGUI>();
                    _activeNodeGUIList.AddRange(_NodeGUIList.ToArray());

                    break;
                //                case "UndoRedoPerformed":
                //                    //do nothing
                //
                //                    break;
                case "SoftDelete":
                case "Delete":

                    if (_activeNodeGUIList != null && _activeNodeGUIList.Count > 0)
                    {

                        //Undo.RecordObject(this, "DELETE NODEGUI");

                        List<NodeGUI> dels = new List<NodeGUI>();
                        dels.AddRange(_activeNodeGUIList.ToArray());

                        for (int i = 0; i < dels.Count; i++)
                        {
                            RemoveNodeGUI(dels[i]);
                        }

                    }
                    break;
                default:
                    Debug.Log("NodeGraphBase.OnValidateCommand : " + Event.current.commandName);
                    return;
            }
            Event.current.Use();
        }


        protected virtual void OnSaveGraph()
        {
            //Debug.Log("NodeGraphBase.OnSaveGraph ");
        }

        protected virtual void OnOpenGraph()
        {
            //Debug.Log("NodeGraphBase.OnOpenGraph ");
        }

        protected virtual void OnInstallJSONNodeGraph(Dictionary<string, Dictionary<string, string>> ParmsWithTagDic)
        {
            if (ParmsWithTagDic == null || ParmsWithTagDic.Count == 0) return;
            //（默认）读取“NodeGraphParms”标记参数集合
            if (ParmsWithTagDic.ContainsKey("NodeGraphParms"))
            {
                Dictionary<string, string> parms = ParmsWithTagDic["NodeGraphParms"];
                if (parms != null && parms.Count > 0)
                {
                    setSettingsInFile(parms);
                }
            }

            //////

        }

        //------------------------------- 新建

        //当前Follow的TID，用于识别暂存数据
        protected string _CurrentTID;

        /// <summary>
        /// 新建NodeGraph编辑流
        /// </summary>
        public virtual void NewNodeGraph()
        {
            _CurrentTID = NodeGraphTool.GetTIDCode();
            _NodeGUIList = new List<NodeGUI>();
            _activeNodeGUIList = new List<NodeGUI>();
            _connectionPointGUIList = new List<ConnectionPointGUI>();
            _ConnectionGUIList = new List<ConnectionGUI>();
            _nodeGuiIdSet = new HashSet<long>();

            _saveGraphPath = null;
        }

        //------------------------------- 导入导出

        protected string _saveGraphPath;

        

        //SAVE 当前NodeGraph
        public virtual bool SaveGraphToFile(bool saveAs = false)
        {
            if (_NodeGUIList == null || _NodeGUIList.Count == 0) return false;
            //

            if (saveAs)
            {
                _saveGraphPath = EditorUtility.SaveFilePanel("SaveAs Graph", "", "myNGraph", "json");
                if (string.IsNullOrEmpty(_saveGraphPath)) return false;
            }
            else
            {
                if (string.IsNullOrEmpty(_saveGraphPath))
                {
                    _saveGraphPath = EditorUtility.SaveFilePanel("Save Graph", "", "myNGraph", "json");

                    if (string.IsNullOrEmpty(_saveGraphPath)) return false;
                }
            }

            //事件调用
            OnSaveGraph();

            return SaveGraphToFile(_saveGraphPath);
        }

        private bool SaveGraphToFile(string savePath)
        {
            NodeGraphFile f = new NodeGraphFile(_CurrentTID);

            int i, len = _NodeGUIList.Count;
            for (i = 0; i < len; i++)
            {
                f.InserNode(_NodeGUIList[i]);
            }

            len = _ConnectionGUIList.Count;
            for (i = 0; i < len; i++)
            {
                f.InserConnection(_ConnectionGUIList[i]);
            }

            //存入当前界面参数
            Dictionary<string, string> parmsDic = new Dictionary<string, string>();
            parmsDic.Add("_LAGTag", _LAGTag.ToString());
            parmsDic.Add("_isShowToolArea", _isShowToolArea.ToString());
            parmsDic.Add("_isShowInspector", _isShowInspector.ToString());
            parmsDic.Add("_InspectorWidth", _InspectorWidth.ToString());
            parmsDic.Add("_windowMinSize", "<" + _windowMinSize.x + "," + _windowMinSize.y + ">");
            parmsDic.Add("_NodeGraphCanvasScrollPos", "<" + _NodeGraphCanvasScrollPos.x + "," + _NodeGraphCanvasScrollPos.y + ">");
            parmsDic.Add("_MinNodeGraphSize", "<" + _MinNodeGraphSize.x + "," + _MinNodeGraphSize.y + "," + _MinNodeGraphSize.width + "," + _MinNodeGraphSize.height + ">");
            f.InserParmsDic("NodeGraphParms", parmsDic);

            //save
            string fileStr = f.fileToString();
            string fileHead = JConfigParser.WirteJsonHeadTag("NodeGraph File");
            return AorIO.SaveStringToFile(savePath, fileHead + fileStr);
        }

        //Open NodeGraph
        public virtual void OpenGraph()
        {
            string opend = EditorUtility.OpenFilePanelWithFilters("Open Graph", "", new string[] { "json", "json" });
            if (string.IsNullOrEmpty(opend)) return;

            OnOpenGraph();

            _saveGraphPath = opend;

            OpenGraph(_saveGraphPath);
        }

        public virtual void OpenGraph(string path)
        {
            string src = AorIO.ReadStringFormFile(path);

            if (JConfigParser.CheckJsonHeadTagDefind(src))
            {
                OpenGraphFromJSON(JConfigParser.splitJsonHeadTag(src));
            }
            else
            {
                OpenGraphFromJSON(src);
            }
        }

        public virtual void OpenGraphFromJSON(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                //重置暂存List
                _activeNodeGUIList = new List<NodeGUI>();
                _NodeGUIList = new List<NodeGUI>();
                _connectionPointGUIList = new List<ConnectionPointGUI>();
                _ConnectionGUIList = new List<ConnectionGUI>();
                _nodeGuiIdSet = new HashSet<long>();

                try
                {
                    NodeGraphFile.InstallJSONNodeGraph(json, this, OnInstallJSONNodeGraph);

                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

    }
}
