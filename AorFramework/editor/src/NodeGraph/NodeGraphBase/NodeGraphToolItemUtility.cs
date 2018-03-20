using System;
using System.Reflection;
using AorBaseUtility;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class NodeGraphToolItemUtility
    {

        public static void CallToolItemDefinedMethod(string FullClassNameAtCallData, Vector2 inputPos)
        {
            string[] dataSp = FullClassNameAtCallData.Split('|');
            if (dataSp != null && dataSp.Length >= 3)
            {
                CallToolItemUseGlobalMethod(dataSp[0], dataSp[1], dataSp[2], inputPos);
            }
            else
            {
                Debug.LogError("NodeGraphToolItemUtility.CallToolItemDefinedMethod Error :: Try call CallToolItemDefinedMethod prams error: " + (dataSp == null ? "null" : dataSp.Length.ToString()));
            }
        }

        public static void CallToolItemUseGlobalMethod(string DataClassName, string ControlerClassName, string GUIControllerClassName, Vector2 inputPos)
        {

            Type DataType = Assembly.GetExecutingAssembly().GetType(DataClassName);
            if (DataType == null)
            {
                Debug.LogError("NodeGraphToolItemUtility.CallToolItemUseGlobalMethod Error :: can not find " + DataClassName + " in " + Assembly.GetExecutingAssembly().FullName);
                return;
            }
            object data = Assembly.GetExecutingAssembly().CreateInstance(DataType.FullName,true,BindingFlags.Default,null,new object[]{ NodeGraphBase.Instance.GetNewNodeID() },null,null);
            data.ref_SetField_Inst_NonPublic("m_name", DataType.Name);

            Type ctlType = Assembly.GetExecutingAssembly().GetType(ControlerClassName);
            if (ctlType == null)
            {
                Debug.LogError("NodeGraphToolItemUtility.CallToolItemUseGlobalMethod Error :: can not find " + ControlerClassName + " in " + Assembly.GetExecutingAssembly().FullName);
                return;
            }
            object ctl = Assembly.GetExecutingAssembly().CreateInstance(ctlType.FullName, true, BindingFlags.Default, null, null, null, null);

            Type GUIctlType = Assembly.GetExecutingAssembly().GetType(GUIControllerClassName);
            if (GUIctlType == null)
            {
                Debug.LogError("NodeGraphToolItemUtility.CallToolItemUseGlobalMethod Error :: can not find " + GUIControllerClassName + " in " + Assembly.GetExecutingAssembly().FullName);
                return;
            }
            object gui = Assembly.GetExecutingAssembly().CreateInstance(GUIctlType.FullName, true, BindingFlags.Default, null, null, null, null);

            NodeGUI node = new NodeGUI((INodeData)data, (INodeController)ctl, (INodeGUIController)gui);
            node.position = inputPos;

            node.controller.setup(node);
            node.GUIController.setup(node);

            NodeGraphBase.Instance.AddNodeGUI(node);
        }

    }
}
