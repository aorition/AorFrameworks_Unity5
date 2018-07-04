using System;
using System.Collections.Generic;
using AorBaseUtility;
using Framework.Editor;
using NodeGraph.SupportLib;
using UnityEditor;
using UnityEngine;

namespace Framework.NodeGraph
{
    [NodeToolItem("#<reference>",
        "Framework.NodeGraph",
        "CheckReferenceData|CheckReferenceController|CheckReferenceGUIController",
        "Action")]
    public class CheckReferenceGUIController : NodeGUIController
    {

       // public static string[] FKModeLabelDefine = {"无", "预制体", "场景", "图像", "材质", "FBX", "Shader", "字体"};

        public enum ActionID 
        {
            non=0,
            FindReference_Up=1,
            FindReference_Down=2,
            FindReference_cross= 3,
            FindREference_None=4,
        }

        public override string GetNodeLabel()
        {
            return NodeGraphLagDefind.Get("reference");
        }

        private Vector2 _MinSizeDefind = new Vector2(200, 120);
        public override Vector2 GetNodeMinSizeDefind()
        {
            return _MinSizeDefind;
        }

        public override void DrawConnectionTip(Vector3 centerPos, ConnectionGUI connection)
        {
            //string
            string info = "0";
            object ConnectionValue = connection.GetConnectionValue(false);
            if (ConnectionValue != null)
            {
                if (ConnectionValue is Array)
                {
                    info = (ConnectionValue as Array).Length.ToString();
                }
            }

            //size
            Vector2 CTSzie = new Vector2(NodeGraphTool.GetConnectCenterTipLabelWidth(info) + 4, NodeGraphDefind.ConnectCenterTipLabelPreHeight);

            //rect
            connection.CenterRect = new Rect(centerPos.x - CTSzie.x * 0.5f, centerPos.y - CTSzie.y * 0.5f, CTSzie.x, CTSzie.y);

            //ConnectionTip
            GUI.Label(connection.CenterRect, info, GetConnectCenterTipStyle());

            //右键菜单检测
            if (Event.current.button == 1 && Event.current.isMouse && connection.CenterRect.Contains(Event.current.mousePosition))
            {
                DrawCenterTipContextMenu(connection);
                Event.current.Use();
            }
        }

        //        private 
        //        public override Vector2 GetNodeMinSizeDefind()
        //        {
        //            return base.GetNodeMinSizeDefind();
        //        }

        public override void DrawNodeInspector(float inspectorWidth)
        {
            if (m_nodeGUI == null) return;

            GUILayout.BeginVertical("box", GUILayout.Width(inspectorWidth));

          
            int _ActionID = (int)m_nodeGUI.data.ref_GetField_Inst_Public("ActionID");
            if (_ActionID == null) _ActionID = 0;
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            ActionID _actionid = (ActionID)EditorGUILayout.EnumPopup("选择处理动作", (ActionID)_ActionID);
            GUILayout.Space(15);

            GUILayout.EndHorizontal();
            if ((int)_actionid != _ActionID)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("ActionID", (int)_actionid);
                m_nodeGUI.data.ref_SetField_Inst_Public("Redependence", null);
                m_nodeGUI.data.ref_SetField_Inst_Public("Redependence_Down", null);
            }
            if ((int )_actionid == 0)
            {
               
            }
            if ((int )_actionid == 1)
            {
                ActionOne();
            }
            if((int )_actionid ==2)
            {
                ActionTwo();
            }
            if ((int)_actionid == 3)
            {
                ActionThree();
            }
            if ((int)_actionid == 4)
            {
                ActionFour();
            }


            GUILayout.EndVertical();

            base.DrawNodeInspector(inspectorWidth);
           
        }

      

