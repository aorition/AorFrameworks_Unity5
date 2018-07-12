using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


namespace Framework.UI.Utility
{

    /// <summary>
    /// 
    /// Author : aorition@qq.com
    /// 
    /// 运行时文件浏览器,目前只兼容到window平台
    /// 
    /// 注意,非编辑器使用时不可访问(亦或是遍历)一些指定受保护的文件路径: (包括但不限于)
    ///     
    ///     1. Application.dataPath 指向的文件路径      :: (不能遍历)
    ///     2. C:\\Users\\Administrator                 :: (受保护,不能访问)
    ///     3. C:\\                                     :: (受保护,不能访问)
    /// 
    /// </summary>
    public class RTFBrowser : MonoBehaviour
    {

        private const string _UIPrefabPath = "RuntimeBaseRes/RTFBrowser/UIPrefab/RTFBrowser";

        private const string _UISubPath_RTBNGButton = "RuntimeBaseRes/RTFBrowser/UIPrefab/RTBNGButton";
        private const string _UISubPath_RTBNGText = "RuntimeBaseRes/RTFBrowser/UIPrefab/RTBNGText";
        private const string _UISubPath_RTFBInfoItem = "RuntimeBaseRes/RTFBrowser/UIPrefab/RTFBInfoItem";

        public static void CreateBrowser(RectTransform parent, Action<RTFBrowser> finishCallback)
        {
            ResourcesLoadBridge.LoadPrefab(_UIPrefabPath, (g, p) =>
            {
                g.transform.SetParent(parent, false);
                RTFBrowser view = g.AddComponent<RTFBrowser>();
                finishCallback(view);
            });
        }

        public static void CreateBrowser(RectTransform parent, string entryPath, Action<RTFBrowser> finishCallback)
        {
            ResourcesLoadBridge.LoadPrefab(_UIPrefabPath, (g, p) =>
            {
                g.transform.SetParent(parent, false);
                RTFBrowser view = g.AddComponent<RTFBrowser>();
                view.entryPath = entryPath;
                finishCallback(view);
            });
        }

        /// ////

        private bool _isDirty = true;

