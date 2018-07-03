using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AorBaseUtility;
using AorBaseUtility.Config;
using AorBaseUtility.MiniJSON;
using UnityEngine;
using Framework.core;

namespace Framework.NodeGraph
{
    public class NodeGraphFile
    {

        /// <summary>
        /// 装载JSON数据，到NodeGraph编辑器
        /// </summary>
        /// <param name="src">JSON数据</param>
        /// <param name="manager">NodeGraph编辑器</param>
        public static void InstallJSONNodeGraph(string src, NodeGraphBase manager, Action<Dictionary<string, Dictionary<string, string>>> finishCallBack)
        {

            Dictionary<string, string> usingTypeDic = new Dictionary<string, string>();

            Dictionary<string, object> SrcDic = Json.DecodeToDic(src);
            if (SrcDic != null)
            {
                int i, len;
                //TID
                manager.ref_SetField_Inst_NonPublic("_CurrentTID", SrcDic["TID"].ToString());

                //Using
                IList usinglist = (IList)SrcDic["Using"];
                len = usinglist.Count;
                for (i = 0; i < len; i++)
                {
                    usingTypeDic.Add(i.ToString(), usinglist[i].ToString());
                }

                //创建 NodeGUI && idSet && TreeNode
                IList nodes = (IList) SrcDic["Nodes"];
                len = nodes.Count;
                for (i = 0; i < len; i++)
                {
                    Dictionary<string, object> nDic = (Dictionary<string, object>) nodes[i];

                    //优先 data
                    string dataTypeIdx = nDic["DataType"].ToString();
                    Dictionary<string, object> dDic = (Dictionary<string, object>)nDic["Data"];
                    Type dataType = NodeGraphTool.getTypeInAssembly(usingTypeDic[dataTypeIdx]);
                    object data = JConfigParser.ToConfig(dataType, dDic);

                    //controller
                    //object controller = null;
                    string controllerIdx = nDic["Controller"].ToString();
                    object controller = NodeGraphTool.CreateInstanceByAssembly(usingTypeDic[controllerIdx]);
//                    Type controllerType = NodeGraphTool.getTypeInAssembly(usingTypeDic[controllerIdx]);
//                    ConstructorInfo[] cinfos = controllerType.GetConstructors();
//                    if (cinfos != null && cinfos.Length > 0)
//                    {
//                        //约定NodeController只能有一个构造函数:NodeController(INodeData data)
//                        controller = cinfos[0].Invoke(new object[] {data});
//                    }

                    //GUIController
                    string GUIControllerIdx = nDic["GUI"].ToString();
                    object GUIController = NodeGraphTool.CreateInstanceByAssembly(usingTypeDic[GUIControllerIdx]);

                    //NodeGUI
                    NodeGUI nodeGui = new NodeGUI((INodeData)data,(INodeController)controller,(INodeGUIController)GUIController);
                    nodeGui.position = (Vector2)JConfigParser.ParseValue(typeof (Vector2), nDic["Pos"].ToString());
                    nodeGui.size = (Vector2)JConfigParser.ParseValue(typeof(Vector2), nDic["Size"].ToString());

                    //将Node传递给GUIController
                    controller.InvokePublicMethod("setup", new object[] {nodeGui});
                    GUIController.InvokePublicMethod("setup", new object[] {nodeGui});
//                    GUIController.ref_SetField_Inst_NonPublic("m_nodeGUI", nodeGui);

                    //isMain
                    bool isMain = bool.Parse(nDic["isMain"].ToString());

                    if (data != null && controller != null && GUIController != null)
                    {
                        manager.AddNodeGUI(nodeGui);

                        if (isMain)
                        {
                            manager.SetMainNode(nodeGui);
                        }
                    }
                    else
                    {
                        Debug.LogError("*** NodeGraphFile.InstallJSONNodeGraph Error :: Build NodeGUI fail ,NodeGUI.id = " + (data == null ? "null" : ((INodeData)data).id.ToString()) +
                                       " ,data = " + (data == null ? "null" : data.ToString()) + ",controller = " +(controller == null ? "null" : controller.ToString()) + ",GUIController = " + (GUIController == null ? "null" : GUIController.ToString()) + ".");
                    }

                }

                //创建 ConnectionGUI
                IList connectInfos = (IList)SrcDic["Connections"];
                len = connectInfos.Count;
                for (i = 0; i < len; i++)
                {
                    Dictionary<string, object> cDic = (Dictionary<string, object>) connectInfos[i];

                    //Output ConnectionPointGUI
                    int nid_out = int.Parse(cDic["OutNode"].ToString());
                    long pid_out = long.Parse(cDic["OutPoint"].ToString());
                    ConnectionPointGUI output = manager.GetConnectionPointGui(nid_out, pid_out, ConnectionPointInoutType.Output);

                    //Input ConnectionPointGUI
                    int nid_in = int.Parse(cDic["InNode"].ToString());
                    long pid_in = long.Parse(cDic["InPoint"].ToString());
                    ConnectionPointGUI input = manager.GetConnectionPointGui(nid_in, pid_in, ConnectionPointInoutType.Input);

                    manager.CreateConnection(output, input, false);
                }

                //创建 ParmsDics
                IList parmDicsList = (IList)SrcDic["ParmsTAGDic"];
                if (parmDicsList != null && parmDicsList.Count > 0)
                {

                    Dictionary<string, Dictionary<string, string>> ParmsWithTagDic =  new Dictionary<string, Dictionary<string, string>>();

                    len = parmDicsList.Count;
                    for (i = 0; i < len; i++)
                    {
                        Dictionary<string, object> sub = (Dictionary<string, object>) parmDicsList[i];
                        
                        string tag = sub["tag"].ToString();
                        Dictionary<string,string> dic = new Dictionary<string, string>();

                        IList list = (IList)sub["dic"];
                        if (list != null && list.Count > 0)
                        {
                            for (int j = 0; j < list.Count; j++)
                            {
                                Dictionary<string, object> ss = (Dictionary<string, object>) list[j];
                                foreach (string key in ss.Keys)
                                {
                                    dic.Add(key, ss[key].ToString());
                                }
                            }
                        }

                        if (dic.Count > 0)
                        {
                            ParmsWithTagDic.Add(tag, dic);
                        }
                    }

                    if (finishCallBack != null && ParmsWithTagDic.Count > 0) finishCallBack(ParmsWithTagDic);

                }
            }
        }