        //改变
        //{"无", "预制体", "图像", "材质", "FBX", "Shader"};
        private void _changeFKMode(int mode)
        {
            //m_nodeGUI.data.ref_SetField_Inst_Public("FilterMode", mode);

            List<string> keys = new List<string>();
            List<bool> IgnoreCs = new List<bool>();

            switch (mode)
            {
                //预制体
                case 1:
                    keys.Add(".prefab");    IgnoreCs.Add(true);
                    break;
                //场景
                case 2:
                    keys.Add(".unity"); IgnoreCs.Add(true);
                    break;
                //图像
                case 3:
                    keys.Add(".jpg");   IgnoreCs.Add(true);
                    keys.Add(".gif");   IgnoreCs.Add(true);
                    keys.Add(".bmp");   IgnoreCs.Add(true);
                    keys.Add(".tiff");   IgnoreCs.Add(true);
                    keys.Add(".iff");   IgnoreCs.Add(true);
                    keys.Add(".pict");   IgnoreCs.Add(true);
                    keys.Add(".dds");   IgnoreCs.Add(true);
                    keys.Add(".jpeg");  IgnoreCs.Add(true);
                    keys.Add(".png");   IgnoreCs.Add(true);
                    keys.Add(".tga");   IgnoreCs.Add(true);
                    keys.Add(".exr");   IgnoreCs.Add(true);
                    keys.Add(".psd");   IgnoreCs.Add(true);
                    break;
                //材质
                case 4:
                    keys.Add(".mat"); IgnoreCs.Add(true);
                    break;
                //FBX
                case 5:
                    keys.Add(".fbx"); IgnoreCs.Add(true);
                    break;
                //Shader
                case 6:
                    keys.Add(".shader"); IgnoreCs.Add(true);
                    break;
                //Font
                case 7:
                    keys.Add(".ttf"); IgnoreCs.Add(true);
                    keys.Add(".otf"); IgnoreCs.Add(true);
                    break;
                default:
                    //
                    break;
            }

            if (keys.Count > 0 && IgnoreCs.Count > 0)
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("FilterKeys", keys.ToArray());
                m_nodeGUI.data.ref_SetField_Inst_Public("IgnoreCase", IgnoreCs.ToArray());
            }
            else
            {
                m_nodeGUI.data.ref_SetField_Inst_Public("FilterKeys", null);
                m_nodeGUI.data.ref_SetField_Inst_Public("IgnoreCase", null);
            }

