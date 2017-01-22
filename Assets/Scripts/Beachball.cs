using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Beachball : MonoBehaviour {

    Water pool;
    [SerializeField]
    float sampleDistance = 0.2f;
    [SerializeField]
    float forceMultiplier = 1;

	void Awake () {
        pool = FindObjectOfType<Water>();
	}
	
	void FixedUpdate () {
        Vector2 force = pool.SampleWaterGradient(transform.position, sampleDistance);
        GetComponent<Rigidbody2D>().AddForce(force * forceMultiplier, ForceMode2D.Force);
	}
}
