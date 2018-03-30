using System;
using System.Collections.Generic;
using UnityEngine;

namespace YoukiaUnity.CinemaSystem
{
    public class CinemaObject : MonoBehaviour, ICinemaObjectGizmo
    {

        [NonSerialized]
        public GameObject LoadedGameObject;

        /// <summary>
        /// 记录预关联角色模式下关联的角色预制体路径（主要用于查错）
        /// </summary>
        [SerializeField]//[HideInInspector]
        private string _ObjectLoadPath;
        public string ObjectLoadPath
        {
            get { return _ObjectLoadPath; }
        }

        [NonSerialized]
        public string name = "NoInstall";

        /// <summary>
        /// 缩放值
        /// </summary>
        public float ObjectScale = 1f;

        public void Reset()
        {
            LoadedGameObject = null;
        }

        private void Update()
        {
            if (LoadedGameObject)
            {
                LoadedGameObject.transform.localScale = new Vector3(ObjectScale, ObjectScale, ObjectScale);
            }
        }

    }
}