            m_nodeGUI.SetDirty();
        }

        public override List<ConnectionPointGUI> GetConnectionPointInfo(GetConnectionPointMode GetMode)
        {
            if (_ConnectionPointGUIList == null)
            {
                ConnectionPointGUI p0 = new ConnectionPointGUI(100, 0, 1, typeof(string[]).Name, "input", m_nodeGUI, NodeGraphLagDefind.Get("input"), new Vector2(100,60),  ConnectionPointInoutType.MutiInput);
                ConnectionPointGUI p1 = new ConnectionPointGUI(101, 0, 1, typeof(string[]).Name, "AssetsPath", m_nodeGUI, NodeGraphLagDefind.Get("output"), new Vector2(120, 60), ConnectionPointInoutType.Output);
                _ConnectionPointGUIList = new List<ConnectionPointGUI>() {p0, p1};
            }

            return _GetConnectionPointsByMode(GetMode);
        }




        private void ActionOne()
        {
            GUILayout.Label("以选查找引用路径:");
            string[] SearchPath = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("SearchPath");
            if (SearchPath == null || SearchPath.Length < 1)
            {

                if (GUILayout.Button("SelectSearchPath"))
                {
                    List<string> _searchPath = new List<string>();
                    if (Selection.objects != null && Selection.objects.Length > 0)
                    {

                        for (int i = 0; i < Selection.objects.Length; i++)
                        {
                            _searchPath.Add(AssetDatabase.GetAssetPath(Selection.objects[i]));
                        }

                    }
                    else
                    {
                        _searchPath[0] = EditorUtility.OpenFolderPanel("指定搜索文件夹", null, null);
                    }
                    m_nodeGUI.data.ref_SetField_Inst_Public("SearchPath", _searchPath.ToArray());
                }
            }
            else
            {

                for (int i = 0; i < SearchPath.Length; i++)
                {
                    GUILayout.Label(SearchPath[i]);
                }

                if (GUILayout.Button("SelectSearchPath"))
                {
                    List<string> _searchPath = new List<string>();
                    if (Selection.objects != null && Selection.objects.Length > 0)
                    {

                        for (int i = 0; i < Selection.objects.Length; i++)
                        {
                            _searchPath.Add(AssetDatabase.GetAssetPath(Selection.objects[i]));
                        }

                    }
                    else
                    {
                        _searchPath[0] = EditorUtility.OpenFolderPanel("指定搜索文件夹", null, null);
                    }
                    m_nodeGUI.data.ref_SetField_Inst_Public("SearchPath", _searchPath.ToArray());
                }


            }

            Dictionary<string, List<string>> _redependence = (Dictionary<string, List<string>>)m_nodeGUI.data.ref_GetField_Inst_Public("Redependence");
            if (_redependence != null && _redependence.Count > 0)
            {
                foreach (KeyValuePair<string, List<string>> pair in _redependence)
                {

                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();

                    GUILayout.Label("引用对象");
                    string[] _st = pair.Key.Split('/');
                    if (GUILayout.Button(_st[_st.Length - 1]))
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Key);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();

                    GUILayout.Label("所有引用" + pair.Value.Count);
                    if (GUILayout.Button("全选"))
                    {
                        List<UnityEngine.Object> _ob = new List<UnityEngine.Object>();
                        for (int i = 0; i < pair.Value.Count; i++)
                        {
                            _ob.Add(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Value[i]));
                        }
                        Selection.objects = _ob.ToArray();
                    }
                    GUILayout.EndHorizontal();
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        string[] _str = pair.Value[i].Split('/');
                        if (GUILayout.Button(_str[_str.Length - 1]))
                        {
                            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Value[i]);
                        }
                        GUILayout.Space(1);
                    }


                }
            }
            GUILayout.Space(10);


            GUILayout.Space(10);
            if (GUILayout.Button("Update"))
            {
                if (m_nodeGUI.data.ref_GetField_Inst_Public("SearchPath") != null)
                {
                    m_nodeGUI.controller.update();
                }
                else
                {
                    EditorUtility.DisplayDialog("请选择查找范围", "选中目标文件或文件夹点SelectSearchPath按钮", "知道了！");
                }
            }
        }

        private void ActionTwo()
        {
            
            GUILayout.Label ("查找下级引用");
            GUILayout.Space(10);

             Dictionary <string ,List <string >> _redependece= (Dictionary <string ,List <string >>)m_nodeGUI.data.ref_GetField_Inst_Public("Redependence_Down");
            if (_redependece != null && _redependece.Count > 0)
            {
                GUILayout.Space(5);
               
                foreach (KeyValuePair<string, List<string>> pair in _redependece)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                   
                    GUILayout.Label("查找对象");
                    string[] _s = pair.Key.Split('/');
                   if( GUILayout.Button (_s[_s.Length -1] ))
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Key);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    string _self=null ;
                    for (int i=0;i<pair.Value.Count;i++)
                    {
                        UnityEngine.Object _souroce = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Key);
                        UnityEngine.Object _dstance = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Value [i]);

                        if (_souroce.Equals (_dstance ))
                        {
                            _self= pair .Value [i];   
                        }
                      
                    }
                    if(!string .IsNullOrEmpty (_self ) )
                    {
                        pair.Value.Remove((string)_self);
                    }
                    if(GUILayout .Button ("全部选择"))
                    {
                        List<UnityEngine.Object> _obj = new List<UnityEngine.Object>();
                        for (int i = 0; i < pair.Value.Count; i++)
                        {
                           _obj .Add (  AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Value[i]));
                        }
                        if (_obj !=null&&_obj.Count >0 )
                        {
                            Selection.objects = _obj.ToArray ();
                        }
                    }
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        string[] _st=pair .Value [i].Split ('/');
                        if(GUILayout .Button (_st [_st.Length -1]))
                        {
                            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Value[i]);
                        }
                        GUILayout.Space(1);
                    }


                }
                
                GUILayout.Space(5);




            }
            GUILayout.Space(10);
            if (GUILayout.Button("Update"))
            {
                
                    m_nodeGUI.controller.update();
            
            }
        }


        private void ActionThree()
        {
            GUILayout.Label("以选查找引用路径:");
            string[] SearchPath = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("SearchPath");
            if (SearchPath == null || SearchPath.Length < 1)
            {

                if (GUILayout.Button("SelectSearchPath"))
                {
                    List<string> _searchPath = new List<string>();
                    if (Selection.objects != null && Selection.objects.Length > 0)
                    {

                        for (int i = 0; i < Selection.objects.Length; i++)
                        {
                            _searchPath.Add(AssetDatabase.GetAssetPath(Selection.objects[i]));
                        }

                    }
                    else
                    {
                        _searchPath[0] = EditorUtility.OpenFolderPanel("指定搜索文件夹", null, null);
                    }
                    m_nodeGUI.data.ref_SetField_Inst_Public("SearchPath", _searchPath.ToArray());
                }
            }
            else
            {

                for (int i = 0; i < SearchPath.Length; i++)
                {
                    GUILayout.Label(SearchPath[i]);
                }

                if (GUILayout.Button("SelectSearchPath"))
                {
                    List<string> _searchPath = new List<string>();
                    if (Selection.objects != null && Selection.objects.Length > 0)
                    {

                        for (int i = 0; i < Selection.objects.Length; i++)
                        {
                            _searchPath.Add(AssetDatabase.GetAssetPath(Selection.objects[i]));
                        }

                    }
                    else
                    {
                        _searchPath[0] = EditorUtility.OpenFolderPanel("指定搜索文件夹", null, null);
                    }
                    m_nodeGUI.data.ref_SetField_Inst_Public("SearchPath", _searchPath.ToArray());
                }


            }
            GUILayout.Label("查找交叉引用");
            GUILayout.Space(10);

            Dictionary<string, List<string>> _redependece = (Dictionary<string, List<string>>)m_nodeGUI.data.ref_GetField_Inst_Public("Redependence_Cross");
            if (_redependece != null && _redependece.Count > 0)
            {
                GUILayout.Space(5);

                foreach (KeyValuePair<string, List<string>> pair in _redependece)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();

                    GUILayout.Label("查找对象");
                    string[] _s = pair.Key.Split('/');
                    if (GUILayout.Button(_s[_s.Length - 1]))
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Key);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    string _self = null;
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        UnityEngine.Object _souroce = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Key);
                        UnityEngine.Object _dstance = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Value[i]);

                        if (_souroce.Equals(_dstance))
                        {
                            _self = pair.Value[i];
                        }

                    }
                    if (!string.IsNullOrEmpty(_self))
                    {
                        pair.Value.Remove((string)_self);
                    }
                    if (GUILayout.Button("全部选择"))
                    {
                        List<UnityEngine.Object> _obj = new List<UnityEngine.Object>();
                        for (int i = 0; i < pair.Value.Count; i++)
                        {
                            _obj.Add(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Value[i]));
                        }
                        if (_obj != null && _obj.Count > 0)
                        {
                            Selection.objects = _obj.ToArray();
                        }
                    }
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        string[] _st = pair.Value[i].Split('/');
                        if (GUILayout.Button(_st[_st.Length - 1]))
                        {
                            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Value[i]);
                        }
                        GUILayout.Space(1);
                    }


                }

                GUILayout.Space(5);


            }
            GUILayout.Space(10);
            if (GUILayout.Button("Update"))
            {
                if (m_nodeGUI.data.ref_GetField_Inst_Public("SearchPath") != null)
                {
                    m_nodeGUI.controller.update();
                }
                else
                {
                    EditorUtility.DisplayDialog("请选择查找范围", "选中目标文件或文件夹点SelectSearchPath按钮", "知道了！");
                }
            }
        }

        private void ActionFour()
        {
            GUILayout.Label("以选查找无引用路径:");
            string[] SearchPath = (string[])m_nodeGUI.data.ref_GetField_Inst_Public("SearchPath");
            if (SearchPath == null || SearchPath.Length < 1)
            {

                if (GUILayout.Button("SelectSearchPath"))
                {
                    List<string> _searchPath = new List<string>();
                    if (Selection.objects != null && Selection.objects.Length > 0)
                    {

                        for (int i = 0; i < Selection.objects.Length; i++)
                        {
                            _searchPath.Add(AssetDatabase.GetAssetPath(Selection.objects[i]));
                        }

                    }
                    else
                    {
                        _searchPath[0] = EditorUtility.OpenFolderPanel("指定搜索文件夹", null, null);
                    }
                    m_nodeGUI.data.ref_SetField_Inst_Public("SearchPath", _searchPath.ToArray());
                }
            }
            else
            {

                for (int i = 0; i < SearchPath.Length; i++)
                {
                    GUILayout.Label(SearchPath[i]);
                }

                if (GUILayout.Button("SelectSearchPath"))
                {
                    List<string> _searchPath = new List<string>();
                    if (Selection.objects != null && Selection.objects.Length > 0)
                    {

                        for (int i = 0; i < Selection.objects.Length; i++)
                        {
                            _searchPath.Add(AssetDatabase.GetAssetPath(Selection.objects[i]));
                        }

                    }
                    else
                    {
                        _searchPath[0] = EditorUtility.OpenFolderPanel("指定搜索文件夹", null, null);
                    }
                    m_nodeGUI.data.ref_SetField_Inst_Public("SearchPath", _searchPath.ToArray());
                }


            }
            GUILayout.Label("查找无引用资源");
            GUILayout.Space(10);

            Dictionary<string, List<string>> _redependece_None = (Dictionary<string, List<string>>)m_nodeGUI.data.ref_GetField_Inst_Public("Redependence_None");
         
            if (_redependece_None != null && _redependece_None.Count > 0)
            {
                GUILayout.Space(5);

                foreach (KeyValuePair<string, List<string>> pair in _redependece_None)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();

                    GUILayout.Label("查找对象");
                    string[] _s = pair.Key.Split('/');
                    if (GUILayout.Button(_s[_s.Length - 1]))
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Key);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    string _self = null;
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                       
                        
                            UnityEngine.Object _souroce = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Key);
                            UnityEngine.Object _dstance = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Value[i]);

                            if (_souroce.Equals(_dstance))
                            {
                                _self = pair.Value[i];
                            }
                        

                    }
                    if (!string.IsNullOrEmpty(_self))
                    {
                        pair.Value.Remove((string)_self);
                    }
                    if (GUILayout.Button("全部选择"))
                    {
                        List<UnityEngine.Object> _obj = new List<UnityEngine.Object>();
                        for (int i = 0; i < pair.Value.Count; i++)
                        {
                            _obj.Add(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Value[i]));
                        }
                        if (_obj != null && _obj.Count > 0)
                        {
                            Selection.objects = _obj.ToArray();
                        }
                    }
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        if (new EditorAssetInfo(pair.Value[i]).suffix != "meta")
                        {

                            string[] _st = pair.Value[i].Split('/');

                            if (GUILayout.Button(_st[_st.Length - 1]))
                            {
                                Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pair.Value[i]);
                            }
                            GUILayout.Space(1);
                        }
                    }


                }

                GUILayout.Space(5);


            }
            GUILayout.Space(10);
            if (GUILayout.Button("Update"))
            {
                if (m_nodeGUI.data.ref_GetField_Inst_Public("SearchPath") != null)
                {
                    m_nodeGUI.controller.update();
                }
                else
                {
                    EditorUtility.DisplayDialog("请选择查找范围", "选中目标文件或文件夹点SelectSearchPath按钮", "知道了！");
                }
            }
        }



    }

    }


  