        private StringBuilder _header;
        private StringBuilder _nodes;
        private StringBuilder _connects;

        private List<string> _usingList;

        private StringBuilder _parmsWirter;

        private StringBuilder _inserKV;

        public NodeGraphFile(string TID)
        {
            _header = new StringBuilder();
            _nodes = new StringBuilder();
            _connects = new StringBuilder();

            _usingList = new List<string>();

            _parmsWirter = new StringBuilder();

            //TID
            _header.Append("{\"TID\": \"" + TID + "\",");

            //Using
            _header.Append("\"Using\":[");

            _nodes.Append("\"Nodes\":[");
            _connects.Append("\"Connections\":[");

            //parmsDic
            _parmsWirter.Append("\"ParmsTAGDic\":[");

        }

        private int _inserKVNum = 0;
        /// <summary>
        /// 插入KeyValue
        /// </summary>
        public void InserKeyVale(string key, object value)
        {
            if(_inserKV == null) _inserKV = new StringBuilder();

            if (_inserKVNum > 0) _inserKV.Append(",");

            _inserKV.Append("\"" + key + "\":");

            _inserKV.Append(JConfigParser._ObjToJSON(value, value.GetType()));

            _inserKVNum ++;
        }
        
        private int _CheckUsing(string TypeFullName)
        {
            int idx;
            if (_usingList.Contains(TypeFullName))
            {
                idx = _usingList.IndexOf(TypeFullName);
            }
            else
            {
                _usingList.Add(TypeFullName);
                idx = _usingList.Count - 1;
            }
            return idx;
        }

