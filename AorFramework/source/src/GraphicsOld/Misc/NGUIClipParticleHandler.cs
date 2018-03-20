using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGUIClipParticleHandler : MonoBehaviour {

	void Awake ()
	{
        gameObject.AddComponent(Runtime.GetType("NGUIClipParticle"));
    }

    void FixedUpdate()
    {
        if (gameObject.AddComponent(Runtime.GetType("NGUIClipParticle")))
        {
            DestroyImmediate(this);
        }
    }


}
