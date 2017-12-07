using System;
using System.Collections.Generic;
using AorFramework.AorTile3D.runtime;
using AorFramework.AorTile3D.runtimeEditor;
using AorFrameworks;
using UnityEngine;

namespace Assets.AorTile3D.Scripts.runtimeEditor
{
    public class AorTile3DEditorUIView : MonoBehaviour
    {

        public enum ATDESubDefine
        {
            MenuArea,
            NmapArea,
            TileSettingArea,
            ToolArea,
            OtherArea,

            Counts
        }
        public enum ATDEUIViewStates
        {
            Menu,
            NMap,
            TileSetting,
            Tool,
            Other,

            Counts
        }

        #region Parameters Define

        public float _NmapSize = 0.8f;
        public float _TileSettingArea_width = 600;
        public float _menuArea_height = 50;
        public float _toolArea_height = 60;

        //define sub UI default state
        //MenuArea,NmapArea,TileSettingArea,ToolArea,OtherArea
//        private int[] _UIStates = {1, 0, 0, 0, 0};
        private int[] _UIStates = {1, 1, 1, 1, 1};

        #endregion

        #region Fields Define

        private bool _isInit = false;

        private RectTransform _UIBG;
        /// <summary>
        /// 显示/关闭EditorUI背景
        /// </summary>
        public bool EnableUIBG
        {
            get
            {
                if (_UIBG)
                {
                    return _UIBG.gameObject.activeSelf;
                }
                return false;
            }
            set
            {
                if (_UIBG)
                {
                    if(_UIBG.gameObject.activeSelf != value) _UIBG.gameObject.SetActive(value);
                }
            }
        }

        private Dictionary<ATDESubDefine, RectTransform> _SubUIDic;

        private ATEMenuView _menuView;
        private ATEToolBarView _toolbarView;
      //  private planCass _planView;

        private bool _isDirty = false;

        #endregion

        #region Event Define;

        public Action<ATEToolBarView.ToolBarStatus> onToolBarStatusChanged;
        private void _onToolBarStatusChanged(ATEToolBarView.ToolBarStatus status)
        {
            if (onToolBarStatusChanged != null)
            {
                onToolBarStatusChanged(status);
            }
        }

        #endregion

        private void Awake()
        {

            _UIBG = transform.FindRectTransform("_UIBG");

            _SubUIDic = new Dictionary<ATDESubDefine, RectTransform>();
            int i, len = (int) ATDESubDefine.Counts;
            for (i = 0; i < len; i++)
            {
                ATDESubDefine define = (ATDESubDefine) i;
                RectTransform subRT = transform.FindRectTransform(define.ToString());
                if (!subRT)
                {
                    Debug.LogError("Init Error ..");
                    return;
                }
                _SubUIDic.Add(define, subRT);
            }

            _isInit = true;
        }

        private void OnEnable()
        {
            //            if (_isInit)
            //            {
            //                updateUI();
            //            }
            updateUI();
        }
        
        private void Update()
        {
            if (_isDirty)
            {
                updateUI();
                _isDirty = false;
            }
        }

        public void setUIEnable(ATDESubDefine subUI, bool enable)
        {
            _UIStates[(int) subUI] = enable ? 1 : 0;
            _isDirty = true;
        }

