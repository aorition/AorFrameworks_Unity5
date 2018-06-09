using Framework.Graphic;
using UnityEngine;
using UnityEditor;

namespace Framework.Editor
{

    public class PrefabLightmapDataEditor
    {
        // 把renderer上面的lightmap信息保存起来，以便存储到prefab上面  
        [MenuItem("FrameworkTools/Lightmap工具/导出到对象", false)]
        public static void SaveLightmapInfo()
        {

            GameObject[] selects = Selection.gameObjects;

            if (selects != null && selects.Length > 0)
            {

                int i, len = selects.Length;
                for (i = 0; i < len; i++)
                {

                    PrefabLightmapData data = selects[i].GetComponent<PrefabLightmapData>();
                    if (data == null)
                    {
                        data = selects[i].AddComponent<PrefabLightmapData>();
                    }
                    data.SaveLightmap();
                    EditorUtility.SetDirty(selects[i]);
                }

            }

        }

        // 把保存的lightmap信息恢复到renderer上面  
        [MenuItem("FrameworkTools/Lightmap工具/从对象上恢复Lightmap信息", false)]
        public static void LoadLightmapInfo()
        {

            GameObject[] selects = Selection.gameObjects;

            if (selects != null && selects.Length > 0)
            {

                int i, len = selects.Length;
                for (i = 0; i < len; i++)
                {

                    PrefabLightmapData data = selects[i].GetComponent<PrefabLightmapData>();
                    if (data)
                    {
                        data.LoadLightmap();
                        EditorUtility.SetDirty(selects[i]);
                    }

                }

            }
        }
    }
}
