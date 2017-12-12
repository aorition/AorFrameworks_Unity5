using System;
using System.Collections.Generic;
using UnityEngine;
using YoukiaUnity.Misc;

public class ScaleController : MonoBehaviour
{
    
    [SerializeField]
    [SetProperty("Scale")]
    private float _scale = 1.0f;

    private Vector3 _orgScale;

    public float Scale
    {
        get { return _scale; }
        set
        {
            if (Application.isEditor)
            {
                _scale = value;
                _setScale();
            }
            else
            {
                if (value != _scale)
                {
                    _scale = value;
                    _setScale();
                }
            }
        }
    }

    private void _setScale()
    {
        if (!_isStarted || !enabled) return;
        _trans.localScale = new Vector3(_scale, _scale, _scale);
    }

    [SerializeField]
    private Transform _trans;

    public void SetScaleTransform(Transform trans)
    {
        _trans = trans;
        _orgScale = _trans.localScale;
    }

    void OnEnable()
    {
        if (_isStarted)
        {
            _setScale();
        }
    }

    private bool _isStarted = false;
    void Start()
    {
        if (_trans == null)
        {
            SetScaleTransform(transform);
        }

        _isStarted = true;

        _setScale();
    }


    void OnDisable()
    {
        _trans.localScale = _orgScale;
    }

}
