using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic
{
    /// <summary>
    /// 
    /// Todo ::
    ///     ----- 尚未解决的问题 ::
    /// 
    ///         **  目前深度排序问题靠材质写深度来实现. 
    ///             而使用BillBoardBase.Depth属性来模拟的深度排序只能解决一个SpriteEntity内部的排序,多个SpriteEntity可能为造成排序混乱.(已解决)
    /// 
    /// 
    /// </summary>
    public class BillBoardManager : ManagerBase
    {

        private static string _NameDefine = "BillBoardManager";

        //========= Manager 模版 =============================
        //
        // 基于MonoBehavior的Manager类 需要遵循的约定:
        //
        // *. 采用_instance字段保存静态单例.
        // *. 非自启动Manager必须提供CreateInstance静态方法.
        // *. 提供Request静态方法.
        // *. 提供IsInit静态方法判定改Manager是否初始化
        // *. 须Awake中调用ManagerBase.VerifyUniqueOnInit验证单例唯一
        // *. 须Awake中调用ManagerBase.VerifyUniqueOnInit验证单例唯一
        //
        //=============== 基于MonoBehavior的Manager====================

        //@@@ 静态方法实现区

        private static BillBoardManager _instance;
        public static BillBoardManager Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 创建带有独立GameObject的Instance
        /// </summary>
        public static BillBoardManager CreateInstance(Transform parenTransform = null)
        {
            ManagerBase.CreateInstance<BillBoardManager>(ref _instance, _NameDefine, parenTransform);
            //自动初始化
            if (!_instance._isInit)
            {
                _instance._isSetuped = true;
                _instance.__init();
            }
            return _instance;
        }

        /// <summary>
        /// 在目标GameObject上的创建Instance
        /// </summary>
        public static BillBoardManager CreateInstanceOnGameObject(GameObject target)
        {
            //自动初始化
            ManagerBase.CreateInstanceOnGameObject<BillBoardManager>(ref _instance, target);
            if (!_instance._isInit)
            {
                _instance._isSetuped = true;
                _instance.__init();
            }
            return _instance;
        }

        public static void Request(Action GraphicsManagerIniteDoSh)
        {
            CreateInstance();
            ManagerBase.Request(ref _instance, GraphicsManagerIniteDoSh);
        }

        public static bool IsInit()
        {
            return ManagerBase.VerifyIsInit(ref _instance);
        }

        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        //@@@ override方法实现区

        protected override void Awake()
        {
            base.Awake();
            ManagerBase.VerifyUniqueOnInit(ref _instance, this, () =>
            {
                gameObject.name = _NameDefine;
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ManagerBase.VerifyUniqueOnDispose(ref _instance, this);
        }

        //=======================================================================

        //@@@ Manager功能实现区

        private static Camera _mainCamera;
        public static Camera MainCamera
        {
            get
            {
                if (!_mainCamera)
                {
                    _mainCamera = GraphicsManager.IsInit() ? GraphicsManager.Instance.MainCamera : Camera.current;
                }
                return _mainCamera;
            }
        }

        public enum CombineMode
        {
            SimpleCombine, //简单合并同个材质
            ZShortCombine, //根据ZDepth合并材质
        }

        //TODO:处理顶点超过VBO上限或过多导致重置Mesh开销巨大的情况
        public const int DEFAULT_SPRITE_BUFFER_COUNT = 8;

        public CombineMode _combineMode = CombineMode.ZShortCombine;

        private readonly List<BillBoardBase> _billBoards = new List<BillBoardBase>();
        private readonly List<SpriteEntity> _entitys = new List<SpriteEntity>();

        //放弃使用字典管理
       // private readonly Dictionary<Material, SpriteEntity> _entitys = new Dictionary<Material, SpriteEntity>();

        protected bool _reBuildEntitys = false;

        protected void Update()
        {

            int i, len;
            if (_combineMode == CombineMode.ZShortCombine)
            {

                len = _billBoards.Count;
                for (i = 0; i < len; i++)
                {
                    BillBoardBase b3 = _billBoards[i];
                    float nlength = _getLength(b3.transform.position, MainCamera.transform.position);
                    if (!nlength.Equals(b3.Depth))
                    {
                        b3.Depth = nlength;
                        _reBuildEntitys = true;
                    }
                }

                if (_reBuildEntitys)
                {

                    len = _entitys.Count;
                    for (i = 0; i < len; i++)
                    {
                        _entitys[i].Dispose();
                    }
                    _entitys.Clear();

                    _billBoards.Sort((s1, s2) =>
                    {
                        return s1.Depth.CompareTo(s2.Depth);
                    });

                    Material lastMaterial = null;
                    int lastIdx = -1;
                    len = _billBoards.Count;
                    for (i = 0; i < len; i++)
                    {

                        BillBoardBase sprite = _billBoards[i];

                        SpriteEntity entity;
                        if (lastMaterial != sprite.Material)
                        {
                            entity = new SpriteEntity(sprite.Material);
                            entity.Entity.transform.SetParent(transform, false);
                            entity.Entity.layer = sprite.gameObject.layer;
                            _entitys.Add(entity);
                            lastIdx++;
                        }
                        _entitys[lastIdx].AddSprite(sprite);
                        lastMaterial = sprite.Material;
                    }

                    _reBuildEntitys = false;
                }
            }

            len = _entitys.Count;
            for (i = 0; i < len; i++)
            {
                _entitys[i].Update(i);
            }

        }

        public void AddSprite(BillBoardBase sprite)
        {
            if (!_billBoards.Contains(sprite))
            {
                _billBoards.Add(sprite);
                if (_combineMode == CombineMode.ZShortCombine)
                {
                    _reBuildEntitys = true;
                }else if (_combineMode == CombineMode.SimpleCombine)
                {
                    SpriteEntity entity = _entitys.Find(m => m.Material.Equals(sprite.Material));
                    if (entity == null)
                    {
                        entity = new SpriteEntity(sprite.Material);
                        entity.Entity.transform.SetParent(transform, false);
                        entity.Entity.layer = sprite.gameObject.layer;
                        _entitys.Add(entity);
                    }
                    entity.AddSprite(sprite);
                }
            }
        }

        public void RemoveSprite(BillBoardBase sprite)
        {
            if (_billBoards.Contains(sprite))
            {

                _billBoards.Remove(sprite);
                if (_combineMode == CombineMode.ZShortCombine)
                {
                    _reBuildEntitys = true;
                }else if (_combineMode == CombineMode.SimpleCombine)
                {
                    SpriteEntity entity = _entitys.Find(m => m.Material.Equals(sprite.Material));
                    if (entity != null)
                    {
                        entity.RemoveSprite(sprite);
                        if (entity.Count == 0)
                        {
                            _entitys.Remove(entity);
                            entity.Dispose();
                        }
                    }
                }

            }
        }

        private float _getLength(Vector3 a, Vector3 b)
        {
            float x = a.x - b.x;
            float y = a.y - b.y;
            float z = a.z - b.z;
            return Mathf.Abs(x*x + y*y + z*z);
        }

        //-------------------------------

        private class SpriteEntity
        {

            private const int RenderQueueReserve = 500;

            public GameObject Entity;
            public Mesh Mesh;
            public Vector3[] Vertices;
            public Vector2[] TexCoords;
            public Color[] Colors;
            public int[] Indices;

            public Material Material;

            private int _bufferCount;
            private readonly List<BillBoardBase> _sprites = new List<BillBoardBase>();

            private int _renderQueue;

            private MeshRenderer _meshRenderer;

            public int Count
            {
                get { return _sprites.Count; }
            }

            public SpriteEntity(Material mat)
            {

                Material = mat;

                Entity = new GameObject("BillBoardEntity@" + Material.name);
                Mesh = new Mesh { name = "BillBoardMesh@" + Material.name };

                _meshRenderer = Entity.AddComponent<MeshRenderer>();
                _meshRenderer.material = Material;

                _renderQueue = Material.shader.renderQueue;

                Entity.AddComponent<MeshFilter>().mesh = Mesh;



                _bufferCount = DEFAULT_SPRITE_BUFFER_COUNT;
                _InitAttributes();
            }

            private void _InitAttributes()
            {
                Vertices = new Vector3[_bufferCount * 4];
                TexCoords = new Vector2[_bufferCount * 4];
                Colors = new Color[_bufferCount * 4];
                Indices = new int[_bufferCount * 6];

                for (int i = 0; i < _bufferCount; i++)
                {
                    TexCoords[i * 4] = new Vector2(0, 0);
                    TexCoords[i * 4 + 1] = new Vector2(0, 1);
                    TexCoords[i * 4 + 2] = new Vector2(1, 1);
                    TexCoords[i * 4 + 3] = new Vector2(1, 0);

                    Colors[i * 4] = new Color(1, 1, 1, 1);
                    Colors[i * 4 + 1] = new Color(1, 1, 1, 1);
                    Colors[i * 4 + 2] = new Color(1, 1, 1, 1);
                    Colors[i * 4 + 3] = new Color(1, 1, 1, 1);

                    Indices[i * 6] = i * 4;
                    Indices[i * 6 + 1] = i * 4 + 1;
                    Indices[i * 6 + 2] = i * 4 + 3;
                    Indices[i * 6 + 3] = i * 4 + 3;
                    Indices[i * 6 + 4] = i * 4 + 1;
                    Indices[i * 6 + 5] = i * 4 + 2;
                }

                Mesh.Clear();
                Mesh.vertices = Vertices;
                Mesh.uv = TexCoords;
                Mesh.colors = Colors;
                Mesh.triangles = Indices;
            }

            private static Vector3 __CameraMatrixProjection(Camera cam, Transform trans, Vector3 point)
            {
                Vector3 v0 = cam.cameraToWorldMatrix * point;
                v0 += trans.position;
                return v0;
            }

            private int _addShaderSortCache = -1;

            public void Update(int addRenderQueue = 0)
            {
                if (!_addShaderSortCache.Equals(addRenderQueue))
                {
                    _addShaderSortCache = addRenderQueue;
                    _meshRenderer.material.renderQueue = _renderQueue + RenderQueueReserve - _addShaderSortCache;
                }

                _sprites.Sort((s1, s2) =>
                {
                    return s1.Depth.CompareTo(s2.Depth);
                });
                _sprites.Reverse();

                for (int i = 0; i < _sprites.Count; i++)
                {
                    if (_sprites[i].Forward)
                    {
                        if (MainCamera)
                        {
                            if (_sprites[i].UseLookAtMethod)
                            {
                                //LookAt方式
                                _sprites[i].transform.LookAt(MainCamera.transform.position, Vector3.up);
                                Vector3 eular = _sprites[i].transform.eulerAngles;
                                if (_sprites[i].LockToY)
                                {
                                    _sprites[i].transform.eulerAngles = new Vector3(0, eular.y + 180,
                                        0);
                                }
                                else
                                {
                                    _sprites[i].transform.eulerAngles = new Vector3(-eular.x, eular.y + 180,
                                        eular.z);
                                }
                                Matrix4x4 worldMatrix = _sprites[i].transform.localToWorldMatrix;
                                Vertices[i * 4] = worldMatrix.MultiplyPoint(_sprites[i].Vertices[0]);
                                Vertices[i * 4 + 1] = worldMatrix.MultiplyPoint(_sprites[i].Vertices[1]);
                                Vertices[i * 4 + 2] = worldMatrix.MultiplyPoint(_sprites[i].Vertices[2]);
                                Vertices[i * 4 + 3] = worldMatrix.MultiplyPoint(_sprites[i].Vertices[3]);
                            }
                            else
                            {
                                Transform _currentT = _sprites[i].transform;
                                BillBoardBase _currentBbs = _sprites[i];
                                Vertices[i * 4] = __CameraMatrixProjection(_mainCamera, _currentT, _currentBbs.Vertices[0]);
                                Vertices[i * 4 + 1] = __CameraMatrixProjection(_mainCamera, _currentT, _currentBbs.Vertices[1]);
                                Vertices[i * 4 + 2] = __CameraMatrixProjection(_mainCamera, _currentT, _currentBbs.Vertices[2]);
                                Vertices[i * 4 + 3] = __CameraMatrixProjection(_mainCamera, _currentT, _currentBbs.Vertices[3]);
                            }
                        }
                    }
                    else
                    {
                        Matrix4x4 worldMatrix = _sprites[i].transform.localToWorldMatrix;
                        Vertices[i * 4] = worldMatrix.MultiplyPoint(_sprites[i].Vertices[0]);
                        Vertices[i * 4 + 1] = worldMatrix.MultiplyPoint(_sprites[i].Vertices[1]);
                        Vertices[i * 4 + 2] = worldMatrix.MultiplyPoint(_sprites[i].Vertices[2]);
                        Vertices[i * 4 + 3] = worldMatrix.MultiplyPoint(_sprites[i].Vertices[3]);
                    }

                    Colors[i * 4] = _sprites[i].Colors[0];
                    Colors[i * 4 + 1] = _sprites[i].Colors[1];
                    Colors[i * 4 + 2] = _sprites[i].Colors[2];
                    Colors[i * 4 + 3] = _sprites[i].Colors[3];

                    TexCoords[i * 4] = _sprites[i].TexCoords[0];
                    TexCoords[i * 4 + 1] = _sprites[i].TexCoords[1];
                    TexCoords[i * 4 + 2] = _sprites[i].TexCoords[2];
                    TexCoords[i * 4 + 3] = _sprites[i].TexCoords[3];
                }
                Mesh.vertices = Vertices;
                Mesh.uv = TexCoords;
                Mesh.colors = Colors;
                Mesh.RecalculateBounds();
            }

            public void AddSprite(BillBoardBase sprite)
            {
                if (_sprites.Contains(sprite))
                {
                    return;
                }

                _sprites.Add(sprite);

                if (_bufferCount < _sprites.Count)
                {
                    _bufferCount *= 2;
                    _InitAttributes();
                }
            }

            public void RemoveSprite(BillBoardBase sprite)
            {
                if (!_sprites.Remove(sprite))
                {
                    return;
                }
                if (_bufferCount > DEFAULT_SPRITE_BUFFER_COUNT && _bufferCount > _sprites.Count * 4)
                {
                    _bufferCount /= 2;
                    _InitAttributes();
                }
                else
                {
                    Vertices[_sprites.Count * 4] = Vector3.zero;
                    Vertices[_sprites.Count * 4 + 1] = Vector3.zero;
                    Vertices[_sprites.Count * 4 + 2] = Vector3.zero;
                    Vertices[_sprites.Count * 4 + 3] = Vector3.zero;
                }
            }

            public void Dispose()
            {
                Destroy(Entity);
                Destroy(Mesh);
                _sprites.Clear();
            }
        }

    }
}
