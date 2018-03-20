using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AorBaseUtility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AorFrameworks.Tools
{
    public class TransformRecorder : MonoBehaviour
    {

        public const string DEFAULTSAVEDIR = "TransformRecorderDatas";

        public bool UseFixedUpdate = false;

        [SerializeField] private string _savePath;

        private void Awake()
        {
            if (string.IsNullOrEmpty(_savePath))
            {
                _createSavePath();
            }
        }

        private Vector3 _posCache;
        private Vector3 _scaleCache;
        private Quaternion _rotateCache;

        private void OnEnable()
        {
            CacheUpdate();
        }

        private void OnDisable()
        {
            CacheUpdate();
        }

        private void _createSavePath()
        {
            string saveFileName = SceneManager.GetActiveScene().name + "_" + gameObject.getHierarchyPath().Replace("/", "_") + gameObject.name;
            _savePath = Application.dataPath.Replace("Assets","") + DEFAULTSAVEDIR + "/" + saveFileName + ".txt";
        }

        private void FixedUpdate()
        {
            if (!UseFixedUpdate) return;

            if (isDirty())
            {
                CacheUpdate();
            }
        }

        private void Update()
        {
            if (UseFixedUpdate) return;

            if (isDirty())
            {
                CacheUpdate();
            }
        }

        private void CacheUpdate()
        {
            _scaleCache = transform.localScale;
            _rotateCache = transform.localRotation;
            _posCache = transform.localPosition;

            SaveToFile();
        }

        private bool isDirty()
        {
            return (transform.localScale != _scaleCache || transform.localRotation != _rotateCache || transform.localPosition != _posCache);
        }

        public void SaveToFile()
        {
            string newFlieData = _scaleCache.x + "|" + _scaleCache.y + "|" + _scaleCache.z + "|" + _rotateCache.x + "|" +
                                 _rotateCache.y + "|" + _rotateCache.z + "|" + _rotateCache.w + "|" + _posCache.x + "|" +
                                 _posCache.y + "|" + _posCache.z;
            AorIO.SaveStringToFile(_savePath, newFlieData);
        }

        public void LoadFromFile()
        {
            if (string.IsNullOrEmpty(_savePath))
            {
                _createSavePath();
            }

            string newFlieData = AorIO.ReadStringFormFile(_savePath);

            if (string.IsNullOrEmpty(newFlieData))
            {
                //Error
                return;
            }

            string[] dataSP = newFlieData.Split('|');

            transform.localScale = new Vector3(float.Parse(dataSP[0]), float.Parse(dataSP[1]), float.Parse(dataSP[2]));
            transform.localRotation = new Quaternion(float.Parse(dataSP[3]), float.Parse(dataSP[4]), float.Parse(dataSP[5]), float.Parse(dataSP[6]));
            transform.localPosition = new Vector3(float.Parse(dataSP[7]), float.Parse(dataSP[8]), float.Parse(dataSP[9]));

        }

        public void ClearCacheFiles()
        {

            string cdir = Application.dataPath.Replace("Assets", "") + DEFAULTSAVEDIR;
            if (Directory.Exists(cdir))
            {
                Directory.Delete(cdir, true);
            }

        }

    }
}
