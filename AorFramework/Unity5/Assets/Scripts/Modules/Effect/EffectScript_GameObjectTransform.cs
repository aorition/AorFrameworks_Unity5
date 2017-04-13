using UnityEngine;
using System;

public class EffectScript_GameObjectTransform : MonoBehaviour
{

    public float Speed = 10;
    public AnimationCurve WeightCurve;
    [Space(10)]
    public Vector3 Position = Vector3.zero;
    public Vector3 EulerAngles = Vector3.zero;
    public Vector3 Scale = Vector3.zero;

    private float startTime;
    //  private float currentTime;
    void Awake()
    {
        startTime = Time.realtimeSinceStartup;

    }

    private Quaternion quat = Quaternion.Euler(1, 1, 1);
    void FixedUpdate()
    {

        float data = WeightCurve.Evaluate(Time.realtimeSinceStartup - startTime);
        data = data*Speed*Time.deltaTime;
        transform.localPosition += Position * data ;
        quat = Quaternion.Euler(EulerAngles * data);
        transform.localRotation *= quat;


        transform.localScale += Scale * data ;
     //   print(data);
    }
}


