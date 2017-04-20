using AorFramework.NodeGraph.Utility;
using System;
using System.Collections.Generic;
//using AorFramework.NodeGraph.Utility;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph.Tool
{
    public class NodeGraphSettingWindow : EditorWindow
    {

        public static NodeGraphSettingWindow init(NodeGraphBaseSetting setting)
        {
            NodeGraphSettingWindow w = EditorWindow.GetWindow<NodeGraphSettingWindow>("Settings",true,typeof(AssetsNodeGraph));
            w.setup(setting);
            return w;
        }

        private NodeGraphBaseSetting _setting;
        private bool _isInit = false;

        public void setup(NodeGraphBaseSetting setting)
        {
            _setting = setting;
            _isInit = true;
        }

        private void OnGUI()
        {

            #region 编译中
            if (EditorApplication.isCompiling)
            {
                ShowNotification(new GUIContent("Compiling Please Wait..."));
                Repaint();
                return;
            }
            RemoveNotification();
            #endregion

            if (_setting == null)
            {
                GUILayout.Label("未初始化 ... ...");
                GUILayout.FlexibleSpace();
                AorGUILayout.Horizontal(() =>
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(Screen.width + " , " + Screen.height);
                });
                return;
            }

            //TODO 工具箱快捷项目管理

            //TODO 用户自建快捷菜单管理 （自建快捷菜单功能还没写 。。。）


            //test
            if (GUILayout.Button("test"))
            {
                string p = EditorUtility.SaveFilePanel("...", "", "", "txt");
                if (!string.IsNullOrEmpty(p))
                {
                    LayoutUtility.SaveLayoutToAsset(p);
                }
            }
            
        }

    }
}
