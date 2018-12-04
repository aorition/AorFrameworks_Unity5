using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Framework;
using System.Collections;

public class ConsoleLogHandler : ILogHandler
{

    public int MaxLogsLimit = 120;
    private readonly List<string> _logs = new List<string>();
    
    private ConsoleView _view;
    private ILogHandler _backup;
    public ConsoleLogHandler(ConsoleView view, ILogHandler baseHandler) {
        _view = view;
        _backup = baseHandler;
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        _addLogMess("*** Exception > " + exception.Message + " \n" + exception.ToString());
        _backup.LogException(exception, context);
    }

    private string _hr;
    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {

        switch (logType)
        {
            case LogType.Assert:
                _hr = "<color=\"glay\">A > {0}</color>";
                break;
            case LogType.Log:
                _hr = "<color=\"white\">L > {0}</color>";
                break;
            case LogType.Warning:
                _hr = "<color=\"yellow\">W > {0}</color>";
                break;
            case LogType.Error:
                _hr = "<color=\"red\">E > {0}</color>"; 
                break;
            case LogType.Exception:
                _hr = "<color=\"red\">** E > {0}</color>";
                break;
        }

        if (Debug.logger.IsLogTypeAllowed(logType))
        {
            _addLogMess(string.Format(_hr, string.Format(format, args)));
        }
        _backup.LogFormat(logType, context, format, args);
    }

    private void _addLogMess(string mess) {
        _logs.Add(mess);

        if(_logs.Count > MaxLogsLimit)
        {
            int r = _logs.Count - MaxLogsLimit;
            _logs.RemoveRange(0, r);
        }
        _view.text = _exStr();
    }

    private StringBuilder _strBuilder = new StringBuilder();
    private string _exStr() {
        _strBuilder.Length = 0;
        for (int i = 0; i < _logs.Count; i++)
        {
            _strBuilder.Append(_logs[i] + "\n");
        }
        return _strBuilder.ToString();
    }

    public ILogHandler Dispose() {
        if (_view) _view = null;
        ILogHandler r = _backup;
        _backup = null;
        return r;
    }

}

public class ConsoleView : MonoBehaviour
{
    private static ConsoleView _instance;
    public static ConsoleView Instance
    {
        get { return _instance; }
    }

    public static bool HasInstance
    {
        get { return _instance != null; }
    }

    public static void CreateInstance(Transform parent, int fontSize = 32)
    {
        if (!_instance)
        {
            ResourcesLoadBridge.LoadPrefab("UIPrefabs/ConsolViewUI/ConsolViewUI", (prefab, objs)=> {
                if (prefab)
                {
                    prefab.name = "ConsoleView";
                    prefab.transform.SetParent(parent, false);
                    _instance = prefab.AddComponent<ConsoleView>();
                }
            });

        }
    }
    //------------------------------------------------------------------------------

    private Text _trace;
    private Button _hiddenBtn;
    private Button _showBtn;
    private ScrollRect _sRect;
    private Transform _hideNode;

    private void Awake()
    {

        _hideNode = transform.Find("Content");
        _sRect = _hideNode.Find("ScrollView").GetComponent<ScrollRect>();
        _showBtn = transform.Find("ShowBtn").GetComponent<Button>();
        _trace = _hideNode.Find("ScrollView/Viewport/Content").GetComponent<Text>();
        _trace.text = "";
        _hiddenBtn = _hideNode.Find("HiddenBtn").GetComponent<Button>();
    }

    private ConsoleLogHandler _handler;
    public ConsoleLogHandler Handler {
        get { return _handler; }
    }

    public bool AutoLockScroll = true;

    [SerializeField]
    private bool m_hidden = false;
    public bool Hidden {
        get { return m_hidden; }
        set {
            m_hidden = value;
            if(_hideNode) _hideNode.gameObject.SetActive(!m_hidden);
            if (_showBtn) _showBtn.gameObject.SetActive(m_hidden);
        }
    }

    private int _holdNum = 0;
    IEnumerator holdOnForShow() {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);
            if (_holdNum >= 2)
            {
                Hidden = false;
            }
            _holdNum = 0;
        }
    }

    private void OnEnable()
    {
        _handler = new ConsoleLogHandler(this, Debug.logger.logHandler);
        Debug.logger.logHandler = _handler;
        
        _showBtn.onClick.AddListener(() => {
            //Hidden = false;
            _holdNum++;
        });
        _hiddenBtn.onClick.AddListener(() => {
            Hidden = true;
        });
        Hidden = m_hidden;
        //
        StartCoroutine(holdOnForShow());
    }

    private void OnDisable()
    {
        _trace.text = string.Empty;
        Debug.logger.logHandler = _handler.Dispose();
        _handler = null;

        _showBtn.onClick.RemoveAllListeners();
        _hiddenBtn.onClick.RemoveAllListeners();

    }

    public string text
    {
        get {
            return _trace.text;
        }
        set {
            _trace.text = value;
            transform.SetAsLastSibling();
            if (AutoLockScroll) {
                _sRect.verticalNormalizedPosition = 0;
            }
        }
    }



}