        private int _nodesNum = 0;
        public void InserNode(NodeGUI node)
        {
            if (_nodesNum > 0) _nodes.Append(",");

            StringBuilder builder = new StringBuilder("{");

            //isMain
            if (NodeGraphBase.Instance.MainNode != null && NodeGraphBase.Instance.MainNode == node)
            {
                builder.Append("\"isMain\":\"true\",");
            }
            else
            {
                builder.Append("\"isMain\":\"false\",");
            }

            //pos
            builder.Append("\"Pos\":\"<" + node.position.x + "," + node.position.y + ">\",");

            //size
            builder.Append("\"Size\":\"<" + node.size.x + "," + node.size.y + ">\",");

            //controller
            builder.Append("\"Controller\":\"" + _CheckUsing(node.controller.GetType().FullName).ToString() + "\",");

            //GUIController
            builder.Append("\"GUI\":\"" + _CheckUsing(node.GUIController.GetType().FullName).ToString() + "\",");

            //DataType
            builder.Append("\"DataType\":\"" + _CheckUsing(node.data.GetType().FullName).ToString() + "\",");

            //data
            builder.Append("\"Data\":");
            string value = JConfigParser.ToJSON((Config)node.data);
            string tagHead = "";
            value = JConfigParser.splitJsonHeadTag(value, ref tagHead);
            builder.Append(value);

            //
            builder.Append("}");

            _nodes.Append(builder.ToString());

            _nodesNum++;
        }

        private int _connectionNum = 0;
        public void InserConnection(ConnectionGUI connection)
        {
            if (_connectionNum > 0) _connects.Append(",");

            StringBuilder builder = new StringBuilder("{");

            //out
            builder.Append("\"OutNode\":\"" + connection.OutputPointGui.node.data.id + "\",");
            builder.Append("\"OutPoint\":\"" + connection.OutputPointGui.id + "\",");
            //builder.Append("\"OutType\":\"" + connection.OutputPointGui.DataTypeName + "\",");
            //in
            builder.Append("\"InNode\":\"" + connection.InputPointGui.node.data.id + "\",");
            builder.Append("\"InPoint\":\"" + connection.InputPointGui.id + "\"");
            //builder.Append("\"InType\":\"" + connection.InputPointGui.DataTypeName + "\"");
            
            builder.Append("}");
            _connects.Append(builder.ToString());

            _connectionNum++;
        }

        private int _parmDicNum = 0;
        //写入其他参数
        public void InserParmsDic(string parmsTAG, Dictionary<string, string> keyValue)
        {
            if (_parmDicNum > 0) _parmsWirter.Append(",");

            StringBuilder builder = new StringBuilder("{");

            builder.Append("\"tag\":\"" + parmsTAG + "\",");
            builder.Append("\"dic\":[");

            int k = 0;
            foreach (string key in keyValue.Keys)
            {
                if (k > 0) builder.Append(",");
                builder.Append("{\"" + key + "\":\"" + keyValue[key] + "\"}");
                k ++;
            }

            builder.Append("]}");
            _parmsWirter.Append(builder.ToString());

            _parmDicNum++;
        }

        public string fileToString()
        {

            int i, len = _usingList.Count;
            for (i = 0; i < len; i++)
            {
                if (i > 0) _header.Append(",");

                _header.Append("\"" + _usingList[i] + "\"");
            }

            return _header.ToString() + "]," +
                   (_inserKV != null ? _inserKV.ToString() + "," : "") +
                   _nodes.ToString() + "]," +
                   _connects.ToString() + "]," +
                   _parmsWirter.ToString() + "]}";
        }

    }
}
