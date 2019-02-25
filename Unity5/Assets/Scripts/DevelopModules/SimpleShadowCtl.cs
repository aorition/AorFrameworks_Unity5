using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShadowCtl : MonoBehaviour
{

    private static readonly float BaseShadowLen = 0.25f; 

    [Range(0f, 1f)]
    [SerializeField]
    private float _shadowAlpha = 0.5f;
    public float shadowAlpha
    {
        get { return _shadowAlpha; }
        set { _shadowAlpha = Mathf.Clamp01(value); }
    }

    [Range(0f, 1f)]
    [SerializeField]
    private float _shadowLen = 1f;
    public float shadowLen
    {
        get { return _shadowLen; }
        set { _shadowLen = Mathf.Clamp01(value); }
    }

    private bool _isSetuped = false;
    private Material[] _materials;

    private void Awake()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer)
        {
            _materials = new Material[renderer.materials.Length];

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                _materials[i] = new Material(renderer.materials[i]);
                _materials[i].name = renderer.materials[i].name + "_shadow";
            }

            renderer.materials = _materials;

            _isSetuped = true;
        }
    }

    private float _shadowAlphaCache = -1;
    private float _shadowLenCache = -1;
    private void Update()
    {
        if (!_isSetuped) return;
        //
        if (!_shadowAlphaCache.Equals(_shadowAlpha))
        {
            _shadowAlphaCache = _shadowAlpha;
            updateMaterials();
        }
        if (!_shadowLenCache.Equals(_shadowLen))
        {
            _shadowLenCache = _shadowLen;

            transform.localScale = new Vector3(1, _shadowLenCache*BaseShadowLen, 1);
        }

    }

    private void updateMaterials()
    {

        Color c = new Color(0, 0, 0, _shadowAlpha);

        for (int i = 0; i < _materials.Length; i++)
        {
            _materials[i].SetColor("_Color",c);
        }
    }

}
