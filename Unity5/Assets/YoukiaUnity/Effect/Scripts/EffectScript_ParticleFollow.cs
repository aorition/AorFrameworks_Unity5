using System;
using UnityEngine;
using Framework.AnimLinkage;

public class EffectScript_ParticleFollow : MonoBehaviour, ISimulateAble
{

    public int ParticleMaxCount = 50;
    private Quaternion q;
    private ParticleSystem _particle;
    private ParticleSystem.Particle[] pList; 
    private void OnEnable()
    {
        _particle = GetComponent<ParticleSystem>();

        if (pList == null || pList.Length != ParticleMaxCount)
        {
            pList = new ParticleSystem.Particle[ParticleMaxCount];
        }
    }

    private void LateUpdate()
    {
        Process(0);
    }

    public void Process(float time)
    {
        if (_particle.isPlaying)
        {

            int pLen = _particle.GetParticles(pList);
            for (int i = 0; i < pLen; i++)
            {
                q = Quaternion.FromToRotation(new Vector3(0, 0, 1), pList[i].velocity.normalized);
                pList[i].rotation3D = q.eulerAngles;
            }
            _particle.SetParticles(pList, pLen);
        }
    }
}
