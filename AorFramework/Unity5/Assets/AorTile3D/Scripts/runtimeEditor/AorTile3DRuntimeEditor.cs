using System;
using System.Collections.Generic;
using AorBaseUtility;
using AorFramework.AorTile3D.runtime;
using AorFrameworks;
using Assets.AorTile3D.Scripts.runtimeEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AorFramework.AorTile3D.runtimeEditor
{

    public class AorTile3DRuntimeEditor
    {

        #region static 方法集

        private static AorTile3DRuntimeEditor _instance;

        public static AorTile3DRuntimeEditor Instance
        {
            get { return _instance; }
        }

        public static AorTile3DRuntimeEditor GetInstance(Transform rooTransform, runtime.AorTile3D aorTile3D)
        {
            if (_instance == null)
            {
                _instance = new AorTile3DRuntimeEditor(rooTransform, aorTile3D);
                return _instance;
            }
            return null;
        }

        #endregion

        #region 事件接口

        public Action<int[]> onPressOnIndex;
        private void _onPressOnIndex(int[] ints)
        {
            if (onPressOnIndex != null) onPressOnIndex(ints);
            //
        }

        /// <summary>
        /// 当NCamera Active 发生改变
        /// </summary>
        public Action<bool> onNCameraActiveChanged;
        private void _onNCameraActiveChanged(bool active)
        {
            _UIView.setUIEnable(AorTile3DEditorUIView.ATDESubDefine.NmapArea, active);
            if (onNCameraActiveChanged != null)
                onNCameraActiveChanged(active);
        }

        #endregion


        ///////////////////////////////////////////
        // Define
        private int UICamera_depth = 100;
        private bool UICamera_orthographic = true;
        private float UICamera_orthographicSize = 100;
        private int UICamera_sortingOrder = 31;
        private Vector3 UICamera_localPosition = new Vector3(0, -10000, 0);

        private Vector2 UICanvas_referenceResolution = new Vector2(1024, 768);
        private float UICanvas_matchWidthOrHeight = 1;

        private runtime.AorTile3D _AorTile3D;

        private Transform _rooTransform;
        public Transform rooTransform
        {
            get { return _rooTransform; }
        }

        private Transform _rEditorRooTransform;
        public Transform EditorRooTransform
        {
            get { return _rEditorRooTransform; }
        }

        private Canvas _rUICanvas;
        public Canvas UICanvas
        {
            get { return _rUICanvas; }
        }

        private AorTile3DEditorUIView _UIView;
        public AorTile3DEditorUIView UIView
        {
            get { return _UIView; }
        }

        private AorTile3DRuntimeEditor(Transform rooTransform, runtime.AorTile3D aorTile3D)
        {
            _rooTransform = rooTransform;
            _AorTile3D = aorTile3D;

            GameObject rRoot = new GameObject("_EditorRoot");
            _rEditorRooTransform = rRoot.transform;
            _rEditorRooTransform.SetParent(_rooTransform, false);
            _rEditorRooTransform.SetAsFirstSibling();

            CreateREUI();

            //初始事件
            AorTile3DManager.Instance.onNCameraActiveChanged += _onNCameraActiveChanged;
            AorTile3DManager.Instance.onCreateScene += _OnCreateSenceDo;
            AorTile3DManager.Instance.onDisposeScene += _OnDisposeSenceDo;
        }

        public void Dispose()
        {
            //初始事件
            AorTile3DManager.Instance.onNCameraActiveChanged -= _onNCameraActiveChanged;
            AorTile3DManager.Instance.onCreateScene -= _OnCreateSenceDo;
            AorTile3DManager.Instance.onDisposeScene -= _OnDisposeSenceDo;

            DestroyLinstener();

            if (_instance == this) _instance = null;
        }

        private void StartLinstener()
        {
            _UIView.onToolBarStatusChanged += _OnToolBarStatusChanged;
        }

        private void DestroyLinstener()
        {

            _UIView.onToolBarStatusChanged -= _OnToolBarStatusChanged;
            
            if (AorTile3DManager.Instance.sceneController == null) return;
            AorTile3DManager.Instance.sceneController.onPressOnIndex -= _onPressOnIndex;
        }

        private void CreateREUI()
        {


            GameObject cr = new GameObject("eUICamera");
            cr.transform.SetParent(_rEditorRooTransform, false);
            cr.transform.SetAsFirstSibling();
            cr.transform.localPosition = UICamera_localPosition;
            Camera uiCamera = cr.AddComponent<Camera>();
            uiCamera.clearFlags = CameraClearFlags.Depth;
            uiCamera.depth = UICamera_depth;
            uiCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");
            uiCamera.orthographic = UICamera_orthographic;
            uiCamera.orthographicSize = UICamera_orthographicSize;

            GameObject ec = new GameObject("eCanvas");
            ec.layer = LayerMask.NameToLayer("UI");
            ec.transform.SetParent(_rEditorRooTransform, false);

            _rUICanvas = ec.AddComponent<Canvas>();
            _rUICanvas.renderMode = RenderMode.ScreenSpaceCamera;
            _rUICanvas.worldCamera = uiCamera;
            _rUICanvas.sortingOrder = UICamera_sortingOrder;

            CanvasScaler canvasScaler = ec.AddComponent<CanvasScaler>();
            canvasScaler.referenceResolution = UICanvas_referenceResolution;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.matchWidthOrHeight = UICanvas_matchWidthOrHeight;

//            GraphicRaycaster gr = ec.AddComponent<GraphicRaycaster>();
            ec.AddComponent<GraphicRaycaster>();
            
            AorResourceLoader.LoadPrefab(AorTile3dEditorUIDefines.MainUIPath, UIgo =>
            {
                UIgo.transform.SetParent(ec.transform, false);
                _UIView = UIgo.AddComponent<AorTile3DEditorUIView>();
            });
        }

        /// <summary>
        //  控制NCamera隐藏或者显示
        /// </summary>
        public void SetNCameraActive(bool active)
        {
            if (AorTile3DManager.Instance != null && AorTile3DManager.Instance.sceneController != null)
            {
                AorTile3DManager.Instance.sceneController.SetNCameraActive(active);
            }
        }

        private void _OnCreateSenceDo()
        {
            if (_UIView && _UIView.EnableUIBG)
            {
                //打开UITool
                _UIView.setUIEnable(AorTile3DEditorUIView.ATDESubDefine.ToolArea, true);
                //关闭UI背景
                _UIView.EnableUIBG = false;
                
            }
        }
        private void _OnDisposeSenceDo()
        {

            if (_UIView && !_UIView.EnableUIBG)
            {
                //关闭UITool
                _UIView.setUIEnable(AorTile3DEditorUIView.ATDESubDefine.ToolArea, false);
                //开启UI背景
                _UIView.EnableUIBG = true;
            }
        }

        private void _OnToolBarStatusChanged(ATEToolBarView.ToolBarStatus status)
        {
            //test code 
            //工具改变
            Debug.Log("********** 工具改变 :: " + status);
        }

        private void setupTile3DScene(AorTile3DScene scene)
        {
            AorTile3DSceneController controller = new AorTile3DSceneController(scene, _rooTransform);
            controller.onPressOnIndex += ints =>
            {
                //test code
                Debug.Log("onPressOnIndex > [" + ints[0]
                          + "," + ints[1]
                          + "," + ints[2]
                          + "]"
                    );
            };
            controller.onBorderChange += s =>
            {
                _AorTile3D.RefreshMapDraw();

                //test code
                Debug.Log("onBorderChange > [" + s.borderCenter[0]
                          + "," + s.borderCenter[1]
                          + "," + s.borderCenter[2]
                          + "]"
                    );

            };

            AorTile3DManager.Instance.setupTile3DScene(scene, controller);

            //这里状态笔刷
            controller.TCameraHandler.onPressOnIndex += _onPressOnIndex;

            StartLinstener();
        }

        #region 打开新建界面

        public void OpenBuildSceneUI()
        {

            //Todo 检查当前是否有正在编辑的Scene

            AorResourceLoader.LoadPrefab(AorTile3dEditorUIDefines.BuildSceneViewPath, g =>
            {
                g.transform.SetParent(Instance.UICanvas.transform, false);
                g.AddComponent<BuildSceneView>().OnBuildBtnClick = (view) =>
                {
                    float[] size = view.GetIFDUnitSize();
                    int[] length = view.GetIFDMapSize();
                    if (size != null && length != null)
                    {
                        TileMapData tmData = new TileMapData(size[0], size[1], size[2], length[0], length[1]);
                        AorTile3DScene scene = new AorTile3DScene(tmData, new[] {0, 0, 0}, new[] {10, 10, 5});

                        setupTile3DScene(scene);

                    }
                    else
                    {
                        AorTile3DManager.ThrowError(
                            "*** AorTile3DRuntimeEditor.buildEditMapSecne Error :: 缺失必要参数, 创建mapData失败.");
                    }
                };
                //
            });
        }

        #endregion

        #region 打开load界面

        public void OpenLoadDataUI()
        {

            //Todo 检查当前已经存在的地图数据
            if (AorTile3DManager.Instance.currentScene != null)
            {
                //销毁当前场景
                AorTile3DManager.Instance.DisposeCurrentScene();
            }

            AorRTFBrowser.CreateBrowser(Instance.UICanvas.GetComponent<RectTransform>(),
            view =>
            {
                view.Title = "打开地图";
                view.VerificationExist = true;
                view.onClickOKButton += (p, n) =>
                {
                    bool hasSuffix = n.Contains(".");

                    string infoName;
                    string dataName;
                    if (hasSuffix)
                    {
                        dataName = n;
                        string[] ts = n.Split('.');
                        infoName = ts[0] + "_info." + ts[1];
                    }
                    else
                    {
                        dataName = n + ".txt";
                        infoName = n + "_info.txt";
                    }

                    string infoStr = AorIO.ReadStringFormFile(p + "/" + infoName);
                    string dataStr = AorIO.ReadStringFormFile(p + "/" + dataName);

                    AorTile3DScene scene = TileMapDataUtils.CreateAorTile3DSceneWithText(infoStr, dataStr);
                    setupTile3DScene(scene);
                };
            });
        }

        #endregion

        #region 打开save界面

        public void OpenSaveDataUI()
        {
            AorRTFBrowser.CreateBrowser(Instance.UICanvas.GetComponent<RectTransform>(),
            view =>
            {
                view.Title = "保存地图";
                view.onClickOKButton += (p, n) =>
                {
                    string path = p;
                    string fname = string.IsNullOrEmpty(n) ? "AorTileMapData" : n;

                    if (fname.Contains("."))
                    {
                        fname = fname.Substring(0, fname.IndexOf('.'));
                    }

                    if (AorTile3DManager.Instance.currentScene != null)
                    {
                        string dataInfo = TileMapDataUtils.ExprotTileDataInfoToText(AorTile3DManager.Instance.currentScene);
                        AorIO.SaveStringToFile(path + "/" + fname + "_info.txt", dataInfo);
                        string data = TileMapDataUtils.ExportTileDataToText(AorTile3DManager.Instance.currentScene.mapData);
                        AorIO.SaveStringToFile(path + "/" + fname + ".txt", data);
                    }
                    else
                    {
                        //Todo 当前没有地图数据
                    }

                };
            });
        }

        #endregion

        public void createSecne(TileMapData tmData)
        {

        }

    }
}
