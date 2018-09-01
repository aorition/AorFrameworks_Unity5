using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using System.Linq;
using AorBaseUtility;

/// <summary>
/// 高级资源查找器
/// 
/// Author : Aorition
/// 
/// </summary>
public class AssetsFindEditorWindow : EditorWindow
{
    [MenuItem("FrameworkTools/资源管理/高级资源查找器",false)]
    public static AssetsFindEditorWindow init()
    {
        AssetsFindEditorWindow w = EditorWindow.GetWindow<AssetsFindEditorWindow>();
        w.titleContent = new GUIContent("高级资源检索");
        return w;
    }

    /// <summary>
    /// 获得文件夹内的所有资源
    /// </summary>
    private static List<string> GetFolderResources(string resourcePath)
    {

        List<string> allfile = new List<string>();
        //Resource资源路径
        string[] files = Directory.GetFiles(resourcePath, "*.*", SearchOption.AllDirectories);

        int i, len = files.Length;

        for (i = 0; i < len; i++)
        {

            string file = files[i];
            string realFile = file.Replace("\\", "/");
            realFile = realFile.Replace(Application.dataPath, "Assets");


            if (string.IsNullOrEmpty(realFile))
                continue;
            if (!allfile.Contains(realFile))
                allfile.Add(realFile);
        }

        return allfile;
    }

