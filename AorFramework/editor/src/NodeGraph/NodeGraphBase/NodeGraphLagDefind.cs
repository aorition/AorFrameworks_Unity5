using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Framework.NodeGraph
{
    public class NodeGraphLagDefind
    {

        public enum NodeGraphLagTag : int
        {
            EN = 0,
            CH,
        }

        /// <summary>
        /// 获取Label (通过NodeGraphLagDefind定义)
        /// </summary>
        public static string GetLabelDefine(int lagID)
        {
            if (NodeGraphBase.Instance != null) return NodeGraphLagDefind.GetLabelDefine<NodeGraphLagDef>(NodeGraphBase.Instance.LAGTag, lagID);
            return NodeGraphLagDefind.GetLabelDefine<NodeGraphLagDef>(NodeGraphLagDefind.NodeGraphLagTag.EN, lagID);
        }

        public static Regex _repReg = new Regex("\\d+");

        /// <summary>
        /// 转换AssetNodeGraphLagDef中的数字为{数字}
        /// </summary>
        private static string _transSharpToStringFormatTag(string src)
        {
            if (_repReg.IsMatch(src))
            {
                MatchCollection mc = _repReg.Matches(src);
                int i, len = mc.Count;
                for (i = 0; i < len; i++)
                {
                    string d = mc[i].Value;
                    src = src.Replace(mc[i].Value, "{" + d + "}");
                }
                return src;
            }
            else
            {
                return src;
            }
        }

        public static string GetLabelDefine<T>(NodeGraphLagTag tag, int lagID) where T : struct
        {
            if (typeof(Enum) != typeof(T).BaseType)
            {
                throw new Exception("The T type parameter for the current generic class must be an enumerated type.");
            }

            int defId = lagID*Enum.GetNames(typeof (NodeGraphLagTag)).Length + (int) tag;
            string defStr = Enum.Parse(typeof (T), defId.ToString()).ToString();
            return _transSharpToStringFormatTag(defStr.Replace("_"," "));
        }

        public enum NodeGraphLagDef : int
        {
            test, 测试,//0
            WelCome_to_use_NodeGraph, 欢迎使用_NodeGraph,//1
            New, 新建,//2
            Open, 打开,//3
            NodeGraph_has_been_not_Initialization, NodeGraph_未初始化,//4
            Prompt,提示,//5
            has_not_saved, 尚未保存,//6
            need_to_save, 是否保存,//7
            Save,保存,//8
            Canel,取消,//9
            SaveAs,保存到,//10
            Inspector, INSPECTOR,//11
            This_Node_need_update_data,此节点需要更新数据,//12
            ToolBar,工具箱,//13
            Delete,删除,//14
            Set_Node_As_MainNode,将Node设为MainNode,//15
            Cancel_Set_MainNode,取消设置为MainNode,//16
            FastFollowRun,快捷执行,//17
            SettingsPanel,设置面板,//18
        }
    }
}
