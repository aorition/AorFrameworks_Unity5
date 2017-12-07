using System;
using System.Collections.Generic;
using AorFramework.AorTile3D.runtimeEditor;
using UnityEngine;
using UnityEngine.UI;
using AorFrameworks;

namespace Assets.AorTile3D.Scripts.runtimeEditor
{
    public class ATEToolBarView : MonoBehaviour
    {

        public Action<ToolBarStatus> onToolBarStatusChanged;

        public enum ToolBarStatus
        {
            noTool,
            select,
            brush,
        }

        public AorTile3DEditorUIView rootView;

        private RectTransform _toolArea;
        private RectTransform _optionArea;

        private List<Button> _btnCache = new List<Button>();

        private ToolBarStatus _currentToolBarStatus;

        private void Awake()
        {
            _toolArea = transform.FindRectTransform("ToolArea");
            _optionArea = transform.FindRectTransform("OptionArea");

            setupUI();
        }

        private void OnDestroy()
        {
            onToolBarStatusChanged = null;
            rootView = null;
            _toolArea = null;
            _optionArea = null;

            _btnCache.Clear();
            _btnCache = null;
        }

        private void setupUI()
        {

            createToolBarBtn(AorTile3dEditorUIDefines.icon_toolbar_noTool, ToolBarStatus.noTool, (btn) =>
            {
                //Debug.Log("**************** ");
            }, true);

            createToolBarBtn(AorTile3dEditorUIDefines.icon_toolbar_select, ToolBarStatus.select, (btn) =>
            {
                //Debug.Log("**************** ");
            });

            //test code
            createToolBarBtn(AorTile3dEditorUIDefines.icon_toolbar_brush, ToolBarStatus.brush, (btn) =>
            {
                //Debug.Log("**************** ");
            });

        }

        private void _SetBtnSelected(Button btn)
        {

            string selectName = btn.name;
            for (int i = 0; i < _btnCache.Count; i++)
            {
                if (selectName == _btnCache[i].name)
                {
                    _btnCache[i].interactable = false;
                }
                else
                {
                    _btnCache[i].interactable = true;
                }
            }
            _currentToolBarStatus = (ToolBarStatus)Enum.Parse(typeof(ToolBarStatus), selectName);
            if (onToolBarStatusChanged != null) onToolBarStatusChanged(_currentToolBarStatus);
        }

        /// <summary>
        /// 创建按钮
        /// </summary>
        public void createToolBarBtn(string iconPath, ToolBarStatus btnName, Action<Button> onClickAction, bool defaultSelect = false)
        {
            AorResourceLoader.LoadPrefab(AorTile3dEditorUIDefines.ToolBarButton, g =>
            {
                g.transform.SetParent(_toolArea, false);
                //

                Image image = g.GetComponent<Image>();

                AorResourceLoader.LoadSprite(iconPath, (sp) =>
                {
                    image.sprite = sp;
                });

                Button btn = g.GetComponent<Button>();
                btn.name = btnName.ToString();
                btn.onClick.AddListener(() =>
                {
                    _SetBtnSelected(btn);
                    onClickAction(btn);
                });

                _btnCache.Add(btn);

                if (defaultSelect)
                {
                    _SetBtnSelected(btn);
                }
            });

        }

    }
}
