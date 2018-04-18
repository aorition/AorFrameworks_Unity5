using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AorBaseUtility;
using UnityEngine;

namespace Framework.NodeGraph
{
    /// <summary>
    /// 提供底层工具
    /// </summary>
    public class NodeGraphTool
    {

        private static Assembly _currentAssembly;
        /// <summary>
        /// 根据Type完全限定名，创建Type的实例
        /// </summary>
        public static object CreateInstanceByAssembly(string fullName)
        {
            if (_currentAssembly == null) _currentAssembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            try
            {
                object obj = _currentAssembly.CreateInstance(fullName);
                return obj;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return null;
        }
        /// <summary>
        /// 根据Type完全限定名，返回Type
        /// </summary>
        public static Type getTypeInAssembly(string fullName)
        {
            if (_currentAssembly == null) _currentAssembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            return _currentAssembly.GetType(fullName);
        }

        /// <summary>
        /// 载入Texture2D
        /// </summary>
        public static Texture2D LoadTextureFromFile(string path)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(File.ReadAllBytes(path));
            return texture;
        }

        public const int ULONGSIZE = 64;

        /// <summary>
        /// 获取一个根据系统时间生成的唯一ID -> TID
        /// </summary>
        /// <returns></returns>
        public static string GetTIDCode()
        {
            //-- 简易算法（准唯一ID）
            DateTime t = DateTime.Now;
            long time = t.ToBinary();
            ulong utime = (time < 0 ? ulong.MaxValue - ((ulong)-time) :  (ulong)time);

            return StringToMD5(utime.ToString());
        }

        /// <summary>
        /// 获取MD5加密后的字符串(32位加密)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String StringToMD5(String s)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();            
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();
 
            string ret = "";
            for(int i = 0; i<bytes.Length ; i++)
            {                
                ret += Convert.ToString(bytes[i],16).PadLeft(2,'0');
            }
  
            return ret.PadLeft(32,'0');
        }

        public static List<NodeGUI> CopyActiveNodes;

        
        public static void PasteNodes(Vector2 centerPos)
        {
            if (NodeGraphBase.Instance == null || CopyActiveNodes == null || CopyActiveNodes.Count == 0) return;

            int i, len = CopyActiveNodes.Count;

            //计算bounds
            Bounds bounds = new Bounds();
            for (i = 0; i < len; i++)
            {
                if (i == 0)
                {
                    bounds.SetMinMax(new Vector3(CopyActiveNodes[i].position.x, CopyActiveNodes[i].position.y,0), new Vector3(CopyActiveNodes[i].position.x, CopyActiveNodes[i].position.y, 0));
                }
                else
                {
                    bounds.Encapsulate(new Vector3(CopyActiveNodes[i].position.x, CopyActiveNodes[i].position.y, 0));
                }
            }

            for (i = 0; i < len; i++)
            {
                NodeGUI n = CloneNodeGui(CopyActiveNodes[i]);
                Vector2 npos = new Vector2(
                                                centerPos.x + n.position.x - bounds.size.x * 0.5f,
                                                centerPos.y + n.position.y - bounds.size.y * 0.5f
                                            );

                n.data.ref_SetField_Inst_NonPublic("m_position", npos);
                NodeGraphBase.Instance.AddNodeGUI(n);
            }
        }

        public static NodeGUI CloneNodeGui(NodeGUI src)
        {
            //优先 data
            Type dataType = src.data.GetType();
            object data = CreateInstanceByAssembly(dataType.FullName);

            FieldInfo[] dataFieldInfos = dataType.GetFields(BindingFlags.Instance| BindingFlags.NonPublic| BindingFlags.Public| BindingFlags.GetField);
            foreach (FieldInfo dataFieldInfo in dataFieldInfos)
            {
                if (dataFieldInfo.Name == "id")
                {
                    long newNodeId = (long)NodeGraphBase.Instance.GetNewNodeID();
                    dataFieldInfo.SetValue(data, newNodeId);
                }
                else
                {
                    dataFieldInfo.SetValue(data, dataFieldInfo.GetValue(src.data));
                }
            }

            //controller
            object controller = null;
            Type controllerType = src.controller.GetType();
            ConstructorInfo[] cinfos = controllerType.GetConstructors();
            if (cinfos != null && cinfos.Length > 0)
            {
                //约定NodeController只能有一个构造函数:NodeController(INodeData data)
                controller = cinfos[0].Invoke(new object[] { data });
            }

            //GUIController
            object GUIController = CreateInstanceByAssembly(src.GUIController.GetType().FullName);

            //NodeGUI
            NodeGUI nodeGui = new NodeGUI((INodeData)data, (INodeController)controller, (INodeGUIController)GUIController);

            //将Node传递给GUIController
            GUIController.ref_SetField_Inst_NonPublic("m_nodeGUI", nodeGui);

            return nodeGui;

        }

        public static float GetConnectCenterTipLabelWidth(string info)
        {
            if (string.IsNullOrEmpty(info))
            {
                return 100f;
            }
            else
            {
                return info.Length * NodeGraphDefind.ConnectCenterTipLabelPreWidth + NodeGraphDefind.ConnectCenterTipMargin * 2;
            }
        }
    }
}
