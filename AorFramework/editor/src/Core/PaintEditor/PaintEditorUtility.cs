using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.editor
{
    /// <summary>
    /// 这里的静态方法 基本是从 UnityEditor.TerrainInspector 里搬过来的GUI绘制封装方法
    /// </summary>
    public class PaintEditorUtility
    {
        public class Styles
        {
            public GUIStyle gridList = (GUIStyle)"GridList";
            public GUIStyle gridListText = (GUIStyle)"GridListText";
            public GUIStyle label = (GUIStyle)"RightLabel";
            public GUIStyle largeSquare = (GUIStyle)"Button";
            public GUIStyle command = (GUIStyle)"Command";
            public Texture settingsIcon = EditorGUIUtility.IconContent("SettingsIcon").image;
            public GUIContent[] toolIcons = new GUIContent[7]
            {
                EditorGUIUtility.IconContent("TerrainInspector.TerrainToolRaise", "|Raise and lower the terrain height."),
                EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSetHeight", "|Set the terrain height."),
                EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSmoothHeight", "|Smooth the terrain height."),
                EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSplat", "|Paint the terrain texture."),
                EditorGUIUtility.IconContent("TerrainInspector.TerrainToolTrees", "|Place trees"),
                EditorGUIUtility.IconContent("TerrainInspector.TerrainToolPlants", "|Place plants, stones and other small foilage"),
                EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSettings", "|Settings for the terrain")
            };
            public GUIContent[] toolNames = new GUIContent[7]
            {
                new GUIContent("Raise / Lower Terrain|Click to raise. Hold down shift to lower."),
                new GUIContent("Paint Height|Hold shift to sample target height."),
                new GUIContent("Smooth Height"),
                new GUIContent("Paint Texture|Select a texture below, then click to paint"),
                new GUIContent("Place Trees|Hold down shift to erase trees.\nHold down ctrl to erase the selected tree type."),
                new GUIContent("Paint Details|Hold down shift to erase.\nHold down ctrl to erase the selected detail type."),
                new GUIContent("Terrain Settings")
            };
            public GUIContent brushSize = new GUIContent("Brush Size|Size of the brush used to paint");
            public GUIContent opacity = new GUIContent("Opacity|Strength of the applied effect");
            public GUIContent settings = new GUIContent("Settings");
            public GUIContent brushes = new GUIContent("Brushes");
//            public GUIContent mismatchedTerrainData = EditorGUIUtility.TextContentWithIcon("The TerrainData used by the TerrainCollider component is different from this terrain. Would you like to assign the same TerrainData to the TerrainCollider component?", "console.warnicon");
            public GUIContent mismatchedTerrainData = new GUIContent("The TerrainData used by the TerrainCollider component is different from this terrain. Would you like to assign the same TerrainData to the TerrainCollider component?", "console.warnicon");
            public GUIContent assign = new GUIContent("Assign");
            public GUIContent textures = new GUIContent("Textures");
            public GUIContent editTextures = new GUIContent("Edit Textures...");
            public GUIContent trees = new GUIContent("Trees");
            public GUIContent noTrees = new GUIContent("No Trees defined|Use edit button below to add new tree types.");
            public GUIContent editTrees = new GUIContent("Edit Trees...|Add/remove tree types.");
            public GUIContent treeDensity = new GUIContent("Tree Density|How dense trees are you painting");
            public GUIContent treeHeight = new GUIContent("Tree Height|Height of the planted trees");
            public GUIContent treeHeightRandomLabel = new GUIContent("Random?|Enable random variation in tree height (variation)");
            public GUIContent treeHeightRandomToggle = new GUIContent("|Enable random variation in tree height (variation)");
            public GUIContent lockWidth = new GUIContent("Lock Width to Height|Let the tree width be the same with height");
            public GUIContent treeWidth = new GUIContent("Tree Width|Width of the planted trees");
            public GUIContent treeWidthRandomLabel = new GUIContent("Random?|Enable random variation in tree width (variation)");
            public GUIContent treeWidthRandomToggle = new GUIContent("|Enable random variation in tree width (variation)");
            public GUIContent treeColorVar = new GUIContent("Color Variation|Amount of random shading applied to trees");
            public GUIContent treeRotation = new GUIContent("Random Tree Rotation|Enable?");
            public GUIContent massPlaceTrees = new GUIContent("Mass Place Trees");
            public GUIContent details = new GUIContent("Details");
            public GUIContent editDetails = new GUIContent("Edit Details...|Add/remove detail meshes");
            public GUIContent detailTargetStrength = new GUIContent("Target Strength|Target amount");
            public GUIContent heightmap = new GUIContent("Heightmap");
            public GUIContent importRaw = new GUIContent("Import Raw...");
            public GUIContent exportRaw = new GUIContent("Export Raw...");
            public GUIContent flatten = new GUIContent("Flatten");
            public GUIContent overrideSmoothness = new GUIContent("Override Smoothness|If checked, the smoothness value specified below will be used for all splat layers, otherwise smoothness of each individual splat layer will be controlled by the alpha channel of the splat texture.");
            public GUIContent bakeLightProbesForTrees = new GUIContent("Bake Light Probes For Trees|If the option is enabled, Unity will create internal light probes at the position of each tree (these probes are internal and will not affect other renderers in the scene) and apply them to tree renderers for lighting. Otherwise trees are still affected by LightProbeGroups. The option is only effective for trees that have LightProbe enabled on their prototype prefab.");
            public GUIContent resolution = new GUIContent("Resolution");
            public GUIContent refresh = new GUIContent("Refresh");
        }

        private static Styles m_styles;
        public static Styles styles
        {
            get
            {
                if(m_styles == null) m_styles = new Styles();
                return m_styles;
            }
        }

        public static int AspectSelectionGrid(int selected, Texture[] textures, int approxSize, GUIStyle style, string emptyString, out bool doubleClick)
        {
            GUILayout.BeginVertical((GUIStyle)"box", GUILayout.MinHeight(10f));
            int num1 = 0;
            doubleClick = false;
            if (textures.Length != 0)
            {
                float num2 = (EditorGUIUtility.currentViewWidth - 20f) / (float)approxSize;
                int num3 = (int)Mathf.Ceil((float)textures.Length / num2);
                Rect aspectRect = GUILayoutUtility.GetAspectRect(num2 / (float)num3);
                Event current = Event.current;
                if (current.type == EventType.MouseDown && current.clickCount == 2 && aspectRect.Contains(current.mousePosition))
                {
                    doubleClick = true;
                    current.Use();
                }
                num1 = GUI.SelectionGrid(aspectRect, Math.Min(selected, textures.Length - 1), textures, Mathf.RoundToInt(EditorGUIUtility.currentViewWidth - 20f) / approxSize, style);
            }
            else
                GUILayout.Label(emptyString);
            GUILayout.EndVertical();
            return num1;
        }

        private static Rect GetBrushAspectRect(int elementCount, int approxSize, int extraLineHeight, out int xCount)
        {
            xCount = (int)Mathf.Ceil((EditorGUIUtility.currentViewWidth - 20f) / (float)approxSize);
            int num = elementCount / xCount;
            if (elementCount % xCount != 0)
                ++num;
            Rect aspectRect = GUILayoutUtility.GetAspectRect((float)xCount / (float)num);
            Rect rect = GUILayoutUtility.GetRect(10f, (float)(extraLineHeight * num));
            aspectRect.height += rect.height;
            return aspectRect;
        }

        public static int AspectSelectionGridImageAndText(int selected, GUIContent[] textures, int approxSize, GUIStyle style, string emptyString, out bool doubleClick)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MinHeight(10f));
            int num = 0;
            doubleClick = false;
            if (textures.Length != 0)
            {
                int xCount = 0;
                Rect brushAspectRect = GetBrushAspectRect(textures.Length, approxSize, 12, out xCount);
                Event current = Event.current;
                if (current.type == EventType.MouseDown && current.clickCount == 2 && brushAspectRect.Contains(current.mousePosition))
                {
                    doubleClick = true;
                    current.Use();
                }
                num = GUI.SelectionGrid(brushAspectRect, Math.Min(selected, textures.Length - 1), textures, xCount, style);
            }
            else
                GUILayout.Label(emptyString);
            GUILayout.EndVertical();
            return num;
        }

    }
}
