using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ParticleSystem))]
public class DestroyParticleOnStop : MonoBehaviour {

    ParticleSystem part;

    bool hasStarted;

	void Awake () {
        part = GetComponent<ParticleSystem>();
	}
	
	void Update () {
        if (part.isPlaying && !hasStarted)
            hasStarted = true;

        if (hasStarted && part.isStopped)
            Destroy(gameObject);
	}
}
