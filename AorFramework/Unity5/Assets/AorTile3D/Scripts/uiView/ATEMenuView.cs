using System;
using System.Collections.Generic;
using AorFramework.AorTile3D.runtimeEditor;
using AorFrameworks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.AorTile3D.Scripts.runtimeEditor
{
    public class ATEMenuView : MonoBehaviour
    {

        public AorTile3DEditorUIView rootView;

        private RectTransform _iconArea;
        private RectTransform _menuArea;

        private void Awake()
        {
            _iconArea = transform.FindRectTransform("IconArea");
            _menuArea = transform.FindRectTransform("MenuArea");

            setupUI();
        }

        private void setupUI()
        {
            
            createMenuBtn("新建", (b) =>
            {
                AorTile3DRuntimeEditor.Instance.OpenBuildSceneUI();
            });
            createMenuBtn("打开", (b) =>
            {
                AorTile3DRuntimeEditor.Instance.OpenLoadDataUI();
            });
            createMenuBtn("保存", (b) =>
            {
                AorTile3DRuntimeEditor.Instance.OpenSaveDataUI();
            });
            //Test Code
            createMenuBtn("测试按钮4", (b) =>
            {
                Debug.Log("***** 测试按钮4");
            });
            createMenuBtn("测试按钮5", (b) =>
            {
                Debug.Log("***** 测试按钮5");
            });
        }

        /// <summary>
        /// 创建按钮
        /// </summary>
        public void createMenuBtn(string label, Action<Button> onClickAction)
        {
            AorResourceLoader.LoadPrefab(AorTile3dEditorUIDefines.MenuBtnPath, g =>
            {
                g.transform.SetParent(_menuArea, false);
                //

                Text lb = g.transform.Find("Text").GetComponent<Text>();

                RTSizeListener rlListener = g.AddComponent<RTSizeListener>();
                rlListener.Tartget = lb.rectTransform;
                rlListener.onTargetSizeChange += (v2, rt, tar) =>
                {
                    rt.sizeDelta = new Vector2(v2.x + 20, rt.sizeDelta.y);
                };
                
                lb.text = label;

                Button btn = g.GetComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    onClickAction(btn);
                });
            });

        }

    }
}
