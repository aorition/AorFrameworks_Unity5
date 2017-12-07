using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AorFramework.AorTile3D.runtimeEditor;


namespace AorFramework.AorTile3D.runtime
{

    /// <summary>
    /// AorTile3D 运行时主控制器
    /// 
    /// 
    /// 
    /// </summary>
    public class AorTile3D : MonoBehaviour
    {

        [SerializeField]
        private bool isEditorMode = false;

        [SerializeField]
        private bool isCreateNRCamera = false;
        [SerializeField]
        private bool isCreateTCamera = false;

        [SerializeField]
        private bool autoRefreshMapDraw = false;

        [SerializeField]
        private bool DrawEmptyTile = true;


        private void Awake()
        {
            AorTile3DManager.onManagerError += s =>
            {
                Debug.Log(s);
            };
        }

        private void Start()
        {

            //在场景创建是检查T相机
            AorTile3DManager.Instance.onCreateScene += _CheckTCamera;

            if (isEditorMode)
            {
                AorTile3DRuntimeEditor.GetInstance(transform, this);

                if (isCreateNRCamera)
                {
                    AorTile3DRuntimeEditor.Instance.SetNCameraActive(true);
                }

            }
            else
            {

                AorTile3DataInstaller installer = gameObject.GetComponent<AorTile3DataInstaller>();
                if (installer && installer.linkdTextData)
                {

                    string mapTextData = installer.linkdTextData.text;

                    TileMapData tmdata = TileMapDataUtils.CreateTileDataFormText(mapTextData);

                    //创建AorTile3DScene
                    AorTile3DScene scene;

                    if (installer.linkdTextInfoData)
                    {
                        string mapTextDataInfo = installer.linkdTextInfoData.text;
                        scene = TileMapDataUtils.CreateAorTile3DSceneWithText(mapTextDataInfo, tmdata);
                    }
                    else
                    {
                        scene = new AorTile3DScene(tmdata, installer.definedBorderCenter, installer.definedBorderHalfSize);
                    }

                    //创建SceneController
                    AorTile3DSceneController sceneController = new AorTile3DSceneController(scene, transform);
                    sceneController.onBorderChange += s =>
                    {
                        //数据边框改变后,需要重新刷新生成的实际地图
                        if (autoRefreshMapDraw)
                        {
                            RefreshMapDraw();
                        }
                    };

                    AorTile3DManager.Instance.setupTile3DScene(scene, sceneController);
                    RefreshMapDraw();

                    if (installer.AutoDestroyOnDataLoaded)
                    {
                        GameObject.Destroy(installer);
                    }

                    if (isCreateNRCamera)
                    {
                        NavigatorCameraHandler nch = sceneController.NCameraHandler;
                    }

                }
                
            }
            
        }

        private void Update()
        {
            if (AorTile3DManager.Instance != null)
            {
                AorTile3DManager.Instance.Update();
            }
        }

        private List<GameObject> _delGOList = new List<GameObject>(); 
        private List<string> _delKeysList = new List<string>(); 
        private HashSet<string> _tileRegSet = new HashSet<string>(); 
        private Dictionary<string,GameObject> _TileGOCacheDic = new Dictionary<string, GameObject>();
        private GameObject _loadTileGameObjectFormCache(string name, string path)
        {
            if (_TileGOCacheDic.ContainsKey(name))
            {
                return _TileGOCacheDic[name];
            }
            else
            {
                GameObject tdAsset = Resources.Load<GameObject>(path);
                if (tdAsset)
                {
                    GameObject tdGo = GameObject.Instantiate(tdAsset);
                    tdGo.name = name;
                    _TileGOCacheDic.Add(name, tdGo);
                    return tdGo;
                }
            }
            return null;
        }
        
        private void _CheckTCamera()
        {
            if (isCreateTCamera)
            {
                AorTile3DScene scene = AorTile3DManager.Instance.currentScene;
                AorTile3DSceneController sceneController = AorTile3DManager.Instance.sceneController;

                //初始化TCamera位置
                Vector3 pos = AorTile3DUtils.Int3ToVector3(scene.borderCenter);
                sceneController.TCameraHandler.SetPosition(new Vector3(
                    pos.x * scene.mapData.tileSize[0],
                    pos.z * scene.mapData.tileSize[1],
                    pos.y * scene.mapData.tileSize[2]
                    ), true);

                sceneController.TCameraHandler.onMouseClick = (i, c, a, s) =>
                {

                    if (i == 2)
                    {
                        Debug.Log("**　恢复默认相机FOV");
                    }

                };

            }
        }

        public void RefreshMapDraw()
        {
            if(AorTile3DManager.Instance == null || AorTile3DManager.Instance.currentScene == null) return;

            _tileRegSet.Clear();

            //            AorTile3DManager.Instance.sceneController.rebuildTilesLayer();
            AorTile3DScene scene = AorTile3DManager.Instance.currentScene;
            Transform _tl = AorTile3DManager.Instance.sceneController.tilesLayer;

            int[] min = scene.borderMin;
            int[] max = scene.borderMax;

            for (int u = min[0]; u < max[0]; u++)
            {
                for (int v = min[1]; v < max[1]; v++)
                {
                    TileData td = scene.mapData.GetTileData(u, v);
                    if (td != null)
                    {

                        string n = u + "_" + v + "_" + td.height;
                        string p = scene.mapData.GetTileModelPath(td.modelId);

                        GameObject tdGo = _loadTileGameObjectFormCache(n, p);
                        if (tdGo)
                        {

                            _tileRegSet.Add(tdGo.name);

                            tdGo.transform.SetParent(_tl, false);

                            tdGo.transform.position = new Vector3(
                                u*scene.mapData.tileSize[0],
                                td.height*scene.mapData.tileSize[2],
                                v*scene.mapData.tileSize[1]
                                );
                        }

                    }
                    else
                    {
                        //没有被填充TileData时
                        if (DrawEmptyTile)
                        {
                            string n = u + "_" + v + "_0";
                            string p = "Prefabs/DefaultTile";

                            GameObject tdGo = _loadTileGameObjectFormCache(n, p);
                            if (tdGo)
                            {

                                _tileRegSet.Add(tdGo.name);

                                tdGo.transform.SetParent(_tl, false);
                                tdGo.transform.localScale = new Vector3(
                                    scene.mapData.tileSize[0],
                                    scene.mapData.tileSize[2],
                                    scene.mapData.tileSize[1]
                                    );
                                tdGo.transform.position = new Vector3(
                                    u * scene.mapData.tileSize[0],
                                    0,
                                    v * scene.mapData.tileSize[1]
                                    );
                            }
                        }
                    }
                }
            }

            //清理多余
            foreach (string key in _TileGOCacheDic.Keys)
            {
                if (!_tileRegSet.Contains(key))
                {

                    GameObject del = _TileGOCacheDic[key];

                    _delKeysList.Add(key);
                    _delGOList.Add(del);
                }
            }

            foreach (string s in _delKeysList)
            {
                _TileGOCacheDic.Remove(s);
            }
            _delKeysList.Clear();

            foreach (GameObject o in _delGOList)
            {
                GameObject.Destroy(o);
            }
            _delGOList.Clear();
        }


    }

}

