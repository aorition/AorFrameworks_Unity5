using UnityEngine;
using UnityEditor;
using Framework.Extends;
using System.Collections.Generic;

public class QueueActionDebugWindow : EditorWindow
{

    private static GUIStyle _titleStyle;
    protected static GUIStyle titleStyle
    {
        get
        {
            if (_titleStyle == null)
            {
                _titleStyle = EditorStyles.largeLabel.Clone();
                _titleStyle.fontSize = 16;
            }
            return _titleStyle;
        }
    }

    private static GUIStyle _sTitleStyle;
    protected static GUIStyle sTitleStyle
    {
        get
        {
            if (_sTitleStyle == null)
            {
                _sTitleStyle = EditorStyles.largeLabel.Clone();
                _sTitleStyle.fontSize = 14;
                _sTitleStyle.fontStyle = FontStyle.Bold;
            }
            return _sTitleStyle;
        }
    }

    private static GUIStyle _itemLabelStyle;
    protected static GUIStyle itemLabelStyle
    {
        get
        {
            if (_itemLabelStyle == null)
            {
                _itemLabelStyle = GUI.skin.GetStyle("button").Clone();
                _itemLabelStyle.fontSize = 12;
                _itemLabelStyle.fontStyle = FontStyle.Bold;
                _itemLabelStyle.alignment = TextAnchor.MiddleLeft;
                _itemLabelStyle.padding = new RectOffset(20, 20, 0, 0);
                _itemLabelStyle.normal.textColor = Color.white;
            }
            return _itemLabelStyle;
        }
    }

    //--------

    private static QueueActionDebugWindow _instance;

    [MenuItem("GameDebugTools/QueueAction/Open")]
    public static QueueActionDebugWindow init() {
        _instance = EditorWindow.GetWindow<QueueActionDebugWindow>();

        return _instance;
    }

    private List<string> _nameList;
    private void Awake()
    {
        //;
    }

    private Vector2 _scrollPos = new Vector2();
    private void OnGUI()
    {
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);
        {
            GUILayout.Space(15);
            _draw_toolTitle_UI();
            GUILayout.Space(15);
            _draw_Main_UI();
            GUILayout.Space(15);
        }
        GUILayout.EndScrollView();
        Repaint();
    }

    private void _draw_toolTitle_UI()
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("      GameAction队列监视器      ", titleStyle);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
        GUILayout.EndVertical();
    }

    private void _draw_subTitle_UI(string label)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(label, sTitleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private void _draw_Main_UI()
    {

        _nameList = QueueActionManager.GetQueueNames();
        if (_nameList.Count > 0)
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);

                for (int i = 0; i < _nameList.Count; i++)
                {
                    if (i > 0)
                    {
                        GUILayout.Space(5);
                    }

                    GUILayout.BeginHorizontal("box");
                    {
                        GUILayout.Label(_nameList[i]);
                    }
                    GUILayout.EndHorizontal();

                }
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
        }
    }

}