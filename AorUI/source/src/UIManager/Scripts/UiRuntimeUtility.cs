using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public class UiRuntimeUtility
    {
        //--------------------------------------------------------- 基础类prefab

        /// <summary>
        /// 创建基础UIGameObject
        /// </summary>
        public static RectTransform CreateUI_base(   string name = "UIBase", Transform parent = null,
                                                        float x = 0, float y = 0, float w = 0, float h = 0,
                                                        float anchorsMinX = 0, float anchorsMinY = 0,
                                                        float anchorsMaxX = 1f, float anchorsMaxY = 1f,
                                                        float pivotX = 0.5f, float pivotY = 0.5f)
        {
            GameObject go = new GameObject(name);
            go.layer = 5; // LayerMask.UI

            if (parent != null)
            {
                go.transform.SetParent(parent, false);
            }

            RectTransform rt = go.AddComponent<RectTransform>();

            rt.pivot = new Vector2(pivotX, pivotY);
            rt.anchorMin = new Vector2(anchorsMinX, anchorsMinY);
            rt.anchorMax = new Vector2(anchorsMaxX, anchorsMaxY);

            rt.localPosition = new Vector3(x, y);
            rt.anchoredPosition = new Vector2(x, y);
            rt.sizeDelta = new Vector2(w, h);

            return rt;
        }
        //// <summary>
        ///  创建一个Text Prefab;
        /// 包含组件[Text]
        /// </summary>
        public static Text CreateUI_Text(           string name = "Text", Transform parent = null,
                                                        float x = 0, float y = 0, float w = 0, float h = 0,
                                                        float anchorsMinX = 0, float anchorsMinY = 0,
                                                        float anchorsMaxX = 1f, float anchorsMaxY = 1f,
                                                        float pivotX = 0.5f, float pivotY = 0.5f)
        {
            Text txt = CreateUI_base(name, parent, x, y, w, h, anchorsMinX, anchorsMinY, anchorsMaxX, anchorsMaxY, pivotX, pivotY)
                .gameObject
                .AddComponent<Text>();
            return txt;
        }
        //// <summary>
        ///  创建一个Image Prefab;
        /// 包含组件[Image]
        /// </summary>
        public static Image CreateUI_Image(         string name = "Image", Transform parent = null,
                                                        float x = 0, float y = 0, float w = 0, float h = 0,
                                                        float anchorsMinX = 0, float anchorsMinY = 0,
                                                        float anchorsMaxX = 1f, float anchorsMaxY = 1f,
                                                        float pivotX = 0.5f, float pivotY = 0.5f)
        {
            Image img = CreateUI_base(name, parent, x, y, w, h, anchorsMinX, anchorsMinY, anchorsMaxX, anchorsMaxY, pivotX, pivotY)
                .gameObject
                .AddComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0.65f);
            return img;
        }
    }
}
