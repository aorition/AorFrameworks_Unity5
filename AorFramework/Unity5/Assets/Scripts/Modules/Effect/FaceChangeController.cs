using System;
using System.Collections.Generic;
using UnityEngine;

public class FaceChangeController : MonoBehaviour
{

    [SerializeField]
    private string _OverideFaceName;

    private Transform _OverrideFace;

    private Material _bodyMaterial;
    
    void OnEnable()
    {
        if (_isStarted && _isSetuped)
        {
            init();
        }
    }

    private bool _isSetuped = false;
    /// <summary>
    /// 通过 body Material 和 face节点的Transform 进行初始化
    /// </summary>
    /// <param name="material"></param>
    /// <param name="face"></param>
    public void Setup(Material material, Transform face)
    {
        _bodyMaterial = material;
        _OverrideFace = face;

        _OverideFaceName = face.gameObject.name;

        _isSetuped = true;

        if (_isStarted)
        {
            init();
        }
    }

    /// <summary>
    /// 通过 face节点名称进行初始化
    /// 
    /// *限制： body节点和face节点都必须是本脚本所在节点的子集。
    /// 
    /// </summary>
    /// <param name="faceName"></param>
    public void Setup(string faceName)
    {

        _OverideFaceName = faceName;

        Transform bodyT = FindSkinnedMeshInChildren(transform, "_body_");
        if (bodyT != null)
        {
            _bodyMaterial = bodyT.GetComponent<SkinnedMeshRenderer>().material;
        }

        if (_bodyMaterial == null)
        {
            Debug.Log("*** FaceChangeController.Start Error :: Can not find _bodyMaterial");
            return;
        }

        if (_OverrideFace == null)
        {
            _OverrideFace = FindSkinnedMeshInChildren(transform, _OverideFaceName);

            if (_OverrideFace == null)
            {
                Debug.Log("*** FaceChangeController.Start Error :: Can not find _OverrideFace");
                return;
            }
        }

        _isSetuped = true;

        if (_isStarted)
        {
            init();
        }
    }

    private bool _isStarted = false;
    void Start()
    {
        /// 自动初始化，前提是_OverideFaceName不为空
        if (!_isSetuped && !string.IsNullOrEmpty(_OverideFaceName))
        {
            Setup(_OverideFaceName);
        }

        _isStarted = true;
        init();
    }

    private void init()
    {
        _bodyMaterial.SetFloat("_HideFace", 0.02f);
        if (!_OverrideFace.gameObject.activeInHierarchy)
        {
            _OverrideFace.gameObject.SetActive(true);
        }
    }

    void OnDisable()
    {
        _bodyMaterial.SetFloat("_HideFace", 0.0f);
        if (_OverrideFace.gameObject.activeInHierarchy)
        {
            _OverrideFace.gameObject.SetActive(false);
        }
    }

    private Transform FindSkinnedMeshInChildren(Transform t, string findKey)
    {

        List<SkinnedMeshRenderer> smrs = t.FindComponentListInChildren<SkinnedMeshRenderer>();
        if (smrs != null && smrs.Count > 0)
        {
            int i, len = smrs.Count;
            for (i = 0; i < len; i++)
            {
                if (smrs[i].gameObject.name.Contains(findKey))
                {
                    return smrs[i].transform;
                }
            }
        }
        

        return null;
    }

}
