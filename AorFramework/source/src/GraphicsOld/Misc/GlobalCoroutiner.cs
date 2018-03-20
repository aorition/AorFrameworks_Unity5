using System;
using System.Collections.Generic;
using UnityEngine;

namespace YoukiaUnity
{
    public class GlobalCoroutiner : MonoBehaviour
    {
        public const string NameDefine = "GLCoroutiner";
        private static GlobalCoroutiner _coroutiner;
        public static GlobalCoroutiner ins
        {
            get
            {
                if (!_coroutiner)
                {
                    GameObject glcGo = GameObject.Find(GlobalCoroutiner.NameDefine);
                    if (!glcGo)
                    {
                        glcGo = new GameObject(GlobalCoroutiner.NameDefine);
                    }
                    _coroutiner = glcGo.GetComponent<GlobalCoroutiner>();
                    if (!_coroutiner)
                    {
                        _coroutiner = glcGo.AddComponent<GlobalCoroutiner>();
                    }
                }
                return _coroutiner;
            }
        }
    }
}
