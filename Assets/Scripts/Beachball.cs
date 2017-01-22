using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Beachball : MonoBehaviour
{

    Water pool;
    [SerializeField]
    float sampleDistance = 0.2f;
    [SerializeField]
    float forceMultiplier = 1;
    [SerializeField]
    float ballBob = 1;
    [SerializeField]
    ParticleSystem[] particles;
    [SerializeField]
    ParticleSystem splash;

    bool waitOne = true;

    void Awake()
    {
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

    void FixedUpdate()
    {
        Vector2 force = pool.SampleWaterGradient(transform.position, sampleDistance);
        GetComponent<Rigidbody2D>().AddForce(force * forceMultiplier, ForceMode2D.Force);
    }

    public void Score(int player)
    {
        GetComponent<Animator>().SetTrigger("Score");
        particles[player].Play();
    }

    public void Splash()
    {
        Instantiate(splash, transform.position, Quaternion.identity);
        GetComponent<AudioSource>().pitch = Random.Range(1.5f, 1.9f);
        GetComponent<AudioSource>().Play();
    }
}
