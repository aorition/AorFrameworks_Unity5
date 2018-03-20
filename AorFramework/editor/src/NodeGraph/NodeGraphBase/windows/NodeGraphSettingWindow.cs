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
                GUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();
                GUILayout.Label(Screen.width + " , " + Screen.height);

                GUILayout.EndHorizontal();
                return;
            }

            GUILayout.BeginVertical(GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));

                //tilte Area
                GUILayout.BeginVertical("box", GUILayout.Height(Screen.height*0.12f), GUILayout.ExpandWidth(true));
                    GUILayout.FlexibleSpace();
                    draw_titleArea();
                    GUILayout.FlexibleSpace();
               GUILayout.EndVertical();

                //Base main setting Area
                GUILayout.BeginVertical("box",GUILayout.Height(Screen.height * 0.8f),GUILayout.ExpandWidth(true));
                draw_mainArea();
                GUILayout.EndVertical();

                //Base btn Area
                GUILayout.BeginVertical("box", GUILayout.Height(Screen.height * 0.08f), GUILayout.ExpandWidth(true));
                draw_settingMenuArea();
                GUILayout.EndVertical();

            GUILayout.EndVertical();
            
        }

        private void draw_titleArea()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("NodeGraph Settings : ");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private int _mainTab_id = 0;
        private string[] _mainTabLabels = { "BasicSetting", "SortcutCMenuSupervise", "QuickMenuSupervise" };

        private void draw_mainArea()
        {
            _mainTab_id = GUILayout.Toolbar(_mainTab_id, _mainTabLabels);
            switch (_mainTab_id)
            {
                case 1:
                    draw_mainArea_1();
                    break;
                case 2:
                    draw_mainArea_2();
                    break;
                default:
                    draw_mainArea_0();
                    break;
            }
        }

        //BasicSetting
        private void draw_mainArea_0()
        {
            GUILayout.Label("BasicSetting");
        }

        //SortcutCMenuSupervise
        private void draw_mainArea_1()
        {

            //TODO 工具箱快捷项目管理

            GUILayout.Label("SortcutCMenuSupervise");
        }

        //QuickMenuSupervise
        private void draw_mainArea_2()
        {

            //TODO 用户自建快捷菜单管理 （自建快捷菜单功能还没写 。。。）

            GUILayout.Label("QuickMenuSupervise");
        }

        private void draw_settingMenuArea()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply",GUILayout.Width(120)))
            {
                //
                
                Close();
            }
            if (GUILayout.Button("Cancel", GUILayout.Width(120)))
            {
                //
                Close();
            }
            GUILayout.EndHorizontal();
        }

    }
}
