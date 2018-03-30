using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScript_VMaskShaderController : MonoBehaviour, ISimulateAble
{

    public float VMaskValue = 0;

    private Material _material;
    private Renderer _renderer;

    private bool _isStarted = false;
    private float _time;

    private void OnEnable()
    {
        if (!_isStarted)
        {
            init();
        }
    }

    private void init()
    {
        _renderer = gameObject.GetComponent<Renderer>();
        if (_renderer)
        {
            _material = _renderer.material;
        }
        _isStarted = true;
    }

    private void OnDestroy() { 
        _material = null;
        _renderer = null;
    }

    // Update is called once per frame
	void Update ()
	{
	    if (!_material || !_renderer) return;

	    Process(VMaskValue);
        _time += Time.deltaTime;
    }

    public void Process(float time)
    {

        if (!_isStarted)
        {
            init();
        }

        VMaskValue = Mathf.Clamp01(VMaskValue);
        _material.SetFloat("_VEValue", VMaskValue);

    }

}
