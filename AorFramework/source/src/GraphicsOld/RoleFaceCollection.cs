using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using YoukiaCore;

namespace YoukiaUnity.Graphics
{
    public class RoleFaceCollection : MonoBehaviour
    {

        /// <summary>
        /// CinemaClip 卸载接口
        /// </summary>
        public static void UnloadFacePrefab(GameObject prefab)
        {
            YKBridgeUtils.UnloadPrefab(prefab);
        }

        [SerializeField]
        private string _SetFaceNameWithStart;
        private void Start()
        {
            if (!string.IsNullOrEmpty(_SetFaceNameWithStart))
            {
                string label = _SetFaceNameWithStart;
                SetRoleFace(label);
                _SetFaceNameWithStart = String.Empty;
            }
        }
        
        public static void LoadFacePrefab(string path, Action<GameObject> loadedCallback, params object[] param)
        {
            YKBridgeUtils.LoadPrefab(path, loadedCallback, param);
        }

        [SerializeField]
        private Transform _faceRootBone;
        public Transform FaceRootBoneTransform
        {
            get { return _faceRootBone; }
        }

        [SerializeField]
        private string[] _faceLabels;
        public string[] GetFaceLabels()
        {
            return _faceLabels;
        }

        [SerializeField]
        private string[] _faceLoadPaths;
        public string[] GetFaceLoadPaths()
        {
            return _faceLoadPaths;
        }

        [SerializeField]
        private Transform _bodyT;
        public Transform BindingBodyTransform
        {
            get { return _bodyT; }
        }

        [SerializeField]
        private bool _isUIRole;
        public bool isUIRole
        {
            get { return _isUIRole; }
        }

        /// <summary>
        /// 获取face load路径
        /// </summary>
        public string GetFaceLoadPath(string label)
        {
            string o = string.Empty;
            for (int i = 0; i < _faceLabels.Length; i++)
            {
                if (label == _faceLabels[i])
                {
                    o = _faceLoadPaths[i];
                    break;
                }
            }
            return o;
        }

        public void ResetRoleFace()
        {
            FaceChangeController fcc = gameObject.GetComponent<FaceChangeController>();
            if (fcc)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(fcc);
                }
                else
                {
                    GameObject.DestroyImmediate(fcc);
                }
            }
            _currentFaceLabel = string.Empty;
        }

        private string _currentFaceLabel;
        public string CurrentFaceLabel
        {
            get { return _currentFaceLabel; }
        }

        public void SetRoleFace(string label)
        {

            if (label == _currentFaceLabel)
            {
                Log.warning("RoleFaceCollection.SetRoleFace warning :: Set Same Face label : " + label + ", this action must be Ignore.");
            }

            string path = GetFaceLoadPath(label);
            if (!string.IsNullOrEmpty(path))
            {
                LoadFacePrefab(path, (face) =>
                {

                    if (!face)
                    {
                        Log.error("*** RoleFaceCollection.SetRoleFace Error :: can not find " + label + " to load. (path = " + path +")");
                        return;
                    }

                    face.transform.SetParent(_bodyT, false);

                    //初始化
                    //SkinnedMeshRenderer rsmr = _bodyT.GetComponent<SkinnedMeshRenderer>();
                    SkinnedMeshRenderer smr = face.GetComponent<SkinnedMeshRenderer>();
                    
                    smr.rootBone = _faceRootBone;
                    //smr.bones = rsmr.bones;
                    smr.bones = new Transform[] { _faceRootBone };
                    smr.updateWhenOffscreen = true;

                    Material material = _bodyT.GetComponent<Renderer>().material;

                    if (material && face)
                    {
                        FaceChangeController fcc = gameObject.GetComponent<FaceChangeController>();
                        if (!fcc) fcc = gameObject.AddComponent<FaceChangeController>();
                        if (!fcc.enabled) fcc.enabled = true;
                        fcc.Setup(material, face.transform, _isUIRole);
                        _currentFaceLabel = label;
                    }
                    else
                    {
                        //移除
                        ResetRoleFace();
                    }
                });
            }
        }

    }
}
