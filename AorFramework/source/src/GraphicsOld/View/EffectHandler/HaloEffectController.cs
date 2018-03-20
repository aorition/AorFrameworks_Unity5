using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoukiaUnity.View;

public class HaloEffectController : EffectDescript
{

    public float DelayMoveRate = 0.5f;

    public Vector3 FHeadOffest;

    public bool Advanced = false;
    
    private Transform _ring;
    private Vector3 _ringPosCache;

    private Vector3 _targetPos;
    private Vector3 _dir;

    private void Awake()
    {
        effectPivotType = EffectPivotType.Follow;
        effectDisposeType = EffectDisposeType.Despawn;
        SurvivalTime = 0f;
    }

    private void OnEnable()
    {
        _ring = transform.GetChild(0);
        SimpleCoreFunc();
        _ringPosCache = _targetPos;
    }

    protected void LateUpdate()
    {

        if (Advanced)
        {
            AdvancedCoreFunc();
        }
        else
        {
            SimpleCoreFunc();
        }

    }

    private void SimpleCoreFunc()
    {

		_targetPos = transform.position + new Vector3(FHeadOffest.x * transform.lossyScale.x,FHeadOffest.y * transform.lossyScale.y,FHeadOffest.z * transform.lossyScale.z);
        _ring.position = _targetPos;

        _ring.rotation = Quaternion.identity;
    }

    private void AdvancedCoreFunc()
    {

		_targetPos = transform.position + new Vector3(FHeadOffest.x * transform.lossyScale.x,FHeadOffest.y * transform.lossyScale.y,FHeadOffest.z * transform.lossyScale.z);
        _ringPosCache += (_targetPos - _ringPosCache) * DelayMoveRate;
        _ring.position = _ringPosCache;

        _dir = (_ringPosCache - transform.position).normalized;
        _ring.rotation = Quaternion.FromToRotation(Vector3.down, _dir);

        //        transform.LookAt(FHeadTarget.position);



       // _ring.rotation = Quaternion.identity;

    }

}
