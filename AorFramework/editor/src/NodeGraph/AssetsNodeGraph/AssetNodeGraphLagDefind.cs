﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AorFramework.NodeGraph
{
    public class AssetNodeGraphLagDefind
    {

        /// <summary>
        /// 获取Label (通过AssetNodeGraphLagDefind定义)
        /// </summary>
        public static string GetLabelDefine(int lagID)
        {
            if (NodeGraphBase.Instance != null) return NodeGraphLagDefind.GetLabelDefine<AssetNodeGraphLagDef>(NodeGraphBase.Instance.LAGTag, lagID);
            return NodeGraphLagDefind.GetLabelDefine<AssetNodeGraphLagDef>(NodeGraphLagDefind.NodeGraphLagTag.EN, lagID);
        }

        public enum AssetNodeGraphLagDef : int
        {
            ImportRes, 资源输入,//0
            ResFilter, 资源过滤,//1
            RemoveActiveNodes, 删除选中节点,//2
            PrefabProcessor, Prefab处理器,//3
            CustomScript, 自定义脚本,//4
            Description,描述,//5
            ResultInfo,结果信息,//6
            Input,输入,//7
            Output,输出,//8
            Prefab,预制体,//9
            Path,路径,//10
            ObjectSelector,对象选择器,//11
            Selector,选择器,//12
            Add_0_To_ShortcutMenu,添加_0_到快捷菜单,//13
        }


    }
}
