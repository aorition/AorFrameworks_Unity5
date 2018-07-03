using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    public class AssetsNodeGraph : NodeGraphBase
    {

        [MenuItem(NodeGraphDefind.MENU_MAIN_DIR + "/AssetsNodeGraph")]
        public static void init()
        {

//            if (NodeGraphBase.Instance != null)
//            {
//                NodeGraphBase.Instance.Close();
//                return;
//            }

            AssetsNodeGraph w = EditorWindow.GetWindow<AssetsNodeGraph>("AssetsNodeGraph");
            w.setup();
        }

        //AssetNodeGraph 对 NodeToolItemAttribute.ToolItemLabelDefine 定义了由 AssetNodeGraphLagDefind类 提供的多语言代替标签
        private Regex TALabelLagIDReg = new Regex("#<[a-zA-Z0-9]+>");

        protected override string _GetToolItemLabel(string label)
        {
            string itemLabel = label;

            if (TALabelLagIDReg.IsMatch(label))
            {
                MatchCollection mc = TALabelLagIDReg.Matches(label);
                int i, len = mc.Count;
                for (i = 0; i < len; i++)
                {
                    Match m = mc[i];
                    string t = m.Value.Substring(2);
                    t = t.Substring(0, t.Length - 1);
                    itemLabel = itemLabel.Replace(m.Value, NodeGraphLagDefind.Get(t));
                }

                return itemLabel;

            }
            else
            {
                return label;
            }
        }

    }
}