    /// <summary>
    /// 获取选择的资源，包含子文件夹
    /// </summary>
    public static List<string> FindAssetPathsBySelect(ref string searchPath)
    {
        searchPath = "";
        UnityEngine.Object[] objs = Selection.objects;
        if (objs != null && objs.Length > 0)
        {
            int i, len = objs.Length;
            List<string> s = new List<string>();
            for (i = 0; i < len; i++)
            {
                string str = AssetDatabase.GetAssetPath(objs[i]);
                if (!string.IsNullOrEmpty(str))
                {

                    searchPath += str + "\n";

                    if (str.Contains('.'))
                    {
                        s.Add(str);
                    }
                    else
                    {
                        string dicP = Application.dataPath.Replace("Assets", "") + str;
                        //Debug.Log("******** dicP = " + dicP);
                        if (Directory.Exists(dicP))
                        {

                            //Debug.Log("******** true :: " + str);

                            //处理选中的是文件夹::
                            List<string> shs = GetFolderResources(dicP);
                            // Debug.Log("******** shs :: " + (shs == null ? "null" : shs.Count.ToString()));
                            if (shs != null && shs.Count > 0)
                            {
                                string[] shStrings = shs.ToArray();
                                //_targetWindow.AddBuildAssetsPath(shs.ToArray());
                                for (var u = 0; u < shStrings.Length; u++)
                                {

                                    string spath = shStrings[u];

                                    if (!s.Contains(spath))
                                    {
                                        s.Add(spath);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return s;
        }
        return null;
    }
    
    public List<string> _AssetsPath ;
    public List<List<string>> _AssetsSubInfos;

    private string _searchKeyworks;

    private void addPath(List<string> list)
    {
        if (_AssetsPath == null || list == null || list.Count == 0) return;
        int i, len = list.Count;
        for (i = 0; i < len; i++)
        {
            if (!_AssetsPath.Contains(list[i]))
                _AssetsPath.Add(list[i]);
        }
    }

    void Awake()
    {
        _AssetsPath = new List<string>();
        _AssetsSubInfos = new List<List<string>>();
    }

    private void OnGUI()
    {

        if (_AssetsPath == null || _AssetsSubInfos == null)
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

        GUILayout.Space(5);
        draw_search_UI();
        GUILayout.Space(15);

        AorGUILayout.Vertical("box", () =>
        {
            draw_AssetInfoUI();
            GUILayout.Space(10);
            draw_main();

        });

        draw_io_UI();

        Repaint();
    }

    private void draw_AssetInfoUI()
    {
        AorGUILayout.Horizontal("box", () =>
        {
            AorGUILayout.Vertical(() =>
            {
                GUILayout.FlexibleSpace();
                //_searchPath
                AorGUILayout.Horizontal(() =>
                {
                    GUILayout.Space(20);
                    GUILayout.Label("搜索路径 : ");
                    GUILayout.Label(_searchPath);
                    GUILayout.FlexibleSpace();
                });
                AorGUILayout.Horizontal(() =>
                {
                    GUILayout.Space(20);
                    GUILayout.Label("结果数量 : ");
                    GUILayout.Label(_AssetsPath.Count.ToString());
                    GUILayout.FlexibleSpace();
                });
                GUILayout.FlexibleSpace();
            });
            GUILayout.FlexibleSpace();
            /*
            AorGUILayout.Vertical(() =>
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("清除列表"), GUILayout.Height(22)))
                {
                    if (EditorUtility.DisplayDialog("警告", "你确定要清除当前查找结果", "确定", "取消"))
                    {
                        _AssetsPath.Clear();
                        _infoListForPageDirty = true;
                    }
                }
                GUILayout.FlexibleSpace();
            });*/
        }, GUILayout.Height(50));
    }
    
    private string[] _searchTypeLabels = new[] { "查找文件关键字", "查找组件", "查找无效组件","查找资源引用"};
    private int _searchType;

    private string _searchPath;

    //重置数据
    private void _resetAllData()
    {
        _selectIndex = -1;
        _searchPath = "";
        _search00_input = "";
        _search01_input = "";
        _AssetsPath.Clear();
        _AssetsSubInfos.Clear();
        _infoListForPageDirty = true;
        _search03_autoGenDTree = true;
    }

    private void draw_search_UI()
    {
        int newType = GUILayout.Toolbar(_searchType, _searchTypeLabels);

        if (newType != _searchType)
        {
            //切换Tag
            if (_AssetsPath != null && _AssetsPath.Count > 0)
            {
                if (EditorUtility.DisplayDialog("提示", "切换TAG会清空当前搜索结果，确定需要切换TAG？", "确定", "取消"))
                {
                    //切换重置
                    _resetAllData();
                }
                else
                {
                    newType = _searchType;
                }
            }
        }

        switch (newType)
        {
            case 1:
                draw_search01_UI();

                break;
            case 2:
                draw_search02_UI();
                break;
            case 3:
                draw_search03_UI();
                break;
            default:
                //0
                draw_search00_UI();
                break;
        }
        _searchType = newType;
    }

    private bool _alwaysIgnoreMETAFile = true;

    #region search00 关键字查找

    private bool _ignoreCase = false;
    private string _search00_input;

    private void draw_search00_UI()
    {
        AorGUILayout.Horizontal(() =>
        {
            GUILayout.Label("选项:");
            GUILayout.Space(10);
            AorGUILayout.Horizontal("box", () =>
            {
                _alwaysIgnoreMETAFile = GUILayout.Toggle(_alwaysIgnoreMETAFile, "忽略META文件");
                _ignoreCase = GUILayout.Toggle(_ignoreCase, "忽略大小写");
            });
        });
        AorGUILayout.Horizontal(() =>
        {
            AorGUILayout.Vertical(() =>
            {
                GUILayout.FlexibleSpace();
                AorGUILayout.Horizontal(() =>
                {
                    _search00_input = EditorGUILayout.TextField("", _search00_input, "ToolbarSeachTextField");
                    if (GUILayout.Button("", "ToolbarSeachCancelButton"))
                    {
                        //reset
                        search00_resetInput();
                    }
                });
                GUILayout.FlexibleSpace();
            }, GUILayout.Height(26));
            GUILayout.Space(10);
            string searchBtnLabel;
            if (_AssetsPath != null && _AssetsPath.Count > 0)
            {
                searchBtnLabel = "重新查找";
            }
            else
            {
                searchBtnLabel = "开始查找";
            }

            if (GUILayout.Button(searchBtnLabel, GUILayout.Width(80), GUILayout.Height(26)))
            {
                search00_search();
            }

        });
    }

    private void search00_resetInput()
    {
        _search00_input = "";
        GUI.FocusControl(null);
    }

    private class FilterCom
    {

        public string com;
        public bool isExclude;
        public bool isRegular;
        public Regex comRegex;

        public FilterCom(string srcStr)
        {
            com = srcStr;

            //检查是否是排除
            if (com.IndexOf('!') == 0)
            {
                isExclude = true;
                com = com.Substring(1);
            }

            //检查是否是正则表达式
            if (com.IndexOf('/') == 0 && com.LastIndexOf('/') == com.Length - 1)
            {
                com = com.Substring(1, com.Length - 2);
                comRegex = new Regex(com);
            }
        }
    }

    private List<FilterCom> _search00_filters;

    private void search00_search()
    {
        if (string.IsNullOrEmpty(_search00_input))
        {
            if (!EditorUtility.DisplayDialog("提示", "你似乎没有设置任何过滤条件，是否继续？", "继续", "取消"))
            {
                return;
            }
        }

        //创建 过滤指令集合
        int i, len;
        _search00_filters = new List<FilterCom>();

        if (_alwaysIgnoreMETAFile)
        {
            _search00_filters.Add(new FilterCom("!.meta"));
        }

        if (!string.IsNullOrEmpty(_search00_input))
        {


            //            string allFilterStr = _search00_input.Trim();
            _searchKeyworks = _search00_input.Trim();
            if (_searchKeyworks.IndexOf(' ') != -1)
            {
                //多个 过滤条件
                string[] filterStrs = _searchKeyworks.Split(' ');
                len = filterStrs.Length;
                for (i = 0; i < len; i++)
                {
                    if (!string.IsNullOrEmpty(filterStrs[i]))
                    {
                        _search00_filters.Add(new FilterCom(filterStrs[i]));
                    }
                }

            }
            else
            {
                _search00_filters.Add(new FilterCom(_searchKeyworks));
            }
        }

        // ----- 开始
        //
        //

        //目标搜索
        List<string> preAssets = FindAssetPathsBySelect(ref _searchPath);

        if (preAssets == null || preAssets.Count == 0)
        {
            //目标搜索无效，则搜索全部资源
            preAssets = GetFolderResources(Application.dataPath);
            _searchPath = "Assets/";
        }

        //if(_AssetsPath == null)
        _AssetsPath = new List<string>();
        _AssetsSubInfos = new List<List<string>>();

        len = preAssets.Count;
        for (i = 0; i < len; i++)
        {
            EditorUtility.DisplayProgressBar("Searching ...", "查找资源中 ... ...", (float) i/len);
            //
            bool isAdd = true;
            string path = preAssets[i];

            int f, flen = _search00_filters.Count;
            for (f = 0; f < flen; f++)
            {
                if (!isAdd) break;
                //
                if (_search00_filters[f].isRegular)
                {
                    if (_search00_filters[f].comRegex.IsMatch(path) == _search00_filters[f].isExclude)
                    {
                        isAdd = false;
                    }
                }
                else
                {
                    //如果过滤条件为后缀名，则自动忽略大小写
                    if (_search00_filters[f].com.Contains('.'))
                    {
                        if (path.ToLower().Contains(_search00_filters[f].com.ToLower()) ==
                            _search00_filters[f].isExclude)
                        {
                            isAdd = false;
                        }
                    }
                    else
                    {
                        string key = path.Substring(path.LastIndexOf('/'));
                        if (_ignoreCase)
                        {
                            if (key.ToLower().Contains(_search00_filters[f].com.ToLower()) ==
                                _search00_filters[f].isExclude)
                            {
                                isAdd = false;
                            }
                        }
                        else
                        {
                            if (key.Contains(_search00_filters[f].com) == _search00_filters[f].isExclude)
                            {
                                isAdd = false;
                            }
                        }
                    }
                }
            }

            if (isAdd)
            {
                if (!_AssetsPath.Contains(path))
                {
                    _AssetsPath.Add(path);
                    _subInfoListForPage.Add(null);
                }
            }

        }

        EditorUtility.ClearProgressBar();
        _infoListForPageDirty = true;
    }


    #endregion

    #region search01 查找组件

    private string _search01_input;
    private List<FilterCom> _search01_filters;
    private void draw_search01_UI()
    {
        AorGUILayout.Horizontal(() =>
        {
            AorGUILayout.Vertical(() =>
            {
                GUILayout.FlexibleSpace();
                AorGUILayout.Horizontal(() =>
                {
                    _search01_input = EditorGUILayout.TextField("", _search01_input, "ToolbarSeachTextField");
                    if (GUILayout.Button("", "ToolbarSeachCancelButton"))
                    {
                        //reset
                        search01_resetInput();
                    }
                });
                GUILayout.FlexibleSpace();
            }, GUILayout.Height(26));
            GUILayout.Space(10);
            string searchBtnLabel;
            if (_AssetsPath != null && _AssetsPath.Count > 0)
            {
                searchBtnLabel = "重新查找";
            }
            else
            {
                searchBtnLabel = "开始查找";
            }

            if (GUILayout.Button(searchBtnLabel, GUILayout.Width(80), GUILayout.Height(26)))
            {
                search01_search();
            }

        });
    }

    private void search01_resetInput()
    {
        _search01_input = "";
        GUI.FocusControl(null);
    }

    private void search01_search()
    {
        if (string.IsNullOrEmpty(_search01_input))
        {
            if (!EditorUtility.DisplayDialog("提示", "你似乎没有设置任何过滤条件，是否继续？", "继续", "取消"))
            {
                return;
            }
        }

        //创建 过滤指令集合
        int i, len;
        _search01_filters = new List<FilterCom>();

        if (!string.IsNullOrEmpty(_search01_input))
        {
            //            string allFilterStr = _search01_input.Trim();
            _searchKeyworks = _search01_input.Trim();
            if (_searchKeyworks.IndexOf(' ') != -1)
            {
                //多个 过滤条件
                string[] filterStrs = _searchKeyworks.Split(' ');
                len = filterStrs.Length;
                for (i = 0; i < len; i++)
                {
                    if (!string.IsNullOrEmpty(filterStrs[i]))
                    {
                        _search01_filters.Add(new FilterCom(filterStrs[i]));
                    }
                }

            }
            else
            {
                _search01_filters.Add(new FilterCom(_searchKeyworks));
            }
        }

        // ----- 开始
        //
        //

        //目标搜索
        List<string> preAssets = FindAssetPathsBySelect(ref _searchPath);

        if (preAssets == null || preAssets.Count == 0)
        {
            //目标搜索无效，则搜索全部资源
            preAssets = GetFolderResources(Application.dataPath);
            _searchPath = "Assets/";
        }

        //if(_AssetsPath == null)
        _AssetsPath = new List<string>();
        _AssetsSubInfos = new List<List<string>>();

        len = preAssets.Count;
        for (i = 0; i < len; i++)
        {
            EditorUtility.DisplayProgressBar("Searching ...", "查找资源中 ... ...", (float)i / len);
            //
            bool isAdd = false;
            string path = preAssets[i];

            //只处理 预制体
            if (path.ToLower().Contains(".prefab"))
            {
                GameObject assetGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (assetGameObject)
                {
                    int f, flen = _search01_filters.Count;
                    for (f = 0; f < flen; f++)
                    {
                        
                        //不支持 正则表达式
                        if (!_search01_filters[f].isRegular)
                        {
                            isAdd = Search01Check(assetGameObject, _search01_filters[f].com);
                            if (_search01_filters[f].isExclude)
                                isAdd = !isAdd;
                        }

                        if (!isAdd) break;

                    }
                }
            }

            if (isAdd)
            {
                if (!_AssetsPath.Contains(path))
                {
                    _AssetsPath.Add(path);
                    _AssetsSubInfos.Add(null);
                }
            }

        }

        EditorUtility.ClearProgressBar();
        _infoListForPageDirty = true;
    }

    private bool Search01Check(GameObject go, string ckString)
    {
        Component[] components = go.GetComponentsInChildren<Component>();
        if (components == null) return false;
        int i, len = components.Length;
        for (i = 0; i < len; i++)
        {

            Component component = components[i];
            if (component != null)
            {
                Type type = components[i].GetType();
                if (type.Name == ckString)
                {
                    return true;
                }
            }
            else
            {
                //关键字支持 null / missing 表示无效组件
                if (ckString.ToLower() == "null" || ckString.ToLower() == "missing")
                {
                    return true;
                }

            }
        }
        return false;
    }

    #endregion
    
    #region search02 查找无效组件

    private void draw_search02_UI()
    {

        string searchBtnLabel;
        if (_AssetsPath != null && _AssetsPath.Count > 0)
        {
            searchBtnLabel = "重新查找";
        }
        else
        {
            searchBtnLabel = "开始查找";
        }

        if (GUILayout.Button(searchBtnLabel, GUILayout.Height(26)))
        {
            search02_search();
        }
    }

    private void search02_search()
    {

        // ----- 开始
        //
        //

        int i, len;

        //目标搜索
        List<string> preAssets = FindAssetPathsBySelect(ref _searchPath);

        if (preAssets == null || preAssets.Count == 0)
        {
            //目标搜索无效，则搜索全部资源
            preAssets = GetFolderResources(Application.dataPath);
            _searchPath = "Assets/";
        }

        //if(_AssetsPath == null)
        _AssetsPath = new List<string>();
        _AssetsSubInfos = new List<List<string>>();

        len = preAssets.Count;
        for (i = 0; i < len; i++)
        {
            EditorUtility.DisplayProgressBar("Searching ...", "查找资源中 ... ...", (float)i / len);
            //
            bool isAdd = false;
            string path = preAssets[i];

            //只处理 预制体
            if (path.ToLower().Contains(".prefab"))
            {
                GameObject assetGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (assetGameObject)
                {
                    /*
                    Component[] components = assetGameObject.GetComponentsInChildren<Component>();
                    int u, ulen = components.Length;
                    for (u = 0; u < ulen; u++)
                    {
                        if (components[u] == null)
                        {
                            if (!_AssetsPath.Contains(path))
                            {
                                _AssetsPath.Add(path);
                            }
                        }
                    }*/
                    List<string> subPaths = new List<string>();
                    findMissingComponentLoop(assetGameObject.transform,ref subPaths);

                    if (subPaths != null && subPaths.Count > 0)
                    {
                        if (!_AssetsPath.Contains(path))
                        {
                            _AssetsPath.Add(path);
                            _AssetsSubInfos.Add(subPaths);
                        }
                    }

                }
            }

        }

        EditorUtility.ClearProgressBar();
        _infoListForPageDirty = true;
    }

    private void findMissingComponentLoop(Transform t, ref List<string> subPaths)
    {
        if (subPaths == null)
        {
            subPaths = new List<string>();
        }

        Component[] components = t.GetComponents<Component>();
        int i, len = components.Length;
        for (i = 0; i < len; i++)
        {
            if (components[i] == null)
            {
                string subPath = t.getHierarchyPath();
                subPaths.Add(subPath);
            }
        }

        if (t.childCount > 0)
        {
            len = t.childCount;
            for (i = 0; i < len; i++)
            {
                Transform sub = t.GetChild(i);
                findMissingComponentLoop(sub,ref subPaths);
            }
        }
    }

    #endregion

    #region search03 查找资源引用

    private bool _search03_autoGenDTree = true;
    private void draw_search03_UI()
    {
        AorGUILayout.Horizontal(() =>
        {
            GUILayout.Label("选项:");
            GUILayout.Space(10);
            AorGUILayout.Horizontal("box", () =>
            {
                _search03_autoGenDTree = GUILayout.Toggle(_search03_autoGenDTree,
                    new GUIContent("自动创建依赖关系树", "关闭此项后需手动创建/更新，但查询引用会变快"));
            });

        });

        if (!_search03_autoGenDTree)
        {
            AorGUILayout.Horizontal("box", () =>
            {
                if (_dependenciesTree == null)
                {
                    GUI.color = Color.yellow;
                    if (GUILayout.Button("创建依赖关系树", GUILayout.Height(26)))
                    {
                        buildingDependenciesTree();
                    }
                    GUI.color = Color.white;
                }
                else
                {
                    GUI.color = Color.green;
                    if (GUILayout.Button("更新依赖关系树", GUILayout.Height(26)))
                    {
                        buildingDependenciesTree();
                    }
                    GUI.color = Color.white;
                }
            });
        }

        string searchBtnLabel;
        if (_AssetsPath != null && _AssetsPath.Count > 0)
        {
            searchBtnLabel = "重新查找";
        }
        else
        {
            searchBtnLabel = "开始查找";
        }

        if (GUILayout.Button(searchBtnLabel, GUILayout.Height(26)))
        {
            search03_search();
        }
    }

    private Dictionary<string, List<string>> _dependenciesTree;

    private void buildingDependenciesTree()
    {
        int i, len;
        //建立依赖关系树
        if (_dependenciesTree == null || _search03_autoGenDTree)
        {
            _dependenciesTree = new Dictionary<string, List<string>>();
            List<string> allFiles = GetFolderResources(Application.dataPath);

            len = allFiles.Count;
            for (i = 0; i < len; i++)
            {
                EditorUtility.DisplayProgressBar("请稍后", "正在建立依赖关系树 ... ...", (float)i / len);
                string p = allFiles[i].ToLower();
                if (p.Contains(".prefab") || p.Contains(".mat") || p.Contains(".fbx") || p.Contains(".anim") ||
                    p.Contains(".controller"))
                {
                    List<string> dependencies = new List<string>();
                    string[] depsStr = AssetDatabase.GetDependencies(allFiles[i], true);
                    dependencies.AddRange(depsStr);
                    dependencies.Remove(allFiles[i]);

                    if (!_dependenciesTree.ContainsKey(allFiles[i]))
                    {
                        _dependenciesTree.Add(allFiles[i], dependencies);
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }
    }

    private void search03_search()
    {

        //建立依赖关系树
        if (_search03_autoGenDTree || _dependenciesTree == null)
        {
            buildingDependenciesTree();
        }

        // ----- 开始
        //
        //

        //目标搜索
        List<string> preAssets = FindAssetPathsBySelect(ref _searchPath);

        if (preAssets == null || preAssets.Count == 0)
        {
            //目标搜索无效，则搜索全部资源
            preAssets = GetFolderResources(Application.dataPath);
            _searchPath = "Assets/";
        }

        //if(_AssetsPath == null)
        _AssetsPath = new List<string>();
        _AssetsSubInfos = new List<List<string>>();

        int i,len = preAssets.Count;
        for (i = 0; i < len; i++)
        {
            EditorUtility.DisplayProgressBar("Searching ...", "查找资源中 ... ...", (float)i / len);
            //
            string path = preAssets[i];
            foreach (KeyValuePair<string, List<string>> keyValuePair in _dependenciesTree)
            {
                if (keyValuePair.Value.Contains(path))
                {
                    int index;
                    if (_AssetsPath.Contains(path))
                    {
                        index = _AssetsPath.IndexOf(path);
                        _AssetsSubInfos[index].Add(keyValuePair.Key);
                    }
                    else
                    {
                        _AssetsPath.Add(path);
                        _AssetsSubInfos.Add(new List<string>() {keyValuePair.Key});
                    }
                }
            }
        }

        EditorUtility.ClearProgressBar();
        _infoListForPageDirty = true;
    }

    #endregion

    //------------------------------

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

            //_subInfoListForPage
            if (_subInfoListForPage == null)
            {
                _subInfoListForPage = new List<List<string>>();
            }
            else
            {
                _subInfoListForPage.Clear();
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
                if (_AssetsSubInfos != null && _AssetsSubInfos.Count > i)
                {
                    _subInfoListForPage.Add(_AssetsSubInfos[i]);
                }
            }

            _listScrollPos = Vector2.zero;
            _selectIndex = -1;
            _infoListForPageDirty = false;
        }
    }

    private int _pageId = 0;
    private int _MaxPrePage = 200;
    private List<string> _infoListForPage;
    private List<List<string>> _subInfoListForPage;

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
                        if (_subInfoListForPage != null && idx < _subInfoListForPage.Count)
                        {
                            draw_main_item_select(path, idx, _subInfoListForPage[idx]);
                        }
                        else
                        {
                            draw_main_item_select(path, idx, null);
                        }
                    }
                    else
                    {
                        if (_subInfoListForPage != null && idx < _subInfoListForPage.Count)
                        {
                            draw_main_item_normal(path, idx, _subInfoListForPage[idx]);
                        }
                        else
                        {
                            draw_main_item_normal(path, idx, null);
                        }
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

    private void draw_main_item_normal(string path, int idx, List<string> subPaths)
    {
        AorGUILayout.Horizontal("box", () =>
        {
            AorGUILayout.Vertical(() =>
            {
                GUILayout.FlexibleSpace();
                //                GUILayout.Label(new GUIContent(path));
                if (GUILayout.Button(new GUIContent(path),"label"))
                {
//                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
                    _selectIndex = idx;
                }
                GUILayout.FlexibleSpace();
            });
            GUILayout.FlexibleSpace();

            if (_searchType == 3 && subPaths != null)
            {
                GUILayout.Label("被引用数量：" + subPaths.Count);
                GUILayout.Space(10);
            }

            AorGUILayout.Vertical(() =>
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent(">", "选择此条资源")))
                {
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
                    _selectIndex = idx;
                }
                GUILayout.FlexibleSpace();
            },GUILayout.Width(30));
        }, GUILayout.Height(40));
    }

    private void draw_main_item_select(string path, int idx, List<string> subPaths)
    {
        GUI.color = Color.yellow;

        AorGUILayout.Horizontal("box", () =>
        {
            AorGUILayout.Vertical(() =>
            {
                GUILayout.FlexibleSpace();
                AorGUILayout.Horizontal(() =>
                {
                    GUILayout.Label(new GUIContent(path));
                    GUILayout.FlexibleSpace();
                    if (_searchType == 3 && subPaths != null)
                    {
                        GUILayout.Label("被引用数量：" + subPaths.Count);
                        GUILayout.Space(10);
                    }
                });

                if (subPaths != null && subPaths.Count > 0)
                {
                    AorGUILayout.Vertical("box", () =>
                    {

                        int i, len = subPaths.Count;
                        for (i = 0; i < len; i++)
                        {
                            AorGUILayout.Horizontal(() =>
                            {
                                GUILayout.Space(100);
                                GUI.color = Color.cyan;
                                if (_searchType == 3)
                                {
                                    string subp = subPaths[i];
                                    if (GUILayout.Button(new GUIContent(subp)))
                                    {
                                        UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(subp);
                                        if (obj)
                                        {
                                            Selection.activeObject = obj;
                                        }
                                    }
                                }
                                else
                                {
                                    GUILayout.Label(new GUIContent(subPaths[i]));
                                }
                                GUI.color = Color.white;
                            });
                        }

                    });
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

        GUI.color = Color.white;
    }

    private void draw_io_UI()
    {
        AorGUILayout.Horizontal(() =>
        {
            if (_AssetsPath != null && _AssetsPath.Count > 0)
            {
                //导出结果
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("保存搜索结果.."))
                {
                    string savePath = EditorUtility.SaveFilePanel("保存", "AssetFind_result", "", "json");
                    if (!string.IsNullOrEmpty(savePath))
                    {

                        string json = _bulidJsonData();
                        AorIO.SaveStringToFile(savePath, json);

                    }

                }
            }
            else
            {
                //导入Json
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("加载搜索结果"))
                {
                    string openPath = EditorUtility.OpenFilePanel("打开", "", "json");
                    if (!string.IsNullOrEmpty(openPath))
                    {
                        string json = AorIO.ReadStringFormFile(openPath);
                        if (!string.IsNullOrEmpty(json))
                        {
                            _importJsonData(json);
                        }
                    }
                }
            }
        });
    }

    private string _bulidJsonData()
    {
        StringBuilder _s = new StringBuilder();

        string sk;
        string ig = "";
        switch (_searchType)
        {
            case 3:
                sk = "Dependencies";
                break;
            case 2:
                sk = "missing";
                break;
            case 1:
                sk = _searchKeyworks;
                break;
            default:
                sk = _searchKeyworks;
                ig = ",\"IgnoreMETA\":\"" + _alwaysIgnoreMETAFile.ToString() + "\"";
                break;
        }

        _s.Append("{\"name\":\"AssetsFindResult\",\"searchPaths\":\""+ _searchPath + "\",\"searchType\":" + _searchType + ig +",\"searchKey\":\"" + sk + "\",\"data\":[");

        int i, len = _AssetsPath.Count;
        for (i = 0; i < len; i++)
        {
            if (i > 0)
            {
                _s.Append(',');
            }

            _s.Append("{\"path\":\"" + _AssetsPath[i] + "\",\"subInfo\":[");
            if (_AssetsSubInfos[i] != null && _AssetsSubInfos[i].Count > 0)
            {

                int s, slen = _AssetsSubInfos[i].Count;
                for (s = 0; s < slen; s++)
                {

                    if (s > 0)
                    {
                        _s.Append(',');
                    }

                    _s.Append("\"" + _AssetsSubInfos[i][s] + "\"");

                }
            }
            _s.Append("]}");
        }

        _s.Append("]}");

        return _s.ToString();
    }

    private void _importJsonData(string json)
    {
        Dictionary<string, object> jsonDic = MiniJSON.Json.DecodeToDic(json);
        if (jsonDic != null)
        {
            if (jsonDic["name"] == null || (string) jsonDic["name"] != "AssetsFindResult")
            {
                EditorUtility.DisplayDialog("提示", "加载的Json无效.", "确定");
                return;
            }

            _searchType = int.Parse(jsonDic["searchType"].ToString());

            _searchPath = jsonDic["searchPaths"].ToString();

            if (_searchType == 0 || _searchType == 1)
            {
                _searchKeyworks = jsonDic["searchKey"].ToString();
                if (_searchType == 0)
                {
                    _alwaysIgnoreMETAFile = bool.Parse(jsonDic["IgnoreMETA"].ToString());
                    _search00_input = _searchKeyworks;
                }
                else
                {
                    _search01_input = _searchKeyworks;
                }
            }

            IList ilist = (IList) jsonDic["data"];
            if (_AssetsPath == null)
            {
                _AssetsPath = new List<string>();
            }
            else
            {
                _AssetsPath.Clear();
            }

            if (_AssetsSubInfos == null)
            {
                _AssetsSubInfos = new List<List<string>>();
            }
            else
            {
                _AssetsSubInfos.Clear();
            }

            foreach (object o in ilist)
            {
                Dictionary<string, object> info = (Dictionary<string, object>) o;
                string path = info["path"].ToString();
                _AssetsPath.Add(path);

                IList subList = (IList) info["subInfo"];
                if (subList != null && subList.Count > 0)
                {

                    List<string> sList = new List<string>();
                    foreach (object o1 in subList)
                    {
                        sList.Add(o1.ToString());
                    }
                    _AssetsSubInfos.Add(sList);
                }
                else
                {
                    _AssetsSubInfos.Add(null);
                }
            }

            _infoListForPageDirty = true;
        }
        else
        {

            EditorUtility.DisplayDialog("提示", "无法解析加载文件.", "确定");
        }
    }

}
