using System;
using System.Collections.Generic;

using UnityEngine;
using YoukiaCore;
using Object = UnityEngine.Object;

namespace YoukiaUnity.View
{

    /// <summary>
    /// 物体上的挂载点，用以快速定位物体内的子物体
    /// </summary>
    public class PivotPointData : MonoBehaviour
    {

        public static void CreatePivotPointData(GameObject gameObject, bool tryFindPoint = true)
        {
            PivotPointData ppd = gameObject.AddComponent<PivotPointData>();
            if (tryFindPoint)
            {
                ppd.TryFindDefaultPoints();
            }
        }

        [SerializeField]
        protected List<string> _pivotNameList = new List<string>()
        {
            "hit",
            "body",
            "material",
            "face",
            "hurt",
            "namePoint",
            "standby1",
            "standby2",
            "standby3",
            "standby4"
        };

        public List<string> pivotNameList
        {
            get { return _pivotNameList; }
        }

        [SerializeField]
        protected List<UnityEngine.Object> _dataList;

        //尝试查找 默认约定的 点位/物件 并记录
        public void TryFindDefaultPoints()
        {
            _dataList = new List<Object>();

            int i, len;
            Transform FbxRoot = null;
            len = transform.childCount;
            for (i = 0; i < len; i++)
            {
                if (transform.GetChild(i).name.Contains("_root"))
                {
                    FbxRoot = transform.GetChild(i);
                    break;
                }
            }
            if (FbxRoot == null) return;

            //找 hit
            Transform Bip001Spine1 = FbxRoot.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1");
            if (Bip001Spine1 != null)
            {
                _dataList.Add(Bip001Spine1.gameObject);
            }
            else
            {
                _dataList.Add(null);
            }

            //找 body
            Transform body0 = null;
            len = FbxRoot.childCount;
            for (i = 0; i < len; i++)
            {
                if (FbxRoot.GetChild(i).name.Contains("_body_0"))
                {
                    body0 = FbxRoot.GetChild(i);
                    break;
                }
            }
            if (body0 != null)
            {
                _dataList.Add(body0.gameObject);
            }
            else
            {
                _dataList.Add(null);
            }

            //找 Material
            if (body0 != null)
            {
                Renderer renderer = body0.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material mt = renderer.sharedMaterial;
                    if (mt != null)
                    {
                        _dataList.Add(mt);
                    }
                    else
                    {
                        _dataList.Add(null);
                    }
                }
                else
                {
                    _dataList.Add(null);
                }
            }
            else
            {
                _dataList.Add(null);
            }

            //找 face
            Transform face0 = null;
            len = FbxRoot.childCount;
            for (i = 0; i < len; i++)
            {
                if (FbxRoot.GetChild(i).name.Contains("_face_0"))
                {
                    face0 = FbxRoot.GetChild(i);
                    break;
                }
            }
            if (face0 != null)
            {
                _dataList.Add(face0.gameObject);
            }
            else
            {
                _dataList.Add(null);
            }

            //hurt
            Transform hurt0 = transform.FindChild("namePoint/namePoint");
            if (hurt0 != null)
            {
                _dataList.Add(hurt0.gameObject);
            }
            else
            {
                _dataList.Add(null);
            }

        }

        public UnityEngine.Object GetPivot(int PivotId)
        {
            if (PivotId >= 0 && PivotId < _dataList.Count)
            {
				return _dataList[PivotId];
            }
            else
            {
                Debug.Log("*** PivotPointData.getPivot Error : PivotId overflowed the dataList. PivotId = " + PivotId + " , dataList.Count = " + _dataList.Count);
                return null;
            }
        } 

        public T GetPivot<T>(string pName) where T : class
        {
            if (_pivotNameList == null || _pivotNameList.Count == 0 || string.IsNullOrEmpty(pName) || _dataList == null)
                return null;

            int index = -1;
            for (int i = 0; i < _pivotNameList.Count; i++)
            {
                if (_pivotNameList[i] == pName)
                    index = i;

            }
            if (index == -1)
                return null;


            if (index >= 0 && index < _dataList.Count)
            {
                return _dataList[index] as T;
            }

            return null;
        }

    }

}

