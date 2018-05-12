using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.editor
{

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

        public static PaintHandle PaintPrev = PaintHandle.Hide_preview;
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
                _brushPrev.orthographicSize = (_brushSize * CurrentSelect.localScale.x) * (MeshSize.x / 100);
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
                    _updateBrushTexStrength();
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

                    _brushPrev.material = material;
                    _brushPrev.nearClipPlane = -20f;
                    _brushPrev.farClipPlane = 20f;
                    __updateBrushPrevOrthographicSize();
                    _brushPrev.ignoreLayers = ~LayerMask;
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
            get { return _BrushMask;}
        }

        private static Texture2D m_Preview;

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
                    float num = (float)_brushSize / _BrushMask.width;
                    for (int index1 = 0; index1 < _brushSize; ++index1)
                    {
                        for (int index2 = 0; index2 < _brushSize; ++index2)
                            _BrushTexStrength[index1 * _brushSize + index2] = _BrushMask.GetPixelBilinear(((float)index2 + 0.5f) / num, (float)index1 / num).a;
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

        public static float GetStrengthInt(int ix, int iy)
        {
            ix = Mathf.Clamp(ix, 0, _brushSize - 1);
            iy = Mathf.Clamp(iy, 0, _brushSize - 1);
            return _BrushTexStrength[iy * _brushSize + ix];
        }

    }

    public enum TerrainTool
    {
        None = -1,
        PaintHeight = 0,
        SetHeight = 1,
        SmoothHeight = 2,
        PaintTexture = 3,
        PlaceTree = 4,
        PaintDetail = 5,
        TerrainSettings = 6,
        TerrainToolCount = 7,
    }

    public class HeightmapPainter
    {
        public int size;
        public float strength;
        public float targetHeight;
        public TerrainTool tool;
        public TerrainData terrainData;

        private float Smooth(int x, int y)
        {
            float num1 = 0.0f;
            float num2 = 1f / this.terrainData.size.y;
            return (num1 + this.terrainData.GetHeight(x, y) * num2 + this.terrainData.GetHeight(x + 1, y) * num2 + this.terrainData.GetHeight(x - 1, y) * num2 + (float)((double)this.terrainData.GetHeight(x + 1, y + 1) * (double)num2 * 0.75) + (float)((double)this.terrainData.GetHeight(x - 1, y + 1) * (double)num2 * 0.75) + (float)((double)this.terrainData.GetHeight(x + 1, y - 1) * (double)num2 * 0.75) + (float)((double)this.terrainData.GetHeight(x - 1, y - 1) * (double)num2 * 0.75) + this.terrainData.GetHeight(x, y + 1) * num2 + this.terrainData.GetHeight(x, y - 1) * num2) / 8f;
        }

        private float ApplyBrush(float height, float brushStrength, int x, int y)
        {
            if (this.tool == TerrainTool.PaintHeight)
                return height + brushStrength;
            if (this.tool == TerrainTool.SetHeight)
            {
                if ((double)this.targetHeight > (double)height)
                {
                    height += brushStrength;
                    height = Mathf.Min(height, this.targetHeight);
                    return height;
                }
                height -= brushStrength;
                height = Mathf.Max(height, this.targetHeight);
                return height;
            }
            if (this.tool == TerrainTool.SmoothHeight)
                return Mathf.Lerp(height, this.Smooth(x, y), brushStrength);
            return height;
        }

        public void PaintHeight(float xCenterNormalized, float yCenterNormalized)
        {
            int num1;
            int num2;
            if (this.size % 2 == 0)
            {
                num1 = Mathf.CeilToInt(xCenterNormalized * (float)(this.terrainData.heightmapWidth - 1));
                num2 = Mathf.CeilToInt(yCenterNormalized * (float)(this.terrainData.heightmapHeight - 1));
            }
            else
            {
                num1 = Mathf.RoundToInt(xCenterNormalized * (float)(this.terrainData.heightmapWidth - 1));
                num2 = Mathf.RoundToInt(yCenterNormalized * (float)(this.terrainData.heightmapHeight - 1));
            }
            int num3 = this.size / 2;
            int num4 = this.size % 2;
            int xBase = Mathf.Clamp(num1 - num3, 0, this.terrainData.heightmapWidth - 1);
            int yBase = Mathf.Clamp(num2 - num3, 0, this.terrainData.heightmapHeight - 1);
            int num5 = Mathf.Clamp(num1 + num3 + num4, 0, this.terrainData.heightmapWidth);
            int num6 = Mathf.Clamp(num2 + num3 + num4, 0, this.terrainData.heightmapHeight);
            int width = num5 - xBase;
            int height = num6 - yBase;
            float[,] heights = this.terrainData.GetHeights(xBase, yBase, width, height);
            for (int index1 = 0; index1 < height; ++index1)
            {
                for (int index2 = 0; index2 < width; ++index2)
                {
                    float strengthInt = EditPaintBrush.GetStrengthInt(xBase + index2 - (num1 - num3), yBase + index1 - (num2 - num3));
                    float num7 = this.ApplyBrush(heights[index1, index2], strengthInt * this.strength, index2 + xBase, index1 + yBase);
                    heights[index1, index2] = num7;
                }
            }
            this.terrainData.SetHeightsDelayLOD(xBase, yBase, heights);
        }

    }

    public class SplatPainter
    {
        public int size;
        public float strength;
        public float target;
        public TerrainData terrainData;
        public TerrainTool tool;

        private float ApplyBrush(float height, float brushStrength)
        {
            if ((double)this.target > (double)height)
            {
                height += brushStrength;
                height = Mathf.Min(height, this.target);
                return height;
            }
            height -= brushStrength;
            height = Mathf.Max(height, this.target);
            return height;
        }

        private void Normalize(int x, int y, int splatIndex, float[,,] alphamap)
        {
            float num1 = alphamap[y, x, splatIndex];
            float num2 = 0.0f;
            int length = alphamap.GetLength(2);
            for (int index = 0; index < length; ++index)
            {
                if (index != splatIndex)
                    num2 += alphamap[y, x, index];
            }
            if ((double)num2 > 0.01)
            {
                float num3 = (1f - num1) / num2;
                for (int index = 0; index < length; ++index)
                {
                    if (index != splatIndex)
                        alphamap[y, x, index] *= num3;
                }
            }
            else
            {
                for (int index = 0; index < length; ++index)
                    alphamap[y, x, index] = index != splatIndex ? 0.0f : 1f;
            }
        }

        public void Paint(float xCenterNormalized, float yCenterNormalized, int splatIndex)
        {
            if (splatIndex >= this.terrainData.alphamapLayers)
                return;
            int num1 = Mathf.FloorToInt(xCenterNormalized * (float)this.terrainData.alphamapWidth);
            int num2 = Mathf.FloorToInt(yCenterNormalized * (float)this.terrainData.alphamapHeight);
            int num3 = Mathf.RoundToInt((float)this.size) / 2;
            int num4 = Mathf.RoundToInt((float)this.size) % 2;
            int x1 = Mathf.Clamp(num1 - num3, 0, this.terrainData.alphamapWidth - 1);
            int y1 = Mathf.Clamp(num2 - num3, 0, this.terrainData.alphamapHeight - 1);
            int num5 = Mathf.Clamp(num1 + num3 + num4, 0, this.terrainData.alphamapWidth);
            int num6 = Mathf.Clamp(num2 + num3 + num4, 0, this.terrainData.alphamapHeight);
            int width = num5 - x1;
            int height = num6 - y1;
            float[,,] alphamaps = this.terrainData.GetAlphamaps(x1, y1, width, height);
            for (int y2 = 0; y2 < height; ++y2)
            {
                for (int x2 = 0; x2 < width; ++x2)
                {
                    float strengthInt = EditPaintBrush.GetStrengthInt(x1 + x2 - (num1 - num3 + num4), y1 + y2 - (num2 - num3 + num4));
                    float num7 = this.ApplyBrush(alphamaps[y2, x2, splatIndex], strengthInt * this.strength);
                    alphamaps[y2, x2, splatIndex] = num7;
                    this.Normalize(x2, y2, splatIndex, alphamaps);
                }
            }
            this.terrainData.SetAlphamaps(x1, y1, alphamaps);
        }
    }

    public class DetailPainter
    {
        public int size;
        public float opacity;
        public float targetStrength;
        public TerrainData terrainData;
        public TerrainTool tool;
        public bool randomizeDetails;
        public bool clearSelectedOnly;

        public void Paint(float xCenterNormalized, float yCenterNormalized, int detailIndex)
        {
            if (detailIndex >= this.terrainData.detailPrototypes.Length)
                return;
            int num1 = Mathf.FloorToInt(xCenterNormalized * (float)this.terrainData.detailWidth);
            int num2 = Mathf.FloorToInt(yCenterNormalized * (float)this.terrainData.detailHeight);
            int num3 = Mathf.RoundToInt((float)this.size) / 2;
            int num4 = Mathf.RoundToInt((float)this.size) % 2;
            int xBase = Mathf.Clamp(num1 - num3, 0, this.terrainData.detailWidth - 1);
            int yBase = Mathf.Clamp(num2 - num3, 0, this.terrainData.detailHeight - 1);
            int num5 = Mathf.Clamp(num1 + num3 + num4, 0, this.terrainData.detailWidth);
            int num6 = Mathf.Clamp(num2 + num3 + num4, 0, this.terrainData.detailHeight);
            int num7 = num5 - xBase;
            int num8 = num6 - yBase;
            int[] numArray = new int[1]
            {
        detailIndex
            };
            if ((double)this.targetStrength < 0.0 && !this.clearSelectedOnly)
                numArray = this.terrainData.GetSupportedLayers(xBase, yBase, num7, num8);
            for (int index1 = 0; index1 < numArray.Length; ++index1)
            {
                int[,] detailLayer = this.terrainData.GetDetailLayer(xBase, yBase, num7, num8, numArray[index1]);
                for (int index2 = 0; index2 < num8; ++index2)
                {
                    for (int index3 = 0; index3 < num7; ++index3)
                    {
                        float t = this.opacity * EditPaintBrush.GetStrengthInt(xBase + index3 - (num1 - num3 + num4), yBase + index2 - (num2 - num3 + num4));
                        float b = this.targetStrength;
                        float num9 = Mathf.Lerp((float)detailLayer[index2, index3], b, t);
                        detailLayer[index2, index3] = Mathf.RoundToInt(num9 - 0.5f + UnityEngine.Random.value);
                    }
                }
                this.terrainData.SetDetailLayer(xBase, yBase, numArray[index1], detailLayer);
            }
        }
    }

    #region 暂时放弃移入

    /*
    (暂时放弃移入)
    public class TreePainter
    {
        public static float brushSize = 40f;
        public static float spacing = 0.8f;
        public static bool lockWidthToHeight = true;
        public static bool randomRotation = true;
        public static bool allowHeightVar = true;
        public static bool allowWidthVar = true;
        public static float treeColorAdjustment = 0.4f;
        public static float treeHeight = 1f;
        public static float treeHeightVariation = 0.1f;
        public static float treeWidth = 1f;
        public static float treeWidthVariation = 0.1f;
        public static int selectedTree = -1;

        private static Color GetTreeColor()
        {
            Color color = Color.white * UnityEngine.Random.Range(1f, 1f - TreePainter.treeColorAdjustment);
            color.a = 1f;
            return color;
        }

        private static float GetTreeHeight()
        {
            float num = !TreePainter.allowHeightVar ? 0.0f : TreePainter.treeHeightVariation;
            return TreePainter.treeHeight * UnityEngine.Random.Range(1f - num, 1f + num);
        }

        private static float GetTreeWidth()
        {
            float num = !TreePainter.allowWidthVar ? 0.0f : TreePainter.treeWidthVariation;
            return TreePainter.treeWidth * UnityEngine.Random.Range(1f - num, 1f + num);
        }

        private static float GetTreeRotation()
        {
            return !TreePainter.randomRotation ? 0.0f : UnityEngine.Random.Range(0.0f, 6.283185f);
        }

        public static void PlaceTrees(Terrain terrain, float xBase, float yBase)
        {
            int prototypeCount = TerrainInspectorUtil.GetPrototypeCount(terrain.terrainData);
            if (TreePainter.selectedTree == -1 || TreePainter.selectedTree >= prototypeCount || !TerrainInspectorUtil.PrototypeIsRenderable(terrain.terrainData, TreePainter.selectedTree))
                return;
            int num1 = 0;
            TreeInstance instance = new TreeInstance();
            instance.position = new Vector3(xBase, 0.0f, yBase);
            instance.color = (Color32)TreePainter.GetTreeColor();
            instance.lightmapColor = (Color32)Color.white;
            instance.prototypeIndex = TreePainter.selectedTree;
            instance.heightScale = TreePainter.GetTreeHeight();
            instance.widthScale = !TreePainter.lockWidthToHeight ? TreePainter.GetTreeWidth() : instance.heightScale;
            instance.rotation = TreePainter.GetTreeRotation();
            if (Event.current.type != EventType.MouseDrag && (double)TreePainter.brushSize <= 1.0 || TerrainInspectorUtil.CheckTreeDistance(terrain.terrainData, instance.position, instance.prototypeIndex, TreePainter.spacing))
            {
                terrain.AddTreeInstance(instance);
                ++num1;
            }
            Vector3 prototypeExtent = TerrainInspectorUtil.GetPrototypeExtent(terrain.terrainData, TreePainter.selectedTree);
            prototypeExtent.y = 0.0f;
            float num2 = TreePainter.brushSize / (float)((double)prototypeExtent.magnitude * (double)TreePainter.spacing * 0.5);
            int num3 = Mathf.Clamp((int)((double)num2 * (double)num2 * 0.5), 0, 100);
            for (int index = 1; index < num3 && num1 < num3; ++index)
            {
                Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
                insideUnitCircle.x *= TreePainter.brushSize / terrain.terrainData.size.x;
                insideUnitCircle.y *= TreePainter.brushSize / terrain.terrainData.size.z;
                Vector3 position = new Vector3(xBase + insideUnitCircle.x, 0.0f, yBase + insideUnitCircle.y);
                if ((double)position.x >= 0.0 && (double)position.x <= 1.0 && ((double)position.z >= 0.0 && (double)position.z <= 1.0) && TerrainInspectorUtil.CheckTreeDistance(terrain.terrainData, position, TreePainter.selectedTree, TreePainter.spacing * 0.5f))
                {
                    instance = new TreeInstance();
                    instance.position = position;
                    instance.color = (Color32)TreePainter.GetTreeColor();
                    instance.lightmapColor = (Color32)Color.white;
                    instance.prototypeIndex = TreePainter.selectedTree;
                    instance.heightScale = TreePainter.GetTreeHeight();
                    instance.widthScale = !TreePainter.lockWidthToHeight ? TreePainter.GetTreeWidth() : instance.heightScale;
                    instance.rotation = TreePainter.GetTreeRotation();
                    terrain.AddTreeInstance(instance);
                    ++num1;
                }
            }
        }

        public static void RemoveTrees(Terrain terrain, float xBase, float yBase, bool clearSelectedOnly)
        {
            float radius = TreePainter.brushSize / terrain.terrainData.size.x;
            terrain.RemoveTrees(new Vector2(xBase, yBase), radius, !clearSelectedOnly ? -1 : TreePainter.selectedTree);
        }

        public static void MassPlaceTrees(TerrainData terrainData, int numberOfTrees, bool randomTreeColor, bool keepExistingTrees)
        {
            int length = terrainData.treePrototypes.Length;
            if (length == 0)
            {
                Debug.Log((object)"Can't place trees because no prototypes are defined");
            }
            else
            {
                Undo.RegisterCompleteObjectUndo((UnityEngine.Object)terrainData, "Mass Place Trees");
                TreeInstance[] treeInstanceArray1 = new TreeInstance[numberOfTrees];
                int num = 0;
                while (num < treeInstanceArray1.Length)
                {
                    TreeInstance treeInstance = new TreeInstance();
                    treeInstance.position = new Vector3(UnityEngine.Random.value, 0.0f, UnityEngine.Random.value);
                    if ((double)terrainData.GetSteepness(treeInstance.position.x, treeInstance.position.z) < 30.0)
                    {
                        treeInstance.color = (Color32)(!randomTreeColor ? Color.white : TreePainter.GetTreeColor());
                        treeInstance.lightmapColor = (Color32)Color.white;
                        treeInstance.prototypeIndex = UnityEngine.Random.Range(0, length);
                        treeInstance.heightScale = TreePainter.GetTreeHeight();
                        treeInstance.widthScale = !TreePainter.lockWidthToHeight ? TreePainter.GetTreeWidth() : treeInstance.heightScale;
                        treeInstance.rotation = TreePainter.GetTreeRotation();
                        treeInstanceArray1[num++] = treeInstance;
                    }
                }
                if (keepExistingTrees)
                {
                    TreeInstance[] treeInstances = terrainData.treeInstances;
                    TreeInstance[] treeInstanceArray2 = new TreeInstance[treeInstances.Length + treeInstanceArray1.Length];
                    Array.Copy((Array)treeInstances, 0, (Array)treeInstanceArray2, 0, treeInstances.Length);
                    Array.Copy((Array)treeInstanceArray1, 0, (Array)treeInstanceArray2, treeInstances.Length, treeInstanceArray1.Length);
                    treeInstanceArray1 = treeInstanceArray2;
                }
                terrainData.treeInstances = treeInstanceArray1;
                terrainData.RecalculateTreePositions();
            }
        }
    }
    */

    #endregion


}


