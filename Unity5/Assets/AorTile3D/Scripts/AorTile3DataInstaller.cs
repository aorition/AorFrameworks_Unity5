using System;
using System.Collections.Generic;
using UnityEngine;

namespace AorFramework.AorTile3D.runtime
{
    public class AorTile3DataInstaller : MonoBehaviour
    {

        [Tooltip("地图数据Info(TXT格式,使用Info文件后<相机初始位置><刷新边界设置>设置将被Info数据替代)")]
        [SerializeField]
        private TextAsset _linkdTextInfoData;
        public TextAsset linkdTextInfoData
        {
            get { return _linkdTextInfoData; }
        }

        [Tooltip("相机初始位置")]
        [SerializeField]
        private Vector3 _definedBorderCenter = Vector3.zero;
        public int[] definedBorderCenter
        {
            get { return new [] {(int)_definedBorderCenter.x, (int)_definedBorderCenter.y, (int)_definedBorderCenter.z}; }
        }

        [Tooltip("刷新边界设置")]
        [SerializeField] private Vector3 _definedBorderHalfSize = new Vector3(10, 10, 5);
        public int[] definedBorderHalfSize
        {
            get { return new[] { (int)_definedBorderHalfSize.x, (int)_definedBorderHalfSize.y, (int)_definedBorderHalfSize.z }; }
        }
        
        [Tooltip("地图数据(TXT格式)")]
        [SerializeField]
        private TextAsset _linkdTextData;
        public TextAsset linkdTextData
        {
            get { return _linkdTextData; }
        }

        [Tooltip("数据装载后自动移除该Installer")]
        [SerializeField]
        private bool _AutoDestroyOnDataLoaded = false;
        public bool AutoDestroyOnDataLoaded
        {
            get { return _AutoDestroyOnDataLoaded; }
        }
    }
}