        private string _title = "文件浏览:";

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    _isDirty = true;
                }
            }
        }

        /// <summary>
        /// 验证 onClickOKButton 事件输出的 path 是存在的
        /// </summary>
        public bool VerificationExist = false;

        /// <summary>
        /// 初始化是载入的路径,可以为空
        /// </summary>
        public string entryPath;

        private RectTransform _window;

        public RectTransform windowRT
        {
            get { return _window; }
        }

        private Text _label;
        private RectTransform _NGRoot;
        private RectTransform _NGContent; //导航条空间
        private RectTransform _innerSContent;
        private Button _okBtn;
        private Button _cancelBtn;
        private InputField _pathField;

        private Button _nBtn;
        private Button _pBtn;

        private bool _autoIgnoreMetaFile = true;
        private List<string> _filterList;
        private bool _isWhiteList = false;
        private bool _ignoreCase = true;
        private bool _endWithMatch = true;
        private bool _folderMode = false;

        private void Awake()
        {
            _window = transform.FindRectTransform("window");
            _label = _window.FindRectTransform("labelText").GetComponent<Text>();
            _NGRoot = _window.FindRectTransform("NGContent");
            _NGContent = _NGRoot.FindRectTransform("ScrollView/Viewport/Content");
            _innerSContent = _window.FindRectTransform("innerContent/ScrollView/Viewport/Content");
            _pathField = _window.FindRectTransform("pathField").GetComponent<InputField>();

            _pathField.onEndEdit.AddListener(_onInputEnd);

            _okBtn = _window.FindRectTransform("OKBtn").GetComponent<Button>();
            _okBtn.onClick.AddListener(_onClickOKButton);
            _cancelBtn = _window.FindRectTransform("CancelBtn").GetComponent<Button>();
            _cancelBtn.onClick.AddListener(Dispose);

            _nBtn = _window.FindRectTransform("NTArea/>Btn").GetComponent<Button>();
            _nBtn.onClick.AddListener(_HT_next);
            _nBtn.gameObject.SetActive(false);

            _pBtn = _window.FindRectTransform("NTArea/<Btn").GetComponent<Button>();
            _pBtn.onClick.AddListener(_HT_previous);
            _pBtn.gameObject.SetActive(false);
        }

        /// <summary>
        /// 启用路径模式
        /// </summary>
        public void SetFolderMode(bool folderMode)
        {
            _folderMode = folderMode;
            _isDirty = true;
        }

        /// <summary>
        /// 选项设置
        /// </summary>
        /// <param name="autoIgnoreMetaFile">自动忽略.meta文件</param>
        public void SetOptions(bool autoIgnoreMetaFile = true)
        {
            _autoIgnoreMetaFile = autoIgnoreMetaFile;
            _isDirty = true;
        }

        /// <summary>
        /// 选项设置
        /// </summary>
        /// <param name="filterList">过滤列表</param>
        /// <param name="isWhiteList">白名单过滤方式</param>
        /// <param name="ignoreCase">匹配忽略大小写</param>
        /// <param name="endWithMatch">尾端匹配模式</param>
        public void SetOptions(List<string> filterList, bool isWhiteList = false, bool ignoreCase = true,
            bool endWithMatch = true)
        {
            _filterList = filterList;
            _isWhiteList = isWhiteList;
            _ignoreCase = ignoreCase;
            _endWithMatch = endWithMatch;
            _isDirty = true;
        }

        private void OnDestroy()
        {
            _window = null;
            _label = null;
            _NGContent = null;
            _innerSContent = null;

            _pathField = null;
            _okBtn = null;
            _cancelBtn = null;

            _nBtn = null;
            _pBtn = null;
        }

        public void Dispose()
        {
            ResourcesLoadBridge.UnLoadPrefab(gameObject);
        }

        private void _onInputEnd(string s)
        {
            if (string.IsNullOrEmpty(s)) return;

            //特殊处理单独的window盘符
            if ((Application.platform == RuntimePlatform.WindowsEditor ||
                 Application.platform == RuntimePlatform.WindowsPlayer)
                && s.Length == 2 && s.EndsWith(":")
                )
            {
                _HT_add(getDir());
                _fdList = new List<string>() {s};
                _isDirty = true;
                return;
            }


            if (!s.Contains("\\") && !s.Contains("/")) return;

            //这里验证输入的路径的有效性
            s = s.Replace('\\', '/');
            string[] plist = s.Split('/');
            int i, len = plist.Length;
            string sp = string.Empty;
            for (i = 0; i < len; i++)
            {
                if (i > 0)
                {
                    sp += "/" + plist[i];

                    try
                    {
                        //发现路径不存在,则清空
                        if (!Directory.Exists(sp))
                        {
                            _pathField.text = "";
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        //发生了超出沙箱的可能
                        return;
                    }
                }
                else
                {
                    //现在只处理是window平台
                    if (Application.platform == RuntimePlatform.WindowsPlayer ||
                        Application.platform == RuntimePlatform.WindowsEditor)
                    {
                        sp = plist[i];
                    }
                    else
                    {
                        //尝试写个linux兼容..
                        sp = "/";
                    }
                }
            }

            _HT_add(getDir());

            _fdList = new List<string>();
            for (i = 0; i < len; i++)
            {
                string c = plist[i];
                if (!string.IsNullOrEmpty(c))
                {
                    _fdList.Add(c);
                }
            }

            _pathField.text = "";
            _isDirty = true;
        }

        private StringBuilder _strBuff;

        private void _initStrBuff()
        {
            if (_strBuff == null)
            {
                _strBuff = new StringBuilder();
            }
            else
            {
                _strBuff.Length = 0;
            }
        }

        private string getDir()
        {
            _initStrBuff();
            for (int i = 0; i < _fdList.Count; i++)
            {
                if (i > 0)
                {
                    _strBuff.Append('/');
                }
                _strBuff.Append(_fdList[i]);
            }
            return _strBuff.ToString();
        }

        private void Start()
        {

            //初始化路径判断
            string dfPath;
            if (string.IsNullOrEmpty(entryPath))
            {
                dfPath = Application.isEditor ? Application.dataPath : Application.persistentDataPath;
            }
            else
            {
                dfPath = entryPath.Replace("\\", "/");
            }

            //数据初始化
            _fdList = new List<string>(dfPath.Split('/'));
            _htList.Add(getDir());
        }

        private void Update()
        {
            if (_isDirty)
            {
                clearUI();
                drawUI();
                _isDirty = false;
            }
        }

        private void clearUI()
        {
            //clear NGarea
            _NGContent.DisposeChildren();

            _innerSContent.DisposeChildren();
        }

        private List<string> _fdList = new List<string>();
        //[SerializeField] //debug
        private List<string> _htList = new List<string>();

        //核心绘制
        private void drawUI()
        {

            if (_folderMode)
            {
                _label.text = _title + "(文件夹模式)";
                if (_pathField.gameObject.activeSelf) _pathField.gameObject.SetActive(false);

                _NGRoot.anchorMin = new Vector2(0, 0);
                _NGRoot.anchorMax = new Vector2(1, 0);
                _NGRoot.anchoredPosition = new Vector2(0, 100);
                _NGRoot.sizeDelta = new Vector2(-40, 30);

            }
            else
            {
                _label.text = _title;
                if (!_pathField.gameObject.activeSelf) _pathField.gameObject.SetActive(true);

                _NGRoot.anchorMin = new Vector2(0.2f, 1);
                _NGRoot.anchorMax = new Vector2(0.9f, 1);
                _NGRoot.anchoredPosition = new Vector2(0, -10);
                _NGRoot.sizeDelta = new Vector2(-10, 30);

            }

            //绘制导航数据
            drawNGbtns();

            drawInnerContent();

        }

        private struct ARTFBFileInfo
        {
            public ARTFBFileInfo(string name, long bytes, bool isDirectory)
            {
                this.name = name;
                this.bytes = bytes;
                this.isDirectory = isDirectory;
            }

            public bool isDirectory;
            public string name;
            public long bytes;

        }

        /// <summary>  
        /// 转换方法  
        /// </summary>  
        /// <param name="size">字节值</param>  
        /// <returns></returns>  
        private String HumanReadableFilesize(double size)
        {
            String[] units = new String[] {"B", "KB", "MB", "GB", "TB", "PB"};
            double mod = 1024.0;
            int i = 0;
            while (size >= mod)
            {
                size /= mod;
                i++;
            }
            return Math.Round(size) + units[i];
        }

        private void drawInnerContent()
        {

            //生成数据
            string p = getDir();
            string[] dirs;
            string[] files = null;
            if (_fdList.Count == 1)
            {
                try
                {
                    dirs = Directory.GetDirectories(p + "\\");
                    if (!_folderMode) files = Directory.GetFiles(p + "\\");
                }
                catch (Exception ex)
                {
                    //超出沙箱了
                    _HT_previous();
                    return;
                }
            }
            else
            {
                try
                {
                    dirs = Directory.GetDirectories(p);
                    if (!_folderMode) files = Directory.GetFiles(p);
                }
                catch (Exception ex)
                {
                    //超出沙箱了
                    _HT_previous();
                    return;
                }
            }

            List<ARTFBFileInfo> infoList = new List<ARTFBFileInfo>();
            Action<string, string> iAddinfoListFunc = (f0, n0) =>
            {
                long bytes = 0;
                try
                {
                    //Todo 这里可以使用一个更好的获取文件大小的方法
                    byte[] fbytes = File.ReadAllBytes(f0);
                    bytes = fbytes.LongLength;
                }
                catch (Exception ex)
                {
                    //说明当前文件被其他程序引用,无法读取
                    bytes = 0;
                }
                infoList.Add(new ARTFBFileInfo(n0, bytes, false));
            };

            if (_fdList.Count > 1)
            {
                infoList.Add(new ARTFBFileInfo("[..]", 0, true));
            }
            int i, len = dirs.Length;
            for (i = 0; i < len; i++)
            {
                dirs[i] = dirs[i].Replace('\\', '/');
                string n = "[" + dirs[i].Replace(p + "/", "") + "]";

                infoList.Add(new ARTFBFileInfo(n, 0, true));
            }

            if (!_folderMode)
            {
                len = files.Length;
                for (i = 0; i < len; i++)
                {

                    string f1 = files[i].Replace('\\', '/');

                    if (_autoIgnoreMetaFile && f1.ToLower().EndsWith(".meta")) continue;

                    string n1 = f1.Replace(p + "/", "");

                    if (_filterList != null && _filterList.Count > 0)
                    {

                        bool pass = _ignoreCase
                            ? _filterList.Exists(
                                s =>
                                    (_endWithMatch
                                        ? n1.ToLower().EndsWith(s.ToLower())
                                        : n1.ToLower().Contains(s.ToLower())))
                            : _filterList.Exists(s => (_endWithMatch ? n1.EndsWith(s) : n1.Contains(s)));
                        if (_isWhiteList ? pass : !pass)
                        {
                            iAddinfoListFunc(f1, n1);
                        }
                    }
                    else
                    {
                        iAddinfoListFunc(f1, n1);
                    }

                }
            }

            //Todo 动作排序方式扩展
            //按名字升序排序
            infoList.Sort((a, b) =>
            {
                return a.name.CompareTo(b.name);
            });

            //开始绘制
            len = infoList.Count;
            for (i = 0; i < len; i++)
            {
                ARTFBFileInfo cinfo = infoList[i];
                ResourcesLoadBridge.LoadPrefab(_UISubPath_RTFBInfoItem, (g, param) =>
                {
                    g.transform.SetParent(_innerSContent, false);
                    //
                    Text n = g.transform.Find("Name").GetComponent<Text>();
                    n.text = cinfo.name;

                    Text s = g.transform.Find("Size").GetComponent<Text>();
                    double d = (double) cinfo.bytes;
                    s.text = cinfo.isDirectory ? "-" : HumanReadableFilesize(d);

                    Button b = g.GetComponent<Button>();
                    b.name = cinfo.name;
                    if (cinfo.isDirectory)
                    {
                        b.onClick.AddListener(() =>
                        {
                            _pathField.text = "";
                            _jumpSubFd(b.name);
                        });
                    }
                    else
                    {
                        b.onClick.AddListener(() =>
                        {
                            _pathField.text = b.name;
                        });
                    }
                });

            }



        }

        private Transform _NGBr;

        private void drawNGbtns()
        {

            ResourcesLoadBridge.LoadPrefab(_UISubPath_RTBNGText, (g, param) =>
            {
                g.transform.SetParent(_NGContent, false);
                g.name = "NGBr";
                //
                Text t = g.GetComponent<Text>();
                t.text = "     ";

                _NGBr = g.transform;

                _NGBr.SetAsLastSibling();
            });

            for (int i = 0; i < _fdList.Count; i++)
            {
                if (i > 0)
                {
                    createRTBNGText("/");
                }

                if (i == (_fdList.Count - 1))
                {
                    createRTBNGText(_fdList[i]);
                }
                else
                {
                    createNGBtn(_fdList[i]);
                }
            }


            //Todo 这里可以增加从后对齐功能
            //

        }

        private void createRTBNGText(string label)
        {
            ResourcesLoadBridge.LoadPrefab(_UISubPath_RTBNGText, (g, param) =>
            {
                g.transform.SetParent(_NGContent, false);
                //
                Text t = g.GetComponent<Text>();
                t.text = label;
            });
        }

        /// <summary>
        /// 创建NG按钮
        /// </summary>
        private void createNGBtn(string label)
        {
            ResourcesLoadBridge.LoadPrefab(_UISubPath_RTBNGButton, (g, param) =>
            {
                g.transform.SetParent(_NGContent, false);
                //

                Text lb = g.transform.Find("Text").GetComponent<Text>();

                RectTransformSizeListener rlListener = g.AddComponent<RectTransformSizeListener>();
                rlListener.Tartget = lb.rectTransform;
                rlListener.onTargetSizeChange += (v2, rt, tar) =>
                {
                    rt.sizeDelta = new Vector2(v2.x + 10, rt.sizeDelta.y);
                };


                lb.text = label;

                Button btn = g.GetComponent<Button>();
                btn.name = label;
                btn.onClick.AddListener(() =>
                {
                    _jumpFd(btn.name);
                });
            });

        }

        private void _HT_previous()
        {
            _htPoint --;
            if (_htPoint < 0) _htPoint = 0;
            _fdList = new List<string>(_htList[_htPoint].Split('/'));
            _setNPbtnVisible();
            _isDirty = true;
        }

        private void _HT_next()
        {
            _htPoint++;
            if (_htPoint >= _htList.Count - 1) _htPoint = _htList.Count - 1;
            _fdList = new List<string>(_htList[_htPoint].Split('/'));
            _setNPbtnVisible();
            _isDirty = true;
        }

        private void _setNPbtnVisible()
        {
            if (_htList.Count < 2)
            {
                _nBtn.gameObject.SetActive(false);
                _pBtn.gameObject.SetActive(false);
                return;
            }
            else
            {
                if (_htPoint == 0)
                {
                    _nBtn.gameObject.SetActive(true);
                    _pBtn.gameObject.SetActive(false);
                }
                else if (_htPoint >= _htList.Count - 1)
                {
                    _nBtn.gameObject.SetActive(false);
                    _pBtn.gameObject.SetActive(true);
                }
                else
                {
                    _nBtn.gameObject.SetActive(true);
                    _pBtn.gameObject.SetActive(true);
                }
            }
        }

        //[SerializeField] //debug
        private int _htPoint = 0;
        private int _htMaxNum = 50;

        private void _HT_add(string path)
        {
            List<string> n = new List<string>();
            for (int i = 0; i <= _htPoint; i++)
            {
                string c = _htList[i];
                n.Add(c);
            }

            _htList = n;
            if (_htList.Count > 50)
            {
                _htList.RemoveAt(0);
                _htList.Add(path);
            }
            else
            {
                _htList.Add(path);
            }
            _htPoint = _htList.Count - 1;
            _setNPbtnVisible();
        }

        private void _jumpSubFd(string node)
        {

            if (node == "[..]")
            {
                _fdList.RemoveAt(_fdList.Count - 1);
                _isDirty = true;
                return;
            }

            string n = node.Substring(1, node.Length - 2);
            _fdList.Add(n);

            _HT_add(getDir());

            _isDirty = true;
        }

        private void _jumpFd(string node)
        {

            List<string> n = new List<string>();
            for (int i = 0; i < _fdList.Count; i++)
            {
                string c = _fdList[i];
                n.Add(c);
                if (c == node) break;
            }

            _fdList = n;

            _HT_add(getDir());

            _isDirty = true;
        }

        /// <summary>
        /// 选定事件 : 传回 路径,文件名
        /// </summary>
        public Action<string, string> onClickOKButton;

        private void _onClickOKButton()
        {
            string p = getDir();
            string n = _pathField.text;
            if (VerificationExist)
            {
                try
                {
                    if (File.Exists(p + "/" + n))
                    {
                        if (onClickOKButton != null) onClickOKButton(p, n);
                    }
                }
                catch (Exception ex)
                {
                    //
                }
            }
            else
            {
                if (onClickOKButton != null) onClickOKButton(p, n);
            }
            Dispose();
        }

    }
}