using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Beachball : MonoBehaviour {

    Water pool;
    [SerializeField]
    float sampleDistance = 0.2f;
    [SerializeField]
    float forceMultiplier = 1;
    [SerializeField]
    float ballBob = 1;
    [SerializeField]
    ParticleSystem[] particles;

    bool waitOne = true;

    void Awake () {
        pool = FindObjectOfType<Water>();
	}
	
    void Update()
    {
        if (waitOne)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            waitOne = false;
            return;
        }

        float scale = 1 + pool.SampleWater(transform.position) * ballBob;
        transform.localScale = new Vector3(scale, scale, scale);
    }

	void FixedUpdate () {
        Vector2 force = pool.SampleWaterGradient(transform.position, sampleDistance);
        GetComponent<Rigidbody2D>().AddForce(force * forceMultiplier, ForceMode2D.Force);
	}

    public void Score(int player)
    {
        GetComponent<Animator>().SetTrigger("Score");
        particles[player].Play();
        GetComponent<AudioSource>().Play();
    }

    public void PlaySound(AudioClip sound)
    {
        GetComponent<AudioSource>().PlayOneShot(sound);
    }
}
