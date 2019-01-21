using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.editor
{

    public class BrushEditorBase : UnityEditor.Editor
    {

        #region 笔刷属性

        public bool BrushRelative
        {
            get { return EditPaintBrush.BrushRelative; }
            set { EditPaintBrush.BrushRelative = value; }
        }

        public EditPaintBrush.PaintHandle PaintPrev
        {
            get { return EditPaintBrush.PaintPrev; }
            set { EditPaintBrush.PaintPrev = value; }
        }

        public float MaskTexUVCoord
        {
            get { return EditPaintBrush.MaskTexUVCoord; }
            set { EditPaintBrush.MaskTexUVCoord = value; }
        }

        public Transform CurrentSelect
        {
            get { return EditPaintBrush.CurrentSelect; }
            set { EditPaintBrush.CurrentSelect = value; }
        }

        public int BrushSize
        {
            get { return EditPaintBrush.BrushSize; }
            set { EditPaintBrush.BrushSize = value; }
        }

        public int LayerMask
        {
            get { return EditPaintBrush.LayerMask; }
            set { EditPaintBrush.LayerMask = value; }
        }

        public Projector BrushPrev
        {
            get { return EditPaintBrush.BrushPrev; }
        }

        public void BrushPrevDispose()
        {
            EditPaintBrush.BrushPrevDispose();
        }

        #endregion

        //---------------------------------------------------

        /// <summary>
        /// 销毁时亦销毁笔刷对象
        /// </summary>
        protected virtual void OnDestroy()
        {
            BrushPrevDispose();
        }

        /// <summary>
        /// 绘制开关
        /// </summary>
        public bool PaintingEnable = true;

        /// <summary>
        /// 在OnSceneGUI方法之前调用, 可以在此判定是否需要进行笔刷绘制
        /// </summary>
        protected virtual void OnPrePainting()
        {
            //
        }

        /// <summary>
        /// 笔刷绘制处理方法, 笔刷在探测可绘制时调用此函数
        /// </summary>
        protected virtual void OnPainting(Event e, RaycastHit raycastHit)
        {
            //复写此方法实现更复杂的事件逻辑处理 ...
        }

        /// <summary>
        /// 在笔刷状态为EditPaintBrush.PaintHandle.Classic时调用该方法,子类覆盖此方法用于实现brushPrev如何显示
        /// </summary>
        protected virtual void OnBrushPreview()
        {
            
        }

        protected  virtual void OnSceneGUI()
        {

            //处理依赖组件
            _checkComponentDependent();

            OnPrePainting();

            if (!PaintingEnable)
            {
                BrushPrevDispose();
                return;
            }

            //归置当前选择
            CurrentSelect = Selection.activeTransform;

            Event e = Event.current;
            HandleUtility.AddDefaultControl(0);
            RaycastHit raycastHit = new RaycastHit();

            if (    (e.type == EventType.KeyDown && e.keyCode == KeyCode.KeypadPlus)
                ||  (e.type == EventType.KeyDown && e.keyCode == KeyCode.Equals && e.shift)
            )
            {
                BrushSize += 1;
            }
            else if (   (e.type == EventType.KeyDown && e.keyCode == KeyCode.KeypadMinus)
                 ||     (e.type == EventType.KeyDown && e.keyCode == KeyCode.Minus && e.shift)    
            ){
                BrushSize -= 1;
            }

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, LayerMask))
            {
                CurrentSelect = raycastHit.transform;
                if (CurrentSelect)
                {
                    BrushPrev.transform.localEulerAngles = new Vector3(90, CurrentSelect.localEulerAngles.y, 0);
                    BrushPrev.transform.position = raycastHit.point;

                    if (PaintPrev != EditPaintBrush.PaintHandle.Classic &&
                        PaintPrev != EditPaintBrush.PaintHandle.Hide_preview &&
                        PaintPrev != EditPaintBrush.PaintHandle.Follow_Normal_WireCircle)
                    {
                        Handles.color = new Color(1f, 1f, 0f, 0.05f);
                        Handles.DrawSolidDisc(raycastHit.point, raycastHit.normal,
                            BrushPrev.orthographicSize * 0.9f);
                    }
                    else if (PaintPrev != EditPaintBrush.PaintHandle.Classic &&
                             PaintPrev != EditPaintBrush.PaintHandle.Hide_preview &&
                             PaintPrev != EditPaintBrush.PaintHandle.Follow_Normal_Circle)
                    {
                        Handles.color = new Color(1f, 1f, 0f, 1f);
                        Handles.DrawWireDisc(raycastHit.point, raycastHit.normal,
                            BrushPrev.orthographicSize*0.9f);
                    }

                    if (PaintPrev == EditPaintBrush.PaintHandle.Hide_preview)
                    {
                        BrushPrev.enabled = false;
                    }
                    else
                    {
                        BrushPrev.enabled = true;
                        OnBrushPreview();
                    }

                    OnPainting(e, raycastHit);
                }
            }

        }

        protected Collider _collider;

        /// <summary>
        /// 检查依赖组件
        /// </summary>
        protected virtual void _checkComponentDependent()
        {
            if (!_collider)
            {
                _collider = (target as MonoBehaviour).GetComponent<Collider>();
                if (!_collider)
                {
                    _collider = (target as MonoBehaviour).gameObject.AddComponent<MeshCollider>();

                    SkinnedMeshRenderer skinned = (target as MonoBehaviour).gameObject.GetComponent<SkinnedMeshRenderer>();
                    if (skinned) (_collider as MeshCollider).sharedMesh = skinned.sharedMesh;
                }
            }
        }

    }

    public static class EditPaintBrush
    {

        public const string InsNameDefine = "EditPainterInc";
        public const string InsDisposeTagDefine = "(Disposed)";

        public enum PaintHandle
        {
            Classic,
            Follow_Normal_Circle,
            Follow_Normal_WireCircle,
            Hide_preview
        }

        public static PaintHandle PaintPrev = PaintHandle.Follow_Normal_Circle;
        public static float MaskTexUVCoord = 1f;

        /// <summary>
        /// 表示笔刷大小是否相对于绘制对象的大小, 依赖于CurrentSelect属性
        /// </summary>
        public static bool BrushRelative = true;

        private static void __UpdateMeshSize()
        {
            if (_currentSelect)
            {
                Terrain terrain = CurrentSelect.GetComponent<Terrain>();
                MeshFilter SizeOfGeo = CurrentSelect.GetComponent<MeshFilter>();
                SkinnedMeshRenderer skinned = CurrentSelect.GetComponent<SkinnedMeshRenderer>();
                if (terrain)
                {
                    MeshSize = new Vector2(terrain.terrainData.heightmapWidth,
                        terrain.terrainData.heightmapHeight);
                }
                else if (skinned)
                {
                    MeshSize = new Vector2(skinned.bounds.size.x, skinned.bounds.size.z);
                }
                else if (SizeOfGeo)
                {
                    MeshSize = new Vector2(SizeOfGeo.sharedMesh.bounds.size.x,
                        SizeOfGeo.sharedMesh.bounds.size.z);
                }
            }
        }

        /// <summary>
        /// 当前绘制绘制对象
        /// </summary>
        private static Transform _currentSelect;
        public static Transform CurrentSelect
        {
            get
            {
                return _currentSelect;
            }
            set
            {
                _currentSelect = value;
            }
        }

        private static void __updateBrushPrevOrthographicSize()
        {
            if (BrushRelative && _currentSelect)
            {
                __UpdateMeshSize();
                _brushPrev.orthographicSize = (_brushSize*CurrentSelect.localScale.x) * (MeshSize.x / 100);
            }
            else
            {
                _brushPrev.orthographicSize = _brushSize * 0.1f;
            }
        }

        private static Vector2 MeshSize = Vector2.one;
        private static int _brushSize = 10;
        public static int BrushSize
        {
            get { return _brushSize; }
            set
            {
                _brushSize = value;
                if (_brushPrev)
                {
                    __updateBrushPrevOrthographicSize();
                }
            }
        }

        public static int LayerMask = -1;
        //----

        private static Projector _brushPrev;
        public static Projector BrushPrev
        {
            get
            {
                if (!_brushPrev)
                {
                    if (!CurrentSelect) return null;
                    var ProjectorB = GameObject.Find(InsNameDefine);
                    if (!ProjectorB) ProjectorB = new GameObject(InsNameDefine);
                    _brushPrev = ProjectorB.GetComponent(typeof(Projector)) as Projector;
                    if (!_brushPrev) _brushPrev = ProjectorB.AddComponent(typeof(Projector)) as Projector;
                    ProjectorB.hideFlags = HideFlags.HideInHierarchy;// | HideFlags.DontSave;
                    _brushPrev.orthographic = true; //先设置类型，再修改参数，不然会覆盖

                    Material material = EditorGUIUtility.LoadRequired("SceneView/TerrainBrushMaterial.mat") as Material;
                    material.SetTexture("_CutoutTex", (Texture)EditorGUIUtility.Load(EditorResourcesUtility.brushesPath + "brush_cutout.png"));
                    Debug.LogWarning(material);
                    _brushPrev.material = material;
                    _brushPrev.nearClipPlane = -20f;
                    _brushPrev.farClipPlane = 20f;
                    __updateBrushPrevOrthographicSize();
                    _brushPrev.ignoreLayers =LayerMask;
                    _brushPrev.transform.Rotate(90, -90, 0);
                }
                return _brushPrev;
            }
        }

        public static void BrushPrevDispose()
        {
            if (_brushPrev)
            {
                GameObject del = _brushPrev.gameObject;
                _brushPrev = null;

                del.name = del.name + InsDisposeTagDefine;
                del.SetActive(false);
                EditorPlusMethods.NextEditorApplicationUpdateDo(() =>
                {
                    if (Application.isPlaying)
                    {
                        GameObject.Destroy(del);
                    }
                    else
                    {
                        GameObject.DestroyImmediate(del);
                    }
                });
            }
        }

        public static float Opacity = 1f;

        private static float[] _BrushTexStrength;
        private static int _BrushTexSize;
        private static Texture2D _BrushMask;
        public static Texture2D BrushMask
        {
            get { return _BrushMask; }
        }

        private static Texture2D m_Preview;

        //---------------------------------------

        public static bool Load(Texture2D brushTex, int size)
        {
            if (_brushPrev != null && m_Preview != null)
                _brushPrev.material.mainTexture = m_Preview;
            if (_BrushMask == brushTex && size == _BrushTexSize && _BrushTexStrength != null)
                return true;
            if (brushTex != null)
            {
                float num = (float)size;
                _brushSize = size;
                _BrushTexStrength = new float[_brushSize * _brushSize];
                if (_brushSize > 3)
                {
                    for (int index1 = 0; index1 < _brushSize; ++index1)
                    {
                        for (int index2 = 0; index2 < _brushSize; ++index2)
                            _BrushTexStrength[index1 * _brushSize + index2] = brushTex.GetPixelBilinear(((float)index2 + 0.5f) / num, (float)index1 / num).a;
                    }
                }
                else
                {
                    for (int index = 0; index < _BrushTexStrength.Length; ++index)
                        _BrushTexStrength[index] = 1f;
                }
                UnityEngine.Object.DestroyImmediate((UnityEngine.Object)m_Preview);
                m_Preview = new Texture2D(_brushSize, _brushSize, TextureFormat.RGBA32, false);
                m_Preview.hideFlags = HideFlags.HideAndDontSave;
                m_Preview.wrapMode = TextureWrapMode.Repeat;
                m_Preview.filterMode = UnityEngine.FilterMode.Point;
                Color[] colors = new Color[_brushSize * _brushSize];
                for (int index = 0; index < colors.Length; ++index)
                    colors[index] = new Color(1f, 1f, 1f, _BrushTexStrength[index]);
                m_Preview.SetPixels(0, 0, _brushSize, _brushSize, colors, 0);
                m_Preview.Apply();
                BrushPrev.material.mainTexture = m_Preview;
                _BrushMask = brushTex;
                return true;
            }
            _BrushTexStrength = new float[1];
            _BrushTexStrength[0] = 1f;
            _brushSize = 1;
            return false;
        }

        public static bool LoadMeskTex(Texture2D brushTex, int brushTexSize)
        {
            if (m_Preview != null)
                BrushPrev.material.mainTexture = (Texture)m_Preview;
            if (_BrushMask == brushTex && brushTexSize == _BrushTexSize && _BrushTexStrength != null)
                return _updateBrushTexStrength();
            if (brushTex != null)
            {
                _BrushTexSize = brushTexSize;
                UnityEngine.Object.DestroyImmediate(m_Preview);
                m_Preview = new Texture2D(_BrushTexSize, _BrushTexSize, TextureFormat.RGBA32, false);
                m_Preview.hideFlags = HideFlags.HideAndDontSave;
                m_Preview.wrapMode = TextureWrapMode.Repeat;
                m_Preview.filterMode = UnityEngine.FilterMode.Point;
                Color[] colors = new Color[_BrushTexSize * _BrushTexSize];
                for (int index1 = 0; index1 < _BrushTexSize; ++index1)
                {
                    for (int index2 = 0; index2 < _BrushTexSize; ++index2)
                        colors[index1 * _BrushTexSize + index2] = brushTex.GetPixelBilinear(index1, index2);
                }
                m_Preview.SetPixels(0, 0, _BrushTexSize, _BrushTexSize, colors, 0);
                m_Preview.Apply();
                BrushPrev.material.mainTexture = (Texture)m_Preview;
                _BrushMask = brushTex;
                return _updateBrushTexStrength();
            }
            return _updateBrushTexStrength();
        }

        private static bool _updateBrushTexStrength()
        {
            if (_BrushMask != null)
            {

                _BrushTexStrength = new float[_brushSize * _brushSize];
                if (_brushSize > 3)
                {
                    float num = (float)_BrushTexSize / _brushSize;
                    for (int index1 = 0; index1 < _brushSize; ++index1)
                    {
                        for (int index2 = 0; index2 < _brushSize; ++index2)
                            _BrushTexStrength[index1 * _brushSize + index2] = _BrushMask.GetPixelBilinear(((float)index2 + 0.5f) * num, (float)index1 * num).a;
                    }
                }
                else
                {
                    for (int index = 0; index < _BrushTexStrength.Length; ++index)
                        _BrushTexStrength[index] = 1f;
                }
                return true;
            }
            _BrushTexStrength = new float[1];
            _BrushTexStrength[0] = 1f;
            _BrushTexSize = 1;
            return false;
        }

        //---------------------------------------

        public static float GetStrengthInt(int ix, int iy)
        {
            ix = Mathf.Clamp(ix, 0, _brushSize - 1);
            iy = Mathf.Clamp(iy, 0, _brushSize - 1);
            return _BrushTexStrength[iy * _brushSize + ix];
        }

    }

}


