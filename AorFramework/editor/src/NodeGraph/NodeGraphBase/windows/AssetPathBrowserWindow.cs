using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AorFramework.NodeGraph.Tool
{

    public class AssetPathBrowserWindow : EditorWindow
    {

        private static GUIStyle _assetItemLabelStyle;

        public static GUIStyle GetAssetItemLabelStyle()
        {
            if (_assetItemLabelStyle == null)
            {
                _assetItemLabelStyle = GUI.skin.GetStyle("Label").Clone();
                _assetItemLabelStyle.wordWrap = true;
            }
            return _assetItemLabelStyle;
        }

        public static AssetPathBrowserWindow init(string[] AssetsPathList)
        {
            AssetPathBrowserWindow w = EditorWindow.CreateInstance<AssetPathBrowserWindow>();
            w.titleContent = new GUIContent("AssetPathBrowser");
            w.setup(new List<string>(AssetsPathList));
            w.Show();
            return w;
        }

        public static AssetPathBrowserWindow init(List<string> AssetsPathList)
        {
            AssetPathBrowserWindow w = EditorWindow.CreateInstance<AssetPathBrowserWindow>();
            w.titleContent = new GUIContent("AssetPathBrowser");
            w.setup(AssetsPathList);
            return w;
        }

        public List<string> _AssetsPath;

        public void setup(List<string> AssetsPathList)
        {
            _AssetsPath = AssetsPathList;
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

            if (_AssetsPath == null)
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

            AorGUILayout.Vertical("box", () =>
            {

                GUILayout.BeginHorizontal("box");
                
                GUILayout.Label("输入" + _AssetsPath.Count + "个资源 ：共 " + _pageNum + "页（每页最多" + _MaxPrePage + "条)" ,GetAssetItemLabelStyle());

                if (GUILayout.Button(new GUIContent("SelectAll"), GUILayout.Width(100)))
                {
                    //Todo 
                    List<UnityEngine.Object> pathList = new List<UnityEngine.Object>();
                    for (int i = 0; i < _AssetsPath.Count; i++)
                    {
                        UnityEngine.Object pathOb = AssetDatabase.LoadMainAssetAtPath(_AssetsPath[i]);
                        if (pathOb != null)
                        {
                            pathList.Add(pathOb);
                        }
                    }
                    Selection.objects = pathList.ToArray();

                }

                GUILayout.EndHorizontal();

                draw_main();
            });

            Repaint();
        }

        private Vector2 _listScrollPos;

        private bool _infoListForPageDirty = true;
        private void UpdateInfoListForPage()
        {
            if (_infoListForPage == null || _infoListForPageDirty)
            {
                if (_infoListForPage == null)
                {
                    _infoListForPage = new List<string>();
                }
                else
                {
                    _infoListForPage.Clear();
                }

                int start = _MaxPrePage * _pageId;
                int i, len = start + _MaxPrePage;
                for (i = start; i < len; i++)
                {
                    if (i >= _AssetsPath.Count)
                    {
                        break;
                    }

                    _infoListForPage.Add(_AssetsPath[i]);
                }

                _listScrollPos = Vector2.zero;
                _selectIndex = -1;
                _infoListForPageDirty = false;
            }
        }

        private int _pageId = 0;
        private int _MaxPrePage = 200;
        private List<string> _infoListForPage;

        private int _pageNum;
        private int _selectIndex = -1;

        private void draw_main()
        {

            _pageNum = Mathf.CeilToInt((float)_AssetsPath.Count / _MaxPrePage);
            _pageId = Mathf.Clamp(_pageId, 0, _pageNum);
            UpdateInfoListForPage();

            _listScrollPos = AorGUILayout.ScrollView(_listScrollPos, (v2) =>
            {

                int i, len = _infoListForPage.Count;

                if (len > 0)
                {
                    for (i = 0; i < len; i++)
                    {

                        int idx = i;
                        string path = _infoListForPage[idx];
                        if (idx == _selectIndex)
                        {
                            draw_main_item_select(path, idx);
                        }
                        else
                        {
                            draw_main_item_normal(path, idx);
                        }

                    }
                }
                else
                {
                    GUILayout.Label(new GUIContent("没有资源..."));
                }
            });

            //draw 翻页
            if (_pageNum > 1)
            {
                AorGUILayout.Horizontal("box", () =>
                {

                    if (_pageId > 0)
                    {
                        if (GUILayout.Button(new GUIContent("<-"), GUILayout.Width(30)))
                        {
                            _pageId--;
                            _infoListForPageDirty = true;
                            Repaint();
                        }

                    }
                    else
                    {
                        if (GUILayout.Button(new GUIContent("|"), GUILayout.Width(30)))
                        {
                            //do nothing
                        }
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("转到");
                    int nid = EditorGUILayout.IntSlider(_pageId + 1, 1, _pageNum);
                    if ((nid - 1) != _pageId)
                    {
                        _pageId = (nid - 1);
                        _infoListForPageDirty = true;
                        Repaint();
                    }
                    GUILayout.Label("页");
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Page :" + (_pageId + 1) + " / " + _pageNum);
                    if ((_pageId + 1) < _pageNum)
                    {
                        if (GUILayout.Button(new GUIContent("->"), GUILayout.Width(30)))
                        {
                            _pageId++;
                            _infoListForPageDirty = true;
                            Repaint();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(new GUIContent("|"), GUILayout.Width(30)))
                        {
                            //do nothing
                        }
                    }

                });
            }
        }

        private void draw_main_item_select(string path, int idx)
        {
            GUI.color = Color.yellow;

            AorGUILayout.Horizontal("box", () =>
            {
                AorGUILayout.Vertical(() =>
                {
                    GUILayout.FlexibleSpace();
                    AorGUILayout.Horizontal(() =>
                    {
                        GUILayout.Label(new GUIContent(path), GetAssetItemLabelStyle());
                        GUILayout.FlexibleSpace();
                    });
                    GUILayout.FlexibleSpace();
                });
                GUILayout.FlexibleSpace();
                AorGUILayout.Vertical(() =>
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(new GUIContent(">", "选择此条资源")))
                    {
                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
                        _selectIndex = idx;
                    }
                    GUILayout.FlexibleSpace();
                }, GUILayout.Width(30));
            }, GUILayout.Height(40));

            GUI.color = Color.white;
        }

        private void draw_main_item_normal(string path, int idx)
        {
            AorGUILayout.Horizontal("box", () =>
            {
                AorGUILayout.Vertical(() =>
                {
                    GUILayout.FlexibleSpace();
                    //                GUILayout.Label(new GUIContent(path));
                    if (GUILayout.Button(new GUIContent(path), "label"))
                    {
                        //                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
                        _selectIndex = idx;
                    }
                    GUILayout.FlexibleSpace();
                });
                GUILayout.FlexibleSpace();
                AorGUILayout.Vertical(() =>
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(new GUIContent(">", "选择此条资源")))
                    {
                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
                        _selectIndex = idx;
                    }
                    GUILayout.FlexibleSpace();
                }, GUILayout.Width(30));
            }, GUILayout.Height(40));
        }

    }
}
