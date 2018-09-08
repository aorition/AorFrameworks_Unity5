using UnityEngine;

namespace Framework.Utility
{

    public class TextureUtility
    {

        private static Texture2D _defaultNormalTex;
        public static Texture2D DefaultNormalTex {
            get {
                if (!_defaultNormalTex) {
                    _defaultNormalTex = new Texture2D(16, 16, TextureFormat.RGB24, false);
                    _defaultNormalTex.name = "DefaultNormal";
                    _defaultNormalTex.wrapMode = TextureWrapMode.Repeat;
                    int len = 256;
                    Color32[] color32s = new Color32[len];
                    for (int i = 0; i < len; i++) {
                        color32s[i] = new Color32(125, 125, 255, 255);
                    }
                    _defaultNormalTex.SetPixels32(color32s);
                }
                return _defaultNormalTex;
            }
        }

    }
}