        private void updateUI()
        {
            
            RectTransform subT;

            //Nmap
            #region Nmap

            subT = _SubUIDic[ATDESubDefine.NmapArea];
            if (_UIStates[(int) ATDEUIViewStates.NMap].Equals(0))
            {
                if (subT.gameObject.activeSelf)
                    subT.gameObject.SetActive(false);
            }
            else
            {
                if (!subT.gameObject.activeSelf)
                    subT.gameObject.SetActive(true);
            }

            #endregion

            //Menu
            #region Menu
            subT = _SubUIDic[ATDESubDefine.MenuArea];
            if (!_menuView)
            {
                //初始化
                AorResourceLoader.LoadPrefab(AorTile3dEditorUIDefines.MenuViewPath, g =>
                {
                    g.transform.SetParent(_SubUIDic[ATDESubDefine.MenuArea], false);
                    _menuView = g.AddComponent<ATEMenuView>();
                    _menuView.rootView = this;
                    //
                });
            }
            if (_UIStates[(int) ATDEUIViewStates.Menu].Equals(0))
            {
                if (subT.gameObject.activeSelf)
                    subT.gameObject.SetActive(false);
            }
            else
            {
                Vector2 maxValue = Vector2.one;
                if (!_UIStates[(int) ATDEUIViewStates.NMap].Equals(0))
                {
                    maxValue = new Vector2(_NmapSize, 1);
                }
                subT.SmartResize(new Vector2(0, 1), new Vector2(0, 1), maxValue, Vector3.zero, new Vector2(0, _menuArea_height));

                if (!subT.gameObject.activeSelf)
                    subT.gameObject.SetActive(true);
            }

            #endregion

            //TileSettingArea
            #region TileSettingArea

            subT = _SubUIDic[ATDESubDefine.TileSettingArea];
            //test code
//            if (!_planView)
//            {
//                //初始化
//                AorResourceLoader.LoadPrefab(AorTile3dEditorUIDefines.MenuViewPath, g =>
//                {
//                    g.transform.SetParent(_SubUIDic[ATDESubDefine.MenuArea], false);
//                    _menuView = g.AddComponent<ATEMenuView>();
//                    _menuView.rootView = this;
//                    //
//                });
//            }

            if (_UIStates[(int)ATDEUIViewStates.TileSetting].Equals(0))
            {
                if (subT.gameObject.activeSelf)
                    subT.gameObject.SetActive(false);
            }
            else
            {

                Vector2 anchoredPos = Vector2.zero;
                if (!_UIStates[(int)ATDEUIViewStates.Menu].Equals(0))
                {
                    anchoredPos = new Vector2(0, _menuArea_height);
                }
                subT.SmartResize(new Vector2(0, 0.5f), new Vector2(0, 0), new Vector2(0,1), anchoredPos, new Vector2(_TileSettingArea_width, 0));

                if (!subT.gameObject.activeSelf)
                    subT.gameObject.SetActive(true);
            }

            #endregion

            //ToolArea
            #region Tool

            subT = _SubUIDic[ATDESubDefine.ToolArea];

            if (!_toolbarView)
            {
                //初始化
                AorResourceLoader.LoadPrefab(AorTile3dEditorUIDefines.ToolBarView, g =>
                {
                    g.transform.SetParent(_SubUIDic[ATDESubDefine.ToolArea], false);
                    _toolbarView = g.AddComponent<ATEToolBarView>();
                    _toolbarView.rootView = this;
                    _toolbarView.onToolBarStatusChanged += _onToolBarStatusChanged;
                    //
                });
            }

            if (_UIStates[(int)ATDEUIViewStates.Tool].Equals(0))
            {
                if (subT.gameObject.activeSelf)
                    subT.gameObject.SetActive(false);
            }
            else
            {
                Vector2 anchoredPos = Vector2.zero;
                if (!_UIStates[(int)ATDEUIViewStates.TileSetting].Equals(0))
                {
                    anchoredPos = new Vector2(_TileSettingArea_width, 0);
                }
                subT.SmartResize(new Vector2(0.5f, 0), new Vector2(0, 0), new Vector2(1, 0), anchoredPos, new Vector2(0, _toolArea_height));

                if (!subT.gameObject.activeSelf)
                    subT.gameObject.SetActive(true);
            }

            #endregion

            //OtherArea
            #region Other

            subT = _SubUIDic[ATDESubDefine.OtherArea];
            if (_UIStates[(int)ATDEUIViewStates.Other].Equals(0))
            {
                if (subT.gameObject.activeSelf)
                    subT.gameObject.SetActive(false);
            }
            else
            {
                Vector2 anchorMax = Vector2.one;
                Vector2 anchorPos = Vector2.zero;
                Vector2 sizeDelta = Vector2.zero;
                if (!_UIStates[(int) ATDEUIViewStates.NMap].Equals(0))
                {
                    anchorMax = new Vector2(1, _NmapSize);
                }
                else if (!_UIStates[(int)ATDEUIViewStates.Menu].Equals(0))
                {
                    anchorPos = new Vector2(0, _menuArea_height);
                }

                if (!_UIStates[(int) ATDEUIViewStates.Tool].Equals(0))
                {
                    sizeDelta = new Vector2(0, _toolArea_height);
                }

                subT.SmartResize(new Vector2(1, 0.5f), new Vector2(_NmapSize, 0), anchorMax, anchorPos, sizeDelta);

                if (!subT.gameObject.activeSelf)
                    subT.gameObject.SetActive(true);
            }

            #endregion

        }

    }
}
